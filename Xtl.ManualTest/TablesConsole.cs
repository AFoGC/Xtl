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

            Console.WriteLine("------------New Table-----------");
            Console.WriteLine("Films: ");
            foreach (Film film in films)
            {
                Console.WriteLine($"Name: {film.Name}, Genre: {film.Genre.Name}");
            }
            Console.WriteLine("Genres: ");
            foreach (Genre genre in genres)
            {
                Console.WriteLine($"{genre.Name}");
                foreach(Film film in genre.Films)
                {
                    Console.WriteLine($"\t{film.Name}");
                }
            }
        }
    }
}
