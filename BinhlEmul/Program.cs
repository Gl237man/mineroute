using System;
using RouteUtils;

namespace BinhlEmul
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string fileName = args.Length < 1 ? "test" : args[0];

            var node = new Node(fileName + ".binhl");

            var world = new World(node);

            /*
            world.SetPortValue("datain", false);
            world.SetPortValue("clk", false);
            world.SetPortValue("sclr", false);
            for (var i = 0; i < 50; i++)
            {
                world.Tick();
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            world.SetPortValue("clk", true);
            for (var i = 0; i < 50; i++)
            {
                world.Tick();
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            world.SetPortValue("clk", true);
            for (int i = 0; i < 50; i++)
            {
                world.Tick();
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }
            */

            //Test

            world.SetPortValue("I0", false);
            world.SetPortValue("I1", false);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();

            Console.WriteLine("O0=" + world.GetPortValue("O0"));

            world.SetPortValue("I0", true);
            world.SetPortValue("I1", false);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();

            Console.WriteLine("O0=" + world.GetPortValue("O0"));

            world.SetPortValue("I0", false);
            world.SetPortValue("I1", true);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();

            Console.WriteLine("O0=" + world.GetPortValue("O0"));

            world.SetPortValue("I0", true);
            world.SetPortValue("I1", true);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();

            Console.WriteLine("O0=" + world.GetPortValue("O0"));
        }
    }
}