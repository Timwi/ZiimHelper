using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using RT.Util;
using RT.Util.Drawing;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;
using RT.Util.Xml;

namespace ZiimHelper
{
    abstract class Item
    {
        public abstract IEnumerable<Item> AllItems { get; }
        public abstract IEnumerable<ArrowInfo> AllArrows { get; }
        public abstract IEnumerable<Cloud> AllClouds { get; }
        public abstract UserAction GetMoveAction(int deltaX, int deltaY);
        public abstract void DrawSelected(Graphics g, int cellSize);
        public abstract bool IsContainedIn(int minX, int minY, int maxX, int maxY);
        public abstract void GetBounds(out int minX, out int maxX, out int minY, out int maxY);
        public abstract Item ItemAt(int x, int y);
    }

    sealed class Cloud : Item
    {
        public Cloud() { }
        public Cloud(IEnumerable<Item> items) { Items.AddRange(items); }

        public List<Item> Items = new List<Item>();
        public Color Color = Color.FromArgb(64, 64, 192, 255);
        public string Label;
        public int LabelFromX, LabelFromY, LabelToX, LabelToY;
        public override IEnumerable<Item> AllItems { get { return this.Concat(Items.SelectMany(item => item.AllItems)); } }
        public override IEnumerable<ArrowInfo> AllArrows { get { return Items.SelectMany(item => item.AllArrows); } }
        public override IEnumerable<Cloud> AllClouds { get { return this.Concat(Items.SelectMany(i => i.AllClouds)); } }
        public IEnumerable<Tuple<ArrowInfo, Cloud>> ArrowsWithParents
        {
            get
            {
                return Items.OfType<ArrowInfo>().Select(arrow => Tuple.Create(arrow, this)).Concat(
                             Items.OfType<Cloud>().SelectMany(cloud => cloud.ArrowsWithParents));
            }
        }
        public override UserAction GetMoveAction(int deltaX, int deltaY)
        {
            return new MultiAction(
                Items.Select(item => item.GetMoveAction(deltaX, deltaY))
                    .Concat(new MoveLabel(this, true, LabelFromX, LabelFromY, LabelFromX + deltaX, LabelFromY + deltaY))
                    .Concat(new MoveLabel(this, false, LabelToX, LabelToY, LabelToX + deltaX, LabelToY + deltaY)));
        }
        public override void GetBounds(out int minX, out int maxX, out int minY, out int maxY)
        {
            bool first = true;
            minX = maxX = minY = maxY = 0;
            foreach (var item in Items)
            {
                if (first)
                {
                    item.GetBounds(out minX, out maxX, out minY, out maxY);
                    first = false;
                }
                else
                {
                    int miX, maX, miY, maY;
                    item.GetBounds(out miX, out maX, out miY, out maY);
                    minX = Math.Min(minX, miX);
                    maxX = Math.Max(maxX, maX);
                    minY = Math.Min(minY, miY);
                    maxY = Math.Max(maxY, maY);
                }
            }
        }
        public override void DrawSelected(Graphics g, int cellSize)
        {
            drawCloud(g, cellSize, outline: new Pen(Brushes.Blue, 2), margin: cellSize / 10);
        }
        public void DrawCloud(Graphics g, int cellSize, bool own)
        {
            drawCloud(g, cellSize, fill: true, margin: cellSize / 10, style: own ? FontStyle.Bold : FontStyle.Regular);
        }

        [XmlIgnore]
        private static FontFamily _cloudFont = new FontFamily("Gentium Book Basic");

        private void drawCloud(Graphics g, int cellSize, Pen outline = null, bool fill = false, int margin = 0, FontStyle style = FontStyle.Regular)
        {
            if (AllArrows.All(a => a.IsTerminal))
                return;
            int minX, maxX, minY, maxY;
            GetBounds(out minX, out maxX, out minY, out maxY);
            var taken = Ut.NewArray<bool>(maxX - minX + 1, maxY - minY + 1);
            foreach (var arr in AllArrows)
            {
                if (arr.IsTerminal)
                    continue;
                foreach (var dir in arr.Directions)
                {
                    int x = arr.X, y = arr.Y;
                    taken[x - minX][y - minY] = true;
                    bool pointsToOutside = false;
                    do
                    {
                        x += dir.XOffset();
                        y += dir.YOffset();
                        if (x < minX || x > maxX || y < minY || y > maxY)
                        {
                            pointsToOutside = true;
                            break;
                        }
                    }
                    while (!AllArrows.Any(a => a.X == x && a.Y == y && !a.IsTerminal));
                    if (pointsToOutside)
                        continue;

                    x = arr.X; y = arr.Y;
                    do
                    {
                        taken[x - minX][y - minY] = true;
                        x += dir.XOffset();
                        y += dir.YOffset();
                    }
                    while (x >= minX && x <= maxX && y >= minY && y <= maxY && !AllArrows.Any(a => a.X == x && a.Y == y && !a.IsTerminal));
                }
            }

            var path = Util.CloudPath(new Virtual2DArrayImpl((x, y) =>
            {
                if (x < 0 || x > maxX - minX || y < 0 || y > maxY - minY)
                    return false;
                var left = Enumerable.Range(0, maxX - minX + 1).IndexOf(i => taken[i][y]);
                var right = maxX - minX - Enumerable.Range(0, maxX - minX + 1).IndexOf(i => taken[maxX - minX - i][y]);
                var top = Enumerable.Range(0, maxY - minY + 1).IndexOf(i => taken[x][i]);
                var bottom = maxY - minY - Enumerable.Range(0, maxY - minY + 1).IndexOf(i => taken[x][maxY - minY - i]);
                return (left != -1 && left <= x && right >= x) || (top != -1 && top <= y && bottom >= y);
            }) { Width = maxX - minX + 1, Height = maxY - minY + 1 }, cellSize, margin);

            var m = new Matrix();
            m.Translate(cellSize * minX, cellSize * minY);
            path.Transform(m);

            if (fill)
            {
                if (Label != null)
                {
                    var distX = LabelToX - LabelFromX;
                    var distY = LabelToY - LabelFromY;
                    var dist = (float) (Math.Sqrt(distX * distX + distY * distY) * cellSize);

                    if (dist > 0)
                    {
                        var centerPoint = new PointF((LabelFromX + LabelToX + 1) * cellSize / 2, (LabelFromY + LabelToY + 1) * cellSize / 2);
                        var stringPath = new GraphicsPath();
                        stringPath.AddString(
                            Label,
                            _cloudFont,
                            (int) style,
                            g.GetMaximumFontSize(_cloudFont, Label, style, maxWidth: dist) * g.DpiY / 72,
                            centerPoint,
                            Util.CenterCenter
                        );
                        var rotate = new Matrix();
                        rotate.RotateAt((float) (Math.Atan2(distY, distX) * 180 / Math.PI), centerPoint);
                        stringPath.Transform(rotate);
                        path.AddPath(stringPath, false);
                    }
                }
                g.FillPath(new SolidBrush(Color.FromArgb(64, Color)), path);
            }
            if (outline != null)
                g.DrawPath(outline, path);
        }

        public override bool IsContainedIn(int minX, int minY, int maxX, int maxY)
        {
            return Items.All(i => (i is ArrowInfo && ((ArrowInfo) i).IsTerminal) || i.IsContainedIn(minX, minY, maxX, maxY));
        }

        public override Item ItemAt(int x, int y)
        {
            foreach (var item in Items.OrderByDescending(i => i is ArrowInfo))
            {
                var itemAt = item.ItemAt(x, y);
                if (itemAt != null)
                    return item;    // NOT itemAt
            }
            return null;
        }
    }

    abstract class ArrowInfo : Item
    {
        public int X { get; set; }
        public int Y { get; set; }
        [XmlIgnoreIfDefault, ClassifyIgnoreIfDefault]
        public bool Marked { get; set; }
        [XmlIgnoreIfDefault, ClassifyIgnoreIfDefault]
        public string Annotation { get; set; }
        public string CoordsString { get { return "(" + X + ", " + Y + ")"; } }
        public abstract char Character { get; }
        public virtual bool IsTerminal { get { return false; } }
        public abstract IEnumerable<Direction> Directions { get; }
        public override IEnumerable<Item> AllItems { get { return new[] { this }; } }
        public override IEnumerable<ArrowInfo> AllArrows { get { return new[] { this }; } }
        public override IEnumerable<Cloud> AllClouds { get { return Enumerable.Empty<Cloud>(); } }
        public override Item ItemAt(int x, int y) { return x == X && y == Y ? this : null; }

        public override void GetBounds(out int minX, out int maxX, out int minY, out int maxY)
        {
            minX = maxX = X;
            minY = maxY = Y;
        }
        public override string ToString() { return CoordsString + (Annotation == null ? "" : " " + Annotation) + (Marked ? " [M] " : " ") + Character; }
        public abstract void Rotate(bool clockwise);
        public override UserAction GetMoveAction(int deltaX, int deltaY) { return new MoveArrow(this, X, Y, X + deltaX, Y + deltaY); }
        public abstract void Reorient(bool a, bool b, bool c, bool d);
        public override void DrawSelected(Graphics g, int cellSize)
        {
            g.DrawEllipse(new Pen(Brushes.Blue, 2), X * cellSize + cellSize / 10, Y * cellSize + cellSize / 10, cellSize * 8 / 10, cellSize * 8 / 10);
        }

        public override bool IsContainedIn(int minX, int minY, int maxX, int maxY)
        {
            return X >= minX && X <= maxX && Y >= minY && Y <= maxY;
        }
    }

    class SingleArrowInfo : ArrowInfo
    {
        public Direction Direction { get; set; }
        [XmlIgnoreIfDefault, ClassifyIgnoreIfDefault]
        public bool IsTerminalArrow { get; set; }
        public override char Character { get { return IsTerminalArrow ? Direction.ToCharDouble() : Direction.ToChar(); } }
        public override void Rotate(bool clockwise) { Direction = (Direction) (((int) Direction + (clockwise ? 1 : 7)) % 8); }
        public override IEnumerable<Direction> Directions { get { return new[] { Direction }; } }
        public override bool IsTerminal { get { return IsTerminalArrow; } }
        public override void Reorient(bool a, bool b, bool c, bool d)
        {
            Direction =
                b && c ? Direction.Down :
                !c && d ? Direction.DownLeft :
                !d && a ? Direction.Left :
                !a && b ? Direction.UpLeft :
                !b && !c ? Direction.Up :
                c && !d ? Direction.UpRight :
                d && !a ? Direction.Right :
                a && !b ? Direction.DownRight : Ut.Throw<Direction>(new InvalidOperationException());
        }
    }

    sealed class DoubleArrowInfo : ArrowInfo
    {
        public DoubleDirection Direction { get; set; }
        public override char Character { get { return Direction.ToChar(); } }
        public override void Rotate(bool clockwise) { Direction = (DoubleDirection) (((int) Direction + (clockwise ? 1 : 3)) % 4); }
        public override IEnumerable<Direction> Directions { get { return new[] { Direction.GetDirection1(), Direction.GetDirection2() }; } }
        public override void Reorient(bool a, bool b, bool c, bool d)
        {
            Direction =
                b == c ? DoubleDirection.UpDown :
                c != d ? DoubleDirection.UpRightDownLeft :
                d != a ? DoubleDirection.RightLeft :
                a != b ? DoubleDirection.DownRightUpLeft : Ut.Throw<DoubleDirection>(new InvalidOperationException());
        }
    }
}
