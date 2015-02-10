using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UopGen
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i <= 32; i++)
            {
                GenNOT(i);
                GenLL(i);
                GenRR(i);
                GenLLC(i);
                GenRRC(i);
            }
        }

        private static void GenNOT(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i=0;i<wide;i++)
            {
                ostr += "NODE:INPort:I" + i.ToString() + "\r\n";
            }
            for (int i=0;i<wide;i++)
            {
                ostr += "NODE:OUTPort:O" + i.ToString() + "\r\n";
            }
            //Gen NODES
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:NOT:GL_NOT_" + i.ToString() + "\r\n";
            }
            //Gen WIRES
            for (int i = 0; i < wide; i++)
            {
                ostr += "WIRE:I" + i.ToString() + "-O0" + ":GL_NOT_" + i.ToString() + "-I0" + "\r\n";
                ostr += "WIRE:GL_NOT_" + i.ToString() + "-O0" + ":O" + i.ToString() + "-I0" + "\r\n";
            }
            Console.WriteLine("NOT_" + wide);
            System.IO.File.WriteAllText("NOT_" + wide + ".MNET",ostr);
        }
        private static void GenLL(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:INPort:I" + i.ToString() + "\r\n";
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:OUTPort:O" + i.ToString() + "\r\n";
            }
            
            //Gen WIRES
            for (int i = 0; i < wide-1; i++)
            {
                ostr += "WIRE:I" + i.ToString() + "-O0" + ":O" + (i+1).ToString() + "-I0" + "\r\n";
            }
            Console.WriteLine("LL_" + wide);
            System.IO.File.WriteAllText("LL_" + wide + ".MNET", ostr);
        }
        private static void GenRR(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:INPort:I" + i.ToString() + "\r\n";
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:OUTPort:O" + i.ToString() + "\r\n";
            }

            //Gen WIRES
            for (int i = 1; i < wide; i++)
            {
                ostr += "WIRE:I" + (i).ToString() + "-O0" + ":O" + (i-1).ToString() + "-I0" + "\r\n";
            }
            Console.WriteLine("RR_" + wide);
            System.IO.File.WriteAllText("RR_" + wide + ".MNET", ostr);
        }
        private static void GenLLC(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:INPort:I" + i.ToString() + "\r\n";
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:OUTPort:O" + i.ToString() + "\r\n";
            }

            //Gen WIRES
            for (int i = 0; i < wide - 1; i++)
            {
                ostr += "WIRE:I" + i.ToString() + "-O0" + ":O" + (i + 1).ToString() + "-I0" + "\r\n";
            }
            ostr += "WIRE:I" + (wide-1).ToString() + "-O0" + ":O" + "0" + "-I0" + "\r\n";
            Console.WriteLine("LLC_" + wide);
            System.IO.File.WriteAllText("LLC_" + wide + ".MNET", ostr);
        }
        private static void GenRRC(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:INPort:I" + i.ToString() + "\r\n";
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += "NODE:OUTPort:O" + i.ToString() + "\r\n";
            }

            //Gen WIRES
            for (int i = 1; i < wide; i++)
            {
                ostr += "WIRE:I" + (i).ToString() + "-O0" + i.ToString() + ":O" + (i - 1).ToString() + "-I0" + i.ToString() + "\r\n";
            }
            ostr += "WIRE:I" + "0" + "-O0" + ":O" + (wide - 1).ToString() + "-I0" + "\r\n";
            Console.WriteLine("RRC_" + wide);
            System.IO.File.WriteAllText("RRC_" + wide + ".MNET", ostr);
        }

    }
}
