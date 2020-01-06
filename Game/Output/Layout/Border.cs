using System;
using Game.Output.Primitives;

namespace Game.Output.Layout
{
    public sealed class Border : IReadOnlyBorder
    {
        private readonly Fill namePlate;
        private readonly Fill topLeft;
        private readonly Fill topRight;
        private readonly Fill bottomLeft;
        private readonly Fill bottomRight;
        private readonly Fill leftStroke;
        private readonly Fill topStroke;
        private readonly Fill rightStroke;
        private readonly Fill bottomStroke;

        private readonly short largestTopOffset;
        private readonly short largestLeftOffset;
        private readonly short largestBottomOffset;
        private readonly short largestRightOffset;

        private readonly Region innerRegion;
        private IReadOnlyRegion[,] components;

        internal Border(
            Region outerRegion,
            FormattedString? namePlate = null,
            FormattedString? topLeft = null,
            FormattedString? topRight = null,
            FormattedString? bottomLeft = null,
            FormattedString? bottomRight = null,
            FormattedString? leftStroke = null,
            FormattedString? topStroke = null,
            FormattedString? rightStroke = null,
            FormattedString? bottomStroke = null,
            CharColors? borderFill = null)
        {
            this.OuterRegion = outerRegion;
            this.innerRegion = new Region(outerRegion.TopLeft, outerRegion.BottomRight);

            this.namePlate = namePlate == null ? Fill.Empty : new Fill(namePlate, borderFill);
            this.topLeft = topLeft == null ? Fill.Empty : new Fill(topLeft, borderFill);
            this.topRight = topRight == null ? Fill.Empty : new Fill(topRight, borderFill);
            this.bottomLeft = bottomLeft == null ? Fill.Empty : new Fill(bottomLeft, borderFill);
            this.bottomRight = bottomRight == null ? Fill.Empty : new Fill(bottomRight, borderFill);
            this.leftStroke = leftStroke == null ? Fill.Empty : new Fill(leftStroke, borderFill);
            this.topStroke = topStroke == null ? Fill.Empty : new Fill(topStroke, borderFill);
            this.rightStroke = rightStroke == null ? Fill.Empty : new Fill(rightStroke, borderFill);
            this.bottomStroke = bottomStroke == null ? Fill.Empty : new Fill(bottomStroke, borderFill);

            this.components =
                new IReadOnlyRegion[,]
                {
                    { this.topLeft.Region, this.topStroke.Region, this.topRight.Region },
                    { this.leftStroke.Region, this.innerRegion, this.rightStroke.Region },
                    { this.bottomLeft.Region, this.bottomStroke.Region, this.bottomRight.Region }
                };

            this.largestTopOffset = Largest(
                this.topLeft.Region.Height,
                this.topRight.Region.Height,
                this.topStroke.Region.Height,
                this.namePlate.Region.Height);
            this.largestLeftOffset = Largest(
                this.topLeft.Region.Width,
                this.topRight.Region.Width,
                this.leftStroke.Region.Width);
            this.largestBottomOffset = Largest(
                this.bottomLeft.Region.Height,
                this.bottomRight.Region.Height,
                this.bottomStroke.Region.Height);
            this.largestRightOffset = Largest(
                this.topRight.Region.Width,
                this.bottomRight.Region.Width,
                this.rightStroke.Region.Width);

            this.OuterRegion.OnChangeRequested +=
                (obj, e) =>
                {
                    // Only let the outer region change if the inner region allows it
                    e.Cancel(this.innerRegion.SimulateRequestChange(
                        e.AfterChange.TopLeft + new Coord(this.largestLeftOffset, this.largestTopOffset),
                        e.AfterChange.BottomRight - new Coord(this.largestRightOffset, this.largestBottomOffset)));
                };
            this.OuterRegion.OnChanged +=
                (obj, e) =>
                {
                    this.MaybeRecalculate(e.AfterChange.TopLeft - e.BeforeChange.TopLeft, e.ChangeTypes);
                };

            this.AdjustInnerRegion();
            this.Recalculate();
        }

        public Region OuterRegion { get; }

        public IReadOnlyRegion InnerRegion => this.innerRegion;

        IReadOnlyRegion IReadOnlyBorder.OuterRegion => this.OuterRegion;

        public void Draw(ISink sink)
        {
            this.topLeft.Draw(sink);
            this.topStroke.Draw(sink);

            this.topRight.Draw(sink);
            this.leftStroke.Draw(sink);

            this.bottomLeft.Draw(sink);
            this.rightStroke.Draw(sink);

            this.bottomRight.Draw(sink);
            this.bottomStroke.Draw(sink);

            this.namePlate.Draw(sink);
        }

        public void Draw(ISink sink, Rectangle window)
        {
            ////throw new NotImplementedException("TODO complicated");

            this.topLeft.Draw(sink);
            this.topStroke.Draw(sink);

            this.topRight.Draw(sink);
            this.leftStroke.Draw(sink);

            this.bottomLeft.Draw(sink);
            this.rightStroke.Draw(sink);

            this.bottomRight.Draw(sink);
            this.bottomStroke.Draw(sink);

            this.namePlate.Draw(sink);
        }

        public void InvertColor()
        {
            this.topLeft.InvertColor();
            this.topStroke.InvertColor();
            this.topRight.InvertColor();
            this.leftStroke.InvertColor();
            this.bottomLeft.InvertColor();
            this.rightStroke.InvertColor();
            this.bottomRight.InvertColor();
            this.bottomStroke.InvertColor();
            this.namePlate.InvertColor();
        }

        private void MaybeRecalculate(Coord delta, RegionChangeTypes changeType)
        {
            this.AdjustInnerRegion();
            if (changeType.HasFlag(RegionChangeTypes.Resize))
            {
                this.Recalculate();
            }
            else
            {
                this.namePlate.Region.TryTranslate(delta, out _);
                this.topLeft.Region.TryTranslate(delta, out _);
                this.topRight.Region.TryTranslate(delta, out _);
                this.bottomLeft.Region.TryTranslate(delta, out _);
                this.bottomRight.Region.TryTranslate(delta, out _);
                this.leftStroke.Region.TryTranslate(delta, out _);
                this.topStroke.Region.TryTranslate(delta, out _);
                this.rightStroke.Region.TryTranslate(delta, out _);
                this.bottomStroke.Region.TryTranslate(delta, out _);
            }
        }

        private void AdjustInnerRegion()
        {
            this.innerRegion.TrySetCorners(
               this.OuterRegion.TopLeft + new Coord(this.largestLeftOffset, this.largestTopOffset),
               this.OuterRegion.BottomRight - new Coord(this.largestRightOffset, this.largestBottomOffset),
               false, // This method is called when a resize was allowed, so any subsequent cancel is a race condition
               out _);
        }

        private void Recalculate()
        {
            this.topLeft.Region.TryMoveTo(
                this.OuterRegion.TopLeft,
                out _);
            this.topRight.Region.TryMoveTo(
                new Coord(
                    (short)(this.OuterRegion.BottomRight.X - this.topRight.Region.Width),
                    this.OuterRegion.TopLeft.Y),
                out _);
            this.bottomLeft.Region.TryMoveTo(
                new Coord(
                    this.OuterRegion.TopLeft.X,
                    (short)(this.OuterRegion.BottomRight.Y - this.bottomLeft.Region.Height)),
                out _);
            this.bottomRight.Region.TryMoveTo(
                new Coord(
                    (short)(this.OuterRegion.BottomRight.X - this.bottomRight.Region.Width),
                    (short)(this.OuterRegion.BottomRight.Y - this.bottomRight.Region.Height)),
                out _);

            this.topStroke.Region.Width =
                (short)(this.OuterRegion.Width - this.topLeft.Region.Width - this.topRight.Region.Width);
            this.topStroke.Region.TryMoveTo(
                new Coord(
                    (short)(this.OuterRegion.TopLeft.X + this.topLeft.Region.Width),
                    this.OuterRegion.TopLeft.Y),
                out _);

            this.leftStroke.Region.Height =
                (short)(this.OuterRegion.Height - this.topLeft.Region.Height - this.bottomLeft.Region.Height);
            this.leftStroke.Region.TryMoveTo(
                new Coord(
                    this.OuterRegion.TopLeft.X,
                    (short)(this.OuterRegion.TopLeft.Y + this.topLeft.Region.Height)),
                out _);

            this.rightStroke.Region.Height =
                (short)(this.OuterRegion.Height - this.topRight.Region.Height - this.bottomRight.Region.Height);
            this.rightStroke.Region.TryMoveTo(
                new Coord(
                    (short)(this.OuterRegion.BottomRight.X - this.rightStroke.Region.Width),
                    (short)(this.OuterRegion.TopLeft.Y + this.topRight.Region.Height)),
                out _);

            this.bottomStroke.Region.Width =
                (short)(this.OuterRegion.Width - this.bottomLeft.Region.Width - this.bottomRight.Region.Width);
            this.bottomStroke.Region.TryMoveTo(
                new Coord(
                    (short)(this.OuterRegion.TopLeft.X + this.bottomLeft.Region.Width),
                    (short)(this.OuterRegion.BottomRight.Y - bottomStroke.Region.Height)),
                out _);

            this.namePlate.Region.TryMoveTo(
                new Coord(
                    (short)(this.OuterRegion.TopLeft.X + this.topLeft.Region.Width + 1),
                    this.OuterRegion.TopLeft.Y),
                out _);
        }

        private static short Largest(params short[] values)
        {
            short largest = 0;
            foreach (short value in values)
            {
                if (value > largest)
                {
                    largest = value;
                }
            }

            return largest;
        }

        public IReadOnlyRegion GetComponent(BorderComponentType component) =>
            component switch
            {
                BorderComponentType.TopLeft => this.components[0, 0],
                BorderComponentType.Top => this.components[0, 1],
                BorderComponentType.TopRight => this.components[0, 2],
                BorderComponentType.Left => this.components[1, 0],
                BorderComponentType.Center => this.components[1, 1],
                BorderComponentType.Right => this.components[1, 2],
                BorderComponentType.BottomLeft => this.components[2, 0],
                BorderComponentType.Bottom => this.components[2, 1],
                BorderComponentType.BottomRight => this.components[2, 2],
                _ => throw new ArgumentException("Unrecognized border component type.", nameof(component))
            };
    }
}
