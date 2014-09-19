using System;
using RouteUtils;

namespace BinhlEmul
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string fileName = args.Length < 1 ? "TRIG_D" : args[0];

            var node = new Node(fileName + ".binhl");

            var world = new World(node);

            var r = new Render(world);

            world.SetPortValue("datain", false);
            world.SetPortValue("clk", false);
            world.SetPortValue("sclr", false);

            int tim = 0;

            for (var i = 0; i < 10; i++)
            {
                world.Tick();
                r.GetSingeLayeImage().Save("I" + tim + ".png"); tim++;
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            world.SetPortValue("clk", true);
            for (var i = 0; i < 10; i++)
            {
                world.Tick();
                r.GetSingeLayeImage().Save("I" + tim + ".png"); tim++;
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            world.SetPortValue("clk", false);
            for (int i = 0; i < 10; i++)
            {
                world.Tick();
                r.GetSingeLayeImage().Save("I" + tim + ".png"); tim++;
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            world.SetPortValue("datain", true);

            world.SetPortValue("clk", false);
            for (int i = 0; i < 10; i++)
            {
                world.Tick();
                r.GetSingeLayeImage().Save("I" + tim + ".png"); tim++;
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            world.SetPortValue("clk", true);
            for (var i = 0; i < 10; i++)
            {
                world.Tick();
                r.GetSingeLayeImage().Save("I" + tim + ".png"); tim++;
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            world.SetPortValue("clk", false);
            for (int i = 0; i < 10; i++)
            {
                world.Tick();
                r.GetSingeLayeImage().Save("I" + tim + ".png"); tim++;
                Console.WriteLine("regout=" + world.GetPortValue("regout"));
            }

            //Test
            /*
            
            
            
            world.SetPortValue("I0", false);
            world.SetPortValue("I1", false);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();

            Console.WriteLine("O0={0}", world.GetPortValue("O0"));

            world.SetPortValue("I0", true);
            world.SetPortValue("I1", false);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();

            Console.WriteLine("O0={0}", world.GetPortValue("O0"));

            world.SetPortValue("I0", false);
            world.SetPortValue("I1", true);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();

            Console.WriteLine("O0={0}", world.GetPortValue("O0"));

            world.SetPortValue("I0", true);
            world.SetPortValue("I1", true);
            world.Tick();
            world.Tick();
            world.Tick();
            world.Tick();
            */
            Console.WriteLine("O0={0}", world.GetPortValue("O0"));

            r.GetSingeLayeImage().Save("I.png");
        }
    }
}