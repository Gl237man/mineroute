using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnetsynt3
{
    class Wire
    {
        public string SrcName;
        public string DistName;
        public string SrcPort;
        public string DistPort;
        public bool Placed;
        public override string ToString()
        {
            return "WIRE:" + SrcName + "-" + SrcPort + ":" + DistName + "-" + DistPort;
        }
        public void ReadFromString(string instr)
        {
            string[] tstr = instr.Split(':');
            SrcName = tstr[1].Split('-')[0];
            SrcPort = tstr[1].Split('-')[1];
            DistName = tstr[2].Split('-')[0];
            DistPort = tstr[2].Split('-')[1];
        }
    }
}
