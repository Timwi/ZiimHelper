using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RT.Util;
using System.Drawing.Drawing2D;

namespace ZiimHelper
{
    static class Util
    {
        public static readonly StringFormat CenterCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        public static GraphicsPath CloudPath(Virtual2DArray<bool> cloud, int cellSize, float margin)
        {
            var outlines = boolsToPaths(cloud);
            var diameter = new SizeF(cellSize / 2, cellSize / 2);
            var result = new GraphicsPath();
            if (diameter.Width == 0 || diameter.Height == 0)
                return result;
            for (int i = 0; i < outlines.Length; i++)
            {
                result.StartFigure();
                for (int j = 0; j < outlines[i].Length; j++)
                {
                    var point1 = outlines[i][j];
                    var point2 = outlines[i][(j + 1) % outlines[i].Length];
                    var point3 = outlines[i][(j + 2) % outlines[i].Length];
                    var rect = new Rectangle(point2.X * cellSize, point2.Y * cellSize, cellSize, cellSize);

                    int dir1 = getDir(point1, point2);
                    int dir2 = getDir(point2, point3);

                    // Rounded corners ("outer" corners)
                    if (dir1 == 0 && dir2 == 2) // top left corner
                        result.AddArc(rect.X + margin, rect.Y + margin, diameter.Width, diameter.Height, 180, 90);
                    else if (dir1 == 2 && dir2 == 3)  // top right corner
                        result.AddArc(rect.X - margin - diameter.Width, rect.Y + margin, diameter.Width, diameter.Height, 270, 90);
                    else if (dir1 == 3 && dir2 == 1) // bottom right corner
                        result.AddArc(rect.X - margin - diameter.Width, rect.Y - margin - diameter.Height, diameter.Width, diameter.Height, 0, 90);
                    else if (dir1 == 1 && dir2 == 0) // bottom left corner
                        result.AddArc(rect.X + margin, rect.Y - margin - diameter.Height, diameter.Width, diameter.Height, 90, 90);

                    // Unrounded corners ("inner" corners)
                    else if (dir1 == 1 && dir2 == 3) // top left corner
                        result.AddLine(rect.X - margin, rect.Y - margin, rect.X - margin, rect.Y - margin);
                    else if (dir1 == 0 && dir2 == 1) // top right corner
                        result.AddLine(rect.X + margin, rect.Y - margin, rect.X + margin, rect.Y - margin);
                    else if (dir1 == 2 && dir2 == 0) // bottom right corner
                        result.AddLine(rect.X + margin, rect.Y + margin, rect.X + margin, rect.Y + margin);
                    else if (dir1 == 3 && dir2 == 2) // bottom left corner
                        result.AddLine(rect.X - margin, rect.Y + margin, rect.X - margin, rect.Y + margin);
                }
                result.CloseFigure();
            }
            return result;
        }

        private static int getDir(Point from, Point to)
        {
            return from.X == to.X
                ? (from.Y > to.Y ? 0 : 3)
                : (from.X > to.X ? 1 : 2);
        }

        private static Point[][] boolsToPaths(Virtual2DArray<bool> input)
        {
            int width = input.Width;
            int height = input.Height;

            var results = new List<Point[]>();
            var visitedUpArrow = Ut.NewArray<bool>(width, height);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    // every region must have at least one up arrow (left edge)
                    if (!visitedUpArrow[i][j] && input.Get(i, j) && !input.Get(i - 1, j))
                        results.Add(tracePolygon(input, i, j, visitedUpArrow));

            return results.ToArray();
        }

        private static Point[] tracePolygon(Virtual2DArray<bool> input, int i, int j, bool[][] visitedUpArrow)
        {
            var result = new List<Point>();
            var dir = Direction.Up;

            while (true)
            {
                // In each iteration of this loop, we move from the current edge to the next one.
                // We have to prioritise right-turns so that the diagonal-adjacent case is handled correctly.
                // Every time we take a 90° turn, we add the corner coordinate to the result list.
                // When we get back to the original edge, the polygon is complete.
                switch (dir)
                {
                    case Direction.Up:
                        // If we’re back at the beginning, we’re done with this polygon
                        if (visitedUpArrow[i][j])
                            return result.ToArray();

                        visitedUpArrow[i][j] = true;

                        if (!input.Get(i, j - 1))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Right;
                        }
                        else if (input.Get(i - 1, j - 1))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Left;
                            i--;
                        }
                        else
                            j--;
                        break;

                    case Direction.Down:
                        j++;
                        if (!input.Get(i - 1, j))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Left;
                            i--;
                        }
                        else if (input.Get(i, j))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Right;
                        }
                        break;

                    case Direction.Left:
                        if (!input.Get(i - 1, j - 1))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Up;
                            j--;
                        }
                        else if (input.Get(i - 1, j))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Down;
                        }
                        else
                            i--;
                        break;

                    case Direction.Right:
                        i++;
                        if (!input.Get(i, j))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Down;
                        }
                        else if (input.Get(i, j - 1))
                        {
                            result.Add(new Point(i, j));
                            dir = Direction.Up;
                            j--;
                        }
                        break;
                }
            }
        }
    }

    /// <summary>Encapsulates a two-dimensional array of values of a specified type which are retrieved using a Get() method.</summary>
    /// <remarks><see cref="MoveFinder"/> and <see cref="PushFinder"/> implement this.</remarks>
    interface Virtual2DArray<T>
    {
        /// <summary>Width of the array.</summary>
        int Width { get; }
        /// <summary>Height of the array.</summary>
        int Height { get; }
        /// <summary>Method to retrieve a value from the array.</summary>
        /// <param name="x">Index along the x-axis.</param>
        /// <param name="y">Index along the y-axis.</param>
        /// <returns>The value at the position (x, y) in the array.</returns>
        T Get(int x, int y);
    }

    sealed class Virtual2DArrayImpl : Virtual2DArray<bool>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private Func<int, int, bool> _get;
        public bool Get(int x, int y) { return _get(x, y); }
        public Virtual2DArrayImpl(Func<int, int, bool> get) { _get = get; }
    }
}
