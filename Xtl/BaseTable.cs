using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
        public abstract Type RecordType { get; }
    }
}
