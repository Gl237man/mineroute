using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateTestForAllLut
{
    class Program
    {
        static void Main(string[] args)
        {
            //int lutNum = 0x5632;
            //Gen MNET
            for (int lutNum = 0; lutNum <= 0xFFFF; lutNum++)
            {
                string MNETFile = "";
                MNETFile += "NODE:INPort:I0" + "\r\n";
                MNETFile += "NODE:INPort:I1" + "\r\n";
                MNETFile += "NODE:INPort:I2" + "\r\n";
                MNETFile += "NODE:INPort:I3" + "\r\n";
                MNETFile += "NODE:OUTPort:nout" + "\r\n";
                MNETFile += "NODE:OUTPort:cout" + "\r\n";
                MNETFile += "NODE:C2LUT_" + lutNum.ToString("X4") + "_datac:lut1" + "\r\n";
                MNETFile += "WIRE:lut1-cout:cout-I0" + "\r\n";
                MNETFile += "WIRE:lut1-combout:nout-I0" + "\r\n";
                MNETFile += "WIRE:I0-O0:lut1-dataa" + "\r\n";
                MNETFile += "WIRE:I1-O0:lut1-datab" + "\r\n";
                MNETFile += "WIRE:I2-O0:lut1-datac" + "\r\n";
                MNETFile += "WIRE:I3-O0:lut1-datad" + "\r\n";
                System.IO.File.WriteAllText(lutNum.ToString("X4").Substring(0, 1) + @"\lut_" + lutNum.ToString("X4") + ".MNET", MNETFile);
                //Gen Test
                string TestFile = "";
                TestFile += "load ( lut_" + lutNum.ToString("X4") + "_D )" + "\r\n";
                TestFile += "wait (50)" + "\r\n";
                TestFile += "checkstruct()" + "\r\n";
                TestFile += "checkio()" + "\r\n";
                for (int i = 0; i < 16; i++)
                {
                    int[] bits = GetBits(i);
                    TestFile += "set(I0, " + bits[0] + ")" + "\r\n";
                    TestFile += "set(I1, " + bits[1] + ")" + "\r\n";
                    TestFile += "set(I2, " + bits[2] + ")" + "\r\n";
                    TestFile += "set(I3, " + bits[3] + ")" + "\r\n";
                    TestFile += "wait(120)" + "\r\n";
                    TestFile += "read(nout)" + "\r\n";
                    TestFile += "test(nout , " + GetBits(lutNum)[i] + ")" + "\r\n";
                }
                System.IO.File.WriteAllText(lutNum.ToString("X4").Substring(0,1)+ @"\lut_" + lutNum.ToString("X4") + ".emu", TestFile);
                Console.WriteLine("lut_" + lutNum.ToString("X4") + ".emu - Generated");
            }
            //Gen test.CMD
            StringBuilder sb = new StringBuilder();
            string CMDFile = "";
            for (int lutNum = 0; lutNum <= 0xFFFF; lutNum++)
            {
                sb.Append("cd " + lutNum.ToString("X4").Substring(0, 1) + "\r\n");
                sb.Append("MnetLutDecomposite.exe " + "lut_" + lutNum.ToString("X4") + "\r\n");
                sb.Append("mnetsynt2.exe " + "lut_" + lutNum.ToString("X4") + "_D" + "\r\n");
                sb.Append("BinhlEmul.exe " + "lut_" + lutNum.ToString("X4") + ".emu" + "\r\n");
                sb.Append("cd .." + "\r\n");
            }
            CMDFile = sb.ToString();
            System.IO.File.WriteAllText("run.cmd", CMDFile);

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
