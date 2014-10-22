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

    public class SymbolTable
    {
        public List<WireObj> WireObjs = new List<WireObj>();
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
    }
}
