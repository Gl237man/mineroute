namespace BinhlEmul.WorldObjects
{
    internal class RedstoneTorch : WorldObject
    {
        public Direction PlaceBlockDirect;
        //public int Delay;
        public RedstoneTorch(int x, int y, int z, Direction dir, World world)
            : base(x, y, z, world)
        {
            PlaceBlockDirect = dir;
        }

        public override void Tick()
        {
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