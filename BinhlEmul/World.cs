using System.Collections.Generic;
using System.Linq;
using BinhlEmul.WorldObjects;
using RouteUtils;

namespace BinhlEmul
{
    public class World
    {
        public List<IoPort> InPorts;
        public bool NotFullTick;
        public WorldObject[,,] ObjectMatrix;
        public List<IoPort> OutPorts;
        public int WorldSizeX;
        public int WorldSizeY;
        public int WorldSizeZ;

        public World(Node node)
        {
            ObjectMatrix = new WorldObject[node.SizeX, node.SizeY, node.SizeZ];
            WorldSizeX = node.SizeX;
            WorldSizeY = node.SizeY;
            WorldSizeZ = node.SizeZ;
            for (int x = 0; x < node.SizeX; x++)
            {
                for (int y = 0; y < node.SizeY; y++)
                {
                    for (int z = 0; z < node.SizeZ; z++)
                    {
                        switch (node.DataMatrix[x, WorldSizeY - y - 1, z])
                        {
                            case "k":
                                ObjectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "W":
                                ObjectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "w":
                                ObjectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "S":
                                ObjectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "0":
                                ObjectMatrix[x, y, z] = new Air(x, y, z, this);
                                break;
                            case "#":
                                ObjectMatrix[x, y, z] = new RedstoneWire(x, y, z, this);
                                break;
                            case "^":
                                ObjectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Forward, 1, this);
                                break;
                            case "<":
                                ObjectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Left, 1, this);
                                break;
                            case ">":
                                ObjectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Right, 1, this);
                                break;
                            case "v":
                                ObjectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Backword, 1, this);
                                break;
                            case "=":
                                ObjectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Forward, 4, this);
                                break;
                            case "_":
                                ObjectMatrix[x, y, z] = new RedstoneTorch(x, y, z, Direction.Backword, this);
                                break;
                            case "-":
                                ObjectMatrix[x, y, z] = new RedstoneTorch(x, y, z, Direction.Forward, this);
                                break;
                            case "*":
                                ObjectMatrix[x, y, z] = new RedstoneTorch(x, y, z, Direction.Down, this);
                                break;
                        }
                    }
                }
            }

            //Загрузка портов
            InPorts = new List<IoPort>();
            OutPorts = new List<IoPort>();

            foreach (INPort port in node.InPorts)
            {
                int x = port.PosX;
                int y = WorldSizeY - port.PosY - 1;
                int z = 0;
                for (int j = 0; j < WorldSizeZ; j++)
                {
                    if (ObjectMatrix[x, y, z].GetType() == typeof (Cloth))
                    {
                        z = j;
                    }
                }
                var p = new IoPort {X = x, Y = y, Z = z, Name = port.Name, Value = false};
                InPorts.Add(p);
                ObjectMatrix[x, y, z] = new RedstoneWire(x, y, z, this);
                ((RedstoneWire) ObjectMatrix[x, y, z]).Blocked = true;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).RedValue = 15;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).IsActivated = true;
            }

            foreach (OUTPort port in node.OutPorts)
            {
                int x = port.PosX;
                int y = WorldSizeY - port.PosY - 1;
                int z = 0;
                for (int j = 0; j < WorldSizeZ; j++)
                {
                    if (ObjectMatrix[x, y, z].GetType() == typeof (Cloth))
                    {
                        z = j;
                    }
                }
                var p = new IoPort {X = x, Y = y, Z = z, Name = port.Name, Value = false};
                OutPorts.Add(p);
                ObjectMatrix[x, y, z] = new RedstoneWire(x, y, z, this);
            }
        }

        public bool GetPortValue(string portName)
        {
            return (from port in OutPorts where port.Name == portName select port.Value).FirstOrDefault();
        }

        public void SetPortValue(string portName, bool value)
        {
            foreach (IoPort port in InPorts.Where(port => port.Name == portName))
            {
                port.Value = value;
                if (value)
                {
                    ObjectMatrix[port.X, port.Y, port.Z].RedValue = 15;
                    ObjectMatrix[port.X, port.Y, port.Z].IsActivated = true;
                }
                else
                {
                    ObjectMatrix[port.X, port.Y, port.Z].RedValue = 0;
                    ObjectMatrix[port.X, port.Y, port.Z].IsActivated = false;
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
                for (int x = 0; x < WorldSizeX; x++)
                {
                    for (int y = 0; y < WorldSizeY; y++)
                    {
                        for (int z = 0; z < WorldSizeZ; z++)
                        {
                            if (ObjectMatrix[x, y, z].GetType() == typeof (RedstoneWire))
                            {
                                ObjectMatrix[x, y, z].Tick();
                            }
                        }
                    }
                }
            }
            //Оброботка тика блоков
            for (int x = 0; x < WorldSizeX; x++)
            {
                for (int y = 0; y < WorldSizeY; y++)
                {
                    for (int z = 0; z < WorldSizeZ; z++)
                    {
                        if (ObjectMatrix[x, y, z].GetType() != typeof (RedstoneWire))
                        {
                            ObjectMatrix[x, y, z].Tick();
                        }
                    }
                }
            }
            //Обновление состояния портов
            foreach (IoPort t in OutPorts)
            {
                t.Value = ObjectMatrix[t.X, t.Y, t.Z].RedValue > 0;
            }
        }

        public WorldObject GetObject(int xCoord, int yCoord, int zCoord)
        {
            if (xCoord < 0 || xCoord >= WorldSizeX) return new Air(xCoord, yCoord, zCoord, this);
            if (yCoord < 0 || yCoord >= WorldSizeY) return new Air(xCoord, yCoord, zCoord, this);
            if (zCoord < 0 || zCoord >= WorldSizeZ) return new Air(xCoord, yCoord, zCoord, this);

            return ObjectMatrix[xCoord, yCoord, zCoord];
        }
    }
}