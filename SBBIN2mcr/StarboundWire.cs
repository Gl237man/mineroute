using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBBIN2mcr
{
    class StarboundWire
    {
        public int startx;
        public int starty;

        public int endx;
        public int endy;

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

        public string ToString()
        {
            return "W:" + startx.ToString() + ":" + starty.ToString() + ":" + endx.ToString() + ":" + endy.ToString();
        }
    }
}
