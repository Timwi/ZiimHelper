using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.Drawing;
using RT.Util.ExtensionMethods;
using RT.Util.Forms;

namespace ZiimHelper
{
    public partial class Mainform : ManagedForm
    {
        private FontFamily _arrowFont = new FontFamily("Cambria");
        private FontFamily _instructionFont = new FontFamily("Gentium Book Basic");
        private FontFamily _warningFont = new FontFamily("Calibri");

        public Mainform()
            : base(ZiimHelperProgram.Settings.FormSettings)
        {
            InitializeComponent();
            revert();
        }

        private void newArrow(object sender, EventArgs __)
        {
            var newArrow = sender == miNewSingleArrow ? (ArrowInfo) new SingleArrowInfo() : new DoubleArrowInfo();

            bool done = false;
            if (ctList.SelectedItems.Count == 1)
            {
                var sel = ctList.SelectedItems[0];
                var sai = sel as SingleArrowInfo;
                var dai = sel as DoubleArrowInfo;
                if (sai != null && sai.PointTo == null)
                {
                    newArrow.X = sai.X + sai.Direction.XOffset() * sai.Distance;
                    newArrow.Y = sai.Y + sai.Direction.YOffset() * sai.Distance;
                    sai.PointTo = newArrow;
                    done = true;
                }
                else if (dai != null)
                {
                    if (dai.PointTo1 == null)
                    {
                        newArrow.X = dai.X + dai.Direction.GetDirection1().XOffset() * dai.Distance1;
                        newArrow.Y = dai.Y + dai.Direction.GetDirection1().YOffset() * dai.Distance1;
                        dai.PointTo1 = newArrow;
                        done = true;
                    }
                    else if (dai.PointTo2 == null)
                    {
                        newArrow.X = dai.X + dai.Direction.GetDirection2().XOffset() * dai.Distance2;
                        newArrow.Y = dai.Y + dai.Direction.GetDirection2().YOffset() * dai.Distance2;
                        dai.PointTo2 = newArrow;
                        done = true;
                    }
                }
            }

            if (!done)
            {
                var already = new Dictionary<int, Dictionary<int, ArrowInfo>>();
                foreach (var item in ctList.Items.Cast<ArrowInfo>())
                    already.AddSafe(item.X, item.Y, item);

                while (already.ContainsKey(newArrow.X) && already[newArrow.X].ContainsKey(newArrow.Y))
                {
                    newArrow.X += Rnd.Next(5) - 2;
                    newArrow.Y += Rnd.Next(5) - 2;
                }
            }

            var o = ctList.OutlineIndex;
            ctList.Items.Insert(o, newArrow);
            ctList.ClearSelected();
            ctList.SelectedIndex = o;
            ctImage.Refresh();
        }

        private void deleteArrow(object _, EventArgs __)
        {
            if (ctList.SelectedIndices.Count == 0)
                return;
            var selectedIndex = ctList.SelectedIndices.Cast<int>().Order().First();
            var toRemove = ctList.SelectedItems.Cast<ArrowInfo>().ToList();
            foreach (var a in toRemove)
                ctList.Items.Remove(a);
            foreach (var arrow in ctList.Items.Cast<ArrowInfo>())
                foreach (var removed in toRemove)
                    arrow.ProcessRemoved(removed);
            if (ctList.Items.Count > 0)
            {
                ctList.ClearSelected();
                ctList.SelectedIndex = selectedIndex >= ctList.Items.Count ? selectedIndex - 1 : selectedIndex;
            }
            refresh();
        }

        private bool _suppressRepaint;

        private void moveUpInList(object _, EventArgs __)
        {
            if (ctList.SelectedIndices.Count != 1)
                return;
            var s = ctList.SelectedIndex;
            if (s == 0)
                return;
            _suppressRepaint = true;
            var item = ctList.Items[s];
            ctList.Items.RemoveAt(s);
            ctList.Items.Insert(s - 1, item);
            ctList.ClearSelected();
            ctList.SelectedIndex = s - 1;
            _suppressRepaint = false;
            ctImage.Invalidate();
        }

        private void moveDownInList(object _, EventArgs __)
        {
            if (ctList.SelectedIndices.Count != 1)
                return;
            var s = ctList.SelectedIndex;
            if (s == ctList.Items.Count - 1)
                return;
            _suppressRepaint = true;
            var item = ctList.Items[s];
            ctList.Items.RemoveAt(s);
            ctList.Items.Insert(s + 1, item);
            ctList.ClearSelected();
            ctList.SelectedIndex = s + 1;
            _suppressRepaint = false;
            ctImage.Invalidate();
        }

        private void paint(object _, PaintEventArgs e)
        {
            if (_paintCellSize < 1)
                return;
            e.Graphics.SetHighQuality();
            foreach (var arr in ctList.SelectedItems.Cast<ArrowInfo>())
                e.Graphics.DrawEllipse(new Pen(Brushes.Red, 2),
                    _paintTarget.Left + (arr.X - _paintMinX) * _paintCellSize + _paintCellSize / 10,
                    _paintTarget.Top + (arr.Y - _paintMinY) * _paintCellSize + _paintCellSize / 10,
                    _paintCellSize * 8 / 10, _paintCellSize * 8 / 10);

            if (_imageMouseDown != null)
            {
                var minX = Math.Min(_imageMouseDown.Value.X, _imageMouseDraggedTo.X);
                var minY = Math.Min(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y);
                var maxX = Math.Max(_imageMouseDown.Value.X, _imageMouseDraggedTo.X);
                var maxY = Math.Max(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y);
                var rect = new Rectangle(_paintTarget.Left + _paintCellSize * minX, _paintTarget.Top + _paintCellSize * minY, _paintCellSize * (maxX - minX + 1), _paintCellSize * (maxY - minY + 1));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 32, 128, 192)), rect);
                e.Graphics.DrawRectangle(new Pen(Brushes.Blue, 2), rect);
            }
        }

        private int _paintMinX;
        private int _paintMinY;
        private int _paintMaxX;
        private int _paintMaxY;
        private Rectangle _paintTarget;
        private int _paintCellSize;
        private float _paintFontSize;

        private void paintBuffer(object _, PaintEventArgs e)
        {
            var margin = 15;

            e.Graphics.Clear(Color.Gray);
            if (ctList.Items.Count < 1)
                return;

            var fit = new Size(_paintMaxX - _paintMinX + 1, _paintMaxY - _paintMinY + 1).FitIntoMaintainAspectRatio(new Rectangle(margin, margin, ctImage.ClientSize.Width - 2 * margin, ctImage.ClientSize.Height - 2 * margin));
            var w = fit.Width - fit.Width % (_paintMaxX - _paintMinX + 1);
            var h = fit.Height - fit.Height % (_paintMaxY - _paintMinY + 1);

            _paintMinX = ctList.Items.Cast<ArrowInfo>().Min(a => a.X);
            _paintMinY = ctList.Items.Cast<ArrowInfo>().Min(a => a.Y);
            _paintMaxX = ctList.Items.Cast<ArrowInfo>().Max(a => a.X);
            _paintMaxY = ctList.Items.Cast<ArrowInfo>().Max(a => a.Y);

            _paintTarget = new Rectangle(ctImage.ClientSize.Width / 2 - w / 2, ctImage.ClientSize.Height / 2 - h / 2, w, h);
            _paintCellSize = _paintTarget.Width / (_paintMaxX - _paintMinX + 1);
            _paintFontSize = "↑↗→↘↓↙←↖↕⤢↔⤡".Min(ch => e.Graphics.GetMaximumFontSize(new SizeF(_paintCellSize, _paintCellSize), _arrowFont, ch.ToString()));

            e.Graphics.FillRectangle(Brushes.White, _paintTarget);
            e.Graphics.DrawRectangle(Pens.Black, _paintTarget);
            var m = new Matrix();
            m.Translate(_paintTarget.Left, _paintTarget.Top);
            e.Graphics.Transform = m;
            paintInto(e.Graphics, _paintCellSize, _paintFontSize);
            e.Graphics.ResetTransform();
        }

        private void paintInto(Graphics g, int cellSize, float fontSize)
        {
            g.SetHighQuality();

            if (miGrid.Checked)
            {
                for (int i = 1; i <= _paintMaxX - _paintMinX; i++)
                    g.DrawLine(Pens.DarkGray, i * cellSize, 0, i * cellSize, (_paintMaxY - _paintMinY + 1) * cellSize);
                for (int j = 1; j <= _paintMaxY - _paintMinY; j++)
                    g.DrawLine(Pens.DarkGray, 0, j * cellSize, (_paintMaxX - _paintMinX + 1) * cellSize, j * cellSize);
            }

            var hitFromDic = new Dictionary<ArrowInfo, List<Direction>>();

            foreach (var arr in ctList.Items.Cast<ArrowInfo>())
            {
                g.DrawString(arr.Arrow.ToString(), new Font(_arrowFont, fontSize), arr.Marked ? Brushes.Red : Brushes.Black, (arr.X - _paintMinX) * cellSize + cellSize / 2, (arr.Y - _paintMinY) * cellSize + cellSize / 2,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                if (miAnnotations.Checked)
                {
                    if (arr.Annotation != null)
                        drawInRoundedRectangle(g, arr.Annotation, new PointF((arr.X - _paintMinX) * cellSize + cellSize / 2, (arr.Y - _paintMinY) * cellSize), Color.FromArgb(0xEE, 0xEE, 0xFF), Color.Blue, Color.DarkBlue);
                    if (arr.Warning != null)
                        drawInRoundedRectangle(g, arr.Warning, new PointF((arr.X - _paintMinX) * cellSize + cellSize / 2, (arr.Y - _paintMinY) * cellSize + cellSize * 4 / 5), Color.FromArgb(0xFF, 0xEE, 0xEE), Color.Red, Color.DarkRed);
                }

                if (miConnectionLines.Checked || miInstructions.Checked)
                {
                    var sai = arr as SingleArrowInfo;
                    var dai = arr as DoubleArrowInfo;
                    if (sai != null && sai.PointTo != null)
                        hitFromDic.AddSafe(sai.PointTo, sai.Direction);
                    else if (dai != null)
                    {
                        if (dai.PointTo1 != null)
                            hitFromDic.AddSafe(dai.PointTo1, dai.Direction.GetDirection1());
                        if (dai.PointTo2 != null)
                            hitFromDic.AddSafe(dai.PointTo2, dai.Direction.GetDirection2());
                    }
                }
            }

            if (miConnectionLines.Checked || miInstructions.Checked)
            {
                foreach (var arr in ctList.Items.Cast<ArrowInfo>())
                {
                    var x = (arr.X - _paintMinX) * cellSize;
                    var y = (arr.Y - _paintMinY) * cellSize;

                    var directions = hitFromDic.ContainsKey(arr) ? hitFromDic[arr] : null;
                    if (directions != null && miConnectionLines.Checked)
                    {
                        foreach (var direction in directions)
                            g.DrawString(direction.ToChar().ToString(), new Font(_arrowFont, fontSize / 3), Brushes.DarkGreen,
                                x + cellSize / 2 - cellSize * direction.XOffset() * 3 / 10,
                                y + cellSize / 2 - cellSize * direction.YOffset() * 3 / 10,
                                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }

                    if (miInstructions.Checked)
                    {
                        string instruction = null;
                        Direction dir = 0;
                        if (arr is SingleArrowInfo)
                        {
                            var sai = (SingleArrowInfo) arr;
                            dir = sai.Direction;
                            if (directions == null || directions.Count == 0)   // { 0 }
                                instruction = "0";
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 3) % 8))     // invert
                                instruction = "I";
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 5) % 8))     // no-op
                                instruction = "N";
                            else if (directions.Count == 2 && directions.Contains((Direction) (((int) sai.Direction + 1) % 8)) && directions.Contains((Direction) (((int) sai.Direction + 7) % 8)))  // concatenator
                                instruction = "C";
                        }
                        else
                        {
                            var dai = (DoubleArrowInfo) arr;
                            if (directions != null && directions.Count == 1 && directions[0] == (Direction) (((int) dai.Direction.GetDirection1() + 2) % 8))  // splitter
                            {
                                instruction = "S";
                                dir = dai.Direction.GetDirection1();
                            }
                            else if (directions != null && directions.Count == 1 && directions[0] == (Direction) (((int) dai.Direction.GetDirection1() + 6) % 8))  // splitter
                            {
                                instruction = "S";
                                dir = dai.Direction.GetDirection2();
                            }
                            else if (directions != null && directions.Count == 1 && directions[0] == (Direction) (((int) dai.Direction.GetDirection1() + 1) % 8))  // isZero
                            {
                                instruction = "Z";
                                dir = dai.Direction.GetDirection1();
                            }
                            else if (directions != null && directions.Count == 1 && directions[0] == (Direction) (((int) dai.Direction.GetDirection1() + 5) % 8))  // isZero
                            {
                                instruction = "Z";
                                dir = dai.Direction.GetDirection2();
                            }
                            else if (directions != null && directions.Count == 1 && directions[0] == (Direction) (((int) dai.Direction.GetDirection1() + 3) % 8))  // isEmpty
                            {
                                instruction = "E";
                                dir = dai.Direction.GetDirection1();
                            }
                            else if (directions != null && directions.Count == 1 && directions[0] == (Direction) (((int) dai.Direction.GetDirection1() + 7) % 8))  // isEmpty
                            {
                                instruction = "E";
                                dir = dai.Direction.GetDirection1();
                            }
                        }

                        if (instruction == null)
                            g.FillEllipse(new SolidBrush(Color.FromArgb(64, 255, 128, 128)), x, y, cellSize, cellSize);
                        else
                            g.DrawString(instruction, new Font(_instructionFont, fontSize / 2), Brushes.Black,
                                (float) (x + cellSize / 2 + Math.Cos(Math.PI / 4 * (int) dir) * cellSize / 4),
                                (float) (y + cellSize / 2 + Math.Sin(Math.PI / 4 * (int) dir) * cellSize / 4),
                                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                            );
                    }
                }
            }

            if (miConnectionLines.Checked)
            {
                foreach (var arr in ctList.Items.Cast<ArrowInfo>())
                {
                    var sai = arr as SingleArrowInfo;
                    var dai = arr as DoubleArrowInfo;
                    ArrowInfo targetArr;
                    if (sai != null && sai.PointTo != null)
                        drawHit(g, sai.X - _paintMinX, sai.Y - _paintMinY, sai.Direction.XOffset(), sai.Direction.YOffset(), sai.PointTo.X - _paintMinX, sai.PointTo.Y - _paintMinY, cellSize, sai.Distance);
                    else if (dai != null)
                    {
                        if (dai.PointTo1 != null)
                            drawHit(g, dai.X - _paintMinX, dai.Y - _paintMinY, dai.Direction.GetDirection1().XOffset(), dai.Direction.GetDirection1().YOffset(), dai.PointTo1.X - _paintMinX, dai.PointTo1.Y - _paintMinY, cellSize, dai.Distance1);
                        if (dai.PointTo2 != null)
                            drawHit(g, dai.X - _paintMinX, dai.Y - _paintMinY, dai.Direction.GetDirection2().XOffset(), dai.Direction.GetDirection2().YOffset(), dai.PointTo2.X - _paintMinX, dai.PointTo2.Y - _paintMinY, cellSize, dai.Distance2);
                    }
                }
            }
        }

        private void drawHit(Graphics g, int fromX, int fromY, int dirX, int dirY, int toX, int toY, int cellSize, int expectedDistance)
        {
            var correct = toX - fromX == dirX * expectedDistance && toY - fromY == dirY * expectedDistance;
            var acceptable = dirX == 0 || dirY == 0 || (toX - fromX) / dirX == (toY - fromY) / dirY;
            g.DrawLine(new Pen(new SolidBrush(correct ? Color.LightGreen : Color.Red), correct || acceptable ? 1f : 2f) /*{ EndCap = LineCap.ArrowAnchor }*/,
                cellSize * fromX + cellSize / 2 + dirX * cellSize / 4, cellSize * fromY + cellSize / 2 + dirY * cellSize / 4,
                cellSize * toX + cellSize / 2 - dirX * cellSize / 4, cellSize * toY + cellSize / 2 - dirY * cellSize / 4);
        }

        private void drawInRoundedRectangle(Graphics g, string text, PointF location, Color background, Color outline, Color textColor)
        {
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var size = g.MeasureString(text, new Font(_warningFont, _paintFontSize / 5), int.MaxValue, sf) + new SizeF(6, 2);
            var realLocation = location - new SizeF(size.Width / 2, size.Height / 2);
            var path = GraphicsUtil.RoundedRectangle(new RectangleF(realLocation, size), 1/*5*/);
            g.FillPath(new SolidBrush(background), path);
            g.DrawPath(new Pen(outline, 1), path);
            g.DrawString(text, new Font(_warningFont, _paintFontSize / 5), new SolidBrush(textColor), new RectangleF(realLocation + new SizeF(3, 1), size - new SizeF(6, 2)), sf);
        }

        private void rotate(object sender, EventArgs __)
        {
            if (ctList.SelectedIndices.Count == 0)
                return;
            foreach (var item in ctList.SelectedItems.Cast<ArrowInfo>())
                item.Rotate(sender == miRotateClockwise);
            refresh();
        }

        private void reflow(object _, EventArgs __)
        {
            try
            {
                var todo = ctList.Items.Cast<ArrowInfo>().ToList();
                var done = new List<ArrowInfo>();
                var taken = new Dictionary<int, Dictionary<int, ArrowInfo>>();

                var node = ctList.SelectedItems.Cast<ArrowInfo>().FirstOrDefault() ?? todo.First();
                while (true)
                {
                    reflowRecurse(todo, done, taken, node, node.X, node.Y);
                    if (todo.Count == 0)
                        break;
                    node = todo.First();
                }

                int minX = 0, maxX = 0, minY = 0, maxY = 0;
                using (var en = ctList.Items.Cast<ArrowInfo>().GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        minX = maxX = en.Current.X;
                        minY = maxY = en.Current.Y;
                        while (en.MoveNext())
                        {
                            minX = Math.Min(minX, en.Current.X);
                            maxX = Math.Max(maxX, en.Current.X);
                            minY = Math.Min(minY, en.Current.Y);
                            maxY = Math.Max(maxY, en.Current.Y);
                        }
                    }
                }

                // Check whether any arrows are pointing to an arrow that they shouldn’t
                var check = Ut.Lambda((ArrowInfo item, Direction dir, string msg) =>
                {
                    var xoff = dir.XOffset();
                    var yoff = dir.YOffset();
                    int x = item.X, y = item.Y;
                    while (true)
                    {
                        x += xoff;
                        y += yoff;
                        if (x < minX || x > maxX || y < minY || y > maxY)
                            break;
                        if (taken.ContainsKey(x) && taken[x].ContainsKey(y) && taken[x][y].X == x && taken[x][y].Y == y)
                            item.Warning = msg;
                    }
                });
                foreach (var item in ctList.Items.Cast<ArrowInfo>())
                {
                    if (item is SingleArrowInfo)
                    {
                        var sai = (SingleArrowInfo) item;
                        if (sai.PointTo == null)
                            check(item, sai.Direction, "Points to arrow.");
                    }
                    else
                    {
                        var dai = (DoubleArrowInfo) item;
                        if (dai.PointTo1 == null)
                            check(item, dai.Direction.GetDirection1(), "Points to arrow (1).");
                        if (dai.PointTo2 == null)
                            check(item, dai.Direction.GetDirection2(), "Points to arrow (2).");
                    }
                }
            }
            catch (Exception exc)
            {
                DlgMessage.Show(exc.Message, "Exception", DlgType.Error, "&OK");
            }
            refresh();
        }

        private void reflowRecurse(List<ArrowInfo> todo, List<ArrowInfo> done, Dictionary<int, Dictionary<int, ArrowInfo>> taken, ArrowInfo cur, int x, int y)
        {
            cur.Warning = taken.ContainsKey(x) && taken[x].ContainsKey(y) ? "Conflicts with {0}.".Fmt(taken[x][y].CoordsString) : null;
            cur.X = x;
            cur.Y = y;
            todo.Remove(cur);
            done.Add(cur);
            taken.AddSafe(x, y, cur);

            Action<int, int, int, ArrowInfo> setWarning = (xOffset, yOffset, distance, item) =>
            {
                string warning = "Conflicts with {0}.".Fmt(item.CoordsString);
                for (int i = 0; i < distance; i++)
                {
                    foreach (var don in done)
                        if (don != cur && don.X == x + xOffset * i && don.Y == y + yOffset * i)
                            don.Warning = don.Warning.AddLine(warning);
                    taken.AddSafe(x + xOffset * i, y + yOffset * i, item);
                }
            };

            var sai = cur as SingleArrowInfo;
            var dai = cur as DoubleArrowInfo;

            if (sai != null && sai.PointTo != null)
            {
                var xOffset = sai.Direction.XOffset();
                var yOffset = sai.Direction.YOffset();
                setWarning(xOffset, yOffset, sai.Distance, sai);

                if (todo.Contains(sai.PointTo))
                    reflowRecurse(todo, done, taken, sai.PointTo, x + xOffset * sai.Distance, y + yOffset * sai.Distance);
                else if (done.Contains(sai.PointTo))
                {
                    if (sai.PointTo.X != x + sai.Direction.XOffset() * sai.Distance || sai.PointTo.Y != y + sai.Direction.YOffset() * sai.Distance)
                        cur.Warning = cur.Warning.AddLine("Arrow doesn’t join up.");
                }
                else
                    cur.Warning = cur.Warning.AddLine("Points into nirvana.");
            }
            else if (dai != null)
            {
                if (dai.PointTo1 != null)
                {
                    var xOffset = dai.Direction.GetDirection1().XOffset();
                    var yOffset = dai.Direction.GetDirection1().YOffset();
                    setWarning(xOffset, yOffset, dai.Distance1, dai);

                    if (todo.Contains(dai.PointTo1))
                        reflowRecurse(todo, done, taken, dai.PointTo1, x + xOffset * dai.Distance1, y + yOffset * dai.Distance1);
                    else if (done.Contains(dai.PointTo1))
                    {
                        if (dai.PointTo1.X != x + dai.Direction.GetDirection1().XOffset() * dai.Distance1 || dai.PointTo1.Y != y + dai.Direction.GetDirection1().YOffset() * dai.Distance1)
                            cur.Warning = cur.Warning.AddLine("Doesn’t join up (1).");
                    }
                    else
                        cur.Warning = cur.Warning.AddLine("Points into nirvana (1).");
                }

                if (dai.PointTo2 != null)
                {
                    var xOffset = dai.Direction.GetDirection2().XOffset();
                    var yOffset = dai.Direction.GetDirection2().YOffset();
                    setWarning(xOffset, yOffset, dai.Distance2, dai);

                    if (todo.Contains(dai.PointTo2))
                        reflowRecurse(todo, done, taken, dai.PointTo2, x + xOffset * dai.Distance2, y + yOffset * dai.Distance2);
                    else if (done.Contains(dai.PointTo2))
                    {
                        if (dai.PointTo2.X != x + dai.Direction.GetDirection2().XOffset() * dai.Distance2 || dai.PointTo2.Y != y + dai.Direction.GetDirection2().YOffset() * dai.Distance2)
                            cur.Warning = cur.Warning.AddLine("Doesn’t join up (2).");
                    }
                    else
                        cur.Warning = cur.Warning.AddLine("Points into nirvana (2).");
                }
            }

            foreach (var a in todo.Concat(done).ToList())
            {
                sai = a as SingleArrowInfo;
                dai = a as DoubleArrowInfo;
                if (sai != null && sai.PointTo == cur)
                {
                    if (todo.Contains(sai))
                        reflowRecurse(todo, done, taken, sai, cur.X - sai.Direction.XOffset() * sai.Distance, cur.Y - sai.Direction.YOffset() * sai.Distance);
                    else if (sai.X != cur.X - sai.Direction.XOffset() * sai.Distance || sai.Y != cur.Y - sai.Direction.YOffset() * sai.Distance)
                        sai.Warning = "Doesn’t join up.";
                }
                else if (dai != null)
                {
                    if (todo.Contains(dai))
                    {
                        if (dai.PointTo1 == cur)
                            reflowRecurse(todo, done, taken, dai, cur.X - dai.Direction.GetDirection1().XOffset() * dai.Distance1, cur.Y - dai.Direction.GetDirection1().YOffset() * dai.Distance1);
                        if (dai.PointTo2 == cur)
                            reflowRecurse(todo, done, taken, dai, cur.X - dai.Direction.GetDirection2().XOffset() * dai.Distance2, cur.Y - dai.Direction.GetDirection2().YOffset() * dai.Distance2);
                    }
                    else
                    {
                        if (dai.PointTo1 == cur && (dai.X != cur.X - dai.Direction.GetDirection1().XOffset() * dai.Distance1 || dai.Y != cur.Y - dai.Direction.GetDirection1().YOffset() * dai.Distance1))
                            dai.Warning = "Doesn’t join up (1).";
                        if (dai.PointTo2 == cur && (dai.X != cur.X - dai.Direction.GetDirection2().XOffset() * dai.Distance2 || dai.Y != cur.Y - dai.Direction.GetDirection2().YOffset() * dai.Distance2))
                            dai.Warning = "Doesn’t join up (2).";
                    }
                }
            }
        }

        private void changeDistance(object sender, EventArgs __)
        {
            var changeBy = sender == miDecreaseDistance || sender == miDecrease2ndDistance ? -1 : 1;
            if (sender == miIncreaseDistance || sender == miDecreaseDistance)
            {
                foreach (var item in ctList.SelectedItems.OfType<SingleArrowInfo>())
                    item.Distance += changeBy;
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    item.Distance1 += changeBy;
            }
            if (sender == miIncrease2ndDistance || sender == miDecrease2ndDistance)
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    item.Distance2 += changeBy;
            ctList.RefreshItems();
        }

        private void pointTo(object sender, EventArgs __)
        {
            ArrowInfo pointTo = null;
            foreach (var item in sender == miPointTo
                ? ctList.SelectedItems.Cast<ArrowInfo>().Select(a => a is DoubleArrowInfo ? ((DoubleArrowInfo) a).PointTo1 : ((SingleArrowInfo) a).PointTo)
                : ctList.SelectedItems.OfType<DoubleArrowInfo>().Select(a => a.PointTo2))
                pointTo = pointTo ?? item;
            var pointToCoords = pointTo == null ? null : pointTo.CoordsString;

            tryAgain:
            pointToCoords = InputBox.GetLine("Coordinates of arrow to point to (as “x, y”)?", pointToCoords, "Point to");
            if (pointToCoords == null)
                return;

            if (pointToCoords == "")
            {
                if (sender == miPointTo)
                {
                    foreach (var item in ctList.SelectedItems.OfType<SingleArrowInfo>())
                        item.PointTo = null;
                    foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                        item.PointTo1 = null;
                }
                else
                    foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                        item.PointTo2 = null;
                refresh();
                return;
            }

            Match m;
            if (!(m = Regex.Match(pointToCoords, @"^\s*(\d+)\s*,\s*(\d+)\s*$")).Success)
            {
                var result = DlgMessage.Show("You didn’t type a valid set of coordinates (two numbers separated by a comma).", "Error", DlgType.Error, "&Try again", "&Cancel");
                if (result == 0)
                    goto tryAgain;
                return;
            }
            var x = int.Parse(m.Groups[1].Value);
            var y = int.Parse(m.Groups[2].Value);

            var target = ctList.Items.Cast<ArrowInfo>().FirstOrDefault(a => a.X == x && a.Y == y);
            if (target == null)
            {
                var result = DlgMessage.Show("There is no arrow with those coordinates.", "Error", DlgType.Error, "&Try again", "&Cancel");
                if (result == 0)
                    goto tryAgain;
                return;
            }

            if (sender == miPointTo)
            {
                foreach (var item in ctList.SelectedItems.OfType<SingleArrowInfo>())
                    item.PointTo = target;
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    item.PointTo1 = target;
            }
            else
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    item.PointTo2 = target;

            refresh();
        }

        private bool _requireRefresh = false;

        private void refresh()
        {
            _requireRefresh = true;
            new Thread(() =>
            {
                Thread.Sleep(100);
                tryAgain:
                try
                {
                    Invoke(new Action(() =>
                    {
                        if (_requireRefresh)
                        {
                            _requireRefresh = false;
                            ctImage.Refresh();
                            ctList.RefreshItems();
                        }
                    }));
                }
                catch (NullReferenceException)
                {
                    Thread.Sleep(1000);
                    goto tryAgain;
                }
            }).Start();
        }

        private void save(object _ = null, EventArgs __ = null)
        {
            foreach (var sai in ctList.Items.OfType<SingleArrowInfo>())
                if (sai.PointTo != null && !ctList.Items.Contains(sai.PointTo))
                    sai.PointTo = null;
            foreach (var dai in ctList.Items.OfType<DoubleArrowInfo>())
            {
                if (dai.PointTo1 != null && !ctList.Items.Contains(dai.PointTo1))
                    dai.PointTo1 = null;
                if (dai.PointTo2 != null && !ctList.Items.Contains(dai.PointTo2))
                    dai.PointTo2 = null;
            }

            ZiimHelperProgram.Settings.Arrows = ctList.Items.Cast<ArrowInfo>().ToList();
            ZiimHelperProgram.Settings.SelectedIndices = ctList.SelectedIndices.Cast<int>().ToList();
            ZiimHelperProgram.Settings.OutlineIndex = ctList.OutlineIndex;
            ZiimHelperProgram.Settings.ViewGrid = miGrid.Checked;
            ZiimHelperProgram.Settings.ViewConnectionLines = miConnectionLines.Checked;
            ZiimHelperProgram.Settings.ViewInstructions = miInstructions.Checked;
            ZiimHelperProgram.Settings.ViewAnnotations = miAnnotations.Checked;
            ZiimHelperProgram.Settings.Save(onFailure: SettingsOnFailure.ShowRetryOnly);
        }

        private void moveArrow(object sender, EventArgs __)
        {
            var xOffset = sender == miMoveLeft ? -1 : sender == miMoveRight ? 1 : 0;
            var yOffset = sender == miMoveUp ? -1 : sender == miMoveDown ? 1 : 0;
            foreach (var item in ctList.SelectedItems.Cast<ArrowInfo>())
            {
                item.X += xOffset;
                item.Y += yOffset;
            }
            refresh();
        }

        private void selectedIndexChanged(object _, EventArgs __)
        {
            if (!_suppressRepaint)
                ctImage.Invalidate();
        }

        private void sort(object _, EventArgs __)
        {
            _suppressRepaint = true;
            var items = ctList.Items.Cast<ArrowInfo>().ToList();
            var selectedItems = ctList.SelectedItems.Cast<ArrowInfo>().ToList();
            ctList.Items.Clear();
            items.Sort((a1, a2) => new Func<int, int>(x => x != 0 ? x : a1.X.CompareTo(a2.X))(a1.Y.CompareTo(a2.Y)));
            foreach (var item in items)
                ctList.Items.Add(item);
            foreach (var item in selectedItems)
                ctList.SelectedItems.Add(item);
            _suppressRepaint = false;
            refresh();
        }

        private Point? _imageMouseDown;
        private Point _imageMouseDraggedTo;

        private void imageMouseDown(object sender, MouseEventArgs e)
        {
            if (_paintCellSize < 1)
                return;
            _imageMouseDraggedTo = new Point((e.X - _paintTarget.Left) / _paintCellSize, (e.Y - _paintTarget.Top) / _paintCellSize);
            _imageMouseDown = _imageMouseDraggedTo;
            ctImage.Invalidate();
        }

        private void imageMouseMove(object sender, MouseEventArgs e)
        {
            if (_paintCellSize < 1 || _imageMouseDown == null)
                return;
            _imageMouseDraggedTo = new Point((e.X - _paintTarget.Left) / _paintCellSize, (e.Y - _paintTarget.Top) / _paintCellSize);
            ctImage.Invalidate();
        }

        private void imageMouseUp(object sender, MouseEventArgs e)
        {
            if (_paintCellSize < 1 || _imageMouseDown == null)
                return;
            var minX = Math.Min(_imageMouseDown.Value.X, _imageMouseDraggedTo.X);
            var minY = Math.Min(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y);
            var maxX = Math.Max(_imageMouseDown.Value.X, _imageMouseDraggedTo.X);
            var maxY = Math.Max(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y);
            _imageMouseDown = null;
            _suppressRepaint = true;
            if (!Ut.Shift)
                ctList.ClearSelected();
            foreach (var item in ctList.Items.Cast<ArrowInfo>().ToList())
                if (item.X - _paintMinX >= minX && item.X - _paintMinX <= maxX && item.Y - _paintMinY >= minY && item.Y - _paintMinY <= maxY && !ctList.SelectedItems.Contains(item))
                    ctList.SelectedItems.Add(item);
            _suppressRepaint = false;
            ctImage.Invalidate();
        }

        private void revert(object sender = null, EventArgs __ = null)
        {
            if (sender != null)
                SettingsUtil.LoadSettings(out ZiimHelperProgram.Settings);
            ctList.Items.Clear();
            foreach (var arr in ZiimHelperProgram.Settings.Arrows)
                ctList.Items.Add(arr);
            foreach (var index in ZiimHelperProgram.Settings.SelectedIndices)
                ctList.SelectedIndices.Add(index);

            miGrid.Checked = ZiimHelperProgram.Settings.ViewGrid;
            miConnectionLines.Checked = ZiimHelperProgram.Settings.ViewConnectionLines;
            miInstructions.Checked = ZiimHelperProgram.Settings.ViewInstructions;
            miAnnotations.Checked = ZiimHelperProgram.Settings.ViewAnnotations;
            if (ZiimHelperProgram.Settings.OutlineIndex >= 0 && ZiimHelperProgram.Settings.OutlineIndex < ctList.Items.Count)
                ctList.OutlineIndex = ZiimHelperProgram.Settings.OutlineIndex;

            refresh();
        }

        private sealed class arrowException : Exception
        {
            public ArrowInfo Arrow { get; private set; }
            public arrowException(ArrowInfo arrow, string message) : base(message) { Arrow = arrow; }
        }

        private void adjustDistances(object _, EventArgs __)
        {
            var actions = new List<Action>();

            try
            {
                foreach (var arrow in ctList.SelectedItems.Cast<ArrowInfo>())
                {
                    if (arrow is SingleArrowInfo)
                    {
                        var sai = (SingleArrowInfo) arrow;
                        if (sai.PointTo != null)
                        {
                            if (sai.Direction.XOffset() == 0 || sai.Direction.YOffset() == 0 || (sai.PointTo.X - sai.X) / sai.Direction.XOffset() == (sai.PointTo.Y - sai.Y) / sai.Direction.YOffset())
                                actions.Add(() => { sai.Distance = sai.Direction.XOffset() == 0 ? (sai.PointTo.Y - sai.Y) / sai.Direction.YOffset() : (sai.PointTo.X - sai.X) / sai.Direction.XOffset(); });
                            else
                                throw new arrowException(sai, "Arrow “{0}” is not aligned with its direction.".Fmt(arrow.CoordsString));
                        }
                    }
                    else
                    {
                        var dai = (DoubleArrowInfo) arrow;
                        if (dai.PointTo1 != null)
                        {
                            if (dai.Direction.GetDirection1().XOffset() == 0 || dai.Direction.GetDirection1().YOffset() == 0 || (dai.PointTo1.X - dai.X) / dai.Direction.GetDirection1().XOffset() == (dai.PointTo1.Y - dai.Y) / dai.Direction.GetDirection1().YOffset())
                                actions.Add(() => { dai.Distance1 = dai.Direction.GetDirection1().XOffset() == 0 ? (dai.PointTo1.Y - dai.Y) / dai.Direction.GetDirection1().YOffset() : (dai.PointTo1.X - dai.X) / dai.Direction.GetDirection1().XOffset(); });
                            else
                                throw new arrowException(dai, "Arrow at {0} is not aligned with its direction.".Fmt(arrow.CoordsString));
                        }
                        if (dai.PointTo2 != null)
                        {
                            if (dai.Direction.GetDirection2().XOffset() == 0 || dai.Direction.GetDirection2().YOffset() == 0 || (dai.PointTo2.X - dai.X) / dai.Direction.GetDirection2().XOffset() == (dai.PointTo2.Y - dai.Y) / dai.Direction.GetDirection2().YOffset())
                                actions.Add(() => { dai.Distance2 = dai.Direction.GetDirection2().XOffset() == 0 ? (dai.PointTo2.Y - dai.Y) / dai.Direction.GetDirection2().YOffset() : (dai.PointTo2.X - dai.X) / dai.Direction.GetDirection2().XOffset(); });
                            else
                                throw new arrowException(dai, "Arrow at {0} is not aligned with its direction.".Fmt(arrow.CoordsString));
                        }
                    }
                }
            }
            catch (arrowException e)
            {
                var result = DlgMessage.Show(e.Message, "Error", DlgType.Error, "&Go to that arrow", "&Cancel");
                if (result == 0)
                {
                    ctList.SelectedItems.Clear();
                    ctList.SelectedItems.Add(e.Arrow);
                }
                return;
            }

            foreach (var action in actions)
                action();
            refresh();
        }

        private void toggleMark(object sender, EventArgs e)
        {
            foreach (var arrow in ctList.SelectedItems.Cast<ArrowInfo>())
                arrow.Marked = !arrow.Marked;
            refresh();
        }

        private void copySource(object sender, EventArgs e)
        {
            if (ctList.Items.Count == 0)
            {
                Clipboard.SetText("");
                return;
            }

            var minX = ctList.Items.Cast<ArrowInfo>().Min(a => a.X);
            var minY = ctList.Items.Cast<ArrowInfo>().Min(a => a.Y);
            var maxX = ctList.Items.Cast<ArrowInfo>().Max(a => a.X);
            var maxY = ctList.Items.Cast<ArrowInfo>().Max(a => a.Y);

            var arr = Ut.NewArray<char>(maxY - minY + 1, maxX - minX + 1, (i, j) => ' ');
            foreach (var arrow in ctList.Items.Cast<ArrowInfo>())
                arr[arrow.Y - minY][arrow.X - minX] = arrow.Arrow;
            Clipboard.SetText(arr.Select(row => " " + row.JoinString()).JoinString(Environment.NewLine));
        }

        private void copyImage(object sender, EventArgs __)
        {
            tryAgain:
            var inputStr = InputBox.GetLine(
                sender == miCopyImageByFont ? "Specify the desired font size:" :
                sender == miCopyImageByWidth ? "Specify the desired bitmap width:" :
                sender == miCopyImageByHeight ? "Specify the desired bitmap height:" : Ut.Throw<string>(new InvalidOperationException()),

                sender == miCopyImageByFont ? "19" :
                sender == miCopyImageByWidth ? "1000" :
                sender == miCopyImageByHeight ? "1000" : Ut.Throw<string>(new InvalidOperationException()),

                "Copy image", "&OK", "&Cancel"
            );
            if (inputStr == null)
                return;
            int input;
            if (!int.TryParse(inputStr, out input))
            {
                var result = DlgMessage.Show("The specified input is not a valid integer.", "Error", DlgType.Error, "&Try again", "&Cancel");
                if (result == 0)
                    goto tryAgain;
                return;
            }

            if (ctList.Items.Count == 0)
            {
                using (var tmpBmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
                    Clipboard.SetImage(tmpBmp);
                return;
            }

            float fontSize;
            int cellWidth = 0;
            using (var tmpBmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(tmpBmp))
            {
                if (sender == miCopyImageByFont)
                    fontSize = input;
                else if (sender == miCopyImageByWidth || sender == miCopyImageByHeight)
                {
                    float low = 1;
                    float? high = null;
                    while (high == null || high.Value - low > 0.1)
                    {
                        var trySize = high == null ? low + 1024 : (low + high.Value) / 2;
                        var tryFont = new Font(_arrowFont, trySize, FontStyle.Bold);
                        var size = "↖↑↗→↘↓↙←↕⤢↔⤡"
                            .Select(ch => g.MeasureString(ch.ToString(), tryFont))
                            .Max(sz => sender == miCopyImageByWidth ? sz.Width : sz.Height);
                        if (size * (sender == miCopyImageByWidth ? _paintMaxX - _paintMinX + 1 : _paintMaxY - _paintMinY + 1) > input)
                            high = trySize;
                        else
                            low = trySize;
                    }
                    fontSize = low;
                }
                else
                    throw new InvalidOperationException();

                var font = new Font(_arrowFont, fontSize, FontStyle.Bold);
                foreach (var ch in "↖↑↗→↘↓↙←↕⤢↔⤡")
                    cellWidth = Math.Max(cellWidth, (int) Math.Floor(g.MeasureString(ch.ToString(), font).Width));
            }

            using (var bmpReal = new Bitmap(cellWidth * (_paintMaxX - _paintMinX + 1), cellWidth * (_paintMaxY - _paintMinY + 1), PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmpReal))
            {
                g.Clear(Color.White);
                paintInto(g, cellWidth, fontSize);
                Clipboard.SetImage(bmpReal);
            }
        }

        private void annotate(object sender, EventArgs e)
        {
            var annotation = ctList.SelectedItems.OfType<ArrowInfo>().Select(arr => arr.Annotation).JoinString();
            var newAnnotation = InputBox.GetLine("Annotation:", annotation, "Annotation", "&OK", "&Cancel");
            if (newAnnotation != null)
            {
                foreach (var arr in ctList.SelectedItems.OfType<ArrowInfo>())
                    arr.Annotation = newAnnotation;
                refresh();
            }
        }

        private void toggleViewOption(object sender, EventArgs __)
        {
            var tsmi = sender as ToolStripMenuItem;
            tsmi.Checked = !tsmi.Checked;
            refresh();
        }
    }
}
