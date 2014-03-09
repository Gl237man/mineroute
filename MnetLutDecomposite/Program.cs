using System;
using System.Collections.Generic;
using System.Text;

namespace MnetLutDecomposite
{
    class Program
    {
        static Mnet MainNet;

        static void Main(string[] args)
        {
            MainNet = new Mnet();
            MainNet.ReadMnetFile(@"test2.MNET");
            List<Node> Luts = MainNet.GetLuts();
            List<Mnet> LutsMnet = new List<Mnet>();
            for (int i = 0; i < Luts.Count; i++)
            {
                Mnet Lnet = new Mnet();
                Lnet.ReadMnetFile(@"MNETLib\" + Luts[i].GetLutKey().Substring(0, 1) + @"\lut_" + Luts[i].GetLutKey() + ".MNET");
                LutsMnet.Add(Lnet);
            }
            
        }

        
    }
}
