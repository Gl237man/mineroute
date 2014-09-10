using System.Collections.Generic;
using System.Linq;

namespace MnetLutDecomposite
{
    class Mnet
    {
        public List<Node> Nodes;
        public List<Wire> Wires;

        public void ReadMnetFileBl(string fileName, BinLib.Blib bl)
        {
            Nodes = new List<Node>();
            Wires = new List<Wire>();
            string[] tstr = bl.ReadAllLines(fileName);

            for (int i = 0; i < tstr.Length; i++)
            {
                tstr[i] = tstr[i].Replace("cin", "datac");
            }

            foreach (string t in tstr)
            {
                if (t.Split(':')[0] == "NODE")
                {
                    var n = new Node();
                    n.ReadFromString(t);
                    Nodes.Add(n);
                }
                if (t.Split(':')[0] == "WIRE")
                {
                    var w = new Wire();
                    w.ReadFromString(t);
                    Wires.Add(w);
                }
            }
        }

        public void ReadMnetFile(string fileName)
        {
            Nodes = new List<Node>();
            Wires = new List<Wire>();
            string[] tstr = System.IO.File.ReadAllLines(fileName);

            for (int i = 0; i < tstr.Length; i++)
            {
                tstr[i] = tstr[i].Replace("cin", "datac");
            }

                foreach (string str in tstr)
                {
                    if (str.Split(':')[0] == "NODE")
                    {
                        var n = new Node();
                        n.ReadFromString(str);
                        Nodes.Add(n);
                    }
                    if (str.Split(':')[0] == "WIRE")
                    {
                        var w = new Wire();
                        w.ReadFromString(str);
                        Wires.Add(w);
                    }
                }
        }
        public void RemoveNode(string nodeName)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].NodeName == nodeName)
                {
                    Nodes.RemoveAt(i);
                    return;
                }
            }
        }
        public void RemoveWireTo(string name,string port)
        {
            for (int i = 0; i < Wires.Count; i++)
            {
                if ((Wires[i].DistName == name) && (Wires[i].DistPort == port))
                {
                    Wires.RemoveAt(i);
                    return;
                }
            }
        }
        public void RemoveWireFrom(string name, string port)
        {
            for (int i = 0; i < Wires.Count; i++)
            {
                if ((Wires[i].SrcName == name) && (Wires[i].SrcPort == port))
                {
                    Wires.RemoveAt(i);
                    return;
                }
            }
        }
        public List<Node> GetLuts()
        {
            return Nodes.Where(t => t.IsLut()).ToList();
        }

        internal void RenameElement(string @from, string to)
        {
            foreach (var node in Nodes)
            {
                if (node.NodeName == @from)
                    node.NodeName = to;
            }
            foreach (Wire wire in Wires)
            {
                if (wire.DistName == @from)
                    wire.DistName = to;
                if (wire.SrcName == @from)
                    wire.SrcName = to;
            }
        }

        internal Wire FindWireFrom(string nodeName)
        {
            return Wires.FirstOrDefault(t => t.SrcName == nodeName);
        }

        internal Wire FindWireFromPort(string nodeName, string portName)
        {
            return Wires.FirstOrDefault(t => t.SrcName == nodeName && t.SrcPort == portName);
        }

        internal Wire FindWireTo(string nodeName)
        {
            return Wires.FirstOrDefault(t => t.DistName == nodeName);
        }

        internal Wire FindWireToPort(string nodeName,string portName)
        {
            return Wires.FirstOrDefault(t => t.DistName == nodeName && t.DistPort == portName);
        }

        internal string GetSting()
        {
            string ostr = Nodes.Aggregate("", (current, t) => current + (t.ToString() + "\r\n"));

            return Wires.Aggregate(ostr, (current, t) => current + (t.ToString() + "\r\n"));
        }

        internal Node FindNode(string p)
        {
            return Nodes.FirstOrDefault(t => t.NodeName == p);
        }
    }
}
