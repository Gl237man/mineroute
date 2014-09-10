using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinLib;

namespace MnetLutDecomposite
{
    internal static class Program
    {
        private static Mnet _mainNet;
        private static int _glAnDindex;
        private static int _glORindex;
        private static int _glDuPindex;
        private static int _glNoTindex;

        private static void Main(string[] args)
        {
            string file = args.Length == 0 ? "test" : args[0];

            _mainNet = new Mnet();
            _mainNet.ReadMnetFile(file + @".MNET");

            var portsRep = new List<Cpoint>();
            List<Node> luts = _mainNet.GetLuts();
            var lutsMnet = new List<Mnet>();
            var bl = new Blib();
            bl.Load("MNETLib.BinLib");
            //"MNETLib.BinLib"

            foreach (Node node in luts)
            {
                var lnet = new Mnet();
                node.HaveCout = СheckCout(node, _mainNet.Wires);

                lnet.ReadMnetFileBl(@"lut_" + node.GetLutKey() + ".MNET", bl);
                if (!node.HaveCout)
                {
                    lutsMnet.Add(lnet);
                }
                else
                {
                    var lnetC = new Mnet();
                    lnetC.ReadMnetFileBl(@"lutc_" + node.GetLutKey().Substring(2, 2) + ".MNET", bl);
                    Mnet combined = MnetComb(lnet, lnetC);
                    lutsMnet.Add(combined);
                }
            }
            RenameLutNodes(lutsMnet);
            RemoveLutFromMainNet(_mainNet, luts);
            ReplacePortsByCpoints(portsRep, lutsMnet, luts);

            RpeplaceWireToCpoints(portsRep);
            CombineAllWiresAndNodes(lutsMnet);
            string exportStr = _mainNet.GetSting();
            File.WriteAllText(file + @"_D.MNET", exportStr);
        }

        private static Mnet MnetComb(Mnet lnet, Mnet lnetC)
        {
            NodeShiftRename(lnetC);
            int dupC = 0;
            //Удаление Одинаковых Портов
            var nodeToRemove = new List<Node>();
            foreach (Node node in lnet.Nodes)
            {
                nodeToRemove.AddRange(lnetC.Nodes.Where(t => node.NodeName == t.NodeName));
            }
            foreach (Node node in nodeToRemove)
            {
                lnetC.RemoveNode(node.NodeName);
            }

            //Поиск Дублирующих Линий

            var wireToRemove = new List<Wire>();

            for (int i = 0; i < lnet.Wires.Count; i++)
            {
                for (int j = 0; j < lnetC.Wires.Count; j++)
                {
                    if (lnet.Wires[i].SrcName == lnetC.Wires[j].SrcName)
                    {
                        if (lnet.Wires[i].SrcPort == lnetC.Wires[j].SrcPort)
                        {
                            //Создание Dup
                            lnet.Nodes.Add(new Node {NodeName = "DUPC" + dupC, NodeType = "DUP2"});
                            lnet.Wires.Add(new Wire
                            {
                                SrcName = "DUPC" + dupC,
                                SrcPort = "O0",
                                DistName = lnet.Wires[i].DistName,
                                DistPort = lnet.Wires[i].DistPort
                            });
                            lnet.Wires.Add(new Wire
                            {
                                SrcName = "DUPC" + dupC,
                                SrcPort = "O1",
                                DistName = lnetC.Wires[j].DistName,
                                DistPort = lnetC.Wires[j].DistPort
                            });
                            lnet.Wires[i].DistName = "DUPC" + dupC;
                            lnet.Wires[i].DistPort = "I0";
                            wireToRemove.Add(lnetC.Wires[j]);
                            dupC++;
                        }
                    }
                }
            }
            //Удаление Дублирующих линий
            foreach (Wire wire in wireToRemove)
            {
                lnetC.RemoveWireFrom(wire.SrcName, wire.SrcPort);
            }
            //Слияние
            for (int i = 0; i < lnetC.Wires.Count; i++)
            {
                lnet.Wires.Add(lnetC.Wires[i]);
            }

            for (int i = 0; i < lnetC.Nodes.Count; i++)
            {
                lnet.Nodes.Add(lnetC.Nodes[i]);
            }

            return lnet;
        }

        private static void NodeShiftRename(Mnet lnetC)
        {
            foreach (Wire wire in lnetC.Wires)
            {
                if (lnetC.FindNode(wire.DistName).NodeType != "INPort" &&
                    lnetC.FindNode(wire.DistName).NodeType != "OUTPort")
                {
                    wire.DistName = "CC" + wire.DistName;
                }
                if (lnetC.FindNode(wire.SrcName).NodeType != "INPort" &&
                    lnetC.FindNode(wire.SrcName).NodeType != "OUTPort")
                {
                    wire.SrcName = "CC" + wire.SrcName;
                }
            }

            foreach (Node node in lnetC.Nodes)
            {
                if (node.NodeType != "INPort" && node.NodeType != "OUTPort")
                {
                    node.NodeName = "CC" + node.NodeName;
                }
            }
        }

        private static bool СheckCout(Node node, IEnumerable<Wire> list)
        {
            return list.Where(t => node.NodeName == t.SrcName).Any(t => t.SrcPort == "cout");
        }

        private static void CombineAllWiresAndNodes(IEnumerable<Mnet> lutsMnet)
        {
            foreach (Mnet mnet in lutsMnet)
            {
                for (int j = 0; j < mnet.Nodes.Count; j++)
                {
                    _mainNet.Nodes.Add(mnet.Nodes[j]);
                }

                for (int j = 0; j < mnet.Wires.Count; j++)
                {
                    _mainNet.Wires.Add(mnet.Wires[j]);
                }
            }
        }

        private static void RpeplaceWireToCpoints(IEnumerable<Cpoint> portsRep)
        {
            foreach (Cpoint cpoint in portsRep)
            {
                Wire tWire = _mainNet.FindWireToPort(cpoint.Name.Split('-')[0], cpoint.Name.Split('-')[1]);
                if (tWire != null)
                {
                    tWire.DistName = cpoint.DistName;
                    tWire.DistPort = cpoint.DistPort;
                }
                Wire rWire = _mainNet.FindWireFromPort(cpoint.Name.Split('-')[0], cpoint.Name.Split('-')[1]);
                if (rWire != null)
                {
                    rWire.SrcName = cpoint.DistName;
                    rWire.SrcPort = cpoint.DistPort;
                }
            }
        }

        private static void ReplacePortsByCpoints(List<Cpoint> portsRep, List<Mnet> lutsMnet, List<Node> luts)
        {
            for (int i = 0; i < luts.Count; i++)
            {
                List<Node> nports = lutsMnet[i].Nodes.Where(t => t.NodeType == "INPort").ToList();
                List<Node> ouTports = lutsMnet[i].Nodes.Where(t => t.NodeType == "OUTPort").ToList();

                //Создание линков вместо портов
                int i1 = i;
                portsRep.AddRange(from t in nports
                    select lutsMnet[i].FindWireFrom(t.NodeName)
                    into w
                    where w != null
                    select
                        new Cpoint
                        {
                            Name = luts[i1].NodeName + "-" + w.SrcName,
                            DistName = w.DistName,
                            DistPort = w.DistPort
                        });

                int i2 = i;
                portsRep.AddRange(from t in ouTports
                    select lutsMnet[i].FindWireTo(t.NodeName)
                    into w
                    where w != null
                    select
                        new Cpoint
                        {
                            Name = luts[i2].NodeName + "-" + w.DistName,
                            DistName = w.SrcName,
                            DistPort = w.SrcPort
                        });
                //Удаление портов
                foreach (Node node in nports)
                {
                    lutsMnet[i].RemoveNode(node.NodeName);
                }
                foreach (Node node in ouTports)
                {
                    lutsMnet[i].RemoveNode(node.NodeName);
                }
                //Удаление Линков несуществующих портов
                foreach (Node node in nports)
                {
                    lutsMnet[i].RemoveWireFrom(node.NodeName, "O0");
                }
                foreach (Node node in ouTports)
                {
                    lutsMnet[i].RemoveWireTo(node.NodeName, "I0");
                }
            }
        }

        private static void RemoveLutFromMainNet(Mnet mainNet, IEnumerable<Node> luts)
        {
            foreach (Node node in luts)
            {
                mainNet.RemoveNode(node.NodeName);
            }
        }

        private static void RenameLutNodes(IEnumerable<Mnet> lutsMnet)
        {
            foreach (Mnet mnet in lutsMnet)
            {
                foreach (Node node in mnet.Nodes)
                {
                    if (node.NodeType == "AND")
                    {
                        mnet.RenameElement(node.NodeName, "GL_AND_" + _glAnDindex);
                        _glAnDindex++;
                    }
                    if (node.NodeType == "OR")
                    {
                        mnet.RenameElement(node.NodeName, "GL_OR_" + _glORindex);
                        _glORindex++;
                    }
                    if (node.NodeType.StartsWith("DUP"))
                    {
                        mnet.RenameElement(node.NodeName, "GL_DUP_" + _glDuPindex);
                        _glDuPindex++;
                    }
                    if (node.NodeType.StartsWith("NOT"))
                    {
                        mnet.RenameElement(node.NodeName, "GL_NOT_" + _glNoTindex);
                        _glNoTindex++;
                    }
                }
            }
        }
    }
}