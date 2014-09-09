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
        public RedstoneRepiter(int X, int Y, int Z, Direction dir,int Delay,World W) : base(X, Y, Z,W)
        {
            Direct = dir;
            this.Delay = Delay;
        }
        public override void Tick()
        {
            throw new NotImplementedException();
        }
    }
    
}
