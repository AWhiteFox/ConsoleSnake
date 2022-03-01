using System;
using System.Runtime.InteropServices;

namespace ConsoleSnake.External
{
    public static class Kernel32
    {
        public const int STD_OUTPUT_HANDLE = -11;

        public static readonly IntPtr StdOutputHandle;

        static Kernel32()
        {
            StdOutputHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD dwSize;
            public COORD dwCursorPosition;
            public ushort wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
        }

        [DllImport("kernel32.dll")] 
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")] 
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")] 
        public static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleHandle, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll")] 
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll")] 
        public static extern bool SetConsoleDisplayMode(IntPtr hConsoleHandle, uint dwFlags, out COORD lpNewScreenBufferDimensions);

        [DllImport("kernel32.dll")] 
        public static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleHandle, COORD dwSize);
    }
}
