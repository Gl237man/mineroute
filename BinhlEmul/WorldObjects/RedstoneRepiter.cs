namespace BinhlEmul.WorldObjects
{
    internal class RedstoneRepiter : WorldObject
    {
        public int Delay;
        public Direction Direct;
        public int TimeToStop;
        public bool Act;
        public bool[] ActArch;

        public RedstoneRepiter(int x, int y, int z, Direction dir, int delay, World world) : base(x, y, z, world)
        {
            Direct = dir;
            Delay = delay;
            TimeToStop = 0;
            Act = false;
            ActArch = new bool[Delay];
        }

        public override void Tick()
        {
            Act = false;

            if (TimeToStop > 0) TimeToStop--;

            if (GetObject(Direction.Backword).IsActivated && Direct == Direction.Forward)
            {
                Act = true;
            }
            if (GetObject(Direction.Forward).IsActivated && Direct == Direction.Backword)
            {
                Act = true;
            }
            if (GetObject(Direction.Left).IsActivated && Direct == Direction.Right)
            {
                Act = true;
            }
            if (GetObject(Direction.Right).IsActivated && Direct == Direction.Left)
            {
                Act = true;
            }

            for (int i = 1; i < Delay; i++)
            {
                ActArch[i - 1] = ActArch[i];
            }
            ActArch[Delay - 1] = Act;

            if (ActArch[0]) TimeToStop = Delay;

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