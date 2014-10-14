using System.Collections.Generic;
using RouteUtils;

namespace EDF2MNET
{
    static class Program
    {
        static readonly List<string> Mainportlist = new List<string>();
        static readonly List<string> MainportlistT = new List<string>();
        static void Main(string[] args)
        {
            string fileName = args.Length < 1 ? "tm" : args[0];


            string ostr = "";
            string[] instr = System.IO.File.ReadAllLines(fileName+".edf");
            int ldes = 0;//library DESIGN
            while (!instr[ldes].Contains("library DESIGN"))
            {
                ldes++;
            }
            int portstart = ldes;
            while (!instr[portstart].Contains("(port "))
            {
                portstart++;
            }
            int portend = portstart;
            while (!instr[portend].Contains("(contents"))
            {
                if (instr[portend].Contains("(port "))
                {
                    string parseStr = instr[portend].Replace(")", " ").Replace("(", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] mstr = parseStr.Split(' ');
                    if (mstr[2] == "rename")
                    {
                        if (mstr[6] == "INPUT")
                        {
                            ostr += "NODE:INPort:" + mstr[3] + "\n";
                            Mainportlist.Add(mstr[3]);
                            MainportlistT.Add("I");
                        }
                        if (mstr[6] == "OUTPUT")
                        {
                            ostr += "NODE:OUTPort:" + mstr[3] + "\n";
                            Mainportlist.Add(mstr[3]);
                            MainportlistT.Add("O");
                        }
                    }
                    else
                    {
                        if (mstr[4] == "INPUT")
                        {
                            ostr += "NODE:INPort:" + mstr[2] + "\n";
                            Mainportlist.Add(mstr[2]);
                            MainportlistT.Add("I");
                        }
                        if (mstr[4] == "OUTPUT")
                        {
                            ostr += "NODE:OUTPort:" + mstr[2] + "\n";
                            Mainportlist.Add(mstr[2]);
                            MainportlistT.Add("O");
                        }
                    }
                }
                portend++;
            }
            while (!instr[portend].Contains("(design "))
            {
                if (instr[portend].Contains("instance "))
                {
                    string parseStr = instr[portend].Replace(")", " ").Replace("(", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] mstr = parseStr.Split(' ');
                    ostr += "NODE:" + mstr[6] + ":" + mstr[2] + "\n";
                    var node = new Node(mstr[6] + ".binhl");
                    foreach (InPort port in node.InPorts)
                    {
                        Mainportlist.Add(mstr[2] + "-" +port.Name);
                        MainportlistT.Add("I");
                    }
                    foreach (OutPort port in node.OutPorts)
                    {
                        Mainportlist.Add(mstr[2] + "-" + port.Name);
                        MainportlistT.Add("O");
                    }
                }
                if (instr[portend].Contains("(net "))
                {
                    string parseStr = instr[portend].Replace(")", " ").Replace("(", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] mstr = parseStr.Split(' ');
                    var portlist = new List<string>();
                    var portlistT = new List<string>();
                    for (int i = 0; i < mstr.Length; i++)
                    {
                        if (mstr[i] == "portRef")
                        {
                            if (mstr[i + 2] == "instanceRef")
                            {
                                portlist.Add(mstr[i + 3] + "-" + mstr[i + 1]);
                                portlistT.Add(FindPortType(mstr[i + 3] + "-" + mstr[i + 1]));
                            }
                            else
                            {
                                if (FindPortType(mstr[i + 1]) == "I")
                                {
                                    portlistT.Add("O");
                                    portlist.Add(mstr[i + 1] + "-O0");
                                }
                                else
                                {
                                    portlistT.Add("I");
                                    portlist.Add(mstr[i + 1] + "-I0");
                                }
                            }
                        }
                    }
                    if (portlist.Count == 2)
                    {
                        if (portlistT[0] == "O")
                        {
                            ostr += "WIRE:" + portlist[0] + ":" + portlist[1] + "\n";
                        }
                        else
                        {
                            ostr += "WIRE:" + portlist[1] + ":" + portlist[0] + "\n";
                        }
                    }
                    if (portlist.Count == 3)
                    {
                        ostr += "NODE:dup2:" + mstr[2] + "\n";
                        int op = 0;
                        for (int j = 0; j < portlist.Count; j++)
                        {
                            if (portlistT[j] == "O")
                            {
                                ostr += "WIRE:" + portlist[j] + ":" + mstr[2] + "-I0" + "\n";
                            }
                            else
                            {
                                ostr += "WIRE:" + mstr[2] + "-O" + op + ":" + portlist[j] + "\n";
                                op++;
                            }
                        }
                    }
                }
                portend++;
            }

            System.IO.File.WriteAllText(fileName+".MNET", ostr);
            //for (int i = 0; i < instr.Length; i++)
            //{
 
            //}
        }

        private static string FindPortType(string portName)
        {
            for (int i = 0; i < Mainportlist.Count; i++)
            {
                if (Mainportlist[i] == portName)
                {
                    return MainportlistT[i];
                }
            }
            return " ";
        }
    }
}
