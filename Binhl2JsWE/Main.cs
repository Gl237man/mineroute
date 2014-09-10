using System;
using System.IO;
using System.Text;
using RouteUtils;

namespace Binhl2JsWE
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string fileName = args.Length < 1 ? "test" : args[0];


            var node = new Node(fileName + ".binhl");
            string genScript = "";
            genScript += @"// $Id$" + "\r\n";


            genScript += @"importPackage(Packages.java.io);" + "\r\n";
            genScript += @"importPackage(Packages.java.awt);" + "\r\n";
            genScript += @"importPackage(Packages.com.sk89q.worldedit);" + "\r\n";
            genScript += @"importPackage(Packages.com.sk89q.worldedit.blocks);" + "\r\n";
            genScript += @"var origin = player.getBlockOn();" + "\r\n";
            genScript += @"var sess = context.remember();" + "\r\n";

            int tvn = 0;
            int strn = 0;

            var sb = new StringBuilder();

            sb.Append(@"function v" + tvn + " (){" + "\r\n");
            for (int i = 0; i < node.SizeX; i++)
            {
                for (int j = 0; j < node.SizeY; j++)
                {
                    for (int k = 0; k < node.SizeZ; k++)
                    {
                        if (node.DataMatrix[i, j, k] == "0") continue;
                        sb.Append(GetCoordSring(i, j, k));
                        sb.Append(GetBlockStr(node.DataMatrix[i, j, k]));
                        strn++;
                        if (strn <= 200) continue;
                        sb.Append(@"};" + "\r\n");
                        sb.Append(@"v" + tvn + "();" + "\r\n");
                        tvn++;
                        Console.Write(tvn);
                        sb.Append(@"function v" + tvn + " (){" + "\r\n");
                        strn = 0;
                        //DataMatrix[i, j, k] = "0";
                    }
                }
            }

            sb.Append(@"};" + "\r\n");
            sb.Append(@"v" + tvn + "();" + "\r\n");

            genScript += sb.ToString();

            File.WriteAllText(fileName + ".js", genScript);
        }

        private static string GetBlockStr(string bckS)
        {
            string ost = "";
            switch (bckS)
            {
                case "k":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 4));\r\n";
                    break;
                case "W":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 15));\r\n";
                    break;
                case "w":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 0));\r\n";
                    break;
                case "S":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.CLOTH, 3));\r\n";
                    break;
                case "0":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.AIR, 0));\r\n";
                    break;
                case "#":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_WIRE, 0));\r\n";
                    break;
                case "^":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 1));\r\n";
                    break;
                case "<":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 0));\r\n";
                    break;
                case ">":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 2));\r\n";
                    break;
                case "v":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 3));\r\n";
                    break;
                case "=":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_REPEATER_OFF, 13));\r\n";
                    break;
                case "_":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_TORCH_ON, 1));\r\n";
                    break;
                case "-":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_TORCH_ON, 2));\r\n";
                    break;
                case "*":
                    ost = "sess.setBlock(pt, new BaseBlock(BlockID.REDSTONE_TORCH_ON, 5));\r\n";
                    break;
            }

            return ost;
        }

        private static string GetCoordSring(int x, int y, int z)
        {
            string str = string.Format("var pt = origin.add({0},{1},{2});\r\n", y, z, x);
            return str;
        }
    }
}