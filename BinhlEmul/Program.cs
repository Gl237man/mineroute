using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteUtils;

namespace BinhlEmul
{
    class Program
    {
        static void Main(string[] args)
        {
            string FileName = "";
            if (args.Length < 1)
            {
                FileName = "test";
            }
            else
            {
                FileName = args[0];
            }

            Node N = new Node(FileName + ".binhl");

            World W = new World(N);



        }
    }
}
