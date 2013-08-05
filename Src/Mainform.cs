﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using RT.Util;
using RT.Util.Dialogs;
using RT.Util.Drawing;
using RT.Util.ExtensionMethods;
using RT.Util.Forms;
using RT.Util.Serialization;
using RT.Util.Xml;

namespace ZiimHelper
{
    public partial class Mainform : ManagedForm
    {
        private Cloud _editingCloud = new Cloud();
        private Stack<Cloud> _outerClouds = new Stack<Cloud>();
        private Cloud _file = new Cloud();
        private string _filename = null;
        private bool _fileChanged = false;
        private HashSet<Item> _selected = new HashSet<Item>();

        private FontFamily _arrowFont = new FontFamily("Cambria");
        private FontFamily _instructionFont = new FontFamily("Gentium Book Basic");
        private FontFamily _annotationFont = new FontFamily("Calibri");

        private ClassifyOptions _classifyOptions = new ClassifyOptions();

        private sealed class ModeInfo
        {
            public ToolStripMenuItem MenuItem { get; private set; }
            public EditMode EditMode { get; private set; }
            public ModeInfo(ToolStripMenuItem menuItem, EditMode editMode) { MenuItem = menuItem; EditMode = editMode; }
        }
        private ModeInfo[] _modes;

        public Mainform()
            : base(ZiimHelperProgram.Settings.FormSettings)
        {
            InitializeComponent();

            _classifyOptions.AddTypeOptions(typeof(Color), new ColorClassifyOptions());

            _modes = Ut.NewArray(
                new ModeInfo(miMoveSelect, EditMode.MoveSelect),
                new ModeInfo(miDraw, EditMode.Draw),
                new ModeInfo(miSetLabelPosition, EditMode.SetLabelPosition)
            );

            setUi();
            setMode(ZiimHelperProgram.Settings.EditMode);
        }

        private void deleteItem(object _, EventArgs __)
        {
            if (_selected.Count == 0)
                return;
            _editingCloud.Items.RemoveAll(_selected.Contains);
            _selected.Clear();
            _fileChanged = true;
            refresh();
        }

        private void paint(object _, PaintEventArgs e)
        {
            if (_paintCellSize < 1)
                return;
            e.Graphics.SetHighQuality();
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

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
            else if (miSetLabelPosition.Checked)
            {
                foreach (var pt in Ut.NewArray(
                    new[] { _editingCloud.LabelFromX, _editingCloud.LabelFromY },
                    new[] { _editingCloud.LabelToX, _editingCloud.LabelToY }
                ))
                {
                    int x = pt[0] - _paintMinX, y = pt[1] - _paintMinY;
                    var rect = new Rectangle(x * _paintCellSize + _paintTarget.Left, y * _paintCellSize + _paintTarget.Top, _paintCellSize, _paintCellSize);
                    e.Graphics.DrawEllipse(new Pen(Brushes.Magenta, 3), rect);
                }
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
            if (_editingCloud.Items.Count < 1)
                return;

            _editingCloud.GetBounds(out _paintMinX, out _paintMaxX, out _paintMinY, out _paintMaxY);

            var fit = new Size(_paintMaxX - _paintMinX + 1, _paintMaxY - _paintMinY + 1).FitIntoMaintainAspectRatio(new Rectangle(margin, margin, ctImage.ClientSize.Width - 2 * margin, ctImage.ClientSize.Height - 2 * margin));
            var w = fit.Width - fit.Width % (_paintMaxX - _paintMinX + 1);
            var h = fit.Height - fit.Height % (_paintMaxY - _paintMinY + 1);

            _paintTarget = new Rectangle(ctImage.ClientSize.Width / 2 - w / 2, ctImage.ClientSize.Height / 2 - h / 2, w, h);
            _paintCellSize = _paintTarget.Width / (_paintMaxX - _paintMinX + 1);
            _paintFontSize = "↑↗→↘↓↙←↖↕⤢↔⤡".Min(ch => e.Graphics.GetMaximumFontSize(new SizeF(_paintCellSize, _paintCellSize), _arrowFont, ch.ToString()));

            e.Graphics.FillRectangle(Brushes.White, _paintTarget);
            e.Graphics.DrawRectangle(Pens.Black, _paintTarget);
            using (var tr = new GraphicsTransformer(e.Graphics).Translate(_paintTarget.Left, _paintTarget.Top))
                paintInto(e.Graphics, _paintCellSize, _paintFontSize);
        }

        private void paintInto(Graphics g, int cellSize, float fontSize)
        {
            g.SetHighQuality();
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            var maxSize = Math.Max(_paintMaxX - _paintMinX + 1, _paintMaxY - _paintMinY + 1);

            if (miGrid.Checked)
            {
                for (int i = 1; i <= _paintMaxX - _paintMinX; i++)
                    g.DrawLine(Pens.DarkGray, i * cellSize, 0, i * cellSize, (_paintMaxY - _paintMinY + 1) * cellSize);
                for (int j = 1; j <= _paintMaxY - _paintMinY; j++)
                    g.DrawLine(Pens.DarkGray, 0, j * cellSize, (_paintMaxX - _paintMinX + 1) * cellSize, j * cellSize);
            }

            if (miInnerClouds.Checked || miOwnCloud.Checked)
                using (var tr = new GraphicsTransformer(g).Translate(-_paintMinX * cellSize, -_paintMinY * cellSize))
                    foreach (var cloud in miInnerClouds.Checked ? _editingCloud.AllClouds : new[] { _editingCloud })
                        if (cloud != _editingCloud || miOwnCloud.Checked)
                            cloud.DrawCloud(g, cellSize, cloud == _editingCloud);

            var hitFromDic = new Dictionary<ArrowInfo, List<Direction>>();

            foreach (var inf in _editingCloud.ArrowsWithParents.OrderBy(awp => !awp.Item1.IsTerminal))
            {
                var arr = inf.Item1;
                var parentCloud = inf.Item2;

                if ((!arr.IsTerminal || parentCloud != _editingCloud || miOwnCloud.Checked) &&
                    (!arr.IsTerminal || parentCloud == _editingCloud || miInnerClouds.Checked))
                    g.DrawString(
                        arr.Character.ToString(),
                        new Font(_arrowFont, fontSize),
                        arr.IsTerminal ? new SolidBrush(parentCloud.Color) : arr.Marked ? Brushes.Red : Brushes.Black,
                        (arr.X - _paintMinX) * cellSize + cellSize / 2, (arr.Y - _paintMinY) * cellSize + cellSize / 2,
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                    );

                if (miCoordinates.Checked)
                    g.DrawString(arr.CoordsString, new Font(_annotationFont, fontSize / 4), Brushes.Black, (arr.X - _paintMinX) * cellSize, (arr.Y - _paintMinY) * cellSize);

                if (miAnnotations.Checked && arr.Annotation != null && !arr.IsTerminal)
                    drawInRoundedRectangle(g, arr.Annotation, new PointF((arr.X - _paintMinX) * cellSize + cellSize / 2, (arr.Y - _paintMinY) * cellSize), Color.FromArgb(0xEE, 0xEE, 0xFF), Color.Blue, Color.DarkBlue);

                if (!arr.IsTerminal || parentCloud == _editingCloud)
                {
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
            }

            foreach (var inf in _editingCloud.ArrowsWithParents.OrderBy(awp => !awp.Item1.IsTerminal))
            {
                var arrow = inf.Item1;
                var parentCloud = inf.Item2;
                var x = (arrow.X - _paintMinX) * cellSize;
                var y = (arrow.Y - _paintMinY) * cellSize;

                if (miInstructions.Checked || arrow.IsTerminal)
                {
                    var directions = !arrow.IsTerminal && hitFromDic.ContainsKey(arrow) ? hitFromDic[arrow] : null;
                    string instruction = null;
                    Direction dir = 0;
                    if (arrow is SingleArrowInfo)
                    {
                        var sai = (SingleArrowInfo) arrow;
                        dir = sai.Direction;
                        if (!sai.IsTerminal)
                        {
                            if (directions == null || directions.Count == 0)   // { 0 }
                                instruction = "0";
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 1) % 8))     // stdin
                                instruction = "R";
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 3) % 8))     // invert
                                instruction = "I";
                            else if (directions.Count == 1 && directions[0] == (Direction) (((int) sai.Direction + 5) % 8))     // no-op
                                instruction = "N";
                            else if (directions.Count == 2 && directions.Contains((Direction) (((int) sai.Direction + 1) % 8)) && directions.Contains((Direction) (((int) sai.Direction + 7) % 8)))  // concatenator
                                instruction = "C";
                            else if (directions.Count == 2 && directions.Contains((Direction) (((int) sai.Direction + 3) % 8)) && directions.Contains((Direction) (((int) sai.Direction + 5) % 8)))  // label
                                instruction = "L";
                        }
                    }
                    else
                    {
                        var dai = (DoubleArrowInfo) arrow;
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

                    if (arrow.IsTerminal)
                    {
                        if (arrow.Annotation != null && ((miOwnCloud.Checked && parentCloud == _editingCloud) || (miInnerClouds.Checked && parentCloud != _editingCloud)))
                        {
                            var p = new[] { new Point(0, 0) };
                            g.Transform.TransformPoints(p);
                            using (var tr = new GraphicsTransformer(g).Translate(0, -cellSize / 4).RotateAt(45 * ((int) dir % 4 - 2), p[0]).Translate(x + cellSize / 2, y + cellSize / 2))
                            {
                                g.DrawString(
                                    arrow.Annotation,
                                    new Font(_annotationFont, g.GetMaximumFontSize(new SizeF(cellSize * 4 / 5, cellSize * 4 / 5), _annotationFont, arrow.Annotation)),
                                    arrow.IsTerminal ? new SolidBrush(parentCloud.Color) : Brushes.Black,
                                    0, 0,
                                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                                );
                            }
                        }
                    }
                    else if (instruction != null)
                    {
                        g.DrawString(instruction, new Font(_instructionFont, fontSize / 2), Brushes.Black,
                            (float) (x + cellSize / 2 + Math.Cos(Math.PI / 4 * (int) dir) * cellSize / 4),
                            (float) (y + cellSize / 2 + Math.Sin(Math.PI / 4 * (int) dir) * cellSize / 4),
                            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                        );
                    }
                    else
                        g.FillEllipse(new SolidBrush(Color.FromArgb(64, 255, 128, 128)), x, y, cellSize, cellSize);
                }
            }

            StringFormat sf = null;
            foreach (var pair in _editingCloud.AllArrows.UniquePairs())
                if (pair.Item1.X == pair.Item2.X && pair.Item1.Y == pair.Item2.Y && pair.Item1.IsTerminal == pair.Item2.IsTerminal)
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
                var found = _editingCloud.AllArrows.FirstOrDefault(arrow => !arrow.IsTerminal && arrow.X == x && arrow.Y == y);
                if (found != null)
                    return found;
            }
            while (x >= _paintMinX && x <= _paintMaxX && y >= _paintMinY && y <= _paintMaxY);
            return null;
        }

        private void drawInRoundedRectangle(Graphics g, string text, PointF location, Color background, Color outline, Color textColor)
        {
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var size = g.MeasureString(text, new Font(_annotationFont, _paintFontSize / 5), int.MaxValue, sf) + new SizeF(6, 2);
            var realLocation = location - new SizeF(size.Width / 2, size.Height / 2);
            var path = GraphicsUtil.RoundedRectangle(new RectangleF(realLocation, size), Math.Min(size.Width, size.Height) / 3);
            g.FillPath(new SolidBrush(background), path);
            g.DrawPath(new Pen(outline, 1), path);
            g.DrawString(text, new Font(_annotationFont, _paintFontSize / 5), new SolidBrush(textColor), new RectangleF(realLocation + new SizeF(3, 1), size - new SizeF(6, 2)), sf);
        }

        private void rotate(object sender, EventArgs __)
        {
            if (_selected.Count == 0)
                return;

            if (_selected.Count == 1 && _selected.Single() is ArrowInfo)
            {
                ((ArrowInfo) _selected.Single()).Rotate(sender == miRotateClockwise);
                _fileChanged = true;
                refresh();
                return;
            }

            var minX = _selected.SelectMany(itm => itm.AllArrows).Min(a => a.X);
            var minY = _selected.SelectMany(itm => itm.AllArrows).Min(a => a.Y);
            var maxX = _selected.SelectMany(itm => itm.AllArrows).Max(a => a.X);
            var maxY = _selected.SelectMany(itm => itm.AllArrows).Max(a => a.Y);

            foreach (var arrow in _selected.SelectMany(itm => itm.AllArrows))
            {
                var x = arrow.X - minX;
                var y = arrow.Y - minY;

                if (sender == miRotateClockwise)
                {
                    arrow.X = minX + (maxY - minY) - y;
                    arrow.Y = minY + x;
                    arrow.Rotate(true);
                    arrow.Rotate(true);
                }
                else
                {
                    arrow.X = minX + y;
                    arrow.Y = minY + (maxX - minX) - x;
                    arrow.Rotate(false);
                    arrow.Rotate(false);
                }
            }
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
            ClassifyXml.SerializeToFile(_file, _filename, _classifyOptions);
            _fileChanged = false;
            return true;
        }

        private void saveAs(object _, EventArgs __)
        {
            saveAs();
        }

        private bool saveAs()
        {
            using (var dlg = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "ziimx",
                Title = "Save Ziim program as",
                Filter = "Annotated Ziim programs|*.ziimx|Plain-text Ziim programs|*.ziim"
            })
            {
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return false;
                _filename = dlg.FileName;
                return save();
            }
        }

        private bool _draggingSelectionRectangle;
        private bool _draggingLabelPosition;
        private Point _mouseDown;
        private Point _mouseDraggedTo;
        private Item _mouseMoving;
        private ArrowInfo _arrowReorienting;

        private void mouseDown(object sender, MouseEventArgs e)
        {
            var x = _paintCellSize == 0 ? 0 : divRoundDown(e.X - _paintTarget.Left, _paintCellSize) + _paintMinX;
            var y = _paintCellSize == 0 ? 0 : divRoundDown(e.Y - _paintTarget.Top, _paintCellSize) + _paintMinY;
            _mouseDown = _mouseDraggedTo = new Point(x, y);

            var clickedOn = _editingCloud.ItemAt(x, y);

            if (miMoveSelect.Checked && clickedOn != null && !Ut.Ctrl && !Ut.Shift)
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
                _fileChanged = true;
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
                    _editingCloud.Items.Add(_arrowReorienting);
                    _selected.Clear();
                    _selected.Add(_arrowReorienting);
                    refresh();
                }
            }
            else if (miSetLabelPosition.Checked)
            {
                _fileChanged = true;
                _draggingLabelPosition = true;
                _editingCloud.LabelFromX = x;
                _editingCloud.LabelFromY = y;
                _editingCloud.LabelToX = x;
                _editingCloud.LabelToY = y;
                refresh();
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

            if (_draggingLabelPosition)
            {
                _editingCloud.LabelToX = x;
                _editingCloud.LabelToY = y;
                refresh();
                return;
            }

            var deltaX = x - _mouseDraggedTo.X;
            var deltaY = y - _mouseDraggedTo.Y;
            _mouseDraggedTo = new Point(x, y);

            if (miMoveSelect.Checked)
                ctImage.Cursor = _mouseMoving != null || (!Ut.Ctrl && !Ut.Shift && _editingCloud.AllArrows.Any(a => a.X == x && a.Y == y)) ? Cursors.SizeAll : Cursors.Default;

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
            _draggingLabelPosition = false;
            if (_draggingSelectionRectangle)
            {
                if (!Ut.Shift)
                    _selected.Clear();

                Item alreadyItem;
                if (_mouseDown.X == _mouseDraggedTo.X && _mouseDown.Y == _mouseDraggedTo.Y && (alreadyItem = _editingCloud.ItemAt(_mouseDraggedTo.X, _mouseDraggedTo.Y)) != null && _selected.Contains(alreadyItem))
                    _selected.Remove(alreadyItem);
                else
                {
                    foreach (var item in _editingCloud.Items.Where(item => item.IsContainedIn(
                        Math.Min(_mouseDown.X, _mouseDraggedTo.X),
                        Math.Min(_mouseDown.Y, _mouseDraggedTo.Y),
                        Math.Max(_mouseDown.X, _mouseDraggedTo.X),
                        Math.Max(_mouseDown.Y, _mouseDraggedTo.Y))))
                        _selected.Add(item);
                }
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
            miInnerClouds.Checked = ZiimHelperProgram.Settings.ViewInnerClouds;
            miOwnCloud.Checked = ZiimHelperProgram.Settings.ViewOwnCloud;
            miCoordinates.Checked = ZiimHelperProgram.Settings.ViewCoordinates;

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
            foreach (var arrow in _selected.SelectMany(itm => itm.AllArrows))
                arrow.Marked = !arrow.Marked;
            _fileChanged = true;
            refresh();
        }

        private void copySource(object sender, EventArgs e)
        {
            if (_editingCloud.Items.Count == 0)
            {
                Clipboard.SetText("");
                return;
            }

            var minX = _editingCloud.AllArrows.Min(a => a.X);
            var minY = _editingCloud.AllArrows.Min(a => a.Y);
            var maxX = _editingCloud.AllArrows.Max(a => a.X);
            var maxY = _editingCloud.AllArrows.Max(a => a.Y);

            var arr = Ut.NewArray<char>(maxY - minY + 1, maxX - minX + 1, (i, j) => ' ');
            foreach (var arrow in _editingCloud.AllArrows)
                if (!arrow.IsTerminal)
                    arr[arrow.Y - minY][arrow.X - minX] = arrow.Character;
            Clipboard.SetText(arr.Select(row => " " + row.JoinString()).JoinString(Environment.NewLine));
        }

        private void copyImage(object sender, EventArgs __)
        {
            tryAgain:
            var inputStr = InputBox.GetLine(
                sender == miCopyImageByFont ? "Specify the desired font size:" :
                sender == miCopyImageByWidth ? "Specify the desired bitmap width:" :
                sender == miCopyImageByHeight ? "Specify the desired bitmap height:" : Ut.Throw<string>(new InvalidOperationException()),

                (sender == miCopyImageByFont ? ZiimHelperProgram.Settings.LastCopyImageFontSize :
                sender == miCopyImageByWidth ? ZiimHelperProgram.Settings.LastCopyImageWidth :
                sender == miCopyImageByHeight ? ZiimHelperProgram.Settings.LastCopyImageHeight : Ut.Throw<int>(new InvalidOperationException())).ToString(),

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

            if (sender == miCopyImageByFont)
                ZiimHelperProgram.Settings.LastCopyImageFontSize = input;
            else if (sender == miCopyImageByWidth)
                ZiimHelperProgram.Settings.LastCopyImageWidth = input;
            else if (sender == miCopyImageByHeight)
                ZiimHelperProgram.Settings.LastCopyImageHeight = input;
            ZiimHelperProgram.Settings.SaveQuiet();

            if (_editingCloud.Items.Count == 0)
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
            var annotation = _selected.SelectMany(itm => itm.AllArrows).Select(arr => arr.Annotation).JoinString();
            var newAnnotation = InputBox.GetLine("Annotation:", annotation, "Annotation", "&OK", "&Cancel");
            if (newAnnotation != null)
            {
                foreach (var arr in _selected.SelectMany(itm => itm.AllArrows))
                {
                    arr.Annotation = string.IsNullOrWhiteSpace(newAnnotation) ? null : newAnnotation;
                    _fileChanged = true;
                }
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
            foreach (var inf in _modes)
                inf.MenuItem.Checked = (mode == inf.EditMode);
            setCursor();
        }

        private void setCursor()
        {
            if (miMoveSelect.Checked)
            {
                if (Ut.Ctrl || Ut.Shift)
                    ctImage.Cursor = Cursors.Default;
                else
                {
                    var mousePosition = ctImage.PointToClient(Control.MousePosition);
                    var x = _paintCellSize == 0 ? 0 : (mousePosition.X - _paintTarget.Left) / _paintCellSize + _paintMinX;
                    var y = _paintCellSize == 0 ? 0 : (mousePosition.Y - _paintTarget.Top) / _paintCellSize + _paintMinY;
                    ctImage.Cursor = _editingCloud.AllArrows.Any(a => a.X == x && a.Y == y) ? Cursors.SizeAll : Cursors.Default;
                }
            }
            else if (miDraw.Checked)
                ctImage.Cursor = Cursors.Cross;
            else if (miSetLabelPosition.Checked)
                ctImage.Cursor = Cursors.Hand;
        }

        private void switchMode(object sender, EventArgs __)
        {
            setMode(_modes.First(inf => sender == inf.MenuItem).EditMode);
            refresh();
        }

        private void fileNew(object _, EventArgs __)
        {
            if (!canDestroy())
                return;
            _filename = null;
            _file = new Cloud();
            _editingCloud = new Cloud();
            _outerClouds.Clear();
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
                DefaultExt = "ziimx",
                Multiselect = false,
                Title = "Open Ziim program",
                Filter = "Annotated Ziim programs|*.ziimx|Plain-text Ziim programs|*.ziim"
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
                var text = File.ReadAllText(filename);
                if (text.All(ch => " \r\n↖↑↗→↘↓↙←↕⤢↔⤡".Contains(ch)))
                {
                    // treat as plain-text program
                    var arrows = text.Replace("\r", "").Split('\n')
                        .SelectMany((line, lineNumber) => line.SelectMany((ch, chIndex) =>
                        {
                            var p = "↑↗→↘↓↙←↖".IndexOf(ch);
                            if (p != -1)
                                return new ArrowInfo[] { new SingleArrowInfo { X = chIndex, Y = lineNumber, Direction = (Direction) p, IsTerminalArrow = false, Marked = false } };
                            p = "↕⤢↔⤡".IndexOf(ch);
                            if (p != -1)
                                return new ArrowInfo[] { new DoubleArrowInfo { X = chIndex, Y = lineNumber, Direction = (DoubleDirection) p, Marked = false } };
                            return Enumerable.Empty<ArrowInfo>();
                        }));
                    _file = new Cloud(arrows);
                    _filename = filename + "x";
                }
                else
                {
                    _file = ClassifyXml.DeserializeFile<Cloud>(filename, _classifyOptions);
                    _filename = filename;
                }
                _editingCloud = _file;
                _outerClouds.Clear();
            }
            catch (Exception e)
            {
                DlgMessage.Show("The file could not be opened:\n\n" + e.Message, "Error", DlgType.Error);
                return;
            }
            _selected.Clear();
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
            ZiimHelperProgram.Settings.ViewInnerClouds = miInnerClouds.Checked;
            ZiimHelperProgram.Settings.ViewOwnCloud = miOwnCloud.Checked;
            ZiimHelperProgram.Settings.ViewCoordinates = miCoordinates.Checked;
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

        private void move(object sender, EventArgs __)
        {
            var xOffset = sender == miMoveLeft ? -1 : sender == miMoveRight ? 1 : 0;
            var yOffset = sender == miMoveUp ? -1 : sender == miMoveDown ? 1 : 0;
            foreach (var item in _selected)
                item.Move(xOffset, yOffset);
            _fileChanged = true;
            refresh();
        }

        private void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            setCursor();

            if (e.KeyData == Keys.Back)
            {
                backToOuterCloud();
                return;
            }

            var xOffset = e.KeyData == Keys.Left ? -1 : e.KeyData == Keys.Right ? 1 : 0;
            var yOffset = e.KeyData == Keys.Up ? -1 : e.KeyData == Keys.Down ? 1 : 0;
            if (xOffset != 0 || yOffset != 0)
            {
                foreach (var item in _selected)
                    item.Move(xOffset, yOffset);
                _fileChanged = true;
                refresh();
            }
        }

        private void backToOuterCloud(object _ = null, EventArgs __ = null)
        {
            if (_outerClouds.Count == 0)
            {
                DlgMessage.Show("You are already at the top level.", "Back to outer cloud", DlgType.Info);
                return;
            }

            _selected.Clear();
            _selected.Add(_editingCloud);
            _editingCloud = _outerClouds.Pop();
            refresh();
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
                DefaultExt = "ziimx",
                Multiselect = false,
                Title = "Import Ziim sub-program",
                Filter = "Annotated Ziim programs|*.ziimx|Plain-text Ziim programs|*.ziim"
            })
            {
                if (dlg.ShowDialog() == DialogResult.Cancel)
                    return;
                Cloud file;
                try
                {
                    file = XmlClassify.LoadObjectFromXmlFile<Cloud>(dlg.FileName);
                }
                catch (Exception e)
                {
                    DlgMessage.Show("The file could not be opened:\n\n" + e.Message, "Error", DlgType.Error);
                    return;
                }
                jiggle(file);
                _editingCloud.Items.Add(file);
                _selected.Clear();
                _selected.Add(file);
                _fileChanged = true;
                refresh();
            }
        }

        private void jiggle(params Item[] items)
        {
            var squaresTaken = new Dictionary<int, HashSet<int>>();
            foreach (var arrow in _editingCloud.AllArrows)
                squaresTaken.AddSafe(arrow.X, arrow.Y);
            int radius = 0, px = 0, py = 0;
            while (items.SelectMany(item => item.AllArrows).Any(arr => squaresTaken.Contains(arr.X, arr.Y)))
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
                foreach (var item in items)
                    item.Move(x - px, y - py);
                px = x;
                py = y;
            }
        }

        private void toggleTerminal(object _, EventArgs __)
        {
            bool any = false;
            foreach (var singleArrow in _selected.OfType<SingleArrowInfo>())
            {
                singleArrow.IsTerminalArrow = !singleArrow.IsTerminal;
                any = true;
            }
            if (any)
                refresh();
        }

        private void cloudColor(object _, EventArgs __)
        {
            using (var dlg = new ColorDialog { Color = _editingCloud.Color })
            {
                var result = dlg.ShowDialog();
                if (result == DialogResult.Cancel)
                    return;
                _editingCloud.Color = dlg.Color;
                _fileChanged = true;
                ctImage.Invalidate();
            }
        }

        private void setLabel(object _, EventArgs __)
        {
            var text = InputBox.GetLine("Enter text:", _editingCloud.Label ?? "", useMultilineBox: true);
            if (text != null)
            {
                _editingCloud.Label = string.IsNullOrEmpty(text) ? null : text;
                _fileChanged = true;
                refresh();
            }
        }

        private void editInnerCloud(object sender, EventArgs e)
        {
            Cloud newEditingCloud;
            if (_selected.Count != 1 || (newEditingCloud = _selected.First() as Cloud) == null)
                return;

            _outerClouds.Push(_editingCloud);
            _editingCloud = newEditingCloud;
            _selected.Clear();
            refresh();
        }

        private void cutOrCopy(object sender, EventArgs __)
        {
            if (_selected.Count > 0)
                Clipboard.SetText(ClassifyXml.Serialize(_selected.ToArray(), _classifyOptions).ToString());
            if (sender == miCut)
            {
                _editingCloud.Items.RemoveAll(_selected.Contains);
                _selected.Clear();
                _fileChanged = true;
                refresh();
            }
        }

        private void paste(object _, EventArgs __)
        {
            try
            {
                var clipboard = ClassifyXml.Deserialize<Item[]>(XElement.Parse(Clipboard.GetText()));
                jiggle(clipboard);
                _editingCloud.Items.AddRange(clipboard);
                _selected.Clear();
                _selected.AddRange(clipboard);
                _fileChanged = true;
                refresh();
            }
            catch (Exception e)
            {
                DlgMessage.Show("The contents of the clipboard cannot be pasted:{0}{0}{1}".Fmt(Environment.NewLine, e.Message), "Paste", DlgType.Error);
            }
        }
    }
}
