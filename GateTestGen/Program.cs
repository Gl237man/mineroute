using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateTestGen
{
    class Program
    {
        static void Main(string[] args)
        {
            //AND Gen
            for (int wide = 2; wide < 9; wide++)
            {
                string TestFile = "";
                TestFile += "load ( AND" + wide + " )" + "\r\n";
                TestFile += "wait (5)" + "\r\n";
                TestFile += "checkstruct()" + "\r\n";
                TestFile += "checkio()" + "\r\n";
                int pow = Power(wide);
                for (int i = 0; i < pow; i++)
                {
                    int[] bits = GetBits(i);
                    for (int j = 0; j < wide; j++)
                    {
                        TestFile += "set(I" + j + ", " + bits[j] + ")" + "\r\n";
                    }

                    TestFile += "wait(5)" + "\r\n";
                    TestFile += "read(O0)" + "\r\n";
                    TestFile += "test(O0 , " + Convert.ToInt32(i == pow-1) + ")" + "\r\n";
                }
                System.IO.File.WriteAllText("AND" + wide + ".emu", TestFile);
                Console.WriteLine("AND" + wide);
            }
            //OR Gen
            for (int wide = 2; wide < 9; wide++)
            {
                string TestFile = "";
                TestFile += "load ( OR" + wide + " )" + "\r\n";
                TestFile += "wait (5)" + "\r\n";
                TestFile += "checkstruct()" + "\r\n";
                TestFile += "checkio()" + "\r\n";
                int pow = Power(wide);
                for (int i = 0; i < pow; i++)
                {
                    int[] bits = GetBits(i);
                    for (int j = 0; j < wide; j++)
                    {
                        TestFile += "set(I" + j + ", " + bits[j] + ")" + "\r\n";
                    }

                    TestFile += "wait(5)" + "\r\n";
                    TestFile += "read(O0)" + "\r\n";
                    TestFile += "test(O0 , " + Convert.ToInt32(i != 0) + ")" + "\r\n";
                }
                System.IO.File.WriteAllText("OR" + wide + ".emu", TestFile);
                Console.WriteLine("OR" + wide);
            }
        }
        private static int Power(int i)
        {
            int p = 1;
            for (int q = 0; q < i; q++)
            {
                p = p * 2;
            }
            return p;
        }
        private static int[] GetBits(int i)
        {
            int[] k = new int[16];
            for (int j = 0; j < 16; j++)
            {
                k[j] = i & 1;
                i = i >> 1;
            }
            return k;
        }
    }
}
