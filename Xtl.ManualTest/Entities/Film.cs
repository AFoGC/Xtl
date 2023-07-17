using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.ManualTest.Entities
{
    public class Film : Record
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? WatchDate { get; set; }
        public int RealiseDate { get; set; }
        public int GenreId { get; set; }
    }
}
