using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.ManualTest.Entities
{
    public class FilmsTable : Table<Film>
    {
        public int MarksSystem { get; set; }

        public FilmsTable()
        {
            MarksSystem = 0;
        }
    }
}
