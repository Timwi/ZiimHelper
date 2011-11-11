using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.Drawing;
using RT.Util.ExtensionMethods;
using RT.Util.Forms;
using RT.Util.Xml;

namespace ZiimHelper
{
    public partial class Mainform : ManagedForm
    {
        private ZiimFile _file = new ZiimFile();
        private string _filename = null;
        private bool _fileChanged = false;
        private HashSet<Item> _selected = new HashSet<Item>();

        private FontFamily _arrowFont = new FontFamily("Cambria");
        private FontFamily _instructionFont = new FontFamily("Gentium Book Basic");
        private FontFamily _warningFont = new FontFamily("Calibri");

        public Mainform()
            : base(ZiimHelperProgram.Settings.FormSettings)
        {
            InitializeComponent();
            setUi();
            setMode(ZiimHelperProgram.Settings.EditMode);
        }

        private void deleteArrow(object _, EventArgs __)
        {
            if (_selected.Count == 0)
                return;
            _file.Items.RemoveAll(_selected.Contains);
            _selected.Clear();
            refresh();
        }

        private void paint(object _, PaintEventArgs e)
        {
            if (_paintCellSize < 1)
                return;
            e.Graphics.SetHighQuality();

            using (new GraphicsTransformer(e.Graphics).Translate(_paintTarget.Left - _paintMinX * _paintCellSize, _paintTarget.Top - _paintMinY * _paintCellSize))
                foreach (var item in _selected)
                    item.DrawSelected(e.Graphics, _paintCellSize);

            if (_draggingSelectionRectangle)
            {
                var minX = Math.Min(_mouseDown.X, _mouseDraggedTo.X) - _paintMinX;
                var minY = Math.Min(_mouseDown.Y, _mouseDraggedTo.Y) - _paintMinY;
                var maxX = Math.Max(_mouseDown.X, _mouseDraggedTo.X) - _paintMinX;
                var maxY = Math.Max(_mouseDown.Y, _mouseDraggedTo.Y) - _paintMinY;
                var rect = new Rectangle(_paintTarget.Left + _paintCellSize * minX, _paintTarget.Top + _paintCellSize * minY, _paintCellSize * (maxX - minX + 1), _paintCellSize * (maxY - minY + 1));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 32, 128, 192)), rect);
                e.Graphics.DrawRectangle(new Pen(Brushes.Blue, 2), rect);
            }
            else if (_arrowReorienting != null)
            {
                var t = _paintTarget.Location + new Size(-_paintMinX * _paintCellSize, -_paintMinY * _paintCellSize);
                using (var tr = new GraphicsTransformer(e.Graphics).Translate(t.X, t.Y))
                    _arrowReorienting.DrawReorienting(e.Graphics, _paintCellSize);
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
            if (_file.Items.Count < 1)
                return;

            _paintMinX = _file.Items.SelectMany(itm => itm.Arrows).Min(a => a.X);
            _paintMinY = _file.Items.SelectMany(itm => itm.Arrows).Min(a => a.Y);
            _paintMaxX = _file.Items.SelectMany(itm => itm.Arrows).Max(a => a.X);
            _paintMaxY = _file.Items.SelectMany(itm => itm.Arrows).Max(a => a.Y);

            var fit = new Size(_paintMaxX - _paintMinX + 1, _paintMaxY - _paintMinY + 1).FitIntoMaintainAspectRatio(new Rectangle(margin, margin, ctImage.ClientSize.Width - 2 * margin, ctImage.ClientSize.Height - 2 * margin));
            var w = fit.Width - fit.Width % (_paintMaxX - _paintMinX + 1);
            var h = fit.Height - fit.Height % (_paintMaxY - _paintMinY + 1);

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
            var maxSize = Math.Max(_paintMaxX - _paintMinX + 1, _paintMaxY - _paintMinY + 1);

            if (miGrid.Checked)
            {
                for (int i = 1; i <= _paintMaxX - _paintMinX; i++)
                    g.DrawLine(Pens.DarkGray, i * cellSize, 0, i * cellSize, (_paintMaxY - _paintMinY + 1) * cellSize);
                for (int j = 1; j <= _paintMaxY - _paintMinY; j++)
                    g.DrawLine(Pens.DarkGray, 0, j * cellSize, (_paintMaxX - _paintMinX + 1) * cellSize, j * cellSize);
            }

            if (miClouds.Checked)
                using (var tr = new GraphicsTransformer(g).Translate(-_paintMinX * _paintCellSize, -_paintMinY * _paintCellSize))
                    foreach (var cloud in _file.Items.SelectMany(itm => itm.Clouds))
                        cloud.DrawCloud(g, _paintCellSize);

            var hitFromDic = new Dictionary<ArrowInfo, List<Direction>>();

            foreach (var arr in _file.Items.SelectMany(itm => itm.Arrows))
            {
                g.DrawString(arr.Arrow.ToString(), new Font(_arrowFont, fontSize), arr.Marked ? Brushes.Red : Brushes.Black, (arr.X - _paintMinX) * cellSize + cellSize / 2, (arr.Y - _paintMinY) * cellSize + cellSize / 2,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                if (miAnnotations.Checked && arr.Annotation != null)
                    drawInRoundedRectangle(g, arr.Annotation, new PointF((arr.X - _paintMinX) * cellSize + cellSize / 2, (arr.Y - _paintMinY) * cellSize), Color.FromArgb(0xEE, 0xEE, 0xFF), Color.Blue, Color.DarkBlue);

                foreach (var dir in arr.Directions)
                {
                    var pointTo = getPointTo(arr.X, arr.Y, dir.XOffset(), dir.YOffset());
                    if (miConnectionLines.Checked)
                    {
                        var toX = pointTo == null ? arr.X + maxSize * dir.XOffset() : pointTo.X;
                        var toY = pointTo == null ? arr.Y + maxSize * dir.YOffset() : pointTo.Y;
                        while (toX < _paintMinX - 1 || toY < _paintMinY - 1 || toX > _paintMaxX + 1 || toY > _paintMaxY + 1) { toX -= dir.XOffset(); toY -= dir.YOffset(); }
                        g.DrawLine(new Pen(Color.LightGreen) { EndCap = LineCap.ArrowAnchor },
                            cellSize * (arr.X - _paintMinX) + cellSize / 2 + dir.XOffset() * cellSize / 2, cellSize * (arr.Y - _paintMinY) + cellSize / 2 + dir.YOffset() * cellSize / 2,
                            cellSize * (toX - _paintMinX) + cellSize / 2 - dir.XOffset() * cellSize * 4 / 10, cellSize * (toY - _paintMinY) + cellSize / 2 - dir.YOffset() * cellSize * 4 / 10);
                    }
                    if (miInstructions.Checked && pointTo != null)
                        hitFromDic.AddSafe(pointTo, dir);
                }
            }

            if (miInstructions.Checked)
            {
                foreach (var arr in _file.Items.SelectMany(itm => itm.Arrows))
                {
                    var x = (arr.X - _paintMinX) * cellSize;
                    var y = (arr.Y - _paintMinY) * cellSize;

                    var directions = hitFromDic.ContainsKey(arr) ? hitFromDic[arr] : null;

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
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 1) % 8))     // stdin
                                instruction = "s";
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 3) % 8))     // invert
                                instruction = "I";
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 5) % 8))     // no-op
                                instruction = "N";
                            else if (directions.Count == 2 && directions.Contains((Direction) (((int) sai.Direction + 1) % 8)) && directions.Contains((Direction) (((int) sai.Direction + 7) % 8)))  // concatenator
                                instruction = "C";
                            else if (directions.Count == 2 && directions.Contains((Direction) (((int) sai.Direction + 3) % 8)) && directions.Contains((Direction) (((int) sai.Direction + 5) % 8)))  // label
                                instruction = "L";
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
                                dir = dai.Direction.GetDirection2();
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

            StringFormat sf = null;
            foreach (var pair in _file.Items.SelectMany(i => i.Arrows).UniquePairs())
                if (pair.Item1.X == pair.Item2.X && pair.Item1.Y == pair.Item2.Y)
                    g.DrawString("!!!", new Font(_arrowFont, fontSize, FontStyle.Italic), Brushes.Red,
                        cellSize * (pair.Item1.X - _paintMinX) + cellSize / 2, cellSize * (pair.Item1.Y - _paintMinY) + cellSize / 2,
                        sf ?? (sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }));
        }

        private ArrowInfo getPointTo(int x, int y, int xOffset, int yOffset)
        {
            do
            {
                x += xOffset;
                y += yOffset;
                var found = _file.Items.SelectMany(itm => itm.Arrows).FirstOrDefault(arrow => arrow.X == x && arrow.Y == y);
                if (found != null)
                    return found;
            }
            while (x >= _paintMinX && x <= _paintMaxX && y >= _paintMinY && y <= _paintMaxY);
            return null;
        }

        private void drawInRoundedRectangle(Graphics g, string text, PointF location, Color background, Color outline, Color textColor)
        {
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var size = g.MeasureString(text, new Font(_warningFont, _paintFontSize / 5), int.MaxValue, sf) + new SizeF(6, 2);
            var realLocation = location - new SizeF(size.Width / 2, size.Height / 2);
            var path = GraphicsUtil.RoundedRectangle(new RectangleF(realLocation, size), Math.Min(size.Width, size.Height) / 3);
            g.FillPath(new SolidBrush(background), path);
            g.DrawPath(new Pen(outline, 1), path);
            g.DrawString(text, new Font(_warningFont, _paintFontSize / 5), new SolidBrush(textColor), new RectangleF(realLocation + new SizeF(3, 1), size - new SizeF(6, 2)), sf);
        }

        private void rotate(object sender, EventArgs __)
        {
            if (_selected.Count == 0)
                return;
            foreach (var arrow in _selected.SelectMany(itm => itm.Arrows))
                arrow.Rotate(sender == miRotateClockwise);
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

        private void save(object _, EventArgs __)
        {
            save();
        }

        private bool save()
        {
            if (_filename == null)
                return saveAs();
            XmlClassify.SaveObjectToXmlFile(_file, _filename);
            _fileChanged = false;
            return true;
        }

        private void saveAs(object _, EventArgs __)
        {
            saveAs();
        }

        private bool saveAs()
        {
            using (var dlg = new SaveFileDialog { AddExtension = true, DefaultExt = "ziim", Title = "Save Ziim program as", Filter = "Ziim programs|*.ziim" })
            {
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return false;
                _filename = dlg.FileName;
                return save();
            }
        }

        private void move(object sender, EventArgs __)
        {
            var xOffset = sender == miMoveLeft ? -1 : sender == miMoveRight ? 1 : 0;
            var yOffset = sender == miMoveUp ? -1 : sender == miMoveDown ? 1 : 0;
            var squaresTaken = new Dictionary<int, HashSet<int>>();
            foreach (var arrow in _file.Items.SelectMany(item => item.Arrows))
                squaresTaken.AddSafe(arrow.X, arrow.Y);

            bool anyTaken;
            do
            {
                anyTaken = false;
                foreach (var arrow in _selected.SelectMany(item => item.Arrows))
                {
                    arrow.X += xOffset;
                    arrow.Y += yOffset;
                    if (squaresTaken.Contains(arrow.X, arrow.Y))
                        anyTaken = true;
                }
            }
            while (anyTaken);
            refresh();
        }

        private bool _draggingSelectionRectangle;
        private Point _mouseDown;
        private Point _mouseDraggedTo;
        private Item _mouseMoving;
        private ArrowInfo _arrowReorienting;

        private void mouseDown(object sender, MouseEventArgs e)
        {
            var x = _paintCellSize == 0 ? 0 : divRoundDown(e.X - _paintTarget.Left, _paintCellSize) + _paintMinX;
            var y = _paintCellSize == 0 ? 0 : divRoundDown(e.Y - _paintTarget.Top, _paintCellSize) + _paintMinY;
            _mouseDown = _mouseDraggedTo = new Point(x, y);

            var clickedOn = _file.Items.FirstOrDefault(item => item.Arrows.Any(arr => arr.X == x && arr.Y == y));

            if (miMoveSelect.Checked && clickedOn != null && !Ut.Ctrl)
            {
                if (!_selected.Contains(clickedOn) && !Ut.Shift)
                    _selected.Clear();
                _selected.Add(clickedOn);
                _mouseMoving = clickedOn;
                ctImage.Invalidate();
            }
            else if (miMoveSelect.Checked)
            {
                _draggingSelectionRectangle = true;
                ctImage.Invalidate();
            }
            else if (miDraw.Checked)
            {
                if (clickedOn is ArrowInfo)
                {
                    _arrowReorienting = (ArrowInfo) clickedOn;
                    _selected.Clear();
                    _selected.Add(clickedOn);
                    ctImage.Invalidate();
                }
                else
                {
                    _arrowReorienting = Ut.Shift ? (ArrowInfo) new DoubleArrowInfo { X = x, Y = y } : new SingleArrowInfo { X = x, Y = y };
                    _file.Items.Add(_arrowReorienting);
                    _selected.Clear();
                    _selected.Add(_arrowReorienting);
                    refresh();
                }
            }
        }

        private static int divRoundDown(int dividend, int divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();
            if (divisor == -1 && dividend == Int32.MinValue)
                throw new OverflowException();
            return (dividend % divisor) == 0 || ((divisor > 0) == (dividend > 0))
                ? dividend / divisor
                : dividend / divisor - 1;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            var x = _paintCellSize == 0 ? 0 : divRoundDown(e.X - _paintTarget.Left, _paintCellSize) + _paintMinX;
            var y = _paintCellSize == 0 ? 0 : divRoundDown(e.Y - _paintTarget.Top, _paintCellSize) + _paintMinY;
            if (x == _mouseDraggedTo.X && y == _mouseDraggedTo.Y)
                return;
            var deltaX = x - _mouseDraggedTo.X;
            var deltaY = y - _mouseDraggedTo.Y;
            _mouseDraggedTo = new Point(x, y);

            if (miMoveSelect.Checked)
                ctImage.Cursor = _mouseMoving != null || (!Ut.Ctrl && _file.Items.SelectMany(itm => itm.Arrows).Any(a => a.X == x && a.Y == y)) ? Cursors.SizeAll : Cursors.Default;

            if (_draggingSelectionRectangle)
                ctImage.Invalidate();
            else if (_mouseMoving != null)
            {
                foreach (var item in _selected)
                    item.Move(deltaX, deltaY);
                refresh();
                _fileChanged = true;
            }
            else if (_arrowReorienting != null)
            {
                _arrowReorienting.Reorient(
                    2 * (y - _arrowReorienting.Y) > x - _arrowReorienting.X,      //~\_
                    y - _arrowReorienting.Y > 2 * (x - _arrowReorienting.X),     // \
                    y - _arrowReorienting.Y > 2 * (_arrowReorienting.X - x),     // /
                    2 * (y - _arrowReorienting.Y) > _arrowReorienting.X - x     // _/~
                );
                refresh();
                _fileChanged = true;
            }
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            if (_draggingSelectionRectangle)
            {
                if (!Ut.Shift)
                    _selected.Clear();

                foreach (var item in _file.Items.Where(item => item.IsContainedIn(
                    Math.Min(_mouseDown.X, _mouseDraggedTo.X),
                    Math.Min(_mouseDown.Y, _mouseDraggedTo.Y),
                    Math.Max(_mouseDown.X, _mouseDraggedTo.X),
                    Math.Max(_mouseDown.Y, _mouseDraggedTo.Y))))
                    _selected.Add(item);

                _draggingSelectionRectangle = false;
                ctImage.Invalidate();
            }
            else if (_mouseMoving != null || _arrowReorienting != null)
            {
                _mouseMoving = null;
                _arrowReorienting = null;
                ctImage.Invalidate();
            }
        }

        private void revert(object _, EventArgs __)
        {
            if (_filename == null)
                return;
            if (DlgMessage.Show("Really lose all changes since last save?", "Revert", DlgType.Question, "&Discard changes", "&Keep changes and cancel") == 1)
                return;
            fileOpen(_filename);
            setUi();
        }

        private void setUi()
        {
            miGrid.Checked = ZiimHelperProgram.Settings.ViewGrid;
            miConnectionLines.Checked = ZiimHelperProgram.Settings.ViewConnectionLines;
            miInstructions.Checked = ZiimHelperProgram.Settings.ViewInstructions;
            miAnnotations.Checked = ZiimHelperProgram.Settings.ViewAnnotations;
            miClouds.Checked = ZiimHelperProgram.Settings.ViewClouds;

            (ZiimHelperProgram.Settings.EditMode == EditMode.MoveSelect ? miMoveSelect :
                ZiimHelperProgram.Settings.EditMode == EditMode.Draw ? miDraw : Ut.Throw<ToolStripMenuItem>(new InvalidOperationException())).Checked = true;

            refresh();
        }

        private sealed class arrowException : Exception
        {
            public ArrowInfo Arrow { get; private set; }
            public arrowException(ArrowInfo arrow, string message) : base(message) { Arrow = arrow; }
        }

        private void toggleMark(object sender, EventArgs e)
        {
            foreach (var arrow in _selected.SelectMany(itm => itm.Arrows))
                arrow.Marked = !arrow.Marked;
            _fileChanged = true;
            refresh();
        }

        private void copySource(object sender, EventArgs e)
        {
            if (_file.Items.Count == 0)
            {
                Clipboard.SetText("");
                return;
            }

            var minX = _file.Items.SelectMany(itm => itm.Arrows).Min(a => a.X);
            var minY = _file.Items.SelectMany(itm => itm.Arrows).Min(a => a.Y);
            var maxX = _file.Items.SelectMany(itm => itm.Arrows).Max(a => a.X);
            var maxY = _file.Items.SelectMany(itm => itm.Arrows).Max(a => a.Y);

            var arr = Ut.NewArray<char>(maxY - minY + 1, maxX - minX + 1, (i, j) => ' ');
            foreach (var arrow in _file.Items.SelectMany(itm => itm.Arrows))
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

            if (_file.Items.Count == 0)
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
            var annotation = _selected.SelectMany(itm => itm.Arrows).Select(arr => arr.Annotation).JoinString();
            var newAnnotation = InputBox.GetLine("Annotation:", annotation, "Annotation", "&OK", "&Cancel");
            if (newAnnotation != null)
            {
                foreach (var arr in _selected.SelectMany(itm => itm.Arrows))
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

        private void setMode(EditMode mode)
        {
            foreach (var tup in Ut.NewArray(
                Tuple.Create(miMoveSelect, EditMode.MoveSelect, true),
                Tuple.Create(miDraw, EditMode.Draw, true)
            ))
            {
                var thisOne = mode == tup.Item2;
                tup.Item1.Checked = thisOne;
                tup.Item1.Visible = tup.Item3 || thisOne;
            }
            setCursor();
        }

        private void setCursor()
        {
            if (miMoveSelect.Checked)
            {
                if (Ut.Ctrl)
                    ctImage.Cursor = Cursors.Default;
                else
                {
                    var mousePosition = ctImage.PointToClient(Control.MousePosition);
                    var x = _paintCellSize == 0 ? 0 : (mousePosition.X - _paintTarget.Left) / _paintCellSize + _paintMinX;
                    var y = _paintCellSize == 0 ? 0 : (mousePosition.Y - _paintTarget.Top) / _paintCellSize + _paintMinY;
                    ctImage.Cursor = _file.Items.SelectMany(itm => itm.Arrows).Any(a => a.X == x && a.Y == y) ? Cursors.SizeAll : Cursors.Default;
                }
            }
            else if (miDraw.Checked)
                ctImage.Cursor = Cursors.Cross;
        }

        private void switchMode(object sender, EventArgs __)
        {
            setMode(
                sender == miMoveSelect ? EditMode.MoveSelect :
                sender == miDraw ? EditMode.Draw : Ut.Throw<EditMode>(new InvalidOperationException()));
        }

        private void fileNew(object _, EventArgs __)
        {
            if (!canDestroy())
                return;
            _filename = null;
            _file = new ZiimFile();
            _selected.Clear();
            _fileChanged = false;
            refresh();
        }

        private bool canDestroy()
        {
            if (!_fileChanged)
                return true;
            switch (DlgMessage.Show("Save changes to this file?", "File has changed", DlgType.Question, "&Save", "&Discard", "&Cancel"))
            {
                case 0: return save();  // save
                case 1: return true;    // discard
                default: return false;  // cancel
            }
        }

        private void open(object _, EventArgs __)
        {
            if (!canDestroy())
                return;
            using (var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "ziim",
                Multiselect = false,
                Title = "Open Ziim program",
                Filter = "Ziim programs|*.ziim"
            })
            {
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return;
                fileOpen(dlg.FileName);
                return;
            }
        }

        private void fileOpen(string filename)
        {
            try
            {
                _file = XmlClassify.LoadObjectFromXmlFile<ZiimFile>(filename);
            }
            catch (Exception e)
            {
                DlgMessage.Show("The file could not be opened:\n\n" + e.Message, "Error", DlgType.Error);
                return;
            }
            _selected.Clear();
            _filename = filename;
            _fileChanged = false;
            refresh();
        }

        private void formClose(object sender, FormClosingEventArgs e)
        {
            // Save settings
            ZiimHelperProgram.Settings.ViewGrid = miGrid.Checked;
            ZiimHelperProgram.Settings.ViewConnectionLines = miConnectionLines.Checked;
            ZiimHelperProgram.Settings.ViewInstructions = miInstructions.Checked;
            ZiimHelperProgram.Settings.ViewAnnotations = miAnnotations.Checked;
            ZiimHelperProgram.Settings.ViewClouds = miClouds.Checked;
            ZiimHelperProgram.Settings.EditMode = miDraw.Checked ? EditMode.Draw : EditMode.MoveSelect;

            if (!canDestroy())
            {
                e.Cancel = true;
                return;
            }
        }

        private void exit(object _, EventArgs __)
        {
            if (!canDestroy())
                return;
            Application.Exit();
        }

        private void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            setCursor();
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            setCursor();
        }

        private void import(object _, EventArgs __)
        {
            using (var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "ziim",
                Multiselect = false,
                Title = "Import Ziim sub-program",
                Filter = "Ziim programs|*.ziim"
            })
            {
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return;
                ZiimFile file;
                try
                {
                    file = XmlClassify.LoadObjectFromXmlFile<ZiimFile>(dlg.FileName);
                }
                catch (Exception e)
                {
                    DlgMessage.Show("The file could not be opened:\n\n" + e.Message, "Error", DlgType.Error);
                    return;
                }
                var cloud = new Cloud { Items = file.Items };
                _file.Items.Add(cloud);
                _selected.Add(cloud);
                jiggle(cloud);
                _fileChanged = true;
                refresh();
            }
        }

        private void jiggle(Item item)
        {
            var squaresTaken = new Dictionary<int, HashSet<int>>();
            foreach (var arrow in _file.Items.Except(new[] { item }).SelectMany(itm => itm.Arrows))
                squaresTaken.AddSafe(arrow.X, arrow.Y);
            int radius = 0, px = 0, py = 0;
            while (item.Arrows.Any(arr => squaresTaken.Contains(arr.X, arr.Y)))
            {
                int x = px, y = px;
                if (x == radius)
                {
                    if (y == radius)
                    {
                        radius++;
                        x = -radius;
                        y = -radius;
                    }
                    else
                    {
                        x = 0;
                        y++;
                    }
                }
                else
                    x++;
                foreach (var arr in item.Arrows)
                {
                    arr.X += x - px;
                    arr.Y += y - py;
                }
                px = x;
                py = y;
            }
        }
    }
}
