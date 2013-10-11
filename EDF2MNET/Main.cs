using System;
using System.Collections.Generic;
using RouteUtils;

namespace EDF2MNET
{
    class Program
    {
        static List<string> Mainportlist = new List<string>();
        static List<string> MainportlistT = new List<string>();
        static void Main(string[] args)
        {

            string FileName = "";
            if (args.Length < 1)
            {
                FileName = "tm";
            }
            else
            {
                FileName = args[0];
            }


            string ostr = "";
            string[] instr = System.IO.File.ReadAllLines(FileName+".edf");
            int portstart = 0;
            int ldes = 0;//library DESIGN
            while (!instr[ldes].Contains("library DESIGN"))
            {
                ldes++;
            }
            portstart = ldes;
            while (!instr[portstart].Contains("(port "))
            {
                portstart++;
            }
            int portend = portstart;
            while (!instr[portend].Contains("(contents"))
            {
                if (instr[portend].Contains("(port "))
                {
                    
                    
                    string ParseStr = instr[portend].Replace(")", " ").Replace("(", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] Mstr = ParseStr.Split(' ');
                    if (Mstr[2] == "rename")
                    {
                        if (Mstr[6] == "INPUT")
                        {
                            ostr += "NODE:INPort:" + Mstr[3] + "\n";
                            Mainportlist.Add(Mstr[3]);
                            MainportlistT.Add("I");
                        }
                        if (Mstr[6] == "OUTPUT")
                        {
                            ostr += "NODE:OUTPort:" + Mstr[3] + "\n";
                            Mainportlist.Add(Mstr[3]);
                            MainportlistT.Add("O");
                        }
                    }
                    else
                    {
                        if (Mstr[4] == "INPUT")
                        {
                            ostr += "NODE:INPort:" + Mstr[2] + "\n";
                            Mainportlist.Add(Mstr[2]);
                            MainportlistT.Add("I");
                        }
                        if (Mstr[4] == "OUTPUT")
                        {
                            ostr += "NODE:OUTPort:" + Mstr[2] + "\n";
                            Mainportlist.Add(Mstr[2]);
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
                    string ParseStr = instr[portend].Replace(")", " ").Replace("(", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] Mstr = ParseStr.Split(' ');
                    ostr += "NODE:" + Mstr[6] + ":" + Mstr[2] + "\n";
                    Node N = new Node(Mstr[6] + ".binhl");
                    for (int j = 0; j < N.InPorts.Length; j++)
                    {
                        Mainportlist.Add(Mstr[2] + "-" +N.InPorts[j].Name);
                        MainportlistT.Add("I");
                    }
                    for (int j = 0; j < N.OutPorts.Length; j++)
                    {
                        Mainportlist.Add(Mstr[2] + "-" + N.OutPorts[j].Name);
                        MainportlistT.Add("O");
                    }
                }
                if (instr[portend].Contains("(net "))
                {
                    string ParseStr = instr[portend].Replace(")", " ").Replace("(", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] Mstr = ParseStr.Split(' ');
                    List<string> portlist = new List<string>();
                    List<string> portlistT = new List<string>();
                    for (int i = 0; i < Mstr.Length; i++)
                    {
                        if (Mstr[i] == "portRef")
                        {
                            if (Mstr[i + 2] == "instanceRef")
                            {
                                portlist.Add(Mstr[i + 3] + "-" + Mstr[i + 1]);
                                portlistT.Add(FindPortType(Mstr[i + 3] + "-" + Mstr[i + 1]));
                            }
                            else
                            {
                                if (FindPortType(Mstr[i + 1]) == "I")
                                {
                                    portlistT.Add("O");
                                    portlist.Add(Mstr[i + 1] + "-O0");
                                }
                                else
                                {
                                    portlistT.Add("I");
                                    portlist.Add(Mstr[i + 1] + "-I0");
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
                        ostr += "NODE:dup2:" + Mstr[2] + "\n";
                        int op = 0;
                        for (int j = 0; j < portlist.Count; j++)
                        {
                            if (portlistT[j] == "O")
                            {
                                ostr += "WIRE:" + portlist[j] + ":" + Mstr[2] + "-I0" + "\n";
                            }
                            else
                            {
                                ostr += "WIRE:" + Mstr[2] + "-O" + op.ToString() + ":" + portlist[j] + "\n";
                                op++;
                            }
                        }
                    }
                }
                portend++;
            }

            System.IO.File.WriteAllText(FileName+".MNET", ostr);
            //for (int i = 0; i < instr.Length; i++)
            //{
 
            //}
        }

        private static string FindPortType(string PortName)
        {
            for (int i = 0; i < Mainportlist.Count; i++)
            {
                if (Mainportlist[i] == PortName)
                {
                    return MainportlistT[i];
                }
            }
            return " ";
        }
    }
}
