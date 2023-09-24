using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Xtl
{
    public class TablesCollectionBuilder
    {
        private readonly List<BaseTable> _tables;
        private readonly TablesCollection _tablesCollection;

        public TablesCollectionBuilder(TablesCollection tablesCollection)
        {
            _tables = new List<BaseTable>();
            _tablesCollection = tablesCollection;
        }

        public void AddTable<TTable, TRecord>(Action<TableBuilder<TTable, TRecord>> buildAction) where TTable : Table<TRecord>, new() where TRecord : Record, new()
        {
            _tablesCollection.AddTable<TTable, TRecord>(buildAction);
            /*
            TTable table = new TTable();
            TableBuilder<TTable, TRecord> builder = new TableBuilder<TTable, TRecord>(_tablesCollection);
            buildAction(builder);
            table.SetBuilder(builder);
            _tables.Add(table);
            */
        }

        public void AddOneToMany<TMany, TOne>(Expression<Func<TOne, int>> getIdExpression, Expression<Func<TOne, TMany>> hasOne, Expression<Func<TMany, RecordsCollection<TOne>>> hasMany) where TMany : Record, new() where TOne : Record, new()
        {
            Table<TOne> ones = _tablesCollection.GetTableByRecord<TOne>();
            ones.TableBuilder.EntityBuilder.RelationRules.HasOne(getIdExpression, hasOne);

            Table<TMany> manies = _tablesCollection.GetTableByRecord<TMany>();
            manies.TableBuilder.EntityBuilder.RelationRules.HasMany(getIdExpression, hasOne, hasMany);
        }

        public void AddOneToOne<TMain, TSub>(Expression<Func<TMain, TSub>> hasOne)
        {

        }
    }
}
