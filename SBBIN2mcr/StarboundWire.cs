using System;

namespace SBBIN2mcr
{
    internal class StarboundWire
    {
        public int endx;
        public int endy;
        public int startx;
        public int starty;

        public StarboundWire()
        {
        }

        public StarboundWire(string fromstr)
        {
            startx = Convert.ToInt32(fromstr.Split(':')[1]);
            starty = Convert.ToInt32(fromstr.Split(':')[2]);
            endx = Convert.ToInt32(fromstr.Split(':')[3]);
            endy = Convert.ToInt32(fromstr.Split(':')[4]);
        }

        public override string ToString()
        {
            return "W:" + startx + ":" + starty + ":" + endx + ":" + endy;
        }
    }
}