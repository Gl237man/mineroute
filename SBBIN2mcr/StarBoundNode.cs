using System;
using System.Collections.Generic;

namespace SBBIN2mcr
{
    internal class StarBoundNode
    {
        public string NodeID;
        public string NodeType;
        public List<StarBoundPort> Ports;
        public int xcoord;
        public int ycoord;

        public StarBoundNode(string frstr)
        {
            NodeType = frstr.Split(':')[1];
            xcoord = Convert.ToInt32(frstr.Split(':')[2]);
            ycoord = Convert.ToInt32(frstr.Split(':')[3]);
        }

        public StarBoundNode(string type, string ID, int x, int y)
        {
            Ports = new List<StarBoundPort>();
            NodeID = ID;
            xcoord = x;
            ycoord = y;
            switch (type)
            {
                case "GND":
                    NodeType = "OR_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "VCC":
                    NodeType = "NOT_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "INPort":
                    NodeType = "BUTTON"; //В будующем заменю на переключатели или двери если нет переключателей
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "O0", xcoord = 1, ycoord = 1, NodeOwner = this});
                    break;
                case "OUTPort":
                    NodeType = "BLUB";
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = 0, NodeOwner = this});
                    break;
                case "TRIG_D":
                    NodeType = "D_TRIG";
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "clk", xcoord = 0, ycoord = -1, NodeOwner = this});
                    Ports.Add(new StarBoundPort {PortName = "datain", xcoord = 0, ycoord = 1, NodeOwner = this});
                    Ports.Add(new StarBoundPort {PortName = "regout", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "OR":
                    NodeType = "OR_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = -1, NodeOwner = this});
                    Ports.Add(new StarBoundPort {PortName = "I1", xcoord = 0, ycoord = 1, NodeOwner = this});
                    Ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "AND":
                    NodeType = "AND_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = -1, NodeOwner = this});
                    Ports.Add(new StarBoundPort {PortName = "I1", xcoord = 0, ycoord = 1, NodeOwner = this});
                    Ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "NOT":
                    NodeType = "NOT_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = 0, NodeOwner = this});
                    Ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                default:

                    break;
            }
        }

        public override string ToString()
        {
            return "N:" + NodeType + ":" + xcoord + ":" + ycoord;
        }
    }
}