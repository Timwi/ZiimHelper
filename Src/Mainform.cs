using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Forms;
using System.Drawing;
using RT.Util.Drawing;
using RT.Util;
using System.Drawing.Drawing2D;

namespace ZiimHelper
{
    public partial class Mainform : ManagedForm
    {
        public Mainform()
            : base(ZiimHelperProgram.Settings.FormSettings)
        {
            InitializeComponent();
            revert();
        }

        private void newArrow(object sender, EventArgs __)
        {
            var nameGen = new NameGenerator();
            var names = new HashSet<string>(ctList.Items.Cast<ArrowInfo>().Select(a => a.Name));
            string newName;
            do
                newName = nameGen.NextName();
            while (names.Contains(newName));
            var arr = sender == miNewSingleArrow ? (ArrowInfo) new SingleArrowInfo { Name = newName } : new DoubleArrowInfo { Name = newName };

            bool done = false;
            if (ctList.SelectedItems.Count == 1)
            {
                var sel = ctList.SelectedItems[0];
                var sai = sel as SingleArrowInfo;
                var dai = sel as DoubleArrowInfo;
                if (sai != null && sai.PointTo == null)
                {
                    arr.X = sai.X + sai.Direction.XOffset() * sai.Distance;
                    arr.Y = sai.Y + sai.Direction.YOffset() * sai.Distance;
                    sai.PointTo = newName;
                    done = true;
                }
                else if (dai != null)
                {
                    if (dai.PointTo1 == null)
                    {
                        arr.X = dai.X + dai.Direction.GetDirection1().XOffset() * dai.Distance1;
                        arr.Y = dai.Y + dai.Direction.GetDirection1().YOffset() * dai.Distance1;
                        dai.PointTo1 = newName;
                        done = true;
                    }
                    else if (dai.PointTo2 == null)
                    {
                        arr.X = dai.X + dai.Direction.GetDirection2().XOffset() * dai.Distance2;
                        arr.Y = dai.Y + dai.Direction.GetDirection2().YOffset() * dai.Distance2;
                        dai.PointTo2 = newName;
                        done = true;
                    }
                }
            }

            if (!done)
            {
                var already = new Dictionary<int, Dictionary<int, ArrowInfo>>();
                foreach (var item in ctList.Items.Cast<ArrowInfo>())
                    already.AddSafe(item.X, item.Y, item);

                while (already.ContainsKey(arr.X) && already[arr.X].ContainsKey(arr.Y))
                {
                    arr.X += Rnd.Next(5) - 2;
                    arr.Y += Rnd.Next(5) - 2;
                }
            }

            var o = ctList.OutlineIndex;
            ctList.Items.Insert(o, arr);
            ctList.ClearSelected();
            ctList.SelectedIndex = o;
            ctImage.Refresh();
        }

        private void deleteArrow(object _, EventArgs __)
        {
            if (ctList.SelectedIndices.Count == 0)
                return;
            var s = ctList.SelectedIndices.Cast<int>().Order().First();
            foreach (var a in ctList.SelectedItems.Cast<ArrowInfo>().ToList())
                ctList.Items.Remove(a);
            if (ctList.Items.Count > 0)
            {
                ctList.ClearSelected();
                ctList.SelectedIndex = s >= ctList.Items.Count ? s - 1 : s;
            }
            ctImage.Refresh();
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

        private void rename(object _, EventArgs __)
        {
            if (ctList.SelectedIndices.Count != 1)
                return;
            var s = ctList.SelectedIndex;
            var item = (ArrowInfo) ctList.Items[s];
            var newName = item.Name;

            tryAgain:
            newName = InputBox.GetLine("New name:", newName, "Rename arrow");
            if (newName == null)
                return;

            var already = ctList.Items.Cast<ArrowInfo>().FirstOrDefault(a => a.Name == newName);
            if (already != null)
            {
                switch (DlgMessage.Show("This name is already in use.", "Rename", DlgType.Error, "&Rename again", "&Go to arrow with this name", "&Cancel"))
                {
                    case 0:     // Rename again
                        goto tryAgain;

                    case 1:     // Go to arrow
                        ctList.ClearSelected();
                        ctList.SelectedItem = already;
                        return;

                    default:    // Cancel
                        return;
                }
            }

            foreach (var it in ctList.Items.OfType<SingleArrowInfo>())
                if (it.PointTo == item.Name)
                    it.PointTo = newName;
            foreach (var it in ctList.Items.OfType<DoubleArrowInfo>())
            {
                if (it.PointTo1 == item.Name)
                    it.PointTo1 = newName;
                if (it.PointTo2 == item.Name)
                    it.PointTo2 = newName;
            }
            item.Name = newName;
            refresh();
        }

        private void paint(object _, PaintEventArgs e)
        {
            if (_paintCellSize < 1)
                return;
            e.Graphics.SetHighQuality();
            foreach (var arr in ctList.SelectedItems.Cast<ArrowInfo>())
                e.Graphics.DrawEllipse(new Pen(Brushes.Red, 2),
                    _paintTarget.Left + (arr.X - _paintMinX) * _paintCellSize + _paintCellSize / 8,
                    _paintTarget.Top + (arr.Y - _paintMinY) * _paintCellSize + _paintCellSize / 8,
                    _paintCellSize * 6 / 8, _paintCellSize * 6 / 8);

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

        private void paintBuffer(object _, PaintEventArgs e)
        {
            var margin = 15;

            e.Graphics.Clear(Color.Gray);
            if (ctList.Items.Count < 1)
                return;

            _paintMinX = ctList.Items.Cast<ArrowInfo>().Min(a => a.X);
            _paintMinY = ctList.Items.Cast<ArrowInfo>().Min(a => a.Y);
            _paintMaxX = ctList.Items.Cast<ArrowInfo>().Max(a => a.X);
            _paintMaxY = ctList.Items.Cast<ArrowInfo>().Max(a => a.Y);
            _paintTarget = new Size(_paintMaxX - _paintMinX + 1, _paintMaxY - _paintMinY + 1).FitIntoMaintainAspectRatio(new Rectangle(margin, margin, ctImage.ClientSize.Width - 2 * margin, ctImage.ClientSize.Height - 2 * margin));

            e.Graphics.SetHighQuality();
            e.Graphics.FillRectangle(Brushes.White, _paintTarget);
            e.Graphics.DrawRectangle(Pens.Black, _paintTarget);

            _paintCellSize = _paintTarget.Width / (_paintMaxX - _paintMinX + 1);

            for (int i = 1; i <= _paintMaxX - _paintMinX; i++)
                e.Graphics.DrawLine(Pens.Black, _paintTarget.Left + i * _paintCellSize, _paintTarget.Top, _paintTarget.Left + i * _paintCellSize, _paintTarget.Bottom);
            for (int j = 1; j <= _paintMaxY - _paintMinY; j++)
                e.Graphics.DrawLine(Pens.Black, _paintTarget.Left, _paintTarget.Top + j * _paintCellSize, _paintTarget.Right, _paintTarget.Top + j * _paintCellSize);

            var arrowsByName = ctList.Items.Cast<ArrowInfo>().ToDictionary(a => a.Name);
            var hitFromDic = new Dictionary<ArrowInfo, List<Direction>>();

            foreach (var arr in ctList.Items.Cast<ArrowInfo>())
            {
                var fontSize = e.Graphics.GetMaximumFontSize(new SizeF(_paintCellSize, _paintCellSize), ctImage.Font.FontFamily, arr.Arrow);
                e.Graphics.DrawString(arr.Arrow, new Font(ctImage.Font.FontFamily, fontSize), Brushes.Black, _paintTarget.Left + (arr.X - _paintMinX) * _paintCellSize + _paintCellSize / 2, _paintTarget.Top + (arr.Y - _paintMinY) * _paintCellSize + _paintCellSize / 2,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                drawInRoundedRectangle(e.Graphics, arr.Name, new PointF(_paintTarget.Left + (arr.X - _paintMinX) * _paintCellSize + _paintCellSize / 2, _paintTarget.Top + (arr.Y - _paintMinY) * _paintCellSize), Color.FromArgb(0xEE, 0xEE, 0xFF), Color.Blue, Color.DarkBlue);
                if (arr.Warning != null)
                    drawInRoundedRectangle(e.Graphics, arr.Warning, new PointF(_paintTarget.Left + (arr.X - _paintMinX) * _paintCellSize + _paintCellSize / 2, _paintTarget.Top + (arr.Y - _paintMinY) * _paintCellSize + _paintCellSize * 4 / 5), Color.FromArgb(0xFF, 0xEE, 0xEE), Color.Red, Color.DarkRed);

                var sai = arr as SingleArrowInfo;
                var dai = arr as DoubleArrowInfo;
                ArrowInfo targetArr;
                if (sai != null && sai.PointTo != null && arrowsByName.TryGetValue(sai.PointTo, out targetArr))
                {
                    hitFromDic.AddSafe(targetArr, sai.Direction);
                    drawHit(e.Graphics, sai.X - _paintMinX, sai.Y - _paintMinY, sai.Direction.XOffset(), sai.Direction.YOffset(), targetArr.X - _paintMinX, targetArr.Y - _paintMinY, _paintCellSize, _paintTarget.Left, _paintTarget.Top);
                }
                else if (dai != null)
                {
                    if (dai.PointTo1 != null && arrowsByName.TryGetValue(dai.PointTo1, out targetArr))
                    {
                        hitFromDic.AddSafe(targetArr, dai.Direction.GetDirection1());
                        drawHit(e.Graphics, dai.X - _paintMinX, dai.Y - _paintMinY, dai.Direction.GetDirection1().XOffset(), dai.Direction.GetDirection1().YOffset(), targetArr.X - _paintMinX, targetArr.Y - _paintMinY, _paintCellSize, _paintTarget.Left, _paintTarget.Top);
                    }
                    if (dai.PointTo2 != null && arrowsByName.TryGetValue(dai.PointTo2, out targetArr))
                    {
                        hitFromDic.AddSafe(targetArr, dai.Direction.GetDirection2());
                        drawHit(e.Graphics, dai.X - _paintMinX, dai.Y - _paintMinY, dai.Direction.GetDirection2().XOffset(), dai.Direction.GetDirection2().YOffset(), targetArr.X - _paintMinX, targetArr.Y - _paintMinY, _paintCellSize, _paintTarget.Left, _paintTarget.Top);
                    }
                }
            }

            foreach (var kvp in hitFromDic)
            {
                var arr = kvp.Key;
                var smallFontSize = e.Graphics.GetMaximumFontSize(new SizeF(_paintCellSize / 3, _paintCellSize / 3), ctImage.Font.FontFamily, arr.Arrow);
                //new Font(ctImage.Font.FontFamily, ctImage.Font.Size * .75f)
                foreach (var dir in kvp.Value)
                    e.Graphics.DrawString(dir.ToStringExt(), new Font(ctImage.Font.FontFamily, smallFontSize), Brushes.DarkGreen,
                        _paintTarget.Left + (arr.X - _paintMinX) * _paintCellSize + _paintCellSize / 2 - _paintCellSize * dir.XOffset() * 3 / 10,
                        _paintTarget.Top + (arr.Y - _paintMinY) * _paintCellSize + _paintCellSize / 2 - _paintCellSize * dir.YOffset() * 3 / 10,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void drawHit(Graphics g, int fromX, int fromY, int dirX, int dirY, int toX, int toY, int cellSize, int left, int top)
        {
            g.DrawLine(new Pen(new SolidBrush(Color.Orange)) { EndCap = LineCap.ArrowAnchor },
                left + cellSize * fromX + cellSize / 2 + dirX * cellSize / 4, top + cellSize * fromY + cellSize / 2 + dirY * cellSize / 4,
                left + cellSize * toX + cellSize / 2 - dirX * cellSize / 4, top + cellSize * toY + cellSize / 2 - dirY * cellSize / 4);
        }

        private void drawInRoundedRectangle(Graphics g, string text, PointF location, Color background, Color outline, Color textColor)
        {
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
            var size = g.MeasureString(text, ctImage.Font, int.MaxValue, sf) + new SizeF(10, 10);
            var realLocation = location - new SizeF(size.Width / 2, 10);
            var path = GraphicsUtil.RoundedRectangle(new RectangleF(realLocation, size), 5);
            g.FillPath(new SolidBrush(background), path);
            g.DrawPath(new Pen(outline, 2), path);
            g.DrawString(text, ctImage.Font, new SolidBrush(textColor), new RectangleF(realLocation + new SizeF(5, 5), size - new SizeF(10, 10)), sf);
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
                var todo = ctList.Items.Cast<ArrowInfo>().ToDictionary(a => a.Name);
                var done = new Dictionary<string, ArrowInfo>();
                var taken = new Dictionary<int, Dictionary<int, string>>();

                var node = ctList.SelectedItems.Cast<ArrowInfo>().FirstOrDefault() ?? todo.First().Value;
                while (true)
                {
                    reflowRecurse(todo, done, taken, node, node.X, node.Y);
                    if (todo.Count == 0)
                        break;
                    node = todo.First().Value;
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
                Action<ArrowInfo, Direction, string> check = (item, dir, msg) =>
                {
                    var xoff = dir.XOffset();
                    var yoff = dir.YOffset();
                    int x = item.X, y = item.Y;
                    do
                    {
                        x += xoff;
                        y += yoff;
                        if (taken.ContainsKey(x) && taken[x].ContainsKey(y))
                            item.Warning = msg;
                    }
                    while (x > minX && x < maxX && y > minY && y < maxY);
                };
                foreach (var item in ctList.Items.Cast<ArrowInfo>())
                {
                    SingleArrowInfo sai;
                    DoubleArrowInfo dai;
                    if ((sai = item as SingleArrowInfo) != null && sai.PointTo == null)
                        check(item, sai.Direction, "Points to arrow.");
                    else if ((dai = item as DoubleArrowInfo) != null)
                    {
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

        private void reflowRecurse(Dictionary<string, ArrowInfo> todo, Dictionary<string, ArrowInfo> done, Dictionary<int, Dictionary<int, string>> taken, ArrowInfo cur, int x, int y)
        {
            cur.Warning = taken.ContainsKey(x) && taken[x].ContainsKey(y) ? taken[x][y] : null;
            cur.X = x;
            cur.Y = y;
            todo.Remove(cur.Name);
            done[cur.Name] = cur;

            Action<int, int, int, string> setWarning = (xOffset, yOffset, distance, warning) =>
            {
                for (int i = 0; i < distance; i++)
                {
                    foreach (var don in done.Values)
                        if (don != cur && don.X == x + xOffset * i && don.Y == y + yOffset * i)
                            don.Warning = don.Warning.AddLine(warning);
                    taken.AddSafe(x + xOffset * i, y + yOffset * i, warning);
                }
            };

            var sai = cur as SingleArrowInfo;
            var dai = cur as DoubleArrowInfo;
            ArrowInfo target;

            if (sai != null && sai.PointTo != null)
            {
                var xOffset = sai.Direction.XOffset();
                var yOffset = sai.Direction.YOffset();
                setWarning(xOffset, yOffset, sai.Distance, "Cell taken by {0}.".Fmt(sai.Name));

                if (todo.TryGetValue(sai.PointTo, out target))
                    reflowRecurse(todo, done, taken, target, x + xOffset * sai.Distance, y + yOffset * sai.Distance);
                else if (done.TryGetValue(sai.PointTo, out target))
                {
                    if (target.X != x + sai.Direction.XOffset() * sai.Distance || target.Y != y + sai.Direction.YOffset() * sai.Distance)
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
                    setWarning(xOffset, yOffset, dai.Distance1, "Cell taken by {0}.".Fmt(dai.Name));

                    if (todo.TryGetValue(dai.PointTo1, out target))
                        reflowRecurse(todo, done, taken, target, x + xOffset * dai.Distance1, y + yOffset * dai.Distance1);
                    else if (done.TryGetValue(dai.PointTo1, out target))
                    {
                        if (target.X != x + dai.Direction.GetDirection1().XOffset() * dai.Distance1 || target.Y != y + dai.Direction.GetDirection1().YOffset() * dai.Distance1)
                            cur.Warning = cur.Warning.AddLine("Doesn’t join up (1).");
                    }
                    else
                        cur.Warning = cur.Warning.AddLine("Points into nirvana (1).");
                }

                if (dai.PointTo2 != null)
                {
                    var xOffset = dai.Direction.GetDirection2().XOffset();
                    var yOffset = dai.Direction.GetDirection2().YOffset();
                    setWarning(xOffset, yOffset, dai.Distance2, "Cell taken by {0}.".Fmt(dai.Name));

                    if (todo.TryGetValue(dai.PointTo2, out target))
                        reflowRecurse(todo, done, taken, target, x + xOffset * dai.Distance2, y + yOffset * dai.Distance2);
                    else if (done.TryGetValue(dai.PointTo2, out target))
                    {
                        if (target.X != x + dai.Direction.GetDirection2().XOffset() * dai.Distance2 || target.Y != y + dai.Direction.GetDirection2().YOffset() * dai.Distance2)
                            cur.Warning = cur.Warning.AddLine("Doesn’t join up (2).");
                    }
                    else
                        cur.Warning = cur.Warning.AddLine("Points into nirvana (2).");
                }
            }

            foreach (var a in todo.Values.Concat(done.Values).ToList())
            {
                sai = a as SingleArrowInfo;
                dai = a as DoubleArrowInfo;
                if (sai != null && todo.ContainsKey(sai.Name) && sai.PointTo == cur.Name)
                    reflowRecurse(todo, done, taken, sai, cur.X - sai.Direction.XOffset() * sai.Distance, cur.Y - sai.Direction.YOffset() * sai.Distance);
                else if (sai != null && sai.PointTo == cur.Name)
                {
                    if (sai.X != cur.X - sai.Direction.XOffset() * sai.Distance || sai.Y != cur.Y - sai.Direction.YOffset() * sai.Distance)
                        sai.Warning = "Doesn’t join up.";
                }
                else if (dai != null && todo.ContainsKey(dai.Name))
                {
                    if (dai.PointTo1 == cur.Name)
                        reflowRecurse(todo, done, taken, dai, cur.X - dai.Direction.GetDirection1().XOffset() * dai.Distance1, cur.Y - dai.Direction.GetDirection1().YOffset() * dai.Distance1);
                    if (dai.PointTo2 == cur.Name)
                        reflowRecurse(todo, done, taken, dai, cur.X - dai.Direction.GetDirection2().XOffset() * dai.Distance2, cur.Y - dai.Direction.GetDirection2().YOffset() * dai.Distance2);
                }
                else if (dai != null)
                {
                    if (dai.PointTo1 == cur.Name && (dai.X != cur.X - dai.Direction.GetDirection1().XOffset() * dai.Distance1 || dai.Y != cur.Y - dai.Direction.GetDirection1().YOffset() * dai.Distance1))
                        dai.Warning = "Doesn’t join up (1).";
                    if (dai.PointTo2 == cur.Name && (dai.X != cur.X - dai.Direction.GetDirection2().XOffset() * dai.Distance2 || dai.Y != cur.Y - dai.Direction.GetDirection2().YOffset() * dai.Distance2))
                        dai.Warning = "Doesn’t join up (2).";
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
            string def = null;
            foreach (var item in sender == miPointTo
                ? ctList.SelectedItems.Cast<ArrowInfo>().Select(a => a is DoubleArrowInfo ? ((DoubleArrowInfo) a).PointTo1 : ((SingleArrowInfo) a).PointTo)
                : ctList.SelectedItems.OfType<DoubleArrowInfo>().Select(a => a.PointTo2))
                def = def == null ? item : "";

            tryAgain:
            var i = InputBox.GetLine("Name of arrow to point to?", def, "Point to");
            if (i == null)
                return;

            if (i == "")
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

            var target = ctList.Items.Cast<ArrowInfo>().FirstOrDefault(a => a.Name == i);
            if (target == null)
            {
                var result = DlgMessage.Show("An arrow with that name doesn’t exist.", "Error", DlgType.Error, "&Try again", "&Cancel");
                if (result == 0)
                    goto tryAgain;
                return;
            }

            if (sender == miPointTo)
            {
                foreach (var item in ctList.SelectedItems.OfType<SingleArrowInfo>())
                    item.PointTo = i;
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    item.PointTo1 = i;
            }
            else
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    item.PointTo2 = i;

            refresh();
        }

        private void refresh()
        {
            ctList.RefreshItems();
            ctImage.Refresh();
        }

        private void save(object _ = null, EventArgs __ = null)
        {
            ZiimHelperProgram.Settings.Arrows = ctList.Items.Cast<ArrowInfo>().ToList();
            ZiimHelperProgram.Settings.SelectedIndices = ctList.SelectedIndices.Cast<int>().ToList();
            ZiimHelperProgram.Settings.OutlineIndex = ctList.OutlineIndex;

            SettingsUtil.SaveSettings(ZiimHelperProgram.Settings, SettingsUtil.OnFailure.ShowRetryOnly);
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

        private void sort(object sender, EventArgs __)
        {
            _suppressRepaint = true;
            var items = ctList.Items.Cast<ArrowInfo>().ToList();
            var selectedItems = ctList.SelectedItems.Cast<ArrowInfo>().ToList();
            ctList.Items.Clear();
            if (sender == miSortByCoordinate)
                items.Sort((a1, a2) => new Func<int, int>(x => x != 0 ? x : a1.X.CompareTo(a2.X))(a1.Y.CompareTo(a2.Y)));
            else
                items.Sort((a1, a2) => string.Compare(a1.Name, a2.Name, StringComparison.OrdinalIgnoreCase));
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
            ctList.ClearSelected();
            foreach (var item in ctList.Items.Cast<ArrowInfo>().ToList())
                if (item.X - _paintMinX >= minX && item.X - _paintMinX <= maxX && item.Y - _paintMinY >= minY && item.Y - _paintMinY <= maxY)
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
            if (ZiimHelperProgram.Settings.OutlineIndex >= 0 && ZiimHelperProgram.Settings.OutlineIndex < ctList.Items.Count)
                ctList.OutlineIndex = ZiimHelperProgram.Settings.OutlineIndex;
            refresh();
        }
    }
}
