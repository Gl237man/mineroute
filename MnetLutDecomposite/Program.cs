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
                file = "test2";
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
            for (int i = 0; i < Luts.Count; i++)
            {
                Mnet Lnet = new Mnet();
                Lnet.ReadMnetFile(@"MNETLib\" + Luts[i].GetLutKey().Substring(0, 1) + @"\lut_" + Luts[i].GetLutKey() + ".MNET");
                LutsMnet.Add(Lnet);
            }
            RenameLutNodes(LutsMnet);
            RemoveLutFromMainNet(MainNet,Luts);
            ReplacePortsByCpoints(PortsRep, LutsMnet, Luts);

            RpeplaceWireToCpoints(PortsRep);
            CombineAllWiresAndNodes(LutsMnet);
            string ExportStr = MainNet.GetSting();
            System.IO.File.WriteAllText(file + @"_D.MNET", ExportStr);
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
