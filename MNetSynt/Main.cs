using System;
using System.Collections.Generic;
using System.IO;
using RouteUtils;

namespace MNetSynt
{
    internal static class Program
    {
        private static List<Node> _nlist;
        private static List<string> _uname;
        private static List<int> _coordX;
        private static List<int> _coordY;
        private static List<int> _portNum;
        private static List<Wire> _wlist;
        private static int[] _wprior;
        private static string[,,] _mask;
        private static int[,,] _wavemap;

        private static int SizeX;
        private static int _sizeY;

        private static int _restartnum;
        private static int _szMod = -50;
        private static bool Vmode;
        private static int VsizeMod = -12;

        private static void Main(string[] args)
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
            string[] InFileStr = File.ReadAllLines(FileName + ".MNET");
            _nlist = new List<Node>();
            _wlist = new List<Wire>();
            _uname = new List<string>();

            for (int i = 0; i < InFileStr.Length; i++)
            {
                string[] IfDat = InFileStr[i].Split(':');
                if (IfDat[0] == "NODE")
                {
                    _nlist.Add(new Node(IfDat[1] + ".binhl"));
                    _uname.Add(IfDat[2]);
                }
                if (IfDat[0] == "WIRE")
                {
                    _wlist.Add(new Wire(IfDat[1], IfDat[2]));
                }
            }

            _wprior = new int[_wlist.Count];

            for (int i = 0; i < _wprior.Length; i++)
            {
                _wprior[i] = 0;
            }

            restart:
            Console.WriteLine("SZMod " + _szMod);
            Console.WriteLine("VsizeMod " + VsizeMod);
            Console.WriteLine("placewidch " + placewidch);

            _restartnum++;
            Console.Write(".");
            if (_wprior[0] > _wprior.Length)
            {
                for (int i = 0; i < _wprior.Length; i++)
                {
                    //Wprior[i] = 0;
                    Console.Write("ERRR");
                }
                placewidch++;
                VsizeMod++;
            }


            if (_restartnum > 100)
            {
                _restartnum = 0;
                if (Vmode)
                {
                    VsizeMod++;
                }
                else
                {
                    _szMod++;
                }
            }
            //wlist sort
            for (int i = 0; i < _wprior.Length; i++)
            {
                for (int j = 1; j < _wprior.Length; j++)
                {
                    if (_wprior[j - 1] < _wprior[j])
                    {
                        int t0 = _wprior[j];
                        _wprior[j] = _wprior[j - 1];
                        _wprior[j - 1] = t0;
                        Wire t1 = _wlist[j];
                        _wlist[j] = _wlist[j - 1];
                        _wlist[j - 1] = t1;
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

            for (int i = 0; i < _nlist.Count; i++)
            {
                if (_nlist[i].SizeX > maxx)
                {
                    maxx = _nlist[i].SizeX;
                }
                if (_nlist[i].SizeY > maxy)
                {
                    maxy = _nlist[i].SizeY;
                }
                if (_nlist[i].InPorts.Length > maxports)
                {
                    maxports = _nlist[i].InPorts.Length;
                }
                if (_nlist[i].OutPorts.Length > maxports)
                {
                    maxports = _nlist[i].OutPorts.Length;
                }
                if (_nlist[i].Name != "INPort")
                {
                    if (_nlist[i].Name != "OUTPort")
                    {
                        NotPortNodeNum++;
                    }
                }

                if (_nlist[i].Name == "INPort")
                {
                    inpnum++;
                }
                if (_nlist[i].Name == "OUTPort")
                {
                    outpnum++;
                }
            }


            if (!Vmode)
            {
                SizeX = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1)*(maxx + maxports*2) + inpnum*2 + outpnum*2 +
                        _szMod;
                _sizeY = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1)*(maxy + maxports*2) + inpnum*2 + outpnum*2 +
                         _szMod;
                if (_sizeY < 1 || SizeX < 1)
                {
                    _szMod++;
                    //goto restart;
                }
            }
            else
            {
                SizeX = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1)*(maxx + maxports*2) + inpnum*2 + outpnum*2 +
                        _szMod;
                _sizeY = (Convert.ToInt32(Math.Sqrt(NotPortNodeNum)) + 1)*(maxy + maxports*2) + inpnum*2 + outpnum*2 +
                         _szMod + VsizeMod;
                if (_sizeY < 1 || SizeX < 1)
                {
                    VsizeMod++;
                    goto restart;
                }
            }
            SizeX = 128;
            _sizeY = 256;
            int pmax = Convert.ToInt32(Math.Sqrt(NotPortNodeNum));
            //int px = 0;
            //int py = 0;

            _coordX = new List<int>();
            _coordY = new List<int>();

            int tipn = 0;
            int topn = 0;

            _portNum = new List<int>();

            //MultiLineSort
            var RyadList = new int[_uname.Count];
            PovtorSort:
            var KFList = new int[_uname.Count];
            int kz = 0;
            string s = _uname[0];

            for (int i = 0; i < _wlist.Count; i++)
            {
                string Sname = _wlist[i].StartName.Split('-')[0];
                string Ename = _wlist[i].EndName.Split('-')[0];
                //finding Sindex
                int Sidx = 0;
                int Eidx = 0;
                for (int j = 0; j < _uname.Count; j++)
                {
                    if (_uname[j] == Sname)
                    {
                        Sidx = j;
                    }
                    if (_uname[j] == Ename)
                    {
                        Eidx = j;
                    }
                }
                if (_nlist[Sidx].Name != "INPort")
                {
                    if (_nlist[Eidx].Name != "OUTPort")
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
            for (int i = 0; i < _nlist.Count; i++)
            {
                if (_nlist[i].InPorts.Length <= KFList[i]*2)
                {
                    kz++;
                    RyadList[i]++;
                }
            }
            if (kz > 0) goto PovtorSort;


            int RyadNum = Max(RyadList) + 1;

            for (int i = 0; i < _nlist.Count; i++)
            {
                if (_nlist[i].Name != "INPort")
                {
                    if (_nlist[i].Name != "OUTPort")
                    {
                        _coordX.Add(0);
                        _coordY.Add(0);
                        _portNum.Add(0);
                    }
                }
                if (_nlist[i].Name == "INPort")
                {
                    _coordX.Add(tipn*2 + 3);
                    _coordY.Add(0);
                    _portNum.Add(tipn);
                    tipn++;
                }
                if (_nlist[i].Name == "OUTPort")
                {
                    _coordX.Add(topn*2 + 3);
                    _coordY.Add(_sizeY - 3);
                    _portNum.Add(topn);
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

            var INPortInRyad = new int[RyadNum];
            var OUTPortInRyad = new int[RyadNum];
            var MaxVsizeObjInRyad = new int[RyadNum];
            var MinVsizeObjInRyad = new int[RyadNum];
            int INPortNum = 0;
            int OUTPortNum = 0;
            //pass1
            int Tline = 3 + tipn*2 + 1 + 2;

            for (int i = 0; i < RyadNum; i++)
            {
                MinVsizeObjInRyad[i] = 9999999;
                int lastx = 2;
                int n = 0;
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (RyadList[j] == i)
                    {
                        if (_nlist[j].Name != "INPort")
                        {
                            if (_nlist[j].Name != "OUTPort")
                            {
                                _coordX[j] = lastx;
                                _coordY[j] = Tline; //(RyadList[j] + 1) * 40;
                                lastx += _nlist[j].SizeX + n;
                                INPortInRyad[i] += _nlist[j].InPorts.Length;
                                OUTPortInRyad[i] += _nlist[j].OutPorts.Length;
                                if (MaxVsizeObjInRyad[i] < _nlist[j].SizeY) MaxVsizeObjInRyad[i] = _nlist[j].SizeY;
                                if (MinVsizeObjInRyad[i] > _nlist[j].SizeY) MinVsizeObjInRyad[i] = _nlist[j].SizeY;
                                //n--;
                            }
                        }
                        if (_nlist[j].Name == "INPort")
                        {
                            INPortNum++;
                        }
                        if (_nlist[j].Name == "OUTPort")
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
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (RyadList[j] == i)
                    {
                        if (_nlist[j].Name != "INPort")
                        {
                            if (_nlist[j].Name != "OUTPort")
                            {
                                _coordX[j] = lastx;
                                _coordY[j] = Tline + 0 + INPortInRyad[i]*2; //(RyadList[j] + 1) * 40;
                                lastx += _nlist[j].SizeX + n;
                                //INPortInRyad[i] += Nlist[j].InPorts.Length;
                                //OUTPortInRyad[i] += Nlist[j].OutPorts.Length;
                                //if (MaxVsizeObjInRyad[i] < Nlist[j].SizeY) MaxVsizeObjInRyad[i] = Nlist[j].SizeY;
                                //if (MinVsizeObjInRyad[i] > Nlist[j].SizeY) MinVsizeObjInRyad[i] = Nlist[j].SizeY;
                                //n--;
                            }
                        }
                    }
                }
                Tline += INPortInRyad[i]*2 + MaxVsizeObjInRyad[i] + OUTPortInRyad[i]*2 + 2;
            }
            _sizeY = Tline + OUTPortNum*2 + 4;
            SizeX = _sizeY;
            //SizeY = 128;
            for (int i = 0; i < _nlist.Count; i++)
            {
                if (_nlist[i].Name == "OUTPort")
                {
                    _coordY[i] = _sizeY - 3;
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
            _mask = new string[SizeX, _sizeY, 2];

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    if (i == 0)
                    {
                        _mask[i, j, 0] = " ";
                        _mask[i, j, 1] = " ";
                    }
                    else
                    {
                        _mask[i, j, 0] = " ";
                        _mask[i, j, 1] = " ";
                    }
                }
            }

            try
            {
                for (int i = 0; i < _nlist.Count; i++)
                {
                    for (int j = 0; j < _nlist[i].SizeX; j++)
                    {
                        for (int k = 0; k < _nlist[i].SizeY; k++)
                        {
                            _mask[_coordX[i] + j, _coordY[i] + k, 0] = "#";
                            _mask[_coordX[i] + j, _coordY[i] + k, 1] = "#";
                        }
                    }
                }
            }
            catch
            {
                _szMod++;
                goto restart;
            }
            //maskupdate


            //DrawMask();
            //IOLonger
            tipn = 0;
            topn = 0;

            for (int i = 0; i < _wlist.Count; i++)
            {
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (_wlist[i].StartName.Split('-')[0] == _uname[j])
                    {
                        for (int k = 0; k < _nlist[j].OutPorts.Length; k++)
                        {
                            if (_wlist[i].StartName.Split('-')[1] == _nlist[j].OutPorts[k].Name)
                            {
                                _wlist[i].StartX = _nlist[j].OutPorts[k].PosX + _coordX[j];
                                _wlist[i].StartY = _nlist[j].OutPorts[k].PosY + _coordY[j];
                                _mask[_wlist[i].StartX, _wlist[i].StartY, 0] = " ";
                                _mask[_wlist[i].StartX, _wlist[i].StartY, 1] = " ";
                                for (int q = 0; q < (k + 1)*2; q++)
                                {
                                    try
                                    {
                                        _mask[_wlist[i].StartX - 1, _wlist[i].StartY + q, 0] = "x";
                                        _mask[_wlist[i].StartX + 1, _wlist[i].StartY + q, 0] = "x";
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

            for (int i = 0; i < _wlist.Count; i++)
            {
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (_wlist[i].EndName.Split('-')[0] == _uname[j])
                    {
                        for (int k = 0; k < _nlist[j].InPorts.Length; k++)
                        {
                            if (_wlist[i].EndName.Split('-')[1] == _nlist[j].InPorts[k].Name)
                            {
                                _wlist[i].EndX = _nlist[j].InPorts[k].PosX + _coordX[j];
                                _wlist[i].EndY = _nlist[j].InPorts[k].PosY + _coordY[j];
                                _mask[_wlist[i].EndX, _wlist[i].EndY, 0] = " ";
                                _mask[_wlist[i].EndX, _wlist[i].EndY, 1] = " ";

                                for (int q = 0; q < (k + 1)*2; q++)
                                {
                                    _mask[_wlist[i].EndX + 1, _wlist[i].EndY - q, 0] = "x";
                                    _mask[_wlist[i].EndX - 1, _wlist[i].EndY - q, 0] = "x";
                                }
                            }
                        }
                    }
                }
            }
            //mask2
            for (int i = 0; i < RyadNum; i++)
            {
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (RyadList[j] == i)
                    {
                        if (_nlist[j].Name != "INPort")
                        {
                            if (_nlist[j].Name != "OUTPort")
                            {
                                for (int port = 0; port < _nlist[j].InPorts.Length; port++)
                                {
                                    int portStx = _nlist[j].InPorts[port].PosX + _coordX[j];
                                    int portSty = _nlist[j].InPorts[port].PosY + _coordY[j];
                                    for (int k = 0; k < INPortInRyad[i]*2 + 2; k++)
                                    {
                                        _mask[portStx - 1, portSty - k, 0] = "X";
                                        _mask[portStx + 1, portSty - k, 0] = "X";
                                    }
                                    portSty = portSty - INPortInRyad[i]*2 + 0;
                                    _mask[portStx, portSty - 1, 0] = "X";
                                    for (int k = portStx + 1; k < SizeX; k++)
                                    {
                                        _mask[k, portSty - 1, 0] = "X";
                                        _mask[k, portSty - 0, 0] = " ";
                                        _mask[k, portSty + 1, 0] = "X";
                                    }
                                    INPortInRyad[i]--;
                                }
                                for (int port = 0; port < _nlist[j].OutPorts.Length; port++)
                                {
                                    int portStx = _nlist[j].OutPorts[port].PosX + _coordX[j];
                                    int portSty = _nlist[j].OutPorts[port].PosY + _coordY[j];
                                    for (int k = 0;
                                        k < OUTPortInRyad[i]*2 + 3 + MaxVsizeObjInRyad[i] - _nlist[j].SizeY;
                                        k++)
                                    {
                                        _mask[portStx - 1, portSty + k, 0] = "X";
                                        _mask[portStx + 1, portSty + k, 0] = "X";
                                    }

                                    portSty = portSty + OUTPortInRyad[i]*2 + 1 + MaxVsizeObjInRyad[i] - _nlist[j].SizeY;
                                    _mask[portStx, portSty + 1, 0] = "X";
                                    for (int k = portStx + 1; k < SizeX; k++)
                                    {
                                        _mask[k, portSty - 1, 0] = "X";
                                        _mask[k, portSty - 0, 0] = " ";
                                        _mask[k, portSty + 1, 0] = "X";
                                    }

                                    OUTPortInRyad[i]--;
                                }
                            }
                        }

                        if (_nlist[j].Name == "INPort")
                        {
                            for (int port = 0; port < _nlist[j].OutPorts.Length; port++)
                            {
                                int portStx = _nlist[j].OutPorts[port].PosX + _coordX[j];
                                int portSty = _nlist[j].OutPorts[port].PosY + _coordY[j];
                                //for (int k = 0; k < OUTPortNum * 2 + 3 + MaxVsizeObjInRyad[i] - Nlist[j].SizeY; k++)
                                for (int k = 0; k < INPortNum*2 + 3 + 0; k++)
                                {
                                    _mask[portStx - 1, portSty + k, 0] = "X";
                                    _mask[portStx + 1, portSty + k, 0] = "X";
                                }

                                //portSty = portSty + OUTPortNum * 2 + 1 + MaxVsizeObjInRyad[i] - Nlist[j].SizeY;
                                portSty = portSty + INPortNum*2 + 1 + 0;
                                _mask[portStx, portSty + 1, 0] = "X";
                                for (int k = portStx + 1; k < SizeX; k++)
                                {
                                    _mask[k, portSty - 1, 0] = "X";
                                    _mask[k, portSty - 0, 0] = " ";
                                    _mask[k, portSty + 1, 0] = "X";
                                }

                                INPortNum--;
                            }
                        }
                        if (_nlist[j].Name == "OUTPort")
                        {
                            for (int port = 0; port < _nlist[j].InPorts.Length; port++)
                            {
                                int portStx = _nlist[j].InPorts[port].PosX + _coordX[j];
                                int portSty = _nlist[j].InPorts[port].PosY + _coordY[j];
                                for (int k = 0; k < OUTPortNum*2 + 2; k++)
                                {
                                    _mask[portStx - 1, portSty - k, 0] = "X";
                                    _mask[portStx + 1, portSty - k, 0] = "X";
                                }
                                portSty = portSty - OUTPortNum*2 + 0;
                                _mask[portStx, portSty - 1, 0] = "X";
                                for (int k = portStx + 1; k < SizeX; k++)
                                {
                                    _mask[k, portSty - 1, 0] = "X";
                                    _mask[k, portSty - 0, 0] = " ";
                                    _mask[k, portSty + 1, 0] = "X";
                                }
                                OUTPortNum--;
                            }
                        }
                    }
                }
            }


            for (int i = 0; i < RyadNum; i++)
            {
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (RyadList[j] == i)
                    {
                        if (_nlist[j].Name != "INPort")
                        {
                            if (_nlist[j].Name != "OUTPort")
                            {
                                for (int port = 0; port < _nlist[j].InPorts.Length; port++)
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
            int Over9000 = SizeX + _sizeY*2;

            //DrawMask();

            for (int i = 0; i < _wlist.Count; i++)
            {
                Console.Write(".");
                _wavemap = new int[SizeX, _sizeY, 2];

                int wlen = 1;

                _wavemap[_wlist[i].StartX, _wlist[i].StartY, 0] = 1;

                for (int j = 1; j < Over9000; j++)
                {
                    for (int k = 0; k < SizeX; k++)
                    {
                        for (int l = 0; l < _sizeY; l++)
                        {
                            for (int m = 0; m < 2; m++)
                            {
                                if (_wavemap[k, l, m] == j && k - 1 >= 0)
                                {
                                    if (_wavemap[k - 1, l, m] == 0)
                                    {
                                        if (_mask[k - 1, l, m] == " ")
                                        {
                                            _wavemap[k - 1, l, m] = j + 1;
                                        }
                                    }
                                }

                                if (_wavemap[k, l, m] == j && k + 1 < SizeX)
                                {
                                    if (_wavemap[k + 1, l, m] == 0)
                                    {
                                        if (_mask[k + 1, l, m] == " ")
                                        {
                                            _wavemap[k + 1, l, m] = j + 1;
                                        }
                                    }
                                }

                                if (_wavemap[k, l, m] == j && l - 1 >= 0)
                                {
                                    if (_wavemap[k, l - 1, m] == 0)
                                    {
                                        if (_mask[k, l - 1, m] == " ")
                                        {
                                            _wavemap[k, l - 1, m] = j + 1;
                                        }
                                    }
                                }

                                if (_wavemap[k, l, m] == j && l + 1 < _sizeY)
                                {
                                    if (_wavemap[k, l + 1, m] == 0)
                                    {
                                        if (_mask[k, l + 1, m] == " ")
                                        {
                                            _wavemap[k, l + 1, m] = j + 1;
                                        }
                                    }
                                }

                                if (_wavemap[k, l, m] == j && m - 1 >= 0)
                                {
                                    if (_wavemap[k, l, m - 1] == 0)
                                    {
                                        if (_mask[k, l, m - 1] == " ")
                                        {
                                            _wavemap[k, l, m - 1] = j + 1;
                                        }
                                    }
                                }

                                if (_wavemap[k, l, m] == j && m + 1 <= 1)
                                {
                                    if (_wavemap[k, l, m + 1] == 0)
                                    {
                                        if (_mask[k, l, m + 1] == " ")
                                        {
                                            _wavemap[k, l, m + 1] = j + 1;
                                        }
                                    }
                                }

                                if (k == _wlist[i].EndX)
                                {
                                    if (l == _wlist[i].EndY)
                                    {
                                        if (m == 0)
                                        {
                                            if (_wavemap[k, l, m] > 0)
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
                    _wprior[i]++;
                    if (i == 0) placewidch++;
                    Console.WriteLine("Unrouted " + i);
                    goto restart;
                }

                //DrawWave(); //to debug
                //BackTracing
                _wlist[i].WirePointX = new int[wlen + 1];
                _wlist[i].WirePointY = new int[wlen + 1];
                _wlist[i].WirePointZ = new int[wlen + 1];
                _wlist[i].WirePointX[wlen] = _wlist[i].EndX;
                _wlist[i].WirePointY[wlen] = _wlist[i].EndY;
                _wlist[i].WirePointZ[wlen] = 0;

                _wlist[i].WirePointX[0] = _wlist[i].StartX;
                _wlist[i].WirePointY[0] = _wlist[i].StartY;
                _wlist[i].WirePointZ[0] = 0;

                int tx = _wlist[i].EndX;
                int ty = _wlist[i].EndY;
                int tz = 0;

                for (int j = 1; j < wlen; j++)
                {
                    if ((tx - 1) > 0)
                        if (_wavemap[tx - 1, ty, tz] < _wavemap[tx, ty, tz] && _wavemap[tx - 1, ty, tz] > 0)
                        {
                            tx = tx - 1;
                            goto endselect;
                        }

                    if ((tx + 1) < SizeX)
                        if (_wavemap[tx + 1, ty, tz] < _wavemap[tx, ty, tz] && _wavemap[tx + 1, ty, tz] > 0)
                        {
                            tx = tx + 1;
                            goto endselect;
                        }

                    if ((ty - 1) > 0)
                        if (_wavemap[tx, ty - 1, tz] < _wavemap[tx, ty, tz] && _wavemap[tx, ty - 1, tz] > 0)
                        {
                            ty = ty - 1;
                            goto endselect;
                        }

                    if ((ty + 1) < _sizeY)
                        if (_wavemap[tx, ty + 1, tz] < _wavemap[tx, ty, tz] && _wavemap[tx, ty + 1, tz] > 0)
                        {
                            ty = ty + 1;
                            goto endselect;
                        }


                    if ((tz + 1) < 2)
                        if (_wavemap[tx, ty, tz + 1] < _wavemap[tx, ty, tz] && _wavemap[tx, ty, tz + 1] > 0)
                        {
                            tz = tz + 1;
                            goto endselect;
                        }


                    if ((tz - 1) >= 0)
                        if (_wavemap[tx, ty, tz - 1] < _wavemap[tx, ty, tz] && _wavemap[tx, ty, tz - 1] > 0)
                        {
                            tz = tz - 1;
                        }

                    endselect:

                    _wlist[i].WirePointX[wlen - j] = tx;
                    _wlist[i].WirePointY[wlen - j] = ty;
                    _wlist[i].WirePointZ[wlen - j] = tz;
                }

                // update mask

                for (int j = 0; j < _wlist[i].WirePointX.Length; j++)
                {
                    _mask[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], _wlist[i].WirePointZ[j]] = "#";

                    if (_wlist[i].WirePointX[j] - 1 > 0)
                    {
                        _mask[_wlist[i].WirePointX[j] - 1, _wlist[i].WirePointY[j], _wlist[i].WirePointZ[j]] = "#";
                    }
                    if (_wlist[i].WirePointX[j] + 1 < SizeX)
                    {
                        _mask[_wlist[i].WirePointX[j] + 1, _wlist[i].WirePointY[j], _wlist[i].WirePointZ[j]] = "#";
                    }
                    if (_wlist[i].WirePointY[j] - 1 > 0)
                    {
                        _mask[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j] - 1, _wlist[i].WirePointZ[j]] = "#";
                    }
                    if (_wlist[i].WirePointY[j] + 1 < _sizeY)
                    {
                        _mask[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j] + 1, _wlist[i].WirePointZ[j]] = "#";
                    }
                }
                //Console.Clear();
                //DrawMask();
            }

            //maker:
            //maker
            var outNode = new Node("4AND", SizeX, _sizeY, 4);

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

            for (int i = 0; i < _nlist.Count; i++)
            {
                for (int j = 0; j < _nlist[i].SizeX; j++)
                {
                    for (int k = 0; k < _nlist[i].SizeY; k++)
                    {
                        for (int l = 0; l < _nlist[i].SizeZ; l++)
                        {
                            outNode.DataMatrix[_coordX[i] + j, _coordY[i] + k, l] =
                                _nlist[i].DataMatrix[j, _nlist[i].SizeY - k - 1, l];
                        }
                    }
                }
            }
            outNode.InPorts = new INPort[inpnum];
            outNode.OutPorts = new OutPort[outpnum];

            int tip = 0;
            int top = 0;

            for (int i = 0; i < _nlist.Count; i++)
            {
                if (_nlist[i].Name == "INPort")
                {
                    outNode.InPorts[tip] = new INPort(_uname[i], _nlist[i].InPorts[0].PosX + _coordX[i],
                        _nlist[i].InPorts[0].PosY + _coordY[i]);
                    tip++;
                }
                if (_nlist[i].Name == "OUTPort")
                {
                    outNode.OutPorts[top] = new OutPort(_uname[i], _nlist[i].OutPorts[0].PosX + _coordX[i],
                        _nlist[i].OutPorts[0].PosY + _coordY[i]);
                    top++;
                }
            }
            //goto endmake;

            //make wire
            for (int i = 0; i < _wlist.Count; i++)
            {
                for (int j = 0; j < _wlist[i].WirePointX.Length; j++)
                {
                    if (_wlist[i].WirePointZ[j] == 0)
                    {
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 0] = "w";
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 1] = "#";
                    }

                    if (_wlist[i].WirePointZ[j] == 1)
                    {
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 2] = "w";
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 3] = "#";
                    }
                }
                //upper - downer
                for (int j = 1; j < _wlist[i].WirePointX.Length; j++)
                {
                    if (_wlist[i].WirePointZ[j] != _wlist[i].WirePointZ[j - 1])
                    {
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 0] = "0";
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 1] = "w";
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 2] = "#";
                        outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 3] = "0";
                    }
                }
                //make wire repiters
                int tlen = 2;
                try
                {
                    for (int j = 2; j < _wlist[i].WirePointX.Length - 2; j++)
                    {
                        tlen++;
                        if (tlen > 13)
                        {
                            if (_wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j + 1] &&
                                _wlist[i].WirePointZ[j + 1] == _wlist[i].WirePointZ[j + 2] &&
                                _wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j - 2])
                            {
                                if (_wlist[i].WirePointY[j - 1] == _wlist[i].WirePointY[j + 1])
                                {
                                    if (_wlist[i].WirePointX[j - 1] < _wlist[i].WirePointX[j + 1])
                                    {
                                        if (_wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 1] =
                                                ">";
                                        }

                                        if (_wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 3] =
                                                ">";
                                        }
                                        tlen = 0;
                                    }
                                }
                            }

                            if (_wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j + 1] &&
                                _wlist[i].WirePointZ[j + 1] == _wlist[i].WirePointZ[j + 2] &&
                                _wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j - 2])
                            {
                                if (_wlist[i].WirePointY[j - 1] == _wlist[i].WirePointY[j + 1])
                                {
                                    if (_wlist[i].WirePointX[j - 1] > _wlist[i].WirePointX[j + 1])
                                    {
                                        if (_wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 1] =
                                                "<";
                                        }

                                        if (_wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 3] =
                                                "<";
                                        }
                                        tlen = 0;
                                    }
                                }
                            }

                            if (_wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j + 1] &&
                                _wlist[i].WirePointZ[j + 1] == _wlist[i].WirePointZ[j + 2] &&
                                _wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j - 2])
                            {
                                if (_wlist[i].WirePointX[j - 1] == _wlist[i].WirePointX[j + 1])
                                {
                                    if (_wlist[i].WirePointY[j - 1] > _wlist[i].WirePointY[j + 1])
                                    {
                                        if (_wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 1] =
                                                "v";
                                        }

                                        if (_wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 3] =
                                                "v";
                                        }
                                        tlen = 0;
                                    }
                                }
                            }

                            if (_wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j + 1] &&
                                _wlist[i].WirePointZ[j + 1] == _wlist[i].WirePointZ[j + 2] &&
                                _wlist[i].WirePointZ[j - 1] == _wlist[i].WirePointZ[j - 2])
                            {
                                if (_wlist[i].WirePointX[j - 1] == _wlist[i].WirePointX[j + 1])
                                {
                                    if (_wlist[i].WirePointY[j - 1] < _wlist[i].WirePointY[j + 1])
                                    {
                                        if (_wlist[i].WirePointZ[j] == 0)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 1] =
                                                "^";
                                        }

                                        if (_wlist[i].WirePointZ[j] == 1)
                                        {
                                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 3] =
                                                "^";
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
                    for (int j = 0; j < _wlist[i].WirePointX.Length; j++)
                    {
                        if (_wlist[i].WirePointZ[j] == 0)
                        {
                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 0] = "k";
                            //outNode.DataMatrix[Wlist[i].WirePointX[j], Wlist[i].WirePointY[j], 1] = "#";
                        }

                        if (_wlist[i].WirePointZ[j] == 1)
                        {
                            outNode.DataMatrix[_wlist[i].WirePointX[j], _wlist[i].WirePointY[j], 2] = "k";
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

            for (int i = 0; i < _wlist.Count; i++)
            {
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (_wlist[i].StartName.Split('-')[0] == _uname[j])
                    {
                        for (int k = 0; k < _nlist[j].OutPorts.Length; k++)
                        {
                            if (_wlist[i].StartName.Split('-')[1] == _nlist[j].OutPorts[k].Name)
                            {
                                _wlist[i].StartX = _nlist[j].OutPorts[k].PosX + _coordX[j];
                                _wlist[i].StartY = _nlist[j].OutPorts[k].PosY + _coordY[j];
                                _mask[_wlist[i].StartX, _wlist[i].StartY, 0] = " ";
                                _mask[_wlist[i].StartX, _wlist[i].StartY, 1] = " ";
                                for (int q = 0; q < (k + 1)*2; q++)
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

            for (int i = 0; i < _wlist.Count; i++)
            {
                for (int j = 0; j < _nlist.Count; j++)
                {
                    if (_wlist[i].EndName.Split('-')[0] == _uname[j])
                    {
                        for (int k = 0; k < _nlist[j].InPorts.Length; k++)
                        {
                            if (_wlist[i].EndName.Split('-')[1] == _nlist[j].InPorts[k].Name)
                            {
                                _wlist[i].EndX = _nlist[j].InPorts[k].PosX + _coordX[j];
                                _wlist[i].EndY = _nlist[j].InPorts[k].PosY + _coordY[j];
                                _mask[_wlist[i].EndX, _wlist[i].EndY, 0] = " ";
                                _mask[_wlist[i].EndX, _wlist[i].EndY, 1] = " ";

                                for (int q = 0; q < (k + 1)*2; q++)
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
                    if (_mask[i, j, 0] == "X")
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
                    if (_mask[i, j, 0] == "#")
                    {
                        if (maxR < i)
                        {
                            maxR = i;
                        }
                    }
                    if (_mask[i, j, 1] == "#")
                    {
                        if (maxR < i)
                        {
                            maxR = i;
                        }
                    }
                }
            }

            outNode.SizeX = maxR + 1;

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

        private static int Max(int[] intList)
        {
            int max = intList[0];

            for (int i = 0; i < intList.Length; i++)
            {
                if (intList[i] > max) max = intList[i];
            }

            return max;
        }

        private static void DrawMask()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    Console.Write(_mask[i, j, 0]);
                }
                Console.WriteLine();
            }

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    Console.Write(_mask[i, j, 1]);
                }
                Console.WriteLine();
            }
        }

        private static void DrawWave()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    Console.Write(_wavemap[i, j, 0].ToString("00"));
                }
                Console.WriteLine();
            }

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < _sizeY; j++)
                {
                    Console.Write(_wavemap[i, j, 1].ToString("00"));
                }
                Console.WriteLine();
            }
        }
    }
}