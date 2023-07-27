using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
/*
XmlDocument document = new XmlDocument();

XmlNodeType nodeType = XmlNodeType.Element;
XmlNode xmlNode = document.CreateNode(nodeType, "Number", "");

//var value = new Hiu { ID = 22, Name = "AAA" };
var value = 44;
XmlSerializer serializer = new XmlSerializer(typeof(int));
XmlWriterSettings settings = new XmlWriterSettings() {OmitXmlDeclaration = true };

var output = new StringBuilder();
using (var stringwriter = XmlWriter.Create(output, settings))
{
    serializer.Serialize(stringwriter, value);
    XmlDocument outdoc = new XmlDocument();
    outdoc.LoadXml(output.ToString());
    xmlNode.InnerXml = outdoc.DocumentElement.InnerXml;
    //Console.WriteLine(outdoc.DocumentElement.InnerXml);
}

document.AppendChild(xmlNode);
//Console.WriteLine(document.OuterXml);
document.Save(Console.Out);

public class Hiu{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
}
*/

string xml = "<Hui><ID>24</ID></Hui>";
XmlDocument document = new XmlDocument();
document.LoadXml(xml);

//document.CreateElement();

XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("ID");
XmlSerializer serializer = new XmlSerializer(typeof(int), xmlRootAttribute);

//XDocument xDocument = XDocument.Parse(xml);
//xDocument.CreateReader();

Console.WriteLine();


using (XmlReader reader = new XmlNodeReader(document.DocumentElement.FirstChild))
{
    Hui hui = new Hui();
    hui.id = (int)serializer.Deserialize(reader);
    Console.WriteLine(hui.id);
}

class Hui
{
    public int id = 0;
}


//Console.WriteLine(Convert.ToDateTime("1999-01-02T00:00:00"));