using System;

namespace ResultToBinLib
{
    static class Program
    {
        static void Main()
        {
            var blib = new BinLib.Blib();

            for (int i = 0; i <= 0xFF; i++)
            {
                string fileName = string.Format("OptCo_{0}.txt", i.ToString("X2"));
                string fullName = string.Format(@"Result\OptCo\{0}", fileName);
                string[] s = System.IO.File.ReadAllLines(fullName);
                blib.WriteAllLines(fileName, s);
            }

            for (int i = 0; i <= 0xFFFF; i++)
            {
                string fileName = string.Format("Opt_{0}.txt", i.ToString("X4"));
                string fullName = string.Format(@"Result\{0}\{1}", i.ToString("X4").Substring(0, 1), fileName);
                string[] s = System.IO.File.ReadAllLines(fullName);
                blib.WriteAllLines(fileName, s);
                Console.WriteLine(fullName);
            }
            blib.Save("Result.BinLib");
        }
    }
}
