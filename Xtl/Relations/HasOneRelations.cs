using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.Relations
{
    internal class HasOneRelations<TRecord> where TRecord : Record, new()
    {
        private readonly TablesCollection _tablesCollection;

        private readonly List<Action<TRecord>> _addRelationActions;
        private readonly List<Action<TRecord>> _removeRelationActions;
        private readonly List<Action<TRecord>> _invokeRelationActions;

        public HasOneRelations(TablesCollection tablesCollection)
        {
            _tablesCollection = tablesCollection;

            _addRelationActions = new List<Action<TRecord>>();
            _removeRelationActions = new List<Action<TRecord>>();
            _invokeRelationActions = new List<Action<TRecord>>();
        }

        public void HasOne<TValue>(Expression<Func<TRecord, int>> getIdExpression, Expression<Func<TRecord, TValue>> bindExpression) where TValue : Record, new()
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
                    else
                    {
                        bindProperty.SetValue(record, null);
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
                else
                {
                    bindProperty.SetValue(record, null);
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

            _addRelationActions.Add(bindingAction);
            _removeRelationActions.Add(unbindingAction);
            _invokeRelationActions.Add(invokeBinding);
        }
    }
}
