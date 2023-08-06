using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestXmlTables
{
    public class Test
    {
        private string _hui = string.Empty;
        public int Count = 0;

        public string Hui
        {
            get => _hui;
            set
            {
                _hui = value;
                Count++;
            }
        }
    }
}
