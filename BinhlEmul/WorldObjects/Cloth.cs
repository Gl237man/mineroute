namespace BinhlEmul.WorldObjects
{
    internal class Cloth : WorldObject
    {
        public Cloth(int x, int y, int z, World world)
            : base(x, y, z, world)
        {
        }

        public override bool testState()
        {
            return true;
        }

        public override bool IsActivated
        {
            get
            {
                //Проверка активации проводом
                if (GetObject(Direction.Backword).GetType() == typeof (RedstoneWire) &&
                    (GetObject(Direction.Backword).GetObject(Direction.Left).GetType() != typeof (RedstoneWire) &&
                     (GetObject(Direction.Backword).GetObject(Direction.Right).GetType() != typeof (RedstoneWire) &&
                      GetObject(Direction.Backword).RedValue > 0))) return true;

                if (GetObject(Direction.Forward).GetType() == typeof (RedstoneWire) &&
                    (GetObject(Direction.Forward).GetObject(Direction.Left).GetType() != typeof (RedstoneWire) &&
                     (GetObject(Direction.Forward).GetObject(Direction.Right).GetType() != typeof (RedstoneWire) &&
                      GetObject(Direction.Forward).RedValue > 0))) return true;

                if (GetObject(Direction.Left).GetType() == typeof (RedstoneWire) &&
                    (GetObject(Direction.Left).GetObject(Direction.Forward).GetType() != typeof (RedstoneWire) &&
                     (GetObject(Direction.Left).GetObject(Direction.Backword).GetType() != typeof (RedstoneWire) &&
                      GetObject(Direction.Left).RedValue > 0))) return true;

                if (GetObject(Direction.Right).GetType() == typeof (RedstoneWire) &&
                    GetObject(Direction.Right).GetObject(Direction.Forward).GetType() != typeof (RedstoneWire) &&
                    GetObject(Direction.Right).GetObject(Direction.Backword).GetType() != typeof (RedstoneWire) &&
                    GetObject(Direction.Right).RedValue > 0) return true;

                //Проверка активации повторителем
                if (GetObject(Direction.Backword).GetType() == typeof (RedstoneRepiter) &&
                    (((RedstoneRepiter) (GetObject(Direction.Backword))).Direct == Direction.Forward &&
                     GetObject(Direction.Backword).RedValue > 15)) return true;

                if (GetObject(Direction.Forward).GetType() == typeof (RedstoneRepiter) &&
                    (((RedstoneRepiter) (GetObject(Direction.Forward))).Direct == Direction.Backword &&
                     GetObject(Direction.Forward).RedValue > 15)) return true;

                if (GetObject(Direction.Left).GetType() == typeof (RedstoneRepiter) &&
                    (((RedstoneRepiter) (GetObject(Direction.Left))).Direct == Direction.Right &&
                     GetObject(Direction.Left).RedValue > 15)) return true;

                if (GetObject(Direction.Right).GetType() == typeof (RedstoneRepiter) &&
                    (((RedstoneRepiter) (GetObject(Direction.Right))).Direct == Direction.Left &&
                     GetObject(Direction.Right).RedValue > 15)) return true;

                //Проверка активации факелом
                if (GetObject(Direction.Down).GetType() == typeof (RedstoneTorch) &&
                    GetObject(Direction.Down).RedValue > 15) return true;


                //Проверка активации проводом сверху
                if (GetObject(Direction.Up).GetType() == typeof (RedstoneWire) && GetObject(Direction.Up).RedValue > 0)
                    return true;

                return false;
            }
        }

        public override bool WTick()
        {
            return false;
        }

        public override void Tick()
        {
        }
    }
}