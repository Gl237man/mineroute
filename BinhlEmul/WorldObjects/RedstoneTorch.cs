namespace BinhlEmul.WorldObjects
{
    internal class RedstoneTorch : WorldObject
    {
        public Direction PlaceBlockDirect;
        public int ChangetCount;
        public bool OldValue;
        public bool BurntOut;
        public int BlockTime;
        public RedstoneTorch(int x, int y, int z, Direction dir, World world)
            : base(x, y, z, world)
        {
            PlaceBlockDirect = dir;
            ChangetCount = 0;
            OldValue = false;
            BurntOut = false;
            BlockTime = 0;
        }

        public override bool testState()
        {
            return GetObject(PlaceBlockDirect).GetType() == typeof(Cloth);
        }

        public override void Tick()
        {
            OldValue = IsActivated;
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
            if (OldValue != IsActivated)
                InWorld.TickWire();
        }
    }
}