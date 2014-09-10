using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteUtils;

namespace BinhlEmul
{
    public class World
    {
        public WorldObjects.WorldObject[,,] ObjectMatrix;
        public int WSX;
        public int WSY;
        public int WSZ;
        public bool NotFullTick;

        public World (Node node)
        {
            ObjectMatrix = new WorldObjects.WorldObject[node.SizeX, node.SizeY, node.SizeZ];
            WSX = node.SizeX;
            WSY = node.SizeY;
            WSZ = node.SizeZ;
            for (int x = 0; x < node.SizeX; x++)
            {
                for (int y = 0; y < node.SizeY; y++)
                {
                    for (int z = 0; z < node.SizeZ; z++)
                    {
                        switch (node.DataMatrix[x, WSY - y - 1, z])
                        {
                            case "k":
                                ObjectMatrix[x, y, z] = new WorldObjects.Cloth(x, y, z,this);
                                break;
                            case "W":
                                ObjectMatrix[x, y, z] = new WorldObjects.Cloth(x, y, z,this);
                                break;
                            case "w":
                                ObjectMatrix[x, y, z] = new WorldObjects.Cloth(x, y, z,this);
                                break;
                            case "S":
                                ObjectMatrix[x, y, z] = new WorldObjects.Cloth(x, y, z,this);
                                break;
                            case "0":
                                ObjectMatrix[x, y, z] = new WorldObjects.Air(x, y, z,this);
                                break;
                            case "#":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneWire(x, y, z,this);
                                break;
                            case "^":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneRepiter(x, y, z, WorldObjects.Direction.forward,1,this);
                                break;
                            case "<":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneRepiter(x, y, z, WorldObjects.Direction.left,1,this);
                                break;
                            case ">":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneRepiter(x, y, z, WorldObjects.Direction.right,1,this);
                                break;
                            case "v":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneRepiter(x, y, z, WorldObjects.Direction.backword,1,this);
                                break;
                            case "=":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneRepiter(x, y, z, WorldObjects.Direction.forward,4,this);
                                break;
                            case "_":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneTorch(x, y, z, WorldObjects.Direction.backword,this);
                                break;
                            case "-":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneTorch(x, y, z, WorldObjects.Direction.left,this);
                                break;
                            case "*":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneTorch(x, y, z, WorldObjects.Direction.down,this);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public void Tick()
        {
            NotFullTick = true;
            while (NotFullTick)
            {
                NotFullTick = false;
                for (int x = 0; x < WSX; x++)
                {
                    for (int y = 0; y < WSY; y++)
                    {
                        for (int z = 0; z < WSZ; z++)
                        {
                            if (ObjectMatrix[x, y, z].GetType() == typeof(WorldObjects.RedstoneWire))
                            {
                                ObjectMatrix[x, y, z].Tick();
                            }
                        }
                    }
                }

            }

            for (int x = 0; x < WSX; x++)
            {
                for (int y = 0; y < WSY; y++)
                {
                    for (int z = 0; z < WSZ; z++)
                    {
                        if (ObjectMatrix[x, y, z].GetType() != typeof(WorldObjects.RedstoneWire))
                        {
                            ObjectMatrix[x, y, z].Tick();
                        }
                    }
                }
            }
        }

        public WorldObjects.WorldObject getObject(int Xcoord, int Ycoord, int Zcoord)
        {
            if (Xcoord < 0 || Xcoord >= WSX) return new WorldObjects.Air(Xcoord, Ycoord, Zcoord, this);
            if (Ycoord < 0 || Ycoord >= WSY) return new WorldObjects.Air(Xcoord, Ycoord, Zcoord, this);
            if (Zcoord < 0 || Zcoord >= WSZ) return new WorldObjects.Air(Xcoord, Ycoord, Zcoord, this);

            return ObjectMatrix[Xcoord, Ycoord, Zcoord];
        }
    }
}
