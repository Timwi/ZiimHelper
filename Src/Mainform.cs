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
        private ZiimFile _file = new ZiimFile { Items = new List<ArrowInfo>() };
        private string _filename = null;
        private bool _fileChanged = false;
        private HashSet<ArrowInfo> _selected = new HashSet<ArrowInfo>();

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

        private bool _suppressRepaint;

        private void paint(object _, PaintEventArgs e)
        {
            if (_paintCellSize < 1)
                return;
            e.Graphics.SetHighQuality();
            foreach (var arr in _selected.OfType<ArrowInfo>())
                e.Graphics.DrawEllipse(new Pen(Brushes.Red, 2),
                    _paintTarget.Left + (arr.X - _paintMinX) * _paintCellSize + _paintCellSize / 10,
                    _paintTarget.Top + (arr.Y - _paintMinY) * _paintCellSize + _paintCellSize / 10,
                    _paintCellSize * 8 / 10, _paintCellSize * 8 / 10);

            if (_imageMouseDown != null)
            {
                var minX = Math.Min(_imageMouseDown.Value.X, _imageMouseDraggedTo.X) - _paintMinX;
                var minY = Math.Min(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y) - _paintMinY;
                var maxX = Math.Max(_imageMouseDown.Value.X, _imageMouseDraggedTo.X) - _paintMinX;
                var maxY = Math.Max(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y) - _paintMinY;
                var rect = new Rectangle(_paintTarget.Left + _paintCellSize * minX, _paintTarget.Top + _paintCellSize * minY, _paintCellSize * (maxX - minX + 1), _paintCellSize * (maxY - minY + 1));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 32, 128, 192)), rect);
                e.Graphics.DrawRectangle(new Pen(Brushes.Blue, 2), rect);
            }
            else if (_arrowMoving != null || _arrowReorienting != null)
            {
                var x = (_arrowMoving ?? _arrowReorienting).X;
                var y = (_arrowMoving ?? _arrowReorienting).Y;
                var rect = new Rectangle(_paintTarget.Left + _paintCellSize * (x - _paintMinX), _paintTarget.Top + _paintCellSize * (y - _paintMinY), _paintCellSize, _paintCellSize);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(32, 128, 32, 192)), rect);
                e.Graphics.DrawEllipse(new Pen(Brushes.Green, 3), rect);
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

            _paintMinX = _file.Items.OfType<ArrowInfo>().Min(a => a.X);
            _paintMinY = _file.Items.OfType<ArrowInfo>().Min(a => a.Y);
            _paintMaxX = _file.Items.OfType<ArrowInfo>().Max(a => a.X);
            _paintMaxY = _file.Items.OfType<ArrowInfo>().Max(a => a.Y);

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

            var hitFromDic = new Dictionary<ArrowInfo, List<Direction>>();

            foreach (var arr in _file.Items.OfType<ArrowInfo>())
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
                foreach (var arr in _file.Items.OfType<ArrowInfo>())
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
        }

        private ArrowInfo getPointTo(int x, int y, int xOffset, int yOffset)
        {
            do
            {
                x += xOffset;
                y += yOffset;
                var found = _file.Items.OfType<ArrowInfo>().FirstOrDefault(arrow => arrow.X == x && arrow.Y == y);
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
            foreach (var arrow in _selected.OfType<ArrowInfo>())
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

        private void moveArrow(object sender, EventArgs __)
        {
            var xOffset = sender == miMoveLeft ? -1 : sender == miMoveRight ? 1 : 0;
            var yOffset = sender == miMoveUp ? -1 : sender == miMoveDown ? 1 : 0;
            do
            {
                foreach (var item in _selected.OfType<ArrowInfo>())
                {
                    item.X += xOffset;
                    item.Y += yOffset;
                }
            }
            while (_file.Items.OfType<ArrowInfo>().Except(_selected).SelectMany(arrow => _selected.Select(sel => new { sel, arrow })).Any(tup => tup.sel.X == tup.arrow.X && tup.sel.Y == tup.arrow.Y));
            refresh();
        }

        private void selectedIndexChanged(object _, EventArgs __)
        {
            if (!_suppressRepaint)
                ctImage.Invalidate();
        }

        private Point? _imageMouseDown;
        private Point _imageMouseDraggedTo;
        private ArrowInfo _arrowMoving;
        private ArrowInfo _arrowReorienting;

        private void imageMouseDown(object sender, MouseEventArgs e)
        {
            var x = _paintCellSize == 0 ? 0 : divRoundDown(e.X - _paintTarget.Left, _paintCellSize) + _paintMinX;
            var y = _paintCellSize == 0 ? 0 : divRoundDown(e.Y - _paintTarget.Top, _paintCellSize) + _paintMinY;
            _imageMouseDraggedTo = new Point(x, y);

            ArrowInfo clickedOn = _file.Items.OfType<ArrowInfo>().FirstOrDefault(item => item.X == x && item.Y == y);
            if (miMoveSelect.Checked && clickedOn != null && !Ut.Ctrl)
            {
                if (!_selected.Contains(clickedOn) && !Ut.Shift)
                    _selected.Clear();
                _selected.Add(clickedOn);
                _arrowMoving = clickedOn;
                ctImage.Invalidate();
            }
            else if (miMoveSelect.Checked)
            {
                _imageMouseDown = new Point(x, y);
                ctImage.Invalidate();
            }
            else if (miDraw.Checked)
            {
                if (clickedOn != null)
                {
                    _arrowReorienting = clickedOn;
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

        private void imageMouseMove(object sender, MouseEventArgs e)
        {
            var x = _paintCellSize == 0 ? 0 : divRoundDown(e.X - _paintTarget.Left, _paintCellSize) + _paintMinX;
            var y = _paintCellSize == 0 ? 0 : divRoundDown(e.Y - _paintTarget.Top, _paintCellSize) + _paintMinY;
            if (x == _imageMouseDraggedTo.X && y == _imageMouseDraggedTo.Y)
                return;
            _imageMouseDraggedTo = new Point(x, y);

            if (miMoveSelect.Checked)
                ctImage.Cursor = _arrowMoving != null || (!Ut.Ctrl && _file.Items.OfType<ArrowInfo>().Any(a => a.X == x && a.Y == y)) ? Cursors.SizeAll : Cursors.Default;

            if (_imageMouseDown != null)
                ctImage.Invalidate();
            else if (_arrowMoving != null)
            {
                var xDist = x - _arrowMoving.X;
                var yDist = y - _arrowMoving.Y;
                foreach (var arrow in _selected.OfType<ArrowInfo>())
                {
                    arrow.X += xDist;
                    arrow.Y += yDist;
                }
                refresh();
                _fileChanged = true;
            }
            else if (_arrowReorienting != null)
            {
                var a = 2 * (y - _arrowReorienting.Y) > x - _arrowReorienting.X;      //~\_
                var b = y - _arrowReorienting.Y > 2 * (x - _arrowReorienting.X);     // \
                var c = y - _arrowReorienting.Y > 2 * (_arrowReorienting.X - x);     // /
                var d = 2 * (y - _arrowReorienting.Y) > _arrowReorienting.X - x;    // _/~
                if (_arrowReorienting is SingleArrowInfo)
                    ((SingleArrowInfo) _arrowReorienting).Direction =
                        b && c ? Direction.Down :
                        !c && d ? Direction.DownLeft :
                        !d && a ? Direction.Left :
                        !a && b ? Direction.UpLeft :
                        !b && !c ? Direction.Up :
                        c && !d ? Direction.UpRight :
                        d && !a ? Direction.Right :
                        a && !b ? Direction.DownRight : Ut.Throw<Direction>(new InvalidOperationException());
                else if (_arrowReorienting is DoubleArrowInfo)
                    ((DoubleArrowInfo) _arrowReorienting).Direction =
                        b == c ? DoubleDirection.UpDown :
                        c != d ? DoubleDirection.UpRightDownLeft :
                        d != a ? DoubleDirection.RightLeft :
                        a != b ? DoubleDirection.DownRightUpLeft : Ut.Throw<DoubleDirection>(new InvalidOperationException());
                refresh();
                _fileChanged = true;
            }
        }

        private void imageMouseUp(object sender, MouseEventArgs e)
        {
            if (_imageMouseDown != null)
            {
                var minX = Math.Min(_imageMouseDown.Value.X, _imageMouseDraggedTo.X);
                var minY = Math.Min(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y);
                var maxX = Math.Max(_imageMouseDown.Value.X, _imageMouseDraggedTo.X);
                var maxY = Math.Max(_imageMouseDown.Value.Y, _imageMouseDraggedTo.Y);
                _imageMouseDown = null;
                _suppressRepaint = true;
                if (!Ut.Shift)
                    _selected.Clear();
                foreach (var item in _file.Items.OfType<ArrowInfo>().ToList())
                    if (item.X >= minX && item.X <= maxX && item.Y >= minY && item.Y <= maxY && !_selected.Contains(item))
                        _selected.Add(item);
                _suppressRepaint = false;
                ctImage.Invalidate();
            }
            else if (_arrowMoving != null || _arrowReorienting != null)
            {
                _arrowMoving = null;
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
            foreach (var arrow in _selected.OfType<ArrowInfo>())
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

            var minX = _file.Items.OfType<ArrowInfo>().Min(a => a.X);
            var minY = _file.Items.OfType<ArrowInfo>().Min(a => a.Y);
            var maxX = _file.Items.OfType<ArrowInfo>().Max(a => a.X);
            var maxY = _file.Items.OfType<ArrowInfo>().Max(a => a.Y);

            var arr = Ut.NewArray<char>(maxY - minY + 1, maxX - minX + 1, (i, j) => ' ');
            foreach (var arrow in _file.Items.OfType<ArrowInfo>())
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
            var annotation = _selected.OfType<ArrowInfo>().Select(arr => arr.Annotation).JoinString();
            var newAnnotation = InputBox.GetLine("Annotation:", annotation, "Annotation", "&OK", "&Cancel");
            if (newAnnotation != null)
            {
                foreach (var arr in _selected.OfType<ArrowInfo>())
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
                    ctImage.Cursor = _file.Items.OfType<ArrowInfo>().Any(a => a.X == x && a.Y == y) ? Cursors.SizeAll : Cursors.Default;
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
            _file = new ZiimFile { Items = new List<ArrowInfo>() };
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
    }
}
