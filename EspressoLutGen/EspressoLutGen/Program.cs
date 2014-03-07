using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EspressoLutGen
{
    class Program
    {
        static void Main(string[] args)
        {
            string CmdFile = "";
            
            for (int i = 0; i <= 0xFFFF; i++)
            {
                string s = LutFileGen(i);
                Console.WriteLine("Lut_" + i.ToString("X4") + ".txt");
                System.IO.File.WriteAllText("Lut_" + i.ToString("X4") + ".txt", s);
                CmdFile += "echo " + i.ToString("X4") + "\r\n";
                CmdFile += "espresso.exe " + "Lut_" + i.ToString("X4") + ".txt>"  + "Opt_" + i.ToString("X4") + ".txt" + "\r\n";
                CmdFile += "del " + "Lut_" + i.ToString("X4") + ".txt" + "\r\n";
            }
            
            for (int i = 0; i <= 0x00FF; i++)
            {
                string s = CoutFileGen(i);
                Console.WriteLine("Cout_" + i.ToString("X2") + ".txt");
                System.IO.File.WriteAllText("Cout_" + i.ToString("X2") + ".txt", s);
                CmdFile += "echo " + i.ToString("X2") + "\r\n";
                CmdFile += "espresso.exe " + "Cout_" + i.ToString("X2") + ".txt>" + "OptCo_" + i.ToString("X2") + ".txt" + "\r\n";
                CmdFile += "del " + "Cout_" + i.ToString("X2") + ".txt" + "\r\n";
            }
            System.IO.File.WriteAllText("Start.cmd", CmdFile);


        }


        private static string CoutFileGen(int LutValue)
        {
            string ostr = "";

            int[] bmass = new int[8];

            for (int i = 0; i < 8; i++)
            {
                bmass[i] = LutValue >> i & 1;
            }

            ostr += ".i 3" + "\r\n";
            ostr += ".o 1" + "\r\n";
            ostr += ".ilb B C D" + "\r\n";
            ostr += ".ob F" + "\r\n";
            ostr += "0 0 0 " + bmass[0] + "\r\n";
            ostr += "0 0 1 " + bmass[1] + "\r\n";
            ostr += "0 1 0 " + bmass[2] + "\r\n";
            ostr += "0 1 1 " + bmass[3] + "\r\n";
            ostr += "1 0 0 " + bmass[4] + "\r\n";
            ostr += "1 0 1 " + bmass[5] + "\r\n";
            ostr += "1 1 0 " + bmass[6] + "\r\n";
            ostr += "1 1 1 " + bmass[7] + "\r\n";
            ostr += ".e" + "\r\n";

            return ostr;
        }

        private static string LutFileGen(int LutValue)
        {
            string ostr = "";

            int[] bmass = new int[16];

            for (int i = 0; i < 16; i++)
            {
                bmass[i] = LutValue >> i & 1;
            }

            ostr += ".i 4" + "\r\n";
            ostr += ".o 1" + "\r\n";
            ostr += ".ilb A B C D" + "\r\n";
            ostr += ".ob F" + "\r\n";
            ostr += "0 0 0 0 " + bmass[0] + "\r\n";
            ostr += "0 0 0 1 " + bmass[1] + "\r\n";
            ostr += "0 0 1 0 " + bmass[2] + "\r\n";
            ostr += "0 0 1 1 " + bmass[3] + "\r\n";
            ostr += "0 1 0 0 " + bmass[4] + "\r\n";
            ostr += "0 1 0 1 " + bmass[5] + "\r\n";
            ostr += "0 1 1 0 " + bmass[6] + "\r\n";
            ostr += "0 1 1 1 " + bmass[7] + "\r\n";
            ostr += "1 0 0 0 " + bmass[8] + "\r\n";
            ostr += "1 0 0 1 " + bmass[9] + "\r\n";
            ostr += "1 0 1 0 " + bmass[10] + "\r\n";
            ostr += "1 0 1 1 " + bmass[11] + "\r\n";
            ostr += "1 1 0 0 " + bmass[12] + "\r\n";
            ostr += "1 1 0 1 " + bmass[13] + "\r\n";
            ostr += "1 1 1 0 " + bmass[14] + "\r\n";
            ostr += "1 1 1 1 " + bmass[15] + "\r\n";
            ostr += ".e" + "\r\n";

            return ostr;
        }
    }
}
