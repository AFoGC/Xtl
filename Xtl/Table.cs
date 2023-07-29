using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Xtl
{
    public class Table<T> : BaseTable where T : Record
    {
        private readonly List<T> _records;

        private ITableBuilder<T> _tableBuilder;

        public Table()
        {
            _records = new List<T>();
        }

        internal ITableBuilder<T> TableBuilder => _tableBuilder;
        public override Type RecordType => typeof(T);
        public ICollection<T> Records => _records;
        public T? Default => _tableBuilder.Default;

        internal void SetBuilder(ITableBuilder<T> builder)
        {
            _tableBuilder = builder;
        }

        public override void SaveTable(XmlDocument document)
        {
            XmlNode tableNode = document.CreateNode(XmlNodeType.Element, "Table", null);
            XmlAttribute attribute = document.CreateAttribute("type");
            attribute.InnerText = RecordType.Name;
            tableNode.Attributes.Append(attribute);
            document.DocumentElement.AppendChild(tableNode);

            _tableBuilder.SaveTable(this, tableNode);
        }

        public override void LoadTable(XmlNode tableNode)
        {
            _records.Clear();
            _tableBuilder.LoadTable(this, tableNode);
        }
    }
}
