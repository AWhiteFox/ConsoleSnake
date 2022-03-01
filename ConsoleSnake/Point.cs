namespace ConsoleSnake
{
    public readonly struct Point
    {
        public readonly int x;
        public readonly int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
        public static bool operator ==(Point a, Point b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Point a, Point b) => !(a == b);
        
        public override bool Equals(object obj) => obj is Point other && this == other;
        public override int GetHashCode() => (x * 397) ^ y;
    }
}