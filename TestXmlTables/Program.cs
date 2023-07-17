using System.Text;
using System.Xml;
using System.Xml.Serialization;

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