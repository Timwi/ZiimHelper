using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using RT.Util;
using RT.Util.Drawing;
using RT.Util.ExtensionMethods;
using RT.Util.Xml;

namespace ZiimHelper
{
    abstract class Item
    {
        public abstract IEnumerable<ArrowInfo> Arrows { get; }
        public abstract IEnumerable<Cloud> Clouds { get; }
        public abstract void Move(int deltaX, int deltaY);
        public abstract void DrawSelected(Graphics g, int cellSize);
        public abstract bool IsContainedIn(int minX, int minY, int maxX, int maxY);
    }

    sealed class Cloud : Item
    {
        public Cloud() { }
        public Cloud(IEnumerable<Item> items) { Items.AddRange(items); }

        public List<Item> Items = new List<Item>();
        public Color Color = Color.FromArgb(64, 64, 192, 255);
        public string Label;
        public override IEnumerable<ArrowInfo> Arrows { get { return Items.SelectMany(item => item.Arrows); } }
        public override IEnumerable<Cloud> Clouds { get { return this.Concat(Items.SelectMany(i => i.Clouds)); } }
        public IEnumerable<Tuple<ArrowInfo, Cloud>> ArrowsWithParents
        {
            get
            {
                return Items.OfType<ArrowInfo>().Select(arrow => Tuple.Create(arrow, this)).Concat(
                             Items.OfType<Cloud>().SelectMany(cloud => cloud.ArrowsWithParents));
            }
        }
        public override void Move(int deltaX, int deltaY)
        {
            foreach (var item in Items)
                item.Move(deltaX, deltaY);
        }
        public override void DrawSelected(Graphics g, int cellSize)
        {
            drawCloud(g, cellSize, outline: new Pen(Brushes.Blue, 2), margin: cellSize / 10);
        }
        public void DrawCloud(Graphics g, int cellSize, bool own)
        {
            drawCloud(g, cellSize, fill: true, margin: cellSize / 10, style: own ? FontStyle.Bold : FontStyle.Regular, constrainWidth: own);
        }

        [XmlIgnore]
        private static FontFamily _cloudFont = new FontFamily("Gentium Book Basic");

        private void drawCloud(Graphics g, int cellSize, Pen outline = null, bool fill = false, int margin = 0, FontStyle style = FontStyle.Regular, bool constrainWidth = false)
        {
            var noInputArrows = Arrows.Where(a => !a.IsInput);
            var minX = noInputArrows.Min(a => a.X);
            var maxX = noInputArrows.Max(a => a.X);
            var minY = noInputArrows.Min(a => a.Y);
            var maxY = noInputArrows.Max(a => a.Y);
            var taken = Ut.NewArray<bool>(maxX - minX + 1, maxY - minY + 1);
            foreach (var arr in noInputArrows)
            {
                foreach (var dir in arr.Directions)
                {
                    int x = arr.X, y = arr.Y;
                    do
                    {
                        taken[x - minX][y - minY] = true;
                        x += dir.XOffset();
                        y += dir.YOffset();
                    }
                    while (x >= minX && x <= maxX && y >= minY && y <= maxY && !Arrows.Any(a => a.X == x && a.Y == y && !a.IsInput));
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
                g.FillPath(new SolidBrush(Color.FromArgb(64, Color)), path);
                if (Label != null)
                {
                    g.DrawString(
                        Label,
                        new Font(_cloudFont, g.GetMaximumFontSize(_cloudFont, Label, style, maxWidth: constrainWidth ? (maxX - minX + 1) * cellSize : (float?) null, maxHeight: cellSize), style),
                        new SolidBrush(Color),
                        new RectangleF(minX * cellSize, (Arrows.Max(a => a.Y) + 1) * cellSize, (maxX - minX + 1) * cellSize, 0),
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near }
                    );
                }
            }
            if (outline != null)
                g.DrawPath(outline, path);
        }

        public override bool IsContainedIn(int minX, int minY, int maxX, int maxY)
        {
            return Items.All(i => i.IsContainedIn(minX, minY, maxX, maxY));
        }
    }

    abstract class ArrowInfo : Item
    {
        public int X { get; set; }
        public int Y { get; set; }
        [XmlIgnoreIfDefault]
        public bool Marked { get; set; }
        [XmlIgnoreIfDefault]
        public string Annotation { get; set; }
        public string CoordsString { get { return "(" + X + ", " + Y + ")"; } }
        public abstract char Character { get; }
        public virtual bool IsInput { get { return false; } }
        public abstract IEnumerable<Direction> Directions { get; }
        public override IEnumerable<ArrowInfo> Arrows { get { return new[] { this }; } }
        public override IEnumerable<Cloud> Clouds { get { return Enumerable.Empty<Cloud>(); } }

        public override string ToString() { return CoordsString + (Annotation == null ? "" : " " + Annotation) + (Marked ? " [M] " : " ") + Character; }
        public abstract void Rotate(bool clockwise);
        public override void Move(int deltaX, int deltaY) { X += deltaX; Y += deltaY; }
        public abstract void Reorient(bool a, bool b, bool c, bool d);
        public void DrawReorienting(Graphics g, int cellSize)
        {
            var rect = new Rectangle(cellSize * X, cellSize * Y, cellSize, cellSize);
            g.FillEllipse(new SolidBrush(Color.FromArgb(32, 128, 32, 192)), rect);
            g.DrawEllipse(new Pen(Brushes.Green, 3), rect);
        }
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
        public bool IsInputArrow { get; set; }
        public override char Character { get { return IsInputArrow ? Direction.ToCharDouble() : Direction.ToChar(); } }
        public override void Rotate(bool clockwise) { Direction = (Direction) (((int) Direction + (clockwise ? 1 : 7)) % 8); }
        public override IEnumerable<Direction> Directions { get { return new[] { Direction }; } }
        public override bool IsInput { get { return IsInputArrow; } }
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
