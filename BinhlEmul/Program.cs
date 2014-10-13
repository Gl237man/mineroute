using System;
using RouteUtils;
using System.Linq;
using System.Text.RegularExpressions;

namespace BinhlEmul
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            string fileName = args.Length < 1 ? "testscript.emu" : args[0];

            string[] testfile = System.IO.File.ReadAllLines(fileName);

            bool allTests = true;
            int numTests = 0;

            World world = new World();

            foreach (string str in testfile)
            {
                if (Regex.IsMatch(str, @"\bcheckio.*?\(.*?\)"))
                {
                    allTests = FCheckIo(allTests, world);
                }
                if (Regex.IsMatch(str, @"\bcheckstruct.*?\(.*?\)"))
                {
                    allTests = FTestStruct(world, allTests);
                }
                if (Regex.IsMatch(str, @"\bload.*?\(.*?\)"))
                {
                    world = FLoad(str);
                }
                if (Regex.IsMatch(str, @"\bwait.*?\(.*?\)"))
                {
                    FWait(world, str);
                }
                if (Regex.IsMatch(str, @"\bset.*?\(.*?\)"))
                {
                    FSet(world, str);
                }
                if (Regex.IsMatch(str, @"\bread.*?\(.*?\)"))
                {
                    FRead(world, str);
                }
                if (Regex.IsMatch(str, @"\btest.*?\(.*?\)"))
                {
                    FTest(ref allTests, ref numTests, world, str);
                }
                if (Regex.IsMatch(str, @"\bmultitest.*?\(.*?\)"))
                {
                    FMRead(world, str);
                }
                if (Regex.IsMatch(str, @"\bmultiread.*?\(.*?\)"))
                {
                    FMTest(ref allTests, ref numTests, world, str);
                }
                if (Regex.IsMatch(str, @"\bdrawdebug.*?\(.*?\)"))
                {
                    world.debug = true;
                }
            }

            Log log = new Log(@"binhl.log"); 

            if (allTests)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All Test - OK");
                Console.ForegroundColor = ConsoleColor.White;
                log.Write(world.worldName + " - OK");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("All Test - ERROR");
                Console.ForegroundColor = ConsoleColor.White;
                log.Write(world.worldName + " - ERROR");
            }
            /*
            string fileName = args.Length < 1 ? "TRIG_D" : args[0];

            var node = new Node(fileName + ".binhl");

            var world = new World(node);

            var r = new Render(world);
          
            Console.WriteLine("O0={0}", world.GetPortValue("O0"));

            r.GetSingeLayeImage().Save("I.png");
            */
        }

        private static void FMTest(ref bool allTests, ref int numTests, World world, string str)
        {
            throw new NotImplementedException();
        }

        private static void FMRead(World world, string str)
        {
            throw new NotImplementedException();
        }

        private static bool FCheckIo(bool allTests, World world)
        {
            var ports = world.inPorts.Union(world.outPorts).ToList();
            bool uport = CheckUnionPorts(ports);
            if (uport)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("IO - OK");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("IO - ERROR");
                allTests = false;
                Console.ForegroundColor = ConsoleColor.White;
            }
            return allTests;
        }

        private static bool CheckUnionPorts(System.Collections.Generic.List<IoPort> ports)
        {
            for (int i = 0; i < ports.Count; i++)
            {
                for (int j = 0; j < ports.Count; j++)
                {
                    if (i != j)
                    {
                        if (ports[i].X == ports[j].X && ports[i].Y == ports[j].Y)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static bool FTestStruct(World world,bool  altest)
        {
            bool tstruct = world.TestStruct();
            if (tstruct)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Struct - OK");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Struct - ERROR");
                altest = false;
                Console.ForegroundColor = ConsoleColor.White;
            }
            return altest;
        }

        private static void FRead(World world, string str)
        {
            string port = Regex.Match(str, @"\(.*?\)").Value.Replace("(", "").Replace(")", "").Trim();
            Console.WriteLine("{0}={1}", port, world.GetPortValue(port));
        }

        private static void FTest(ref bool allTests, ref int numTests, World world, string str)
        {
            numTests++;
            string[] fargs = Regex.Match(str, @"\(.*?\)").Value.Replace("(", "").Replace(")", "").Trim().Split(',');
            string port = fargs[0].Trim();
            bool value = Convert.ToInt32(fargs[1].Trim()) == 1;
            if (value == world.GetPortValue(port))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Test port:{0} - OK", port);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Test port:{0} - ERROR", port);
                Console.ForegroundColor = ConsoleColor.White;
                allTests = false;
            }
        }

        private static void FSet(World world, string str)
        {
            string[] fargs = Regex.Match(str, @"\(.*?\)").Value.Replace("(", "").Replace(")", "").Trim().Split(',');
            string port = fargs[0].Trim();
            bool value = Convert.ToInt32(fargs[1].Trim()) == 1;
            world.SetPortValue(port, value);
        }

        private static void FWait(World world, string str)
        {
            int ticks = Convert.ToInt32(Regex.Match(str, @"\(.*?\)").Value.Replace("(", "").Replace(")", "").Trim());
            for (int i = 0; i < ticks; i++)
            {
                world.Tick();
            }
        }

        private static World FLoad(string str)
        {
            World world;
            string fName = Regex.Match(str, @"\(.*?\)").Value.Replace("(", "").Replace(")", "").Trim();
            var node = new Node(fName + ".binhl");
            world = new World(node,fName);
            return world;
        }
    }
}