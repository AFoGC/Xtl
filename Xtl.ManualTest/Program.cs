// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xtl;
using Xtl.ManualTest;
using Xtl.ManualTest.Entities;
TablesCollection collection = new TablesCollection();

collection.Configure(builder =>
{
    builder.AddTable<FilmsTable, Film>(x =>
    {
        x.DefaultTable = new FilmsTable();
        x.DefaultRecord = new Film() { Name = "Hui3" };

        x.AddSaveRule(x => x.MarksSystem, 0);

        x.EntityBuilder.AddSaveRule(x => x.GenreId);
        x.EntityBuilder.AddSaveRule(x => x.Name);
        x.EntityBuilder.AddSaveRule(x => x.RealiseDate);
        x.EntityBuilder.AddSaveRule(x => x.WatchDate);
    });

    builder.AddTable<GenresTable, Genre>(x =>
    {
        x.DefaultTable = new GenresTable();
        x.DefaultRecord = new Genre();

        x.EntityBuilder.AddSaveRule(x => x.Name);
        x.EntityBuilder.AddSaveRule(x => x.IsSerial);
    });

    builder.AddOneToMany<Genre, Film>(x => x.GenreId, x => x.Genre, x => x.Films);
});

collection.Load("Out3.xml");
TablesConsole.WriteTabes(collection);
/*
Table<Film> films = collection.GetTableByRecord<Film>();
Table<Genre> genres = collection.GetTableByRecord<Genre>();

Film film1 = films.Add(new Film { Name = "film1" });
Film film2 = films.Add(new Film { Name = "film2" });
Film film3 = films.Add(new Film { Name = "film3" });

Genre genre1 = genres.Add(new Genre { Name = "Film", IsSerial = false });
Genre genre2 = genres.Add(new Genre { Name = "Serial", IsSerial = true });
TablesConsole.WriteTabes(collection);

genre1.Films.Add(film1);
film2.GenreId = genre1.Id;
TablesConsole.WriteTabes(collection);

genre2.Films.Add(film1);
genre2.Films.Add(film2);
film3.GenreId = genre1.Id;
TablesConsole.WriteTabes(collection);
*/
/*
Film film1 = films.First(x => x.Id == 1); //Film
Film film2 = films.First(x => x.Id == 2); //Serial
Film film3 = films.First(x => x.Id == 3); //Serial
Film film4 = films.First(x => x.Id == 4); //Film

Genre filmGenre = genres.First(x => x.Id == 1);
Genre serialGenre = genres.First(x => x.Id == 2);

TablesConsole.WriteTabes(collection);


serialGenre.Films.Remove(film2);
serialGenre.Films.Add(film2);
TablesConsole.WriteTabes(collection);
*/
//collection.GetTable<FilmsTable>().Add(new Film { Name = "Zalupa"});
//collection.Save("Out4.xml");