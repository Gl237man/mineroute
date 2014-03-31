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
            string file = "test3_D";

            Mnet MainNetwork = new Mnet();
            MainNetwork.ReadMnetFile(file + @".MNET");
            //ReducteDUP(MainNetwork);
            //loadnodes
            Console.WriteLine("Загрузка темплейтов");
            RouteUtils.Node[] mcNodes = new RouteUtils.Node[MainNetwork.nodes.Count];
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                mcNodes[i] = new RouteUtils.Node(MainNetwork.nodes[i].NodeType + ".binhl");
            }
            Console.WriteLine("OK");
            //Place nodes
            int PlaceLayer = 30;
            int PortNum = CalcPortNum(MainNetwork);

            int BaseSize = 5 * PortNum;


            while (!TryPlace(MainNetwork, mcNodes, PlaceLayer, BaseSize))
            {
                BaseSize += 10;
                Console.WriteLine("Размер:" + BaseSize.ToString());
            }
            BaseSize += 30;

            Console.WriteLine("Размещение ОК");

            List<RouteUtils.Cpoint> Cpoints = new List<RouteUtils.Cpoint>();

            //CreateCpointList

            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                for (int j = 0; j < mcNodes[i].InPorts.Length; j++)
                {
                    Cpoints.Add(new RouteUtils.Cpoint
                    {
                        BaseX = mcNodes[i].InPorts[j].PosX + MainNetwork.nodes[i].x,
                        BaseY = mcNodes[i].InPorts[j].PosY + MainNetwork.nodes[i].y,
                        PointName = MainNetwork.nodes[i].NodeName + "-" + mcNodes[i].InPorts[j].Name,
                        indat = true
                    });

                }

                for (int j = 0; j < mcNodes[i].OutPorts.Length; j++)
                {
                    Cpoints.Add(new RouteUtils.Cpoint
                    {
                        BaseX = mcNodes[i].OutPorts[j].PosX + MainNetwork.nodes[i].x,
                        BaseY = mcNodes[i].OutPorts[j].PosY + MainNetwork.nodes[i].y,
                        PointName = MainNetwork.nodes[i].NodeName + "-" + mcNodes[i].OutPorts[j].Name,
                        indat = false
                    });

                }
            }

            int CurrentWireLayer = 1;
            int CurrentRealLayer = 0;
            int WireNum = 0;
            RouteUtils.Wire[] MCWires = new RouteUtils.Wire[MainNetwork.wires.Count];
            //Draw wires in layer

            for (int j = 0; j < 12; j++)
            {

                CurrentWireLayer = j * 2 + 1;
                if (j > 4) CurrentWireLayer += 5;

                CurrentRealLayer = PlaceLayer - 1 - j * 2;

                if (j > 4) CurrentRealLayer -= 2;

                WireNum = 0;

                string[,] WireMask = new string[BaseSize, BaseSize];
                for (int i = 0; i < MainNetwork.wires.Count; i++)
                {

                    List<int> WPX;
                    List<int> WPY;
                    if (!MainNetwork.wires[i].Placed)
                    {
                        PlaceWire(MainNetwork, BaseSize, Cpoints, CurrentWireLayer, CurrentRealLayer, i, MCWires, WireMask, out WPX, out WPY);
                        if (MainNetwork.wires[i].Placed)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            WireNum++;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        Console.WriteLine(MainNetwork.wires[i].ToString());

                    }
                }
                Console.WriteLine("Разведено в текущем слое:" + WireNum);

            }
            RouteUtils.Node OutNode = new RouteUtils.Node("OUT", BaseSize, BaseSize, 40);

            //OutNode.PlaceAnotherNode(new RouteUtils.Node("DUP23.binhl"), 0, 0, 0);
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                OutNode.PlaceAnotherNode(mcNodes[i], MainNetwork.nodes[i].x, MainNetwork.nodes[i].y, MainNetwork.nodes[i].z);

            }
            //LongCpoint

            for (int i = 0; i < Cpoints.Count; i++)
            {
                if (Cpoints[i].usedLayer >= 10)
                {
                    Cpoints[i].usedLayer -= 4;
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 12, PlaceLayer - 11] = "W";
                    if (Cpoints[i].indat)
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 12, PlaceLayer - 10] = "^";
                    else
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 12, PlaceLayer - 10] = "v";

                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 13, PlaceLayer - 11] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 13, PlaceLayer - 10] = "#";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 14, PlaceLayer - 11] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 14, PlaceLayer - 10] = "#";
                }
                for (int j = 0; j < Cpoints[i].usedLayer; j++)
                {
                    if (j <= 10)
                    {
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 1, PlaceLayer - j - 1] = "w";
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 1, PlaceLayer - j - 1 + 1] = "#";
                    }
                    else
                    {
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 4, PlaceLayer - j - 1] = "w";
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 4, PlaceLayer - j - 1 + 1] = "#";
                    }
                }
            }
            //PlaceReapiters
            for (int i = 0; i < MainNetwork.wires.Count; i++)
            {
                MCWires[i].PlaceRepeaters();
            }
            //SyncWires
            List<RouteUtils.Wire> WiresToSync = new List<RouteUtils.Wire>();
            for (int i = 0; i < MainNetwork.wires.Count; i++)
            {
                if (MainNetwork.wires[i].DistPort == "clk")
                {
                    WiresToSync.Add(MCWires[i]);
                }
            }
            int SyncLen = 0;
            for (int i = 0; i < WiresToSync.Count; i++)
            {
                if (WiresToSync[i].calcRepCount() > SyncLen)
                    SyncLen = WiresToSync[i].calcRepCount();
            }

            for (int i = 0; i < WiresToSync.Count; i++)
            {
                WiresToSync[i].repCompincate(SyncLen - WiresToSync[i].calcRepCount());
                WiresToSync[i].Synced = true;
            }


                //PlaceWires
                for (int i = 0; i < MainNetwork.wires.Count; i++)
                {
                    if (MainNetwork.wires[i].Placed)
                    {
                        for (int j = 0; j < MCWires[i].WirePointX.Length; j++)
                        {
                            if (MCWires[i].Synced)
                            {
                                OutNode.DataMatrix[MCWires[i].WirePointX[j], MCWires[i].WirePointY[j], MCWires[i].WirePointZ[j]] = "S";
                            }
                            else
                            {
                                OutNode.DataMatrix[MCWires[i].WirePointX[j], MCWires[i].WirePointY[j], MCWires[i].WirePointZ[j]] = "w";
                            }
                            if (MCWires[i].Rep[j])
                            {
                                OutNode.DataMatrix[MCWires[i].WirePointX[j], MCWires[i].WirePointY[j], MCWires[i].WirePointZ[j] + 1] = MCWires[i].RepNp[j];
                            }
                            else
                            {
                                OutNode.DataMatrix[MCWires[i].WirePointX[j], MCWires[i].WirePointY[j], MCWires[i].WirePointZ[j] + 1] = "#";
                            }
                        }
                    }
                }


                /*
                //PlaceDebuginfo
                int layer = 0;
                for (int x = 0; x < BaseSize; x++)
                {
                    for (int y = 0; y < BaseSize; y++)
                    {
                        if (WireMask[x, y] == "X")
                            OutNode.DataMatrix[x, y, layer] = "k";
                    }
                }

                for (int i = 0; i < WPX.Count; i++)
                {
                    OutNode.DataMatrix[WPX[i], WPY[i], layer] = "w";
                }
                */
                /*
                for (int j = 0; j < Cpoints.Count; j++)
                {
                    OutNode.DataMatrix[Cpoints[j].BaseX, Cpoints[j].BaseY, layer] = "k";
                    OutNode.DataMatrix[Cpoints[j].BaseX - 1, Cpoints[j].BaseY, layer] = "k";
                    OutNode.DataMatrix[Cpoints[j].BaseX + 1, Cpoints[j].BaseY, layer] = "k";
                }
                 */

                OutNode.export("test_D.binhl");


            /*
            List<Node> DupNodes = new List<Node>();
            RemoveDUPNodes(MainNetwork, DupNodes);
            RemoveDUOWires(MainNetwork, DupNodes);

            //Sorting Nodes;
            SortOptimize(MainNetwork);
            */

        }

        private static void PlaceWire(Mnet MainNetwork, int BaseSize, List<RouteUtils.Cpoint> Cpoints, int CurrentWireLayer, int CurrentRealLayer, int WireNum, RouteUtils.Wire[] MCWires, string[,] WireMask, out List<int> WPX, out List<int> WPY)
        {
            //WireMask = new string[BaseSize, BaseSize];

            //PlaceMaskCpoint
            for (int j = 0; j < Cpoints.Count; j++)
            {
                if (Cpoints[j].usedLayer == 0)
                DrawAtMask(WireMask, Cpoints[j].BaseX, Cpoints[j].BaseY + CurrentWireLayer, 1, 2);
            }

            Wire W = MainNetwork.wires[WireNum];

            RouteUtils.Wire MCW = new RouteUtils.Wire(W.SrcName + "-" + W.SrcPort, W.DistName + "-" + W.DistPort);
            //UnmaskStartEndPoint
            RouteUtils.Cpoint SP = FindCpoint(MCW.StartName, Cpoints);
            RouteUtils.Cpoint EP = FindCpoint(MCW.EndName, Cpoints);

            SP.BaseY += CurrentWireLayer;
            EP.BaseY += CurrentWireLayer;

            UnmaskCpoint(WireMask, SP);
            UnmaskCpoint(WireMask, EP);
            //CalcAstar
            int[,] AStarTable = CalcAstar(BaseSize, WireMask, SP);

            //DrawWire


            bool placed = TryPlaceWire(SP, EP, AStarTable, out WPX, out WPY);
            if (placed)
            {
                //WireRemask
                for (int i = 0; i < WPX.Count; i++)
                {
                    DrawAtMask(WireMask, WPX[i], WPY[i], 1, 1);
                }

                MCWires[WireNum] = new RouteUtils.Wire(MCW.StartName, MCW.EndName);

                MCWires[WireNum].WirePointX = new int[WPX.Count];
                MCWires[WireNum].WirePointY = new int[WPX.Count];
                MCWires[WireNum].WirePointZ = new int[WPX.Count];

                for (int i = 0; i < WPX.Count; i++)
                {
                    MCWires[WireNum].WirePointX[WPX.Count - i - 1] = WPX[i];
                    MCWires[WireNum].WirePointY[WPX.Count - i - 1] = WPY[i];
                    MCWires[WireNum].WirePointZ[WPX.Count - i - 1] = CurrentRealLayer;
                }
                MainNetwork.wires[WireNum].Placed = true;

                SP.usedLayer = CurrentWireLayer;
                EP.usedLayer = CurrentWireLayer;
            }
            SP.BaseY -= CurrentWireLayer;
            EP.BaseY -= CurrentWireLayer;
        }

        private static bool TryPlaceWire(RouteUtils.Cpoint SP, RouteUtils.Cpoint EP, int[,] AStarTable, out List<int> WPX, out List<int> WPY)
        {
            WPX = new List<int>();
            WPY = new List<int>();

            bool Wcalc = true;
            int tx = EP.BaseX;
            int ty = EP.BaseY;
            if (AStarTable[tx, ty] == 0)
                return false;
            while (Wcalc)
            {
                bool R = false;
                WPX.Add(tx);
                WPY.Add(ty);
                if (AStarTable[tx - 1, ty] < AStarTable[tx, ty] && AStarTable[tx - 1, ty] != 0 & !R)
                {
                    R = true;
                    tx = tx - 1;
                }

                if (AStarTable[tx + 1, ty] < AStarTable[tx, ty] && AStarTable[tx + 1, ty] != 0 & !R)
                {
                    R = true;
                    tx = tx + 1;
                }

                if (AStarTable[tx, ty + 1] < AStarTable[tx, ty] && AStarTable[tx, ty + 1] != 0 & !R)
                {
                    R = true;
                    ty = ty + 1;
                }

                if (AStarTable[tx, ty - 1] < AStarTable[tx, ty] && AStarTable[tx, ty - 1] != 0 & !R)
                {
                    R = true;
                    ty = ty - 1;
                }
                if (SP.BaseX == tx && SP.BaseY == ty)
                {
                    Wcalc = false;
                    WPX.Add(tx);
                    WPY.Add(ty);
                }
            }
            return true;
        }

        private static int[,] CalcAstar(int BaseSize, string[,] WireMask, RouteUtils.Cpoint SP)
        {
            int[,] AStarTable = new int[BaseSize, BaseSize];
            AStarTable[SP.BaseX, SP.BaseY] = 1;

            bool Calcing = true;

            while (Calcing)
            {
                int aded = 0;
                for (int x = 1; x < BaseSize - 1; x++)
                {
                    for (int y = 1; y < BaseSize - 1; y++)
                    {
                        if (AStarTable[x, y] != 0)
                        {
                            if (AStarTable[x + 1, y] == 0 && WireMask[x + 1, y] != "X")
                            {
                                AStarTable[x + 1, y] = AStarTable[x, y] + 1;
                                aded++;
                            }
                            if (AStarTable[x - 1, y] == 0 && WireMask[x - 1, y] != "X")
                            {
                                AStarTable[x - 1, y] = AStarTable[x, y] + 1;
                                aded++;
                            }
                            if (AStarTable[x, y - 1] == 0 && WireMask[x, y - 1] != "X")
                            {
                                AStarTable[x, y - 1] = AStarTable[x, y] + 1;
                                aded++;
                            }
                            if (AStarTable[x, y + 1] == 0 && WireMask[x, y + 1] != "X")
                            {
                                AStarTable[x, y + 1] = AStarTable[x, y] + 1;
                                aded++;
                            }
                        }
                    }
                }
                if (aded == 0) Calcing = false;
            }
            return AStarTable;
        }

        private static void UnmaskCpoint(string[,] WireMask, RouteUtils.Cpoint SP)
        {
            WireMask[SP.BaseX, SP.BaseY] = "";
            WireMask[SP.BaseX - 1, SP.BaseY] = "";
            WireMask[SP.BaseX + 1, SP.BaseY] = "";

            WireMask[SP.BaseX, SP.BaseY + 1] = "";
            WireMask[SP.BaseX - 1, SP.BaseY + 1] = "";
            WireMask[SP.BaseX + 1, SP.BaseY + 1] = "";
        }

        private static RouteUtils.Cpoint FindCpoint(string p,List<RouteUtils.Cpoint> CPnt)
        {
            for (int j = 0; j < CPnt.Count; j++)
            {
                if (CPnt[j].PointName == p)
                    return CPnt[j];
            }
            return null;
        }

        private static bool TryPlace(Mnet MainNetwork, RouteUtils.Node[] mcNodes, int PlaceLayer, int BaseSize)
        {
            string[,] PlaceMask = new string[BaseSize, BaseSize];

            //PlacePorts
            int potrtx = 1;
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                if (MainNetwork.nodes[i].NodeType.Contains("Port"))
                {
                    MainNetwork.nodes[i].x = potrtx;
                    MainNetwork.nodes[i].y = 1;
                    MainNetwork.nodes[i].z = PlaceLayer;
                    //DrawMask
                    int mx = MainNetwork.nodes[i].x;
                    int my = MainNetwork.nodes[i].y;
                    int mw = mcNodes[i].SizeX;
                    int mh = mcNodes[i].SizeY;

                    DrawAtMask(PlaceMask, mx, my, mw, mh+3);

                    potrtx += 4;
                }
            }

            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                bool placed = false;
                if (MainNetwork.nodes[i].NodeType.Contains("Port"))
                {
                    placed = true;
                }
                for (int x = 1; x < BaseSize; x++)
                {
                    for (int y = 1; y < BaseSize; y++)
                    {
                        if (!placed)
                        {
                            MainNetwork.nodes[i].x = x;
                            MainNetwork.nodes[i].y = y;
                            MainNetwork.nodes[i].z = PlaceLayer;

                            int mx = MainNetwork.nodes[i].x;
                            int my = MainNetwork.nodes[i].y;
                            int mw = mcNodes[i].SizeX;
                            int mh = mcNodes[i].SizeY;

                            if (CanPlace(PlaceMask, mx, my, mw, mh + 3))
                            {
                                DrawAtMask(PlaceMask, mx, my, mw, mh + 3);
                                placed = true;
                            }
                        }
                    }
                }
                if (!placed)
                    return false;
            }
            return true;
        }

        private static bool CanPlace(string[,] PlaceMask, int mx, int my, int mw, int mh)
        {
            mw++;
            mh++;

            for (int x = 0; x < mw; x++)
            {
                for (int y = 0; y < mh; y++)
                {
                    try
                    {
                        if (PlaceMask[mx + x, my + y] == "X") return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void DrawAtMask(string[,] PlaceMask, int mx, int my, int mw, int mh)
        {
            for (int x = -1; x < mw + 1; x++)
            {
                for (int y = -1; y < mh + 1; y++)
                {
                        PlaceMask[mx + x, my + y] = "X";
                }
            }
        }

        private static int CalcPortNum(Mnet MainNetwork)
        {
            int PortNum = 0;
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                if (MainNetwork.nodes[i].NodeType.Contains("Port"))
                {
                    PortNum++;
                }
            }
            return PortNum;
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
