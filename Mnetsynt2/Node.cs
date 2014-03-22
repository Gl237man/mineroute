using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnetsynt2
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
        public bool IsLut()
        {
            return NodeType.StartsWith("C2LUT_");
        }

        public string GetLutKey()
        {
            if (IsLut())
            {
                return NodeType.Substring(6, 4);
            }
            return "";
        }
    }
}
