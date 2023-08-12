// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Xtl;
using Xtl.ManualTest.Entities;


Film film1 = new Film { GenreId = 0, Name = "Hui1", RealiseDate = 2001 };
Film film2 = new Film { GenreId = 0, Name = "Hui2", RealiseDate = 2022 };
Film film3 = new Film { GenreId = 0, Name = "Hui3", RealiseDate = 2013 };
Film film4 = new Film { GenreId = 0, Name = "Hui4", RealiseDate = 2015, WatchDate = new DateTime(1999, 1, 2) };

Genre genre1 = new Genre { Name = "Film", IsSerial = false };
Genre genre2 = new Genre { Name = "Serial", IsSerial = false };

TablesCollection collection = new TablesCollection();
collection.AddTable<FilmsTable, Film>(x =>
{
    x.DefaultTable = new FilmsTable();
    x.DefaultRecord = new Film() { Name = "Hui3" };

    x.AddSaveRule(x => x.MarksSystem, 0);

    x.EntityBuilder.AddSaveRule(x => x.GenreId);
    x.EntityBuilder.AddSaveRule(x => x.Name);
    x.EntityBuilder.AddSaveRule(x => x.RealiseDate);
    x.EntityBuilder.AddSaveRule(x => x.WatchDate);

    x.EntityBuilder.HasOne(y => y.GenreId, y => y.Genre);
});

collection.AddTable<GenresTable, Genre>(x =>
{
    x.DefaultTable = new GenresTable();
    x.DefaultRecord = new Genre();

    x.EntityBuilder.AddSaveRule(x => x.Name);
    x.EntityBuilder.AddSaveRule(x => x.IsSerial);
});

/*
FilmsTable filmsTable = collection.GetTable<FilmsTable>();
GenresTable genresTable = collection.GetTable<GenresTable>();

filmsTable.MarksSystem = 6;

filmsTable.Add(film1);
filmsTable.Add(film2);
filmsTable.Add(film3);
filmsTable.Add(film4);

genresTable.Add(genre1);
genresTable.Add(genre2);

film1.GenreId = 1;
film2.GenreId = 2;
film3.GenreId = 2;
film4.GenreId = 1;

Console.WriteLine(film1.Genre.Name);
Console.WriteLine(film2.Genre.Name);
Console.WriteLine(film3.Genre.Name);
Console.WriteLine(film4.Genre.Name);
*/


collection.Load("Out3.xml");
foreach (var item in collection.GetTable<FilmsTable>())
{
    Console.WriteLine(item.Genre.Name);
}

//collection.GetTable<FilmsTable>().Add(new Film { Name = "Zalupa"});
collection.Save("Out4.xml");