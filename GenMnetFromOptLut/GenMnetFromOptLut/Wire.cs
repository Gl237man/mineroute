namespace GenMnetFromOptLut
{
    class Wire
    {
        public string SrcName;
        public string DistName;
        public string SrcPort;
        public string DistPort;
        public override string ToString()
        {
            return "WIRE:" + SrcName + "-" + SrcPort + ":" + DistName + "-" + DistPort;
        }

    }
}
