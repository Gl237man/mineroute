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
            int lutNum = 0x5632;
            //Gen MNET
            string MNETFile = "";
            MNETFile += "NODE:INPort:I0" + "\r\n";
            MNETFile += "NODE:INPort:I1" + "\r\n";
            MNETFile += "NODE:INPort:I2" + "\r\n";
            MNETFile += "NODE:INPort:I3" + "\r\n";
            MNETFile += "NODE:OUTPort:nout" + "\r\n";
            //MNETFile += "NODE:OUTPort:cout" + "\r\n";
            MNETFile += "NODE:C2LUT_"+ lutNum.ToString("X4") +"_datac:lut1" + "\r\n";
            //MNETFile += "WIRE:lut1-cout:cout-I0" + "\r\n";
            MNETFile += "WIRE:lut1-combout:nout-I0" + "\r\n";
            MNETFile += "WIRE:I0-O0:lut1-dataa" + "\r\n";
            MNETFile += "WIRE:I1-O0:lut1-datab" + "\r\n";
            MNETFile += "WIRE:I2-O0:lut1-datac" + "\r\n";
            MNETFile += "WIRE:I3-O0:lut1-datad" + "\r\n";
            System.IO.File.WriteAllText("lut_" + lutNum.ToString("X4") + ".MNET", MNETFile);
            //Gen Test
            //Gen test.CMD
        }
    }
}
