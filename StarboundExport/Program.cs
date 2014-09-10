using System;
using System.Collections.Generic;
using System.Drawing;

namespace StarboundExport
{
    class Program
    {
        static int Height = 6;
        const int Step = 4;
        static int ImageMult = 25;
        static int tx = 0;
        static int ty = 0;
        static int OptimiseDeep = 3;

        static void Main(string[] args)
        {
            string file = "test2_D";
            if (args.Length > 0)
            {
                file = args[0];
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("H="))
                        Height = Convert.ToInt32(args[i].Split('=')[1]);
                    if (args[i].StartsWith("ImMult="))
                        ImageMult = Convert.ToInt32(args[i].Split('=')[1]);
                    if (args[i].StartsWith("O="))
                        OptimiseDeep = Convert.ToInt32(args[i].Split('=')[1]);
                }
            }

            Console.WriteLine("Использование:");
            Console.WriteLine("");
            Console.WriteLine("StarboundExport [filename] {H=(4)Height,ImMult=(25)imagesize,O=(3)OptimizeDeep}");
            Console.WriteLine("Примеры:");
            Console.WriteLine("");
            Console.WriteLine("StarboundExport test2_D H=4 ImMult=30 O=3");
            Console.WriteLine("StarboundExport test2_D");
            Console.WriteLine("");

            Mnet MainNetwork = new Mnet();
            MainNetwork.ReadMnetFile(file + @".MNET");
            ReducteDUP(MainNetwork);
            List<Node> DupNodes = new List<Node>();
            RemoveDUPNodes(MainNetwork, DupNodes);
            RemoveDUOWires(MainNetwork, DupNodes);

            //Sorting Nodes;
            SortOptimize(MainNetwork);

            //place nods
            List<StarBoundNode> SBNodes = new List<StarBoundNode>();
            PlaceSBNodes(MainNetwork, SBNodes);
            //place wires
            List<StarboundWire> SBWires = new List<StarboundWire>();
            ConnectSBNodes(MainNetwork, SBNodes, SBWires);


            string ostr = GenExportFile(SBNodes, SBWires);
            System.IO.File.WriteAllText(file + ".SBBIN", ostr);
            //CalcLincLengch
            double len = CalcComplexity(SBWires);
            Console.WriteLine("complexity:" + len);
            
            //draw image
            Image IMmain = DrawImage(SBNodes, SBWires);
            //export image
            IMmain.Save(file + ".bmp");

        }

        private static double CalcComplexity(List<StarboundWire> SBWires)
        {
            double len = 0;
            for (int i = 0; i < SBWires.Count; i++)
            {
                len += Math.Sqrt((SBWires[i].startx - SBWires[i].endx) * (SBWires[i].startx - SBWires[i].endx)
                               + (SBWires[i].starty - SBWires[i].endy) * (SBWires[i].starty - SBWires[i].endy));
            }
            return len;
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
                    return list.Count+1;
                if (list[i].NodeName == p)
                    return i;
            }
            return 0;
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

        private static Image DrawImage(List<StarBoundNode> SBNodes, List<StarboundWire> SBWires)
        {
            Image IMmain = new Bitmap((tx + 2) * ImageMult * Step, (ty + 4) * ImageMult * Step);
            Graphics GR = Graphics.FromImage(IMmain);
            GR.Clear(Color.Black);
            Font F = new Font("arial", 8);
            //DrawNodes;
            for (int i = 0; i < SBNodes.Count; i++)
            {
                int x = SBNodes[i].xcoord * ImageMult + ImageMult;
                int y = SBNodes[i].ycoord * ImageMult + ImageMult;
                GR.DrawRectangle(Pens.Green, x, y, ImageMult * 2, ImageMult * 2);
                GR.DrawString(SBNodes[i].NodeType, F, Brushes.Green, x, y - 15);
                //DrawPorts
                for (int j = 0; j < SBNodes[i].Ports.Count; j++)
                {
                    int px = SBNodes[i].Ports[j].xcoord * ImageMult + ImageMult;
                    int py = SBNodes[i].Ports[j].ycoord * ImageMult + ImageMult;

                    GR.DrawEllipse(Pens.Green, px, py, ImageMult, ImageMult);
                }
            }
            //DrawWires;
            for (int i = 0; i < SBWires.Count; i++)
            {
                int sx = SBWires[i].startx * ImageMult + ImageMult + ImageMult / 2;
                int sy = SBWires[i].starty * ImageMult + ImageMult + ImageMult / 2;
                int ex = SBWires[i].endx * ImageMult + ImageMult + ImageMult / 2;
                int ey = SBWires[i].endy * ImageMult + ImageMult + ImageMult / 2;

                GR.DrawLine(Pens.Yellow, sx, sy, ex, ey);

            }
            return IMmain;
        }

        private static string GenExportFile(List<StarBoundNode> SBNodes, List<StarboundWire> SBWires)
        {
            string ostr = "";
            for (int i = 0; i < SBNodes.Count; i++)
            {
                ostr += SBNodes[i].ToString() + "\r\n";
            }
            for (int i = 0; i < SBWires.Count; i++)
            {
                ostr += SBWires[i].ToString() + "\r\n";
            }
            return ostr;
        }

        private static void ConnectSBNodes(Mnet MainNetwork, List<StarBoundNode> SBNodes, List<StarboundWire> SBWires)
        {
            for (int i = 0; i < MainNetwork.wires.Count; i++)
            {
                Wire W = MainNetwork.wires[i];
                StarBoundNode StartNode = FindSBNode(SBNodes, W.SrcName);
                StarBoundNode EndNode = FindSBNode(SBNodes, W.DistName);
                StarBoundPort StartPort = FindSBPort(StartNode, W.SrcPort);
                StarBoundPort EndPort = FindSBPort(EndNode, W.DistPort);

                StarboundWire SBW = new StarboundWire();
                SBW.startx = StartPort.xcoord;
                SBW.starty = StartPort.ycoord;
                SBW.endx = EndPort.xcoord;
                SBW.endy = EndPort.ycoord;
                SBWires.Add(SBW);
            }
        }

        private static StarBoundPort FindSBPort(StarBoundNode SBNode, string PortName)
        {
            for (int i = 0; i < SBNode.Ports.Count; i++)
            {
                if (SBNode.Ports[i].PortName == PortName) return SBNode.Ports[i];
            }
            return null;
        }

        private static StarBoundNode FindSBNode(List<StarBoundNode> SBNodes, string NodeName)
        {
            for (int i = 0; i < SBNodes.Count; i++)
            {
                if (SBNodes[i].NodeID == NodeName) return SBNodes[i];
            }
            return null;
        }

        private static void PlaceSBNodes(Mnet MainNetwork, List<StarBoundNode> SBNodes)
        {
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                if (MainNetwork.nodes[i].NodeType == "INPort")
                {
                    PlaceSBNode(SBNodes, MainNetwork.nodes[i]);
                }
            }

            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                if (MainNetwork.nodes[i].NodeType != "INPort" && MainNetwork.nodes[i].NodeType != "OUTPort")
                {
                    PlaceSBNode(SBNodes, MainNetwork.nodes[i]);
                }
            }

            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                if (MainNetwork.nodes[i].NodeType == "OUTPort")
                {
                    PlaceSBNode(SBNodes, MainNetwork.nodes[i]);
                }
            }
        }

        private static void PlaceSBNode(List<StarBoundNode> SBNodes, Node node)
        {
            SBNodes.Add(new StarBoundNode(node.NodeType, node.NodeName, tx * Step, ty * Step));
            ty++;
            if (ty == Height)
            {
                ty = 0;
                tx++;
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
                        while (MainNetwork.FindWireFrom(MainNetwork.wires[i].DistName)!=null)
                        {
                            Wire W = MainNetwork.FindWireFrom(MainNetwork.wires[i].DistName);
                            W.SrcName = MainNetwork.wires[i].SrcName;
                        }
                        Dnodes.Add(MainNetwork.wires[i].DistName);
                        Dnodes.Add(MainNetwork.wires[i].SrcName);
                    }
                }
            }
             
            //Replace Wire To
            for (int i = 0; i < MainNetwork.wires.Count; i++)
            {
                    if (MainNetwork.FindNode(MainNetwork.wires[i].SrcName).NodeType.StartsWith("DUP"))
                    {
                        string sname = MainNetwork.wires[i].SrcName;
                        while (MainNetwork.FindWireFrom(sname) != null)
                        {
                            Wire W = MainNetwork.FindWireFrom(sname);
                            Wire W2 = MainNetwork.FindWireTo(sname);
                            W.SrcName = W2.SrcName;
                            W.SrcPort = W2.SrcPort;
                        }
                    }
            }
        }
    }
}
