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
        private readonly Record _record;

        private PropertyInfo _idPropertyInfo;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public RecordsCollection(Record record)
        {
            _record = record;
            _records = new ObservableCollection<T>();
        }

        public int Count => _records.Count;
        public bool IsReadOnly => false;

        internal void SetProperty(PropertyInfo property)
        {
            _idPropertyInfo = property;
        }

        public void Add(T item)
        {
            item.PropertyChanged += OnItemPropertyChanged;
            _idPropertyInfo.SetValue(item, _record.Id);
        }

        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            /*
            T item = (T)sender;

            if (e.PropertyName == _idPropertyInfo.Name)
            {
                int id = (int)_idPropertyInfo.GetValue(item);
                if(id == _record.Id)
                {
                    _records.Add(item);
                }
                else
                {
                    _records.Remove(item);
                }
            }
            */
        }

        public bool Remove(T item)
        {
            return _records.Remove(item);
        }

        public void Clear()
        {
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
