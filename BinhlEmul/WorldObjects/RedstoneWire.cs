using System.Linq;

namespace BinhlEmul.WorldObjects
{
    internal class RedstoneWire : WorldObject
    {
        public bool Blocked;

        public RedstoneWire(int x, int y, int z, World world) : base(x, y, z, world)
        {
        }

        public override bool testState()
        {
            return GetObject(Direction.Down).GetType() == typeof(Cloth);
        }

        public override void Tick()
        {
            int oldRedValue = RedValue;
            var maxN = new int[12];


            maxN[0] = GetObject(Direction.Backword).RedValue;
            maxN[1] = GetObject(Direction.Forward).RedValue;
            maxN[2] = GetObject(Direction.Left).RedValue;
            maxN[3] = GetObject(Direction.Right).RedValue;

            // Проверка репитеров
            if (GetObject(Direction.Backword).GetType() == typeof (RedstoneRepiter))
                if (((RedstoneRepiter) (GetObject(Direction.Backword))).Direct != Direction.Forward)
                    maxN[0] = 0;

            if (GetObject(Direction.Forward).GetType() == typeof (RedstoneRepiter))
                if (((RedstoneRepiter) (GetObject(Direction.Forward))).Direct != Direction.Backword)
                    maxN[1] = 0;

            if (GetObject(Direction.Left).GetType() == typeof (RedstoneRepiter))
                if (((RedstoneRepiter) (GetObject(Direction.Left))).Direct != Direction.Right)
                    maxN[2] = 0;

            if (GetObject(Direction.Right).GetType() == typeof (RedstoneRepiter))
                if (((RedstoneRepiter) (GetObject(Direction.Right))).Direct != Direction.Left)
                    maxN[3] = 0;

            //Подьем сигнала
            if (GetObject(Direction.Backword).GetType() == typeof (Air))
            {
                if (GetObject(Direction.Backword).GetObject(Direction.Down).GetType() == typeof (RedstoneWire))
                    maxN[4] = GetObject(Direction.Backword).GetObject(Direction.Down).RedValue;
            }
            else
                maxN[4] = 0;


            if (GetObject(Direction.Forward).GetType() == typeof (Air))
            {
                if (GetObject(Direction.Forward).GetObject(Direction.Down).GetType() == typeof (RedstoneWire))
                    maxN[5] = GetObject(Direction.Forward).GetObject(Direction.Down).RedValue;
            }
            else
                maxN[5] = 0;

            if (GetObject(Direction.Left).GetType() == typeof (Air))
            {
                if (GetObject(Direction.Left).GetObject(Direction.Down).GetType() == typeof (RedstoneWire))
                    maxN[6] = GetObject(Direction.Left).GetObject(Direction.Down).RedValue;
            }
            else
                maxN[6] = 0;
            
            if (GetObject(Direction.Right).GetType() == typeof (Air))
            {
                if (GetObject(Direction.Right).GetObject(Direction.Down).GetType() == typeof (RedstoneWire))
                    maxN[7] = GetObject(Direction.Right).GetObject(Direction.Down).RedValue;
            }
            else
                maxN[7] = 0;

            //Спуск Сигнала
            if (GetObject(Direction.Up).GetType() == typeof (Air))
            {
                if (GetObject(Direction.Backword).GetType() == typeof (Cloth))
                {
                    if (GetObject(Direction.Backword).GetObject(Direction.Up).GetType() == typeof (RedstoneWire))
                        maxN[8] = GetObject(Direction.Backword).GetObject(Direction.Up).RedValue;
                }
                else
                    maxN[8] = 0;

                if (GetObject(Direction.Forward).GetType() == typeof (Cloth))
                {
                    if (GetObject(Direction.Forward).GetObject(Direction.Up).GetType() == typeof (RedstoneWire))
                        maxN[9] = GetObject(Direction.Forward).GetObject(Direction.Up).RedValue;
                }
                else
                    maxN[9] = 0;

                if (GetObject(Direction.Left).GetType() == typeof (Cloth))
                {
                    if (GetObject(Direction.Left).GetObject(Direction.Up).GetType() == typeof (RedstoneWire))
                        maxN[10] = GetObject(Direction.Left).GetObject(Direction.Up).RedValue;
                }
                else
                    maxN[10] = 0;


                if (GetObject(Direction.Right).GetType() == typeof (Cloth))
                {
                    if (GetObject(Direction.Right).GetObject(Direction.Up).GetType() == typeof (RedstoneWire))
                        maxN[11] = GetObject(Direction.Right).GetObject(Direction.Up).RedValue;
                }
                else
                    maxN[11] = 0;
            }

            int max = maxN.Max();

            if (Blocked) return;
            RedValue = max - 1;

            IsActivated = RedValue > 0;

            if (oldRedValue != RedValue) InWorld.NotFullTick = true;
        }
    }
}