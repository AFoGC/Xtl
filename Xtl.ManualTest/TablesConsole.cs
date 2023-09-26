using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xtl.ManualTest.Entities;

namespace Xtl.ManualTest
{
    public class TablesConsole
    {
        public static void WriteTabes(TablesCollection tablesCollection)
        {
            Table<Film> films = tablesCollection.GetTableByRecord<Film>();
            Table<Genre> genres = tablesCollection.GetTableByRecord<Genre>();
            Table<PriorityFilm> priorityFilms = tablesCollection.GetTableByRecord<PriorityFilm>();

            Console.WriteLine("------------New Table-----------");
            Console.WriteLine("Films: ");
            foreach (Film film in films)
            {
                if (film.Genre != null)
                    Console.WriteLine($"{film.Id}) Name: {film.Name}, Genre: {film.Genre.Name}");
                else
                    Console.WriteLine($"{film.Id}) Name: {film.Name}, Genre: NULL");
            }
            Console.WriteLine("PriorityFilms: ");
            foreach (PriorityFilm priorityFilm in priorityFilms)
            {
                Console.WriteLine($"{priorityFilm.Id}) Name: {priorityFilm.Film.Name}, CreationTime: {priorityFilm.CreationTime}");
            }
            Console.WriteLine("Genres: ");
            foreach (Genre genre in genres)
            {
                Console.WriteLine($"{genre.Id}) {genre.Name}");
                foreach(Film film in genre.Films)
                {
                    Console.WriteLine($"\t{film.Name}");
                }
            }
        }
    }
}
