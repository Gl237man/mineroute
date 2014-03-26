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
        public bool[] Rep;
        public bool[] CanRep;
        public string[] RepNp;

        public Wire(string stName, string edName)
        {
            StartName = stName;
            EndName = edName;
        }

        public void PlaceRepeaters()
        {
            Rep = new bool[WirePointX.Length];
            CanRep = new bool[WirePointX.Length];
            RepNp = new string[WirePointX.Length];
            CalcPlaceMap();
            int pos = PlaceReapeterForward(1);
            int lastpos = 0;
            while (pos < WirePointX.Length)
            {
                if (lastpos < 15)
                {
                    lastpos++;
                    pos++;
                }
                else
                {
                    pos = PlaceReapeterBackword(pos);
                    lastpos = 0;
                }
            }
            PlaceReapeterBackword(pos - 1);
        }

        private int PlaceReapeterBackword(int p)
        {
            bool placed = false;
            while (!placed)
            {
                if (CanRep[p])
                {
                    PlaceRepeaters(p);
                    placed = true;
                }
                else
                {
                    p--;
                }
            }
            return p;
        }

        private void CalcPlaceMap()
        {
            CanRep[0] = false;
            CanRep[WirePointX.Length - 1] = false;
            for (int i = 1; i < WirePointX.Length-1; i++)
            {
                CanRep[i] = false;
                if (WirePointX[i - 1] == WirePointX[i + 1])
                    CanRep[i] = true;
                if (WirePointY[i - 1] == WirePointY[i + 1])
                    CanRep[i] = true;
            }
        }

        private int PlaceReapeterForward(int p)
        {
            bool placed = false;
            while (!placed)
            {
                if (CanRep[p])
                {
                    PlaceRepeaters(p);
                    placed = true;
                }
                else
                {
                    p++;
                }
            }
            return p;
        }

        private void PlaceRepeaters(int p)
        {
            Rep[p] = true;
            if (WirePointX[p - 1] == WirePointX[p + 1])
            {
                if (WirePointY[p - 1] > WirePointY[p + 1])
                {
                    RepNp[p] = "^";
                }
                else
                {
                    RepNp[p] = "v";
                }
            }
            else
            {
                if (WirePointX[p - 1] > WirePointX[p + 1])
                {
                    RepNp[p] = "<";
                }
                else
                {
                    RepNp[p] = ">";
                }
            }
        }
    }
}
