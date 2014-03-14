using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarboundExport
{
    class StarBoundPort
    {
        private int xshift;
        private int yshift;
        public string PortName;
        public int xcoord
        {
            get { return NodeOwner.xcoord + xshift; }
            set { xshift = value; }
        }
        public int ycoord
        {
            get { return NodeOwner.ycoord + yshift; }
            set { yshift = value; }
        }
        public StarBoundNode NodeOwner;
    }
    class StarBoundNode
    {
        public string NodeType;
        public string NodeID;
        public int xcoord;
        public int ycoord;
        public List<StarBoundPort> Ports;

        public string ToString()
        {
            return "N:" + NodeType + ":" + xcoord.ToString() + ":" + ycoord.ToString();
        }

        public StarBoundNode(string type, string ID ,int x,int y)
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
                    Ports.Add(new StarBoundPort { PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this });
                    break;
                case "VCC":
                    NodeType = "NOT_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort { PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this });
                    break;
                case "INPort":
                    NodeType = "BUTTON";//В будующем заменю на переключатели или двери если нет переключателей
                    //make ports
                    Ports.Add(new StarBoundPort { PortName = "O0", xcoord = 1, ycoord = 1 ,NodeOwner = this});
                    break;
                case "OUTPort":
                    NodeType = "BLUB";
                    //make ports
                    Ports.Add(new StarBoundPort { PortName = "I0", xcoord = 0, ycoord = 0 ,NodeOwner = this});
                    break;
                case "TRIG_D":
                    NodeType = "D_TRIG";
                    //make ports
                    Ports.Add(new StarBoundPort { PortName = "clk", xcoord = 0, ycoord = -1, NodeOwner = this });
                    Ports.Add(new StarBoundPort { PortName = "datain", xcoord = 0, ycoord = 1, NodeOwner = this });
                    Ports.Add(new StarBoundPort { PortName = "regout", xcoord = 2, ycoord = 0, NodeOwner = this });
                    break;
                case "OR":
                    NodeType = "OR_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort { PortName = "I0", xcoord = 0, ycoord = -1, NodeOwner = this });
                    Ports.Add(new StarBoundPort { PortName = "I1", xcoord = 0, ycoord = 1, NodeOwner = this });
                    Ports.Add(new StarBoundPort { PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this });
                    break;
                case "AND":
                    NodeType = "AND_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort { PortName = "I0", xcoord = 0, ycoord = -1, NodeOwner = this });
                    Ports.Add(new StarBoundPort { PortName = "I1", xcoord = 0, ycoord = 1, NodeOwner = this });
                    Ports.Add(new StarBoundPort { PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this });
                    break;
                case "NOT":
                    NodeType = "NOT_GATE";
                    //make ports
                    Ports.Add(new StarBoundPort { PortName = "I0", xcoord = 0, ycoord = 0, NodeOwner = this });
                    Ports.Add(new StarBoundPort { PortName = "O0", xcoord = 2, ycoord = 0, NodeOwner = this });
                    break;
                default:
                    
                    break;
            }
        }
    }
}
