using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Xtl
{
    public class EntityBuilder<T> where T : Record
    {
        private readonly List<Func<T, XmlDocument, T?, XmlNode?>> _saveActions;
        private readonly XmlSerializer _entitySerializer;

        public EntityBuilder()
        {
            _saveActions = new List<Func<T, XmlDocument, T?, XmlNode?>>();
            _entitySerializer = new XmlSerializer(typeof(T));
        }

        public void AddSaveRule<D>(Expression<Func<T, D>> saveAction)
        {
            Func<T, XmlDocument, T?, XmlNode?> action = (T record, XmlDocument document, T? defRecord) =>
            {
                string name = Helper.GetPropertyInfo(record, saveAction).Name;
                var output = new StringBuilder();

                Func<T, D> func = saveAction.Compile();

                if (defRecord != null)
                    if (Object.Equals(func.Invoke(record), func.Invoke(defRecord)))
                        return null;

                XmlNode node = document.CreateNode(XmlNodeType.Element, name, null);
                node.InnerXml = Helper.ToXmlValue(func.Invoke(record));
                return node;
            };

            _saveActions.Add(action);
        }

        public void SaveNode(XmlNode recordsNode, T record, T? defRecord)
        {
            XmlDocument document = recordsNode.OwnerDocument;
            XmlNode recordNode = document.CreateNode(XmlNodeType.Element, typeof(T).Name, null);
            foreach (var function in _saveActions)
            {
                XmlNode? node = function(record, document, defRecord);

                if (node != null)
                    recordNode.AppendChild(node);
            }
            recordsNode.AppendChild(recordNode);
        }

        public T LoadNode(XmlNode recordNode)
        {
            using (XmlReader reader = new XmlNodeReader(recordNode))
            {
                return (T)_entitySerializer.Deserialize(reader);
            }
        }
    }
}
