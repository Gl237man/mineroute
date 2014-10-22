using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLC
{
    public class WireObj
    {
        public string name;
        public int wide;
    }

    public class PortObj
    {
        public string name;
        public int wide;
        public string type;
    }

    public class SymbolTable
    {
        public List<WireObj> WireObjs = new List<WireObj>();
        public List<PortObj> PortObjs = new List<PortObj>();
        private Parser parser;

        public SymbolTable(Parser parser)
        {
            // TODO: Complete member initialization
            this.parser = parser;
        }

        internal void OpenScope()
        {
            //throw new NotImplementedException();
        }

        internal void CloseScope()
        {
            //throw new NotImplementedException();
        }

        internal void NewWire(string name, int wide)
        {
            WireObjs.Add(new WireObj { name = name ,wide = wide});
        }

        internal void NewPort(string name, string type, int wide)
        {
            PortObjs.Add(new PortObj { name = name, type = type, wide = wide });
        }
    }
}
