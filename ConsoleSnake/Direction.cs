namespace ConsoleSnake
{
    internal enum Direction
    {
        North,
        East,
        South,
        West
    }

    internal static class DirectionExtensions
    {
        public static Point Delta(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return new Point(0, -1);
                case Direction.East: return new Point(1, 0);
                case Direction.South: return new Point(0, 1);
                case Direction.West: return new Point(-1, 0);
            }
            return new Point(0, 0);
        }
    }
}
