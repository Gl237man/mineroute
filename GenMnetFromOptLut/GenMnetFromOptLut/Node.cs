using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMnetFromOptLut
{
    class Node
    {
        public string NodeType;
        public string NodeName;
        public string ToString()
        {
            return "NODE:" + NodeType + ":" + NodeName;
        }

        //NODE:INPort:sig4[1]
    }
}
