using RT.Util.ExtensionMethods;
using System;
using RT.Util;

namespace ZiimHelper
{
    abstract class ArrowInfo
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Warning { get; set; }
        public override string ToString()
        {
            return Name + " (" + X + ", " + Y + ")";
        }
        public abstract string Arrow { get; }
        public abstract void Rotate(bool clockwise);
    }

    sealed class SingleArrowInfo : ArrowInfo
    {
        public Direction Direction { get; set; }
        public string PointTo { get; set; }
        public int Distance { get; set; }
        public override string ToString()
        {
            return base.ToString() + " " + Direction.ToStringExt() + " " + Distance + " " + (PointTo ?? "∅");
        }
        public override string Arrow { get { return Direction.ToStringExt(); } }
        public override void Rotate(bool clockwise)
        {
            if (clockwise)
                Direction = (Direction) (((int) Direction + 1) % 8);
            else
                Direction = (Direction) (((int) Direction + 7) % 8);
        }
    }

    sealed class DoubleArrowInfo : ArrowInfo
    {
        public DoubleDirection Direction { get; set; }
        public string PointTo1 { get; set; }
        public string PointTo2 { get; set; }
        public int Distance1 { get; set; }
        public int Distance2 { get; set; }
        public override string ToString()
        {
            return base.ToString() + " " + Direction.ToStringExt() +
                " ① " + Distance1 + " " + Direction.GetDirection1().ToStringExt() + " " + (PointTo1 ?? "∅") +
                " ② " + Distance2 + " " + Direction.GetDirection2().ToStringExt() + " " + (PointTo2 ?? "∅");
        }
        public override string Arrow { get { return Direction.ToStringExt(); } }
        public override void Rotate(bool clockwise)
        {
            bool swap;
            if (clockwise)
            {
                Direction = (DoubleDirection) (((int) Direction + 1) % 4);
                swap = Direction == DoubleDirection.UpDown;

            }
            else
            {
                Direction = (DoubleDirection) (((int) Direction + 3) % 4);
                swap = Direction == DoubleDirection.DownRightUpLeft;
            }
            if (swap)
            {
                var p = PointTo1; PointTo1 = PointTo2; PointTo2 = p;
                var d = Distance1; Distance1 = Distance2; Distance2 = d;
            }
        }
    }
}
