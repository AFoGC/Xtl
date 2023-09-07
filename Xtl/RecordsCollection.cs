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

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public RecordsCollection()
        {
            _records = new ObservableCollection<T>();
        }

        public int Count => _records.Count;
        public bool IsReadOnly => false;

        internal void SetForeignIdProperty(PropertyInfo property)
        {
            _collectionProperty = property;
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            T item = (T)sender;

            if (e.PropertyName == _collectionProperty.Name)
            {
                Remove(item);
            }
            

            /*
            if (e.PropertyName == _collectionProperty.Name)
            {
                int keyId = (int)_collectionProperty.GetValue(item);
                if (_parentId != keyId)
                {
                    Remove(item);
                }
            }
            */
        }

        public void Add(T item)
        {
            _records.Add(item);
            item.PropertyChanged += OnItemPropertyChanged;
            
        }

        public bool Remove(T item)
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
