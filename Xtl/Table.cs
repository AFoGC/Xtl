﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xtl.Common;
using Xtl.Rules;

namespace Xtl
{
    public class Table<T> : BaseTable, ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : Record, new()
    {
        private readonly ObservableCollection<T> _records;

        private ITableBuilder<T> _tableBuilder;

        public Table()
        {
            _records = new ObservableCollection<T>();
            _records.CollectionChanged += OnRecordsCollectionChanged;
            LastID = 0;
        }

        public override event NotifyCollectionChangedEventHandler? CollectionChanged;
        public override event PropertyChangedEventHandler? RecordsPropertyChanged;
        public override event TableLoadedEventHandler? TableLoaded;
        public override event TableLoadedEventHandler? TableSaved;

        public event PropertyChangedEventHandler? PropertyChanged;

        internal override void InvokeLoaded()
        {
            OnLoaded();
        }

        internal override void InvokeSaved()
        {
            OnSaved();
        }

        protected virtual void OnSaved()
        {
            TableSaved?.Invoke(this);
        }

        protected virtual void OnLoaded()
        {
            TableLoaded?.Invoke(this);
        }

        protected virtual void OnRecordsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(Count));
        }

        protected virtual void OnRecordsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RecordsPropertyChanged?.Invoke(sender, e);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var args = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, args);
        }

        internal ITableBuilder<T> TableBuilder => _tableBuilder;
        internal int LastID { get; private set; }

        public override Type RecordType => typeof(T);
        public T? Default => _tableBuilder.DefaultRecord;
        public int Count => _records.Count;
        public bool IsReadOnly => false;

        internal void SetBuilder(ITableBuilder<T> builder)
        {
            _tableBuilder = builder;
        }

        internal override void SaveTable(XmlDocument document)
        {
            _tableBuilder.SaveTable(this, document);
        }

        internal override void LoadTable(XmlNode tableNode)
        {
            Clear();
            _tableBuilder.LoadTable(this, tableNode);
            T? record = _records.LastOrDefault();

            if (record != null)
            {
                LastID = _tableBuilder.EntityBuilder.IdRule.GetId(record);
            }
            else
            {
                LastID = 0;
            }
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

        public virtual T Add(T item)
        {
            _tableBuilder.EntityBuilder.IdRule.SetNewId(item, LastID);
            LastID = _tableBuilder.EntityBuilder.IdRule.GetId(item);

            AddBinding(item);
            AddInOrder(item);

            _tableBuilder.EntityBuilder.RelationRules.InvokeBinding(item);
            return item;
        }

        private void AddInOrder(T item)
        {
            IdRule<T> idRule = TableBuilder.EntityBuilder.IdRule;

            T? prev = _records.Where(x => idRule.GetId(x) < idRule.GetId(item)).LastOrDefault();

            if (prev != null)
            {
                int index = _records.IndexOf(prev) + 1;
                _records.Insert(index, item);
            }
            else
            {
                _records.Insert(0, item);
            }
        }

        internal void AddLoaded(T item)
        {
            _records.Add(item);
        }

        public virtual void Clear()
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

        public virtual bool Remove(T item)
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
