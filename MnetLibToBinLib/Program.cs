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
                string fileName = "lutc_" + i.ToString("X2") + ".MNET";
                string fullName = @"MNETLib\OptCo\" + fileName;
                string[] s = System.IO.File.ReadAllLines(fullName);
                binlib.WriteAllLines(fileName, s);
            }

            for (int i = 0; i <= 0xFFFF; i++)
            {
                string fileName = "lut_" + i.ToString("X4") + ".MNET";
                string fullName = @"MNETLib\" + i.ToString("X4").Substring(0, 1) + @"\" + fileName;
                string[] s = System.IO.File.ReadAllLines(fullName);
                binlib.WriteAllLines(fileName, s);
                Console.WriteLine(fullName);
            }
            Console.WriteLine("Compressing...");
            binlib.Save("MNETLib.BinLib");
        }
    }
}
