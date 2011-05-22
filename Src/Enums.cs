using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphiteHelper
{
    enum Direction
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    }

    enum DoubleDirection
    {
        UpDown,
        UpRightDownLeft,
        RightLeft,
        DownRightUpLeft
    }
}
