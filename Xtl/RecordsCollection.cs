using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xtl
{
    public class RecordsCollection<T> : ICollection<T>, INotifyCollectionChanged where T : Record, new()
    {
        private readonly ObservableCollection<T> _records;

        private PropertyInfo _collectionProperty;
        private PropertyInfo _foreignKeyProperty;
        private int _parentId;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public RecordsCollection()
        {
            _records = new ObservableCollection<T>();
        }

        public int Count => _records.Count;
        public bool IsReadOnly => false;

        internal void SetHasOneProperty(PropertyInfo hasOneProperty, PropertyInfo foreignKeyProperty, int parentId)
        {
            _collectionProperty = hasOneProperty;
            _foreignKeyProperty = foreignKeyProperty;
            _parentId = parentId;
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            T item = (T)sender;

            if (e.PropertyName == _collectionProperty.Name)
            {
                InternalRemove(item);
            }
        }

        public void Add(T item)
        {
            _foreignKeyProperty.SetValue(item, _parentId);
        }

        public bool Remove(T item)
        {
            bool isContains = _records.Contains(item);

            if (isContains)
            {
                _foreignKeyProperty.SetValue(item, 0);
            }

            return isContains;

            /*
            bool isRemoved = InternalRemove(item);

            if (isRemoved)
            {
                _foreignKeyProperty.SetValue(item, 0);
            }

            return isRemoved;
            */
        }

        internal void InternalAdd(T item)
        {
            _records.Add(item);
            item.PropertyChanged += OnItemPropertyChanged;
        }

        internal bool InternalRemove(T item)
        {
            if (_records.Remove(item))
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            foreach (T item in _records)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                _foreignKeyProperty.SetValue(item, 0);
            }

            _records.Clear();
        }

        public bool Contains(T item)
        {
            return _records.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _records.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _records.GetEnumerator();
        }
    }
}
