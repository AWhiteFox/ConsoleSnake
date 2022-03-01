using ConsoleSnake.External;
using System;

namespace ConsoleSnake
{
    public static class ConsoleUtils
    {
        public const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        public static bool TryEnableAnsi()
        {
            return Kernel32.GetConsoleMode(Kernel32.StdOutputHandle, out uint mode)
                && Kernel32.SetConsoleMode(Kernel32.StdOutputHandle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        public static bool TryForceFullscreen()
        {
            return Kernel32.SetConsoleDisplayMode(Kernel32.StdOutputHandle, 1, out _)
                && Kernel32.GetConsoleScreenBufferInfo(Kernel32.StdOutputHandle, out var info)
                && Kernel32.SetConsoleScreenBufferSize(Kernel32.StdOutputHandle, info.dwMaximumWindowSize);
        }

        public const string ResetBgEsc = "\u001b[49m";

        public static string HsvToEscFg(double hue, double sat, double val) => $"\u001b[38;{HsvToEsc(hue, sat, val)}";

        public static string HsvToEscBg(double hue, double sat, double val) => $"\u001b[48;{HsvToEsc(hue, sat, val)}";

        private static string HsvToEsc(double hue, double sat, double val)
        {
            hue *= 360;
            val *= 255;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            int v = Convert.ToInt32(val);
            int p = Convert.ToInt32(val * (1 - sat));
            int q = Convert.ToInt32(val * (1 - f * sat));
            int t = Convert.ToInt32(val * (1 - (1 - f) * sat));

            if (hi == 0)
                return $"2;{v};{t};{p}m";
            else if (hi == 1)
                return $"2;{q};{v};{p}m";
            else if (hi == 2)
                return $"2;{p};{v};{t}m";
            else if (hi == 3)
                return $"2;{p};{q};{v}m";
            else if (hi == 4)
                return $"2;{t};{p};{v}m";
            else
                return $"2;{v};{p};{q}m";
        }
    }
}
