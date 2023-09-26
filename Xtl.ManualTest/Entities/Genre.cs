using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xtl.ManualTest.Entities
{
    public class Genre : Record
    {
        private int _id = 0;
        private string _name = string.Empty;
        private bool _isSerial = false;

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
        public bool IsSerial
        {
            get => _isSerial;
            set { _isSerial = value; OnPropertyChanged(); }
        }

        public RecordsCollection<Film> Films { get; }

        public Genre()
        {
            Films = new RecordsCollection<Film>();
        }

        public override object Clone()
        {
            return new Genre
            {
                Id = Id,
                Name = Name,
                IsSerial = IsSerial
            };
        }
    }
}
