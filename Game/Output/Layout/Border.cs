using Game.Output.Primitives;

namespace Game.Output.Layout
{
    public sealed class Border : IDrawable
    {
        private readonly Region outerRegion;
        private readonly Label? namePlatePattern;
        private readonly Label? topLeftPattern;
        private readonly Label? topRightPattern;
        private readonly Label? bottomLeftPattern;
        private readonly Label? bottomRightPattern;
        private readonly Label? leftStrokePattern;
        private readonly Label? topStrokePattern;
        private readonly Label? rightStrokePattern;
        private readonly Label? bottomStrokePattern;

        private IDrawable namePlate;
        private IDrawable topLeft;
        private IDrawable topRight;
        private IDrawable bottomLeft;
        private IDrawable bottomRight;
        private IDrawable leftStroke;
        private IDrawable topStroke;
        private IDrawable rightStroke;
        private IDrawable bottomStroke;

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
            this.outerRegion = outerRegion;
            this.InnerRegion = outerRegion;

            this.namePlatePattern = new Label(namePlate);
            this.topLeftPattern = topLeft;
            this.topRightPattern = topRight;
            this.bottomLeftPattern = bottomLeft;
            this.bottomRightPattern = bottomRight;
            this.leftStrokePattern = leftStroke;
            this.topStrokePattern = topStroke;
            this.rightStrokePattern = rightStroke;
            this.bottomStrokePattern = bottomStroke;

            this.outerRegion.OnChanged += (obj, e) => this.Recalculate();

            this.Recalculate();
        }

        public IReadOnlyRegion InnerRegion { get; private set; }

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

        private void Recalculate()
        {
            short largestTopOffset = Largest(
                this.topLeftPattern?.Region.Height,
                this.topRightPattern?.Region.Height,
                this.topStrokePattern?.Region.Height,
                this.namePlatePattern?.Region.Height);
            short largestLeftOffset = Largest(
                this.topLeftPattern?.Region.Width,
                this.topRightPattern?.Region.Width,
                this.leftStrokePattern?.Region.Width);
            short largestBottomOffset = Largest(
                this.bottomLeftPattern?.Region.Height,
                this.bottomRightPattern?.Region.Height,
                this.bottomStrokePattern?.Region.Height);
            short largestRightOffset = Largest(
                this.topRightPattern?.Region.Width,
                this.bottomRightPattern?.Region.Width,
                this.rightStrokePattern?.Region.Width);

            this.InnerRegion = new Region(
                this.outerRegion.TopLeft + new Coord(largestLeftOffset, largestTopOffset),
                this.outerRegion.BottomRight - new Coord(largestRightOffset, largestBottomOffset));

            this.topLeft =
                this.topLeftPattern == null
                    ? Text.Empty
                    : this.topLeftPattern;
            this.topRight =
                this.topRightPattern == null
                    ? Text.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.BottomRight.X - this.topRightPattern.Contents.GetWidth()),
                            this.outerRegion.TopLeft.Y),
                        this.topRightPattern.Contents);
            this.bottomLeft =
                this.bottomLeftPattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            this.outerRegion.TopLeft.X,
                            (short)(this.outerRegion.BottomRight.Y - this.bottomLeftPattern.Contents.GetHeight())),
                        this.bottomLeftPattern.Contents);
            this.bottomRight =
                this.bottomRightPattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.BottomRight.X - this.bottomRightPattern.Contents.GetWidth()),
                            (short)(this.outerRegion.BottomRight.Y - this.bottomRightPattern.Contents.GetHeight())),
                        this.bottomRightPattern.Contents);

            this.topStroke =
                this.topStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.TopLeft.X + this.topLeft.Region.Width),
                            this.outerRegion.TopLeft.Y),
                        RepeatHorizontally(
                            this.topStrokePattern.Contents,
                            (short)(this.outerRegion.Width - this.topLeft.Region.Width - this.topRight.Region.Width)));

            this.leftStroke =
                this.leftStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            this.outerRegion.TopLeft.X,
                            (short)(this.outerRegion.TopLeft.Y + this.topLeft.Region.Height)),
                        RepeatVertically(
                            this.leftStrokePattern.Contents,
                            (short)(this.outerRegion.Height - this.topLeft.Region.Height - this.bottomLeft.Region.Height)));

            this.rightStroke =
                this.rightStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.BottomRight.X - this.rightStrokePattern.Contents.GetWidth()),
                            (short)(this.outerRegion.TopLeft.Y + this.topRight.Region.Height)),
                        RepeatVertically(
                            this.rightStrokePattern.Contents,
                            (short)(this.outerRegion.Height - this.topRight.Region.Height - this.bottomRight.Region.Height)));

            this.bottomStroke =
                this.bottomStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.TopLeft.X + this.bottomLeft.Region.Width),
                            (short)(this.outerRegion.BottomRight.Y - bottomStrokePattern.Contents.GetHeight())),
                        RepeatHorizontally(
                            this.bottomStrokePattern.Contents,
                            (short)(this.outerRegion.Width - this.bottomLeft.Region.Width - this.bottomRight.Region.Width)));

            this.namePlate =
                this.namePlatePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.TopLeft.X + this.topLeft.Region.Width + 1),
                            this.outerRegion.TopLeft.Y),
                        this.namePlatePattern.Contents);
        }

        private static short Largest(params short?[] values)
        {
            short largest = 0;
            foreach (short? value in values)
            {
                if (value.HasValue && value > largest)
                {
                    largest = value.Value;
                }
            }

            return largest;
        }

        private static CharInfo[,] RepeatHorizontally(CharInfo[,] pattern, short width)
        {
            short originalWidth = pattern.GetWidth();

            short height = pattern.GetHeight();
            CharInfo[,] result = new CharInfo[height, width];
            for (short y = 0; y < height; y++)
            {
                for (short x = 0; x < width; x++)
                {
                    result[y, x] = pattern[y, x % originalWidth];
                }
            }

            return result;
        }

        private static CharInfo[,] RepeatVertically(CharInfo[,] pattern, short height)
        {
            short originalHeight = pattern.GetHeight();

            short width = pattern.GetWidth();
            CharInfo[,] result = new CharInfo[height, width];
            for (short y = 0; y < height; y++)
            {
                for (short x = 0; x < width; x++)
                {
                    result[y, x] = pattern[y % originalHeight, x];
                }
            }

            return result;
        }
    }
}
