using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xtl
{
    public class Table<T> : BaseTable where T : Record
    {
        private readonly TablesCollection _tablesCollection;
        private readonly EntityBuilder<T> _entityBuilder;
        private readonly List<T> _records;

        public Table(TablesCollection tablesCollection)
        {
            _records = new List<T>();
            _tablesCollection = tablesCollection;
            _entityBuilder = new EntityBuilder<T>(this);
        }

        internal XmlDocument Document => _tablesCollection.XmlDocument;

        public Type RecordType => typeof(T);
        public T? Default { get; set; }


        public override void SaveTable(XmlDocument document)
        {
            //

            foreach (T record in _records)
            {
                _entityBuilder.SaveNode(, record);
            }
        }
    }
}
