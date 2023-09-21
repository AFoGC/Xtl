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
    internal class EntitySaveRules<TRecord> where TRecord : Record, new()
    {
        private Func<TRecord, XmlDocument, TRecord?, XmlNode?> _saveDelegate;
        private Action<TRecord, XmlNode> _loadDelegate;

        public void AddSaveRule<TValue>(Expression<Func<TRecord, TValue>> saveAction)
        {
            _saveDelegate += (TRecord record, XmlDocument document, TRecord? defRecord) =>
            {
                string name = Helper.GetPropertyInfo(record, saveAction).Name;
                var output = new StringBuilder();

                Func<TRecord, TValue> func = saveAction.Compile();

                if (defRecord != null)
                    if (Object.Equals(func.Invoke(record), func.Invoke(defRecord)))
                        return null;

                XmlNode node = document.CreateNode(XmlNodeType.Element, name, null);
                node.InnerXml = Helper.ToXmlValue(func.Invoke(record));
                return node;
            };

            _loadDelegate += (TRecord record, XmlNode recordNode) =>
            {
                PropertyInfo property = Helper.GetPropertyInfo(record, saveAction);
                XmlRootAttribute xmlRoot = new XmlRootAttribute(property.Name);
                XmlSerializer serializer = new XmlSerializer(property.PropertyType, xmlRoot);

                XmlNode? fieldNode = recordNode[property.Name];
                if (fieldNode != null)
                {
                    object value = Helper.FromXml(property.Name, property.PropertyType, fieldNode);
                    property.SetValue(record, value);
                }
            };
        }

        internal void SaveNode(XmlNode recordsNode, TRecord record, TRecord? defRecord)
        {
            XmlDocument document = recordsNode.OwnerDocument;
            XmlNode recordNode = document.CreateNode(XmlNodeType.Element, typeof(TRecord).Name, null);

            var nodes = _saveDelegate.GetInvocationList()
                .Select(x => (XmlNode?)x.DynamicInvoke(record, document, defRecord));

            foreach (var node in nodes)
                if (node != null)
                    recordNode.AppendChild(node);

            recordsNode.AppendChild(recordNode);
        }

        internal void LoadNode(TRecord record, XmlNode recordNode)
        {
            if (_loadDelegate != null)
                _loadDelegate.Invoke(record, recordNode);
        }
    }
}
