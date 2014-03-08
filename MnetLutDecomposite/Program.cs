using System;
using System.Collections.Generic;
using System.Text;

namespace MnetLutDecomposite
{
    class Program
    {
        static List<Node> nodes;
        static List<Wire> wires;
        static void Main(string[] args)
        {
            ReadMnetFile(@"test2.MNET");
        }

        private static void ReadMnetFile(string FileName)
        {
            nodes = new List<Node>();
            wires = new List<Wire>();
            string[] tstr = System.IO.File.ReadAllLines(FileName);
            for (int i=0;i<tstr.Length;i++)
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
    }
}
