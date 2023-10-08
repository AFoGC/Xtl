using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtl.Common;

namespace Xtl.EventCollection
{
    internal class EventCollection<T> : ICollection<T>, IEventCollection
    {
        private readonly List<T> _list;

        public EventCollection()
        {
            _list = new List<T>();
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;


        public event EventCollectionChangedEventHandler? CollectionChanged;

        public void Add(T item)
        {
            _list.Add(item);
            var args = EventCollectionChangedEventArgs.AddArgs(item);
            CollectionChanged.Invoke(this, args);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
