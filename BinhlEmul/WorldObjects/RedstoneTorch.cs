﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinhlEmul.WorldObjects
{
    class RedstoneTorch : WorldObject
    {
        public Direction PlaceBlockDirect;
        //public int Delay;
        public RedstoneTorch(int X, int Y, int Z, Direction dir, World W)
            : base(X, Y, Z, W)
        {
            PlaceBlockDirect = dir;
        }

        public override void Tick()
        {
            WorldObject w = GetObject(PlaceBlockDirect);
            if (!GetObject(PlaceBlockDirect).IsActivated)
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
