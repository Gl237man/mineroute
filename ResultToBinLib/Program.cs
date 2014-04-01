using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultToBinLib
{
    class Program
    {
        static void Main(string[] args)
        {
            BinLib.Blib B = new BinLib.Blib();

            for (int i = 0; i <= 0xFF; i++)
            {
                //OptCo_
                string FileName = "OptCo_" + i.ToString("X2") + ".txt";
                string FullName = @"Result\OptCo\" + FileName;
                string[] s = System.IO.File.ReadAllLines(FullName);
                B.WriteAllLines(FileName, s);
            }

            for (int i = 0; i <= 0xFFFF; i++)
            {
                string FileName = "Opt_" + i.ToString("X4") + ".txt";
                string FullName = @"Result\" + i.ToString("X4").Substring(0, 1) + @"\" + FileName;
                string[] s = System.IO.File.ReadAllLines(FullName);
                B.WriteAllLines(FileName, s);
                Console.WriteLine(FullName);
            }
            B.Save("Result.BinLib");
        }
    }
}
