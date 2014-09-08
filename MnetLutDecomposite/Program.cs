using System;
using System.Collections.Generic;
using System.Text;

namespace MnetLutDecomposite
{
    class Program
    {
        static Mnet MainNet;
        static int glANDindex = 0;
        static int glORindex = 0;
        static int glDUPindex = 0;
        static int glNOTindex = 0;
        static void Main(string[] args)
        {
            string file = "";
            if (args.Length == 0)
            {
                file = "test";
            }
            else
            {
                file = args[0];
            }

            MainNet = new Mnet();
            MainNet.ReadMnetFile(file + @".MNET");

            List<Cpoint> PortsRep = new List<Cpoint>();
            List<Node> Luts = MainNet.GetLuts();
            List<Mnet> LutsMnet = new List<Mnet>();
            BinLib.Blib Bl = new BinLib.Blib();
            Bl.Load("MNETLib.BinLib");
            //"MNETLib.BinLib"

            for (int i = 0; i < Luts.Count; i++)
            {
                Mnet Lnet = new Mnet();
                Luts[i].HaveCout = СheckCout(Luts[i], MainNet.wires);

                Lnet.ReadMnetFileBl(@"lut_" + Luts[i].GetLutKey() + ".MNET",Bl);
                if (!Luts[i].HaveCout)
                {
                    LutsMnet.Add(Lnet);
                }
                else
                {
                    Mnet LnetC = new Mnet();
                    LnetC.ReadMnetFileBl(@"lutc_" + Luts[i].GetLutKey().Substring(2, 2) + ".MNET",Bl);
                    Mnet Combined = MnetComb(Lnet, LnetC);
                    LutsMnet.Add(Combined);
                }
            }
            RenameLutNodes(LutsMnet);
            RemoveLutFromMainNet(MainNet,Luts);
            ReplacePortsByCpoints(PortsRep, LutsMnet, Luts);

            RpeplaceWireToCpoints(PortsRep);
            CombineAllWiresAndNodes(LutsMnet);
            string ExportStr = MainNet.GetSting();
            System.IO.File.WriteAllText(file + @"_D.MNET", ExportStr);
        }

        private static Mnet MnetComb(Mnet Lnet, Mnet LnetC)
        {
            NodeShiftRename(LnetC);
            int DupC = 0;
            //Удаление Одинаковых Портов
            List<Node> NodeToRemove = new List<Node>();
            for (int i = 0; i < Lnet.nodes.Count; i++)
            {
                for (int j = 0; j < LnetC.nodes.Count; j++)
                {
                    if (Lnet.nodes[i].NodeName == LnetC.nodes[j].NodeName)
                    {
                        NodeToRemove.Add(LnetC.nodes[j]);
                    }
                }
            }
            for (int i = 0; i < NodeToRemove.Count; i++)
            {
                LnetC.RemoveNode(NodeToRemove[i].NodeName);
            }

            //Поиск Дублирующих Линий

            List<Wire> WireToRemove = new List<Wire>();

            for (int i = 0; i < Lnet.wires.Count; i++)
            {
                for (int j = 0; j < LnetC.wires.Count; j++)
                {
                    if (Lnet.wires[i].SrcName == LnetC.wires[j].SrcName)
                    {
                        if (Lnet.wires[i].SrcPort == LnetC.wires[j].SrcPort)
                        {
                            //Создание Dup
                            Lnet.nodes.Add(new Node {NodeName = "DUPC" + DupC,NodeType = "DUP2" });
                            Lnet.wires.Add(new Wire
                            {
                                SrcName = "DUPC" + DupC,
                                SrcPort = "O0",
                                DistName = Lnet.wires[i].DistName,
                                DistPort = Lnet.wires[i].DistPort
                            });
                            Lnet.wires.Add(new Wire
                            {
                                SrcName = "DUPC" + DupC,
                                SrcPort = "O1",
                                DistName = LnetC.wires[j].DistName,
                                DistPort = LnetC.wires[j].DistPort
                            });
                            Lnet.wires[i].DistName = "DUPC" + DupC;
                            Lnet.wires[i].DistPort = "I0";
                            WireToRemove.Add(LnetC.wires[j]);
                            DupC++;
                        }
                    }
                }
            }
            //Удаление Дублирующих линий
            for (int i = 0; i < WireToRemove.Count; i++)
            {
                LnetC.RemoveWireFrom(WireToRemove[i].SrcName, WireToRemove[i].SrcPort);
            }
            //Слияние
            for (int i = 0; i < LnetC.wires.Count; i++)
            {
                Lnet.wires.Add(LnetC.wires[i]);
            }

            for (int i = 0; i < LnetC.nodes.Count; i++)
            {
                Lnet.nodes.Add(LnetC.nodes[i]);
            }

            return Lnet;
        }

        private static void NodeShiftRename(Mnet LnetC)
        {
            for (int i = 0; i < LnetC.wires.Count; i++)
            {
                if (LnetC.FindNode(LnetC.wires[i].DistName).NodeType != "INPort" && LnetC.FindNode(LnetC.wires[i].DistName).NodeType != "OUTPort")
                {
                    LnetC.wires[i].DistName = "CC" + LnetC.wires[i].DistName;
                }
                if (LnetC.FindNode(LnetC.wires[i].SrcName).NodeType != "INPort" && LnetC.FindNode(LnetC.wires[i].SrcName).NodeType != "OUTPort")
                {
                    LnetC.wires[i].SrcName = "CC" + LnetC.wires[i].SrcName;
                }
            }

            for (int i = 0; i < LnetC.nodes.Count; i++)
            {
                if (LnetC.nodes[i].NodeType != "INPort" && LnetC.nodes[i].NodeType != "OUTPort")
                {
                    LnetC.nodes[i].NodeName = "CC" + LnetC.nodes[i].NodeName;
                }
            }
        }

        private static bool СheckCout(Node node, List<Wire> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (node.NodeName == list[i].SrcName)
                {
                    if (list[i].SrcPort == "cout")
                        return true;
                }
            }
            return false;
        }
               
        private static void CombineAllWiresAndNodes(List<Mnet> LutsMnet)
        {
            for (int i = 0; i < LutsMnet.Count; i++)
            {
                for (int j=0;j<LutsMnet[i].nodes.Count;j++)
                {
                    MainNet.nodes.Add(LutsMnet[i].nodes[j]);
                }

                for (int j = 0; j < LutsMnet[i].wires.Count; j++)
                {
                    MainNet.wires.Add(LutsMnet[i].wires[j]);
                }

            }
        }

        private static void RpeplaceWireToCpoints(List<Cpoint> PortsRep)
        {
            for (int i=0;i<PortsRep.Count;i++)
            {
                Wire tWire = MainNet.FindWireToPort(PortsRep[i].Name.Split('-')[0], PortsRep[i].Name.Split('-')[1]);
                if (tWire != null)
                {
                    tWire.DistName = PortsRep[i].DistName;
                    tWire.DistPort = PortsRep[i].DistPort;
                }
                Wire rWire = MainNet.FindWireFromPort(PortsRep[i].Name.Split('-')[0], PortsRep[i].Name.Split('-')[1]);
                if (rWire != null)
                {
                    rWire.SrcName = PortsRep[i].DistName;
                    rWire.SrcPort = PortsRep[i].DistPort;
                }
            }
        }

        private static void ReplacePortsByCpoints(List<Cpoint> PortsRep, List<Mnet> LutsMnet, List<Node> Luts)
        {
            for (int i = 0; i < Luts.Count; i++)
            {
                List<Node> INports = new List<Node>();
                for (int j = 0; j < LutsMnet[i].nodes.Count; j++)
                {
                    if (LutsMnet[i].nodes[j].NodeType == "INPort")
                    {
                        INports.Add(LutsMnet[i].nodes[j]);
                    }
                }
                List<Node> OUTports = new List<Node>();
                for (int j = 0; j < LutsMnet[i].nodes.Count; j++)
                {
                    if (LutsMnet[i].nodes[j].NodeType == "OUTPort")
                    {
                        OUTports.Add(LutsMnet[i].nodes[j]);
                    }
                }
                //Создание линков вместо портов
                for (int j = 0; j < INports.Count; j++)
                {
                    Wire W = LutsMnet[i].FindWireFrom(INports[j].NodeName);
                    if (W != null)
                    {
                        PortsRep.Add(new Cpoint { Name = Luts[i].NodeName + "-" + W.SrcName, DistName = W.DistName, DistPort = W.DistPort });
                    }
                }

                for (int j = 0; j < OUTports.Count; j++)
                {
                    Wire W = LutsMnet[i].FindWireTo(OUTports[j].NodeName);
                    if (W != null)
                    {
                        PortsRep.Add(new Cpoint { Name = Luts[i].NodeName + "-" + W.DistName, DistName = W.SrcName, DistPort = W.SrcPort });
                    }
                }
                //Удаление портов
                for (int j = 0; j < INports.Count; j++)
                {
                    LutsMnet[i].RemoveNode(INports[j].NodeName);
                }
                for (int j = 0; j < OUTports.Count; j++)
                {
                    LutsMnet[i].RemoveNode(OUTports[j].NodeName);
                }
                //Удаление Линков несуществующих портов
                for (int j = 0; j < INports.Count; j++)
                {
                    LutsMnet[i].RemoveWireFrom(INports[j].NodeName, "O0");
                }
                for (int j = 0; j < OUTports.Count; j++)
                {
                    LutsMnet[i].RemoveWireTo(OUTports[j].NodeName,"I0");
                }
            }
        }

        private static void RemoveLutFromMainNet(Mnet MainNet, List<Node> Luts)
        {
            for (int i = 0; i < Luts.Count; i++)
            {
                MainNet.RemoveNode(Luts[i].NodeName);
            }
        }
        private static void RenameLutNodes(List<Mnet> LutsMnet)
        {
            for (int i = 0; i < LutsMnet.Count; i++)
            {
                for (int j = 0; j < LutsMnet[i].nodes.Count; j++)
                {
                    if (LutsMnet[i].nodes[j].NodeType == "AND")
                    {
                        LutsMnet[i].RenameElement(LutsMnet[i].nodes[j].NodeName, "GL_AND_" + glANDindex);
                        glANDindex++;
                    }
                    if (LutsMnet[i].nodes[j].NodeType == "OR")
                    {
                        LutsMnet[i].RenameElement(LutsMnet[i].nodes[j].NodeName, "GL_OR_" + glORindex);
                        glORindex++;
                    }
                    if (LutsMnet[i].nodes[j].NodeType.StartsWith("DUP"))
                    {
                        LutsMnet[i].RenameElement(LutsMnet[i].nodes[j].NodeName, "GL_DUP_" + glDUPindex);
                        glDUPindex++;
                    
                    }
                    if (LutsMnet[i].nodes[j].NodeType.StartsWith("NOT"))
                    {
                        LutsMnet[i].RenameElement(LutsMnet[i].nodes[j].NodeName, "GL_NOT_" + glNOTindex);
                        glNOTindex++;

                    }
                }
            }
        }

        
    }
}
