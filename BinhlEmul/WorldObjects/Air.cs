namespace BinhlEmul.WorldObjects
{
    internal class Air : WorldObject
    {
        public Air(int x, int y, int z, World world) : base(x, y, z, world)
        {
        }

        public override bool testState()
        {
            return true;
        }

        public override void Tick()
        {
        }
    }
}