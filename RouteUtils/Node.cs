using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace RouteUtils
{
    public class Node
    {
        public readonly string[,,] DataMatrix;
        public InPort[] InPorts;
        public readonly string Name;
        public OutPort[] OutPorts;
        public int SizeX;
        public int SizeY;
        public int SizeZ;
        public string[,,] Mask;
        public string NodeName;

        public Node(string name, int sx, int sy, int sz)
        {
            Name = name;
            InPorts = new InPort[0];
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
            string[] spStrName = infdat[stnum].Split(':');
            Name = spStrName[1];

            stnum++;
            string[] spStrInNum = infdat[stnum].Split(':');
            InPorts = new InPort[Convert.ToInt32(spStrInNum[1])];

            stnum++;
            string[] spStrOutNum = infdat[stnum].Split(':');
            OutPorts = new OutPort[Convert.ToInt32(spStrOutNum[1])];

            for (int i = 0; i < InPorts.Length; i++)
            {
                stnum++;
                string[] spStrInPort = infdat[stnum].Split(':');
                InPorts[i] = new InPort(spStrInPort[1], Convert.ToInt32(spStrInPort[2]), Convert.ToInt32(spStrInPort[3]));
            }

            for (int i = 0; i < OutPorts.Length; i++)
            {
                stnum++;
                string[] spStrOutPort = infdat[stnum].Split(':');
                OutPorts[i] = new OutPort(spStrOutPort[1], Convert.ToInt32(spStrOutPort[2]),
                    Convert.ToInt32(spStrOutPort[3]));
            }

            stnum++;
            string[] spStrSize = infdat[stnum].Split(':');

            SizeX = Convert.ToInt32(spStrSize[1]);
            SizeY = Convert.ToInt32(spStrSize[2]);

            stnum++;
            string[] spStrLayers = infdat[stnum].Split(':');

            SizeZ = Convert.ToInt32(spStrLayers[1]);

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
                string[] spStrLayer = infdat[stnum].Split(':');
                int lnum = Convert.ToInt32(spStrLayer[1]);

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

        public void PlaceAnotherNode(Node node, int xCoord, int yCoord, int zCoord)
        {
            for (int x = 0; x < node.SizeX; x++)
            {
                for (int y = 0; y < node.SizeY; y++)
                {
                    for (int z = 0; z < node.SizeZ; z++)
                    {
                        DataMatrix[xCoord + x, yCoord + y, zCoord + z] = node.DataMatrix[x, y, z];
                    }
                }
            }
        }

        public void Export(string fileName)
        {
            string ostr = "";
            ostr += string.Format("Name:{0}\r\n", Name);
            ostr += string.Format("in:{0}\r\n", InPorts.Length);
            ostr += string.Format("out:{0}\r\n", OutPorts.Length);

            ostr = InPorts.Aggregate(ostr, (current, t) => current + ("in:" + t.Name + ":" + t.PosX + ":" + t.PosY + "\r\n"));
            ostr = OutPorts.Aggregate(ostr, (current, t) => current + ("out:" + t.Name + ":" + t.PosX + ":" + t.PosY + "\r\n"));

            ostr += string.Format("size:{0}:{1}\r\n", SizeX, SizeY);
            ostr += string.Format("layers:{0}\r\n", SizeZ);

            var builder = new StringBuilder();

            for (int z = 0; z < SizeZ; z++)
            {
                builder.Append("layer:" + z + "\r\n");
                for (int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        builder.Append(DataMatrix[x, y, z]);
                    }
                    builder.Append("\r\n");
                }
            }
            ostr += builder.ToString();

            File.WriteAllText(fileName, ostr);
        }

/*
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
*/

        
    }
}