using System.Collections.Generic;
using System.Linq;
using BinhlEmul.WorldObjects;
using RouteUtils;

namespace BinhlEmul
{
    public class World
    {
        private readonly List<IoPort> _inPorts;
        private readonly WorldObject[,,] _objectMatrix;
        private readonly List<IoPort> _outPorts;
        public readonly int WorldSizeX;
        public readonly int WorldSizeY;
        public readonly int WorldSizeZ;
        public bool NotFullTick;

        public World()
        {
 
        }

        public World(Node node)
        {
            _objectMatrix = new WorldObject[node.SizeX, node.SizeY, node.SizeZ];
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
                                _objectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "W":
                                _objectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "w":
                                _objectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "S":
                                _objectMatrix[x, y, z] = new Cloth(x, y, z, this);
                                break;
                            case "0":
                                _objectMatrix[x, y, z] = new Air(x, y, z, this);
                                break;
                            case "#":
                                _objectMatrix[x, y, z] = new RedstoneWire(x, y, z, this);
                                break;
                            case "^":
                                _objectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Forward, 1, this);
                                break;
                            case "<":
                                _objectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Right, 1, this);
                                break;
                            case ">":
                                _objectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Left, 1, this);
                                break;
                            case "v":
                                _objectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Backword, 1, this);
                                break;
                            case "=":
                                _objectMatrix[x, y, z] = new RedstoneRepiter(x, y, z, Direction.Forward, 2, this);
                                break;
                            case "_":
                                _objectMatrix[x, y, z] = new RedstoneTorch(x, y, z, Direction.Backword, this);
                                break;
                            case "-":
                                _objectMatrix[x, y, z] = new RedstoneTorch(x, y, z, Direction.Forward, this);
                                break;
                            case "*":
                                _objectMatrix[x, y, z] = new RedstoneTorch(x, y, z, Direction.Down, this);
                                break;
                        }
                    }
                }
            }

            //Загрузка портов
            _inPorts = new List<IoPort>();
            _outPorts = new List<IoPort>();

            foreach (INPort port in node.InPorts)
            {
                int x = port.PosX;
                int y = WorldSizeY - port.PosY - 1;
                int z = 0;
                for (int j = 0; j < WorldSizeZ; j++)
                {
                    if (_objectMatrix[x, y, z].GetType() == typeof (Cloth))
                    {
                        z = j;
                    }
                }
                var p = new IoPort {X = x, Y = y, Z = z, Name = port.Name, Value = false};
                _inPorts.Add(p);
                _objectMatrix[x, y, z] = new RedstoneWire(x, y, z, this);
                ((RedstoneWire) _objectMatrix[x, y, z]).Blocked = true;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).RedValue = 15;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).IsActivated = true;
            }

            foreach (OutPort port in node.OutPorts)
            {
                int x = port.PosX;
                int y = WorldSizeY - port.PosY - 1;
                int z = 0;
                for (int j = 0; j < WorldSizeZ; j++)
                {
                    if (_objectMatrix[x, y, z].GetType() == typeof (Cloth))
                    {
                        z = j;
                    }
                }
                var p = new IoPort {X = x, Y = y, Z = z, Name = port.Name, Value = false};
                _outPorts.Add(p);
                _objectMatrix[x, y, z] = new RedstoneWire(x, y, z, this);
            }
        }

        public bool GetPortValue(string portName)
        {
            return (from port in _outPorts where port.Name == portName select port.Value).FirstOrDefault();
        }

        public void SetPortValue(string portName, bool value)
        {
            foreach (IoPort port in _inPorts.Where(port => port.Name == portName))
            {
                port.Value = value;
                if (value)
                {
                    _objectMatrix[port.X, port.Y, port.Z].RedValue = 15;
                    _objectMatrix[port.X, port.Y, port.Z].IsActivated = true;
                }
                else
                {
                    _objectMatrix[port.X, port.Y, port.Z].RedValue = 0;
                    _objectMatrix[port.X, port.Y, port.Z].IsActivated = false;
                }
            }
        }

        public void Tick()
        {
            //Оброботка тика проводов
            TickWire();
            //Оброботка тика блоков
            for (int x = 0; x < WorldSizeX; x++)
            {
                for (int y = 0; y < WorldSizeY; y++)
                {
                    for (int z = 0; z < WorldSizeZ; z++)
                    {
                        if (_objectMatrix[x, y, z].GetType() != typeof (RedstoneWire))
                        {
                            _objectMatrix[x, y, z].Tick();
                        }
                    }
                }
            }
            TickTorch();
            TickTorch();
            TickTorch();
            //Обновление состояния портов
            foreach (IoPort t in _outPorts)
            {
                t.Value = _objectMatrix[t.X, t.Y, t.Z].RedValue > 0;
            }
        }

        public void TickWire()
        {
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
                            if (_objectMatrix[x, y, z].GetType() == typeof(RedstoneWire))
                            {
                                _objectMatrix[x, y, z].Tick();
                            }
                        }
                    }
                }
            }
        }

        public void TickTorch()
        {
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
                            if (_objectMatrix[x, y, z].GetType() == typeof(RedstoneTorch))
                            {
                                _objectMatrix[x, y, z].Tick();
                            }
                        }
                    }
                }
            }
        }

        public WorldObject GetObject(int xCoord, int yCoord, int zCoord)
        {
            if (xCoord < 0 || xCoord >= WorldSizeX) return new Air(xCoord, yCoord, zCoord, this);
            if (yCoord < 0 || yCoord >= WorldSizeY) return new Air(xCoord, yCoord, zCoord, this);
            if (zCoord < 0 || zCoord >= WorldSizeZ) return new Air(xCoord, yCoord, zCoord, this);

            return _objectMatrix[xCoord, yCoord, zCoord];
        }
    }
}