using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.Rules
{
    internal class IdRule<TRecord> where TRecord : Record, new()
    {
        private readonly Func<TRecord, int> _getIdFunction;
        private readonly PropertyInfo _idProperty;

        public IdRule(Expression<Func<TRecord, int>> idExpression)
        {
            _getIdFunction = idExpression.Compile();
            _idProperty = Helper.GetPropertyInfo(null, idExpression);
        }

        internal int GetId(TRecord record)
        {
            return _getIdFunction(record);
        }

        internal void SetId(TRecord record, int id)
        {
            _idProperty.SetValue(record, id);
        }

        internal void AddIdChangeTracking(TRecord record)
        {
            record.PropertyChanged += OnRecordPropertyChanged;
        }

        internal void RemoveIdChangeTracking(TRecord record)
        {
            record.PropertyChanged -= OnRecordPropertyChanged;
        }

        private void OnRecordPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _idProperty.Name)
            {
                throw new Exception("Primary Key Id Changed");
            }
        }
    }
}
