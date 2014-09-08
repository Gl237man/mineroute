using System;
using System.Collections.Generic;
using RouteUtils;

namespace MNetSynt
{
    class Program
    {
        static List<Node> Nlist;
        static List<string> Uname;
        static List<int> CoordX;
        static List<int> CoordY;
        static List<int> PortNum;
        static List<Wire> Wlist;
        static int[] Wprior;
        static string[, ,] mask;
        static int[, ,] wavemap;

        static int SizeX;
        static int SizeY;

        static int restartnum = 0;
        static int SZMod = -50;
        static bool Vmode = false;
        static int  VsizeMod = -12;
        static void Main(string[] args)
        {
            string FileName = "";
            if (args.Length < 1)
            {
                FileName = "tm";
            }
            else
            {
                FileName = args[0];
            }

            int placewidch = 2;
            //ReadNet
            string[] InFileStr = System.IO.File.ReadAllLines(FileName + ".MNET");
            Nlist = new List<Node>();
            Wlist = new List<Wire>();
            Uname = new List<string>();

            for (int i = 0; i < InFileStr.Length; i++)
            {
                string[] IfDat = InFileStr[i].Split(':');
                if (IfDat[0] == "NODE")
                {
                    Nlist.Add(new Node(IfDat[1] + ".binhl"));
                    Uname.Add(IfDat[2]);
                }
                if (IfDat[0] == "WIRE")
                {
                    Wlist.Add(new Wire(IfDat[1], IfDat[2]));
                }
            }

            Wprior = new int[Wlist.Count];

            for (int i = 0; i < Wprior.Length; i++)
            {
                Wprior[i] = 0;
            }

        restart:
            Console.WriteLine("SZMod " + SZMod.ToString());
            Console.WriteLine("VsizeMod " + VsizeMod.ToString());
            Console.WriteLine("placewidch " + placewidch.ToString());

            restartnum++;
            Console.Write(".");
            if (Wprior[0] > Wprior.Length)
            {
                for (int i = 0; i < Wprior.Length; i++)
                {
                    //Wprior[i] = 0;
                    Console.Write("ERRR");
                }
                placewidch++;
                VsizeMod++;
            }


            if (restartnum > 100)
            {
                
                restartnum = 0;
                if (Vmode)
                {
                    VsizeMod++; 
                }
                else
                {
                    SZMod++;
                }
            }
            //wlist sort
            for (int i = 0; i < Wprior.Length; i++)
            {
                for (int j = 1; j < Wprior.Length; j++)
                {
                    if (Wprior[j - 1] < Wprior[j])
                    {
                        int t0 = Wprior[j];
                        Wprior[j] = Wprior[j - 1];
                        Wprior[j - 1] = t0;
                        Wire t1 = Wlist[j];
                        Wlist[j] = Wlist[j - 1];
                        Wlist[j - 1] = t1;
                    }
                }
            }

            //Placer
            int maxx = 0;
            int maxy = 0;
            int maxports = 0;
            int NotPortNodeNum = 0;
            int outpnum = 0;
            int inpnum = 0;

            for (int i = 0; i < Nlist.Count; i++)
            {
                if (Nlist[i].SizeX > maxx)
                {
                    maxx = Nlist[i].SizeX;
                }
                if (Nlist[i].SizeY > maxy)
                {
                    maxy = Nlist[i].SizeY;
                }
                if (Nlist[i].InPorts.Length > maxports)
                {
                    maxports = Nlist[i].InPorts.Length;
                }
                if (Nlist[i].OutPorts.Length > maxports)
                {
                    maxports = Nlist[i].OutPorts.Length;
                }
                if (Nlist[i].Name != "INPort")
                {
                    if (Nlist[i].Name != "OUTPort")
                    {
                        NotPortNodeNum++;
                    }
                }

                if (Nlist[i].Name == "INPort")
                {
                    inpnum++;
                }
                if (Nlist[i].Name == "OUTPort")
                {
                    outpnum++;
                }

            }


            if (!Vmode)
            {
                SizeX = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1) * (maxx + maxports * 2) + inpnum * 2 + outpnum * 2 + SZMod;
                SizeY = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1) * (maxy + maxports * 2) + inpnum * 2 + outpnum * 2 + SZMod;
                if (SizeY < 1 || SizeX < 1)
                {
                    SZMod++;
                   //goto restart;
                }
            }
            else
            {
                SizeX = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1) * (maxx + maxports * 2) + inpnum * 2 + outpnum * 2 + SZMod;
                SizeY = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1) * (maxy + maxports * 2) + inpnum * 2 + outpnum * 2 + SZMod + VsizeMod;
                if (SizeY < 1 || SizeX < 1)
                {
                    VsizeMod++;
                    goto restart;
                }
            }
            SizeX = 128;
            SizeY = 256;
            int pmax = Convert.ToInt32(Math.Sqrt(NotPortNodeNum));
            //int px = 0;
            //int py = 0;

            CoordX = new List<int>();
            CoordY = new List<int>();

            int tipn = 0;
            int topn = 0;

            PortNum = new List<int>();

            //MultiLineSort
            int[] RyadList = new int[Uname.Count];
            PovtorSort:
            int[] KFList = new int[Uname.Count];
            int kz = 0;
            string s = Uname[0];

            for (int i = 0; i < Wlist.Count; i++)
            {
                string Sname = Wlist[i].StartName.Split('-')[0];
                string Ename = Wlist[i].EndName.Split('-')[0];
                //finding Sindex
                int Sidx = 0;
                int Eidx = 0;
                for (int j = 0; j < Uname.Count; j++)
                {
                    if (Uname[j] == Sname)
                    {
                        Sidx = j;
                    }
                    if (Uname[j] == Ename)
                    {
                        Eidx = j;
                    }
                }
                if (Nlist[Sidx].Name != "INPort")
                {
                    if (Nlist[Eidx].Name != "OUTPort")
                    {
                        if (Sidx != Eidx)
                        {
                            if (RyadList[Sidx] == RyadList[Eidx])
                            {
                                KFList[Eidx]++;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < Nlist.Count; i++)
            {
                if (Nlist[i].InPorts.Length <= KFList[i]*2)
                {
                    kz++;
                    RyadList[i]++;
                }
            }
            if (kz > 0) goto PovtorSort;



            int RyadNum = Max(RyadList)+1;
            
            for (int i = 0; i < Nlist.Count; i++)
            {
                if (Nlist[i].Name != "INPort")
                {
                    if (Nlist[i].Name != "OUTPort")
                    {
                        CoordX.Add(0);
                        CoordY.Add(0);
                        PortNum.Add(0);
                    }
                }
                if (Nlist[i].Name == "INPort")
                {

                    CoordX.Add(tipn * 2 + 3);
                    CoordY.Add(0);
                    PortNum.Add(tipn);
                    tipn++;
                }
                if (Nlist[i].Name == "OUTPort")
                {
                    CoordX.Add(topn * 2 + 3);
                    CoordY.Add(SizeY - 3);
                    PortNum.Add(topn);
                    topn++;
                }
            }
            /*
            for (int i = 0; i < Nlist.Count; i++)
            {
                for (int j = 0; j < Nlist[i].InPorts.Length; j++)
                {
                    for (int k = 0; k < Uname.Count; k++)
                    {
                        if (k != i)
                        {
                            //if (Uname[k] == Wlist[i].StartName.Split('-')[0]
                        }
                    }
                }
            }
            */
            //MultiLinePlace

            int[] INPortInRyad = new int[RyadNum];
            int[] OUTPortInRyad = new int[RyadNum];
            int[] MaxVsizeObjInRyad = new int[RyadNum];
            int[] MinVsizeObjInRyad = new int[RyadNum];
            int INPortNum = 0;
            int OUTPortNum = 0;
            //pass1
            int Tline = 3 + tipn * 2 + 1 +2;

            for (int i = 0; i < RyadNum; i++)
            {
                MinVsizeObjInRyad[i] = 9999999;
                int lastx = 2;
                int n = 0;
                for (int j = 0; j < Nlist.Count; j++)
                {
                    
                    if (RyadList[j] == i)
                    {
                        if (Nlist[j].Name != "INPort")
                        {
                            if (Nlist[j].Name != "OUTPort")
                            {

                                CoordX[j] = lastx;
                                CoordY[j] = Tline;//(RyadList[j] + 1) * 40;
                                lastx += Nlist[j].SizeX + n;
                                INPortInRyad[i] += Nlist[j].InPorts.Length;
                                OUTPortInRyad[i] += Nlist[j].OutPorts.Length;
                                if (MaxVsizeObjInRyad[i] < Nlist[j].SizeY) MaxVsizeObjInRyad[i] = Nlist[j].SizeY;
                                if (MinVsizeObjInRyad[i] > Nlist[j].SizeY) MinVsizeObjInRyad[i] = Nlist[j].SizeY;
                                //n--;
                            }
                        }
                        if (Nlist[j].Name == "INPort")
                        {
                            INPortNum++;
                        }
                        if (Nlist[j].Name == "OUTPort")
                        {
                            OUTPortNum++;
                        }
                    }
                }
            }
            //pass 2
            for (int i = 0; i < RyadNum; i++)
            {
                MinVsizeObjInRyad[i] = 9999999;
                int lastx = 2;
                int n = 0;
                for (int j = 0; j < Nlist.Count; j++)
                {

                    if (RyadList[j] == i)
                    {
                        if (Nlist[j].Name != "INPort")
                        {
                            if (Nlist[j].Name != "OUTPort")
                            {

                                CoordX[j] = lastx;
                                CoordY[j] = Tline + 0 + INPortInRyad[i] * 2 ;//(RyadList[j] + 1) * 40;
                                lastx += Nlist[j].SizeX + n;
                                //INPortInRyad[i] += Nlist[j].InPorts.Length;
                                //OUTPortInRyad[i] += Nlist[j].OutPorts.Length;
                                //if (MaxVsizeObjInRyad[i] < Nlist[j].SizeY) MaxVsizeObjInRyad[i] = Nlist[j].SizeY;
                                //if (MinVsizeObjInRyad[i] > Nlist[j].SizeY) MinVsizeObjInRyad[i] = Nlist[j].SizeY;
                                //n--;
                            }
                        }
                        
                    }
                }
                Tline += INPortInRyad[i] * 2 + MaxVsizeObjInRyad[i] + OUTPortInRyad[i] * 2 + 2;
            }
            SizeY = Tline + OUTPortNum * 2 + 4;
            SizeX = SizeY;
            //SizeY = 128;
            for (int i = 0; i < Nlist.Count; i++)
            {
                if (Nlist[i].Name == "OUTPort")
                {
                    CoordY[i] = SizeY - 3;
                }
            }

            /*
            for (int i = 0; i < Nlist.Count; i++)
            {
                if (Nlist[i].Name != "INPort")
                {
                    if (Nlist[i].Name != "OUTPort")
                    {
                        PortNum.Add(0);
                        CoordX.Add(px * ((maxx + maxports * 2)+placewidch)+6);
                        CoordY.Add(py * ((maxy + maxports * 2)+placewidch) + maxports * 2 + inpnum * 2+6);
                        px++;
                        if (px > pmax)
                        {
                            px = 0;
                            py++;
                        }
                    }
                }



                if (Nlist[i].Name == "INPort")
                {
                    
                    CoordX.Add(tipn * 2+3);
                    CoordY.Add(0);
                    PortNum.Add(tipn);
                    tipn++;
                }
                if (Nlist[i].Name == "OUTPort")
                {
                    CoordX.Add(topn * 2+3);
                    CoordY.Add(SizeY - 3);
                    PortNum.Add(topn);
                    topn++;
                }
            }
             */
            //makebasemask
            mask = new string[SizeX, SizeY, 2];

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    if (i == 0)
                    {
                        mask[i, j, 0] = " ";
                        mask[i, j, 1] = " ";
                    }
                    else
                    {
                        mask[i, j, 0] = " ";
                        mask[i, j, 1] = " ";
                    }
                }
            }

            try
            {
                for (int i = 0; i < Nlist.Count; i++)
                {
                    for (int j = 0; j < Nlist[i].SizeX; j++)
                    {
                        for (int k = 0; k < Nlist[i].SizeY; k++)
                        {
                            mask[CoordX[i] + j, CoordY[i] + k, 0] = "#";
                            mask[CoordX[i] + j, CoordY[i] + k, 1] = "#";
                        }
                    }
                }
            }
            catch
            {
                SZMod++;
                goto restart;
            }
            //maskupdate
            

            //DrawMask();
            //IOLonger
            tipn = 0;
            topn = 0;

            for (int i = 0; i < Wlist.Count; i++)
            {
                for (int j = 0; j < Nlist.Count; j++)
                {
                    if (Wlist[i].StartName.Split('-')[0] == Uname[j])
                    {
                        for (int k = 0; k < Nlist[j].OutPorts.Length; k++)
                        {
                            if (Wlist[i].StartName.Split('-')[1] == Nlist[j].OutPorts[k].Name)
                            {
                                Wlist[i].StartX = Nlist[j].OutPorts[k].PosX + CoordX[j];
                                Wlist[i].StartY = Nlist[j].OutPorts[k].PosY + CoordY[j];
                                mask[Wlist[i].StartX, Wlist[i].StartY, 0] = " ";
                                mask[Wlist[i].StartX, Wlist[i].StartY, 1] = " ";
                                for (int q = 0; q < (k + 1) * 2; q++)
                                {
                                    try
                                    {
                                        mask[Wlist[i].StartX - 1, Wlist[i].StartY + q, 0] = "x";
                                        mask[Wlist[i].StartX + 1, Wlist[i].StartY + q, 0] = "x";
                                    }
                                    catch
                                    {
                                        goto restart;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < Wlist.Count; i++)
            {
                for (int j = 0; j < Nlist.Count; j++)
                {
                    if (Wlist[i].EndName.Split('-')[0] == Uname[j])
                    {
                        for (int k = 0; k < Nlist[j].InPorts.Length; k++)
                        {
                            if (Wlist[i].EndName.Split('-')[1] == Nlist[j].InPorts[k].Name)
                            {
                                Wlist[i].EndX = Nlist[j].InPorts[k].PosX + CoordX[j];
                                Wlist[i].EndY = Nlist[j].InPorts[k].PosY + CoordY[j];
                                mask[Wlist[i].EndX, Wlist[i].EndY, 0] = " ";
                                mask[Wlist[i].EndX, Wlist[i].EndY, 1] = " ";

                                for (int q = 0; q < (k+1) * 2; q++)
                                {
                                    mask[Wlist[i].EndX + 1, Wlist[i].EndY - q, 0] = "x";
                                    mask[Wlist[i].EndX - 1, Wlist[i].EndY - q, 0] = "x";
                                }

                            }
                        }
                    }
                }
            }
            //mask2
            for (int i = 0; i < RyadNum; i++)
            {
                for (int j = 0; j < Nlist.Count; j++)
                {

                    if (RyadList[j] == i)
                    {
                        if (Nlist[j].Name != "INPort")
                        {
                            if (Nlist[j].Name != "OUTPort")
                            {
                                for (int port = 0; port < Nlist[j].InPorts.Length; port++)
                                {
                                    int portStx = Nlist[j].InPorts[port].PosX + CoordX[j];
                                    int portSty = Nlist[j].InPorts[port].PosY + CoordY[j];
                                    for (int k = 0; k < INPortInRyad[i] * 2 + 2; k++)
                                    {
                                        mask[portStx - 1, portSty - k, 0] = "X";
                                        mask[portStx + 1, portSty - k, 0] = "X";
                                    }
                                    portSty = portSty - INPortInRyad[i] * 2 + 0;
                                    mask[portStx, portSty - 1, 0] = "X";
                                    for (int k = portStx + 1; k < SizeX; k++)
                                    {
                                        mask[k, portSty - 1, 0] = "X";
                                        mask[k, portSty - 0, 0] = " ";
                                        mask[k, portSty + 1, 0] = "X";
                                    }
                                    INPortInRyad[i]--;
                                }
                                for (int port = 0; port < Nlist[j].OutPorts.Length; port++)
                                {
                                    int portStx = Nlist[j].OutPorts[port].PosX + CoordX[j];
                                    int portSty = Nlist[j].OutPorts[port].PosY + CoordY[j];
                                    for (int k = 0; k < OUTPortInRyad[i] * 2 + 3 + MaxVsizeObjInRyad[i] - Nlist[j].SizeY; k++)
                                    {
                                        mask[portStx - 1, portSty + k, 0] = "X";
                                        mask[portStx + 1, portSty + k, 0] = "X";
                                    }

                                    portSty = portSty + OUTPortInRyad[i] * 2 + 1 + MaxVsizeObjInRyad[i] - Nlist[j].SizeY;
                                    mask[portStx, portSty + 1, 0] = "X";
                                    for (int k = portStx + 1; k < SizeX; k++)
                                    {
                                        mask[k, portSty - 1, 0] = "X";
                                        mask[k, portSty - 0, 0] = " ";
                                        mask[k, portSty + 1, 0] = "X";
                                    }

                                    OUTPortInRyad[i]--;
                                }
                            }
                        }

                        if (Nlist[j].Name == "INPort")
                        {
                            for (int port = 0; port < Nlist[j].OutPorts.Length; port++)
                            {
                                int portStx = Nlist[j].OutPorts[port].PosX + CoordX[j];
                                int portSty = Nlist[j].OutPorts[port].PosY + CoordY[j];
                                //for (int k = 0; k < OUTPortNum * 2 + 3 + MaxVsizeObjInRyad[i] - Nlist[j].SizeY; k++)
                                for (int k = 0; k < INPortNum * 2 + 3 + 0; k++)
                                {
                                    mask[portStx - 1, portSty + k, 0] = "X";
                                    mask[portStx + 1, portSty + k, 0] = "X";
                                }

                                //portSty = portSty + OUTPortNum * 2 + 1 + MaxVsizeObjInRyad[i] - Nlist[j].SizeY;
                                portSty = portSty + INPortNum * 2 + 1 + 0;
                                mask[portStx, portSty + 1, 0] = "X";
                                for (int k = portStx + 1; k < SizeX; k++)
                                {
                                    mask[k, portSty - 1, 0] = "X";
                                    mask[k, portSty - 0, 0] = " ";
                                    mask[k, portSty + 1, 0] = "X";
                                }

                                INPortNum--;
                            }
                        }
                        if (Nlist[j].Name == "OUTPort")
                        {
                            for (int port = 0; port < Nlist[j].InPorts.Length; port++)
                            {
                                int portStx = Nlist[j].InPorts[port].PosX + CoordX[j];
                                int portSty = Nlist[j].InPorts[port].PosY + CoordY[j];
                                for (int k = 0; k < OUTPortNum * 2 + 2; k++)
                                {
                                    mask[portStx - 1, portSty - k, 0] = "X";
                                    mask[portStx + 1, portSty - k, 0] = "X";
                                }
                                portSty = portSty - OUTPortNum * 2 + 0;
                                mask[portStx, portSty - 1, 0] = "X";
                                for (int k = portStx + 1; k < SizeX; k++)
                                {
                                    mask[k, portSty - 1, 0] = "X";
                                    mask[k, portSty - 0, 0] = " ";
                                    mask[k, portSty + 1, 0] = "X";
                                }
                                OUTPortNum--;
                            }
                        }

                    }
                }
            }



            for (int i = 0; i < RyadNum; i++)
            {
                for (int j = 0; j < Nlist.Count; j++)
                {

                    if (RyadList[j] == i)
                    {
                        if (Nlist[j].Name != "INPort")
                        {
                            if (Nlist[j].Name != "OUTPort")
                            {
                                for (int port = 0; port < Nlist[j].InPorts.Length; port++)
                                {
                                    //portStx = 
                                }
                            }
                        }
                    }
                }
            }

            
            //goto maker;



            //Router
            int Over9000 = SizeX + SizeY*2 ;

            //DrawMask();

            for (int i = 0; i < Wlist.Count; i++)
            {
                Console.Write(".");
                wavemap = new int[SizeX, SizeY, 2];

                int wlen = 1;

                wavemap[Wlist[i].StartX, Wlist[i].StartY, 0] = 1;

                for (int j = 1; j < Over9000; j++)
                {
                    for (int k = 0; k < SizeX; k++)
                    {
                        for (int l = 0; l < SizeY; l++)
                        {
                            for (int m = 0; m < 2; m++)
                            {
                                if (wavemap[k, l, m] == j && k - 1 >= 0)
                                {
                                    if (wavemap[k - 1, l, m] == 0)
                                    {
                                        if (mask[k - 1, l, m] == " ")
                                        {
                                            wavemap[k - 1, l, m] = j+1;
                                        }
                                    }
                                }

                                if (wavemap[k, l, m] == j && k + 1 < SizeX)
                                {
                                    if (wavemap[k + 1, l, m] == 0)
                                    {
                                        if (mask[k + 1, l, m] == " ")
                                        {
                                            wavemap[k + 1, l, m] = j+1;
                                        }
                                    }
                                }

                                if (wavemap[k, l, m] == j && l - 1 >= 0)
                                {
                                    if (wavemap[k, l - 1, m] == 0)
                                    {
                                        if (mask[k, l - 1, m] == " ")
                                        {
                                            wavemap[k, l - 1, m] = j+1;
                                        }
                                    }
                                }

                                if (wavemap[k, l, m] == j && l + 1 < SizeY)
                                {
                                    if (wavemap[k, l + 1, m] == 0)
                                    {
                                        if (mask[k, l + 1, m] == " ")
                                        {
                                            wavemap[k, l + 1, m] = j+1;
                                        }
                                    }
                                }

                                if (wavemap[k, l, m] == j && m - 1 >= 0)
                                {
                                    if (wavemap[k, l, m - 1] == 0)
                                    {
                                        if (mask[k, l , m - 1] == " ")
                                        {
                                            wavemap[k, l, m - 1] = j + 1;
                                        }
                                    }
                                }

                                if (wavemap[k, l, m] == j && m + 1 <= 1)
                                {
                                    if (wavemap[k, l, m + 1] == 0)
                                    {
                                        if (mask[k, l, m + 1] == " ")
                                        {
                                            wavemap[k, l, m + 1] = j + 1;
                                        }
                                    }
                                }

                                if (k == Wlist[i].EndX)
                                {
                                    if (l == Wlist[i].EndY)
                                    {
                                        if (m == 0)
                                        {
                                            if (wavemap[k, l, m] > 0)
                                            {
                                                wlen = j;
                                                j = Over9000 + 1;
                                            }
                                        }
                                    }
                                }

                            }   
                        }
                    }
                    
                }

                if (wlen == 1)
                {
                    Wprior[i]++;
                    if (i == 0) placewidch++;
                    Console.WriteLine("Unrouted "+ i.ToString());
                    goto restart;
                }

                //DrawWave(); //to debug
                //BackTracing
                Wlist[i].WirePointX = new int[wlen+1];
                Wlist[i].WirePointY = new int[wlen+1];
                Wlist[i].WirePointZ = new int[wlen+1];
                Wlist[i].WirePointX[wlen] = Wlist[i].EndX;
                Wlist[i].WirePointY[wlen] = Wlist[i].EndY;
                Wlist[i].WirePointZ[wlen] = 0;

                Wlist[i].WirePointX[0] = Wlist[i].StartX;
                Wlist[i].WirePointY[0] = Wlist[i].StartY;
                Wlist[i].WirePointZ[0] = 0;

                int tx = Wlist[i].EndX;
                int ty = Wlist[i].EndY;
                int tz = 0;

                for (int j = 1; j < wlen; j++)
                {
                    if ((tx - 1) > 0)
                        if (wavemap[tx - 1, ty, tz] < wavemap[tx, ty, tz] && wavemap[tx - 1, ty, tz]>0)
                        {
                            tx = tx - 1;
                            goto endselect;
                        }

                    if ((tx + 1) < SizeX)
                        if (wavemap[tx + 1, ty, tz] < wavemap[tx, ty, tz] && wavemap[tx + 1, ty, tz] > 0)
                        {
                            tx = tx + 1;
                            goto endselect;
                        }

                    if ((ty - 1) > 0)
                        if (wavemap[tx, ty - 1, tz] < wavemap[tx, ty, tz] && wavemap[tx, ty - 1, tz] > 0)
                        {
                            ty = ty - 1;
                            goto endselect;
                        }

                    if ((ty + 1) < SizeY)
                        if (wavemap[tx, ty + 1, tz] < wavemap[tx, ty, tz] && wavemap[tx, ty + 1, tz] > 0)
                        {
                            ty = ty + 1;
                            goto endselect;
                        }


                    if ((tz + 1) < 2)
                        if (wavemap[tx, ty, tz + 1] < wavemap[tx, ty, tz] && wavemap[tx, ty, tz + 1] > 0)
                        {
                            tz = tz + 1;
                            goto endselect;
                        }


                    if ((tz - 1) >= 0)
                        if (wavemap[tx, ty, tz - 1] < wavemap[tx, ty, tz] && wavemap[tx, ty, tz - 1] > 0)
                        {
                            tz = tz - 1;
                            goto endselect;
                        }

                endselect:

                    Wlist[i].WirePointX[wlen - j] = tx;
                    Wlist[i].WirePointY[wlen - j] = ty;
                    Wlist[i].WirePointZ[wlen - j] = tz;
                }

                // update mask

                for (int j = 0; j < Wlist[i].WirePointX.Length; j++)
                {
                    mask[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], Wlist[i].WirePointZ[j]] = "#";

                    if (Wlist[i].WirePointX[j] - 1 > 0)
                    {
                        mask[Wlist[i].WirePointX[j] - 1, Wlist[i].WirePointY[j], Wlist[i].WirePointZ[j]] = "#";
                    }
                    if (Wlist[i].WirePointX[j] + 1 < SizeX)
                    {
                        mask[Wlist[i].WirePointX[j] + 1, Wlist[i].WirePointY[j], Wlist[i].WirePointZ[j]] = "#";
                    }
                    if (Wlist[i].WirePointY[j] - 1 > 0)
                    {
                        mask[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j] - 1, Wlist[i].WirePointZ[j]] = "#";
                    }
                    if (Wlist[i].WirePointY[j] + 1 < SizeY)
                    {
                        mask[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j] + 1, Wlist[i].WirePointZ[j]] = "#";
                    }

                    
                }
                //Console.Clear();
                //DrawMask();


            }

            //maker:
            //maker
            Node outNode = new Node("4AND", SizeX, SizeY, 4);

            for (int j = 0; j < outNode.SizeX; j++)
            {
                for (int k = 0; k < outNode.SizeY; k++)
                {
                    for (int l = 0; l < outNode.SizeZ; l++)
                    {
                        try
                        {
                            outNode.DataMatrix[j, k, l] = "0";
                        }
                        catch
                        {
                            VsizeMod++;
                            goto restart;
                        }
                    }
                }
            }

            for (int i = 0; i < Nlist.Count; i++)
            {
                for (int j = 0; j < Nlist[i].SizeX; j++)
                {
                    for (int k = 0; k < Nlist[i].SizeY; k++)
                    {
                        for (int l = 0; l < Nlist[i].SizeZ; l++)
                        {
                            outNode.DataMatrix[CoordX[i] + j, CoordY[i] + k, l] = Nlist[i].DataMatrix[j,Nlist[i].SizeY - k - 1, l];
                        }   
                    }
                }
            }
            outNode.InPorts = new INPort[inpnum];
            outNode.OutPorts = new OUTPort[outpnum];

            int tip = 0;
            int top = 0;

            for (int i = 0; i < Nlist.Count; i++)
            {
                if (Nlist[i].Name == "INPort")
                {
                    outNode.InPorts[tip] = new INPort(Uname[i], Nlist[i].InPorts[0].PosX + CoordX[i], Nlist[i].InPorts[0].PosY + CoordY[i]);
                    tip++;
                }
                if (Nlist[i].Name == "OUTPort")
                {
                    outNode.OutPorts[top] = new OUTPort(Uname[i], Nlist[i].OutPorts[0].PosX + CoordX[i], Nlist[i].OutPorts[0].PosY + CoordY[i]);
                    top++;
                }
            }
            //goto endmake;
        
            //make wire
            for (int i = 0; i < Wlist.Count; i++)
            {
                for (int j = 0; j < Wlist[i].WirePointX.Length; j++)
                {
                    if (Wlist[i].WirePointZ[j] == 0)
                    {
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 0] = "w";
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = "#";
                    }

                    if (Wlist[i].WirePointZ[j] == 1)
                    {
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 2] = "w";
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 3] = "#";
                    }
                }
                //upper - downer
                for (int j = 1; j < Wlist[i].WirePointX.Length; j++)
                {
                    if (Wlist[i].WirePointZ[j] != Wlist[i].WirePointZ[j-1])
                    {
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 0] = "0";
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = "w";
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 2] = "#";
                        outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 3] = "0";
                    }
                }
                //make wire repiters
                int tlen = 2;
                try
                {
                    for (int j = 2; j < Wlist[i].WirePointX.Length - 2; j++)
                    {
                        tlen++;
                        if (tlen > 13)
                        {
                            if (Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j + 1] && Wlist[i].WirePointZ[j + 1] == Wlist[i].WirePointZ[j + 2] && Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j - 2])
                            {
                                if (Wlist[i].WirePointY[j - 1] == Wlist[i].WirePointY[j + 1])
                                {
                                    if (Wlist[i].WirePointX[j - 1] < Wlist[i].WirePointX[j + 1])
                                    {
                                        if (Wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = ">";
                                        }

                                        if (Wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 3] = ">";
                                        }
                                        tlen = 0;
                                    }
                                }
                            }

                            if (Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j + 1] && Wlist[i].WirePointZ[j + 1] == Wlist[i].WirePointZ[j + 2] && Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j - 2])
                            {
                                if (Wlist[i].WirePointY[j - 1] == Wlist[i].WirePointY[j + 1])
                                {
                                    if (Wlist[i].WirePointX[j - 1] > Wlist[i].WirePointX[j + 1])
                                    {
                                        if (Wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = "<";
                                        }

                                        if (Wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 3] = "<";
                                        }
                                        tlen = 0;
                                    }
                                }
                            }

                            if (Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j + 1] && Wlist[i].WirePointZ[j + 1] == Wlist[i].WirePointZ[j + 2] && Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j - 2])
                            {
                                if (Wlist[i].WirePointX[j - 1] == Wlist[i].WirePointX[j + 1])
                                {
                                    if (Wlist[i].WirePointY[j - 1] > Wlist[i].WirePointY[j + 1])
                                    {
                                        if (Wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = "v";
                                        }

                                        if (Wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 3] = "v";
                                        }
                                        tlen = 0;
                                    }
                                }
                            }

                            if (Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j + 1] && Wlist[i].WirePointZ[j + 1] == Wlist[i].WirePointZ[j + 2] && Wlist[i].WirePointZ[j - 1] == Wlist[i].WirePointZ[j - 2])
                            {
                                if (Wlist[i].WirePointX[j - 1] == Wlist[i].WirePointX[j + 1])
                                {
                                    if (Wlist[i].WirePointY[j - 1] < Wlist[i].WirePointY[j + 1])
                                    {
                                        if (Wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = "^";
                                        }

                                        if (Wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 3] = "^";
                                        }
                                        tlen = 0;
                                    }
                                }
                            }

                            if (tlen != 0)
                            {
                                j -= 2;
                                if (j < 1)
                                {
                                    //to do check
                                    //j = Wlist[i].WirePointX.Length;
                                }
                            }


                        }


                    }
                }
                catch
                {
                    for (int j = 0; j < Wlist[i].WirePointX.Length; j++)
                    {
                        if (Wlist[i].WirePointZ[j] == 0)
                        {
                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 0] = "k";
                            //outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = "#";
                        }

                        if (Wlist[i].WirePointZ[j] == 1)
                        {
                            outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 2] = "k";
                            //outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 3] = "#";
                        }
                    }

                    //placewidch++;
                    //goto restart;
                }
            

            }

            //end make
        //endmake:



            
            //DrawOutMask
            
            tipn = 0;
            topn = 0;

            for (int i = 0; i < Wlist.Count; i++)
            {
                for (int j = 0; j < Nlist.Count; j++)
                {
                    if (Wlist[i].StartName.Split('-')[0] == Uname[j])
                    {
                        for (int k = 0; k < Nlist[j].OutPorts.Length; k++)
                        {
                            if (Wlist[i].StartName.Split('-')[1] == Nlist[j].OutPorts[k].Name)
                            {
                                Wlist[i].StartX = Nlist[j].OutPorts[k].PosX + CoordX[j];
                                Wlist[i].StartY = Nlist[j].OutPorts[k].PosY + CoordY[j];
                                mask[Wlist[i].StartX, Wlist[i].StartY, 0] = " ";
                                mask[Wlist[i].StartX, Wlist[i].StartY, 1] = " ";
                                for (int q = 0; q < (k + 1) * 2; q++)
                                {
                                    try
                                    {
                                        //mask[Wlist[i].StartX - 1, Wlist[i].StartY + q, 0] = "X";
                                        //mask[Wlist[i].StartX + 1, Wlist[i].StartY + q, 0] = "X";
                                    }
                                    catch
                                    {
                                        goto restart;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < Wlist.Count; i++)
            {
                for (int j = 0; j < Nlist.Count; j++)
                {
                    if (Wlist[i].EndName.Split('-')[0] == Uname[j])
                    {
                        for (int k = 0; k < Nlist[j].InPorts.Length; k++)
                        {
                            if (Wlist[i].EndName.Split('-')[1] == Nlist[j].InPorts[k].Name)
                            {
                                Wlist[i].EndX = Nlist[j].InPorts[k].PosX + CoordX[j];
                                Wlist[i].EndY = Nlist[j].InPorts[k].PosY + CoordY[j];
                                mask[Wlist[i].EndX, Wlist[i].EndY, 0] = " ";
                                mask[Wlist[i].EndX, Wlist[i].EndY, 1] = " ";

                                for (int q = 0; q < (k + 1) * 2; q++)
                                {
                                    //mask[Wlist[i].EndX + 1, Wlist[i].EndY - q, 0] = "X";
                                    //mask[Wlist[i].EndX - 1, Wlist[i].EndY - q, 0] = "X";
                                }

                            }
                        }
                    }
                }
            }
                //drawe
            /*
            for (int i = 0; i < RyadNum; i++)
            {
                for (int j = 0; j < Nlist.Count; j++)
                {

                    if (RyadList[j] == i)
                    {
                        if (Nlist[j].Name != "INPort")
                        {
                            if (Nlist[j].Name != "OUTPort")
                            {
                                for (int port = 0; port < Nlist[j].InPorts.Length; port++)
                                {
                                    int portStx = Nlist[j].InPorts[port].PosX + CoordX[j];
                                    int portSty = Nlist[j].InPorts[port].PosY + CoordY[j];
                                    for (int k = 0; k < INPortInRyad[i]*2+2; k++)
                                    {
                                        mask[portStx - 1, portSty - k, 0] = "X";
                                        mask[portStx + 1, portSty - k, 0] = "X";
                                    }
                                    portSty = portSty - INPortInRyad[i] * 2+0;
                                    mask[portStx, portSty - 1, 0] = "X";
                                    for (int k = portStx+1; k < SizeX; k++)
                                    {
                                        mask[k, portSty - 1, 0] = "X";
                                        mask[k, portSty - 0, 0] = " ";
                                        mask[k, portSty + 1, 0] = "X";
                                    }
                                    INPortInRyad[i]--;
                                }
                                for (int port = 0; port < Nlist[j].OutPorts.Length; port++)
                                {
                                    int portStx = Nlist[j].OutPorts[port].PosX + CoordX[j];
                                    int portSty = Nlist[j].OutPorts[port].PosY + CoordY[j];
                                    for (int k = 0; k < OUTPortInRyad[i] * 2 + 3; k++)
                                    {
                                        mask[portStx - 1, portSty + k, 0] = "X";
                                        mask[portStx + 1, portSty + k, 0] = "X";
                                    }

                                    portSty = portSty + OUTPortInRyad[i] * 2 + 1;
                                    mask[portStx, portSty + 1, 0] = "X";
                                    for (int k = portStx + 1; k < SizeX; k++)
                                    {
                                        mask[k, portSty - 1, 0] = "X";
                                        mask[k, portSty - 0, 0] = " ";
                                        mask[k, portSty + 1, 0] = "X";
                                    }

                                    OUTPortInRyad[i]--;
                                }
                            }
                        }
                    }
                }
            }
            */
            for (int i = 0; i < outNode.SizeX; i++)
            {
                for (int j = 0; j < outNode.SizeY; j++)
                {
                    if (mask[i, j, 0] == "X")
                    {
                        //to Debug
                        //outNode.DataMatrix[i, j, 0] = "k";
                    }
                }
            }
            int maxR = 0;
            for (int i = 0; i < outNode.SizeX; i++)
            {
                for (int j = 0; j < outNode.SizeY; j++)
                {
                    if (mask[i, j, 0] == "#")
                    {
                        if (maxR < i)
                        {
                            maxR = i;
                        }
                    }
                    if (mask[i, j, 1] == "#")
                    {
                        if (maxR < i)
                        {
                            maxR = i;
                        }
                    }
                }
            }

            outNode.SizeX = maxR+1;

            //exporter
            Vmode = true;
            if (Vmode)
            {
                outNode.export(FileName + ".binhl");
            }
            else
            {

                Vmode = true;
                goto restart;
            }

        }

		static int Max (int[] intList)
		{
			int max = intList [0];

			for (int i=0; i<intList.Length; i++) 
			{
				if (intList[i]>max) max = intList[i];
			}

			return max;
		}
        private static void DrawMask()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    Console.Write(mask[i, j, 0]);
                }
                Console.WriteLine();
            }

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    Console.Write(mask[i, j, 1]);
                }
                Console.WriteLine();
            }

        }

        private static void DrawWave()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    Console.Write(wavemap[i, j, 0].ToString("00"));
                }
                Console.WriteLine();
            }

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    Console.Write(wavemap[i, j, 1].ToString("00"));
                }
                Console.WriteLine();
            }

        }
    }
}
