using RT.Util.Xml;

namespace ZiimHelper
{
    abstract class ArrowInfo
    {
        public int X { get; set; }
        public int Y { get; set; }
        [XmlIgnoreIfDefault]
        public string Warning { get; set; }
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
        public abstract void ProcessRemoved(ArrowInfo removed);
    }

    sealed class SingleArrowInfo : ArrowInfo
    {
        public Direction Direction { get; set; }
        [XmlIgnoreIfDefault]
        public ArrowInfo PointTo { get; set; }
        public int Distance { get; set; }
        public override string ToString()
        {
            return base.ToString() + " " + Direction.ToChar() + " " + Distance + " " + (PointTo == null ? "∅" : PointTo.CoordsString);
        }
        public override char Arrow { get { return Direction.ToChar(); } }
        public override void Rotate(bool clockwise)
        {
            if (clockwise)
                Direction = (Direction) (((int) Direction + 1) % 8);
            else
                Direction = (Direction) (((int) Direction + 7) % 8);
        }

        public override void ProcessRemoved(ArrowInfo removed)
        {
            if (PointTo == removed)
                PointTo = null;
        }
    }

    sealed class DoubleArrowInfo : ArrowInfo
    {
        public DoubleDirection Direction { get; set; }
        [XmlIgnoreIfDefault]
        public ArrowInfo PointTo1 { get; set; }
        [XmlIgnoreIfDefault]
        public ArrowInfo PointTo2 { get; set; }
        public int Distance1 { get; set; }
        public int Distance2 { get; set; }
        public override string ToString()
        {
            return base.ToString() + " " + Direction.ToChar() +
                " ① " + Distance1 + " " + Direction.GetDirection1().ToChar() + " " + (PointTo1 == null ? "∅" : PointTo1.CoordsString) +
                " ② " + Distance2 + " " + Direction.GetDirection2().ToChar() + " " + (PointTo2 == null ? "∅" : PointTo2.CoordsString);
        }
        public override char Arrow { get { return Direction.ToChar(); } }
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

        public override void ProcessRemoved(ArrowInfo removed)
        {
            if (PointTo1 == removed)
                PointTo1 = null;
            if (PointTo2 == removed)
                PointTo2 = null;
        }
    }
}
