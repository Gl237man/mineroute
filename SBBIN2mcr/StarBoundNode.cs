using System;

namespace SBBIN2mcr
{
    internal class StarBoundNode
    {
        public readonly string NodeType;
        public readonly int Xcoord;
        public readonly int Ycoord;

        public StarBoundNode(string frstr)
        {
            NodeType = frstr.Split(':')[1];
            Xcoord = Convert.ToInt32(frstr.Split(':')[2]);
            Ycoord = Convert.ToInt32(frstr.Split(':')[3]);
        }

/*
        public StarBoundNode(string type, int x, int y)
        {
            List<StarBoundPort> ports = new List<StarBoundPort>();
            Xcoord = x;
            Ycoord = y;
            switch (type)
            {
                case "GND":
                    NodeType = "OR_GATE";
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "VCC":
                    NodeType = "NOT_GATE";
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "INPort":
                    NodeType = "BUTTON"; //В будующем заменю на переключатели или двери если нет переключателей
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "O0", xcoord = 1, ycoord = 1, NodeOwner = this});
                    break;
                case "OUTPort":
                    NodeType = "BLUB";
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = 0, NodeOwner = this});
                    break;
                case "TRIG_D":
                    NodeType = "D_TRIG";
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "clk", xcoord = 0, ycoord = -1, NodeOwner = this});
                    ports.Add(new StarBoundPort {PortName = "datain", xcoord = 0, ycoord = 1, NodeOwner = this});
                    ports.Add(new StarBoundPort {PortName = "regout", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "OR":
                    NodeType = "OR_GATE";
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = -1, NodeOwner = this});
                    ports.Add(new StarBoundPort {PortName = "I1", xcoord = 0, ycoord = 1, NodeOwner = this});
                    ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "AND":
                    NodeType = "AND_GATE";
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = -1, NodeOwner = this});
                    ports.Add(new StarBoundPort {PortName = "I1", xcoord = 0, ycoord = 1, NodeOwner = this});
                    ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                case "NOT":
                    NodeType = "NOT_GATE";
                    //make ports
                    ports.Add(new StarBoundPort {PortName = "I0", xcoord = 0, ycoord = 0, NodeOwner = this});
                    ports.Add(new StarBoundPort {PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this});
                    break;
                default:

                    break;
            }
        }
*/

        public override string ToString()
        {
            return "N:" + NodeType + ":" + Xcoord + ":" + Ycoord;
        }
    }
}