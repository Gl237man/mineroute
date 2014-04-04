using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnetLutDecomposite
{
    class Mnet
    {
        public List<Node> nodes;
        public List<Wire> wires;

        public void ReadMnetFileBl(string FileName, BinLib.Blib BL)
        {
            nodes = new List<Node>();
            wires = new List<Wire>();
            string[] tstr = BL.ReadAllLines(FileName);

            for (int i = 0; i < tstr.Length; i++)
            {
                tstr[i] = tstr[i].Replace("cin", "datac");
            }

            for (int i = 0; i < tstr.Length; i++)
            {
                if (tstr[i].Split(':')[0] == "NODE")
                {
                    Node N = new Node();
                    N.ReadFromString(tstr[i]);
                    nodes.Add(N);
                }
                if (tstr[i].Split(':')[0] == "WIRE")
                {
                    Wire W = new Wire();
                    W.ReadFromString(tstr[i]);
                    wires.Add(W);
                }
            }
        }

        public void ReadMnetFile(string FileName)
        {
            nodes = new List<Node>();
            wires = new List<Wire>();
            string[] tstr = System.IO.File.ReadAllLines(FileName);

            for (int i = 0; i < tstr.Length; i++)
            {
                tstr[i] = tstr[i].Replace("cin", "datac");
            }

                for (int i = 0; i < tstr.Length; i++)
                {
                    if (tstr[i].Split(':')[0] == "NODE")
                    {
                        Node N = new Node();
                        N.ReadFromString(tstr[i]);
                        nodes.Add(N);
                    }
                    if (tstr[i].Split(':')[0] == "WIRE")
                    {
                        Wire W = new Wire();
                        W.ReadFromString(tstr[i]);
                        wires.Add(W);
                    }
                }
        }
        public void RemoveNode(string NodeName)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeName == NodeName)
                {
                    nodes.RemoveAt(i);
                    return;
                }
            }
        }
        public void RemoveWireTo(string name,string port)
        {
            for (int i = 0; i < wires.Count; i++)
            {
                if ((wires[i].DistName == name) && (wires[i].DistPort == port))
                {
                    wires.RemoveAt(i);
                    return;
                }
            }
        }
        public void RemoveWireFrom(string name, string port)
        {
            for (int i = 0; i < wires.Count; i++)
            {
                if ((wires[i].SrcName == name) && (wires[i].SrcPort == port))
                {
                    wires.RemoveAt(i);
                    return;
                }
            }
        }
        public List<Node> GetLuts()
        {
            List<Node> Luts = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].IsLut())
                {
                    Luts.Add(nodes[i]);
                }
            }
            return Luts;
        }

        internal void RenameElement(string From, string To)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeName == From)
                    nodes[i].NodeName = To;
            }
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].DistName == From)
                    wires[i].DistName = To;
                if (wires[i].SrcName == From)
                    wires[i].SrcName = To;
            }
        }

        internal Wire FindWireFrom(string NodeName)
        {
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].SrcName == NodeName)
                {
                    return wires[i];
                }
            }
            return null;
        }

        internal Wire FindWireFromPort(string NodeName, string PortName)
        {
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].SrcName == NodeName && wires[i].SrcPort == PortName)
                {
                    return wires[i];
                }
            }
            return null;
        }

        internal Wire FindWireTo(string NodeName)
        {
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].DistName == NodeName)
                {
                    return wires[i];
                }
            }
            return null;
        }
        internal Wire FindWireToPort(string NodeName,string PortName)
        {
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].DistName == NodeName && wires[i].DistPort == PortName)
                {
                    return wires[i];
                }
            }
            return null;
        }

        internal string GetSting()
        {
            string ostr = "";
            for (int i = 0; i < nodes.Count; i++)
            {
                ostr += nodes[i].ToString() + "\r\n";
            }

            for (int i = 0; i < wires.Count; i++)
            {
                ostr += wires[i].ToString() + "\r\n";
            }

            return ostr;
        }

        internal Node FindNode(string p)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeName == p)
                    return nodes[i];
            }
            return null;
        }
    }
}
