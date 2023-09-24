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
using Xtl.Rules;

namespace Xtl
{
    public class EntityBuilder<TRecord> where TRecord : Record, new()
    {
        private readonly TablesCollection _tablesCollection;

        public EntityBuilder(TablesCollection tablesCollection)
        {
            _tablesCollection = tablesCollection;

            IdRule = new IdRule<TRecord>();
            SaveRules = new EntitySaveRules<TRecord>();
            RelationRules = new EntityRelationRules<TRecord>(tablesCollection, IdRule);
        }

        internal IdRule<TRecord> IdRule { get; }
        internal EntitySaveRules<TRecord> SaveRules { get; }
        internal EntityRelationRules<TRecord> RelationRules { get; }

        public void SetId(Expression<Func<TRecord, int>> idExpression)
        {
            IdRule.SetIdExpression(idExpression);
        }
        

    }
}
