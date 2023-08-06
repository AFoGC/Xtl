using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xtl
{
    public interface ITableBuilder<TRecord> where TRecord : Record, new()
    {
        public void LoadTable(Table<TRecord> table, XmlNode tableNode);
        public void SaveTable(Table<TRecord> table, XmlDocument document);
        public EntityBuilder<TRecord> EntityBuilder { get; }
        public TRecord? DefaultRecord { get; set; }
    }
}
