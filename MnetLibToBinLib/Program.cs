using System;

namespace MnetLibToBinLib
{
    static class Program
    {
        static void Main()
        {
            var binlib = new BinLib.Blib();

            for (int i = 1; i <= 0xFD; i++)
            {
                //OptCo_
                string fileName = "lutc_" + i.ToString("X2") + ".MNET";
                string fullName = @"MNETLib\OptCo\" + fileName;
                string[] s = System.IO.File.ReadAllLines(fullName);
                binlib.WriteAllLines(fileName, s);
            }

            for (int i = 1; i <= 0xFFFD; i++)
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
