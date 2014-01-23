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
        public abstract void Do(HashSet<Item> selected);
        public abstract void Undo(HashSet<Item> selected);
    }

    sealed class MultiAction : UserAction
    {
        public UserAction[] Actions { get; private set; }
        public MultiAction(IEnumerable<UserAction> actions) { Actions = actions.ToArray(); }

        private void process(Action<UserAction> processor, HashSet<Item> selected)
        {
            var newSelection = new HashSet<Item>();
            foreach (var action in Actions)
            {
                processor(action);
                newSelection.AddRange(selected);
            }
            selected.AddRange(newSelection);
        }

        public override void Do(HashSet<Item> selected)
        {
            process(action => action.Do(selected), selected);
        }

        public override void Undo(HashSet<Item> selected)
        {
            process(action => action.Undo(selected), selected);
        }
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

        public override void Do(HashSet<Item> selected)
        {
            selected.Clear();
            Arrow.X = NewX;
            Arrow.Y = NewY;
            selected.Add(Arrow);
        }

        public override void Undo(HashSet<Item> selected)
        {
            selected.Clear();
            Arrow.X = OldX;
            Arrow.Y = OldY;
            selected.Add(Arrow);
        }
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

        public override void Do(HashSet<Item> selected)
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

        public override void Undo(HashSet<Item> selected)
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

        private void add(HashSet<Item> selected)
        {
            selected.Clear();
            ParentCloud.Items.AddRange(Items);
            selected.AddRange(Items);
        }

        private void remove(HashSet<Item> selected)
        {
            selected.Clear();
            ParentCloud.Items.RemoveRange(Items);
        }

        public override void Do(HashSet<Item> selected)
        {
            if (ActionType == ActionType.Remove)
                remove(selected);
            else
                add(selected);
        }

        public override void Undo(HashSet<Item> selected)
        {
            if (ActionType == ActionType.Remove)
                add(selected);
            else
                remove(selected);
        }
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

        public override void Do(HashSet<Item> selected) { Cloud.Color = NewColor; }
        public override void Undo(HashSet<Item> selected) { Cloud.Color = OldColor; }
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

        public override void Do(HashSet<Item> selected) { Cloud.Label = NewLabel; }
        public override void Undo(HashSet<Item> selected) { Cloud.Label = OldLabel; }
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

        public override void Do(HashSet<Item> selected) { Arrow.Annotation = NewAnnotation; }
        public override void Undo(HashSet<Item> selected) { Arrow.Annotation = OldAnnotation; }
    }

    sealed class ToggleMark : UserAction
    {
        public ArrowInfo Arrow { get; private set; }
        public ToggleMark(ArrowInfo arrow) { Arrow = arrow; }
        public override void Do(HashSet<Item> selected)
        {
            selected.Clear();
            Arrow.Marked = !Arrow.Marked;
            selected.Add(Arrow);
        }
        public override void Undo(HashSet<Item> selected) { Do(selected); }
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

        private void rotate(HashSet<Item> selected, bool clockwise)
        {
            selected.Clear();
            selected.Add(Arrow);
            Arrow.Rotate(clockwise);
        }

        public override void Do(HashSet<Item> selected) { rotate(selected, Clockwise); }
        public override void Undo(HashSet<Item> selected) { rotate(selected, !Clockwise); }
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
        private void setDirection(TDirection direction, HashSet<Item> selected)
        {
            selected.Clear();
            selected.Add(Arrow);
            setDirectionImpl(direction);
        }
        protected abstract void setDirectionImpl(TDirection direction);
        public override void Do(HashSet<Item> selected) { setDirection(NewDirection, selected); }
        public override void Undo(HashSet<Item> selected) { setDirection(OldDirection, selected); }
    }

    sealed class ReorientSingleArrow : ReorientArrow<SingleArrowInfo, Direction>
    {
        protected override void setDirectionImpl(Direction direction) { Arrow.Direction = direction; }
        public ReorientSingleArrow(SingleArrowInfo arrow, Direction oldDirection, Direction newDirection) : base(arrow, oldDirection, newDirection) { }
    }
    sealed class ReorientDoubleArrow : ReorientArrow<DoubleArrowInfo, DoubleDirection>
    {
        protected override void setDirectionImpl(DoubleDirection direction) { Arrow.Direction = direction; }
        public ReorientDoubleArrow(DoubleArrowInfo arrow, DoubleDirection oldDirection, DoubleDirection newDirection) : base(arrow, oldDirection, newDirection) { }
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

        private void rotate(HashSet<Item> selected, bool clockwise)
        {
            selected.Clear();
            selected.AddRange(Items);

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

        public override void Do(HashSet<Item> selected) { rotate(selected, Clockwise); }
        public override void Undo(HashSet<Item> selected) { rotate(selected, !Clockwise); }
    }
}
