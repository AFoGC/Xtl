using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xtl
{
    public class TablesCollection
    {
        private List<BaseTable> _tables;

        internal readonly XmlDocument XmlDocument;

        public TablesCollection()
        {
            _tables = new List<BaseTable>();
            XmlDocument = new XmlDocument();
        }

        public void Save(string path)
        {
            foreach (var table in _tables)
            {
                table.SaveTable(XmlDocument);
            }
            
        }

        public void Load(string path)
        {

        }

        
        public void AddTable<T, R>(Action<EntityBuilder<R>> buildAction) where T : Table<R> where R : Record
        {

        }
        
    }
}
