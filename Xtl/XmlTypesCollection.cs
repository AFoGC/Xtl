using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtl
{
    public class XmlTypesCollection
    {
        //private readonly Dictionary<string, Type> _typesDictionary;
        private readonly List<(string, Type)> _typePairs;

        public XmlTypesCollection()
        {
            _typePairs= new List<(string, Type)>();
            //_typesDictionary = new Dictionary<string, Type>();
        }
    }
}
