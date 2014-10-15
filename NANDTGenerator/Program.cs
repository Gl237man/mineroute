using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NANDTGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //GenerateNandT2
            //string node = "";
            GenNANDT2();
            GenNANDT3();
            GenNANDT4();
            GenNANDT5();
            GenNANDT6();
            GenNANDT7();
            GenNANDT8();
        }

        private static void GenNANDT8()
        {
        }

        private static void GenNANDT7()
        {
        }

        private static void GenNANDT6()
        {
        }

        private static void GenNANDT5()
        {
        }

        private static void GenNANDT4()
        {
            for (int val = 0; val < Power(4); val++)
            {
                var bits = GetBits(val);
                string name = "NANDT4_" + val.ToString("X2");
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Name:" + name);
                builder.AppendLine("in:4");
                builder.AppendLine("out:1");
                builder.AppendLine("in:I0:1:10");
                builder.AppendLine("in:I1:3:10");
                builder.AppendLine("in:I2:5:10");
                builder.AppendLine("in:I3:7:10");
                builder.AppendLine("out:O0:3:1");
                builder.AppendLine("size:9:11");
                builder.AppendLine("layers:7");
                builder.AppendLine("layer:0");
                builder.AppendLine("000000000");
                builder.AppendLine("000W00000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("0W0W0W0W0");
                builder.AppendLine("layer:1");
                builder.AppendLine("000W00000");
                builder.AppendLine("000#00000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("0W0W0W0W0");
                builder.AppendLine("0#0#0#0#0");
                builder.AppendLine("layer:2");
                builder.AppendLine("00W#00000");
                builder.AppendLine("00W000000");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0#0#0#0#0");
                builder.AppendLine("000000000");
                builder.AppendLine("layer:3");
                builder.AppendLine("00#000000");
                builder.AppendLine("00_000000");
                builder.AppendLine("0WWWWWWW0");
                builder.AppendLine("0W0W0W0W0");
                builder.AppendLine("0W0W0W0W0");
                builder.AppendLine("0#0#0#0#0");
                builder.AppendLine("0" + (bits[3] == 0 ? "#" : "_") + "0" + (bits[2] == 0 ? "#" : "_") + "0" + (bits[1] == 0 ? "#" : "_") + "0" + (bits[0] == 0 ? "#" : "_") + "0");
                builder.AppendLine("0" + (bits[3] == 0 ? "#" : "W") + "0" + (bits[2] == 0 ? "#" : "W") + "0" + (bits[1] == 0 ? "#" : "W") + "0" + (bits[0] == 0 ? "#" : "W") + "0");
                builder.AppendLine("0#0#0#0#0");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("layer:4");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("0#######0");
                builder.AppendLine("0#0#0#0#0");
                builder.AppendLine("0*0*0*0*0");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("layer:5");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("layer:6");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");
                builder.AppendLine("000000000");

                System.IO.File.WriteAllText(name + ".binhl", builder.ToString());
                Console.WriteLine(name);
            }
        }

        private static void GenNANDT3()
        {
            for (int val = 0; val < Power(3); val++)
            {
                var bits = GetBits(val);
                string name = "NANDT3_" + val.ToString("X2");
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Name:" + name);
                builder.AppendLine("in:3");
                builder.AppendLine("out:1");
                builder.AppendLine("in:I0:1:10");
                builder.AppendLine("in:I1:3:10");
                builder.AppendLine("in:I2:5:10");
                builder.AppendLine("out:O0:3:1");
                builder.AppendLine("size:7:11");
                builder.AppendLine("layers:7");
                builder.AppendLine("layer:0");
                builder.AppendLine("0000000");
                builder.AppendLine("000W000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0W0W0W0");
                builder.AppendLine("layer:1");
                builder.AppendLine("000W000");
                builder.AppendLine("000#000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0W0W0W0");
                builder.AppendLine("0#0#0#0");
                builder.AppendLine("layer:2");
                builder.AppendLine("00W#000");
                builder.AppendLine("00W0000");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0#0#0#0");
                builder.AppendLine("0000000");
                builder.AppendLine("layer:3");
                builder.AppendLine("00#0000");
                builder.AppendLine("00_0000");
                builder.AppendLine("0WWWWW0");
                builder.AppendLine("0W0W0W0");
                builder.AppendLine("0W0W0W0");
                builder.AppendLine("0#0#0#0");
                builder.AppendLine("0" + (bits[2] == 0 ? "#" : "_") + "0" + (bits[1] == 0 ? "#" : "_") + "0" + (bits[0] == 0 ? "#" : "_") + "0");
                builder.AppendLine("0" + (bits[2] == 0 ? "#" : "W") + "0" + (bits[1] == 0 ? "#" : "W") + "0" + (bits[0] == 0 ? "#" : "W") + "0");
                builder.AppendLine("0#0#0#0");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("layer:4");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0#####0");
                builder.AppendLine("0#0#0#0");
                builder.AppendLine("0*0*0*0");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("layer:5");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("layer:6");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                builder.AppendLine("0000000");
                System.IO.File.WriteAllText(name + ".binhl", builder.ToString());
                Console.WriteLine(name);
            }
        }

        private static void GenNANDT2()
        {
            for (int val = 0; val < Power(2); val++)
            {
                var bits = GetBits(val);
                string name = "NANDT2_" + val.ToString("X2");
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Name:" + name);
                builder.AppendLine("in:2");
                builder.AppendLine("out:1");
                builder.AppendLine("in:I0:1:10");
                builder.AppendLine("in:I1:3:10");
                builder.AppendLine("out:O0:3:1");
                builder.AppendLine("size:5:11");
                builder.AppendLine("layers:7");
                builder.AppendLine("layer:0");
                builder.AppendLine("00000");
                builder.AppendLine("000W0");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("0W0W0");
                builder.AppendLine("layer:1");
                builder.AppendLine("000W0");
                builder.AppendLine("000#0");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("0W0W0");
                builder.AppendLine("0#0#0");
                builder.AppendLine("layer:2");
                builder.AppendLine("00W#0");
                builder.AppendLine("00W00");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0#0#0");
                builder.AppendLine("00000");
                builder.AppendLine("layer:3");
                builder.AppendLine("00#00");
                builder.AppendLine("00_00");
                builder.AppendLine("0WWW0");
                builder.AppendLine("0W0W0");
                builder.AppendLine("0W0W0");
                builder.AppendLine("0#0#0");
                builder.AppendLine("0" + (bits[1] == 0 ? "#" : "_") + "0" + (bits[0] == 0 ? "#" : "_") + "0");
                builder.AppendLine("0" + (bits[1] == 0 ? "#" : "W") + "0" + (bits[0] == 0 ? "#" : "W") + "0");
                builder.AppendLine("0#0#0");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("layer:4");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("0###0");
                builder.AppendLine("0#0#0");
                builder.AppendLine("0*0*0");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("layer:5");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("layer:6");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                builder.AppendLine("00000");
                System.IO.File.WriteAllText(name + ".binhl", builder.ToString());
                Console.WriteLine(name);
            }
        }
        private static int Power(int wide)
        {
            int power = 1;
            for (int i = 0; i < wide; i++)
            {
                power = power * 2;
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
