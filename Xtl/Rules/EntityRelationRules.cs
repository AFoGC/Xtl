using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.Rules
{
    internal class EntityRelationRules<TRecord> where TRecord : Record, new()
    {
        private Action<TRecord> _invokeRelations;

        private Action<TRecord> _hasOneRelationBindAction;
        private Action<TRecord> _hasOneRelationUnBindAction;

        private Action<TRecord> _hasOneExclusiveBindAction;
        private Action<TRecord> _hasOneExclusiveUnBindAction;

        private readonly TablesCollection _tablesCollection;
        private readonly IdRule<TRecord> _idRule;

        public EntityRelationRules(TablesCollection tablesCollection, IdRule<TRecord> idRule)
        {
            _tablesCollection = tablesCollection;
            _idRule = idRule;
        }


        internal void HasOne<TValue>(Expression<Func<TRecord, int>> getIdExpression, Expression<Func<TRecord, TValue>> hasOneExpression) where TValue : Record, new()
        {
            PropertyInfo idProperty = Helper.GetPropertyInfo(null, getIdExpression);
            PropertyInfo hasOneProperty = Helper.GetPropertyInfo(null, hasOneExpression);

            Func<TRecord, int> getForeignKeyIdFunc = getIdExpression.Compile();

            PropertyChangedEventHandler propertyChangedEventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                if (e.PropertyName == idProperty.Name)
                {
                    TRecord record = (TRecord)s;
                    Table<TValue> table = _tablesCollection.GetTableByRecord<TValue>();
                    IdRule<TValue> valueIdRule = table.TableBuilder.EntityBuilder.IdRule;

                    int recordId = getForeignKeyIdFunc(record);

                    if (recordId != 0)
                    {
                        TValue value = table.First(x => valueIdRule.GetId(x) == recordId);
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
                IdRule<TValue> valueIdRule = table.TableBuilder.EntityBuilder.IdRule;

                int id = getForeignKeyIdFunc(record);

                if (id != 0)
                {
                    TValue value = table.First(x => valueIdRule.GetId(x) == id);
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
                valuesCollection.SetHasOneProperty(hasOneProperty, foreignKeyProperty, _idRule.GetId(record));

                var values = valuesTable.Where(x => getIdFunc(x) == _idRule.GetId(record));

                foreach (var item in values)
                    valuesCollection.InternalAdd(item);
            };

            PropertyChangedEventHandler valuesPropertyChanged = new PropertyChangedEventHandler((s, e) =>
            {
                TValue value = (TValue)s;

                if (hasOneProperty.Name == e.PropertyName)
                {
                    TRecord record = hasOneFunc(value);

                    if (record != null)
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
            Func<TRecord, TValue> hasOneFunc = hasOneExpression.Compile();
            Table<TValue> table = _tablesCollection.GetTableByRecord<TValue>();
            IdRule<TValue> valueIdRule = table.TableBuilder.EntityBuilder.IdRule;

            PropertyInfo hasOneProperty = Helper.GetPropertyInfo(null, hasOneExpression);

            _invokeRelations += (TRecord record) =>
            {
                TValue value = table.First(x => valueIdRule.GetId(x) == _idRule.GetId(record));
                hasOneProperty.SetValue(record, value);
            };
        }

        internal void HasOneSub<TValue>(Expression<Func<TRecord, TValue>> hasOneExpression, Expression<Func<TValue, TRecord>> hasFkPkExpression) where TValue : Record, new()
        {
            Func<TRecord, TValue> hasOneFunc = hasOneExpression.Compile();
            Func<TValue, TRecord> hasFkPkFunc = hasFkPkExpression.Compile();

            Table<TValue> valuesTable = _tablesCollection.GetTableByRecord<TValue>();
            Table<TRecord> recordsTable = _tablesCollection.GetTableByRecord<TRecord>();

            IdRule<TValue> valueIdRule = valuesTable.TableBuilder.EntityBuilder.IdRule;

            PropertyInfo hasOneProperty = Helper.GetPropertyInfo(null, hasOneExpression);

            valuesTable.CollectionChanged += new NotifyCollectionChangedEventHandler((s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    TValue value = (TValue)e.OldItems[0];
                    TRecord record = hasFkPkFunc(value);

                    hasOneProperty.SetValue(record, null);
                }

                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    foreach (TRecord item in recordsTable)
                    {
                        TValue? value = valuesTable.FirstOrDefault(x => valueIdRule.GetId(x) == _idRule.GetId(item));
                        hasOneProperty.SetValue(item, value);
                    }
                }
            });

            _invokeRelations += (TRecord record) =>
            {
                TValue? value = valuesTable.FirstOrDefault(x => valueIdRule.GetId(x) == _idRule.GetId(record));
                hasOneProperty.SetValue(record, value);
            };
        }

        internal void AddBinding(TRecord record)
        {
            _hasOneRelationBindAction?.Invoke(record);
        }

        internal void RemoveBinding(TRecord record)
        {
            _hasOneRelationUnBindAction?.Invoke(record);
        }

        internal void InvokeBinding(TRecord record)
        {
            _invokeRelations?.Invoke(record);
        }
    }
}
