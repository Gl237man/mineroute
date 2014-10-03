namespace BinhlEmul.WorldObjects
{
    public abstract class WorldObject
    {
        public World InWorld;
        public int RedValue;
        public int Xcoord;
        public int Ycoord;
        public int Zcoord;

        protected WorldObject(int x, int y, int z, World world)
        {
            Xcoord = x;
            Ycoord = y;
            Zcoord = z;
            InWorld = world;
        }

        public virtual bool IsActivated { get; set; }

        public abstract void Tick();
        public abstract bool testState();


/*
        public int GetRedValue(Direction direct)
        {
            return GetObject(direct).RedValue;
        }
*/

/*
        public bool GetRedActivated(Direction direct)
        {
            return GetObject(direct).IsActivated;
        }
*/

        public WorldObject GetObject(Direction direct)
        {
            switch (direct)
            {
                case Direction.Up:
                    return InWorld.GetObject(Xcoord, Ycoord, Zcoord + 1);
                case Direction.Down:
                    return InWorld.GetObject(Xcoord, Ycoord, Zcoord - 1);
                case Direction.Forward:
                    return InWorld.GetObject(Xcoord, Ycoord + 1, Zcoord);
                case Direction.Backword:
                    return InWorld.GetObject(Xcoord, Ycoord - 1, Zcoord);
                case Direction.Left:
                    return InWorld.GetObject(Xcoord + 1, Ycoord, Zcoord);
                case Direction.Right:
                    return InWorld.GetObject(Xcoord - 1, Ycoord, Zcoord);
                default:
                    return null;
            }
        }
    }
}