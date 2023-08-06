using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TestXmlTables;

Test test = new Test();
Type type = test.GetType();
PropertyInfo property = type.GetProperty(nameof(test.Hui));
property.SetValue(test, "AAA");
property.SetValue(test, "BBB");

Console.WriteLine(test.Count);
Console.WriteLine(test.Hui);
