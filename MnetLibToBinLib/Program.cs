using System;

namespace MnetLibToBinLib
{
    static class Program
    {
        static void Main()
        {
            var binlib = new BinLib.Blib();

            for (int i = 0; i <= 0xFF; i++)
            {
                string fileName = string.Format("lutc_{0}.MNET", i.ToString("X2"));
                string fullName = string.Format(@"MNETLib\OptCo\{0}", fileName);
                string[] s = System.IO.File.ReadAllLines(fullName);
                binlib.WriteAllLines(fileName, s);
            }

            for (int i = 0; i <= 0xFFFF; i++)
            {
                string fileName = string.Format("lut_{0}.MNET", i.ToString("X4"));
                string fullName = string.Format(@"MNETLib\{0}\{1}", i.ToString("X4").Substring(0, 1), fileName);
                string[] s = System.IO.File.ReadAllLines(fullName);
                binlib.WriteAllLines(fileName, s);
                Console.WriteLine(fullName);
            }
            Console.WriteLine("Compressing...");
            binlib.Save("MNETLib.BinLib");
        }
    }
}
