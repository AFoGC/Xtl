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
        //private readonly TablesCollection _tablesCollection;
        private readonly EntityBuilder<T> _entityBuilder;
        private readonly List<T> _records;

        public Table(EntityBuilder<T> builder)
        {
            _records = new List<T>();
            //_tablesCollection = tablesCollection;
            //_entityBuilder = new EntityBuilder<T>();
            _entityBuilder = builder;
        }

        //internal XmlDocument Document => _tablesCollection.XmlDocument;
        public override Type RecordType => typeof(T);
        public ICollection<T> Records => _records;
        public T? Default { get; set; }


        public override void SaveTable(XmlDocument document)
        {
            XmlNode node = document.CreateNode(XmlNodeType.Element, "Table", null);
            XmlAttribute attribute = document.CreateAttribute("type");
            attribute.InnerText = RecordType.Name;
            node.Attributes.Append(attribute);

            XmlNode recordsList = document.CreateNode(XmlNodeType.Element, "Records", null);
            document.DocumentElement.AppendChild(node);
            node.AppendChild(recordsList);

            foreach (T record in _records)
            {
                _entityBuilder.SaveNode(recordsList, record, Default);
            }
        }

        public override void LoadTable(XmlNode tableNode)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            _records.Clear();
            foreach (XmlNode node in tableNode["Records"].ChildNodes)
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
