namespace GenMnetFromOptLut
{
    class Node
    {
        public string NodeType;
        public string NodeName;
        public override string ToString()
        {
            return "NODE:" + NodeType + ":" + NodeName;
        }

        //NODE:INPort:sig4[1]
    }
}
