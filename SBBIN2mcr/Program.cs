﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBBIN2mcr
{
    class Program
    {
        const int dDelay = 100;
        const int VMul = 25;
        const int MoveSpeed = 90;
        static void Main(string[] args)
        {
            List<StarBoundNode> nodes = new List<StarBoundNode>();
            List<StarboundWire> wires = new List<StarboundWire>();
            LoadFile(nodes, wires, "test_D.SBBIN");

            string outfile = "";
            outfile += StartClic();
            int xcoord = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                string ToolName = nodes[i].NodeType;
                
                outfile += SelectTool(ToolName);
                
                outfile += MoveTo(nodes[i].xcoord, ref xcoord);

                outfile += PlaceAtY(nodes[i].ycoord);
            }
            //Select zero tool
            outfile += GenKeyPress("D1");

            for (int i = 0; i < wires.Count; i++)
            {
                outfile += MoveTo(wires[i].startx, ref xcoord);
                outfile += ClickAtY(wires[i].starty);
                outfile += MoveTo(wires[i].endx, ref xcoord);
                outfile += ClickAtY(wires[i].endy);
            }
        }

        private static string ClickAtY(int Ycoord)
        {
            int coord = 415 + Ycoord * VMul;
            string outS = "";

            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 840 : " + coord + " : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 840 : " + coord + " : LeftButtonDown : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 840 : " + coord + " : LeftButtonUp : 0 : 0 : 0" + "\r\n";

            return outS;
        }

        private static string MoveTo(int x, ref int xcoord)
        {
            string outfile = "";
            while (xcoord != x)
            {
                if (xcoord < x)
                {
                    //MoveRight
                    outfile += MoverRight( ref xcoord);
                }
                if (xcoord > x)
                {
                    //MoveLeft
                    outfile += MoverLeft( ref xcoord);
                }
            }
            return outfile;
        }

        private static string MoverRight(ref int xcoord)
        {
            string outfile = "";

            outfile += "DELAY : " + dDelay.ToString() + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyDown" + "\r\n";
            outfile += "DELAY : " + dDelay.ToString() + "\r\n";
            outfile += "Keyboard : D : KeyDown" + "\r\n";
            outfile += "DELAY : " + MoveSpeed.ToString() + "\r\n";
            outfile += "Keyboard : D : KeyUp" + "\r\n";
            outfile += "DELAY : " + dDelay.ToString() + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyUp" + "\r\n";

            xcoord++;
            return outfile;
        }

        private static string MoverLeft(ref int xcoord)
        {
            string outfile = "";

            outfile += "DELAY : " + dDelay.ToString() + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyDown" + "\r\n";
            outfile += "DELAY : " + dDelay.ToString() + "\r\n";
            outfile += "Keyboard : A : KeyDown" + "\r\n";
            outfile += "DELAY : " + MoveSpeed.ToString() + "\r\n";
            outfile += "Keyboard : A : KeyUp" + "\r\n";
            outfile += "DELAY : " + dDelay.ToString() + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyUp" + "\r\n";

            xcoord--;
            return outfile;
        }

        private static string StartClic()
        {
            string outS = "";

            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";

            return outS;
        }
        private static string PlaceAtY(int Ycoord)
        {
            int coord = 415 + Ycoord * VMul;
            string outS = "";

            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 865 : " + coord + " : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 865 : " + coord + " : LeftButtonDown : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay.ToString() + "\r\n";
            outS += "Mouse : 865 : " + coord + " : LeftButtonUp : 0 : 0 : 0" + "\r\n";

            return outS;
        }

        private static string SelectTool(string ToolName)
        {
            string outStr = "";
            switch (ToolName)
            {
                case "OR_GATE":
                    outStr += GenKeyPress("D1");
                    outStr += GenKeyPress("D2");
                    break;
                case "NOT_GATE":
                    outStr += GenKeyPress("D1");
                    outStr += GenKeyPress("D3");
                    break;
                case "BUTTON":
                    outStr += GenKeyPress("D1");
                    outStr += GenKeyPress("D4");
                    break;
                case "BLUB":
                    outStr += GenKeyPress("D1");
                    outStr += GenKeyPress("D5");
                    break;
                case "D_TRIG":
                    outStr += GenKeyPress("D1");
                    outStr += GenKeyPress("D6");
                    break;
                case "AND_GATE":
                    outStr += GenKeyPress("D1");
                    outStr += GenKeyPress("D7");
                    break;
                default:
                    break;
            }
            return outStr;
        }

        private static string GenKeyPress(string KeyName)
        {
            string outS = "";

            outS += "DELAY : "+ dDelay.ToString() + "\r\n";
            outS += "Keyboard : " + KeyName + " : KeyDown" + "\r\n";
            outS += "DELAY : "+ dDelay.ToString() + "\r\n";
            outS += "Keyboard : " + KeyName + " : KeyUp" + "\r\n";

            return outS;
        }
        static void LoadFile(List<StarBoundNode> nodes, List<StarboundWire> wires,string filename)
        {
            string[] indat = System.IO.File.ReadAllLines(filename);

            for (int i = 0; i < indat.Length; i++)
            {
                if (indat[i].Split(':')[0] == "W")
                    wires.Add(new StarboundWire(indat[i]));
                if (indat[i].Split(':')[0] == "N")
                    nodes.Add(new StarBoundNode(indat[i]));
            }
        }
    }
}