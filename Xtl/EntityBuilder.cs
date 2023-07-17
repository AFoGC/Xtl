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
        private readonly List<Func<T, XmlNode?>> _saveActions = new List<Func<T, XmlNode?>>();
        //private readonly XmlDocument _document;

        private readonly Table<T> _parentTable;

        public EntityBuilder(Table<T> paretTable)
        {
            _parentTable = paretTable;
        }

        public void AddSaveRule<D>(Expression<Func<T, D>> saveAction)
        {
            Func<T, XmlNode?> action = (T e) =>
            {
                string name = Helper.GetPropertyInfo(e, saveAction).Name;
                var output = new StringBuilder();

                Func<T, D> func = saveAction.Compile();

                if (_parentTable.Default != null)
                {
                    if (func.Invoke(e).Equals(func.Invoke(_parentTable.Default)) == false)
                    {
                        XmlNode node = _parentTable.Document.CreateNode(XmlNodeType.Element, name, null);
                        node.InnerXml = Helper.ToXmlValue(func.Invoke(e));
                        return node;
                    }
                }

                return null;
            };

            _saveActions.Add(action);
        }

        public void SaveNode(XmlNode tableNode, T record)
        {
            XmlNode recordNode = _parentTable.Document.CreateNode(XmlNodeType.Element, typeof(T).Name, null);
            foreach (var function in _saveActions)
            {
                XmlNode? node = function(record);

                if (node != null)
                    recordNode.AppendChild(node);
            }
            tableNode.AppendChild(recordNode);
        }
            
    }
}
