using System;
using System.Collections.Generic;
using System.Drawing;

namespace StarboundExport
{
    class Program
    {
        const int Height = 4;
        const int Step = 3;
        const int ImageMult = 25;
        static int tx = 0;
        static int ty = 0;

        static void Main(string[] args)
        {
            string file = "test2_D";
            if (args.Length > 0)
                file = args[0];

            Mnet MainNetwork = new Mnet();
            MainNetwork.ReadMnetFile(file + @".MNET");
            ReducteDUP(MainNetwork);
            List<Node> DupNodes = new List<Node>();
            RemoveDUPNodes(MainNetwork, DupNodes);
            RemoveDUOWires(MainNetwork, DupNodes);
            //place nods
            List<StarBoundNode> SBNodes = new List<StarBoundNode>();

            PlaceSBNodes(MainNetwork, SBNodes);
            //place wires
            List<StarboundWire> SBWires = new List<StarboundWire>();
            ConnectSBNodes(MainNetwork, SBNodes, SBWires);
            string ostr = GenExportFile(SBNodes, SBWires);
            System.IO.File.WriteAllText(file + ".SBBIN", ostr);
            //TODO
            //draw image
            Image IMmain = new Bitmap((tx + 2) * ImageMult * 3, (ty + 2) * ImageMult * 3);
            Graphics GR = Graphics.FromImage(IMmain);
            GR.Clear(Color.Black);
            Font F = new Font("arial", 8);
            //DrawNodes;
            for (int i = 0; i < SBNodes.Count; i++)
            {
                int x = SBNodes[i].xcoord * ImageMult + ImageMult;
                int y = SBNodes[i].ycoord * ImageMult + ImageMult;
                GR.DrawRectangle(Pens.Green, x, y, ImageMult * 2, ImageMult * 2);
                GR.DrawString(SBNodes[i].NodeType, F, Brushes.Green, x, y-15);
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
                int sx = SBWires[i].startx * ImageMult + ImageMult + ImageMult/2;
                int sy = SBWires[i].starty * ImageMult + ImageMult + ImageMult / 2;
                int ex = SBWires[i].endx * ImageMult + ImageMult + ImageMult / 2;
                int ey = SBWires[i].endy * ImageMult + ImageMult + ImageMult / 2;

                GR.DrawLine(Pens.Yellow, sx, sy, ex, ey);

            }
            //export image
            IMmain.Save(file + ".bmp");

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
