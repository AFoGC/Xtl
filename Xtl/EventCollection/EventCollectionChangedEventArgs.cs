using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtl.Common;

namespace Xtl.EventCollection
{
    public class EventCollectionChangedEventArgs
    {
        public EventCollectionAction Action { get; init; }
        public object? RemovedItem { get; init; }
        public object? AddedItem { get; init; }
        public IEnumerable<object>? ClearedItem { get; init; }

        private EventCollectionChangedEventArgs()
        {

        }

        public static EventCollectionChangedEventArgs AddArgs(object addItem)
        {
            return new EventCollectionChangedEventArgs
            {
                Action = EventCollectionAction.Add,
                AddedItem = addItem
            };
        }

        public static EventCollectionChangedEventArgs RemoveArgs(object removeItem)
        {
            return new EventCollectionChangedEventArgs
            {
                Action = EventCollectionAction.Remove,
                RemovedItem = removeItem
            };
        }

        public static EventCollectionChangedEventArgs ClearArgs(IEnumerable<object> clearItems)
        {
            return new EventCollectionChangedEventArgs
            {
                Action = EventCollectionAction.Clear,
                ClearedItem = clearItems
            };
        }
    }
}
