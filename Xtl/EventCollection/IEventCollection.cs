using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtl.Common;

namespace Xtl.EventCollection
{
    public interface IEventCollection
    {
        public event EventCollectionChangedEventHandler? CollectionChanged;
    }
}
