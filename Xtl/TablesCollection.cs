using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;

namespace Xtl
{
    public class TablesCollection
    {
        private List<BaseTable> _tables;

        public TablesCollection()
        {
            _tables = new List<BaseTable>();
        }

        public void Save(string path)
        {
            XmlDocument document = new XmlDocument();
            XmlNode node = document.CreateNode(XmlNodeType.Element, "Tables", null);
            document.AppendChild(node);

            foreach (var table in _tables)
            {
                table.SaveTable(document);
            }

            using (XmlWriter xmlWriter = XmlWriter.Create(path, new XmlWriterSettings { Indent = true }))
            {
                document.Save(xmlWriter);
            }
        }

        public void Load(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNode mainNode = document.DocumentElement;

            foreach (XmlNode node in mainNode.ChildNodes)
            {
                foreach (BaseTable table in _tables)
                {
                    if (node.Attributes["type"].Value == table.RecordType.Name)
                    {
                        table.LoadTable(node);
                    }
                }
            }
        }

        public void AddTable(BaseTable table)
        {
            _tables.Add(table);
        }
        /*
        public void AddTabl<T, R>(Action<EntityBuilder<R>> buildAction) where T : Table<R>, new() where R : Record
        {

        }
        */
    }
}
