using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.ManualTest.Entities
{
    public class Film : Record
    {
        private int _id = 0;
        private string _name = string.Empty;
        private DateTime? _watchDate;
        private int _realiseDate;
        private int _genreId;

        private Genre _genre = null;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }
        public string Name
        { 
            get => _name; 
            set { _name = value; OnPropertyChanged(); }
        } 
        public DateTime? WatchDate
        {
            get => _watchDate;
            set { _watchDate = value; OnPropertyChanged(); }
        }
        public int RealiseDate
        {
            get => _realiseDate;
            set { _realiseDate = value; OnPropertyChanged(); }
        }
        public int GenreId
        {
            get => _genreId; 
            set { _genreId = value; OnPropertyChanged(); }
        }

        public Genre Genre 
        { 
            get => _genre; 
            private set { _genre = value; OnPropertyChanged(); } 
        }

        public override object Clone()
        {
            return new Film 
            { 
                Id = Id,
                Name = Name, 
                GenreId = GenreId, 
                RealiseDate = RealiseDate, 
                WatchDate = WatchDate 
            };
        }
    }
}
