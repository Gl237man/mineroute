using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinhlEmul.WorldObjects
{
    class Cloth : WorldObject
    {
        public Cloth(int X, int Y, int Z, World W)
            : base(X, Y, Z,W)
        {
            
        }

        public override bool IsActivated
        {
            get {
                //Проверка активации проводом
                if (GetObject(Direction.backword).GetType() == typeof(RedstoneWire))
                {
                    if (GetObject(Direction.backword).GetObject(Direction.left).GetType() != typeof(RedstoneWire))
                    {
                        if (GetObject(Direction.backword).GetObject(Direction.right).GetType() != typeof(RedstoneWire))
                        {
                            if (GetObject(Direction.backword).RedValue > 0) return true;
                        }
                    }
                }

                if (GetObject(Direction.forward).GetType() == typeof(RedstoneWire))
                {
                    if (GetObject(Direction.forward).GetObject(Direction.left).GetType() != typeof(RedstoneWire))
                    {
                        if (GetObject(Direction.forward).GetObject(Direction.right).GetType() != typeof(RedstoneWire))
                        {
                            if (GetObject(Direction.forward).RedValue > 0) return true;
                        }
                    }
                }

                if (GetObject(Direction.left).GetType() == typeof(RedstoneWire))
                {
                    if (GetObject(Direction.left).GetObject(Direction.forward).GetType() != typeof(RedstoneWire))
                    {
                        if (GetObject(Direction.left).GetObject(Direction.backword).GetType() != typeof(RedstoneWire))
                        {
                            if (GetObject(Direction.left).RedValue > 0) return true;
                        }
                    }
                }

                if (GetObject(Direction.right).GetType() == typeof(RedstoneWire))
                {
                    if (GetObject(Direction.right).GetObject(Direction.forward).GetType() != typeof(RedstoneWire))
                    {
                        if (GetObject(Direction.right).GetObject(Direction.backword).GetType() != typeof(RedstoneWire))
                        {
                            if (GetObject(Direction.right).RedValue > 0) return true;
                        }
                    }
                }
                //Проверка активации повторителем
                if (GetObject(Direction.backword).GetType() == typeof(RedstoneRepiter))
                {
                    if (((RedstoneRepiter)(GetObject(Direction.backword))).Direct == Direction.forward)
                    {
                        if (((RedstoneRepiter)(GetObject(Direction.backword))).RedValue>15)
                        {
                            return true;
                        }
                    }
                }

                if (GetObject(Direction.forward).GetType() == typeof(RedstoneRepiter))
                {
                    if (((RedstoneRepiter)(GetObject(Direction.forward))).Direct == Direction.backword)
                    {
                        if (((RedstoneRepiter)(GetObject(Direction.forward))).RedValue > 15)
                        {
                            return true;
                        }
                    }
                }

                if (GetObject(Direction.left).GetType() == typeof(RedstoneRepiter))
                {
                    if (((RedstoneRepiter)(GetObject(Direction.left))).Direct == Direction.backword)
                    {
                        if (((RedstoneRepiter)(GetObject(Direction.left))).RedValue > 15)
                        {
                            return true;
                        }
                    }
                }

                if (GetObject(Direction.right).GetType() == typeof(RedstoneRepiter))
                {
                    if (((RedstoneRepiter)(GetObject(Direction.right))).Direct == Direction.left)
                    {
                        if (((RedstoneRepiter)(GetObject(Direction.right))).RedValue > 15)
                        {
                            return true;
                        }
                    }
                }
                //Проверка активации факелом
                if (GetObject(Direction.down).GetType() == typeof(RedstoneTorch))
                {
                    if (((RedstoneRepiter)(GetObject(Direction.down))).RedValue > 15)
                        {
                            return true;
                        }
                    
                }


                //Проверка активации проводом сверху
                if (GetObject(Direction.up).GetType() == typeof(RedstoneWire))
                {
                    if (GetObject(Direction.up).RedValue > 0) return true;
                }

                return false;
            }
        }

        public override void Tick()
        {
            

        }
    }
}
