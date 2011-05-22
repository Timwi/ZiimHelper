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

namespace GraphiteHelper
{
    public partial class Mainform : ManagedForm
    {
        public Mainform()
            : base(GraphiteHelperProgram.Settings.FormSettings)
        {
            InitializeComponent();
            foreach (var arr in GraphiteHelperProgram.Settings.Arrows)
                ctList.Items.Add(arr);
            foreach (var index in GraphiteHelperProgram.Settings.SelectedIndices)
                ctList.SelectedIndex = index;
            if (GraphiteHelperProgram.Settings.OutlineIndex >= 0 && GraphiteHelperProgram.Settings.OutlineIndex < ctList.Items.Count)
                ctList.OutlineIndex = GraphiteHelperProgram.Settings.OutlineIndex;
        }

        private void newArrow(object sender, EventArgs __)
        {
            var already = new Dictionary<int, Dictionary<int, ArrowInfo>>();
            foreach (var item in ctList.Items.Cast<ArrowInfo>())
                already.AddSafe(item.X, item.Y, item);

            int x = 0;
            int y = 0;
            while (already.ContainsKey(x) && already[x].ContainsKey(y))
            {
                x += Rnd.Next(5) - 2;
                y += Rnd.Next(5) - 2;
            }

            var o = ctList.OutlineIndex;
            var nameGen = new NameGenerator();
            var names = new HashSet<string>(ctList.Items.Cast<ArrowInfo>().Select(a => a.Name));
            string newName;
            do
                newName = nameGen.NextName();
            while (names.Contains(newName));
            ctList.Items.Insert(o, sender == miNewSingleArrow ? (ArrowInfo) new SingleArrowInfo { Name = newName, X = x, Y = y } : new DoubleArrowInfo { Name = newName, X = x, Y = y });
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

        private void moveUp(object _, EventArgs __)
        {
            if (ctList.SelectedIndices.Count != 1)
                return;
            var s = ctList.SelectedIndex;
            if (s == 0)
                return;
            var item = ctList.Items[s];
            ctList.Items.RemoveAt(s);
            ctList.Items.Insert(s - 1, item);
            ctList.ClearSelected();
            ctList.SelectedIndex = s - 1;
        }

        private void moveDown(object _, EventArgs __)
        {
            if (ctList.SelectedIndices.Count != 1)
                return;
            var s = ctList.SelectedIndex;
            if (s == ctList.Items.Count - 1)
                return;
            var item = ctList.Items[s];
            ctList.Items.RemoveAt(s);
            ctList.Items.Insert(s + 1, item);
            ctList.ClearSelected();
            ctList.SelectedIndex = s + 1;
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
            ctList.RefreshItems();
            ctImage.Refresh();
        }

        private void formClosing(object _, FormClosingEventArgs __)
        {
            GraphiteHelperProgram.Settings.Arrows = ctList.Items.Cast<ArrowInfo>().ToList();
            GraphiteHelperProgram.Settings.SelectedIndices = ctList.SelectedIndices.Cast<int>().ToList();
            GraphiteHelperProgram.Settings.OutlineIndex = ctList.OutlineIndex;
        }

        private void paintBuffer(object _, PaintEventArgs e)
        {
            var margin = 15;

            e.Graphics.Clear(Color.Gray);
            if (ctList.Items.Count < 1)
                return;

            var minX = ctList.Items.Cast<ArrowInfo>().Min(a => a.X);
            var minY = ctList.Items.Cast<ArrowInfo>().Min(a => a.Y);
            var maxX = ctList.Items.Cast<ArrowInfo>().Max(a => a.X);
            var maxY = ctList.Items.Cast<ArrowInfo>().Max(a => a.Y);
            var target = new Size(maxX - minX + 1, maxY - minY + 1)
                .FitIntoMaintainAspectRatio(new Rectangle(margin, margin, ctImage.ClientSize.Width - 2 * margin, ctImage.ClientSize.Height - 2 * margin));

            e.Graphics.SetHighQuality();
            e.Graphics.FillRectangle(Brushes.White, target);
            e.Graphics.DrawRectangle(Pens.Black, target);

            var cellSize = target.Width / (maxX - minX + 1);

            for (int i = 1; i <= maxX - minX; i++)
                e.Graphics.DrawLine(Pens.Black, target.Left + i * cellSize, target.Top, target.Left + i * cellSize, target.Bottom);
            for (int j = 1; j <= maxY - minY; j++)
                e.Graphics.DrawLine(Pens.Black, target.Left, target.Top + j * cellSize, target.Right, target.Top + j * cellSize);

            var arrowsByName = ctList.Items.Cast<ArrowInfo>().ToDictionary(a => a.Name);
            var hitFromDic = new Dictionary<ArrowInfo, List<Direction>>();

            foreach (var arr in ctList.Items.Cast<ArrowInfo>())
            {
                var fontSize = e.Graphics.GetMaximumFontSize(new SizeF(cellSize, cellSize), ctImage.Font.FontFamily, arr.Arrow);
                e.Graphics.DrawString(arr.Arrow, new Font(ctImage.Font.FontFamily, fontSize), Brushes.Black, target.Left + (arr.X - minX) * cellSize + cellSize / 2, target.Top + (arr.Y - minY) * cellSize + cellSize / 2,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                drawInRoundedRectangle(e.Graphics, arr.Name, new PointF(target.Left + (arr.X - minX) * cellSize + cellSize / 2, target.Top + (arr.Y - minY) * cellSize), Color.FromArgb(0xEE, 0xEE, 0xFF), Color.Blue, Color.DarkBlue);
                if (arr.Warning != null)
                    drawInRoundedRectangle(e.Graphics, arr.Warning, new PointF(target.Left + (arr.X - minX) * cellSize + cellSize / 2, target.Top + (arr.Y - minY) * cellSize + cellSize * 4 / 5), Color.FromArgb(0xFF, 0xEE, 0xEE), Color.Red, Color.DarkRed);

                var sai = arr as SingleArrowInfo;
                var dai = arr as DoubleArrowInfo;
                ArrowInfo targetArr;
                if (sai != null && sai.PointTo != null && arrowsByName.TryGetValue(sai.PointTo, out targetArr))
                    hitFromDic.AddSafe(targetArr, sai.Direction);
                else if (dai != null)
                {
                    if (dai.PointTo1 != null && arrowsByName.TryGetValue(dai.PointTo1, out targetArr))
                        hitFromDic.AddSafe(targetArr, dai.Direction.GetDirection1());
                    if (dai.PointTo2 != null && arrowsByName.TryGetValue(dai.PointTo2, out targetArr))
                        hitFromDic.AddSafe(targetArr, dai.Direction.GetDirection2());
                }
            }

            foreach (var kvp in hitFromDic)
            {
                var arr = kvp.Key;
                //new Font(ctImage.Font.FontFamily, ctImage.Font.Size * .75f)
                foreach (var dir in kvp.Value)
                    e.Graphics.DrawString(dir.ToStringExt(), ctImage.Font, Brushes.DarkGreen,
                        target.Left + (arr.X - minX) * cellSize + cellSize / 2 - cellSize * dir.XOffset() * 2 / 5,
                        target.Top + (arr.Y - minY) * cellSize + cellSize / 2 - cellSize * dir.YOffset() * 2 / 5,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
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

        private void reflow(object sender, EventArgs e)
        {
            try
            {
                var todo = ctList.Items.Cast<ArrowInfo>().ToDictionary(a => a.Name);
                var done = new Dictionary<string, ArrowInfo>();
                var taken = new Dictionary<int, Dictionary<int, string>>();
                while (todo.Count > 0)
                {
                    var first = todo.First().Value;
                    reflowRecurse(todo, done, taken, first, first.X, first.Y);
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

            var sai = cur as SingleArrowInfo;
            var dai = cur as DoubleArrowInfo;
            ArrowInfo target;

            if (sai != null)
            {
                if (sai.PointTo != null && todo.TryGetValue(sai.PointTo, out target))
                {
                    var xOffset = sai.Direction.XOffset();
                    var yOffset = sai.Direction.YOffset();
                    for (int i = 0; i < sai.Distance; i++)
                        taken.AddSafe(x + xOffset * i, y + yOffset * i, "Cell taken by {0}.".Fmt(sai.Name));
                    reflowRecurse(todo, done, taken, target, x + xOffset * sai.Distance, y + yOffset * sai.Distance);
                }
                else if (sai.PointTo != null && done.TryGetValue(sai.PointTo, out target) && (target.X != x + sai.Direction.XOffset() * sai.Distance || target.Y != y + sai.Direction.YOffset() * sai.Distance))
                    cur.Warning = cur.Warning.AddLine("Arrow doesn’t join up.");
                else if (sai.PointTo != null)
                    cur.Warning = cur.Warning.AddLine("Points into nirvana.");
            }
            else if (dai != null)
            {
                if (dai.PointTo1 != null && todo.TryGetValue(dai.PointTo1, out target))
                {
                    var xOffset = dai.Direction.GetDirection1().XOffset();
                    var yOffset = dai.Direction.GetDirection1().YOffset();
                    for (int i = 0; i < dai.Distance1; i++)
                        taken.AddSafe(x + xOffset * i, y + yOffset * i, "Cell taken by {0}.".Fmt(dai.Name));
                    reflowRecurse(todo, done, taken, target, x + xOffset * dai.Distance1, y + yOffset * dai.Distance1);
                }
                else if (dai.PointTo1 != null && done.TryGetValue(dai.PointTo1, out target) && (target.X != x + dai.Direction.GetDirection1().XOffset() * dai.Distance1 || target.Y != y + dai.Direction.GetDirection1().YOffset() * dai.Distance1))
                    cur.Warning = cur.Warning.AddLine("Doesn’t join up (1).");
                else if (dai.PointTo1 != null)
                    cur.Warning = cur.Warning.AddLine("Points into nirvana (1).");

                if (dai.PointTo2 != null && todo.TryGetValue(dai.PointTo2, out target))
                {
                    var xOffset = dai.Direction.GetDirection2().XOffset();
                    var yOffset = dai.Direction.GetDirection2().YOffset();
                    for (int i = 0; i < dai.Distance2; i++)
                        taken.AddSafe(x + xOffset * i, y + yOffset * i, "Cell taken by {0}.".Fmt(dai.Name));
                    reflowRecurse(todo, done, taken, target, x + xOffset * dai.Distance2, y + yOffset * dai.Distance2);
                }
                else if (dai.PointTo2 != null && done.TryGetValue(dai.PointTo2, out target) && (target.X != x + dai.Direction.GetDirection2().XOffset() * dai.Distance2 || target.Y != y + dai.Direction.GetDirection2().YOffset() * dai.Distance2))
                    cur.Warning = cur.Warning.AddLine("Doesn’t join up (2).");
                else if (dai.PointTo2 != null)
                    cur.Warning = cur.Warning.AddLine("Points into nirvana (2).");
            }
            else
                throw new InvalidOperationException();
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
                    {
                        item.PointTo = null;
                        item.Distance = 0;
                    }
                    foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    {
                        item.PointTo1 = null;
                        item.Distance1 = 0;
                    }
                }
                else
                    foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                    {
                        item.PointTo2 = null;
                        item.Distance2 = 0;
                    }
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
                {
                    item.PointTo = i;
                    if (item.Distance == 0)
                        item.Distance = 1;
                }
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                {
                    item.PointTo1 = i;
                    if (item.Distance1 == 0)
                        item.Distance1 = 1;
                }
            }
            else
                foreach (var item in ctList.SelectedItems.OfType<DoubleArrowInfo>())
                {
                    item.PointTo2 = i;
                    if (item.Distance2 == 0)
                        item.Distance2 = 1;
                }

            refresh();
        }

        private void refresh()
        {
            ctList.RefreshItems();
            ctImage.Refresh();
        }

        private void deducePointedTo(object _, EventArgs __)
        {
            /*
            foreach (var item in ctList.SelectedItems.Cast<ArrowInfo>())
                item.HitFrom = ctList.Items.OfType<SingleArrowInfo>().Where(a => a.PointTo == item.Name).Select(a => a.Direction).Concat(
                    ctList.Items.OfType<DoubleArrowInfo>().SelectMany(a =>
                        a.PointTo1 == item.Name ? new[] { a.Direction.GetDirection1() } : Enumerable.Empty<Direction>().Concat(
                        a.PointTo2 == item.Name ? new[] { a.Direction.GetDirection2() } : Enumerable.Empty<Direction>()))).ToArray();
            refresh();
             * */
        }

        private void save(object _, EventArgs __)
        {
            SettingsUtil.SaveSettings(GraphiteHelperProgram.Settings, SettingsUtil.OnFailure.ShowRetryOnly);
        }
    }
}
