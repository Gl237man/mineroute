using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnetLibToBinLib
{
    class Program
    {
        static void Main(string[] args)
        {
            BinLib.Blib B = new BinLib.Blib();

            for (int i = 1; i <= 0xFD; i++)
            {
                //OptCo_
                string FileName = "lutc_" + i.ToString("X2") + ".MNET";
                string FullName = @"MNETLib\OptCo\" + FileName;
                string[] s = System.IO.File.ReadAllLines(FullName);
                B.WriteAllLines(FileName, s);
            }

            for (int i = 1; i <= 0xFFFD; i++)
            {
                string FileName = "lut_" + i.ToString("X4") + ".MNET";
                string FullName = @"MNETLib\" + i.ToString("X4").Substring(0, 1) + @"\" + FileName;
                string[] s = System.IO.File.ReadAllLines(FullName);
                B.WriteAllLines(FileName, s);
                Console.WriteLine(FullName);
            }
            Console.WriteLine("Compressing...");
            B.Save("MNETLib.BinLib");
        }
    }
}
