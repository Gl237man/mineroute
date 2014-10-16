using System;
using System.Text;

namespace GenerateTestForAllLut
{
    static class Program
    {
        static void Main()
        {
            //Gen MNET
            for (int lutNum = 0; lutNum <= 0xFFFF; lutNum++)
            {
                string mnetFile = "";
                mnetFile += "NODE:INPort:I0\r\n";
                mnetFile += "NODE:INPort:I1\r\n";
                mnetFile += "NODE:INPort:I2\r\n";
                mnetFile += "NODE:INPort:I3\r\n";
                mnetFile += "NODE:OUTPort:nout\r\n";
                mnetFile += "NODE:OUTPort:cout\r\n";
                mnetFile += string.Format("NODE:C2LUT_{0}_datac:lut1\r\n", lutNum.ToString("X4"));
                mnetFile += "WIRE:lut1-cout:cout-I0\r\n";
                mnetFile += "WIRE:lut1-combout:nout-I0\r\n";
                mnetFile += "WIRE:I0-O0:lut1-dataa\r\n";
                mnetFile += "WIRE:I1-O0:lut1-datab\r\n";
                mnetFile += "WIRE:I2-O0:lut1-datac\r\n";
                mnetFile += "WIRE:I3-O0:lut1-datad\r\n";
                System.IO.File.WriteAllText(string.Format("{0}\\lut_{1}.MNET", lutNum.ToString("X4").Substring(0, 1), lutNum.ToString("X4")), mnetFile);
                //Gen Test
                string testFile = "";
                testFile += string.Format("load ( lut_{0}_D_O )\r\n", lutNum.ToString("X4"));
                testFile += "swait ()\r\n";
                testFile += "checkstruct()\r\n";
                testFile += "checkio()\r\n";
                for (int i = 0; i < 16; i++)
                {
                    int[] bits = GetBits(i);
                    testFile += string.Format("set(I0, {0})\r\n", bits[0]);
                    testFile += string.Format("set(I1, {0})\r\n", bits[1]);
                    testFile += string.Format("set(I2, {0})\r\n", bits[2]);
                    testFile += string.Format("set(I3, {0})\r\n", bits[3]);
                    testFile += "swait()\r\n";
                    testFile += "read(nout)\r\n";
                    testFile += string.Format("test(nout , {0})\r\n", GetBits(lutNum)[i]);
                }
                System.IO.File.WriteAllText(string.Format("{0}\\lut_{1}.emu", lutNum.ToString("X4").Substring(0,1), lutNum.ToString("X4")), testFile);
                Console.WriteLine("lut_{0}.emu - Generated", lutNum.ToString("X4"));
            }
            //Gen test.CMD
            var builder = new StringBuilder();
            for (int lutNum = 0; lutNum <= 0xFFFF; lutNum++)
            {
                builder.AppendFormat("cd {0}\r\n", lutNum.ToString("X4").Substring(0, 1));
                builder.AppendFormat("MnetLutDecomposite.exe lut_{0}\r\n", lutNum.ToString("X4"));
                builder.AppendFormat("MnetLutOptimise.exe lut_{0}_D\r\n", lutNum.ToString("X4"));
                builder.AppendFormat("mnetsynt2.exe lut_{0}_D_O\r\n", lutNum.ToString("X4"));
                builder.AppendFormat("BinhlEmul.exe lut_{0}.emu\r\n", lutNum.ToString("X4"));
                builder.Append("cd ..\r\n");
            }
            string cmdFile = builder.ToString();
            System.IO.File.WriteAllText("run.cmd", cmdFile);
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
