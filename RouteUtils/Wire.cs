using System.Linq;

namespace RouteUtils
{
    public class Wire
    {
        public readonly string StartName;
        public readonly string EndName;

        public int StartX;
        public int StartY;
        public int EndX;
        public int EndY;

        public int[] WirePointX;
        public int[] WirePointY;
        public int[] WirePointZ;
        public bool[] Rep;
        private bool[] _canRep;
        public string[] RepNp;
        public bool Synced;

        public int CalcRepCount()
        {
            return Rep.Count(t => t);
        }

        public Wire(string stName, string edName)
        {
            StartName = stName;
            EndName = edName;
        }

        public void PlaceRepeaters()
        {
            Rep = new bool[WirePointX.Length];
            _canRep = new bool[WirePointX.Length];
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
                if (_canRep[p])
                {
                    PlaceRepeater(p);
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
            _canRep[0] = false;
            _canRep[WirePointX.Length - 1] = false;
            for (int i = 1; i < WirePointX.Length-1; i++)
            {
                _canRep[i] = false;
                if (WirePointX[i - 1] == WirePointX[i + 1])
                    _canRep[i] = true;
                if (WirePointY[i - 1] == WirePointY[i + 1])
                    _canRep[i] = true;
            }
        }

        private int PlaceReapeterForward(int p)
        {
            bool placed = false;
            while (!placed)
            {
                if (_canRep[p])
                {
                    PlaceRepeater(p);
                    placed = true;
                }
                else
                {
                    p++;
                }
            }
            return p;
        }

        private void PlaceRepeater(int p)
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

        public void RepCompincate(int p)
        {
            int cpoint = 1;
            while(p>0)
            {
                if (!Rep[cpoint])
                {
                    if (_canRep[cpoint])
                    {
                        PlaceRepeater(cpoint);
                        p--;
                    }
                }
                cpoint++;
            }
        }
    }
}
