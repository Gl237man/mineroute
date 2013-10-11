using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteUtils
{
    public class Wire
    {
        public string StartName;
        public string EndName;

        public int StartX;
        public int StartY;
        public int EndX;
        public int EndY;

        public int[] WirePointX;
        public int[] WirePointY;
        public int[] WirePointZ;

        public Wire(string stName, string edName)
        {
            StartName = stName;
            EndName = edName;
        }
    }
}
