using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public class EntityBuilder<TRecord> where TRecord : Record, new()
    {
        private readonly TablesCollection _tablesCollection;

        private Action<TRecord> _invokeRelations;

        private Func<TRecord, XmlDocument, TRecord?, XmlNode?> _saveDelegate;
        private Action<TRecord, XmlNode> _loadDelegate;

        private Action<TRecord> _hasOneRelationBindAction;
        private Action<TRecord> _hasOneRelationUnBindAction;

        public EntityBuilder(TablesCollection tablesCollection)
        {
            _tablesCollection = tablesCollection;

            AddSaveRule(x => x.Id);
        }

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

        
        internal void HasOne<TValue>(Expression<Func<TRecord, int>> getIdExpression, Expression<Func<TRecord, TValue>> hasOneExpression) where TValue : Record, new()
        {
            PropertyInfo idProperty = Helper.GetPropertyInfo(null, getIdExpression);
            PropertyInfo hasOneProperty = Helper.GetPropertyInfo(null, hasOneExpression);

            Func<TRecord, int> getIdFunc = getIdExpression.Compile();

            PropertyChangedEventHandler propertyChangedEventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                if (e.PropertyName == idProperty.Name)
                {
                    TRecord record = (TRecord)s;
                    Table<TValue> table = _tablesCollection.GetTableByRecord<TValue>();
                    int id = getIdFunc(record);
                    if (id != 0)
                    {
                        TValue value = table.First(x => x.Id == id);
                        hasOneProperty.SetValue(record, value);
                    }
                    else
                    {
                        hasOneProperty.SetValue(record, null);
                    }
                }
            });

            _invokeRelations += (TRecord record) =>
            {
                Table<TValue> table = _tablesCollection.GetTableByRecord<TValue>();
                int id = getIdFunc(record);

                if (id != 0)
                {
                    TValue value = table.First(x => x.Id == id);
                    hasOneProperty.SetValue(record, value);
                }
                else
                {
                    hasOneProperty.SetValue(record, null);
                }
            };

            _hasOneRelationBindAction += (TRecord record) =>
            {
                record.PropertyChanged += propertyChangedEventHandler;
            };

            _hasOneRelationUnBindAction += (TRecord record) =>
            {
                record.PropertyChanged -= propertyChangedEventHandler;
            };
        }

        internal void HasMany<TValue>(Expression<Func<TValue, int>> getForeignKeyExpression, Expression<Func<TValue, TRecord>> hasOneExpression, Expression<Func<TRecord, RecordsCollection<TValue>>> hasManyExpression) where TValue : Record, new()
        {
            Table<TValue> valuesTable = _tablesCollection.GetTableByRecord<TValue>();
            Table<TRecord> recordsTable = _tablesCollection.GetTableByRecord<TRecord>();

            PropertyInfo hasOneProperty = Helper.GetPropertyInfo(null, hasOneExpression);
            PropertyInfo foreignKeyProperty = Helper.GetPropertyInfo(null, getForeignKeyExpression);

            Func<TValue, TRecord> hasOneFunc = hasOneExpression.Compile();
            Func<TRecord, RecordsCollection<TValue>> hasManyFunc = hasManyExpression.Compile();
            Func<TValue, int> getIdFunc = getForeignKeyExpression.Compile();

            _invokeRelations += (TRecord record) =>
            {
                RecordsCollection<TValue> valuesCollection = hasManyFunc(record);
                valuesCollection.SetHasOneProperty(hasOneProperty, foreignKeyProperty, record.Id);

                var values = valuesTable.Where(x => getIdFunc(x) == record.Id);

                foreach(var item in values)
                    valuesCollection.InternalAdd(item);
            };

            PropertyChangedEventHandler valuesPropertyChanged = new PropertyChangedEventHandler((s, e) =>
            {
                TValue value = (TValue)s;

                if (hasOneProperty.Name == e.PropertyName)
                {
                    TRecord record = hasOneFunc(value);

                    if(record != null)
                    {
                        RecordsCollection<TValue> values = hasManyFunc(record);
                        values.InternalAdd(value);
                    }
                }
            });

            valuesTable.RecordsPropertyChanged += valuesPropertyChanged;
        }

        internal void HasOneExclusive<TValue>(Expression<Func<TRecord, TValue>> hasOneExpression) where TValue : Record, new()
        {
            /*
            Func<TRecord, TValue> hasOneFunc = hasOneExpression.Compile();
            Table<TValue> table = _tablesCollection.GetTableByRecord<TValue>();
            PropertyInfo hasOneProperty = Helper.GetPropertyInfo(null, hasOneExpression);

            _invokeRelations += (TRecord record) =>
            {
                TValue value = table.First(x => x.Id == record.Id);
                hasOneProperty.SetValue(record, value);
            };
            */
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

        internal void AddBinding(TRecord record)
        {
            if (_hasOneRelationBindAction != null)
                _hasOneRelationBindAction.Invoke(record);
        }

        internal void RemoveBinding(TRecord record)
        {
            if (_hasOneRelationUnBindAction != null)
                _hasOneRelationUnBindAction.Invoke(record);
        }

        internal void InvokeBinding(TRecord record)
        {
            if (_invokeRelations != null)
                _invokeRelations.Invoke(record);
        }
    }
}
