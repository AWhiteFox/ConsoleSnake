using System;

namespace ConsoleSnake
{
    public static class Screen
    {
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public static void SetSize(int w, int h)
        {
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(w, h);
            Console.SetWindowSize(w, h);
            Width = w;
            Height = h - 1;
        }

        public static void UpdateSize()
        {
            Width = Console.BufferWidth;
            Height = Console.BufferHeight - 1;
        }

        public static void ShrinkToWindowSize()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight - 1;
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(Width, Height + 1);
            Console.SetWindowSize(Width, Height + 1);
        }

        public static void WriteAt(Point p, object o)
        {
            Console.SetCursorPosition(p.x, p.y);
            Console.Out.Write(o);
        }

        public static Point WrapPoint(Point p) => new Point((Width + p.x) % Width, (Height + p.y) % Height);
    }
}