using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xtl.Common;

namespace Xtl
{
    public abstract class BaseTable
    {
        internal abstract void InvokeBindings();
        internal abstract void AddAllBindings();
        internal abstract void InvokeLoaded();
        internal abstract void InvokeSaved();
        internal abstract void SaveTable(XmlDocument document);
        internal abstract void LoadTable(XmlNode tableNode);

        public abstract event NotifyCollectionChangedEventHandler? CollectionChanged;
        public abstract event PropertyChangedEventHandler? RecordsPropertyChanged;
        public abstract event TableLoadedEventHandler? TableLoaded;
        public abstract event TableLoadedEventHandler? TableSaved;

        public abstract Type RecordType { get; }
    }
}
