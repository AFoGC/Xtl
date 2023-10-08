using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtl.EventCollection;

namespace Xtl.Common
{
    public delegate void TableLoadedEventHandler(object sender);
    public delegate void TablesCollectionLoadEventHandler(TablesCollection sender);

    public delegate void EventCollectionChangedEventHandler(object sender, EventCollectionChangedEventArgs e);
}
