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
        public override void Tick()
        {
            throw new NotImplementedException();
        }
    }
}
