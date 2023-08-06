using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.ManualTest.Entities
{
    public class Genre : Record
    {
        public string Name { get; set; } = string.Empty;
        public bool IsSerial { get; set; }

        public override object Clone()
        {
            return new Genre
            {
                Name = Name,
                IsSerial = IsSerial
            };
        }
    }
}
