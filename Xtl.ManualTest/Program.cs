// See https://aka.ms/new-console-template for more information
using System.Text;
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

XmlNode tableNode = document.CreateNode(XmlNodeType.Element, "Tables", null);
document.AppendChild(tableNode);


builder.AddSaveRule(x => x.Id);
builder.AddSaveRule(x => x.GenreId);
builder.AddSaveRule(x => x.Name);
builder.AddSaveRule(x => x.RealiseDate);
builder.AddSaveRule(x => x.WatchDate);

Table<Film> films = new Table<Film>(builder);
films.Default = new Film();
//films.Records.Add(film1);
//films.Records.Add(film2);
//films.Records.Add(film3);
//films.Records.Add(film4);
//films.SaveTable(document);

TablesCollection tablesCollection = new TablesCollection();
tablesCollection.AddTable(films);

tablesCollection.Load("outTables1.xml");
tablesCollection.Save("outTables2.xml");

tablesCollection.AddTabl<FilmsTable, Film>(x =>
{
    x.AddSaveRule(y => y.Id);
});

/*
using (XmlWriter xmlWriter = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
{
    document.Save(xmlWriter);
}
*/
/*
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
/*
Console.WriteLine(Helper.ToXmlValue(DateTime.Now));
*/