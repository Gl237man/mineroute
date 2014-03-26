using System;
using System.Collections.Generic;
using RouteUtils;

namespace Binhl2JsWE
{
    class Program
    {
        static void Main(string[] args)
        {
            string FileName = "";
            if (args.Length < 1)
            {
                FileName = "tm";
            }
            else
            {
                FileName = args[0];
            }


            Node N = new Node(FileName + ".binhl");
            string GenScript = "";
            GenScript += @"// $Id$" + "\r\n";


            GenScript += @"importPackage(Packages.java.io);" + "\r\n";
            GenScript += @"importPackage(Packages.java.awt);" + "\r\n";
            GenScript += @"importPackage(Packages.com.sk89q.worldedit);" + "\r\n";
            GenScript += @"importPackage(Packages.com.sk89q.worldedit.blocks);" + "\r\n";
            GenScript += @"var origin = player.getBlockOn();" + "\r\n";
            GenScript += @"var sess = context.remember();" + "\r\n";

            int tvn = 0;
            int strn = 0;
            GenScript += @"function v" + tvn.ToString() + " (){" + "\r\n";
            for (int i = 0; i < N.SizeX; i++)
            {
                for (int j = 0; j < N.SizeY; j++)
                {
                    
                    for (int k = 0; k < N.SizeZ; k++)
                    {
                        if (N.DataMatrix[i, j, k] != "0")
                        {
                            GenScript += GetCoordSring(i, j, k);
                            GenScript += GetBlockStr(N.DataMatrix[i, j, k]);
                            strn++;
                            if (strn > 200)
                            {
                                GenScript += @"};" + "\r\n";
                                GenScript += @"v" + tvn.ToString() + "();" + "\r\n";
                                tvn++;
                                Console.Write(tvn);
                                GenScript += @"function v" + tvn.ToString() + " (){" + "\r\n";
                                strn = 0;
                            }
                        }
                        //DataMatrix[i, j, k] = "0";
                    }
                }
                
            }

            GenScript += @"};" + "\r\n";
            GenScript += @"v" + tvn.ToString() + "();" + "\r\n";

            System.IO.File.WriteAllText(FileName+".js", GenScript);
        }

        private static string GetBlockStr(string BckS)
        {

            string ost = "";
            switch (BckS)
            {
                case "k":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 4));" + "\r\n";
                    break;
                case "W":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 15));" + "\r\n";
                    break;
                case "w":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 0));" + "\r\n";
                    break;
                case "S":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 3));" + "\r\n";
                    break;
                case "0":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.AIR, 0));" + "\r\n";
                    break;
                case "#":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_WIRE, 0));" + "\r\n";
                    break;
                case "^":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 1));" + "\r\n";
                    break;
                case "<":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 0));" + "\r\n";
                    break;
                case ">":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 2));" + "\r\n";
                    break;
                case "v":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 3));" + "\r\n";
                    break;
                case "=":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 13));" + "\r\n";
                    break;
                case "_":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_TORCH_ON, 1));" + "\r\n";
                    break;
                case "-":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_TORCH_ON, 2));" + "\r\n";
                    break;
                case "*":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_TORCH_ON, 5));" + "\r\n";
                    break;
                default:
                    break;
            }

            return ost;
        }
        private static string GetCoordSring(int x, int y, int z)
        {
            string str = "var pt = origin.add(" + y.ToString() + "," + z.ToString() + ", " + x.ToString() + ");" + "\r\n";

            return str;
        }
    }
}
