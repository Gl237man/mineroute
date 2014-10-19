using System;
using System.Collections.Generic;
using System.Linq;
using RouteUtils;

namespace Mnetsynt3
{
    static class Program
    {
        private const int OptimiseDeep = 99;

        /// <summary>
        /// Разряженность
        /// </summary>
        private const int Dolled = 1;

        private const bool TypeSort = false;

        private static bool DrawAstar = false;

        static void Main(string[] args)
        {
            if (args.Contains("-debug"))
            {
                DrawAstar = true;
            }
            //string file = "test_D_O";
            string file = "lut_001C_D_O";

            if (args.Length > 0)
            {
                file = args[0];
            }


            var mainNetwork = new Mnet();
            mainNetwork.ReadMnetFile(file + @".MNET");
            

            mainNetwork.wireGroups = new List<WireGroup>();
            
            //Перевести соеденения DUP в группы
            Console.WriteLine("Конвертирование DUP");
            var dupList = mainNetwork.nodes.Where(t => t.NodeType.Contains("DUP")).ToList();

            //Обьеденение DUP
            List<Node> list = dupList;
            var dupdupWires = mainNetwork.wires.Where(t => list.FirstOrDefault(a => a.NodeName == t.SrcName) != null &&
                                                           list.FirstOrDefault(a => a.NodeName == t.DistName) != null);



            while (dupdupWires.Any())

            {
                var wire = dupdupWires.First();
                var firstDup = dupList.First(a => a.NodeName == wire.SrcName);
                var secondDup = dupList.First(a => a.NodeName == wire.DistName);
                var fromSecondWires = mainNetwork.wires.Where(t => t.SrcName == secondDup.NodeName).ToList();
                foreach (var w in fromSecondWires)
                {
                    w.SrcName = firstDup.NodeName;
                }
                mainNetwork.wires.Remove(wire);
                mainNetwork.nodes.Remove(secondDup);
            }

            dupList = mainNetwork.nodes.Where(t => t.NodeType.Contains("DUP")).ToList();

            var delinwirelist = new List<Wire>();
            var deloutwirelist = new List<Wire>();
            foreach (Node node in dupList)
            {
                string targetNodeName = node.NodeName;
                var inWires = mainNetwork.wires.Where(t => t.DistName == targetNodeName).ToList();
                var outWires = mainNetwork.wires.Where(t => t.SrcName == targetNodeName).ToList();
                foreach (var t in outWires)
                {
                    t.SrcName = inWires.First().SrcName;
                    t.SrcPort = inWires.First().SrcPort;
                }
                mainNetwork.wireGroups.Add(new WireGroup { GroupName = node.NodeName, WList = outWires });
                delinwirelist.AddRange(inWires);
                deloutwirelist.AddRange(outWires);
            }



            foreach (var t in delinwirelist) mainNetwork.wires.Remove(t);
            foreach (var t in dupList) mainNetwork.nodes.Remove(t);


            //loadnodes
            Console.WriteLine("Загрузка темплейтов");
            var mcNodes = new RouteUtils.Node[mainNetwork.nodes.Count];
            for (int i = 0; i < mainNetwork.nodes.Count; i++)
            {
                mcNodes[i] = new RouteUtils.Node(mainNetwork.nodes[i].NodeType + ".binhl")
                {
                    NodeName = mainNetwork.nodes[i].NodeName
                };
                mainNetwork.nodes[i].McNode = mcNodes[i];
            }
            Console.WriteLine("OK");
            //Place nodes
            int placeLayer;
            int baseSize;

           
            PlaceOptimal(mainNetwork, mcNodes, out placeLayer, out baseSize);
           

            foreach (var t in deloutwirelist) mainNetwork.wires.Remove(t);

            Console.WriteLine("Размещение ОК");

            var cpoints = new List<Cpoint>();

            //CreateCpointList

            for (int i = 0; i < mainNetwork.nodes.Count; i++)
            {
                cpoints.AddRange(mcNodes[i].InPorts.Select(t => new Cpoint
                {
                    BaseX = t.PosX + mainNetwork.nodes[i].X,
                    BaseY = t.PosY + mainNetwork.nodes[i].Y,
                    PointName = mainNetwork.nodes[i].NodeName + "-" + t.Name,
                    Indat = true
                }));

                cpoints.AddRange(mcNodes[i].OutPorts.Select(t => new Cpoint
                {
                    BaseX = t.PosX + mainNetwork.nodes[i].X,
                    BaseY = t.PosY + mainNetwork.nodes[i].Y,
                    PointName = mainNetwork.nodes[i].NodeName + "-" + t.Name,
                    Indat = false
                }));
            }

            SortWire(cpoints, mainNetwork, baseSize);

            var mcWires = new RouteUtils.Wire[mainNetwork.wires.Count];
            //Draw wires in layer
            int totalLayers = 0;
            for (int j = 0; j < 24; j++)
            {
                int currentWireLayer = j * 2 + 1;
                if (j > 4) currentWireLayer += 5;
                if (j > 9) currentWireLayer += 5;
                if (j > 14) currentWireLayer += 5;

                int currentRealLayer = placeLayer - 1 - j * 2;

                if (j > 4) currentRealLayer -= 2;
                if (j > 9) currentRealLayer -= 2;
                if (j > 14) currentRealLayer -= 2;

                int wireNum = 0;

                var wireMask = new char[baseSize, baseSize];

                //Разводка групп
                bool  canplace = true;
                while (canplace)
                {
                    //Разводка Группы

                    //Поиск лучшей группы для разводки
                    foreach (WireGroup group in mainNetwork.wireGroups)
                    {
                        if (!group.Placed)
                        {
                            group.weight = 0;
                            group.CanPlace = true;
                            foreach (Wire wire in group.WList)
                            {
                                //Поиск точек входа выхода
                                Cpoint startPoint = FindCpoint(wire.SrcName + "-" + wire.SrcPort, cpoints);
                                Cpoint endPoint = FindCpoint(wire.DistName + "-" + wire.DistPort, cpoints);



                                //Маскировка неиспользуемых точек;
                                foreach (Cpoint cpoint in cpoints)
                                {
                                    if (cpoint.UsedLayer == 0)
                                        DrawAtMask(wireMask, cpoint.BaseX, cpoint.BaseY + currentWireLayer, 1, 2);
                                }
                                //Отмена маскировки на конечных точках соеденения

                                startPoint.BaseY += currentWireLayer;
                                endPoint.BaseY += currentWireLayer;

                                UnmaskCpoint(wireMask, startPoint);
                                UnmaskCpoint(wireMask, endPoint);
                                var aStarTable = CalcAstar(baseSize, wireMask, startPoint, endPoint);

                                //CalcAstar

                                startPoint.BaseY -= currentWireLayer;
                                endPoint.BaseY -= currentWireLayer;
                                int weight = aStarTable[endPoint.BaseX, endPoint.BaseY + currentWireLayer];
                                if (weight == 0) group.CanPlace = false;
                                group.weight += weight;
                                wire.len = weight;
                                if (DrawAstar) RenderATable(wire + ".png", aStarTable, baseSize, startPoint, endPoint);

                            }
                        }
                    }

                    var bestGroup = mainNetwork.wireGroups.Where(q => q.CanPlace && !q.Placed).OrderBy(t => t.weight).FirstOrDefault();

                    //Разводка 

                    if (bestGroup != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Разводка группы {0} из {1} соеденений", bestGroup.GroupName,bestGroup.WList.Count);
                        Console.ForegroundColor = ConsoleColor.White;
                        bestGroup.Placed = true;
                        bestGroup.WList = bestGroup.WList.OrderBy(t => t.len).ToList();
                        bestGroup.WList.Reverse();
                        foreach (Wire wire in bestGroup.WList)
                        {
                            Cpoint startPoint = FindCpoint(wire.SrcName + "-" + wire.SrcPort, cpoints);
                            Cpoint endPoint = FindCpoint(wire.DistName + "-" + wire.DistPort, cpoints);

                            

                            //Маскировка неиспользуемых точек;
                            foreach (Cpoint cpoint in cpoints)
                            {
                                if (cpoint.UsedLayer == 0)
                                    DrawAtMask(wireMask, cpoint.BaseX, cpoint.BaseY + currentWireLayer, 1, 2);
                            }
                            //Отмена маскировки на конечных точках соеденения

                            startPoint.BaseY += currentWireLayer;
                            endPoint.BaseY += currentWireLayer;

                            UnmaskCpoint(wireMask, startPoint);
                            UnmaskCpoint(wireMask, endPoint);
                            //startPoint.BaseY += 1;
                            //endPoint.BaseY += 1;
                            //UnmaskCpoint(wireMask, startPoint);
                            //UnmaskCpoint(wireMask, endPoint);

                            var aStarTable = CalcAstar(baseSize, wireMask, startPoint, endPoint);
                            //MultiWareAstarUpdate(aStarTable, bestGroup);

                            List<int> wpx;
                            List<int> wpy;

                            bool placed = TryPlaceWire(startPoint, endPoint, aStarTable, out wpx, out wpy);
                            if (placed)
                            {
                                //WireRemask
                                wire.WirePoints = new List<WirePoint>();
                                for (int i = 0; i < wpx.Count; i++)
                                {
                                    wire.WirePoints.Add(new WirePoint { x = wpx[i], y = wpy[i], z = currentRealLayer });
                                }
                                wire.WirePoints.Reverse();
                                
                                startPoint.UsedLayer = currentWireLayer;
                                endPoint.UsedLayer = currentWireLayer;
                                wire.Placed = true;
                            }

                            //startPoint.BaseY -= 1;
                            //endPoint.BaseY -= 1;
                            startPoint.BaseY -= currentWireLayer;
                            endPoint.BaseY -= currentWireLayer;
                            wireNum++;
                            RenderATable(wire + ".png", aStarTable, baseSize, startPoint, endPoint);
                        }

                        foreach (Wire wire in bestGroup.WList)
                        {
                            foreach (var point in wire.WirePoints)
                            {
                                DrawAtMask(wireMask, point.x, point.y, 1, 1);
                            }
                        }
                    }
                    else
                    {
                        canplace = false;
                    }
                }

                //Разводка Соеденений

                for (int i = 0; i < mainNetwork.wires.Count; i++)
                {
                    int mink = 999999;
                    int bestN = 0;

                    for (int k = 0; k < mainNetwork.wires.Count; k++)
                    {
                        if (!mainNetwork.wires[k].Placed)
                        {
                            int bw = FindBestWireToRoute(mainNetwork, baseSize, cpoints, currentWireLayer, currentRealLayer, k, mcWires, wireMask);
                            if (mink > bw && bw != 0)
                            {
                                mink = bw;
                                bestN = k;
                            }
                        }
                    }
                    if (!mainNetwork.wires[bestN].Placed)
                    {
                        List<int> wpx;
                        List<int> wpy;
                        PlaceWire(mainNetwork, baseSize, cpoints, currentWireLayer, currentRealLayer, bestN, mcWires, wireMask, out wpx, out wpy);
                        if (mainNetwork.wires[bestN].Placed)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            wireNum++;
                            Console.WriteLine(mainNetwork.wires[bestN].ToString());
                        }
                        else
                        {
                            i = mainNetwork.wires.Count;
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    }
                    else
                    {
                        i = mainNetwork.wires.Count;
                    }
                }
                Console.WriteLine("Разведено в текущем слое:" + wireNum);
                if (wireNum > 0)
                {
                    totalLayers++;
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine("Всего Слоев:" + totalLayers);

            var outNode = new RouteUtils.Node("OUT", baseSize, baseSize, placeLayer + 10);

            //OutNode.PlaceAnotherNode(new RouteUtils.Node("DUP23.binhl"), 0, 0, 0);
            for (int i = 0; i < mainNetwork.nodes.Count; i++)
            {
                outNode.PlaceAnotherNode(mcNodes[i], mainNetwork.nodes[i].X, mainNetwork.nodes[i].Y, mainNetwork.nodes[i].Z);

            }
            //LongCpoint

            foreach (Cpoint cpoint in cpoints)
            {
                if (cpoint.UsedLayer >= 10)
                {
                    cpoint.UsedLayer -= 4;
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 12, placeLayer - 11] = "W";
                    if (cpoint.Indat)
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 12, placeLayer - 10] = "^";
                    else
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 12, placeLayer - 10] = "v";

                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 13, placeLayer - 11] = "W";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 13, placeLayer - 10] = "#";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 14, placeLayer - 11] = "W";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 14, placeLayer - 10] = "#";
                }

                if (cpoint.UsedLayer >= 22)
                {
                    cpoint.UsedLayer -= 3;
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 27, placeLayer - 23] = "W";
                    if (cpoint.Indat)
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 27, placeLayer - 22] = "^";
                    else
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 27, placeLayer - 22] = "v";

                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 28, placeLayer - 23] = "W";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 28, placeLayer - 22] = "#";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 29, placeLayer - 23] = "W";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 29, placeLayer - 22] = "#";
                }

                if (cpoint.UsedLayer >= 34)
                {
                    cpoint.UsedLayer -= 3;
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 42, placeLayer - 35] = "W";
                    if (cpoint.Indat)
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 42, placeLayer - 34] = "^";
                    else
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 42, placeLayer - 34] = "v";

                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 43, placeLayer - 35] = "W";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 43, placeLayer - 34] = "#";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 44, placeLayer - 35] = "W";
                    outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + 44, placeLayer - 34] = "#";
                }


                for (int j = 0; j < cpoint.UsedLayer; j++)
                {
                    if (j <= 10)
                    {
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 1, placeLayer - j - 1] = "w";
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 1, placeLayer - j - 0] = "#";
                    }
                    if (j > 10 && j <= 22)
                    {
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 4, placeLayer - j - 1] = "w";
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 4, placeLayer - j - 0] = "#";
                    }
                    if (j > 22 && j <= 34)
                    {
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 7, placeLayer - j - 1] = "w";
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 7, placeLayer - j - 0] = "#";
                    }

                    if (j > 34)
                    {
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 10, placeLayer - j - 1] = "w";
                        outNode.DataMatrix[cpoint.BaseX, cpoint.BaseY + j + 10, placeLayer - j - 0] = "#";
                    }
                }
            }
            //PlaceReapiters
            for (int i = 0; i < mainNetwork.wires.Count; i++)
            {
                mcWires[i].PlaceRepeaters();
            }

            //Установка репитеров для групп
            foreach (var group in mainNetwork.wireGroups)
            {
                group.PlaceRepitors();
            }
            //Установка групп соеденений

            //SyncWires
            var wiresToSync = new List<RouteUtils.Wire>();
            for (int i = 0; i < mainNetwork.wires.Count; i++)
            {
                if (mainNetwork.wires[i].DistPort == "clk")
                {
                    wiresToSync.Add(mcWires[i]);
                }
            }
            int syncLen = wiresToSync.Select(t => t.CalcRepCount()).Concat(new[] {0}).Max();

            foreach (RouteUtils.Wire wire in wiresToSync)
            {
                wire.RepCompincate(syncLen - wire.CalcRepCount());
                wire.Synced = true;
            }

            foreach (var group in mainNetwork.wireGroups)
            {
                foreach (var wire in group.WList)
                {
                    foreach (var point in wire.WirePoints)
                    {
                        outNode.DataMatrix[point.x, point.y, point.z] = "S";
                        if (wire.repError)
                            outNode.DataMatrix[point.x, point.y, point.z] = "W";

                        outNode.DataMatrix[point.x, point.y, point.z + 1] = "#";
                        if (point.Repiter)
                            outNode.DataMatrix[point.x, point.y, point.z + 1] = point.RepVapl;
                    }
                }
            }
            //отрисовка ошибочного соеденения
            foreach (var group in mainNetwork.wireGroups)
            {
                foreach (var wire in group.WList)
                {
                    if (wire.repError)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(wire);
                        Console.ForegroundColor = ConsoleColor.White;
                        foreach (var point in wire.WirePoints)
                        {
                            outNode.DataMatrix[point.x, point.y, point.z] = "S";
                            if (wire.repError)
                                outNode.DataMatrix[point.x, point.y, point.z] = "W";

                            outNode.DataMatrix[point.x, point.y, point.z + 1] = "#";
                            if (point.Repiter)
                                outNode.DataMatrix[point.x, point.y, point.z + 1] = point.RepVapl;
                        }
                    }
                }
            }

            //PlaceWires
            for (int i = 0; i < mainNetwork.wires.Count; i++)
            {
                if (mainNetwork.wires[i].Placed)
                {
                    for (int j = 0; j < mcWires[i].WirePointX.Length; j++)
                    {
                        if (mcWires[i].Synced)
                        {
                            outNode.DataMatrix[mcWires[i].WirePointX[j], mcWires[i].WirePointY[j], mcWires[i].WirePointZ[j]] = "S";
                        }
                        else
                        {
                            outNode.DataMatrix[mcWires[i].WirePointX[j], mcWires[i].WirePointY[j], mcWires[i].WirePointZ[j]] = "w";
                        }
                        if (mcWires[i].Rep[j])
                        {
                            outNode.DataMatrix[mcWires[i].WirePointX[j], mcWires[i].WirePointY[j], mcWires[i].WirePointZ[j] + 1] = mcWires[i].RepNp[j];
                        }
                        else
                        {
                            outNode.DataMatrix[mcWires[i].WirePointX[j], mcWires[i].WirePointY[j], mcWires[i].WirePointZ[j] + 1] = "#";
                        }
                    }
                }
            }

            //Обрезка
            Console.WriteLine("Обрезка рабочей Облости");
            var outNodeO = CutOutputNode(placeLayer, baseSize, outNode);
            //Маркировка портов ввода вывода
            outNodeO.InPorts = mainNetwork.nodes.Where(t => t.NodeType == "INPort").Select(t => new InPort(t.NodeName, t.X + 1, t.Y)).ToArray();
            outNodeO.OutPorts = mainNetwork.nodes.Where(t => t.NodeType == "OUTPort").Select(t => new OutPort(t.NodeName, t.X + 1, t.Y)).ToArray();

            Console.WriteLine("Экспорт");
            outNodeO.Export(file + ".binhl");
        }

        private static void MultiWareAstarUpdate(int[,] aStarTable, WireGroup bestGroup)
        {

            var placedWire = bestGroup.WList.Where(t => t.WirePoints != null).ToList();
            int updateVector = aStarTable.GetLength(0) * aStarTable.GetLength(0);
            for (int x = 0; x < aStarTable.GetLength(0); x++)
            {
                for (int y = 0; y < aStarTable.GetLength(0); y++)
                {
                    if (aStarTable[x, y] != 0)
                    {
                        aStarTable[x, y] += updateVector;
                    }
                }
            }

            foreach (var ware in placedWire )
            {
                foreach (var point in ware.WirePoints)
                {
                    aStarTable[point.x, point.y]-=2;
                }
            }
        }

        private static void RenderATable(string p, int[,] aStarTable, int size, Cpoint sp, Cpoint ep)
        {
            System.Drawing.Image im = new System.Drawing.Bitmap(size*10, size*10);
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(im);
            int max = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (aStarTable[i, j] > max) max = aStarTable[i, j];
                }
            }

            gr.Clear(System.Drawing.Color.Black);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (aStarTable[i, j] != 0)
                    {
                        const int r = 0;
                        int g = 255 - Convert.ToInt32(255.0f / max * aStarTable[i, j]);
                        int b = Convert.ToInt32(255.0f / max * aStarTable[i, j]);
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);
                        System.Drawing.Brush brush = new System.Drawing.SolidBrush(c);
                        gr.FillRectangle(brush, i * 10, j * 10, 10, 10);
                    }
                }
            }
            gr.FillRectangle(System.Drawing.Brushes.Red, sp.BaseX * 10, sp.BaseY * 10, 10, 10);
            gr.FillRectangle(System.Drawing.Brushes.Red, ep.BaseX * 10, ep.BaseY * 10, 10, 10);
            im.Save(p.Replace(":","_"));
        }

        private static void PlaceOptimal(Mnet mainNetwork, RouteUtils.Node[] mcNodes, out int PlaceLayer, out int BaseSize)
        {
            //Расчитать baseSize
            PlaceLayer = 60;
            int xStep = mcNodes.Select(t => t.SizeX).Max() + Dolled;
            int yStep = mcNodes.Select(t => t.SizeY).Max() + Dolled;

            BaseSize = (new int[] { Convert.ToInt32(Math.Sqrt(mcNodes.Length)) * xStep, Convert.ToInt32(Math.Sqrt(mcNodes.Length)) * yStep }).Max() + 60;

            //Разместить порты
            var ports = mainNetwork.nodes.Where(t => t.NodeType.Contains("Port"));
            int lastxcoord = 1;
            foreach (var port in ports)
            {
                port.Placed = true;
                port.X = lastxcoord;
                port.Y = 1;
                port.Z = PlaceLayer;
                lastxcoord += mcNodes.FirstOrDefault(t => t.NodeName == port.NodeName).SizeX + Dolled;
            }
            //Расчитать матрицу связоности
            int[,] connectionMatrix = new int[mainNetwork.nodes.Count, mainNetwork.nodes.Count];
            for (int i = 0; i < mainNetwork.nodes.Count; i++)
            {
                for (int j = 0; j < mainNetwork.nodes.Count; j++)
                {
                    connectionMatrix[i, j] = mainNetwork.wires.Where(t => t.SrcName == mainNetwork.nodes[i].NodeName && t.DistName == mainNetwork.nodes[j].NodeName ||
                                                                          t.SrcName == mainNetwork.nodes[j].NodeName && t.DistName == mainNetwork.nodes[i].NodeName).Count();
                }
            }

            //Разместить ноды в ячейки
            var unPlaced = mainNetwork.nodes.Where(t => !t.Placed);
            while (unPlaced.Count() > 0)
            {
                //Поиск нода максимально связанного с установленными
                int[] localConnectionMatrix = new int[mainNetwork.nodes.Count];
                for (int i = 0; i < mainNetwork.nodes.Count; i++)
                {
                    localConnectionMatrix[i] += mainNetwork.wires.Where(t => !mainNetwork.nodes[i].Placed &&
                                                                             (t.SrcName == mainNetwork.nodes[i].NodeName && mainNetwork.nodes.FirstOrDefault(n => n.NodeName == t.DistName).Placed)).Count();
                    localConnectionMatrix[i] += mainNetwork.wires.Where(t => !mainNetwork.nodes[i].Placed &&
                                                                             (t.DistName == mainNetwork.nodes[i].NodeName && mainNetwork.nodes.FirstOrDefault(n => n.NodeName == t.SrcName).Placed)).Count();
                }
                int maxConections = localConnectionMatrix.Max();
                Node nodeToPlace = new Node();
                for (int i = 0; i < mainNetwork.nodes.Count; i++)
                {
                    if (localConnectionMatrix[i] == maxConections)
                    {
                        nodeToPlace = mainNetwork.nodes[i];
                    }
                }
                if (maxConections == 0)
                {
                    nodeToPlace = unPlaced.FirstOrDefault();
                }
                //Поиск места для установки
                int[,] placeMatrix = new int[BaseSize, BaseSize];
                var connectionsa = mainNetwork.wires.Where(k => k.SrcName == nodeToPlace.NodeName).Select(s => mainNetwork.nodes.FirstOrDefault(q => s.DistName == q.NodeName)).Where(l => l.Placed);
                var connectionsb = mainNetwork.wires.Where(k => k.DistName == nodeToPlace.NodeName).Select(s => mainNetwork.nodes.FirstOrDefault(q => s.SrcName == q.NodeName)).Where(l => l.Placed);
                var connections = connectionsa.Union(connectionsb);
                var mcNode = mcNodes.FirstOrDefault(k => k.NodeName == nodeToPlace.NodeName);
                int sizex = mcNode.SizeX + Dolled;
                int sizey = mcNode.SizeY + Dolled;
                var placedNodes = mainNetwork.nodes.Where(t => t.Placed);


                int step = 5;

                char[,] mask = new char[BaseSize, BaseSize];
                foreach (var node in placedNodes)
                {
                    DrawAtMask(mask, node.X, node.Y, node.McNode.SizeX, node.McNode.SizeY);
                }

                for (int i = 0; i < BaseSize; i += step)
                {
                    for (int j = 0; j < BaseSize; j += step)
                    {
                        //bool collision = placedNodes.Where(t => CheckCollision(t, t.mcNode, i, j, mcNode)).Count() > 0;
                        if (BaseSize > i + sizex + 1 && BaseSize > j + sizey + 1)
                            if (!CheckMaskCollision(mask, nodeToPlace, i, j))
                            {
                                foreach (Node n in connections)
                                {
                                    placeMatrix[i, j] += (n.X - i) * (n.X - i) + (n.Y - j) * (n.Y - j);
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
                        if (placeMatrix[i, j] < pcondMin && placeMatrix[i, j] > 0)
                        {
                            xplace = i;
                            yplace = j;
                            pcondMin = placeMatrix[i, j];
                        }
                    }
                }
                nodeToPlace.X = xplace;
                nodeToPlace.Y = yplace;
                nodeToPlace.Z = PlaceLayer;
                nodeToPlace.Placed = true;
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
            foreach (var node in mainNetwork.nodes)
            {
                Gr.DrawRectangle(System.Drawing.Pens.Green, node.X, node.Y, node.McNode.SizeX, node.McNode.SizeY);
            }
            im.Save("1.png");

            //Увеличить плотность
            //throw new NotImplementedException();
        }

        private static bool CheckMaskCollision(char[,] mask, Node node, int x, int y)
        {
            for (int i = 0; i < node.McNode.SizeX; i++)
            {
                for (int j = 0; j < node.McNode.SizeY; j++)
                {
                    if (mask[x + i, y + j] == 'X') return true;
                }
            }
            return false;
        }

        private static bool CheckCollision(Node t, RouteUtils.Node node1, int i, int j, RouteUtils.Node node2)
        {
            bool result = false;
            result = result || CheckCollisionBoxAndPoint(t.X, t.Y, node1.SizeX + Dolled, node1.SizeY + Dolled, i, j);
            result = result || CheckCollisionBoxAndPoint(t.X, t.Y, node1.SizeX + Dolled, node1.SizeY + Dolled, i + node2.SizeX, j);
            result = result || CheckCollisionBoxAndPoint(t.X, t.Y, node1.SizeX + Dolled, node1.SizeY + Dolled, i, j + node2.SizeY);
            result = result || CheckCollisionBoxAndPoint(t.X, t.Y, node1.SizeX + Dolled, node1.SizeY + Dolled, i + node2.SizeX, j + node2.SizeY);


            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + Dolled, node2.SizeY + Dolled, t.X, t.Y);
            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + Dolled, node2.SizeY + Dolled, t.X + node1.SizeX, t.Y);
            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + Dolled, node2.SizeY + Dolled, t.X, t.Y + node1.SizeY);
            result = result || CheckCollisionBoxAndPoint(i, j, node2.SizeX + Dolled, node2.SizeY + Dolled, t.X + node1.SizeX, t.Y + node1.SizeY);

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

        private static int FindBestWireToRoute(Mnet MainNetwork, int BaseSize, List<RouteUtils.Cpoint> Cpoints, int CurrentWireLayer, int CurrentRealLayer, int WireNum, RouteUtils.Wire[] MCWires, char[,] wireMask)
        {
            for (int j = 0; j < Cpoints.Count; j++)
            {
                if (Cpoints[j].UsedLayer == 0)
                    DrawAtMask(wireMask, Cpoints[j].BaseX, Cpoints[j].BaseY + CurrentWireLayer, 1, 2);
            }

            Wire W = MainNetwork.wires[WireNum];

            RouteUtils.Wire MCW = new RouteUtils.Wire(W.SrcName + "-" + W.SrcPort, W.DistName + "-" + W.DistPort);
            //UnmaskStartEndPoint
            RouteUtils.Cpoint SP = FindCpoint(MCW.StartName, Cpoints);
            RouteUtils.Cpoint EP = FindCpoint(MCW.EndName, Cpoints);

            SP.BaseY += CurrentWireLayer;
            EP.BaseY += CurrentWireLayer;

            UnmaskCpoint(wireMask, SP);
            UnmaskCpoint(wireMask, EP);
            //CalcAstar
            int[,] AStarTable = CalcAstar(BaseSize, wireMask, SP, EP);
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

        private static void SortWire(List<RouteUtils.Cpoint> Cpoints, Mnet MainNetwork, int BW)
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


        private static void PlaceWire(Mnet MainNetwork, int BaseSize, List<RouteUtils.Cpoint> Cpoints, int CurrentWireLayer, int CurrentRealLayer, int WireNum, RouteUtils.Wire[] MCWires, char[,] wireMask, out List<int> WPX, out List<int> WPY)
        {
            //wireMask = new string[BaseSize, BaseSize];

            //PlaceMaskCpoint
            for (int j = 0; j < Cpoints.Count; j++)
            {
                if (Cpoints[j].UsedLayer == 0)
                    DrawAtMask(wireMask, Cpoints[j].BaseX, Cpoints[j].BaseY + CurrentWireLayer, 1, 2);
            }

            Wire W = MainNetwork.wires[WireNum];

            RouteUtils.Wire MCW = new RouteUtils.Wire(W.SrcName + "-" + W.SrcPort, W.DistName + "-" + W.DistPort);
            //UnmaskStartEndPoint
            RouteUtils.Cpoint SP = FindCpoint(MCW.StartName, Cpoints);
            RouteUtils.Cpoint EP = FindCpoint(MCW.EndName, Cpoints);

            SP.BaseY += CurrentWireLayer;
            EP.BaseY += CurrentWireLayer;

            UnmaskCpoint(wireMask, SP);
            UnmaskCpoint(wireMask, EP);
            //CalcAstar
            int[,] AStarTable = CalcAstar(BaseSize, wireMask, SP, EP);

            //DrawWire


            bool placed = TryPlaceWire(SP, EP, AStarTable, out WPX, out WPY);
            if (placed)
            {
                //WireRemask
                for (int i = 0; i < WPX.Count; i++)
                {
                    DrawAtMask(wireMask, WPX[i], WPY[i], 1, 1);
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
                //Поиск минимальной точки
                var ways = new int[4];

                ways[0] = AStarTable[tx - 1, ty] - AStarTable[tx, ty];
                ways[1] = AStarTable[tx + 1, ty] - AStarTable[tx, ty];
                ways[2] = AStarTable[tx, ty + 1] - AStarTable[tx, ty];
                ways[3] = AStarTable[tx, ty - 1] - AStarTable[tx, ty];
                if (AStarTable[tx - 1, ty] == 0) ways[0] = -9000;
                if (AStarTable[tx + 1, ty] == 0) ways[1] = -9000;
                if (AStarTable[tx, ty + 1] == 0) ways[2] = -9000;
                if (AStarTable[tx, ty - 1] == 0) ways[3] = -9000;

                int bestWay = 0;
                int bestVal = 9000;
                for (int i = 0; i < ways.Length; i++)
                {
                    if (ways[i] <= bestVal)
                    {
                        if (ways[i] != -9000)
                        {
                            bestWay = i;
                            bestVal = ways[i];
                        }
                    }
                }

                if (bestWay == 0) tx = tx - 1;
                if (bestWay == 1) tx = tx + 1;
                if (bestWay == 2) ty = ty + 1;
                if (bestWay == 3) ty = ty - 1;
                /*
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
                */
                if (SP.BaseX == tx && SP.BaseY == ty)
                {
                    Wcalc = false;
                    WPX.Add(tx);
                    WPY.Add(ty);
                }
            }
            return true;
        }

        private static int[,] CalcAstar(int BaseSize, char[,] wireMask, RouteUtils.Cpoint SP, RouteUtils.Cpoint EP)
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
                            if (AStarTable[x + 1, y] == 0 && wireMask[x + 1, y] != 'X')
                            {
                                AStarTable[x + 1, y] = AStarTable[x, y] + 1;
                                aded++;
                                lsx.Add(x + 1);
                                lsy.Add(y);
                            }
                            if (AStarTable[x - 1, y] == 0 && wireMask[x - 1, y] != 'X')
                            {
                                AStarTable[x - 1, y] = AStarTable[x, y] + 1;
                                aded++;
                                lsx.Add(x - 1);
                                lsy.Add(y);
                            }
                            if (AStarTable[x, y - 1] == 0 && wireMask[x, y - 1] != 'X')
                            {
                                AStarTable[x, y - 1] = AStarTable[x, y] + 1;
                                aded++;
                                lsx.Add(x);
                                lsy.Add(y - 1);
                            }
                            if (AStarTable[x, y + 1] == 0 && wireMask[x, y + 1] != 'X')
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

        private static void UnmaskCpoint(char[,] wireMask, RouteUtils.Cpoint SP)
        {
            wireMask[SP.BaseX, SP.BaseY] = ' ';
            wireMask[SP.BaseX - 1, SP.BaseY] = ' ';
            wireMask[SP.BaseX + 1, SP.BaseY] = ' ';

            wireMask[SP.BaseX, SP.BaseY + 1] = ' ';
            wireMask[SP.BaseX - 1, SP.BaseY + 1] = ' ';
            wireMask[SP.BaseX + 1, SP.BaseY + 1] = ' ';
        }

        private static RouteUtils.Cpoint FindCpoint(string p, List<RouteUtils.Cpoint> CPnt)
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
                    MainNetwork.nodes[i].X = potrtx;
                    MainNetwork.nodes[i].Y = 1;
                    MainNetwork.nodes[i].Z = PlaceLayer;
                    //DrawMask
                    int mx = MainNetwork.nodes[i].X;
                    int my = MainNetwork.nodes[i].Y;
                    int mw = mcNodes[i].SizeX;
                    int mh = mcNodes[i].SizeY;

                    DrawAtMask(PlaceMask, mx, my, mw, mh + 3);

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

                for (int x = lastX; x < BaseSize; x += Dolled)
                {
                    for (int y = 1; y < BaseSize; y += Dolled)
                    {
                        if (!placed)
                        {
                            MainNetwork.nodes[i].X = x;
                            MainNetwork.nodes[i].Y = y;
                            MainNetwork.nodes[i].Z = PlaceLayer;

                            int mx = MainNetwork.nodes[i].X;
                            int my = MainNetwork.nodes[i].Y;
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
                            int wb = CalcTypeWeight(MainNetwork.nodes[j - 1]);
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
