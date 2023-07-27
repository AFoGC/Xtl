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
    public class TableBuilder<T> where T : Record
    {
        private readonly List<Func<Table<T>, XmlDocument, XmlNode?>> _saveActions;
        private readonly List<Action<Table<T>, XmlNode>> _loadActions;

        public EntityBuilder<T> EntityBuilder { get; }

        public T? Default { get; set; }

        public TableBuilder()
        {
            EntityBuilder = new EntityBuilder<T>();

            _saveActions = new List<Func<Table<T>, XmlDocument, XmlNode?>>();
            _loadActions = new List<Action<Table<T>, XmlNode>>();
        }

        public void AddSaveRule<D>(Expression<Func<Table<T>, D>> saveAction, D defaultValue)
        {
            ArgumentNullException.ThrowIfNull(saveAction, nameof(saveAction));
            PropertyInfo property = Helper.GetPropertyInfo(null, saveAction);

            Func<Table<T>, XmlDocument, XmlNode?> saveFunction = (Table<T> table, XmlDocument document) =>
            {
                Func<Table<T>, D> func = saveAction.Compile();
                D value = func(table);

                if (Object.Equals(defaultValue, value))
                {
                    XmlNode node = document.CreateNode(XmlNodeType.Element, property.Name, null);
                    node.InnerXml = Helper.ToXmlValue(value);
                    return node;
                }

                return null;
            };

            Action<Table<T>, XmlNode> loadFunction = (Table<T> table, XmlNode node) =>
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

        public void LoadTable(Table<T> table, XmlNode tableNode)
        {
            foreach (var loadAction in _loadActions)
            {
                loadAction(table, tableNode);
            }

            table.Records.Clear();
            XmlNode? recordsNode = tableNode["Records"];
            if (recordsNode != null)
            {
                foreach (XmlNode node in recordsNode.ChildNodes)
                {
                    T record = EntityBuilder.LoadNode(node);
                    table.Records.Add(record);
                }
            }
        }

        public void SaveTable(Table<T> table, XmlNode tableNode)
        {
            XmlDocument document = tableNode.OwnerDocument;

            foreach (var saveFunction in _saveActions)
            {
                XmlNode node = saveFunction(table, document);
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
