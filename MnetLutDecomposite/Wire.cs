namespace MnetLutDecomposite
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
        public void ReadFromString(string instr)
        {
            string[] tstr = instr.Split(':');
            SrcName = tstr[1].Split('-')[0];
            SrcPort = tstr[1].Split('-')[1];
            DistName = tstr[2].Split('-')[0];
            DistPort = tstr[2].Split('-')[1];
        }
    }
}
