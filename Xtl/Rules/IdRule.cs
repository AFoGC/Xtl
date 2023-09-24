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
        private Func<TRecord, int> _getIdFunction;
        private PropertyInfo _idProperty;
        private bool _hasIdProperty;

        private readonly IdGenerationRule _idGenerationRule;

        public IdRule()
        {
            _idGenerationRule = new IdGenerationRule();
        }

        public bool HasIdProperty => _hasIdProperty;

        internal void SetIdExpression(Expression<Func<TRecord, int>> idExpression)
        {
            if (_hasIdProperty == false)
            {
                _getIdFunction = idExpression.Compile();
                _idProperty = Helper.GetPropertyInfo(null, idExpression);
                _hasIdProperty = true;
            }
            else
            {
                throw new Exception("Id already setted"); ;
            }
        }

        internal void SetIdGenerationExpression(Func<int, int?> idGenerationExpression)
        {
            _idGenerationRule.SetIdGenerationExpression(idGenerationExpression);
        }

        internal int GetId(TRecord record)
        {
            return _getIdFunction(record);
        }

        internal void SetNewId(TRecord record, int counter)
        {
            int? newId = _idGenerationRule.GenerateId(counter);
            if (newId != null)
            {
                _idProperty.SetValue(record, newId);
            }
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
