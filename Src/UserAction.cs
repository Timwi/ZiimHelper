using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using RT.Util;

namespace ZiimHelper
{
    abstract class UserAction
    {
        public abstract void Do();
        public abstract void Undo();
        public abstract IEnumerable<Item> Selection { get; }
    }

    sealed class MultiAction : UserAction
    {
        public UserAction[] Actions { get; private set; }
        public MultiAction(IEnumerable<UserAction> actions) { Actions = actions.ToArray(); }

        public override void Do()
        {
            foreach (var action in Actions)
                action.Do();
        }

        public override void Undo()
        {
            foreach (var action in Actions)
                action.Undo();
        }

        public override IEnumerable<Item> Selection { get { return Actions.SelectMany(a => a.Selection); } }
    }

    sealed class MoveArrow : UserAction
    {
        public ArrowInfo Arrow { get; private set; }
        public int OldX { get; private set; }
        public int OldY { get; private set; }
        public int NewX { get; private set; }
        public int NewY { get; private set; }
        public MoveArrow(ArrowInfo arrow, int oldX, int oldY, int newX, int newY)
        {
            Arrow = arrow;
            OldX = oldX;
            OldY = oldY;
            NewX = newX;
            NewY = newY;
        }

        public override void Do()
        {
            Arrow.X = NewX;
            Arrow.Y = NewY;
        }

        public override void Undo()
        {
            Arrow.X = OldX;
            Arrow.Y = OldY;
        }

        public override IEnumerable<Item> Selection { get { yield return Arrow; } }
    }

    sealed class ConvertArrow : UserAction
    {
        public ArrowInfo OldArrow { get; private set; }
        public ArrowInfo NewArrow { get; private set; }
        public Cloud ParentCloud { get; private set; }

        public ConvertArrow(ArrowInfo arrow, Cloud parentCloud)
        {
            OldArrow = arrow;
            NewArrow = arrow.IfType(
                (SingleArrowInfo sai) => (ArrowInfo) new DoubleArrowInfo { X = sai.X, Y = sai.Y, Annotation = sai.Annotation, Direction = (DoubleDirection) ((int) sai.Direction % 4), Marked = sai.Marked },
                (DoubleArrowInfo dai) => (ArrowInfo) new SingleArrowInfo { X = dai.X, Y = dai.Y, Annotation = dai.Annotation, Direction = (Direction) (int) dai.Direction, Marked = dai.Marked });
            ParentCloud = parentCloud;
        }

        public override void Do()
        {
            ParentCloud.Items.Remove(OldArrow);
            ParentCloud.Items.Add(NewArrow);
        }

        public override void Undo()
        {
            ParentCloud.Items.Remove(NewArrow);
            ParentCloud.Items.Add(OldArrow);
        }

        public override IEnumerable<Item> Selection { get { yield return OldArrow; yield return NewArrow; } }
    }

    sealed class MoveLabel : UserAction
    {
        public Cloud Cloud { get; private set; }
        public bool IsFrom { get; private set; }
        public int OldX { get; private set; }
        public int OldY { get; private set; }
        public int NewX { get; private set; }
        public int NewY { get; private set; }
        public MoveLabel(Cloud cloud, bool isFrom, int oldX, int oldY, int newX, int newY)
        {
            Cloud = cloud;
            IsFrom = isFrom;
            OldX = oldX;
            OldY = oldY;
            NewX = newX;
            NewY = newY;
        }

        public override void Do()
        {
            if (IsFrom)
            {
                Cloud.LabelFromX = NewX;
                Cloud.LabelFromY = NewY;
            }
            else
            {
                Cloud.LabelToX = NewX;
                Cloud.LabelToY = NewY;
            }
        }

        public override void Undo()
        {
            if (IsFrom)
            {
                Cloud.LabelFromX = OldX;
                Cloud.LabelFromY = OldY;
            }
            else
            {
                Cloud.LabelToX = OldX;
                Cloud.LabelToY = OldY;
            }
        }

        public override IEnumerable<Item> Selection { get { yield return Cloud; } }
    }

    enum ActionType
    {
        Add,
        Remove
    }

    sealed class AddOrRemoveItems : UserAction
    {
        public ActionType ActionType { get; private set; }
        public Item[] Items { get; private set; }
        public Cloud ParentCloud { get; private set; }

        public AddOrRemoveItems(ActionType actionType, Item item, Cloud parentCloud)
            : this(actionType, new[] { item }, parentCloud)
        {
        }

        public AddOrRemoveItems(ActionType actionType, IEnumerable<Item> items, Cloud parentCloud)
        {
            ActionType = actionType;
            Items = items.ToArray();
            ParentCloud = parentCloud;
        }

        public override void Do()
        {
            if (ActionType == ActionType.Remove)
                ParentCloud.Items.RemoveRange(Items);
            else
                ParentCloud.Items.AddRange(Items);
        }

        public override void Undo()
        {
            if (ActionType == ActionType.Remove)
                ParentCloud.Items.AddRange(Items);
            else
                ParentCloud.Items.RemoveRange(Items);
        }

        public override IEnumerable<Item> Selection { get { return Items; } }
    }

    sealed class CloudColor : UserAction
    {
        public Cloud Cloud { get; private set; }
        public Color OldColor { get; private set; }
        public Color NewColor { get; private set; }
        public CloudColor(Cloud cloud, Color oldColor, Color newColor)
        {
            Cloud = cloud;
            OldColor = oldColor;
            NewColor = newColor;
        }

        public override void Do() { Cloud.Color = NewColor; }
        public override void Undo() { Cloud.Color = OldColor; }
        public override IEnumerable<Item> Selection { get { yield return Cloud; } }
    }

    sealed class CloudLabel : UserAction
    {
        public Cloud Cloud { get; private set; }
        public string OldLabel { get; private set; }
        public string NewLabel { get; private set; }
        public CloudLabel(Cloud cloud, string oldLabel, string newLabel)
        {
            Cloud = cloud;
            OldLabel = oldLabel;
            NewLabel = newLabel;
        }

        public override void Do() { Cloud.Label = NewLabel; }
        public override void Undo() { Cloud.Label = OldLabel; }
        public override IEnumerable<Item> Selection { get { yield return Cloud; } }
    }

    sealed class ArrowAnnotation : UserAction
    {
        public ArrowInfo Arrow { get; private set; }
        public string OldAnnotation { get; private set; }
        public string NewAnnotation { get; private set; }
        public ArrowAnnotation(ArrowInfo arrow, string oldAnnotation, string newAnnotation)
        {
            Arrow = arrow;
            OldAnnotation = oldAnnotation;
            NewAnnotation = newAnnotation;
        }

        public override void Do() { Arrow.Annotation = NewAnnotation; }
        public override void Undo() { Arrow.Annotation = OldAnnotation; }
        public override IEnumerable<Item> Selection { get { yield return Arrow; } }
    }

    sealed class ToggleMark : UserAction
    {
        public ArrowInfo Arrow { get; private set; }
        public ToggleMark(ArrowInfo arrow) { Arrow = arrow; }
        public override void Do() { Arrow.Marked = !Arrow.Marked; }
        public override void Undo() { Arrow.Marked = !Arrow.Marked; }
        public override IEnumerable<Item> Selection { get { yield return Arrow; } }
    }

    sealed class ToggleTerminal : UserAction
    {
        public SingleArrowInfo Arrow { get; private set; }
        public ToggleTerminal(SingleArrowInfo arrow) { Arrow = arrow; }
        public override void Do() { Arrow.IsTerminalArrow = !Arrow.IsTerminalArrow; }
        public override void Undo() { Arrow.IsTerminalArrow = !Arrow.IsTerminalArrow; }
        public override IEnumerable<Item> Selection { get { yield return Arrow; } }
    }

    sealed class RotateArrow : UserAction
    {
        public ArrowInfo Arrow { get; private set; }
        public bool Clockwise { get; private set; }
        public RotateArrow(ArrowInfo arrow, bool clockwise)
        {
            Arrow = arrow;
            Clockwise = clockwise;
        }
        public override void Do() { Arrow.Rotate(Clockwise); }
        public override void Undo() { Arrow.Rotate(!Clockwise); }
        public override IEnumerable<Item> Selection { get { yield return Arrow; } }
    }

    abstract class ReorientArrow<TArrow, TDirection> : UserAction where TArrow : ArrowInfo
    {
        public TArrow Arrow { get; private set; }
        public TDirection OldDirection { get; private set; }
        public TDirection NewDirection { get; private set; }
        public ReorientArrow(TArrow arrow, TDirection oldDirection, TDirection newDirection)
        {
            Arrow = arrow;
            OldDirection = oldDirection;
            NewDirection = newDirection;
        }
        protected abstract void setDirection(TDirection direction);
        public override void Do() { setDirection(NewDirection); }
        public override void Undo() { setDirection(OldDirection); }
        public override IEnumerable<Item> Selection { get { yield return Arrow; } }
    }

    sealed class ReorientSingleArrow : ReorientArrow<SingleArrowInfo, Direction>
    {
        public ReorientSingleArrow(SingleArrowInfo arrow, Direction oldDirection, Direction newDirection) : base(arrow, oldDirection, newDirection) { }
        protected override void setDirection(Direction direction) { Arrow.Direction = direction; }
    }
    sealed class ReorientDoubleArrow : ReorientArrow<DoubleArrowInfo, DoubleDirection>
    {
        public ReorientDoubleArrow(DoubleArrowInfo arrow, DoubleDirection oldDirection, DoubleDirection newDirection) : base(arrow, oldDirection, newDirection) { }
        protected override void setDirection(DoubleDirection direction) { Arrow.Direction = direction; }
    }

    sealed class RotateItems : UserAction
    {
        public Item[] Items { get; private set; }
        public bool Clockwise { get; private set; }
        public RotateItems(IEnumerable<Item> items, bool clockwise)
        {
            Items = items.ToArray();
            Clockwise = clockwise;
        }

        private void rotate(bool clockwise)
        {
            var arrows = Items.OfType<ArrowInfo>();
            var minX = arrows.Min(a => a.X);
            var minY = arrows.Min(a => a.Y);
            var maxX = arrows.Max(a => a.X);
            var maxY = arrows.Max(a => a.Y);
            var fix = Ut.Lambda((int x, int y, Action<int, int> setXY) =>
            {
                setXY(
                    clockwise ? minX + (maxY - minY) - y + minY : minX + y - minY,
                    clockwise ? minY + x - minX : minY + (maxX - minX) - x + minX);
            });

            foreach (var item in Items)
            {
                Ut.IfType(item,
                    (ArrowInfo arrow) =>
                    {
                        fix(arrow.X, arrow.Y, (x, y) => { arrow.X = x; arrow.Y = y; });
                        arrow.Rotate(clockwise);
                        arrow.Rotate(clockwise);
                    },
                    (Cloud cloud) =>
                    {
                        fix(cloud.LabelFromX, cloud.LabelFromY, (x, y) => { cloud.LabelFromX = x; cloud.LabelFromY = y; });
                        fix(cloud.LabelToX, cloud.LabelToY, (x, y) => { cloud.LabelToX = x; cloud.LabelToY = y; });
                    },
                    wrongType => { throw new InvalidOperationException("Unexpected type of item."); });
            }
        }

        public override void Do() { rotate(Clockwise); }
        public override void Undo() { rotate(!Clockwise); }
        public override IEnumerable<Item> Selection { get { return Items; } }
    }
}
