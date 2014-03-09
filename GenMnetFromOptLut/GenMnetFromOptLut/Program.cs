using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMnetFromOptLut
{
    class Program
    {
        static List<Node> Nodes;
        static List<Wire> Wires;
        static List<Cpoint> Mcpoint;
        static int GlobalAndIndex;
        static int MatrixSize;
        static void Main(string[] args)
        {
            //string OMnet = GenMnet(@"Result\A\Opt_AF62.txt");
            //string OMnet = GenMnet(@"Result\F\Opt_FCCC.txt");

            for (int i = 1; i < 0xFFFE; i++)
            {
                string ADR = i.ToString("X4");
                string OMnet = GenMnet(@"Result\" + ADR.Substring(0, 1) + @"\Opt_" + ADR + ".txt");
                System.IO.File.WriteAllText(@"MNETLib\" + ADR.Substring(0, 1) + @"\lut_" + ADR + ".MNET",OMnet);
            }
            for (int i = 1; i < 0xFE; i++)
            {
                string ADR = i.ToString("X2");
                string OMCnet = GenMnet(@"Result\OptCo\OptCo_" + ADR + ".txt")
                    .Replace("NODE:INPort:dataa\r\n", "")
                    .Replace("datab", "dataa")
                    .Replace("datac", "datab")
                    .Replace("datad", "datac")
                    .Replace("combout", "cout");
                System.IO.File.WriteAllText(@"MNETLib\OptCo\lutc_" + ADR + ".MNET", OMCnet);
            }
            //calc max len
            //CalcMax();



        }

        private static void CalcMax()
        {
            int max = 0;
            for (int i = 0; i <= 0xFFFF; i++)
            {
                string[] AllString = System.IO.File.ReadAllLines(@"OptLut\Opt_" + i.ToString("X4") + ".txt");
                string[] NStr = GetNeedOnly(AllString);
                if (GetNeedOnly(AllString).Length > max) max = GetNeedOnly(AllString).Length;
                if (max == 8)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine(i);
            }
        }

        private static string GenMnet(string FileName)
        {
            GlobalAndIndex = 0;

            Nodes = new List<Node>();
            Wires = new List<Wire>();
            Mcpoint = new List<Cpoint>();
            string[] AllString = System.IO.File.ReadAllLines(FileName);
            string[] NStr = GetNeedOnly(AllString);
            MatrixSize = NStr[0].Length - 2;
            CombGen(NStr.Length);
            GenINCpoints(NStr);
            GenANDS(NStr);

            FindDup();


            string ostr = "";
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].NodeType == "INPort")
                {
                    ostr += Nodes[i].ToString() + "\r\n";
                }
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].NodeType == "OUTPort")
                {
                    ostr += Nodes[i].ToString() + "\r\n";
                }
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].NodeType != "INPort" & Nodes[i].NodeType != "OUTPort")
                {
                    ostr += Nodes[i].ToString() + "\r\n";
                }
            }

            for (int i = 0; i < Wires.Count; i++)
            {
               ostr += Wires[i].ToString() + "\r\n";
               
            }

            Console.WriteLine(FileName);

            return ostr;
        }

        private static void FindDup()
        {
            int dup = 1;
            
            for (int i = 0; i < Wires.Count; i++)
            {
                int d = 0;
                string srcp = Wires[i].SrcPort;
                string srcn = Wires[i].SrcName;
                List<string> distsNames = new List<string>();
                List<string> distsPorts = new List<string>();
                List<int> rmList = new List<int>();
                for (int j = 0; j < Wires.Count; j++)
                {
                    
                        if ((Wires[i].SrcPort == Wires[j].SrcPort) && (Wires[i].SrcName == Wires[j].SrcName))
                        {
                            distsPorts.Add(Wires[j].DistPort);
                            distsNames.Add(Wires[j].DistName);
                            rmList.Add(j);
                            d++;
                        }
                    
                }
                if (d > 1)
                {
                    Nodes.Add(new Node { NodeName = "DUPN" + dup.ToString(), NodeType = "DUP" + d.ToString() });
                    //Wires.RemoveAt(i);
                    i=0;
                    Wires.Add(new Wire { DistName = "DUPN" + dup.ToString(), DistPort = "I0", SrcName = srcn, SrcPort = srcp });
                    for (int j = 0; j < distsNames.Count; j++)
                    {
                        Wires.Add(new Wire { DistName = distsNames[j], DistPort = distsPorts[j], SrcName = "DUPN" + dup.ToString(), SrcPort = "O"+j.ToString() });
                    }
                    dup++;
                    for (int j = 0; j < distsNames.Count; j++)
                    {
                        Wires.RemoveAt(rmList[j]-j);
                    }
                }
            }
        }

        private static void GenANDS(string[] NStr)
        {
            for (int i = 0; i < NStr.Length; i++)
            {
                GenANDSQ(NStr[i],i);
            }
        }

        private static void GenANDSQ(string NStr,int CRseq)
        {
            int andlen = calcAndL(NStr);
            AndGen(andlen, CRseq);
            int q = 0;
            for (int i = 0; i < MatrixSize; i++)
            {
                if (NStr.Substring(i, 1) == "1")
                {
                    CreateWireFromCpoint(FindMcPoint("BIN" + i.ToString()),
                        FindMcPoint("INAND" + CRseq.ToString() + "_" + q.ToString()).DistName, 
                        FindMcPoint("INAND" + CRseq.ToString() + "_" + q.ToString()).DistPort);
                    q++;
                }

                if (NStr.Substring(i, 1) == "0")
                {
                    //Cpoint c1 = FindMcPoint("NIN" + i.ToString());
                    //Cpoint c2 = FindMcPoint("INAND" + CRseq.ToString() + "_" + q.ToString());
                    CreateWireFromCpoint(FindMcPoint("NIN" + i.ToString()),
                        FindMcPoint("INAND" + CRseq.ToString() + "_" + q.ToString()).DistName,
                        FindMcPoint("INAND" + CRseq.ToString() + "_" + q.ToString()).DistPort);
                    q++;
                }
            }


        }

        private static void AndGen(int AndLen,int CrSeq)
        {
            switch (AndLen)
            {
                case 1:
                    Mcpoint.Add(new Cpoint { DistName = FindMcPoint("OR" + CrSeq).DistName, DistPort = FindMcPoint("OR" + CrSeq).DistPort, Name = "INAND" + CrSeq.ToString() + "_0" });
                    break;
                case 2:
                    Nodes.Add(new Node { NodeName = "AND_"+GlobalAndIndex.ToString(), NodeType = "AND" });
                    Mcpoint.Add(new Cpoint { DistName = "AND_" + GlobalAndIndex.ToString(), DistPort = "I0", Name = "INAND" + CrSeq.ToString() + "_0" });
                    Mcpoint.Add(new Cpoint { DistName = "AND_" + GlobalAndIndex.ToString(), DistPort = "I1", Name = "INAND" + CrSeq.ToString() + "_1" });
                    Wires.Add(new Wire { DistName = FindMcPoint("OR" + CrSeq).DistName, DistPort = FindMcPoint("OR" + CrSeq).DistPort, SrcName = "AND_" + GlobalAndIndex.ToString(), SrcPort = "O0" });
                    GlobalAndIndex++;
                    break;
                default:
                    Nodes.Add(new Node { NodeName = "AND_"+GlobalAndIndex.ToString(), NodeType = "AND" });
                    Mcpoint.Add(new Cpoint { DistName = "AND_" + GlobalAndIndex.ToString(), DistPort = "I0", Name = "INAND" + CrSeq.ToString() + "_0" });
                    Mcpoint.Add(new Cpoint { DistName = "AND_" + GlobalAndIndex.ToString(), DistPort = "I1", Name = "INAND" + CrSeq.ToString() + "_1" });
                    //Wires.Add(new Wire { DistName = FindMcPoint("OR" + CrSeq).DistName, DistPort = FindMcPoint("OR" + CrSeq).DistPort, SrcName = "AND_" + GlobalAndIndex.ToString(), SrcPort = "O0" });
                    GlobalAndIndex++;
                                        
                    for (int i = 2; i < AndLen; i++)
                    {
                        Nodes.Add(new Node { NodeName = "AND_" + GlobalAndIndex.ToString(), NodeType = "AND" });
                        Mcpoint.Add(new Cpoint { DistName = "AND_" + GlobalAndIndex.ToString(), DistPort = "I1", Name = "INAND" + CrSeq.ToString() + "_" + i.ToString() });
                        Wires.Add(new Wire { DistName = "AND_" + GlobalAndIndex.ToString(), DistPort = "I0", SrcName = "AND_" + (GlobalAndIndex-1).ToString(), SrcPort = "O0" });
                        GlobalAndIndex++;
                    }
                    Wires.Add(new Wire { DistName = FindMcPoint("OR" + CrSeq).DistName, DistPort = FindMcPoint("OR" + CrSeq).DistPort
                                        ,SrcName = "AND_" + (GlobalAndIndex - 1).ToString(),SrcPort = "O0"});
                    
                    break;
            }

        }

        

        private static int calcAndL(string NStr)
        {
            int tlen = MatrixSize;
            for (int i = 0; i < MatrixSize; i++)
            {
                if (NStr.Substring(i, 1) == "-") tlen--;
            }
            return tlen;
        }

        private static void GenINCpoints(string[] NStr)
        {
            GenBaseInPorts();
            GenNegateInPorts(NStr);
        }

        private static void GenNegateInPorts(string[] NStr)
        {
            for (int i = 0; i < NStr.Length; i++)
            {
                for (int j=0;j<MatrixSize;j++)
                {
                    if (NStr[i].Substring(j, 1) == "0")
                    {
                        if (!NINExist(j))
                        {
                            Nodes.Add(new Node { NodeName = "NOTDat"+j.ToString(), NodeType = "NOT" });
                            CreateWireFromCpoint(FindMcPoint("BIN" + j.ToString()), "NOTDat" + j.ToString(), "IN0");
                            Mcpoint.Add(new Cpoint { DistName = "NOTDat" + j.ToString(), DistPort = "O0", Name = "NIN" + j.ToString() });
                        }
                    }
                }
            }
        }

        private static void CreateWireFromCpoint(Cpoint CP, string TDistName, string TDistPort)
        {
            Wires.Add(new Wire { DistName = TDistName, DistPort = TDistPort, SrcName = CP.DistName, SrcPort = CP.DistPort });
        }

        private static bool NINExist(int NINIndex)
        {
            return FindMcPoint("NIN" + NINIndex.ToString()) != null;
        }

        private static Cpoint FindMcPoint(string PointName)
        {
            for (int i = 0; i < Mcpoint.Count; i++)
            {
                if (Mcpoint[i].Name == PointName) return Mcpoint[i];
            }
            return null;
        }

        private static void GenBaseInPorts()
        {
            Nodes.Add(new Node { NodeName = "datad", NodeType = "INPort" });
            Nodes.Add(new Node { NodeName = "datac", NodeType = "INPort" });
            Nodes.Add(new Node { NodeName = "datab", NodeType = "INPort" });
            Nodes.Add(new Node { NodeName = "dataa", NodeType = "INPort" });
            Mcpoint.Add(new Cpoint { DistName = "datad", DistPort = "O0", Name = "BIN0" });
            Mcpoint.Add(new Cpoint { DistName = "datac", DistPort = "O0", Name = "BIN1" });
            Mcpoint.Add(new Cpoint { DistName = "datab", DistPort = "O0", Name = "BIN2" });
            Mcpoint.Add(new Cpoint { DistName = "dataa", DistPort = "O0", Name = "BIN3" });
        }

        private static void CombGen(int p)
        {
            switch (p)
            {
                case 1:
                    Nodes.Add(new Node { NodeName = "combout", NodeType = "OUTPort" });
                    Mcpoint.Add(new Cpoint { DistName = "combout", DistPort = "I0", Name = "OR0" });
                    break;
                case 2:
                    Nodes.Add(new Node { NodeName = "combout", NodeType = "OUTPort" });
                    Nodes.Add(new Node { NodeName = "OUTOR_1", NodeType = "OR" });
                    Mcpoint.Add(new Cpoint { DistName = "OUTOR_1", DistPort = "I0", Name = "OR0" });
                    Mcpoint.Add(new Cpoint { DistName = "OUTOR_1", DistPort = "I1", Name = "OR1" });
                    Wires.Add(new Wire { DistName = "combout", DistPort = "I0", SrcName = "OUTOR_1", SrcPort = "O0" });
                    break;
                default:
                    Nodes.Add(new Node { NodeName = "combout", NodeType = "OUTPort" });
                    Nodes.Add(new Node { NodeName = "OUTOR_1", NodeType = "OR" });
                    Mcpoint.Add(new Cpoint { DistName = "OUTOR_1", DistPort = "I0", Name = "OR0"});
                    Mcpoint.Add(new Cpoint { DistName = "OUTOR_1", DistPort = "I1", Name = "OR1"});
                    for (int i = 2; i < p; i++)
                    {
                        Nodes.Add(new Node { NodeName = "OUTOR_"+i.ToString() , NodeType = "OR" });
                        Mcpoint.Add(new Cpoint { DistName = "OUTOR_" + i.ToString(), DistPort = "I1", Name = "OR" + i.ToString()});
                        Wires.Add(new Wire { DistName = "OUTOR_" + i.ToString() , DistPort = "I0", SrcName = "OUTOR_"+ (i-1).ToString() , SrcPort = "O0" });
                    }
                    Wires.Add(new Wire { DistName = "combout", DistPort = "I0", SrcName = "OUTOR_" + (p - 1).ToString(), SrcPort = "O0" });
                    
                    break;
            }
            
        }

        private static string[] GetNeedOnly(string[] AllString)
        {
            List<string> Nstr = new List<string>();
            for (int i = 5; i < (AllString.Length - 1); i++)
            {
                Nstr.Add(AllString[i]);
            }
            return Nstr.ToArray();
        }
    }
}
