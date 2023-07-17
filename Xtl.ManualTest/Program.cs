// See https://aka.ms/new-console-template for more information
using System.Xml;
using System.Xml.Serialization;
using Xtl;
using Xtl.ManualTest.Entities;
/*
TablesCollection tablesCollection = new TablesCollection();
tablesCollection.AddTable<FilmsTable, Film>(entity =>
{
    
});
*/
Film film1 = new Film { Id = 1, GenreId = 1, Name = "Hui1", RealiseDate = 2001 };
Film film2 = new Film { Id = 2, GenreId = 2, Name = "Hui2", RealiseDate = 2022 };
Film film3 = new Film { Id = 3, GenreId = 2, Name = "Hui3", RealiseDate = 2013 };
Film film4 = new Film { Id = 4, GenreId = 1, Name = "Hui4", RealiseDate = 2015, WatchDate = new DateTime(1999, 1, 2) };

EntityBuilder<Film> builder = new EntityBuilder<Film>();
XmlDocument document = new XmlDocument();
builder.Document = document;

XmlNode tableNode = document.CreateNode(XmlNodeType.Element, "ArrayOfFilm", null);
document.AppendChild(tableNode);

builder.Default = new Film();
builder.AddSaveRule(x => x.Id);
builder.AddSaveRule(x => x.GenreId);
builder.AddSaveRule(x => x.Name);
builder.AddSaveRule(x => x.RealiseDate);
builder.AddSaveRule(x => x.WatchDate);

builder.SaveNode(tableNode, film1);
builder.SaveNode(tableNode, film2);
builder.SaveNode(tableNode, film3);
builder.SaveNode(tableNode, film4);

/*
using (XmlWriter xmlWriter = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
{
    //document.Save(xmlWriter);
}

XmlSerializer serializer = new XmlSerializer(typeof(List<Film>));



using (XmlWriter xmlWriter = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
{
    //ArrayOfFilm
    //List<Film> films = new List<Film> { film1, film2, film3, film4 };
    //serializer.Serialize(xmlWriter, films);
}

using (XmlWriter writer = XmlWriter.Create("Out1.xml", new XmlWriterSettings { Indent = true }))
{
    document.Save(writer);
}

using (XmlReader reader = XmlReader.Create("Out1.xml"))
{
    List<Film> films = (List<Film>)serializer.Deserialize(reader);

    using (XmlWriter xmlWriter = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
    {
        serializer.Serialize(xmlWriter, films);
    }
}
*/

Console.WriteLine(Helper.ToXmlValue(DateTime.Now));