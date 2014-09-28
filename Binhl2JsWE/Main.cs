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
            genScript += "// $Id$\r\n";


            genScript += "importPackage(Packages.java.io);\r\n";
            genScript += "importPackage(Packages.java.awt);\r\n";
            genScript += "importPackage(Packages.com.sk89q.worldedit);\r\n";
            genScript += "importPackage(Packages.com.sk89q.worldedit.blocks);\r\n";
            genScript += "var origin = player.getBlockOn();\r\n";
            genScript += "var sess = context.remember();\r\n";

            int tempFunctionNum = 0;
            int stringNums = 0;

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat("function v{0} (){{\r\n", tempFunctionNum);
            for (int x = 0; x < node.SizeX; x++)
            {
                for (int y = 0; y < node.SizeY; y++)
                {
                    int iy = node.SizeY - y -1;
                    for (int z = 0; z < node.SizeZ; z++)
                    {
                        if (node.DataMatrix[x, y, z] == "0") continue;
                        stringBuilder.Append(GetCoordSring(x, iy, z));
                        stringBuilder.Append(GetBlockStr(node.DataMatrix[x, y, z]));
                        stringNums++;
                        if (stringNums <= 200) continue;
                        stringBuilder.Append("};\r\n");
                        stringBuilder.AppendFormat("v{0}();\r\n", tempFunctionNum);
                        tempFunctionNum++;
                        Console.Write(tempFunctionNum);
                        stringBuilder.AppendFormat("function v{0} (){{\r\n", tempFunctionNum);
                        stringNums = 0;
                    }
                }
            }

            stringBuilder.Append("};\r\n");
            stringBuilder.AppendFormat("v{0}();\r\n", tempFunctionNum);

            genScript += stringBuilder.ToString();

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