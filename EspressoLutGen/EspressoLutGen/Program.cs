using System;
using System.IO;

namespace EspressoLutGen
{
    internal static class Program
    {
        private static void Main()
        {
            string cmdFile = "";

            for (int i = 0; i <= 0xFFFF; i++)
            {
                string s = LutFileGen(i);
                Console.WriteLine("Lut_{0}.txt", i.ToString("X4"));
                File.WriteAllText(string.Format("Lut_{0}.txt", i.ToString("X4")), s);
                cmdFile += string.Format("echo {0}\r\n", i.ToString("X4"));
                cmdFile += string.Format("espresso.exe Lut_{0}.txt>Opt_{0}.txt\r\n", i.ToString("X4"));
                cmdFile += string.Format("del Lut_{0}.txt\r\n", i.ToString("X4"));
            }

            for (int i = 0; i <= 0x00FF; i++)
            {
                string s = CoutFileGen(i);
                Console.WriteLine("Cout_{0}.txt", i.ToString("X2"));
                File.WriteAllText(string.Format("Cout_{0}.txt", i.ToString("X2")), s);
                cmdFile += string.Format("echo {0}\r\n", i.ToString("X2"));
                cmdFile += string.Format("espresso.exe Cout_{0}.txt>OptCo_{0}.txt\r\n", i.ToString("X2"));
                cmdFile += string.Format("del Cout_{0}.txt\r\n", i.ToString("X2"));
            }
            File.WriteAllText("Start.cmd", cmdFile);
        }


        private static string CoutFileGen(int lutValue)
        {
            string ostr = "";

            var bmass = new int[8];

            for (int i = 0; i < 8; i++)
            {
                bmass[i] = lutValue >> i & 1;
            }

            ostr += ".i 3\r\n";
            ostr += ".o 1\r\n";
            ostr += ".ilb B C D\r\n";
            ostr += ".ob F\r\n";
            ostr += string.Format("0 0 0 {0}\r\n", bmass[0]);
            ostr += string.Format("0 0 1 {0}\r\n", bmass[1]);
            ostr += string.Format("0 1 0 {0}\r\n", bmass[2]);
            ostr += string.Format("0 1 1 {0}\r\n", bmass[3]);
            ostr += string.Format("1 0 0 {0}\r\n", bmass[4]);
            ostr += string.Format("1 0 1 {0}\r\n", bmass[5]);
            ostr += string.Format("1 1 0 {0}\r\n", bmass[6]);
            ostr += string.Format("1 1 1 {0}\r\n", bmass[7]);
            ostr += ".e\r\n";

            return ostr;
        }

        private static string LutFileGen(int lutValue)
        {
            string ostr = "";

            var bmass = new int[16];

            for (int i = 0; i < 16; i++)
            {
                bmass[i] = lutValue >> i & 1;
            }

            ostr += ".i 4\r\n";
            ostr += ".o 1\r\n";
            ostr += ".ilb A B C D\r\n";
            ostr += ".ob F\r\n";
            ostr += string.Format("0 0 0 0 {0}\r\n", bmass[0]);
            ostr += string.Format("0 0 0 1 {0}\r\n", bmass[1]);
            ostr += string.Format("0 0 1 0 {0}\r\n", bmass[2]);
            ostr += string.Format("0 0 1 1 {0}\r\n", bmass[3]);
            ostr += string.Format("0 1 0 0 {0}\r\n", bmass[4]);
            ostr += string.Format("0 1 0 1 {0}\r\n", bmass[5]);
            ostr += string.Format("0 1 1 0 {0}\r\n", bmass[6]);
            ostr += string.Format("0 1 1 1 {0}\r\n", bmass[7]);
            ostr += string.Format("1 0 0 0 {0}\r\n", bmass[8]);
            ostr += string.Format("1 0 0 1 {0}\r\n", bmass[9]);
            ostr += string.Format("1 0 1 0 {0}\r\n", bmass[10]);
            ostr += string.Format("1 0 1 1 {0}\r\n", bmass[11]);
            ostr += string.Format("1 1 0 0 {0}\r\n", bmass[12]);
            ostr += string.Format("1 1 0 1 {0}\r\n", bmass[13]);
            ostr += string.Format("1 1 1 0 {0}\r\n", bmass[14]);
            ostr += string.Format("1 1 1 1 {0}\r\n", bmass[15]);
            ostr += ".e\r\n";

            return ostr;
        }
    }
}