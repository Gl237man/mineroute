using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace RouteUtils
{
    public class Node
    {
        public string[,,] DataMatrix;
        public INPort[] InPorts;
        public string Name;
        public OutPort[] OutPorts;
        public int SizeX;
        public int SizeY;
        public int SizeZ;
        public string[,,] mask;

        public Node(string name, int sx, int sy, int sz)
        {
            Name = name;
            InPorts = new INPort[0];
            OutPorts = new OutPort[0];
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

        public Node(string fileName)
        {
            string[] infdat = File.ReadAllLines(fileName);
            int stnum = 0;
            //Reading Name
            string[] SpStrName = infdat[stnum].Split(':');
            Name = SpStrName[1];

            stnum++;
            string[] SpStrInNum = infdat[stnum].Split(':');
            InPorts = new INPort[Convert.ToInt32(SpStrInNum[1])];

            stnum++;
            string[] SpStrOutNum = infdat[stnum].Split(':');
            OutPorts = new OutPort[Convert.ToInt32(SpStrOutNum[1])];

            for (int i = 0; i < InPorts.Length; i++)
            {
                stnum++;
                string[] SpStrInPort = infdat[stnum].Split(':');
                InPorts[i] = new INPort(SpStrInPort[1], Convert.ToInt32(SpStrInPort[2]), Convert.ToInt32(SpStrInPort[3]));
            }

            for (int i = 0; i < OutPorts.Length; i++)
            {
                stnum++;
                string[] SpStrOutPort = infdat[stnum].Split(':');
                OutPorts[i] = new OutPort(SpStrOutPort[1], Convert.ToInt32(SpStrOutPort[2]),
                    Convert.ToInt32(SpStrOutPort[3]));
            }

            stnum++;
            string[] SpStrSize = infdat[stnum].Split(':');

            SizeX = Convert.ToInt32(SpStrSize[1]);
            SizeY = Convert.ToInt32(SpStrSize[2]);

            stnum++;
            string[] SpStrLayers = infdat[stnum].Split(':');

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
                string[] SpStrLayer = infdat[stnum].Split(':');
                int lnum = Convert.ToInt32(SpStrLayer[1]);

                for (int j = 0; j < SizeY; j++)
                {
                    stnum++;
                    string inst = infdat[stnum];
                    for (int k = 0; k < SizeX; k++)
                    {
                        DataMatrix[k, j, lnum] = inst.Substring(k, 1);
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
            ostr += "Name:" + Name + "\r\n";
            ostr += "in:" + InPorts.Length + "\r\n";
            ostr += "out:" + OutPorts.Length + "\r\n";

            for (int i = 0; i < InPorts.Length; i++)
            {
                ostr += "in:" + InPorts[i].Name + ":" + InPorts[i].PosX + ":" + InPorts[i].PosY + "\r\n";
            }
            for (int i = 0; i < OutPorts.Length; i++)
            {
                ostr += "out:" + OutPorts[i].Name + ":" + OutPorts[i].PosX + ":" + OutPorts[i].PosY + "\r\n";
            }

            ostr += "size:" + SizeX + ":" + SizeY + "\r\n";
            ostr += "layers:" + SizeZ + "\r\n";

            var SB = new StringBuilder();

            for (int i = 0; i < SizeZ; i++)
            {
                SB.Append("layer:" + i + "\r\n");
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeX; k++)
                    {
                        //InvertExport!
                        //SB.Append(DataMatrix[k, SizeY - j - 1, i]);
                        SB.Append(DataMatrix[k, j, i]);
                    }
                    SB.Append("\r\n");
                }
            }
            ostr += SB.ToString();
            //ostr = ostr.Replace("0", " ");

            //DrawImg();

            File.WriteAllText(FileName, ostr);
        }

        private void DrawImg()
        {
            int ms = 10;
            Image immain = new Bitmap(1024, 1024);
            Graphics g = Graphics.FromImage(immain);
            g.Clear(Color.Black);
            for (int i = 0; i < SizeZ; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    for (int k = 0; k < SizeX; k++)
                    {
                        if (DataMatrix[k, j, i] == "w")
                        {
                            var Bpoint = new Point(i*ms + j*(ms/2), k*ms + j*(ms/2));
                            g.DrawLine(Pens.Green, Bpoint.X, Bpoint.Y, Bpoint.X + ms, Bpoint.Y);
                            g.DrawLine(Pens.Green, Bpoint.X, Bpoint.Y, Bpoint.X, Bpoint.Y + ms);
                            g.DrawLine(Pens.Green, Bpoint.X + ms, Bpoint.Y, Bpoint.X + ms, Bpoint.Y + ms);
                            g.DrawLine(Pens.Green, Bpoint.X, Bpoint.Y + ms, Bpoint.X + ms, Bpoint.Y + ms);
                        }
                        if (DataMatrix[k, j, i] == "W")
                        {
                            var Bpoint = new Point(i*ms + j*(ms/2), k*ms + j*(ms/2));
                            g.DrawLine(Pens.Red, Bpoint.X, Bpoint.Y, Bpoint.X + ms, Bpoint.Y);
                            g.DrawLine(Pens.Red, Bpoint.X, Bpoint.Y, Bpoint.X, Bpoint.Y + ms);
                            g.DrawLine(Pens.Red, Bpoint.X + ms, Bpoint.Y, Bpoint.X + ms, Bpoint.Y + ms);
                            g.DrawLine(Pens.Red, Bpoint.X, Bpoint.Y + ms, Bpoint.X + ms, Bpoint.Y + ms);
                        }
                    }
                }
            }
            immain.Save("1.bmp");
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
}