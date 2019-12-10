using Game.Output.Primitives;

namespace Game.Output.Layout
{
    public sealed class Border : IDrawable
    {
        private readonly Label namePlate;
        private readonly Label topLeft;
        private readonly Label topRight;
        private readonly Label bottomLeft;
        private readonly Label bottomRight;
        private readonly Label leftStrokePattern;
        private readonly Label topStrokePattern;
        private readonly Label rightStrokePattern;
        private readonly Label bottomStrokePattern;

        private Label leftStroke;
        private Label topStroke;
        private Label rightStroke;
        private Label bottomStroke;

        private readonly short largestTopOffset;
        private readonly short largestLeftOffset;
        private readonly short largestBottomOffset;
        private readonly short largestRightOffset;

        private readonly Region innerRegion;

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
            FormattedString? bottomStroke = null)
        {
            this.OuterRegion = outerRegion;
            this.innerRegion = new Region(outerRegion.TopLeft, outerRegion.BottomRight);

            this.namePlate = namePlate == null ? Label.Empty : new Label(namePlate);
            this.topLeft = topLeft == null ? Label.Empty : new Label(topLeft);
            this.topRight = topRight == null ? Label.Empty : new Label(topRight);
            this.bottomLeft = bottomLeft == null ? Label.Empty : new Label(bottomLeft);
            this.bottomRight = bottomRight == null ? Label.Empty : new Label(bottomRight);
            this.leftStrokePattern = leftStroke == null ? Label.Empty : new Label(leftStroke);
            this.topStrokePattern = topStroke == null ? Label.Empty : new Label(topStroke);
            this.rightStrokePattern = rightStroke == null ? Label.Empty : new Label(rightStroke);
            this.bottomStrokePattern = bottomStroke == null ? Label.Empty : new Label(bottomStroke);

            this.largestTopOffset = Largest(
                this.topLeft.Region.Height,
                this.topRight.Region.Height,
                this.topStrokePattern.Region.Height,
                this.namePlate.Region.Height);
            this.largestLeftOffset = Largest(
                this.topLeft.Region.Width,
                this.topRight.Region.Width,
                this.leftStrokePattern.Region.Width);
            this.largestBottomOffset = Largest(
                this.bottomLeft.Region.Height,
                this.bottomRight.Region.Height,
                this.bottomStrokePattern.Region.Height);
            this.largestRightOffset = Largest(
                this.topRight.Region.Width,
                this.bottomRight.Region.Width,
                this.rightStrokePattern.Region.Width);

            this.OuterRegion.OnChanged +=
                (obj, e) =>
                {
                    this.MaybeRecalculate(e.CurrentRegion.TopLeft - e.PreviousRegion.TopLeft, e.ChangeType);
                };

            this.Recalculate();
        }

        public Region OuterRegion { get; }

        public IReadOnlyRegion InnerRegion => this.innerRegion;

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

        private void MaybeRecalculate(Coord delta, RegionChangeType changeType)
        {
            if (changeType == RegionChangeType.MoveAndResize || changeType == RegionChangeType.Resize)
            {
                this.Recalculate();
            }
            else
            {
                this.namePlate.Region.Translate(delta);
                this.topLeft.Region.Translate(delta);
                this.topRight.Region.Translate(delta);
                this.bottomLeft.Region.Translate(delta);
                this.bottomRight.Region.Translate(delta);
                this.leftStroke.Region.Translate(delta);
                this.topStroke.Region.Translate(delta);
                this.rightStroke.Region.Translate(delta);
                this.bottomStroke.Region.Translate(delta);
            }
        }

        private void Recalculate()
        {
            this.innerRegion.SetCorners(
                this.OuterRegion.TopLeft + new Coord(this.largestLeftOffset, this.largestTopOffset),
                this.OuterRegion.BottomRight - new Coord(this.largestRightOffset, this.largestBottomOffset));

            this.topLeft.Region.MoveTo(
                this.OuterRegion.TopLeft);
            this.topRight.Region.MoveTo(
                new Coord(
                    (short)(this.OuterRegion.BottomRight.X - this.topRight.Region.Width),
                    this.OuterRegion.TopLeft.Y));
            this.bottomLeft.Region.MoveTo(
                new Coord(
                    this.OuterRegion.TopLeft.X,
                    (short)(this.OuterRegion.BottomRight.Y - this.bottomLeft.Region.Height)));
            this.bottomRight.Region.MoveTo(
                new Coord(
                    (short)(this.OuterRegion.BottomRight.X - this.bottomRight.Region.Width),
                    (short)(this.OuterRegion.BottomRight.Y - this.bottomRight.Region.Height)));

            this.topStroke = this.topStrokePattern.RepeatHorizontally(
                (short)(this.OuterRegion.Width - this.topLeft.Region.Width - this.topRight.Region.Width));
            this.topStroke.Region.MoveTo(
                new Coord(
                    (short)(this.OuterRegion.TopLeft.X + this.topLeft.Region.Width),
                    this.OuterRegion.TopLeft.Y));

            this.leftStroke = this.leftStrokePattern.RepeatVertically(
                (short)(this.OuterRegion.Height - this.topLeft.Region.Height - this.bottomLeft.Region.Height));
            this.leftStroke.Region.MoveTo(
                new Coord(
                    this.OuterRegion.TopLeft.X,
                    (short)(this.OuterRegion.TopLeft.Y + this.topLeft.Region.Height)));

            this.rightStroke = this.rightStrokePattern.RepeatVertically(
                (short)(this.OuterRegion.Height - this.topRight.Region.Height - this.bottomRight.Region.Height));
            this.rightStroke.Region.MoveTo(
                new Coord(
                    (short)(this.OuterRegion.BottomRight.X - this.rightStroke.Region.Width),
                    (short)(this.OuterRegion.TopLeft.Y + this.topRight.Region.Height)));

            this.bottomStroke = this.bottomStrokePattern.RepeatHorizontally(
                (short)(this.OuterRegion.Width - this.bottomLeft.Region.Width - this.bottomRight.Region.Width));
            this.bottomStroke.Region.MoveTo(
                new Coord(
                    (short)(this.OuterRegion.TopLeft.X + this.bottomLeft.Region.Width),
                    (short)(this.OuterRegion.BottomRight.Y - bottomStroke.Region.Height)));

            this.namePlate.Region.MoveTo(
                new Coord(
                    (short)(this.OuterRegion.TopLeft.X + this.topLeft.Region.Width + 1),
                    this.OuterRegion.TopLeft.Y));
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
    }
}
