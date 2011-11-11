using System;
using RT.Util.ExtensionMethods;
using RT.Util.Xml;
using RT.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZiimHelper
{
    [XmlIgnoreIfDefault]
    sealed class ZiimFile
    {
        public List<Item> Items = new List<Item>();
    }
}
