﻿using System;
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
            _records.CollectionChanged += OnRecordsCollectionChanged;
            _counter = 0;
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? RecordsPropertyChanged;

        private void OnRecordsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnRecordsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RecordsPropertyChanged?.Invoke(sender, e);
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

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public T Add()
        {
            T item;
            if (Default != null)
                item = (T)Default.Clone();
            else
                item = new T();

            return Add(item);
        }

        public T Add(T item)
        {
            int id = ++_counter;
            _tableBuilder.EntityBuilder.IdRule.SetId(item, id);

            BaseAdd(item);

            _tableBuilder.EntityBuilder.RelationRules.InvokeBinding(item);
            return item;
        }

        internal void AddLoaded(T item)
        {
            _records.Add(item);
            _counter = _tableBuilder.EntityBuilder.IdRule.GetId(item);
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

        internal override void AddAllBindings()
        {
            foreach (var item in this)
                AddBinding(item);
        }

        private void AddBinding(T item)
        {
            item.PropertyChanged += OnRecordsPropertyChanged;
            _tableBuilder.EntityBuilder.RelationRules.AddBinding(item);
        }

        private void RemoveBindings(T item)
        {
            item.PropertyChanged -= OnRecordsPropertyChanged;
            _tableBuilder.EntityBuilder.RelationRules.RemoveBinding(item);
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
                _tableBuilder.EntityBuilder.RelationRules.InvokeBinding(record);
            }
        }
    }
}
