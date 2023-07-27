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

        private readonly TableBuilder<T> _tableBuilder;

        public Table()
        {
            _records = new List<T>();
            _tableBuilder = new TableBuilder<T>();
        }

        internal TableBuilder<T> TableBuilder => _tableBuilder;
        public override Type RecordType => typeof(T);
        public ICollection<T> Records => _records;
        public T? Default => _tableBuilder.Default;

        

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
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            _records.Clear();
            XmlNode? recordsNode = tableNode["Records"];

            if (recordsNode != null)
            {
                foreach (XmlNode node in recordsNode.ChildNodes)
                {
                    using (XmlReader reader = XmlReader.Create(new StringReader(node.OuterXml)))
                    {
                        T record = (T)serializer.Deserialize(reader);
                        _records.Add(record);
                    }
                }
            }
        }
    }
}
