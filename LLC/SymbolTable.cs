using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLC
{
    public class WireObj
    {
        public string name;
        public int wide;
    }

    public class PortObj
    {
        public string name;
        public int wide;
        public string type;
    }

    public class ConnObj
    {
        public string from;
        public string to;
        public string FromPort;
        public string ToPort;

        public override string ToString()
        {
            return string.Format("{0}:{1}-{2}:{3}", from, FromPort, to, ToPort);
        }
    }

    public class ConstObj
    {
        public string name;
        public int wide;
        public int val;
    }

    public class SymbolTable
    {
        public List<WireObj> WireObjs = new List<WireObj>();
        public List<PortObj> PortObjs = new List<PortObj>();
        public List<ConstObj> ConstObjs = new List<ConstObj>();
        public List<ConnObj> conections = new List<ConnObj>();
        public List<BopObj> Bops = new List<BopObj>();
        private Parser parser;
        public int LastUid;

        public int GetUID()
        {
            int rUid = LastUid;
            LastUid++;
            return rUid;
        }

        public SymbolTable(Parser parser)
        {
            // TODO: Complete member initialization
            this.parser = parser;
        }

        internal void OpenScope()
        {
            //throw new NotImplementedException();
        }

        internal void CloseScope()
        {
            //throw new NotImplementedException();
        }

        internal void NewWire(string name, int wide)
        {
            WireObjs.Add(new WireObj { name = name ,wide = wide});
        }

        internal void NewPort(string name, string type, int wide)
        {
            PortObjs.Add(new PortObj { name = name, type = type, wide = wide });
        }

        internal int bitConv(string p)
        {
            p.Reverse();
            int oint =0;
            foreach (char c in p)
            {
                if (c == '1')
                {
                    oint = (oint << 1) & 1;
                }
                else
                {
                    oint = oint << 1;
                }
            }
            //oint = oint >> 1;
            return oint;
        }

        internal int hexConv(string p)
        {
            p = p.Replace("0", "0000");
            p = p.Replace("1", "Z").Replace("Z", "0001");
            p = p.Replace("2", "0010");
            p = p.Replace("3", "0011");
            p = p.Replace("4", "0100");
            p = p.Replace("5", "0101");
            p = p.Replace("6", "0110");
            p = p.Replace("7", "0111");
            p = p.Replace("8", "1000");
            p = p.Replace("9", "1001");
            p = p.Replace("A", "1010");
            p = p.Replace("B", "1011");
            p = p.Replace("C", "1100");
            p = p.Replace("D", "1101");
            p = p.Replace("E", "1110");
            p = p.Replace("F", "1111");
            return bitConv(p);
        }

        internal string newConst(int wide, int tval)
        {
            string id = "Const" + GetUID();
            ConstObjs.Add(new ConstObj { name = id, val = tval, wide = wide });
            return id;
        }

        internal void NewWire(string wfrom, string wto,string wfromp,string wtop)
        {
            conections.Add(new ConnObj { from = wfrom, to = wto ,FromPort = wfromp,ToPort = wtop });
        }

        internal string NewBOP(string boptype)
        {
            string ID = boptype + "_" + GetUID();
            Bops.Add(new BopObj { Name = ID, opType = boptype });
            return ID;
        }
    }
}
