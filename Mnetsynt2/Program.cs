using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnetsynt2
{
    enum PlaceModes
    {
        Compact1,
        Fast,
        Optimal
    }
    class Program
    {

        static int OptimiseDeep = 99;
        /// <summary>
        /// Разряженность
        /// </summary>
        static int dolled = 1;

        static PlaceModes placeMode = PlaceModes.Optimal;

        static bool WireOpt = true;
        static bool TypeSort = false;
        static bool SortObjectOpt = false;

        static void Main(string[] args)
        {
            //string file = "test_D_O";
            string file = "lut_0006_D_O";

            if (args.Length > 0)
            {
                file = args[0];
            }
            

            Mnet MainNetwork = new Mnet();
            MainNetwork.ReadMnetFile(file + @".MNET");
            if (SortObjectOpt)
            {
                Console.WriteLine("Оптимизация распалажения");
                SortOptimize(MainNetwork);
            }
            //ReducteDUP(MainNetwork);
            //loadnodes
            Console.WriteLine("Загрузка темплейтов");
            RouteUtils.Node[] mcNodes = new RouteUtils.Node[MainNetwork.nodes.Count];
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                mcNodes[i] = new RouteUtils.Node(MainNetwork.nodes[i].NodeType + ".binhl");
                mcNodes[i].NodeName = MainNetwork.nodes[i].NodeName;
                MainNetwork.nodes[i].mcNode = mcNodes[i];
            }
            Console.WriteLine("OK");
            //Place nodes
            int PlaceLayer = 0;
            int BaseSize = 0;

            //switch (placeMode) { }
            switch (placeMode)
            {
                case PlaceModes.Compact1:
                    PlaceCompact(MainNetwork, mcNodes, out PlaceLayer, out BaseSize);
                    break;
                case PlaceModes.Fast:
                    PlaceFast(MainNetwork, mcNodes, out PlaceLayer, out BaseSize);
                    break;
                case PlaceModes.Optimal:
                    PlaceOptimal(MainNetwork, mcNodes, out PlaceLayer, out BaseSize);
                    break;
                default:
                    break;
            }

            


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
                        Indat = true
                    });

                }

                for (int j = 0; j < mcNodes[i].OutPorts.Length; j++)
                {
                    Cpoints.Add(new RouteUtils.Cpoint
                    {
                        BaseX = mcNodes[i].OutPorts[j].PosX + MainNetwork.nodes[i].x,
                        BaseY = mcNodes[i].OutPorts[j].PosY + MainNetwork.nodes[i].y,
                        PointName = MainNetwork.nodes[i].NodeName + "-" + mcNodes[i].OutPorts[j].Name,
                        Indat = false
                    });

                }
            }

            SortWire(Cpoints,MainNetwork,BaseSize);

            int CurrentWireLayer = 1;
            int CurrentRealLayer = 0;
            int WireNum = 0;
            RouteUtils.Wire[] MCWires = new RouteUtils.Wire[MainNetwork.wires.Count];
            //Draw wires in layer
            int TotalLayers = 0;
            for (int j = 0; j < 24; j++)
            {

                CurrentWireLayer = j * 2 + 1;
                if (j > 4) CurrentWireLayer += 5;
                if (j > 9) CurrentWireLayer += 5;
                if (j > 14) CurrentWireLayer += 5;

                CurrentRealLayer = PlaceLayer - 1 - j * 2;

                if (j > 4) CurrentRealLayer -= 2;
                if (j > 9) CurrentRealLayer -= 2;
                if (j > 14) CurrentRealLayer -= 2;

                WireNum = 0;

                char[,] WireMask = new char[BaseSize, BaseSize];


                for (int i = 0; i < MainNetwork.wires.Count; i++)
                {

                    List<int> WPX;
                    List<int> WPY;

                    if (WireOpt)
                    {
                        int Twire = 0;
                        int mink = 999999;
                        int bestN = 0;

                        //TODO Попробовать распаралелить
                        for (int k = 0; k < MainNetwork.wires.Count; k++)
                        {
                            if (!MainNetwork.wires[k].Placed)
                            {
                                int bw = FindBestWireToRoute(MainNetwork, BaseSize, Cpoints, CurrentWireLayer, CurrentRealLayer, k, MCWires, WireMask);
                                if (mink > bw && bw != 0)
                                {
                                    mink = bw;
                                    bestN = k;
                                }
                            }
                        }
                        if (!MainNetwork.wires[bestN].Placed)
                        {
                            PlaceWire(MainNetwork, BaseSize, Cpoints, CurrentWireLayer, CurrentRealLayer, bestN, MCWires, WireMask, out WPX, out WPY);
                            Twire = bestN;
                            if (MainNetwork.wires[bestN].Placed)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                WireNum++;
                                Console.WriteLine(MainNetwork.wires[bestN].ToString());
                            }
                            else
                            {
                                i = MainNetwork.wires.Count;
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                        }
                        else
                        {
                            i = MainNetwork.wires.Count;
                        }

                    }
                    else
                    {
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
                }
                Console.WriteLine("Разведено в текущем слое:" + WireNum);
                if (WireNum > 0)
                {
                    TotalLayers++;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("Всего Слоев:" + TotalLayers);

            RouteUtils.Node OutNode = new RouteUtils.Node("OUT", BaseSize, BaseSize, PlaceLayer + 10);

            //OutNode.PlaceAnotherNode(new RouteUtils.Node("DUP23.binhl"), 0, 0, 0);
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                OutNode.PlaceAnotherNode(mcNodes[i], MainNetwork.nodes[i].x, MainNetwork.nodes[i].y, MainNetwork.nodes[i].z);

            }
            //LongCpoint

            for (int i = 0; i < Cpoints.Count; i++)
            {
                if (Cpoints[i].UsedLayer >= 10)
                {
                    Cpoints[i].UsedLayer -= 4;
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 12, PlaceLayer - 11] = "W";
                    if (Cpoints[i].Indat)
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 12, PlaceLayer - 10] = "^";
                    else
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 12, PlaceLayer - 10] = "v";

                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 13, PlaceLayer - 11] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 13, PlaceLayer - 10] = "#";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 14, PlaceLayer - 11] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 14, PlaceLayer - 10] = "#";
                }

                if (Cpoints[i].UsedLayer >= 22)
                {
                    Cpoints[i].UsedLayer -= 3;
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 27, PlaceLayer - 23] = "W";
                    if (Cpoints[i].Indat)
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 27, PlaceLayer - 22] = "^";
                    else
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 27, PlaceLayer - 22] = "v";

                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 28, PlaceLayer - 23] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 28, PlaceLayer - 22] = "#";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 29, PlaceLayer - 23] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 29, PlaceLayer - 22] = "#";
                }

                if (Cpoints[i].UsedLayer >= 34)
                {
                    Cpoints[i].UsedLayer -= 3;
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 42, PlaceLayer - 35] = "W";
                    if (Cpoints[i].Indat)
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 42, PlaceLayer - 34] = "^";
                    else
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 42, PlaceLayer - 34] = "v";

                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 43, PlaceLayer - 35] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 43, PlaceLayer - 34] = "#";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 44, PlaceLayer - 35] = "W";
                    OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + 44, PlaceLayer - 34] = "#";
                }


                for (int j = 0; j < Cpoints[i].UsedLayer; j++)
                {
                    if (j <= 10)
                    {
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 1, PlaceLayer - j - 1] = "w";
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 1, PlaceLayer - j - 0] = "#";
                    }
                    if (j > 10 && j <= 22)
                    {
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 4, PlaceLayer - j - 1] = "w";
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 4, PlaceLayer - j - 0] = "#";
                    }
                    if (j > 22 && j <= 34)
                    {
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 7, PlaceLayer - j - 1] = "w";
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 7, PlaceLayer - j - 0] = "#";
                    }

                    if (j > 34)
                    {
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 10, PlaceLayer - j - 1] = "w";
                        OutNode.DataMatrix[Cpoints[i].BaseX, Cpoints[i].BaseY + j + 10, PlaceLayer - j - 0] = "#";
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
                if (WiresToSync[i].CalcRepCount() > SyncLen)
                    SyncLen = WiresToSync[i].CalcRepCount();
            }

            for (int i = 0; i < WiresToSync.Count; i++)
            {
                WiresToSync[i].RepCompincate(SyncLen - WiresToSync[i].CalcRepCount());
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

                //Обрезка
                Console.WriteLine("Обрезка рабочей Облости");
                RouteUtils.Node OutNodeO = CutOutputNode(PlaceLayer, BaseSize, OutNode);
                //Маркировка портов ввода вывода
                OutNodeO.InPorts = MainNetwork.nodes.Where(t => t.NodeType == "INPort").Select(t => new RouteUtils.InPort(t.NodeName, t.x+1, t.y)).ToArray();
                OutNodeO.OutPorts = MainNetwork.nodes.Where(t => t.NodeType == "OUTPort").Select(t => new RouteUtils.OutPort(t.NodeName, t.x+1, t.y)).ToArray();

                Console.WriteLine("Экспорт");
                OutNodeO.Export( file + ".binhl");
        }

        private static void PlaceFast(Mnet MainNetwork, RouteUtils.Node[] mcNodes, out int PlaceLayer, out int BaseSize)
        {
            throw new NotImplementedException();
        }

        private static void PlaceOptimal(Mnet MainNetwork, RouteUtils.Node[] mcNodes, out int PlaceLayer, out int BaseSize)
        {
            //Расчитать baseSize
            PlaceLayer = 60;
            int xStep = mcNodes.Select(t => t.SizeX).Max() + dolled;
            int yStep = mcNodes.Select(t => t.SizeY).Max() + dolled;

            BaseSize = (new int[] { Convert.ToInt32(Math.Sqrt(mcNodes.Length)) * xStep, Convert.ToInt32(Math.Sqrt(mcNodes.Length)) * yStep}).Max() + 60;
            
            //Разместить порты
            var ports = MainNetwork.nodes.Where(t => t.NodeType.Contains("Port"));
            int lastxcoord = 1;
            foreach (var port in ports)
            {
                port.placed = true;
                port.x = lastxcoord;
                port.y = 1;
                port.z = PlaceLayer;
                lastxcoord += mcNodes.FirstOrDefault(t => t.NodeName == port.NodeName).SizeX + dolled;
            }
            //Расчитать матрицу связоности
            int[,] connectionMatrix = new int[MainNetwork.nodes.Count, MainNetwork.nodes.Count];
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                for (int j = 0; j < MainNetwork.nodes.Count; j++)
                {
                    connectionMatrix[i, j] = MainNetwork.wires.Where(t => t.SrcName == MainNetwork.nodes[i].NodeName && t.DistName == MainNetwork.nodes[j].NodeName ||
                                                                          t.SrcName == MainNetwork.nodes[j].NodeName && t.DistName == MainNetwork.nodes[i].NodeName).Count();
                }
            }

            //Разместить ноды в ячейки
            var unPlaced = MainNetwork.nodes.Where(t => !t.placed);
            while (unPlaced.Count() > 0)
            {
                //Поиск нода максимально связанного с установленными
                int[] localConnectionMatrix = new int[MainNetwork.nodes.Count];
                for (int i = 0; i < MainNetwork.nodes.Count; i++)
                {
                    localConnectionMatrix[i] += MainNetwork.wires.Where(t => !MainNetwork.nodes[i].placed &&
                                                                             (t.SrcName == MainNetwork.nodes[i].NodeName && MainNetwork.nodes.FirstOrDefault(n=>n.NodeName == t.DistName).placed)).Count();
                    localConnectionMatrix[i] += MainNetwork.wires.Where(t => !MainNetwork.nodes[i].placed &&
                                                                             (t.DistName == MainNetwork.nodes[i].NodeName && MainNetwork.nodes.FirstOrDefault(n => n.NodeName == t.SrcName).placed)).Count();
                }
                int maxConections = localConnectionMatrix.Max();
                Node nodeToPlace = new Node();
                for (int i = 0; i < MainNetwork.nodes.Count; i++)
                {
                    if (localConnectionMatrix[i] == maxConections)
                    {
                        nodeToPlace = MainNetwork.nodes[i];
                    }
                }
                if (maxConections == 0)
                {
                    nodeToPlace = unPlaced.FirstOrDefault();
                }
                //Поиск места для установки
                int[,] placeMatrix = new int[BaseSize,BaseSize];
                var connectionsa = MainNetwork.wires.Where(k => k.SrcName == nodeToPlace.NodeName).Select(s => MainNetwork.nodes.FirstOrDefault(q => s.DistName == q.NodeName)).Where(l => l.placed);
                var connectionsb = MainNetwork.wires.Where(k => k.DistName == nodeToPlace.NodeName).Select(s => MainNetwork.nodes.FirstOrDefault(q => s.SrcName == q.NodeName)).Where(l => l.placed);
                var connections = connectionsa.Union(connectionsb);
                var mcNode = mcNodes.FirstOrDefault(k => k.NodeName == nodeToPlace.NodeName);
                int sizex = mcNode.SizeX + dolled;
                int sizey = mcNode.SizeY + dolled;
                var placedNodes = MainNetwork.nodes.Where(t=>t.placed);


                int step = 5;
                
                char[,] mask = new char[BaseSize, BaseSize];
                foreach (var node in placedNodes)
                {
                    DrawAtMask(mask, node.x, node.y, node.mcNode.SizeX, node.mcNode.SizeY);
                }

                for (int i = 0; i < BaseSize; i+=step)
                {
                    for (int j = 0; j < BaseSize; j+=step)
                    {
                        //bool collision = placedNodes.Where(t => CheckCollision(t, t.mcNode, i, j, mcNode)).Count() > 0;
                        if (BaseSize > i + sizex + 1 && BaseSize > j + sizey + 1)
                            if(!CheckMaskCollision(mask, nodeToPlace,i,j))
                                {
                                    foreach (Node n in connections)
                                    {
                                        placeMatrix[i, j] += (n.x - i) * (n.x - i) + (n.y - j) * (n.y - j);
                                    }
                                }
                    }
                }
                //Установка
                int pcondMin = 90000000;
                int xplace = 0;
                int yplace = 0;
                for (int i = 1; i < BaseSize; i++)
                {
                    for (int j = 1; j < BaseSize; j++)
                    {
                        if (placeMatrix[i, j] < pcondMin && placeMatrix[i, j]>0)
                        {
                            xplace = i;
                            yplace = j;
                            pcondMin = placeMatrix[i, j];
                        }
                    }
                }
                nodeToPlace.x = xplace;
                nodeToPlace.y = yplace;
                nodeToPlace.z = PlaceLayer;
                nodeToPlace.placed = true;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("{0} - Осталось:{1}", nodeToPlace.NodeName, unPlaced.Count());
                Console.ForegroundColor = ConsoleColor.White;
             }
            //foreach(Node node in )

            BaseSize += PlaceLayer;
            //Debug Draw Image
            System.Drawing.Image im = new System.Drawing.Bitmap(BaseSize, BaseSize);
            System.Drawing.Graphics Gr = System.Drawing.Graphics.FromImage(im);
            Gr.Clear(System.Drawing.Color.Black);
            foreach(var node in MainNetwork.nodes)
            {
                Gr.DrawRectangle(System.Drawing.Pens.Green, node.x, node.y, node.mcNode.SizeX, node.mcNode.SizeY);
            }
            im.Save("1.png");

            //Увеличить плотность
            //throw new NotImplementedException();
        }

        private static bool CheckMaskCollision(char[,] mask, Node node,int x,int y)
        {
            for (int i = 0; i < node.mcNode.SizeX; i++)
            {
                for (int j = 0; j < node.mcNode.SizeY; j++)
                {
                    if (mask[x + i, y + j] == 'X') return true;
                }
            }
            return false;
        }

        private static bool CheckCollision(Node t, RouteUtils.Node node1, int i, int j, RouteUtils.Node node2)
        {
            bool result = false;
            result = result || CheckCollisionBoxAndPoint(t.x, t.y, node1.SizeX + dolled, node1.SizeY + dolled, i, j);
            result = result || CheckCollisionBoxAndPoint(t.x, t.y, node1.SizeX + dolled, node1.SizeY + dolled, i + node2.SizeX, j);
            result = result || CheckCollisionBoxAndPoint(t.x, t.y, node1.SizeX + dolled, node1.SizeY + dolled, i, j + node2.SizeY);
            result = result || CheckCollisionBoxAndPoint(t.x, t.y, node1.SizeX + dolled, node1.SizeY + dolled, i + node2.SizeX, j + node2.SizeY);

            
            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + dolled, node2.SizeY + dolled, t.x, t.y);
            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + dolled, node2.SizeY + dolled, t.x + node1.SizeX, t.y);
            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + dolled, node2.SizeY + dolled, t.x, t.y + node1.SizeY);
            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + dolled, node2.SizeY + dolled, t.x + node1.SizeX, t.y + node1.SizeY);
            
            return result;
        }

        private static bool CheckCollisionBoxAndPoint(int ox, int oy, int sx, int sy, int px, int py)
        {
            return (ox <= px && oy <= py && ox + sx >= px && oy + sy >= py);
        }



        private static void PlaceCompact(Mnet MainNetwork, RouteUtils.Node[] mcNodes, out int PlaceLayer, out int BaseSize)
        {
            PlaceLayer = 60;
            int PortNum = CalcPortNum(MainNetwork);

            BaseSize = 5 * PortNum;


            while (!TryPlace(MainNetwork, mcNodes, PlaceLayer, BaseSize))
            {
                BaseSize += 10;
                Console.WriteLine("Размер:" + BaseSize.ToString());
            }
            BaseSize += PlaceLayer;
        }

        private static int FindBestWireToRoute(Mnet MainNetwork, int BaseSize, List<RouteUtils.Cpoint> Cpoints, int CurrentWireLayer, int CurrentRealLayer, int WireNum, RouteUtils.Wire[] MCWires, char[,] WireMask)
        {
            for (int j = 0; j < Cpoints.Count; j++)
            {
                if (Cpoints[j].UsedLayer == 0)
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
            int[,] AStarTable = CalcAstar(BaseSize, WireMask, SP, EP);
            SP.BaseY -= CurrentWireLayer;
            EP.BaseY -= CurrentWireLayer;

            return AStarTable[EP.BaseX, EP.BaseY + CurrentWireLayer];
        }

        private static RouteUtils.Node CutOutputNode(int PlaceLayer, int BaseSize, RouteUtils.Node OutNode)
        {
            //find xs
            int xs = 0;
            int ys = 0;
            int zs = 0;
            for (int i = 1; i < BaseSize; i++)
            {
                int pn = 0;
                for (int y = 0; y < BaseSize; y++)
                {
                    for (int z = 0; z < (PlaceLayer + 10); z++)
                    {
                        if (OutNode.DataMatrix[i, y, z] != "0")
                        {
                            pn++;
                        }
                    }
                }
                if (pn != 0)
                {
                    xs = i - 1;
                    break;
                }
            }

            for (int i = 1; i < BaseSize; i++)
            {
                int pn = 0;
                for (int x = 0; x < BaseSize; x++)
                {
                    for (int z = 0; z < (PlaceLayer + 10); z++)
                    {
                        if (OutNode.DataMatrix[x, i, z] != "0")
                        {
                            pn++;
                        }
                    }
                }
                if (pn != 0)
                {
                    ys = i - 1;
                    break;
                }
            }

            for (int i = 1; i < (PlaceLayer + 10); i++)
            {
                int pn = 0;
                for (int x = 0; x < BaseSize; x++)
                {
                    for (int y = 0; y < BaseSize; y++)
                    {
                        if (OutNode.DataMatrix[x, y, i] != "0")
                        {
                            pn++;
                        }
                    }
                }
                if (pn != 0)
                {
                    zs = i - 1;
                    break;
                }
            }

            int xe = 0;
            int ye = 0;
            int ze = 0;
            for (int i = 1; i < BaseSize; i++)
            {
                int qi = BaseSize - 1 - i;
                int pn = 0;
                for (int y = 0; y < BaseSize; y++)
                {
                    for (int z = 0; z < (PlaceLayer + 10); z++)
                    {
                        if (OutNode.DataMatrix[qi, y, z] != "0")
                        {
                            pn++;
                        }
                    }
                }
                if (pn != 0)
                {
                    xe = qi + 2;
                    break;
                }
            }

            for (int i = 1; i < BaseSize; i++)
            {
                int qi = BaseSize - 1 - i;
                int pn = 0;
                for (int x = 0; x < BaseSize; x++)
                {
                    for (int z = 0; z < (PlaceLayer + 10); z++)
                    {
                        if (OutNode.DataMatrix[x, qi, z] != "0")
                        {
                            pn++;
                        }
                    }
                }
                if (pn != 0)
                {
                    ye = qi + 2;
                    break;
                }
            }

            for (int i = 1; i < (PlaceLayer + 10); i++)
            {
                int qi = (PlaceLayer + 10) - 1 - i;

                int pn = 0;
                for (int x = 0; x < BaseSize; x++)
                {
                    for (int y = 0; y < BaseSize; y++)
                    {
                        if (OutNode.DataMatrix[x, y, qi] != "0")
                        {
                            pn++;
                        }
                    }
                }
                if (pn != 0)
                {
                    ze = qi + 2;
                    break;
                }
            }

            RouteUtils.Node OutNodeO = new RouteUtils.Node("OUT", xe - xs, ye - ys, ze - zs);

            for (int x = xs; x < xe; x++)
            {
                for (int y = ys; y < ye; y++)
                {
                    for (int z = zs; z < ze; z++)
                    {
                        OutNodeO.DataMatrix[x - xs, y - ys, z - zs] = OutNode.DataMatrix[x, y, z];
                    }
                }
            }

            
            return OutNodeO;
        }

        private static void SortWire(List<RouteUtils.Cpoint> Cpoints, Mnet MainNetwork,int BW)
        {
            int Moves = 1;
            while (Moves > 0)
                Moves = 0;
                for (int i = 1; i < MainNetwork.wires.Count; i++)
                {
                    RouteUtils.Cpoint CA = FindCpoint(MainNetwork.wires[i].SrcName + "-" + MainNetwork.wires[i].SrcPort, Cpoints);
                    RouteUtils.Cpoint CB = FindCpoint(MainNetwork.wires[i - 1].SrcName + "-" + MainNetwork.wires[i - 1].SrcPort, Cpoints);
                    int ka = CA.BaseX + CA.BaseY * BW;
                    int kb = CB.BaseX + CB.BaseY * BW;
                    if (ka < kb)
                    {
                        Wire W = MainNetwork.wires[i];
                        MainNetwork.wires[i] = MainNetwork.wires[i - 1];
                        MainNetwork.wires[i - 1] = W;
                    }

                }
        }

       
        private static void PlaceWire(Mnet MainNetwork, int BaseSize, List<RouteUtils.Cpoint> Cpoints, int CurrentWireLayer, int CurrentRealLayer, int WireNum, RouteUtils.Wire[] MCWires, char[,] WireMask, out List<int> WPX, out List<int> WPY)
        {
            //WireMask = new string[BaseSize, BaseSize];

            //PlaceMaskCpoint
            for (int j = 0; j < Cpoints.Count; j++)
            {
                if (Cpoints[j].UsedLayer == 0)
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
            int[,] AStarTable = CalcAstar(BaseSize, WireMask, SP, EP);

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

                SP.UsedLayer = CurrentWireLayer;
                EP.UsedLayer = CurrentWireLayer;
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

        private static int[,] CalcAstar(int BaseSize, char[,] WireMask, RouteUtils.Cpoint SP, RouteUtils.Cpoint EP)
        {
            var AStarTable = new int[BaseSize, BaseSize];
            AStarTable[SP.BaseX, SP.BaseY] = 1;
            var lsx = new List<int>();
            var lsy = new List<int>();

            lsx.Add(SP.BaseX);
            lsy.Add(SP.BaseY);

            bool calcing = true;

            while (calcing)
            {
                int[] csx = lsx.ToArray();
                int[] csy = lsy.ToArray();

                lsx = new List<int>();
                lsy = new List<int>();

                int aded = 0;
                for (int i = 0; i < csx.Length; i++)
                {
                    int x = csx[i];
                    int y = csy[i];

                    if (x == EP.BaseX && y == EP.BaseY)
                        calcing = false;

                    if (x > 1 && x < BaseSize - 2 && y > 1 && y < BaseSize - 2)
                    {
                        if (AStarTable[x, y] != 0)
                        {
                            if (AStarTable[x + 1, y] == 0 && WireMask[x + 1, y] != 'X')
                            {
                                AStarTable[x + 1, y] = AStarTable[x, y] + 1;
                                aded++;
                                lsx.Add(x + 1);
                                lsy.Add(y);
                            }
                            if (AStarTable[x - 1, y] == 0 && WireMask[x - 1, y] != 'X')
                            {
                                AStarTable[x - 1, y] = AStarTable[x, y] + 1;
                                aded++;
                                lsx.Add(x - 1);
                                lsy.Add(y);
                            }
                            if (AStarTable[x, y - 1] == 0 && WireMask[x, y - 1] != 'X')
                            {
                                AStarTable[x, y - 1] = AStarTable[x, y] + 1;
                                aded++;
                                lsx.Add(x);
                                lsy.Add(y - 1);
                            }
                            if (AStarTable[x, y + 1] == 0 && WireMask[x, y + 1] != 'X')
                            {
                                AStarTable[x, y + 1] = AStarTable[x, y] + 1;
                                aded++;
                                lsx.Add(x);
                                lsy.Add(y + 1);
                            }
                        }
                    }
                }
                if (aded == 0) calcing = false;
            }
            return AStarTable;
        }

        private static void UnmaskCpoint(char[,] WireMask, RouteUtils.Cpoint SP)
        {
            WireMask[SP.BaseX, SP.BaseY] = ' ';
            WireMask[SP.BaseX - 1, SP.BaseY] = ' ';
            WireMask[SP.BaseX + 1, SP.BaseY] = ' ';

            WireMask[SP.BaseX, SP.BaseY + 1] = ' ';
            WireMask[SP.BaseX - 1, SP.BaseY + 1] = ' ';
            WireMask[SP.BaseX + 1, SP.BaseY + 1] = ' ';
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
            char[,] PlaceMask = new char[BaseSize, BaseSize];

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

            int lastY = 1;
            int lastX = 1;
            for (int i = 0; i < MainNetwork.nodes.Count; i++)
            {
                bool placed = false;
                if (MainNetwork.nodes[i].NodeType.Contains("Port"))
                {
                    placed = true;
                }

                for (int x = lastX; x < BaseSize; x += dolled)
                {
                    for (int y = 1; y < BaseSize; y += dolled)
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
                                lastX = x;
                                lastY = y;
                            }
                        }
                    }
                }

                if (!placed)
                    return false;
            }
            return true;
        }

        private static bool CanPlace(char[,] PlaceMask, int mx, int my, int mw, int mh)
        {
            mw++;
            mh++;

            for (int x = 0; x < mw; x++)
            {
                for (int y = 0; y < mh; y++)
                {
                    try
                    {
                        if (PlaceMask[mx + x, my + y] == 'X') return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void DrawAtMask(char[,] PlaceMask, int mx, int my, int mw, int mh)
        {
            for (int x = -1; x < mw + 1; x++)
            {
                for (int y = -1; y < mh + 1; y++)
                {
                        PlaceMask[mx + x, my + y] = 'X';
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
                if (list[i].NodeName == p)
                {
                    //if (list[i].NodeType == "OUTPort")
                    //    return list.Count + 1;
                    return i;
                }
                    
            }
            return 0;
        }

        private static void SortOptimize(Mnet MainNetwork)
        {
            for (int i = 0; i < MainNetwork.nodes.Count * OptimiseDeep; i++)
            {
                for (int j = 2; j < (MainNetwork.nodes.Count - 1); j++)
                {
                    if (MainNetwork.nodes[j].NodeType != "INPort" && MainNetwork.nodes[j].NodeType != "OUTPort")
                    {
                        List<Wire> Wlist1 = FindAllWiresTo(MainNetwork.wires, MainNetwork.nodes[j]);
                        //List<Wire> Wlist2 = FindAllWiresTo(MainNetwork.wires, MainNetwork.nodes[j+1]);
                        int ka = 0;
                        if (TypeSort)
                        {
                            int wa = CalcTypeWeight(MainNetwork.nodes[j]);
                            int wb = CalcTypeWeight(MainNetwork.nodes[j-1]);
                            if (wa > wb)
                            {
                                ka = 1;
                            }
                            else
                            {
                                ka = 0;
                            }
                        }
                        else
                        {
                            ka = CalcWireLens(Wlist1, MainNetwork);
                        }
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

        private static int CalcTypeWeight(Node node)
        {
            if (node.NodeType.StartsWith("DUP")) return 1;

            switch (node.NodeType)
            {
                case "TRIG_D":
                    return 5;
                case "AND":
                    return 3;
                case "NOT":
                    return 2;
                case "OR":
                    return 4;
                default:
                    return 0;
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
