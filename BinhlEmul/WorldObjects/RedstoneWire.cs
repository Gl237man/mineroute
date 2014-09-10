using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinhlEmul.WorldObjects
{
    class RedstoneWire: WorldObject
    {
        

        public RedstoneWire(int X, int Y, int Z,World W) : base(X, Y, Z,W)
        {
            
        }

        public override void Tick()
        {
            int OldRedValue = RedValue;
            int[] maxN = new int[8];
            

            maxN[0] = GetObject(Direction.backword).RedValue;
            maxN[1] = GetObject(Direction.forward).RedValue;
            maxN[2] = GetObject(Direction.left).RedValue;
            maxN[3] = GetObject(Direction.right).RedValue;

            if (GetObject(Direction.backword).GetType() == typeof (RedstoneRepiter))
            {
                if (((RedstoneRepiter)(GetObject(Direction.backword))).Direct != Direction.forward)
                {
                    maxN[0] = 0;
                }
            }
            if (GetObject(Direction.forward).GetType() == typeof(RedstoneRepiter))
            {
                if (((RedstoneRepiter)(GetObject(Direction.forward))).Direct != Direction.backword)
                {
                    maxN[1] = 0;
                }
            }
            if (GetObject(Direction.left).GetType() == typeof(RedstoneRepiter))
            {
                if (((RedstoneRepiter)(GetObject(Direction.left))).Direct != Direction.right)
                {
                    maxN[2] = 0;
                }
            }
            if (GetObject(Direction.right).GetType() == typeof(RedstoneRepiter))
            {
                if (((RedstoneRepiter)(GetObject(Direction.right))).Direct != Direction.left)
                {
                    maxN[3] = 0;
                }
            }

            if (GetObject(Direction.backword).GetType() == typeof(Air))
            {
                maxN[4] = GetObject(Direction.backword).GetObject(Direction.down).RedValue;
            }
            else
            {
                maxN[4] = 0;
            }

            if (GetObject(Direction.forward).GetType() == typeof(Air))
            {
                maxN[5] = GetObject(Direction.forward).GetObject(Direction.down).RedValue;
            }
            else
            {
                maxN[5] = 0;
            }

            if (GetObject(Direction.left).GetType() == typeof(Air))
            {
                maxN[6] = GetObject(Direction.left).GetObject(Direction.down).RedValue;
            }
            else
            {
                maxN[6] = 0;
            }

            if (GetObject(Direction.right).GetType() == typeof(Air))
            {
                maxN[7] = GetObject(Direction.right).GetObject(Direction.down).RedValue;
            }
            else
            {
                maxN[7] = 0;
            }

            int max = 0;
            for (int i = 0; i < maxN.Length; i++)
            {
                if (maxN[i] > max) max = maxN[i];
            }

            RedValue = max - 1;
            if (RedValue > 0)
                IsActivated = true;
            else
                IsActivated = false;

            if (OldRedValue != RedValue) InWorld.NotFullTick = true;
        }
    }
}
