using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Xtl
{
    public class Table<T> : BaseTable, ICollection<T>, INotifyCollectionChanged where T : Record, new()
    {
        private readonly ObservableCollection<T> _records;

        private ITableBuilder<T> _tableBuilder;
        private int _counter;

        public Table()
        {
            _records = new ObservableCollection<T>();
            _records.CollectionChanged += RecordsCollectionChanged;
            _counter = 0;
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? RecordsPropertyChanged;

        private void RecordsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        internal ITableBuilder<T> TableBuilder => _tableBuilder;
        public override Type RecordType => typeof(T);
        public T? Default => _tableBuilder.DefaultRecord;
        public int LastID => _counter;

        public int Count => _records.Count;
        public bool IsReadOnly => false;

        internal void SetBuilder(ITableBuilder<T> builder)
        {
            _tableBuilder = builder;
        }

        public override void SaveTable(XmlDocument document)
        {
            _tableBuilder.SaveTable(this, document);
        }

        public override void LoadTable(XmlNode tableNode)
        {
            Clear();
            _tableBuilder.LoadTable(this, tableNode);
        }

        public T Add()
        {
            T item;
            if (Default != null)
                item = (T)Default.Clone();
            else
                item = new T();

            Add(item);
            return item;
        }

        public void Add(T item)
        {
            item.Id = ++_counter;
            BaseAdd(item);

            _tableBuilder.EntityBuilder.InvokeBinding(item);
        }

        internal void AddLoaded(T item)
        {
            BaseAdd(item);
            _counter = item.Id;
        }

        private void BaseAdd(T item)
        {
            AddBinding(item);
            _records.Add(item);
        }

        public void Clear()
        {
            foreach (var record in _records)
                RemoveBindings(record);

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

        public bool Remove(T item)
        {
            RemoveBindings(item);
            return _records.Remove(item);
        }

        private void AddBinding(T item)
        {
            _tableBuilder.EntityBuilder.AddBinding(item);
        }

        private void RemoveBindings(T item)
        {
            _tableBuilder.EntityBuilder.RemoveBinding(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        internal override void InvokeBindings()
        {
            foreach (var record in _records)
            {
                _tableBuilder.EntityBuilder.InvokeBinding(record);
            }
        }
    }
}
