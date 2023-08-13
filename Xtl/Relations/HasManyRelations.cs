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
    internal class HasManyRelations<TRecord> where TRecord : Record, new()
    {
        private readonly TablesCollection _tablesCollection;

        public HasManyRelations(TablesCollection tablesCollection)
        {
            _tablesCollection = tablesCollection;
        }

        public void HasMany<TValue>(Expression<Func<TValue, int>> idExpression, Expression<Func<TRecord, RecordsCollection<TValue>>> hasManyExpression) where TValue : Record, new()
        {
            Func<TRecord, RecordsCollection<TValue>> getCollectionFunc = hasManyExpression.Compile();
            PropertyInfo idProperty = Helper.GetPropertyInfo(null, idExpression);

            PropertyChangedEventHandler tValuePropertyChangedEventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                Table<TValue> valuesTable = _tablesCollection.GetTableByRecord<TValue>();

                TValue valueRecord = (TValue)s;

                if (e.PropertyName == idProperty.Name)
                {

                }
            });
        }
    }
}
