using System;
using System.Collections.Generic;
using System.Text;

namespace RouteUtils
{
    public class Node
    {
        public string Name;
        public INPort[] InPorts;
        public OUTPort[] OutPorts;
        public int SizeX;
        public int SizeY;
        public int SizeZ;
        public string[, ,] DataMatrix;
        public string[, ,] mask;
        public Node(string name, int sx, int sy, int sz)
        {
            Name = name;
            InPorts = new INPort[0];
            OutPorts = new OUTPort[0];
            SizeX = sx;
            SizeY = sy;
            SizeZ = sz;
            DataMatrix = new string[SizeX, SizeY, SizeZ];
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeZ; k++)
                    {
                        DataMatrix[i, j, k] = "0";
                    }
                }
            }
        }

        public void PlaceAnotherNode(Node node, int Xcoord, int Ycoord, int ZCoord)
        {
            for (int x = 0; x < node.SizeX; x++)
            {
                for (int y = 0; y < node.SizeY; y++)
                {
                    for (int z = 0; z < node.SizeZ; z++)
                    {
                        DataMatrix[Xcoord + x, Ycoord + y, ZCoord + z] = node.DataMatrix[x, y, z];
                    }
                }
            }
        }
        public void export(string FileName)
        {
            string ostr = "";
            ostr += "Name:" + Name +"\r\n";
            ostr += "in:" + InPorts.Length +"\r\n";
            ostr += "out:" + OutPorts.Length + "\r\n";

            for (int i = 0; i < InPorts.Length; i++)
            {
                ostr += "in:" + InPorts[i].Name + ":" + InPorts[i].PosX + ":" + InPorts[i].PosY + "\r\n";
            }
            for (int i = 0; i < OutPorts.Length; i++)
            {
                ostr += "out:" + OutPorts[i].Name + ":" + OutPorts[i].PosX + ":" + OutPorts[i].PosY + "\r\n";
            }

            ostr += "size:" + SizeX.ToString() + ":" + SizeY.ToString() + "\r\n";
            ostr += "layers:" + SizeZ.ToString() + "\r\n";
            for (int i = 0; i < SizeZ; i++)
            {
                ostr += "layer:" + i.ToString() + "\r\n";
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeX; k++)
                    {
                        ostr += DataMatrix[k, SizeY - j - 1, i];
                    }
                    ostr += "\r\n";
                }
            }

            //ostr = ostr.Replace("0", " ");

            //DrawImg();

            System.IO.File.WriteAllText(FileName, ostr);

        }

        private void DrawImg()
        {
            int ms = 10;
            System.Drawing.Image immain = new System.Drawing.Bitmap(1024, 1024);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(immain);
            g.Clear(System.Drawing.Color.Black);
            for (int i = 0; i < SizeZ; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeX; k++)
                    {
                        if (DataMatrix[k, j, i]=="w")
                        {
                            System.Drawing.Point Bpoint = new System.Drawing.Point(i * ms + j * (ms / 2), k * ms + j * (ms / 2));
                            g.DrawLine(System.Drawing.Pens.Green, Bpoint.X, Bpoint.Y, Bpoint.X + ms, Bpoint.Y);
                            g.DrawLine(System.Drawing.Pens.Green, Bpoint.X, Bpoint.Y, Bpoint.X, Bpoint.Y + ms);
                            g.DrawLine(System.Drawing.Pens.Green, Bpoint.X + ms, Bpoint.Y, Bpoint.X + ms, Bpoint.Y+ms);
                            g.DrawLine(System.Drawing.Pens.Green, Bpoint.X, Bpoint.Y+ms, Bpoint.X+ms, Bpoint.Y + ms);
                        }
                        if (DataMatrix[k, j, i] == "W")
                        {
                            System.Drawing.Point Bpoint = new System.Drawing.Point(i * ms + j * (ms / 2), k * ms + j * (ms / 2));
                            g.DrawLine(System.Drawing.Pens.Red, Bpoint.X, Bpoint.Y, Bpoint.X + ms, Bpoint.Y);
                            g.DrawLine(System.Drawing.Pens.Red, Bpoint.X, Bpoint.Y, Bpoint.X, Bpoint.Y + ms);
                            g.DrawLine(System.Drawing.Pens.Red, Bpoint.X + ms, Bpoint.Y, Bpoint.X + ms, Bpoint.Y + ms);
                            g.DrawLine(System.Drawing.Pens.Red, Bpoint.X, Bpoint.Y + ms, Bpoint.X + ms, Bpoint.Y + ms);
                        }
                    }
                }
            }
            immain.Save("1.bmp");
        }

        public Node(string FileName)
        {
            string[] Infdat = System.IO.File.ReadAllLines(FileName);
            int stnum = 0;
            //Reading Name
            string[] SpStrName = Infdat[stnum].Split(':');
            Name = SpStrName[1];

            stnum++;
            string[] SpStrInNum = Infdat[stnum].Split(':');
            InPorts = new INPort[Convert.ToInt32(SpStrInNum[1])];

            stnum++;
            string[] SpStrOutNum = Infdat[stnum].Split(':');
            OutPorts = new OUTPort[Convert.ToInt32(SpStrOutNum[1])];

            for (int i = 0; i < InPorts.Length; i++)
            {
                stnum++;
                string[] SpStrInPort = Infdat[stnum].Split(':');
                InPorts[i] = new INPort(SpStrInPort[1], Convert.ToInt32(SpStrInPort[2]), Convert.ToInt32(SpStrInPort[3]));
            }

            for (int i = 0; i < OutPorts.Length; i++)
            {
                stnum++;
                string[] SpStrOutPort = Infdat[stnum].Split(':');
                OutPorts[i] = new OUTPort(SpStrOutPort[1], Convert.ToInt32(SpStrOutPort[2]), Convert.ToInt32(SpStrOutPort[3]));
            }

            stnum++;
            string[] SpStrSize = Infdat[stnum].Split(':');

            SizeX = Convert.ToInt32(SpStrSize[1]);
            SizeY = Convert.ToInt32(SpStrSize[2]);

            stnum++;
            string[] SpStrLayers = Infdat[stnum].Split(':');

            SizeZ = Convert.ToInt32(SpStrLayers[1]);

            DataMatrix = new string[SizeX, SizeY, SizeZ];
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeZ; k++)
                    {
                        DataMatrix[i, SizeY - j - 1, k] = "0";
                    }
                }
            }

            for (int i = 0; i < SizeZ; i++)
            {

                stnum++;
                string[] SpStrLayer = Infdat[stnum].Split(':');
                int lnum = Convert.ToInt32(SpStrLayer[1]);

                for (int j = 0; j < SizeY; j++)
                {
                    stnum++;
                    string inst = Infdat[stnum];
                    for (int k = 0; k < SizeX; k++)
                    {
                        DataMatrix[k, j, lnum] = inst.Substring(k, 1);
                    }
                }
            }

        }

    }
    public class INPort
    {
        public string Name;
        public int PosX;
        public int PosY;
        public INPort(string name, int x, int y)
        {
            Name = name;
            PosX = x;
            PosY = y;
        }
    }
    public class OUTPort
    {
        public string Name;
        public int PosX;
        public int PosY;
        public OUTPort(string name, int x, int y)
        {
            Name = name;
            PosX = x;
            PosY = y;
        }
    }
}
