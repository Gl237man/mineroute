using System;

namespace SBBIN2mcr
{
    internal class StarboundWire
    {
        public readonly int Endx;
        public readonly int Endy;
        public readonly int Startx;
        public readonly int Starty;

        public StarboundWire(string fromstr)
        {
            Startx = Convert.ToInt32(fromstr.Split(':')[1]);
            Starty = Convert.ToInt32(fromstr.Split(':')[2]);
            Endx = Convert.ToInt32(fromstr.Split(':')[3]);
            Endy = Convert.ToInt32(fromstr.Split(':')[4]);
        }

        public override string ToString()
        {
            return "W:" + Startx + ":" + Starty + ":" + Endx + ":" + Endy;
        }
    }
}