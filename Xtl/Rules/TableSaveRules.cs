using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Xtl.Rules
{
    internal class TableSaveRules<TTable, TRecord> where TRecord : Record, new() where TTable : Table<TRecord>
    {
        private Func<Table<TRecord>, XmlDocument, XmlNode?> _saveDelegate;
        private Action<Table<TRecord>, XmlNode> _loadDelegate;
        private Action<Table<TRecord>> _setToDefaultDelegate;

        private readonly EntitySaveRules<TRecord> _entitySaveRules;

        public TableSaveRules(EntitySaveRules<TRecord> entitySaveRules)
        {
            _entitySaveRules = entitySaveRules;
        }

        public void AddTableSaveRule<D>(Expression<Func<TTable, D>> saveAction, D defaultValue)
        {
            ArgumentNullException.ThrowIfNull(saveAction, nameof(saveAction));
            PropertyInfo property = Helper.GetPropertyInfo(null, saveAction);

            _saveDelegate += (Table<TRecord> table, XmlDocument document) =>
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

            _loadDelegate += (Table<TRecord> table, XmlNode node) =>
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

            _setToDefaultDelegate += (Table<TRecord> table) =>
            {
                property.SetValue(table, defaultValue);
            };
        }

        public void LoadTable(Table<TRecord> table, XmlNode tableNode)
        {
            table.Clear();

            _setToDefaultDelegate?.Invoke(table);
            _loadDelegate?.Invoke(table, tableNode);

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

                    _entitySaveRules.LoadNode(record, node);
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

            if (_saveDelegate != null)
            {
                var saveFunctions = _saveDelegate.GetInvocationList()
                .Select(x => (Func<Table<TRecord>, XmlDocument, XmlNode?>)x);

                foreach (var saveFunction in saveFunctions)
                {
                    XmlNode node = saveFunction(table, document);

                    if (node != null)
                        tableNode.AppendChild(node);
                }
            }

            XmlNode recordsNode = document.CreateElement("Records");
            tableNode.AppendChild(recordsNode);

            foreach (var record in table)
            {
                _entitySaveRules.SaveNode(recordsNode, record, table.Default);
            }
        }
    }
}
