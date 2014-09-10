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
        public List<IOPort> InPorts;
        public List<IOPort> OutPorts;

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

            //Загрузка портов
            InPorts = new List<IOPort>();
            OutPorts = new List<IOPort>();

            for (int i = 0; i < node.InPorts.Length; i++)
            {
                int x = node.InPorts[i].PosX;
                int y = WSY - node.InPorts[i].PosY - 1;
                int z = 0;
                for (int j = 0; j < WSZ; j++)
                {
                    if (ObjectMatrix[x, y, z].GetType() == typeof(WorldObjects.Cloth))
                    {
                        z = j;
                    }
                }
                IOPort p = new IOPort();
                p.x = x;
                p.y = y;
                p.z = z;
                p.Name = node.InPorts[i].Name;
                p.value = false;
                InPorts.Add(p);
                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneWire(x, y, z, this);
                ((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).Blocked = true;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).RedValue = 15;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).IsActivated = true;
            }

            for (int i = 0; i < node.OutPorts.Length; i++)
            {
                int x = node.OutPorts[i].PosX;
                int y = WSY - node.OutPorts[i].PosY - 1;
                int z = 0;
                for (int j = 0; j < WSZ; j++)
                {
                    if (ObjectMatrix[x, y, z].GetType() == typeof(WorldObjects.Cloth))
                    {
                        z = j;
                    }
                }
                IOPort p = new IOPort();
                p.x = x;
                p.y = y;
                p.z = z;
                p.Name = node.OutPorts[i].Name;
                p.value = false;
                OutPorts.Add(p);
                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneWire(x, y, z, this);
            }

        }

        public bool GetPortValue(string portName)
        {
            for (int i = 0; i < OutPorts.Count; i++)
            {
                if (OutPorts[i].Name == portName)
                {
                    return OutPorts[i].value;
                }
            }
            return false;
        }

        public void SetPortValue(string portName, bool value)
        {
            for (int i = 0; i < InPorts.Count; i++)
            {
                if (InPorts[i].Name == portName)
                {
                    InPorts[i].value = value;
                    if (value == true)
                    {
                        ((WorldObjects.RedstoneWire)ObjectMatrix[InPorts[i].x, InPorts[i].y, InPorts[i].z]).RedValue = 15;
                        ((WorldObjects.RedstoneWire)ObjectMatrix[InPorts[i].x, InPorts[i].y, InPorts[i].z]).IsActivated = true;

                    }
                    else
                    {
                        ((WorldObjects.RedstoneWire)ObjectMatrix[InPorts[i].x, InPorts[i].y, InPorts[i].z]).RedValue = 0;
                        ((WorldObjects.RedstoneWire)ObjectMatrix[InPorts[i].x, InPorts[i].y, InPorts[i].z]).IsActivated = false;
                    }
                }
            }
        }

        public void Tick()
        {
            //Оброботка тика проводов
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
            //Оброботка тика блоков
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
            //Обновление состояния портов
            for (int i = 0; i < OutPorts.Count; i++)
            {
                if (ObjectMatrix[OutPorts[i].x, OutPorts[i].y, OutPorts[i].z].RedValue > 0)
                {
                    OutPorts[i].value = true;
                }
                else
                {
                    OutPorts[i].value = false;
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
