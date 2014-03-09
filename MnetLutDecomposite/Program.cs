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
        static void Main(string[] args)
        {
            MainNet = new Mnet();
            MainNet.ReadMnetFile(@"test2.MNET");
            
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
                        LutsMnet[i].RenameElement(LutsMnet[i].nodes[j].NodeName, "GL_DUP_" + glORindex);
                        glDUPindex++;
                    }
                }
            }
        }

        
    }
}
