using System.Collections.Generic;
using RT.Util.Xml;

namespace ZiimHelper
{
    abstract class Item { }

    abstract class Cloud : Item
    {
        public List<Item> Arrows;
    }

    abstract class ArrowInfo : Item
    {
        public int X { get; set; }
        public int Y { get; set; }
        [XmlIgnoreIfDefault]
        public bool Marked { get; set; }
        [XmlIgnoreIfDefault]
        public string Annotation { get; set; }
        public override string ToString()
        {
            return CoordsString + (Annotation == null ? "" : " " + Annotation) + (Marked ? " [M]" : "");
        }
        public string CoordsString { get { return "(" + X + ", " + Y + ")"; } }
        public abstract char Arrow { get; }
        public abstract void Rotate(bool clockwise);
        public abstract IEnumerable<Direction> Directions { get; }
    }

    sealed class SingleArrowInfo : ArrowInfo
    {
        public Direction Direction { get; set; }
        public override char Arrow { get { return Direction.ToChar(); } }
        public override void Rotate(bool clockwise) { Direction = (Direction) (((int) Direction + (clockwise ? 1 : 7)) % 8); }
        public override IEnumerable<Direction> Directions { get { return new[] { Direction }; } }
    }

    sealed class DoubleArrowInfo : ArrowInfo
    {
        public DoubleDirection Direction { get; set; }
        public override char Arrow { get { return Direction.ToChar(); } }
        public override void Rotate(bool clockwise) { Direction = (DoubleDirection) (((int) Direction + (clockwise ? 1 : 3)) % 4); }
        public override IEnumerable<Direction> Directions { get { return new[] { Direction.GetDirection1(), Direction.GetDirection2() }; } }
    }
}
