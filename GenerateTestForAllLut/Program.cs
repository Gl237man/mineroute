﻿using System;
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
            int lutNum = 0x5632;
            //Gen MNET
            string MNETFile = "";
            MNETFile += "NODE:INPort:I0" + "\r\n";
            MNETFile += "NODE:INPort:I1" + "\r\n";
            MNETFile += "NODE:INPort:I2" + "\r\n";
            MNETFile += "NODE:INPort:I3" + "\r\n";
            MNETFile += "NODE:OUTPort:nout" + "\r\n";
            MNETFile += "NODE:OUTPort:cout" + "\r\n";
            MNETFile += "NODE:C2LUT_"+ lutNum.ToString("X4") +"_datac:lut1" + "\r\n";
            MNETFile += "WIRE:lut1-cout:cout-I0" + "\r\n";
            MNETFile += "WIRE:lut1-combout:nout-I0" + "\r\n";
            MNETFile += "WIRE:I0-O0:lut1-dataa" + "\r\n";
            MNETFile += "WIRE:I1-O0:lut1-datab" + "\r\n";
            MNETFile += "WIRE:I2-O0:lut1-datac" + "\r\n";
            MNETFile += "WIRE:I3-O0:lut1-datad" + "\r\n";
            System.IO.File.WriteAllText("lut_" + lutNum.ToString("X4") + ".MNET", MNETFile);
            //Gen Test
            string TestFile = "";
            TestFile += "load ( lut_" + lutNum.ToString("X4") +"_D )" + "\r\n";
            TestFile += "wait (5)" + "\r\n";
            TestFile += "checkstruct()" + "\r\n";
            TestFile += "checkio()" + "\r\n";
            for (int i = 0; i < 16; i++)
            {
                int[] bits = GetBits(i);
                TestFile += "set(I0, " + bits[0] + ")" + "\r\n";
                TestFile += "set(I1, " + bits[1] + ")" + "\r\n";
                TestFile += "set(I2, " + bits[2] + ")" + "\r\n";
                TestFile += "set(I3, " + bits[3] + ")" + "\r\n";
                TestFile += "wait(30)" + "\r\n";
                TestFile += "read(nout)" + "\r\n";
                TestFile += "test(nout , " + GetBits(lutNum)[i] + ")" + "\r\n";
            }
            System.IO.File.WriteAllText("lut_" + lutNum.ToString("X4") + ".emu", TestFile);
            //Gen test.CMD
        }

        private static int[] GetBits(int i)
        {
            int[] k = new int[16];
            for (int j = 0; j < 15; j++)
            {
                k[j] = i & 1;
                i = i >> 1;
            }
            return k;
        }
    }
}
