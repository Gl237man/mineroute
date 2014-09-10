namespace SBBIN2mcr
{
    internal class StarBoundPort
    {
        public StarBoundNode NodeOwner;
        public string PortName;
        private int xshift;
        private int yshift;

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
    }
}