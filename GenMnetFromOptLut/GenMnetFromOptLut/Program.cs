using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenMnetFromOptLut
{
    internal static class Program
    {
        private static List<Node> _nodes;
        private static List<Wire> _wires;
        private static List<Cpoint> _mcpoint;
        private static int _globalAndIndex;
        private static int _matrixSize;

        private static void Main()
        {
            //string OMnet = GenMnet(@"Result\A\Opt_AF62.txt");
            //string OMnet = GenMnet(@"Result\F\Opt_FCCC.txt");

            for (int i = 1; i < 0xFFFE; i++)
            {
                string adr = i.ToString("X4");
                string oMnet = GenMnet(@"Result\" + adr.Substring(0, 1) + @"\Opt_" + adr + ".txt");
                File.WriteAllText(@"MNETLib\" + adr.Substring(0, 1) + @"\lut_" + adr + ".MNET", oMnet);
            }
            for (int i = 1; i < 0xFE; i++)
            {
                string adr = i.ToString("X2");
                string omCnet = GenMnet(@"Result\OptCo\OptCo_" + adr + ".txt")
                    .Replace("NODE:INPort:dataa\r\n", "")
                    .Replace("datab", "dataa")
                    .Replace("datac", "datab")
                    .Replace("datad", "datac")
                    .Replace("combout", "cout");
                File.WriteAllText(@"MNETLib\OptCo\lutc_" + adr + ".MNET", omCnet);
            }
            //calc max len
            //CalcMax();
        }

/*
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
*/

        private static string GenMnet(string fileName)
        {
            _globalAndIndex = 0;

            _nodes = new List<Node>();
            _wires = new List<Wire>();
            _mcpoint = new List<Cpoint>();
            string[] allString = File.ReadAllLines(fileName);
            string[] nStr = GetNeedOnly(allString);
            _matrixSize = nStr[0].Length - 2;
            CombGen(nStr.Length);
            GenInCpoints(nStr);
            GenAnds(nStr);

            FindDup();


            string ostr = _nodes.Where(t => t.NodeType == "INPort")
                .Aggregate("", (current, t) => current + (t.ToString() + "\r\n"));

            ostr = _nodes.Where(t => t.NodeType == "OUTPort")
                .Aggregate(ostr, (current, t) => current + (t.ToString() + "\r\n"));

            ostr = _nodes.Where(t => t.NodeType != "INPort" & t.NodeType != "OUTPort")
                .Aggregate(ostr, (current, t) => current + (t.ToString() + "\r\n"));

            ostr = _wires.Aggregate(ostr, (current, t) => current + (t.ToString() + "\r\n"));

            Console.WriteLine(fileName);

            return ostr;
        }

        private static void FindDup()
        {
            int dup = 1;

            for (int i = 0; i < _wires.Count; i++)
            {
                int d = 0;
                string srcp = _wires[i].SrcPort;
                string srcn = _wires[i].SrcName;
                var distsNames = new List<string>();
                var distsPorts = new List<string>();
                var rmList = new List<int>();
                for (int j = 0; j < _wires.Count; j++)
                {
                    if ((_wires[i].SrcPort == _wires[j].SrcPort) && (_wires[i].SrcName == _wires[j].SrcName))
                    {
                        distsPorts.Add(_wires[j].DistPort);
                        distsNames.Add(_wires[j].DistName);
                        rmList.Add(j);
                        d++;
                    }
                }
                if (d > 1)
                {
                    _nodes.Add(new Node {NodeName = "DUPN" + dup, NodeType = "DUP" + d});
                    //Wires.RemoveAt(i);
                    i = 0;
                    _wires.Add(new Wire {DistName = "DUPN" + dup, DistPort = "I0", SrcName = srcn, SrcPort = srcp});
                    for (int j = 0; j < distsNames.Count; j++)
                    {
                        _wires.Add(new Wire
                        {
                            DistName = distsNames[j],
                            DistPort = distsPorts[j],
                            SrcName = "DUPN" + dup,
                            SrcPort = "O" + j
                        });
                    }
                    dup++;
                    for (int j = 0; j < distsNames.Count; j++)
                    {
                        _wires.RemoveAt(rmList[j] - j);
                    }
                }
            }
        }

        private static void GenAnds(string[] nStr)
        {
            for (int i = 0; i < nStr.Length; i++)
            {
                GenAndsq(nStr[i], i);
            }
        }

        private static void GenAndsq(string nStr, int cRseq)
        {
            int andlen = CalcAndL(nStr);
            AndGen(andlen, cRseq);
            int q = 0;
            for (int i = 0; i < _matrixSize; i++)
            {
                if (nStr.Substring(i, 1) == "1")
                {
                    CreateWireFromCpoint(FindMcPoint("BIN" + i),
                        FindMcPoint("INAND" + cRseq + "_" + q).DistName,
                        FindMcPoint("INAND" + cRseq + "_" + q).DistPort);
                    q++;
                }

                if (nStr.Substring(i, 1) == "0")
                {
                    //Cpoint c1 = FindMcPoint("NIN" + i.ToString());
                    //Cpoint c2 = FindMcPoint("INAND" + CRseq.ToString() + "_" + q.ToString());
                    CreateWireFromCpoint(FindMcPoint("NIN" + i),
                        FindMcPoint("INAND" + cRseq + "_" + q).DistName,
                        FindMcPoint("INAND" + cRseq + "_" + q).DistPort);
                    q++;
                }
            }
        }

        private static void AndGen(int andLen, int crSeq)
        {
            switch (andLen)
            {
                case 1:
                    _mcpoint.Add(new Cpoint
                    {
                        DistName = FindMcPoint("OR" + crSeq).DistName,
                        DistPort = FindMcPoint("OR" + crSeq).DistPort,
                        Name = "INAND" + crSeq + "_0"
                    });
                    break;
                case 2:
                    _nodes.Add(new Node {NodeName = "AND_" + _globalAndIndex, NodeType = "AND"});
                    _mcpoint.Add(new Cpoint
                    {
                        DistName = "AND_" + _globalAndIndex,
                        DistPort = "I0",
                        Name = "INAND" + crSeq + "_0"
                    });
                    _mcpoint.Add(new Cpoint
                    {
                        DistName = "AND_" + _globalAndIndex,
                        DistPort = "I1",
                        Name = "INAND" + crSeq + "_1"
                    });
                    _wires.Add(new Wire
                    {
                        DistName = FindMcPoint("OR" + crSeq).DistName,
                        DistPort = FindMcPoint("OR" + crSeq).DistPort,
                        SrcName = "AND_" + _globalAndIndex,
                        SrcPort = "O0"
                    });
                    _globalAndIndex++;
                    break;
                default:
                    _nodes.Add(new Node {NodeName = "AND_" + _globalAndIndex, NodeType = "AND"});
                    _mcpoint.Add(new Cpoint
                    {
                        DistName = "AND_" + _globalAndIndex,
                        DistPort = "I0",
                        Name = "INAND" + crSeq + "_0"
                    });
                    _mcpoint.Add(new Cpoint
                    {
                        DistName = "AND_" + _globalAndIndex,
                        DistPort = "I1",
                        Name = "INAND" + crSeq + "_1"
                    });
                    //Wires.Add(new Wire { DistName = FindMcPoint("OR" + CrSeq).DistName, DistPort = FindMcPoint("OR" + CrSeq).DistPort, SrcName = "AND_" + GlobalAndIndex.ToString(), SrcPort = "O0" });
                    _globalAndIndex++;

                    for (int i = 2; i < andLen; i++)
                    {
                        _nodes.Add(new Node {NodeName = "AND_" + _globalAndIndex, NodeType = "AND"});
                        _mcpoint.Add(new Cpoint
                        {
                            DistName = "AND_" + _globalAndIndex,
                            DistPort = "I1",
                            Name = "INAND" + crSeq + "_" + i
                        });
                        _wires.Add(new Wire
                        {
                            DistName = "AND_" + _globalAndIndex,
                            DistPort = "I0",
                            SrcName = "AND_" + (_globalAndIndex - 1),
                            SrcPort = "O0"
                        });
                        _globalAndIndex++;
                    }
                    _wires.Add(new Wire
                    {
                        DistName = FindMcPoint("OR" + crSeq).DistName,
                        DistPort = FindMcPoint("OR" + crSeq).DistPort
                        ,
                        SrcName = "AND_" + (_globalAndIndex - 1),
                        SrcPort = "O0"
                    });

                    break;
            }
        }


        private static int CalcAndL(string nStr)
        {
            int tlen = _matrixSize;
            for (int i = 0; i < _matrixSize; i++)
            {
                if (nStr.Substring(i, 1) == "-") tlen--;
            }
            return tlen;
        }

        private static void GenInCpoints(string[] nStr)
        {
            GenBaseInPorts();
            GenNegateInPorts(nStr);
        }

        private static void GenNegateInPorts(string[] nStr)
        {
            for (int i = 0; i < nStr.Length; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    if (nStr[i].Substring(j, 1) == "0")
                    {
                        if (!NinExist(j))
                        {
                            _nodes.Add(new Node {NodeName = "NOTDat" + j, NodeType = "NOT"});
                            CreateWireFromCpoint(FindMcPoint("BIN" + j), "NOTDat" + j, "I0");
                            _mcpoint.Add(new Cpoint {DistName = "NOTDat" + j, DistPort = "O0", Name = "NIN" + j});
                        }
                    }
                }
            }
        }

        private static void CreateWireFromCpoint(Cpoint cp, string distName, string distPort)
        {
            _wires.Add(new Wire {DistName = distName, DistPort = distPort, SrcName = cp.DistName, SrcPort = cp.DistPort});
        }

        private static bool NinExist(int ninIndex)
        {
            return FindMcPoint("NIN" + ninIndex) != null;
        }

        private static Cpoint FindMcPoint(string pointName)
        {
            return _mcpoint.FirstOrDefault(t => t.Name == pointName);
        }

        private static void GenBaseInPorts()
        {
            _nodes.Add(new Node {NodeName = "datad", NodeType = "INPort"});
            _nodes.Add(new Node {NodeName = "datac", NodeType = "INPort"});
            _nodes.Add(new Node {NodeName = "datab", NodeType = "INPort"});
            _nodes.Add(new Node {NodeName = "dataa", NodeType = "INPort"});
            _mcpoint.Add(new Cpoint {DistName = "datad", DistPort = "O0", Name = "BIN0"});
            _mcpoint.Add(new Cpoint {DistName = "datac", DistPort = "O0", Name = "BIN1"});
            _mcpoint.Add(new Cpoint {DistName = "datab", DistPort = "O0", Name = "BIN2"});
            _mcpoint.Add(new Cpoint {DistName = "dataa", DistPort = "O0", Name = "BIN3"});
        }

        private static void CombGen(int p)
        {
            switch (p)
            {
                case 1:
                    _nodes.Add(new Node {NodeName = "combout", NodeType = "OUTPort"});
                    _mcpoint.Add(new Cpoint {DistName = "combout", DistPort = "I0", Name = "OR0"});
                    break;
                case 2:
                    _nodes.Add(new Node {NodeName = "combout", NodeType = "OUTPort"});
                    _nodes.Add(new Node {NodeName = "OUTOR_1", NodeType = "OR"});
                    _mcpoint.Add(new Cpoint {DistName = "OUTOR_1", DistPort = "I0", Name = "OR0"});
                    _mcpoint.Add(new Cpoint {DistName = "OUTOR_1", DistPort = "I1", Name = "OR1"});
                    _wires.Add(new Wire {DistName = "combout", DistPort = "I0", SrcName = "OUTOR_1", SrcPort = "O0"});
                    break;
                default:
                    _nodes.Add(new Node {NodeName = "combout", NodeType = "OUTPort"});
                    _nodes.Add(new Node {NodeName = "OUTOR_1", NodeType = "OR"});
                    _mcpoint.Add(new Cpoint {DistName = "OUTOR_1", DistPort = "I0", Name = "OR0"});
                    _mcpoint.Add(new Cpoint {DistName = "OUTOR_1", DistPort = "I1", Name = "OR1"});
                    for (int i = 2; i < p; i++)
                    {
                        _nodes.Add(new Node {NodeName = "OUTOR_" + i, NodeType = "OR"});
                        _mcpoint.Add(new Cpoint {DistName = "OUTOR_" + i, DistPort = "I1", Name = "OR" + i});
                        _wires.Add(new Wire
                        {
                            DistName = "OUTOR_" + i,
                            DistPort = "I0",
                            SrcName = "OUTOR_" + (i - 1),
                            SrcPort = "O0"
                        });
                    }
                    _wires.Add(new Wire
                    {
                        DistName = "combout",
                        DistPort = "I0",
                        SrcName = "OUTOR_" + (p - 1),
                        SrcPort = "O0"
                    });

                    break;
            }
        }

        private static string[] GetNeedOnly(string[] allString)
        {
            var nstr = new List<string>();
            for (int i = 5; i < (allString.Length - 1); i++)
            {
                nstr.Add(allString[i]);
            }
            return nstr.ToArray();
        }
    }
}