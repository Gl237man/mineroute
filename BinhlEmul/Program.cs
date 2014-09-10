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
                FileName = "OR";
            }
            else
            {
                FileName = args[0];
            }

            Node N = new Node(FileName + ".binhl");

            World W = new World(N);

            W.SetPortValue("datain", false);
            W.SetPortValue("clk", false);
            W.SetPortValue("sclr", false);
            for (int i = 0; i < 50; i++)
            {
                W.Tick();
                Console.WriteLine("regout=" + W.GetPortValue("regout"));
            }

            W.SetPortValue("clk", true);
            for (int i = 0; i < 50; i++)
            {
                W.Tick();
                Console.WriteLine("regout=" + W.GetPortValue("regout"));
            }

            W.SetPortValue("clk", true);
            for (int i = 0; i < 50; i++)
            {
                W.Tick();
                Console.WriteLine("regout=" + W.GetPortValue("regout"));
            }

            //Test
            /*
            W.SetPortValue("I0", false);
            W.SetPortValue("I1", false);
            W.Tick();
            W.Tick();
            W.Tick();
            W.Tick();

            Console.WriteLine("O0=" + W.GetPortValue("O0"));

            W.SetPortValue("I0", true);
            W.SetPortValue("I1", false);
            W.Tick();
            W.Tick();
            W.Tick();
            W.Tick();

            Console.WriteLine("O0=" + W.GetPortValue("O0"));

            W.SetPortValue("I0", false);
            W.SetPortValue("I1", true);
            W.Tick();
            W.Tick();
            W.Tick();
            W.Tick();

            Console.WriteLine("O0=" + W.GetPortValue("O0"));

            W.SetPortValue("I0", true);
            W.SetPortValue("I1", true);
            W.Tick();
            W.Tick();
            W.Tick();
            W.Tick();

            Console.WriteLine("O0=" + W.GetPortValue("O0"));
            */

        }
    }
}
