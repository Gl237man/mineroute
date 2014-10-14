namespace RouteUtils
{
    public class InPort
    {
        public readonly string Name;
        public readonly int PosX;
        public readonly int PosY;

        public InPort(string name, int x, int y)
        {
            Name = name;
            PosX = x;
            PosY = y;
        }
    }
}