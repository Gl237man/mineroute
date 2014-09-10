using System.Collections.Generic;
using System.IO;

namespace SBBIN2mcr
{
    internal class Program
    {
        private const int syncpoint = 40;
        private const int dDelay = 85;
        private const int dmelay = 50;
        private const int VMul = 25;
        private const int MoveSpeed = 88;

        private const int x0 = 440;
        private static int totalmoves;
        private static int Clevel = 2;

        private static void Main(string[] args)
        {
            var nodes = new List<StarBoundNode>();
            var wires = new List<StarboundWire>();
            LoadFile(nodes, wires, "test_D.SBBIN");

            string outfile = "";
            outfile += StartClic();
            int xcoord = -1;
            for (int i = 0; i < nodes.Count; i++)
            {
                string ToolName = nodes[i].NodeType;

                outfile += SyncToStart();
                xcoord = -1;

                outfile += SelectTool(ToolName);

                outfile += MoveTo(nodes[i].xcoord, ref xcoord);

                outfile += PlaceAtY(nodes[i].ycoord);
            }
            //Select zero tool
            outfile += GenKeyPress("D1");

            for (int i = 0; i < wires.Count; i++)
            {
                outfile += SyncToStart();
                xcoord = -1;
                outfile += MoveTo(wires[i].startx, ref xcoord);
                outfile += ClickAtY(wires[i].starty);
                outfile += SyncToStart();
                xcoord = -1;
                outfile += MoveTo(wires[i].endx, ref xcoord);
                outfile += ClickAtY(wires[i].endy);
            }

            File.WriteAllText("T.mcr", outfile);
        }

        private static string SetLevelAtY(int Ycoord)
        {
            string outfile = "";
            int needlevel = Ycoord/7;

            while (Clevel != needlevel)
            {
                if (Clevel < needlevel)
                {
                    outfile += LevelDown();
                    Clevel++;
                }
                if (Clevel > needlevel)
                {
                    outfile += LevelUp();
                    Clevel--;
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

        private static string ClickAtY(int Ycoord)
        {
            string outS = "";
            outS += SetLevelAtY(Ycoord);

            Ycoord = Ycoord - Clevel*8;
            int coord = x0 + Ycoord*VMul;


            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Mouse : 840 : " + coord + " : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Mouse : 840 : " + coord + " : LeftButtonDown : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay + "\r\n";
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


            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : A : KeyDown" + "\r\n";
            outfile += "DELAY : " + (10000) + "\r\n";
            outfile += "Keyboard : A : KeyUp" + "\r\n";


            return outfile;
        }

        private static string SyncToStart()
        {
            string outfile = "";


            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : A : KeyDown" + "\r\n";
            outfile += "DELAY : " + (3000) + "\r\n";
            outfile += "Keyboard : A : KeyUp" + "\r\n";


            return outfile;
        }

        private static string MoverRight(ref int xcoord)
        {
            string outfile = "";

            totalmoves++;
            if (totalmoves > syncpoint)
            {
                outfile += SyncToStart(xcoord);
                xcoord = -1;
                totalmoves = 0;
            }


            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyDown" + "\r\n";
            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : D : KeyDown" + "\r\n";
            outfile += "DELAY : " + MoveSpeed + "\r\n";
            outfile += "Keyboard : D : KeyUp" + "\r\n";
            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyUp" + "\r\n";

            xcoord++;
            return outfile;
        }

        private static string MoverLeft(ref int xcoord)
        {
            string outfile = "";

            totalmoves++;
            if (totalmoves > syncpoint)
            {
                outfile += SyncToStart(xcoord);
                xcoord = -1;
                totalmoves = 0;
            }

            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyDown" + "\r\n";
            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : A : KeyDown" + "\r\n";
            outfile += "DELAY : " + MoveSpeed + "\r\n";
            outfile += "Keyboard : A : KeyUp" + "\r\n";
            outfile += "DELAY : " + dmelay + "\r\n";
            outfile += "Keyboard : ShiftLeft : KeyUp" + "\r\n";

            xcoord--;
            return outfile;
        }

        private static string StartClic()
        {
            string outS = "";

            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Mouse : 200 : 200 : Move : 0 : 0 : 0" + "\r\n";

            return outS;
        }

        private static string PlaceAtY(int Ycoord)
        {
            string outS = "";
            outS += SetLevelAtY(Ycoord);

            Ycoord = Ycoord - Clevel*8;

            int coord = x0 + Ycoord*VMul;


            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Mouse : 865 : " + coord + " : Move : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Mouse : 865 : " + coord + " : LeftButtonDown : 0 : 0 : 0" + "\r\n";
            outS += "DELAY : " + dDelay + "\r\n";
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

            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Keyboard : " + KeyName + " : KeyDown" + "\r\n";
            outS += "DELAY : " + dDelay + "\r\n";
            outS += "Keyboard : " + KeyName + " : KeyUp" + "\r\n";

            return outS;
        }

        private static void LoadFile(List<StarBoundNode> nodes, List<StarboundWire> wires, string filename)
        {
            string[] indat = File.ReadAllLines(filename);

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