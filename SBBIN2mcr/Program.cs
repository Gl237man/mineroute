using System.Collections.Generic;
using System.IO;

namespace SBBIN2mcr
{
    internal static class Program
    {
        private const int Syncpoint = 40;
        private const int DDelay = 85;
        private const int Dmelay = 50;
        private const int VMul = 25;
        private const int MoveSpeed = 88;

        private const int X0 = 440;
        private static int _totalmoves;
        private static int _clevel = 2;

        private static void Main(string[] args)
        {
            var nodes = new List<StarBoundNode>();
            var wires = new List<StarboundWire>();
            if (args.Length > 0)
            {
                LoadFile(nodes, wires, args[0]+".SBBIN");
            }
            else
            {
                LoadFile(nodes, wires, "test_D.SBBIN");    
            }
            

            string outfile = "";
            outfile += StartClic();
            int xcoord;
            foreach (StarBoundNode t in nodes)
            {
                string toolName = t.NodeType;

                outfile += SyncToStart();
                xcoord = -1;

                outfile += SelectTool(toolName);

                outfile += MoveTo(t.Xcoord, ref xcoord);

                outfile += PlaceAtY(t.Ycoord);
            }
            //Select zero tool
            outfile += GenKeyPress("D1");

            foreach (StarboundWire t in wires)
            {
                outfile += SyncToStart();
                xcoord = -1;
                outfile += MoveTo(t.Startx, ref xcoord);
                outfile += ClickAtY(t.Starty);
                outfile += SyncToStart();
                xcoord = -1;
                outfile += MoveTo(t.Endx, ref xcoord);
                outfile += ClickAtY(t.Endy);
            }

            File.WriteAllText("T.mcr", outfile);
        }

        private static string SetLevelAtY(int ycoord)
        {
            string outfile = "";
            int needlevel = ycoord/7;

            while (_clevel != needlevel)
            {
                if (_clevel < needlevel)
                {
                    outfile += LevelDown();
                    _clevel++;
                }
                if (_clevel > needlevel)
                {
                    outfile += LevelUp();
                    _clevel--;
                }
            }

            return outfile;
        }

        private static string LevelUp()
        {
            var outfile = "";
            outfile += "DELAY : 100" + "\r\n";
            outfile += "Keyboard : Space : KeyDown" + "\r\n";
            outfile += "DELAY : 1000" + "\r\n";
            outfile += "Keyboard : Space : KeyUp" + "\r\n";
            outfile += "DELAY : 500" + "\r\n";
            return outfile;
        }

        private static string LevelDown()
        {
            var outfile = "";
            outfile += "DELAY : 100" + "\r\n";
            outfile += "Keyboard : S : KeyDown" + "\r\n";
            outfile += "DELAY : 100" + "\r\n";
            outfile += "Keyboard : Space : KeyDown" + "\r\n";
            outfile += "DELAY : 200" + "\r\n";
            outfile += "Keyboard : Space : KeyUp" + "\r\n";
            outfile += "DELAY : 100" + "\r\n";
            outfile += "Keyboard : S : KeyUp" + "\r\n";
            outfile += "DELAY : 500" + "\r\n";
            return outfile;
        }

        private static string ClickAtY(int ycoord)
        {
            string outS = "";
            outS += SetLevelAtY(ycoord);

            ycoord = ycoord - _clevel*8;
            int coord = X0 + ycoord*VMul;


            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 840 : " + coord + " : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 840 : " + coord + " : LeftButtonDown : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + DDelay + "\r\n";
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
                    outfile += MoverRight(ref xcoord);
                }
                if (xcoord > x)
                {
                    //MoveLeft
                    outfile += MoverLeft(ref xcoord);
                }
            }
            return outfile;
        }

        private static string SyncToStart(int xcoord)
        {
            string outfile = "";


            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : A : KeyDown" + "\r\n";
            outfile += "DELAY : " + (10000) + "\r\n";
            outfile += "Keyboard : A : KeyUp" + "\r\n";


            return outfile;
        }

        private static string SyncToStart()
        {
            string outfile = "";


            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : A : KeyDown" + "\r\n";
            outfile += "DELAY : " + (3000) + "\r\n";
            outfile += "Keyboard : A : KeyUp" + "\r\n";


            return outfile;
        }

        private static string MoverRight(ref int xcoord)
        {
            string outfile = "";

            _totalmoves++;
            if (_totalmoves > Syncpoint)
            {
                outfile += SyncToStart(xcoord);
                xcoord = -1;
                _totalmoves = 0;
            }


            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyDown" + "\r\n";
            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : D : KeyDown" + "\r\n";
            outfile += "DELAY : " + MoveSpeed + "\r\n";
            outfile += "Keyboard : D : KeyUp" + "\r\n";
            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyUp" + "\r\n";

            xcoord++;
            return outfile;
        }

        private static string MoverLeft(ref int xcoord)
        {
            string outfile = "";

            _totalmoves++;
            if (_totalmoves > Syncpoint)
            {
                outfile += SyncToStart(xcoord);
                xcoord = -1;
                _totalmoves = 0;
            }

            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyDown" + "\r\n";
            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : A : KeyDown" + "\r\n";
            outfile += "DELAY : " + MoveSpeed + "\r\n";
            outfile += "Keyboard : A : KeyUp" + "\r\n";
            outfile += "DELAY : " + Dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyUp" + "\r\n";

            xcoord--;
            return outfile;
        }

        private static string StartClic()
        {
            string outS = "";

            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";

            return outS;
        }

        private static string PlaceAtY(int Ycoord)
        {
            string outS = "";
            outS += SetLevelAtY(Ycoord);

            Ycoord = Ycoord - _clevel*8;

            int coord = X0 + Ycoord*VMul;


            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 865 : " + coord + " : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 865 : " + coord + " : LeftButtonDown : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Mouse : 865 : " + coord + " : LeftButtonUp : 0 : 0 : 0" + "\r\n";

            return outS;
        }

        private static string SelectTool(string toolName)
        {
            string outStr = "";
            switch (toolName)
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
            }
            return outStr;
        }

        private static string GenKeyPress(string keyName)
        {
            string outS = "";

            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Keyboard : " + keyName + " : KeyDown" + "\r\n";
            outS += "DELAY : " + DDelay + "\r\n";
            outS += "Keyboard : " + keyName + " : KeyUp" + "\r\n";

            return outS;
        }

        private static void LoadFile(List<StarBoundNode> nodes, List<StarboundWire> wires, string filename)
        {
            string[] indat = File.ReadAllLines(filename);

            foreach (string t in indat)
            {
                if (t.Split(':')[0] == "W")
                    wires.Add(new StarboundWire(t));
                if (t.Split(':')[0] == "N")
                    nodes.Add(new StarBoundNode(t));
            }
        }
    }
}