using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnetsynt2
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "test2_D";

            Mnet MainNetwork = new Mnet();
            MainNetwork.ReadMnetFile(file + @".MNET");
            ReducteDUP(MainNetwork);
        }
        private static void ReducteDUP(Mnet MainNetwork)
        {
            List<Node> Nodes = new List<Node>();
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                if (MainNetwork.nodes[i].NodeType.StartsWith("DUP"))
                {
                    Nodes.Add(MainNetwork.nodes[i]);
                }
            }
            // DupUnion

            List<string> Dnodes = new List<string>();
            List<string> Snodes = new List<string>();
            for (int i = 0; i < MainNetwork.wires.Count; i++)
            {
                if (MainNetwork.FindNode(MainNetwork.wires[i].DistName).NodeType.StartsWith("DUP"))
                {
                    if (MainNetwork.FindNode(MainNetwork.wires[i].SrcName).NodeType.StartsWith("DUP"))
                    {
                        while (MainNetwork.FindWireFrom(MainNetwork.wires[i].DistName) != null)
                        {
                            Wire W = MainNetwork.FindWireFrom(MainNetwork.wires[i].DistName);
                            W.SrcName = MainNetwork.wires[i].SrcName;
                        }
                        Dnodes.Add(MainNetwork.wires[i].DistName);
                        Dnodes.Add(MainNetwork.wires[i].SrcName);
                    }
                }
            }
        }
    }
}
