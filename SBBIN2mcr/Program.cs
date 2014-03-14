using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBBIN2mcr
{
    class Program
    {
        const int dDelay = 100;
        static void Main(string[] args)
        {
            List<StarBoundNode> nodes = new List<StarBoundNode>();
            List<StarboundWire> wires = new List<StarboundWire>();
            LoadFile(nodes, wires, "test_D.SBBIN");

            string outfile = "";
            int xcoord = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                string ToolName = nodes[i].NodeType;
                //selectTool
                outfile += SelectTool(ToolName);
            }
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
