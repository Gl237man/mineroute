namespace Mnetsynt3
{
    internal class Node
    {
        public RouteUtils.Node McNode;
        public string NodeName;
        public string NodeType;
        public bool Placed;
        public int X;
        public int Y;
        public int Z;

        public override string ToString()
        {
            return "NODE:" + NodeType + ":" + NodeName;
        }

        public void ReadFromString(string instr)
        {
            string[] tstr = instr.Split(':');
            NodeType = tstr[1];
            NodeName = tstr[2];
        }

        public bool IsLut()
        {
            return NodeType.StartsWith("C2LUT_");
        }

        public string GetLutKey()
        {
            if (IsLut())
            {
                return NodeType.Substring(6, 4);
            }
            return "";
        }
    }
}