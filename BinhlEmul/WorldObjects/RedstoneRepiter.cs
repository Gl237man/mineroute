namespace BinhlEmul.WorldObjects
{
    internal class RedstoneRepiter : WorldObject
    {
        public int Delay;
        public Direction Direct;
        public int TimeToStop;

        public RedstoneRepiter(int x, int y, int z, Direction dir, int delay, World world) : base(x, y, z, world)
        {
            Direct = dir;
            Delay = delay;
            TimeToStop = 0;
        }

        public override void Tick()
        {
            if (TimeToStop > 0) TimeToStop--;

            if (GetObject(Direction.Backword).IsActivated && Direct == Direction.Forward)
            {
                TimeToStop = Delay;
            }
            if (GetObject(Direction.Forward).IsActivated && Direct == Direction.Backword)
            {
                TimeToStop = Delay;
            }
            if (GetObject(Direction.Left).IsActivated && Direct == Direction.Right)
            {
                TimeToStop = Delay;
            }
            if (GetObject(Direction.Right).IsActivated && Direct == Direction.Left)
            {
                TimeToStop = Delay;
            }

            if (TimeToStop > 0)
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