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
                ostr += string.Format("NODE:INPort:I{0}\r\n", i.ToString());
            }
            for (int i=0;i<wide;i++)
            {
                ostr += string.Format("NODE:OUTPort:O{0}\r\n", i.ToString());
            }
            //Gen NODES
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:NOT:GL_NOT_{0}\r\n", i.ToString());
            }
            //Gen WIRES
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("WIRE:I{0}-O0:GL_NOT_{0}-I0\r\n", i);
                ostr += string.Format("WIRE:GL_NOT_{0}-O0:O{0}-I0\r\n", i);
            }
            Console.WriteLine("NOT_{0}", wide);
            System.IO.File.WriteAllText("NOT_" + wide + ".MNET",ostr);
        }
        private static void GenLL(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:INPort:I{0}\r\n", i);
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:OUTPort:O{0}\r\n", i);
            }
            //Gen NODES
            for (int i = 0; i < wide-1; i++)
            {
                ostr += string.Format("NODE:DUMMY:GL_DUMMY_{0}\r\n", i);
            }
            ostr += "NODE:NOPT:GL_NOPT_0\r\n";
            ostr += "NODE:GND:GL_GND_0\r\n";
            //Gen WIRES
            for (int i = 0; i < wide-1; i++)
            {
                ostr += string.Format("WIRE:I{0}-O0:GL_DUMMY_{0}-I0\r\n", i);
                ostr += string.Format("WIRE:GL_DUMMY_{0}-O0:O{1}-I0\r\n", i, i + 1);
            }
            ostr += string.Format("WIRE:I{0}-O0" + ":GL_NOPT_0-I0" + "\r\n", wide - 1);
            ostr += string.Format("WIRE:GL_GND_0-O0:O{0}-I0" + "\r\n", (0));
            Console.WriteLine("LL_{0}", wide);
            System.IO.File.WriteAllText("LL_" + wide + ".MNET", ostr);
        }
        private static void GenRR(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:INPort:I{0}\r\n", i);
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:OUTPort:O{0}\r\n", i);
            }
            //Gen NODES
            for (int i = 1; i < wide; i++)
            {
                ostr += string.Format("NODE:DUMMY:GL_DUMMY_{0}\r\n", i);
            }
            ostr += "NODE:NOPT:GL_NOPT_0\r\n";
            ostr += "NODE:GND:GL_GND_0\r\n";
            //Gen WIRES
            for (int i = 1; i < wide; i++)
            {
                ostr += string.Format("WIRE:I{0}-O0:GL_DUMMY_{0}-I0\r\n", i);
                ostr += string.Format("WIRE:GL_DUMMY_{0}-O0:O{1}-I0\r\n", i, i - 1);
            }
            ostr += string.Format("WIRE:I0-O0:GL_NOPT_0-I0\r\n");
            ostr += string.Format("WIRE:GL_GND_0-O0:O{0}-I0\r\n", wide-1);
            Console.WriteLine("RR_" + wide);
            System.IO.File.WriteAllText("RR_" + wide + ".MNET", ostr);
        }
        private static void GenLLC(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:INPort:I{0}\r\n", i);
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:OUTPort:O{0}\r\n", i);
            }
            //Gen NODES
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:DUMMY:GL_DUMMY_{0}\r\n", i);
            }
            //Gen WIRES
            for (int i = 0; i < wide - 1; i++)
            {
                ostr += string.Format("WIRE:I{0}-O0:GL_DUMMY_{0}-I0\r\n", i);
                ostr += string.Format("WIRE:GL_DUMMY_{0}-O0:O{1}-I0\r\n", i, i + 1);
            }
            ostr += string.Format("WIRE:I{0}-O0:O0-I0\r\n", (wide-1));
            Console.WriteLine("LLC_" + wide);
            System.IO.File.WriteAllText("LLC_" + wide + ".MNET", ostr);
        }
        private static void GenRRC(int wide)
        {
            //Gen ports
            string ostr = "";
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:INPort:I{0}\r\n", i);
            }
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:OUTPort:O{0}\r\n", i);
            }
            //Gen NODES
            for (int i = 0; i < wide; i++)
            {
                ostr += string.Format("NODE:DUMMY:GL_DUMMY_{0}\r\n", i);
            }
            //Gen WIRES
            for (int i = 1; i < wide; i++)
            {
                ostr += string.Format("WIRE:I{0}-O0:GL_DUMMY_{0}-I0\r\n", i);
                ostr += string.Format("WIRE:GL_DUMMY_{0}-O0{0}:O{1}-I0{0}\r\n", i, i - 1);
            }
            ostr += string.Format("WIRE:I0-O0:O{0}-I0\r\n", wide - 1);
            Console.WriteLine("RRC_" + wide);
            System.IO.File.WriteAllText("RRC_" + wide + ".MNET", ostr);
        }

    }
}
