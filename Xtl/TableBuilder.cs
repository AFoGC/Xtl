using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xtl.Rules;

namespace Xtl
{
    public class TableBuilder<TTable, TRecord> : ITableBuilder<TRecord> where TRecord : Record, new() where TTable : Table<TRecord>
    {
        private readonly TableSaveRules<TTable, TRecord> _saveRules;

        public TableBuilder(TablesCollection tablesCollection)
        {
            EntityBuilder = new EntityBuilder<TRecord>(tablesCollection);
            _saveRules = new TableSaveRules<TTable, TRecord>(EntityBuilder.SaveRules);
        }

        public EntityBuilder<TRecord> EntityBuilder { get; }

        public TRecord? DefaultRecord { get; set; }
        public TTable? DefaultTable
        {
            get => _saveRules.DefaultTable;
            set => _saveRules.DefaultTable = value;
        }

        public void AddTableSaveRule<D>(Expression<Func<TTable, D>> saveAction, D defaultValue)
        {
            _saveRules.AddTableSaveRule(saveAction, defaultValue);
        }

        public void LoadTable(Table<TRecord> table, XmlNode tableNode)
        {
            _saveRules.LoadTable(table, tableNode);
        }

        public void SaveTable(Table<TRecord> table, XmlDocument document)
        {
            _saveRules.SaveTable(table, document);
        }

        public void SetEntityId(Expression<Func<TRecord, int>> idExpression)
        {
            EntityBuilder.SetId(idExpression);
            EntityBuilder.SaveRules.AddSaveRule(idExpression);
        }

        public void AddEntitySaveRule<TValue>(Expression<Func<TRecord, TValue>> saveAction)
        {
            EntityBuilder.SaveRules.AddSaveRule(saveAction);
        }
    }
}
