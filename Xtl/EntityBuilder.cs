using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Xtl
{
    public class EntityBuilder<TRecord> where TRecord : Record
    {
        private readonly TablesCollection _tablesCollection;
        private readonly List<Func<TRecord, XmlDocument, TRecord?, XmlNode?>> _saveActions;
        private readonly List<Action<TRecord, XmlNode>> _loadActions;
        private readonly List<Action<TRecord>> _bindActions;
        private readonly List<Action<TRecord>> _unbindActions;
        private readonly List<Action<TRecord>> _invokeBindingActions;

        public EntityBuilder(TablesCollection tablesCollection)
        {
            _tablesCollection = tablesCollection;
            _saveActions = new List<Func<TRecord, XmlDocument, TRecord?, XmlNode?>>();
            _loadActions = new List<Action<TRecord, XmlNode>>();
            _bindActions = new List<Action<TRecord>>();
            _unbindActions = new List<Action<TRecord>>();
            _invokeBindingActions = new List<Action<TRecord>>();

            AddSaveRule(x => x.Id);
        }

        public void AddSaveRule<TValue>(Expression<Func<TRecord, TValue>> saveAction)
        {
            Func<TRecord, XmlDocument, TRecord?, XmlNode?> saveFunction = (TRecord record, XmlDocument document, TRecord? defRecord) =>
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

            Action<TRecord, XmlNode> loadFunction = (TRecord record, XmlNode recordNode) =>
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

            _saveActions.Add(saveFunction);
            _loadActions.Add(loadFunction);
        }

        
        public void AddBinding<TValue>(Expression<Func<TRecord, int>> getIdExpression, Expression<Func<TRecord, TValue>> bindExpression) where TValue : Record, new()
        {
            PropertyInfo idProperty = Helper.GetPropertyInfo(null, getIdExpression);
            PropertyInfo bindProperty = Helper.GetPropertyInfo(null, bindExpression);

            Func<TRecord, int> func = getIdExpression.Compile();

            PropertyChangedEventHandler propertyChangedEventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                if (e.PropertyName == idProperty.Name)
                {
                    TRecord record = (TRecord)s;
                    Table<TValue> table = _tablesCollection.GetTableByRecord<TValue>();
                    int id = func(record);
                    if (id != 0)
                    {
                        TValue value = table.First(x => x.Id == id);
                        bindProperty.SetValue(record, value);
                    }
                }
            });

            Action<TRecord> invokeBinding = (TRecord record) =>
            {
                Table<TValue> table = _tablesCollection.GetTableByRecord<TValue>();
                int id = func(record);

                if (id != 0)
                {
                    TValue value = table.First(x => x.Id == id);
                    bindProperty.SetValue(record, value);
                }
            };

            Action<TRecord> bindingAction = (TRecord record) =>
            {
                record.PropertyChanged += propertyChangedEventHandler;
            };

            Action<TRecord> unbindingAction = (TRecord record) =>
            {
                record.PropertyChanged -= propertyChangedEventHandler;
            };

            _bindActions.Add(bindingAction);
            _unbindActions.Add(unbindingAction);
            _invokeBindingActions.Add(invokeBinding);
        }

        internal void SaveNode(XmlNode recordsNode, TRecord record, TRecord? defRecord)
        {
            XmlDocument document = recordsNode.OwnerDocument;
            XmlNode recordNode = document.CreateNode(XmlNodeType.Element, typeof(TRecord).Name, null);
            foreach (var function in _saveActions)
            {
                XmlNode? node = function(record, document, defRecord);

                if (node != null)
                    recordNode.AppendChild(node);
            }
            recordsNode.AppendChild(recordNode);
        }

        internal void LoadNode(TRecord record, XmlNode recordNode)
        {
            foreach (var loadaction in _loadActions)
                loadaction(record, recordNode);
        }

        internal void AddBinding(TRecord record)
        {
            foreach (var action in _bindActions)
                action(record);
        }

        internal void RemoveBinding(TRecord record)
        {
            foreach (var action in _unbindActions)
                action(record);
        }

        internal void InvokeBinding(TRecord record)
        {
            foreach(var action in _invokeBindingActions)
                action(record);
        }
    }
}
