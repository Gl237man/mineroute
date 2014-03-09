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

        }

        
    }
}
