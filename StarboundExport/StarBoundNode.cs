using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarboundExport
{
    class StarBoundPort
    {
        public string PortName;
        public int xcoord;
        public int ycoord;
    }
    class StarBoundNode
    {
        public string NodeType;
        public string NodeID;
        public int xcoord;
        public int ycoord;
        public List<StarBoundPort> Ports;
 
    }
}
