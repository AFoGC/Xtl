using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Xtl
{
    public class TableBuilder<TTable, TRecord> : ITableBuilder<TRecord> where TRecord : Record, new() where TTable : Table<TRecord>
    {

        private readonly List<Func<Table<TRecord>, XmlDocument, XmlNode?>> _saveActions;
        private readonly List<Action<Table<TRecord>, XmlNode>> _loadActions;
        private readonly List<Action<Table<TRecord>>> _setToDefaultActions;

        public EntityBuilder<TRecord> EntityBuilder { get; }

        public TRecord? DefaultRecord { get; set; }
        public TTable? DefaultTable { get; set; }

        public TableBuilder(TablesCollection tablesCollection)
        {
            EntityBuilder = new EntityBuilder<TRecord>(tablesCollection);

            _saveActions = new List<Func<Table<TRecord>, XmlDocument, XmlNode?>>();
            _loadActions = new List<Action<Table<TRecord>, XmlNode>>();
            _setToDefaultActions = new List<Action<Table<TRecord>>>();
        }

        public void AddSaveRule<D>(Expression<Func<TTable, D>> saveAction, D defaultValue)
        {
            ArgumentNullException.ThrowIfNull(saveAction, nameof(saveAction));
            PropertyInfo property = Helper.GetPropertyInfo(null, saveAction);

            Func<Table<TRecord>, XmlDocument, XmlNode?> saveFunction = (Table<TRecord> table, XmlDocument document) =>
            {
                Func<TTable, D> func = saveAction.Compile();
                D value = func((TTable)table);

                if (Object.Equals(defaultValue, value) == false)
                {
                    XmlNode node = document.CreateNode(XmlNodeType.Element, property.Name, null);
                    node.InnerXml = Helper.ToXmlValue(value);
                    return node;
                }

                return null;
            };

            Action<Table<TRecord>, XmlNode> loadFunction = (Table<TRecord> table, XmlNode node) =>
            {
                XmlElement? element = node[property.Name];
                
                if (element != null)
                {
                    XmlRootAttribute rootAttribute = new XmlRootAttribute(element.Name);
                    XmlSerializer xmlSerializer = new XmlSerializer(property.PropertyType, rootAttribute);

                    using (XmlReader reader = new XmlNodeReader(element))
                    {
                        property.SetValue(table, xmlSerializer.Deserialize(reader));
                    }
                }
            };

            Action<Table<TRecord>> setToDefaultFunction = (Table<TRecord> table) =>
            {
                object value = property.GetValue(DefaultTable);
                property.SetValue(table, value);
            };

            _saveActions.Add(saveFunction);
            _loadActions.Add(loadFunction);
            _setToDefaultActions.Add(setToDefaultFunction);
        }

        public void LoadTable(Table<TRecord> table, XmlNode tableNode)
        {
            table.Clear();

            foreach (var setToDefault in _setToDefaultActions)
                setToDefault(table);

            foreach (var loadAction in _loadActions)
                loadAction(table, tableNode);
            
            XmlNode? recordsNode = tableNode["Records"];
            if (recordsNode != null)
            {
                foreach (XmlNode node in recordsNode.ChildNodes)
                {
                    TRecord record;
                    if (table.Default != null)
                    {
                        record = (TRecord)table.Default.Clone();
                    }
                    else
                    {
                        record = new TRecord();
                    }
                    
                    EntityBuilder.LoadNode(record, node);
                    table.AddLoaded(record);
                }
            }
        }

        public void SaveTable(Table<TRecord> table, XmlDocument document)
        {
            XmlNode tableNode = document.CreateNode(XmlNodeType.Element, "Table", null);
            XmlAttribute attribute = document.CreateAttribute("type");
            attribute.InnerText = table.RecordType.Name;
            tableNode.Attributes.Append(attribute);
            document.DocumentElement.AppendChild(tableNode);

            foreach (var saveFunction in _saveActions)
            {
                XmlNode node = saveFunction(table, document);

                if(node != null)
                    tableNode.AppendChild(node);
            }

            XmlNode recordsNode = document.CreateElement("Records");
            tableNode.AppendChild(recordsNode);

            foreach (var record in table)
            {
                EntityBuilder.SaveNode(recordsNode, record, table.Default);
            }
        }
    }
}
