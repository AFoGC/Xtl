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
    public class TableBuilder<TTable, TRecord> : ITableBuilder<TRecord> where TRecord : Record where TTable : Table<TRecord>
    {
        private readonly List<Func<Table<TRecord>, XmlDocument, XmlNode?>> _saveActions;
        private readonly List<Action<Table<TRecord>, XmlNode>> _loadActions;

        public EntityBuilder<TRecord> EntityBuilder { get; }

        public TRecord? Default { get; set; }

        public TableBuilder()
        {
            EntityBuilder = new EntityBuilder<TRecord>();

            _saveActions = new List<Func<Table<TRecord>, XmlDocument, XmlNode?>>();
            _loadActions = new List<Action<Table<TRecord>, XmlNode>>();
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

            _saveActions.Add(saveFunction);
            _loadActions.Add(loadFunction);
        }

        public void LoadTable(Table<TRecord> table, XmlNode tableNode)
        {
            table.Records.Clear();

            foreach (var loadAction in _loadActions)
            {
                loadAction(table, tableNode);
            }
            
            XmlNode? recordsNode = tableNode["Records"];
            if (recordsNode != null)
            {
                foreach (XmlNode node in recordsNode.ChildNodes)
                {
                    TRecord record = EntityBuilder.LoadNode(node);
                    table.Records.Add(record);
                }
            }
        }

        public void SaveTable(Table<TRecord> table, XmlNode tableNode)
        {
            XmlDocument document = tableNode.OwnerDocument;

            foreach (var saveFunction in _saveActions)
            {
                XmlNode node = saveFunction(table, document);

                if(node != null)
                    tableNode.AppendChild(node);
            }

            XmlNode recordsNode = document.CreateElement("Records");
            tableNode.AppendChild(recordsNode);

            foreach (var record in table.Records)
            {
                EntityBuilder.SaveNode(recordsNode, record, table.Default);
            }
        }
    }
}
