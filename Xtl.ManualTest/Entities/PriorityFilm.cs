using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.ManualTest.Entities
{
    public class PriorityFilm : Record
    {
        private int _id = 0;
        private DateTime? _creationTime = null;
        private Film _film = null;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public DateTime? CreationTime
        {
            get => _creationTime;
            set { _creationTime = value; OnPropertyChanged(); }
        }

        public Film Film
        {
            get => _film;
            set { _film = value; OnPropertyChanged(); }
        }

        public override object Clone()
        {
            return new PriorityFilm
            {
                Id = Id,
                CreationTime = CreationTime
            };
        }
    }
}
