using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnetLutDecomposite
{
    class Mnet
    {
        public List<Node> nodes;
        public List<Wire> wires;

        public void ReadMnetFile(string FileName)
        {
            nodes = new List<Node>();
            wires = new List<Wire>();
            string[] tstr = System.IO.File.ReadAllLines(FileName);
            for (int i = 0; i < tstr.Length; i++)
            {
                if (tstr[i].Split(':')[0] == "NODE")
                {
                    Node N = new Node();
                    N.ReadFromString(tstr[i]);
                    nodes.Add(N);
                }
                if (tstr[i].Split(':')[0] == "WIRE")
                {
                    Wire W = new Wire();
                    W.ReadFromString(tstr[i]);
                    wires.Add(W);
                }
            }
        }

        public List<Node> GetLuts()
        {
            List<Node> Luts = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].IsLut())
                {
                    Luts.Add(nodes[i]);
                }
            }
            return Luts;
        }
    }
}
