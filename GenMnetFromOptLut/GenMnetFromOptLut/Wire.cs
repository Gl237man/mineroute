using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMnetFromOptLut
{
    class Wire
    {
        public string SrcName;
        public string DistName;
        public string SrcPort;
        public string DistPort;
        public string ToString()
        {
            return "WIRE:" + SrcName + "-" + SrcPort + ":" + DistName + "-" + DistPort;
        }

    }
}
