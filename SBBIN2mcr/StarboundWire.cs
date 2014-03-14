using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarboundExport
{
    class StarboundWire
    {
        public int startx;
        public int starty;

        public int endx;
        public int endy;

        public string ToString()
        {
            return "W:" + startx.ToString() + ":" + starty.ToString() + ":" + endx.ToString() + ":" + endy.ToString();
        }
    }
}
