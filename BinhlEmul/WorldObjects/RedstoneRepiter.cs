using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinhlEmul.WorldObjects
{
    class RedstoneRepiter: WorldObject
    {
        public Direction Direct;
        public int Delay;
        public int ost;
        public RedstoneRepiter(int X, int Y, int Z, Direction dir,int Delay,World W) : base(X, Y, Z,W)
        {
            Direct = dir;
            this.Delay = Delay;
            ost = 0;
        }
        public override void Tick()
        {   
            if (ost>0) ost--;

            if (GetObject(Direction.backword).IsActivated && Direct == Direction.forward)
            {
                ost = Delay;
            }
            if (GetObject(Direction.forward).IsActivated && Direct == Direction.backword)
            {
                ost = Delay;
            }
            if (GetObject(Direction.left).IsActivated && Direct == Direction.right)
            {
                ost = Delay;
            }
            if (GetObject(Direction.right).IsActivated && Direct == Direction.left)
            {
                ost = Delay;
            }

            if (ost > 0)
            {
                IsActivated = true;
                RedValue = 16;
            }
            else
            {
                IsActivated = false;
                RedValue = 0;
            }

        }
    }
    
}
