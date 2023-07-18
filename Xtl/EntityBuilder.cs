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
        private readonly List<Func<T, XmlDocument, T?, XmlNode?>> _saveActions = new List<Func<T, XmlDocument, T?, XmlNode?>>();

        public EntityBuilder()
        {
            
        }

        public void AddSaveRule<D>(Expression<Func<T, D>> saveAction)
        {
            Func<T, XmlDocument, T?, XmlNode?> action = (T record, XmlDocument document, T? defRecord) =>
            {
                string name = Helper.GetPropertyInfo(record, saveAction).Name;
                var output = new StringBuilder();

                Func<T, D> func = saveAction.Compile();

                bool isDefaultValue = false;

                if (defRecord != null)
                    if (func.Invoke(record).Equals(func.Invoke(defRecord)))
                        isDefaultValue = true;

                if (isDefaultValue == false)
                {
                    XmlNode node = document.CreateNode(XmlNodeType.Element, name, null);
                    node.InnerXml = Helper.ToXmlValue(func.Invoke(record));
                    return node;
                }

                return null;
            };

            _saveActions.Add(action);
        }

        public void SaveNode(XmlNode tableNode, T record, T? defRecord)
        {
            XmlDocument document = tableNode.OwnerDocument;
            XmlNode recordNode = document.CreateNode(XmlNodeType.Element, typeof(T).Name, null);
            foreach (var function in _saveActions)
            {
                XmlNode? node = function(record, document, defRecord);

                if (node != null)
                    recordNode.AppendChild(node);
            }
            tableNode.AppendChild(recordNode);
        }
            
    }
}
