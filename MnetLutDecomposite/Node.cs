using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnetLutDecomposite
{
    class Node
    {
        public string NodeType;
        public string NodeName;
        public string ToString()
        {
            return "NODE:" + NodeType + ":" + NodeName;
        }
        public void ReadFromString(string instr)
        {
            string[] tstr = instr.Split(':');
            NodeType = tstr[1];
            NodeName = tstr[2];
        }
    }
}
