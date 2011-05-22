using System;

namespace GraphiteHelper
{
    static class Extensions
    {
        public static string ToStringExt(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return "↑";
                case Direction.UpRight: return "↗";
                case Direction.Right: return "→";
                case Direction.DownRight: return "↘";
                case Direction.Down: return "↓";
                case Direction.DownLeft: return "↙";
                case Direction.Left: return "←";
                case Direction.UpLeft: return "↖";
                default: throw new InvalidOperationException();
            }
        }

        public static int XOffset(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                case Direction.Down:
                    return 0;

                case Direction.UpRight:
                case Direction.Right:
                case Direction.DownRight:
                    return 1;

                case Direction.DownLeft:
                case Direction.Left:
                case Direction.UpLeft:
                    return -1;

                default: throw new InvalidOperationException();
            }
        }

        public static int YOffset(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                case Direction.UpRight:
                case Direction.UpLeft:
                    return -1;

                case Direction.Down:
                case Direction.DownRight:
                case Direction.DownLeft:
                    return 1;

                case Direction.Left:
                case Direction.Right:
                    return 0;

                default: throw new InvalidOperationException();
            }
        }

        public static string ToStringExt(this DoubleDirection dir)
        {
            switch (dir)
            {
                case DoubleDirection.UpDown: return "↕";
                case DoubleDirection.UpRightDownLeft: return "⤢";
                case DoubleDirection.RightLeft: return "↔";
                case DoubleDirection.DownRightUpLeft: return "⤡";
                default: throw new InvalidOperationException();
            }
        }

        public static Direction GetDirection1(this DoubleDirection dir)
        {
            switch (dir)
            {
                case DoubleDirection.UpDown: return Direction.Up;
                case DoubleDirection.UpRightDownLeft: return Direction.UpRight;
                case DoubleDirection.RightLeft: return Direction.Right;
                case DoubleDirection.DownRightUpLeft: return Direction.DownRight;
                default: throw new InvalidOperationException();
            }
        }

        public static Direction GetDirection2(this DoubleDirection dir)
        {
            switch (dir)
            {
                case DoubleDirection.UpDown: return Direction.Down;
                case DoubleDirection.UpRightDownLeft: return Direction.DownLeft;
                case DoubleDirection.RightLeft: return Direction.Left;
                case DoubleDirection.DownRightUpLeft: return Direction.UpLeft;
                default: throw new InvalidOperationException();
            }
        }
    }
}
