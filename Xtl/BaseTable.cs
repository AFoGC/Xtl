﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xtl
{
    public abstract class BaseTable
    {
        internal abstract void InvokeBindings();
        public abstract void SaveTable(XmlDocument document);
        public abstract void LoadTable(XmlNode tableNode);
        public abstract Type RecordType { get; }
    }
}
