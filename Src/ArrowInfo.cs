using RT.Util.ExtensionMethods;
using System;

namespace GraphiteHelper
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
            return base.ToString() + " " + Direction.ToStringExt() + (PointTo == null ? "" : " " + Distance + " " + PointTo);
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
                (PointTo1 == null ? "" : " " + Direction.GetDirection1().ToStringExt() + Distance1 + " " + PointTo1) +
                (PointTo2 == null ? "" : " " + Direction.GetDirection2().ToStringExt() + Distance2 + " " + PointTo2);
        }
        public override string Arrow { get { return Direction.ToStringExt(); } }
        public override void Rotate(bool clockwise)
        {
            if (clockwise)
                Direction = (DoubleDirection) (((int) Direction + 1) % 4);
            else
                Direction = (DoubleDirection) (((int) Direction + 3) % 4);
        }
    }
}
