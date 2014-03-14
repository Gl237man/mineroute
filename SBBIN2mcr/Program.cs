using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBBIN2mcr
{
    class Program
    {
        static void Main(string[] args)
        {
            List<StarBoundNode> nodes = new List<StarBoundNode>();
            List<StarboundWire> wires = new List<StarboundWire>();
            string[] indat = System.IO.File.ReadAllLines("test_D.SBBIN");

            for (int i=0;i<indat.Length;i++)
            {
                if (indat[i].Split(':')[0] == "W")
                    wires.Add(new StarboundWire(indat[i]));
                if (indat[i].Split(':')[0] == "N")
                    nodes.Add(new StarBoundNode(indat[i]));
            }

        }
    }
}
