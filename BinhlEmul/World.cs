using System.Collections.Generic;
using System.Linq;
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
            for (var x = 0; x < node.SizeX; x++)
            {
                for (var y = 0; y < node.SizeY; y++)
                {
                    for (var z = 0; z < node.SizeZ; z++)
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
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneTorch(x, y, z, WorldObjects.Direction.forward,this);
                                break;
                            case "*":
                                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneTorch(x, y, z, WorldObjects.Direction.down,this);
                                break;
                        }
                    }
                }
            }

            //Загрузка портов
            InPorts = new List<IOPort>();
            OutPorts = new List<IOPort>();

            foreach (var port in node.InPorts)
            {
                var x = port.PosX;
                var y = WSY - port.PosY - 1;
                var z = 0;
                for (var j = 0; j < WSZ; j++)
                {
                    if (ObjectMatrix[x, y, z].GetType() == typeof(WorldObjects.Cloth))
                    {
                        z = j;
                    }
                }
                var p = new IOPort {x = x, y = y, z = z, Name = port.Name, value = false};
                InPorts.Add(p);
                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneWire(x, y, z, this);
                ((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).Blocked = true;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).RedValue = 15;
                //((WorldObjects.RedstoneWire)ObjectMatrix[x, y, z]).IsActivated = true;
            }

            foreach (var port in node.OutPorts)
            {
                var x = port.PosX;
                var y = WSY - port.PosY - 1;
                var z = 0;
                for (var j = 0; j < WSZ; j++)
                {
                    if (ObjectMatrix[x, y, z].GetType() == typeof(WorldObjects.Cloth))
                    {
                        z = j;
                    }
                }
                var p = new IOPort {x = x, y = y, z = z, Name = port.Name, value = false};
                OutPorts.Add(p);
                ObjectMatrix[x, y, z] = new WorldObjects.RedstoneWire(x, y, z, this);
            }

        }

        public bool GetPortValue(string portName)
        {
            return (from port in OutPorts where port.Name == portName select port.value).FirstOrDefault();
        }

        public void SetPortValue(string portName, bool value)
        {
            for (var i = 0; i < InPorts.Count; i++)
            {
                if (InPorts[i].Name == portName)
                {
                    InPorts[i].value = value;
                    if (value == true)
                    {
                        ObjectMatrix[InPorts[i].x, InPorts[i].y, InPorts[i].z].RedValue = 15;
                        ObjectMatrix[InPorts[i].x, InPorts[i].y, InPorts[i].z].IsActivated = true;

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
                for (var x = 0; x < WSX; x++)
                {
                    for (var y = 0; y < WSY; y++)
                    {
                        for (var z = 0; z < WSZ; z++)
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
            for (var x = 0; x < WSX; x++)
            {
                for (var y = 0; y < WSY; y++)
                {
                    for (var z = 0; z < WSZ; z++)
                    {
                        if (ObjectMatrix[x, y, z].GetType() != typeof(WorldObjects.RedstoneWire))
                        {
                            ObjectMatrix[x, y, z].Tick();
                        }
                    }
                }
            }
            //Обновление состояния портов
            for (var i = 0; i < OutPorts.Count; i++)
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
