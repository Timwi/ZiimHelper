using System;
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
        private Cloud _editingCloud;
        private Stack<Cloud> _outerClouds = new Stack<Cloud>();
        private Cloud _file;
        private string _filename = null;
        private bool _fileChanged = false;
        private HashSet<Item> _selected = new HashSet<Item>();
        private Stack<UserAction> _undo = new Stack<UserAction>();
        private Stack<UserAction> _redo = new Stack<UserAction>();

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

            _file = _editingCloud = new Cloud();

            setUi();
            setMode(ZiimHelperProgram.Settings.EditMode);

            this.MouseWheel += mouseWheel;
        }

        private void deleteItem(object _, EventArgs __)
        {
            if (_selected.Count == 0)
                return;
            Do(new AddOrRemoveItems(ActionType.Remove, _selected, _editingCloud));
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
                using (var tr = new GraphicsTransformer(e.Graphics).Translate(_paintTarget.X, _paintTarget.Y))
                    drawArrow(e.Graphics, _arrowReorienting, _editingCloud, _paintCellSize, _paintFontSize, _editingCloud.Items.Contains(_arrowReorienting) ? Brushes.Orange : Brushes.Green);
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
        private float _zoomFactor = 1;
        private int _scrollX, _scrollY;

        private void paintBuffer(object _, PaintEventArgs e)
        {
            var margin = 15;

            e.Graphics.Clear(Color.Gray);
            if (_editingCloud.Items.Count < 1)
                return;

            _editingCloud.GetBounds(out _paintMinX, out _paintMaxX, out _paintMinY, out _paintMaxY);

            var fit = new Size(_paintMaxX - _paintMinX + 1, _paintMaxY - _paintMinY + 1)
                .FitIntoMaintainAspectRatio(new Rectangle(
                    margin, margin,
                    (int) ((ctImage.ClientSize.Width - 2 * margin) * _zoomFactor),
                    (int) ((ctImage.ClientSize.Height - 2 * margin) * _zoomFactor)));
            var w = fit.Width - fit.Width % (_paintMaxX - _paintMinX + 1);
            var h = fit.Height - fit.Height % (_paintMaxY - _paintMinY + 1);

            _paintTarget = new Rectangle(ctImage.ClientSize.Width / 2 - w / 2 - _scrollX, ctImage.ClientSize.Height / 2 - h / 2 - _scrollY, w, h);
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

            // GRID
            if (miGrid.Checked)
                using (var pen = new Pen(Color.FromArgb(0xEE, 0xEE, 0xEE)))
                {
                    for (int i = 1; i <= _paintMaxX - _paintMinX; i++)
                        g.DrawLine(pen, i * cellSize, 0, i * cellSize, (_paintMaxY - _paintMinY + 1) * cellSize);
                    for (int j = 1; j <= _paintMaxY - _paintMinY; j++)
                        g.DrawLine(pen, 0, j * cellSize, (_paintMaxX - _paintMinX + 1) * cellSize, j * cellSize);
                }

            // CLOUDS (own and inner; not including terminals)
            if (miInnerClouds.Checked || miOwnCloud.Checked)
                using (var tr = new GraphicsTransformer(g).Translate(-_paintMinX * cellSize, -_paintMinY * cellSize))
                    foreach (var cloud in miInnerClouds.Checked ? _editingCloud.AllClouds : new[] { _editingCloud })
                        if (cloud != _editingCloud || miOwnCloud.Checked)
                            cloud.DrawCloud(g, cellSize, cloud == _editingCloud);

            // Create a dictionary to remember which directions each arrow is being pointed at from
            var pointedAtFromDirectionsDic = new Dictionary<ArrowInfo, List<Direction>>();
            foreach (var inf in _editingCloud.ArrowsWithParents)
            {
                var arr = inf.Item1;
                var parentCloud = inf.Item2;

                if (arr.IsTerminal && parentCloud != _editingCloud)
                    continue;

                foreach (var dir in arr.Directions)
                {
                    var pointTo = getPointTo(arr.X, arr.Y, dir.XOffset(), dir.YOffset());
                    if (pointTo != null)
                        pointedAtFromDirectionsDic.AddSafe(pointTo, dir);
                }
            }

            // INSTRUCTIONS (requires pointedAtFromDirectionsDic populated earlier)
            // Also creates a dictionary to remember which instruction each arrow is
            var instructionDic = new Dictionary<ArrowInfo, string>();
            if (miInstructions.Checked || miConnectionLines.Checked)
            {
                foreach (var inf in _editingCloud.ArrowsWithParents)
                {
                    var arrow = inf.Item1;
                    if (arrow.IsTerminal)
                        continue;
                    var parentCloud = inf.Item2;
                    var x = (arrow.X - _paintMinX) * cellSize;
                    var y = (arrow.Y - _paintMinY) * cellSize;

                    List<Direction> directions;
                    if (arrow.IsTerminal || !pointedAtFromDirectionsDic.TryGetValue(arrow, out directions))
                        directions = null;
                    string instruction = null;
                    Direction dir = 0;
                    int yn = 0;

                    Ut.IfType(arrow,
                        (SingleArrowInfo arr) =>
                        {
                            dir = arr.Direction;
                            var relativeDirections = directions.NullOr(dirs => dirs.Select(d => ((int) d - (int) dir + 8) % 8).ToArray());
                            instruction =
                                directions == null ? "0" :
                                directions.Count == 1 ?
                                    relativeDirections[0] == 1 ? "R" :
                                    relativeDirections[0] == 3 ? "I" :
                                    relativeDirections[0] == 5 ? "N" : null :
                                directions.Count == 2 ?
                                    relativeDirections.Contains(1) && relativeDirections.Contains(7) ? "C" :
                                    relativeDirections.Contains(3) && relativeDirections.Contains(5) ? "L" : null : null;
                        },
                        (DoubleArrowInfo arr) =>
                        {
                            if (directions == null || directions.Count != 1)
                                return;
                            switch (((int) directions[0] - (int) arr.Direction.GetDirection1() + 8) % 8)
                            {
                                case 2: instruction = "S"; dir = arr.Direction.GetDirection1(); break;
                                case 6: instruction = "S"; dir = arr.Direction.GetDirection2(); break;
                                case 1: instruction = "Z2"; dir = arr.Direction.GetDirection1(); yn = -1; break;
                                case 5: instruction = "Z1"; dir = arr.Direction.GetDirection2(); yn = -1; break;
                                case 3: instruction = "E1"; dir = arr.Direction.GetDirection1(); yn = 1; break;
                                case 7: instruction = "E2"; dir = arr.Direction.GetDirection2(); yn = 1; break;
                            }
                        }
                    );

                    if (miInstructions.Checked)
                    {
                        if (instruction == null)
                            // Mark invalid arrows with a semitransparent red circle
                            g.FillEllipse(new SolidBrush(Color.FromArgb(64, 255, 128, 128)), x, y, cellSize, cellSize);
                        else
                        {
                            g.DrawString(instruction.Substring(0, 1), new Font(_instructionFont, fontSize / 2), Brushes.Black,
                                (float) (x + cellSize / 2 + Math.Cos(Math.PI / 4 * (int) dir) * cellSize / 4),
                                (float) (y + cellSize / 2 + Math.Sin(Math.PI / 4 * (int) dir) * cellSize / 4),
                                Util.CenterCenter
                            );
                            if (yn != 0)
                            {
                                g.DrawString("y", new Font(_instructionFont, fontSize / 4), Brushes.Black,
                                    (float) (x + cellSize / 2 + Math.Cos(Math.PI / 4 * ((int) dir + 4 - yn)) * cellSize / 3),
                                    (float) (y + cellSize / 2 + Math.Sin(Math.PI / 4 * ((int) dir + 4 - yn)) * cellSize / 3),
                                    Util.CenterCenter
                                );
                                g.DrawString("n", new Font(_instructionFont, fontSize / 4), Brushes.Black,
                                    (float) (x + cellSize / 2 + Math.Cos(Math.PI / 4 * ((int) dir + 4 + yn)) * cellSize / 3),
                                    (float) (y + cellSize / 2 + Math.Sin(Math.PI / 4 * ((int) dir + 4 + yn)) * cellSize / 3),
                                    Util.CenterCenter
                                );
                            }
                            else if (instruction == "C")
                            {
                                g.DrawString("A", new Font(_instructionFont, fontSize / 4), Brushes.Black,
                                    (float) (x + cellSize / 2 + Math.Cos(Math.PI / 4 * ((int) dir + 1)) * cellSize / 3),
                                    (float) (y + cellSize / 2 + Math.Sin(Math.PI / 4 * ((int) dir + 1)) * cellSize / 3),
                                    Util.CenterCenter
                                );
                                g.DrawString("B", new Font(_instructionFont, fontSize / 4), Brushes.Black,
                                    (float) (x + cellSize / 2 + Math.Cos(Math.PI / 4 * ((int) dir + 3)) * cellSize / 3),
                                    (float) (y + cellSize / 2 + Math.Sin(Math.PI / 4 * ((int) dir + 3)) * cellSize / 3),
                                    Util.CenterCenter
                                );
                            }
                        }
                    }

                    if (instruction != null)
                        instructionDic.Add(arrow, instruction);
                }
            }

            // COORDINATES and CONNECTION LINES
            if (miCoordinates.Checked || miConnectionLines.Checked)
            {
                var colors = Ut.NewArray(
                    // constant 0
                    Tuple.Create(Color.FromArgb(0x10, 0x60, 0xa0), DashStyle.Solid),
                    // constant 1
                    Tuple.Create(Color.FromArgb(0x10, 0x60, 0xa0), DashStyle.Dash),
                    // “yes” output
                    Tuple.Create(Color.FromArgb(0x10, 0xa0, 0x10), DashStyle.Solid),
                    // inverted “yes” output
                    Tuple.Create(Color.FromArgb(0x10, 0xa0, 0x10), DashStyle.Dash),
                    // “no” output
                    Tuple.Create(Color.FromArgb(0xa0, 0x20, 0x40), DashStyle.Solid),
                    // inverted “no” output
                    Tuple.Create(Color.FromArgb(0xa0, 0x20, 0x40), DashStyle.Dash),
                    // concat or label output
                    Tuple.Create(Color.FromArgb(0xa0, 0xa0, 0xa0), DashStyle.Solid),
                    // inverted concat or label output
                    Tuple.Create(Color.FromArgb(0xa0, 0xa0, 0xa0), DashStyle.Dash)
                );

                var q = new Queue<Tuple<ArrowInfo, int, bool>>();

                foreach (var inf in _editingCloud.ArrowsWithParents)
                {
                    var arr = inf.Item1;
                    var parentCloud = inf.Item2;
                    var instruction = instructionDic.Get(arr, "X");

                    // COORDINATES
                    if (miCoordinates.Checked)
                        g.DrawString(arr.CoordsString, new Font(_annotationFont, fontSize / 4), Brushes.Black, (arr.X - _paintMinX) * cellSize, (arr.Y - _paintMinY) * cellSize);

                    // Populate the queue
                    if (miConnectionLines.Checked && (!arr.IsTerminal || parentCloud == _editingCloud) && (instruction != "N" && instruction != "I" && instruction != "S" && instruction != "L" && instruction != "C"))
                        q.Enqueue(Tuple.Create(arr, -1, instruction[0] == 'E'));
                }

                // CONNECTION LINES
                if (miConnectionLines.Checked)
                {
                    // 1/2 = waiting with knownEmpty=false/true; 3 = released by loop detector
                    var waitingLsCs = new Dictionary<ArrowInfo, int>();

                    while (q.Count > 0 || waitingLsCs.Count > 0)
                    {
                        ArrowInfo arr;
                        var col = 6;
                        var knownEmpty = false;

                        if (q.Count == 0)
                        {
                            arr = waitingLsCs.FirstOrDefault(kvp => kvp.Value != 3).Key;
                            if (arr == null)
                                System.Diagnostics.Debugger.Break();
                            waitingLsCs[arr] = 3;
                        }
                        else
                        {
                            var item = q.Dequeue();
                            arr = item.Item1;
                            col = item.Item2;
                            knownEmpty = item.Item3;
                        }
                        var flip = false;

                        foreach (var dir in arr.Directions)
                        {
                            switch (instructionDic.Get(arr, "X"))
                            {
                                case "0": col = 0; knownEmpty = false; break;

                                case "Z1": col = flip ? 2 : 4; knownEmpty = false; break;
                                case "E1": col = flip ? 2 : 4; knownEmpty = true; break;
                                case "Z2": col = flip ? 4 : 2; knownEmpty = false; break;
                                case "E2": col = flip ? 4 : 2; knownEmpty = true; break;

                                case "R":
                                case "X": col = 6; knownEmpty = false; break;

                                case "C":
                                case "L": col = 6; break;

                                case "I": col = knownEmpty ? (col & ~1) : (col ^ 1); break;
                            }

                            var pointTo = getPointTo(arr.X, arr.Y, dir.XOffset(), dir.YOffset());
                            var toX = pointTo == null ? arr.X + maxSize * dir.XOffset() : pointTo.X;
                            var toY = pointTo == null ? arr.Y + maxSize * dir.YOffset() : pointTo.Y;
                            while (toX < _paintMinX - 1 || toY < _paintMinY - 1 || toX > _paintMaxX + 1 || toY > _paintMaxY + 1)
                            {
                                toX -= dir.XOffset();
                                toY -= dir.YOffset();
                            }
                            using (var pen = new Pen(colors[col].Item1, 1f) { EndCap = knownEmpty ? LineCap.RoundAnchor : LineCap.ArrowAnchor, DashStyle = colors[col].Item2 })
                            {
                                g.DrawLine(pen,
                                    cellSize * (arr.X - _paintMinX) + cellSize / 2 + dir.XOffset() * cellSize * 4 / 10,
                                    cellSize * (arr.Y - _paintMinY) + cellSize / 2 + dir.YOffset() * cellSize * 4 / 10,
                                    cellSize * (toX - _paintMinX) + cellSize / 2 - dir.XOffset() * cellSize * 4 / 10,
                                    cellSize * (toY - _paintMinY) + cellSize / 2 - dir.YOffset() * cellSize * 4 / 10);
                            }

                            if (pointTo != null && instructionDic.ContainsKey(pointTo))
                            {
                                if (instructionDic[pointTo] == "N" || instructionDic[pointTo] == "I" || instructionDic[pointTo] == "S")
                                    q.Enqueue(Tuple.Create(pointTo, col, knownEmpty));
                                else if (instructionDic[pointTo] == "L" || instructionDic[pointTo] == "C")
                                {
                                    var prevKnownEmpty = true;
                                    switch (waitingLsCs.Get(pointTo, 0))
                                    {
                                        case 0:
                                            waitingLsCs.Add(pointTo, knownEmpty ? 2 : 1);
                                            break;
                                        case 1:
                                            prevKnownEmpty = false;
                                            goto case 2;
                                        case 2:
                                            q.Enqueue(Tuple.Create(pointTo, col, knownEmpty && prevKnownEmpty));
                                            goto case 3;
                                        case 3:
                                            waitingLsCs.Remove(pointTo);
                                            break;
                                    }
                                }
                            }
                            flip = true;
                        }
                    }
                }
            }

            using (var fillBrush = new SolidBrush(Color.FromArgb(0xEE, 0xEE, 0xFF)))
            using (var pen = new Pen(Color.Blue, 1))
            using (var font = new Font(_annotationFont, 8))
            using (var textBrush = new SolidBrush(Color.DarkBlue))
                foreach (var inf in _editingCloud.ArrowsWithParents.OrderBy(awp => !awp.Item1.IsTerminal))
                {
                    var arrow = inf.Item1;
                    var parentCloud = inf.Item2;
                    var x = (arrow.X - _paintMinX) * cellSize;
                    var y = (arrow.Y - _paintMinY) * cellSize;

                    // ARROWS (including terminals)
                    if (!arrow.IsTerminal || (parentCloud == _editingCloud ? miOwnCloud : miInnerClouds).Checked)
                        drawArrow(g, arrow, parentCloud, cellSize, fontSize);

                    // ANNOTATIONS for TERMINALS
                    if (arrow.IsTerminal && arrow.Annotation != null && (parentCloud == _editingCloud ? miOwnCloud : miInnerClouds).Checked)
                    {
                        var p = new[] { new Point(0, 0) };
                        g.Transform.TransformPoints(p);
                        using (var tr = new GraphicsTransformer(g).Translate(0, -cellSize / 4).RotateAt(45 * ((int) ((SingleArrowInfo) arrow).Direction % 4 - 2), p[0]).Translate(x + cellSize / 2, y + cellSize / 2))
                            g.DrawString(
                                arrow.Annotation,
                                new Font(_annotationFont, g.GetMaximumFontSize(new SizeF(cellSize * 4 / 5, cellSize * 4 / 5), _annotationFont, arrow.Annotation)),
                                arrow.IsTerminal ? new SolidBrush(parentCloud.Color) : Brushes.Black,
                                0, 0,
                                Util.CenterCenter
                            );
                    }

                    // ANNOTATIONS for NON-TERMINALS
                    if (miAnnotations.Checked && arrow.Annotation != null && !arrow.IsTerminal)
                    {
                        var size = g.MeasureString(arrow.Annotation, font, int.MaxValue, Util.CenterCenter) + new SizeF(6, 2);
                        var location = new PointF((arrow.X - _paintMinX) * cellSize + cellSize / 2 - size.Width / 2, (arrow.Y - _paintMinY) * cellSize - size.Height / 2);
                        var path = GraphicsUtil.RoundedRectangle(new RectangleF(location, size), Math.Min(size.Width, size.Height) / 3);
                        g.FillPath(fillBrush, path);
                        g.DrawPath(pen, path);
                        g.DrawString(arrow.Annotation, font, textBrush, new RectangleF(location + new SizeF(3, 1), size - new SizeF(6, 2)), Util.CenterCenter);
                    }
                }

            /// “!!!” for arrows that overlap
            foreach (var pair in _editingCloud.AllArrows.UniquePairs())
                if (pair.Item1.X == pair.Item2.X && pair.Item1.Y == pair.Item2.Y && pair.Item1.IsTerminal == pair.Item2.IsTerminal)
                    g.DrawString("!!!", new Font(_arrowFont, fontSize, FontStyle.Italic), Brushes.Red,
                        cellSize * (pair.Item1.X - _paintMinX) + cellSize / 2, cellSize * (pair.Item1.Y - _paintMinY) + cellSize / 2,
                        Util.CenterCenter);
        }

        private void drawArrow(Graphics g, ArrowInfo arrow, Cloud parentCloud, int cellSize, float fontSize, Brush brush = null)
        {
            g.DrawString(
                arrow.Character.ToString(),
                new Font(_arrowFont, fontSize),
                arrow.IsTerminal ? new SolidBrush(parentCloud.Color) : (brush ?? (arrow.Marked ? Brushes.Red : Brushes.Black)),
                (arrow.X - _paintMinX) * cellSize + cellSize / 2, (arrow.Y - _paintMinY) * cellSize + cellSize / 2,
                Util.CenterCenter
            );
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

        private void rotate(object sender, EventArgs __)
        {
            if (_selected.Count == 0)
                return;

            var clockwise = sender == miRotateClockwise;
            ArrowInfo singleArrow;
            if (_selected.Count == 1 && (singleArrow = _selected.Single() as ArrowInfo) != null)
                Do(new RotateArrow(singleArrow, clockwise));
            else
                Do(new RotateItems(_selected.SelectMany(itm => itm.AllItems), clockwise));
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
        private bool _draggingLabelPositionFrom;
        private bool _draggingLabelPositionTo;
        private Point _mouseDown;
        private Point _mouseDraggedTo;
        private Item _mouseMoving;
        private int _mouseOldX;
        private int _mouseOldY;

        // If non-null, the user is currently dragging the mouse to rotate (reorient) an arrow.
        // If this arrow is contained in _editingCloud.Items, the arrow already existed, otherwise it will be added during mouseUp.
        private ArrowInfo _arrowReorienting;
        private Direction _arrowReorientingOriginalDirection;
        private DoubleDirection _arrowReorientingOriginalDoubleDirection;

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
                if (clickedOn is ArrowInfo)
                {
                    // Existing arrow: reorient it
                    _arrowReorienting = (ArrowInfo) clickedOn;
                    if (clickedOn is SingleArrowInfo)
                        _arrowReorientingOriginalDirection = ((SingleArrowInfo) clickedOn).Direction;
                    else
                        _arrowReorientingOriginalDoubleDirection = ((DoubleArrowInfo) clickedOn).Direction;
                }
                else
                    // Create a new arrow; it will be added to _editingCloud.Items during mouse up
                    _arrowReorienting = Ut.Shift ? (ArrowInfo) new DoubleArrowInfo { X = x, Y = y } : new SingleArrowInfo { X = x, Y = y };
                _selected.Clear();
                ctImage.Invalidate();
            }
            else if (miSetLabelPosition.Checked)
            {
                var fromDist = dist(x - _editingCloud.LabelFromX, y - _editingCloud.LabelFromY);
                var toDist = dist(x - _editingCloud.LabelToX, y - _editingCloud.LabelToY);

                if (fromDist < toDist)
                {
                    _mouseOldX = _editingCloud.LabelFromX;
                    _mouseOldY = _editingCloud.LabelFromY;
                    _draggingLabelPositionFrom = true;
                    _editingCloud.LabelFromX = x;
                    _editingCloud.LabelFromY = y;
                }
                else
                {
                    _mouseOldX = _editingCloud.LabelToX;
                    _mouseOldY = _editingCloud.LabelToY;
                    _draggingLabelPositionTo = true;
                    _editingCloud.LabelToX = x;
                    _editingCloud.LabelToY = y;
                }
                refresh();
            }
        }

        private double dist(double xDist, double yDist)
        {
            return Math.Sqrt(xDist * xDist + yDist * yDist);
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

            if (_draggingLabelPositionFrom)
            {
                _mouseDraggedTo.X = _editingCloud.LabelFromX = x;
                _mouseDraggedTo.Y = _editingCloud.LabelFromY = y;
                refresh();
                return;
            }
            else if (_draggingLabelPositionTo)
            {
                _mouseDraggedTo.X = _editingCloud.LabelToX = x;
                _mouseDraggedTo.Y = _editingCloud.LabelToY = y;
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
                Do(_selected.Select(item => item.GetMoveAction(deltaX, deltaY)));
            else if (_arrowReorienting != null)
            {
                _arrowReorienting.Reorient(
                    2 * (y - _arrowReorienting.Y) > x - _arrowReorienting.X,      //~\_
                    y - _arrowReorienting.Y > 2 * (x - _arrowReorienting.X),     // \
                    y - _arrowReorienting.Y > 2 * (_arrowReorienting.X - x),     // /
                    2 * (y - _arrowReorienting.Y) > _arrowReorienting.X - x     // _/~
                );
                ctImage.Invalidate();
            }
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            if (_draggingLabelPositionFrom)
            {
                Do(new MoveLabel(_editingCloud, true, _mouseOldX, _mouseOldY, _editingCloud.LabelFromX, _editingCloud.LabelFromY));
                _draggingLabelPositionFrom = false;
            }
            else if (_draggingLabelPositionTo)
            {
                Do(new MoveLabel(_editingCloud, false, _mouseOldX, _mouseOldY, _editingCloud.LabelToX, _editingCloud.LabelToY));
                _draggingLabelPositionTo = false;
            }
            else if (_draggingSelectionRectangle)
            {
                if (!Ut.Shift)
                    _selected.Clear();

                Item alreadyItem;
                if (_mouseDown.X == _mouseDraggedTo.X && _mouseDown.Y == _mouseDraggedTo.Y && (alreadyItem = _editingCloud.ItemAt(_mouseDraggedTo.X, _mouseDraggedTo.Y)) != null)
                {
                    if (!_selected.Remove(alreadyItem))
                        _selected.Add(alreadyItem);
                }
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
            else if (_arrowReorienting != null)
            {
                if (_editingCloud.Items.Contains(_arrowReorienting))
                {
                    // An existing arrow is reoriented
                    _arrowReorienting.IfType(
                        (SingleArrowInfo sa) =>
                        {
                            if (_arrowReorientingOriginalDirection != sa.Direction)
                                Do(new ReorientSingleArrow(sa, _arrowReorientingOriginalDirection, sa.Direction));
                            else
                                _selected.Add(_arrowReorienting);
                        },
                        (DoubleArrowInfo da) =>
                        {
                            if (_arrowReorientingOriginalDoubleDirection != da.Direction)
                                Do(new ReorientDoubleArrow(da, _arrowReorientingOriginalDoubleDirection, da.Direction));
                            else
                                _selected.Add(_arrowReorienting);
                        },
                        arrow => { throw new InvalidOperationException("Unknown arrow type."); });
                }
                else
                {
                    // A new arrow is created
                    Do(new AddOrRemoveItems(ActionType.Add, _arrowReorienting, _editingCloud));
                }
                _arrowReorienting = null;
                refresh();
            }
            else if (_mouseMoving != null)
            {
                _mouseMoving = null;
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

        private void toggleMark(object _, EventArgs __)
        {
            if (_selected.Count > 0)
                Do(new MultiAction(_selected.SelectMany(itm => itm.AllArrows).Select(arrow => new ToggleMark(arrow))));
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
                        var sizes = "↖↑↗→↘↓↙←↕⤢↔⤡".Select(ch => g.MeasureString(ch.ToString(), tryFont)).Select(sz => sz.Width).ToArray();
                        var size = sizes.Max();
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
                    cellWidth = Math.Max(cellWidth, (int) g.MeasureString(ch.ToString(), font).Width);
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
            if (_selected.Count == 0)
            {
                DlgMessage.Show("Nothing selected.", "Error", DlgType.Error);
                return;
            }
            if (_selected.OfType<Cloud>().Any())
            {
                DlgMessage.Show("Cannot annotate a cloud.", "Error", DlgType.Error);
                return;
            }

            var annotation = _selected.SelectMany(itm => itm.AllArrows).Select(arr => arr.Annotation).FirstOrDefault(ann => !string.IsNullOrWhiteSpace(ann));
            var newAnnotation = InputBox.GetLine("Annotation:", annotation ?? "", "Annotation", "&OK", "&Cancel");
            if (newAnnotation != null)
                Do(new MultiAction(_selected.SelectMany(itm => itm.AllArrows).Select(arrow => new ArrowAnnotation(arrow, arrow.Annotation, newAnnotation == "" ? null : newAnnotation))));
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
            _file = _editingCloud = new Cloud();
            _outerClouds.Clear();
            _selected.Clear();
            _fileChanged = false;
            _undo.Clear();
            _redo.Clear();
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
                _zoomFactor = 1;
                _scrollX = _scrollY = 0;
                return;
            }
        }

        private Cloud readFile(ref string filename)
        {
            var text = File.ReadAllText(filename);
            if (!text.All(ch => " \r\n↖↑↗→↘↓↙←↕⤢↔⤡".Contains(ch)))
                return ClassifyXml.DeserializeFile<Cloud>(filename, _classifyOptions);

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

            // Next time this is saved, save it as XML
            filename = filename + "x";
            return new Cloud(arrows);
        }

        private void fileOpen(string filename)
        {
            try
            {
                var cloud = readFile(ref filename);
                _file = cloud;
            }
            catch (Exception e)
            {
                DlgMessage.Show("The file could not be opened:\n\n" + e.Message, "Error", DlgType.Error);
                return;
            }
            _filename = filename;
            _editingCloud = _file;
            _outerClouds.Clear();
            _selected.Clear();
            _fileChanged = false;
            _undo.Clear();
            _redo.Clear();
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
            if (_selected.Count > 0)
                Do(_selected.Select(item => item.GetMoveAction(
                    sender == miMoveLeft ? -1 : sender == miMoveRight ? 1 : 0,
                    sender == miMoveUp ? -1 : sender == miMoveDown ? 1 : 0)));
        }

        private void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Back)
                backToOuterCloud();
            else if (e.KeyData == (Keys.Back | Keys.Alt))
                undo();
            else if (e.KeyData == (Keys.Back | Keys.Alt | Keys.Shift))
                redo();
            else if (_selected.Count > 0)
            {
                var xOffset = e.KeyData == Keys.Left ? -1 : e.KeyData == Keys.Right ? 1 : 0;
                var yOffset = e.KeyData == Keys.Up ? -1 : e.KeyData == Keys.Down ? 1 : 0;
                if (xOffset != 0 || yOffset != 0)
                    Do(_selected.Select(item => item.GetMoveAction(xOffset, yOffset)));
            }
            setCursor();
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
                try
                {
                    var filename = dlg.FileName;
                    var cloud = readFile(ref filename);
                    Do(new AddOrRemoveItems(ActionType.Add, cloud, _editingCloud));
                }
                catch (Exception e)
                {
                    DlgMessage.Show("The file could not be opened:\n\n" + e.Message, "Error", DlgType.Error);
                    return;
                }
            }
        }

        private void toggleTerminal(object _, EventArgs __)
        {
            var actions = _selected.SelectMany(itm => itm.AllArrows).OfType<SingleArrowInfo>().Select(arrow => new ToggleTerminal(arrow));
            if (actions.Any())
                Do(new MultiAction(actions));
        }

        private void cloudColor(object _, EventArgs __)
        {
            using (var dlg = new ColorDialog { Color = _editingCloud.Color })
            {
                var result = dlg.ShowDialog();
                if (result == DialogResult.OK && dlg.Color != _editingCloud.Color)
                    Do(new CloudColor(_editingCloud, _editingCloud.Color, dlg.Color));
            }
        }

        private void setLabel(object _, EventArgs __)
        {
            var text = InputBox.GetLine("Enter text:", _editingCloud.Label ?? "", useMultilineBox: true);
            if (text == null)
                return;
            if (string.IsNullOrWhiteSpace(text))
                text = null;
            if (text != _editingCloud.Label)
                Do(new CloudLabel(_editingCloud, _editingCloud.Label, text));
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
            if (_selected.Count == 0)
                return;
            Clipboard.SetText(ClassifyXml.Serialize(_selected.ToArray(), _classifyOptions).ToString());
            if (sender == miCut)
                Do(new AddOrRemoveItems(ActionType.Remove, _selected, _editingCloud));
        }

        private void paste(object _, EventArgs __)
        {
            try
            {
                var clipboard = ClassifyXml.Deserialize<Item[]>(XElement.Parse(Clipboard.GetText()), _classifyOptions);
                Do(new AddOrRemoveItems(ActionType.Add, clipboard, _editingCloud));
            }
            catch (Exception e)
            {
                DlgMessage.Show("The contents of the clipboard cannot be pasted:{0}{0}{1}".Fmt(Environment.NewLine, e.Message), "Paste", DlgType.Error);
            }
        }

        private void resetZoom(object sender, EventArgs e)
        {
            _zoomFactor = 1;
            _scrollX = 0;
            _scrollY = 0;
            refresh();
        }

        private void zoomIn(object sender, EventArgs e)
        {
            changeZoom(1.1);
            refresh();
        }

        private void zoomOut(object sender, EventArgs e)
        {
            changeZoom(1 / 1.1);
            refresh();
        }

        private void mouseWheel(object sender, MouseEventArgs e)
        {
            if (Ut.Ctrl)
                changeZoom(Math.Pow(1.1, e.Delta / 120));
            else if (Ut.Shift)
                _scrollX -= e.Delta;
            else
                _scrollY -= e.Delta;
            refresh();
        }

        private void changeZoom(double factor)
        {
            _zoomFactor *= (float) factor;
            _scrollX = (int) (_scrollX * factor);
            _scrollY = (int) (_scrollY * factor);
            refresh();
        }

        private void undo(object _ = null, EventArgs __ = null)
        {
            if (_undo.Count == 0)
                return;
            var action = _undo.Pop();
            action.Undo();
            _selected = action.Selection.Intersect(_editingCloud.Items).ToHashSet();
            _redo.Push(action);
            refresh();
        }

        private void redo(object _ = null, EventArgs __ = null)
        {
            if (_redo.Count == 0)
                return;
            var action = _redo.Pop();
            action.Do();
            _selected = action.Selection.Intersect(_editingCloud.Items).ToHashSet();
            _undo.Push(action);
            refresh();
        }

        private void Do(IEnumerable<UserAction> actions)
        {
            Do(new MultiAction(actions));
        }

        private void Do(UserAction action)
        {
            _fileChanged = true;
            _undo.Push(action);
            _redo.Clear();
            action.Do();
            _selected = action.Selection.Intersect(_editingCloud.Items).ToHashSet();
            refresh();
        }
    }
}
