using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnetsynt2
{
    class Program
    {

        static int OptimiseDeep = 3;

        static void Main(string[] args)
        {
            string file = "test_D";

            Mnet MainNetwork = new Mnet();
            MainNetwork.ReadMnetFile(file + @".MNET");
            ReducteDUP(MainNetwork);
            //loadnodes
            Console.WriteLine("Загрузка темплейтов");
            RouteUtils.Node[] mcNodes = new RouteUtils.Node[MainNetwork.nodes.Count];
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                mcNodes[i] = new RouteUtils.Node(MainNetwork.nodes[i].NodeType + ".binhl");
            }
            Console.WriteLine("OK");
            //Place nodes

            
            RouteUtils.Node OutNode = new RouteUtils.Node("OUT",50, 20, 10);

            OutNode.PlaceAnotherNode(new RouteUtils.Node("DUP23.binhl"), 0, 0, 0);

            OutNode.export("test_D.binhl");
            

            /*
            List<Node> DupNodes = new List<Node>();
            RemoveDUPNodes(MainNetwork, DupNodes);
            RemoveDUOWires(MainNetwork, DupNodes);

            //Sorting Nodes;
            SortOptimize(MainNetwork);
            */

        }

        private static List<Wire> FindAllWiresTo(List<Wire> Wlist, Node node)
        {
            List<Wire> WO = new List<Wire>();
            for (int i = 0; i < Wlist.Count; i++)
            {
                if (Wlist[i].DistName == node.NodeName)
                    WO.Add(Wlist[i]);
                if (Wlist[i].SrcName == node.NodeName)
                    WO.Add(Wlist[i]);
            }
            return WO;
        }

        private static int CalcWireLens(List<Wire> WlistTo, Mnet MainNetwork)
        {
            int k = 0;
            for (int i = 0; i < WlistTo.Count; i++)
            {
                int ka = FindNodeIndex(MainNetwork.nodes, WlistTo[i].SrcName);
                int kb = FindNodeIndex(MainNetwork.nodes, WlistTo[i].DistName);
                k += (kb - ka);
            }
            return k;
        }

        private static int FindNodeIndex(List<Node> list, string p)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].NodeType == "OUTPort")
                    return list.Count + 1;
                if (list[i].NodeName == p)
                    return i;
            }
            return 0;
        }

        private static void SortOptimize(Mnet MainNetwork)
        {
            for (int i = 0; i < MainNetwork.nodes.Count * OptimiseDeep; i++)
            {
                for (int j = 0; j < (MainNetwork.nodes.Count - 1); j++)
                {
                    if (MainNetwork.nodes[j].NodeType != "INPort" && MainNetwork.nodes[j].NodeType != "OUTPort")
                    {
                        List<Wire> Wlist1 = FindAllWiresTo(MainNetwork.wires, MainNetwork.nodes[j]);
                        //List<Wire> Wlist2 = FindAllWiresTo(MainNetwork.wires, MainNetwork.nodes[j+1]);
                        int ka = CalcWireLens(Wlist1, MainNetwork);
                        //int kb = CalcWireLens(Wlist2, MainNetwork);
                        if (ka < 0)
                        {
                            if (MainNetwork.nodes[j + 1].NodeType != "INPort" && MainNetwork.nodes[j + 1].NodeType != "OUTPort")
                            {
                                Node N = new Node();
                                N = MainNetwork.nodes[j];
                                MainNetwork.nodes[j] = MainNetwork.nodes[j + 1];
                                MainNetwork.nodes[j + 1] = N;
                            }
                        }
                        if (ka > 0)
                        {
                            if (MainNetwork.nodes[j - 1].NodeType != "INPort" && MainNetwork.nodes[j - 1].NodeType != "OUTPort")
                            {
                                Node N = new Node();
                                N = MainNetwork.nodes[j];
                                MainNetwork.nodes[j] = MainNetwork.nodes[j - 1];
                                MainNetwork.nodes[j - 1] = N;
                            }
                        }
                    }
                }
            }
        }

        private static void RemoveDUOWires(Mnet MainNetwork, List<Node> Nodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; i < Nodes.Count; i++)
                {
                    MainNetwork.RemoveWireTo(Nodes[i].NodeName, "I" + j.ToString());
                }
            }
        }

        private static void RemoveDUPNodes(Mnet MainNetwork, List<Node> Nodes)
        {

            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                if (MainNetwork.nodes[i].NodeType.StartsWith("DUP"))
                {
                    Nodes.Add(MainNetwork.nodes[i]);
                }
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                MainNetwork.RemoveNode(Nodes[i].NodeName);
            }
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
