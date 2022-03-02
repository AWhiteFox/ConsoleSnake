using ConsoleSnake.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleSnake
{
    internal static class Program
    {
        private const int TPS = 25;

        private static readonly Random _random = new Random();
        private static readonly object _inputLock = new object();
        private static bool _running;

        private const int STARTING_LENGTH = 16;
        private const int EXTENSION_PER_FOOD = 8;
        private static readonly char[] FOOD_CHARS = Enumerable.Range('\x1', 127).Select(x => (char)x).ToArray();

        private static readonly List<Point> _snake = new List<Point>();
        private static Direction _direction = Direction.East;
        private static Point? _foodPos;
        private static int _pendingExtension = 0;

        private const double HUE_STEP = 1.0 / TPS / 64.0;
        private const double TRAIL_SATURATION = 1;
        private const double SATURATION_DECAY = 1.0 / TPS / 4.0;

        private static double hue = 0;
        private static readonly List<TrailElement> _trail = new List<TrailElement>();
        
        public static void Main(string[] args)
        {
            ConsoleUtils.TryEnableAnsi();
            if (!ConsoleUtils.TryForceFullscreen())
            {
                Screen.SetSize(75, 20);
                Console.WriteLine("Set up console window and press any key...");
                Console.ReadKey();
                Screen.ShrinkToWindowSize();
            }
            else
            {
                Screen.UpdateSize();
            }
            Console.CursorVisible = false;
            Console.Clear();
            Console.Out.Write(ConsoleUtils.HsvToEscBg(0, 0, 0) + new string(' ', Screen.Width * (Screen.Height + 1)));
            Console.SetCursorPosition(0, 0);

            lock (_inputLock)
            {
                _running = true;
            }
            Task.Factory.StartNew(HandleInput);

            _pendingExtension = STARTING_LENGTH;

            const int wait = 1000 / TPS;
            var sw = new Stopwatch();
            while (true)
            {
                sw.Restart();
                lock (_inputLock)
                {
                    Update();
                }
                Thread.Sleep((int)Math.Max(0, wait - sw.ElapsedMilliseconds));
            }
        }

        private static void Update()
        {
            if (!TryExtend())
            {
                _running = false;
                return;
            }
            bool shrinked = false;
            if (_pendingExtension > 0)
            {
                _pendingExtension--;
            }
            else
            {
                Shrink();
                shrinked = true;
            }

            if (_foodPos.HasValue)
            {
                if (_snake.Contains(_foodPos.Value))
                {
                    _foodPos = null;
                    _pendingExtension += EXTENSION_PER_FOOD;
                }
                else
                {
                    int i = _random.Next(0, FOOD_CHARS.Length);
                    Screen.WriteAt(_foodPos.Value, ConsoleUtils.HsvToEscFg(hue, 1, 1) + FOOD_CHARS[i]);
                }
            }
            else
            {
                int x = _random.Next(1, Screen.Width);
                int y = _random.Next(1, Screen.Height);
                _foodPos = new Point(x, y);
            }

            for (int i = _trail.Count - 1; i >= 0; i--)
            {
                var elem = _trail[i];
                if (_snake.Contains(elem.Pos))
                {
                    _trail.RemoveAt(i);
                    continue;
                }

                if (elem.IsVisible && !_snake.Contains(elem.Pos))
                {
                    Screen.WriteAt(elem.Pos, ConsoleUtils.HsvToEscBg(elem.Hue, 1, elem.Value) + " " + ConsoleUtils.ResetBgEsc);
                    if (shrinked)
                    {
                        elem.Decay();
                    }
                }
                else
                {
                    Screen.WriteAt(elem.Pos, ConsoleUtils.HsvToEscBg(0, 0, 0) + ' ');
                    _trail.RemoveAt(i);
                }
            }

            hue = (hue + HUE_STEP) % 1;
        }

        private static void Shrink()
        {
            var p = _snake.PopAt(0);
            _trail.Add(new TrailElement()
            {
                Pos = p,
                Hue = (1 + (hue - HUE_STEP * _snake.Count)) % 1
            });
            Screen.WriteAt(p, ' ');
        }

        private static bool TryExtend()
        {
            var next = _snake.Count > 0 ? _snake[_snake.Count - 1] + _direction.Delta() : new Point(Screen.Width / 2, Screen.Height / 2);
            next = Screen.WrapPoint(next);
            if (!_snake.Contains(next))
            {
                _snake.Add(next);
                Screen.WriteAt(next, ConsoleUtils.HsvToEscBg(hue, 1, 1) + " " + ConsoleUtils.ResetBgEsc);
                return true;
            }
            return false;
        }

        private static void SetDirection(Direction direction)
        {
            if (_snake.Count <= 1 || _snake[_snake.Count - 1] + direction.Delta() != _snake[_snake.Count - 2])
            {
                _direction = direction;
            }
        }

        private static void HandleInput()
        {
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                lock (_inputLock)
                {
                    if (!_running)
                    {
                        return;
                    }
                    switch (key)
                    {
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                            SetDirection(Direction.North);
                            break;
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            SetDirection(Direction.West);
                            break;
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            SetDirection(Direction.South);
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            SetDirection(Direction.East);
                            break;
                    }
                }
            }
        }

        private class TrailElement
        {
            public Point Pos { get; set; }
            public double Hue { get; set; }
            public double Value { get; set; } = TRAIL_SATURATION;
            public bool IsVisible => Value > 0;

            public void Decay()
            {
                Value -= SATURATION_DECAY;
                if (Value < 0)
                {
                    Value = 0;
                }
            }
        }
    }
}
