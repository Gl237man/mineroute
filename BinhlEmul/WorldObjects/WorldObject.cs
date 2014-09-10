using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinhlEmul.WorldObjects
{
    public enum Direction
    {
        up,
        down,
        forward,
        backword,
        left,
        right

    }
    public abstract class WorldObject
    {
        public World InWorld;
        public int Xcoord;
        public int Ycoord;
        public int Zcoord;
        public virtual bool IsActivated{get;set;}
        public int RedValue;

        public abstract void Tick();
        

        public WorldObject(int X, int Y, int Z,World W)
        {
            Xcoord = X;
            Ycoord = Y;
            Zcoord = Z;
            InWorld = W;
        }

        public int GetRedValue(Direction direct)
        {
            return GetObject(direct).RedValue;
        }
        public bool GetRedActivated(Direction direct)
        {
            return GetObject(direct).IsActivated;
        }
        public WorldObject GetObject(Direction direct)
        {
            switch (direct)
            {
                case Direction.up:
                    return InWorld.getObject(Xcoord, Ycoord, Zcoord + 1);
                case Direction.down:
                    return InWorld.getObject(Xcoord, Ycoord, Zcoord - 1);
                case Direction.forward:
                    return InWorld.getObject(Xcoord, Ycoord + 1, Zcoord);
                case Direction.backword:
                    return InWorld.getObject(Xcoord, Ycoord - 1, Zcoord);
                case Direction.left:
                    return InWorld.getObject(Xcoord + 1, Ycoord, Zcoord);
                case Direction.right:
                    return InWorld.getObject(Xcoord - 1, Ycoord, Zcoord);
                default:
                    return null;
            }
        }
    }
}
