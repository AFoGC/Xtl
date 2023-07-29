// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Xtl;
using Xtl.ManualTest.Entities;


Film film1 = new Film { Id = 1, GenreId = 1, Name = "Hui1", RealiseDate = 2001 };
Film film2 = new Film { Id = 2, GenreId = 2, Name = "Hui2", RealiseDate = 2022 };
Film film3 = new Film { Id = 3, GenreId = 2, Name = "Hui3", RealiseDate = 2013 };
Film film4 = new Film { Id = 4, GenreId = 1, Name = "Hui4", RealiseDate = 2015, WatchDate = new DateTime(1999, 1, 2) };

TablesCollection collection = new TablesCollection();
collection.AddTable<FilmsTable, Film>(x =>
{
    x.Default = new Film() { Name = "Hui3" };

    x.AddSaveRule(x => x.MarksSystem, 0);

    x.EntityBuilder.AddSaveRule(x => x.Id);
    x.EntityBuilder.AddSaveRule(x => x.GenreId);
    x.EntityBuilder.AddSaveRule(x => x.Name);
    x.EntityBuilder.AddSaveRule(x => x.RealiseDate);
    x.EntityBuilder.AddSaveRule(x => x.WatchDate);
});

//FilmsTable filmsTable = collection.GetTable<FilmsTable>();

//filmsTable.MarksSystem = 6;

//filmsTable.Records.Add(film1);
//filmsTable.Records.Add(film2);
//filmsTable.Records.Add(film3);
//filmsTable.Records.Add(film4);

collection.Load("Out3.xml");
collection.Save("Out4.xml");