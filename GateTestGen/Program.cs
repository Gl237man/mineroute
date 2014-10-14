using System;
using System.IO;

namespace GateTestGen
{
    internal static class Program
    {
        private static void Main()
        {
            //AND Gen
            for (int wide = 2; wide < 9; wide++)
            {
                string testFile = "";
                testFile += string.Format("load ( AND{0} )\r\n", wide);
                testFile += "wait (5)\r\n";
                testFile += "checkstruct()\r\n";
                testFile += "checkio()\r\n";
                int power = Power(wide);
                for (int i = 0; i < power; i++)
                {
                    int[] bits = GetBits(i);
                    for (int j = 0; j < wide; j++)
                    {
                        testFile += string.Format("set(I{0}, {1})\r\n", j, bits[j]);
                    }

                    testFile += "wait(5)\r\n";
                    testFile += "read(O0)\r\n";
                    testFile += string.Format("test(O0 , {0})\r\n", Convert.ToInt32(i == power - 1));
                }
                File.WriteAllText(string.Format("AND{0}.emu", wide), testFile);
                Console.WriteLine("AND{0}", wide);
            }
            //OR Gen
            for (int wide = 2; wide < 9; wide++)
            {
                string testFile = "";
                testFile += string.Format("load ( OR{0} )\r\n", wide);
                testFile += "wait (5)\r\n";
                testFile += "checkstruct()\r\n";
                testFile += "checkio()\r\n";
                int power = Power(wide);
                for (int i = 0; i < power; i++)
                {
                    int[] bits = GetBits(i);
                    for (int j = 0; j < wide; j++)
                    {
                        testFile += string.Format("set(I{0}, {1})\r\n", j, bits[j]);
                    }

                    testFile += "wait(5)\r\n";
                    testFile += "read(O0)\r\n";
                    testFile += string.Format("test(O0 , {0})\r\n", Convert.ToInt32(i != 0));
                }
                File.WriteAllText(string.Format("OR{0}.emu", wide), testFile);
                Console.WriteLine("OR{0}", wide);
            }
        }

        private static int Power(int wide)
        {
            int power = 1;
            for (int i = 0; i < wide; i++)
            {
                power = power*2;
            }
            return power;
        }

        private static int[] GetBits(int val)
        {
            var bits = new int[16];
            for (int i = 0; i < 16; i++)
            {
                bits[i] = val & 1;
                val = val >> 1;
            }
            return bits;
        }
    }
}