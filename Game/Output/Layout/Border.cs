using Game.Output.Primitives;

namespace Game.Output.Layout
{
    public sealed class Border : IDrawable
    {
        private readonly Region outerRegion;
        private readonly CharInfo[,]? namePlatePattern;
        private readonly CharInfo[,]? topLeftPattern;
        private readonly CharInfo[,]? topRightPattern;
        private readonly CharInfo[,]? bottomLeftPattern;
        private readonly CharInfo[,]? bottomRightPattern;
        private readonly CharInfo[,]? leftStrokePattern;
        private readonly CharInfo[,]? topStrokePattern;
        private readonly CharInfo[,]? rightStrokePattern;
        private readonly CharInfo[,]? bottomStrokePattern;

        private Text namePlate;
        private Rectangle topLeft;
        private Rectangle topRight;
        private Rectangle bottomLeft;
        private Rectangle bottomRight;
        private Rectangle leftStroke;
        private Rectangle topStroke;
        private Rectangle rightStroke;
        private Rectangle bottomStroke;

        internal Border(
            Region outerRegion,
            CharInfo[,]? namePlate = null,
            CharInfo[,]? topLeft = null,
            CharInfo[,]? topRight = null,
            CharInfo[,]? bottomLeft = null,
            CharInfo[,]? bottomRight = null,
            CharInfo[,]? leftStroke = null,
            CharInfo[,]? topStroke = null,
            CharInfo[,]? rightStroke = null,
            CharInfo[,]? bottomStroke = null)
        {
            this.outerRegion = outerRegion;
            this.InnerRegion = outerRegion;

            this.namePlatePattern = namePlate;
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

        public Region InnerRegion { get; private set; }

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
                this.topLeftPattern?.GetHeight(),
                this.topRightPattern?.GetHeight(),
                this.topStrokePattern?.GetHeight(),
                this.namePlatePattern?.GetHeight());
            short largestLeftOffset = Largest(
                this.topLeftPattern?.GetWidth(),
                this.topRightPattern?.GetWidth(),
                this.leftStrokePattern?.GetWidth());
            short largestBottomOffset = Largest(
                this.bottomLeftPattern?.GetHeight(),
                this.bottomRightPattern?.GetHeight(),
                this.bottomStrokePattern?.GetHeight());
            short largestRightOffset = Largest(
                this.topRightPattern?.GetWidth(),
                this.bottomRightPattern?.GetWidth(),
                this.rightStrokePattern?.GetWidth());

            this.InnerRegion = new Region(
                this.outerRegion.TopLeft + new Coord(largestLeftOffset, largestTopOffset),
                this.outerRegion.BottomRight - new Coord(largestRightOffset, largestBottomOffset));

            this.topLeft =
                this.topLeftPattern == null
                    ? Rectangle.Empty
                    : new Rectangle(this.outerRegion.TopLeft, this.topLeftPattern);
            this.topRight =
                this.topRightPattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.BottomRight.X - this.topRightPattern.GetWidth()),
                            this.outerRegion.TopLeft.Y),
                        this.topRightPattern);
            this.bottomLeft =
                this.bottomLeftPattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            this.outerRegion.TopLeft.X,
                            (short)(this.outerRegion.BottomRight.Y - this.bottomLeftPattern.GetHeight())),
                        this.bottomLeftPattern);
            this.bottomRight =
                this.bottomRightPattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.BottomRight.X - this.bottomRightPattern.GetWidth()),
                            (short)(this.outerRegion.BottomRight.Y - this.bottomRightPattern.GetHeight())),
                        this.bottomRightPattern);

            this.topStroke =
                this.topStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.TopLeft.X + this.topLeft.Region.Width),
                            this.outerRegion.TopLeft.Y),
                        RepeatHorizontally(
                            this.topStrokePattern,
                            (short)(this.outerRegion.Width - this.topLeft.Region.Width - this.topRight.Region.Width)));

            this.leftStroke =
                this.leftStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            this.outerRegion.TopLeft.X,
                            (short)(this.outerRegion.TopLeft.Y + this.topLeft.Region.Height)),
                        RepeatVertically(
                            this.leftStrokePattern,
                            (short)(this.outerRegion.Height - this.topLeft.Region.Height - this.bottomLeft.Region.Height)));

            this.rightStroke =
                this.rightStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.BottomRight.X - this.rightStrokePattern.GetWidth()),
                            (short)(this.outerRegion.TopLeft.Y + this.topRight.Region.Height)),
                        RepeatVertically(
                            this.rightStrokePattern,
                            (short)(this.outerRegion.Height - this.topRight.Region.Height - this.bottomRight.Region.Height)));

            this.bottomStroke =
                this.bottomStrokePattern == null
                    ? Rectangle.Empty
                    : new Rectangle(
                        new Coord(
                            (short)(this.outerRegion.TopLeft.X + this.bottomLeft.Region.Width),
                            (short)(this.outerRegion.BottomRight.Y - bottomStrokePattern.GetHeight())),
                        RepeatHorizontally(
                            this.bottomStrokePattern,
                            (short)(this.outerRegion.Width - this.bottomLeft.Region.Width - this.bottomRight.Region.Width)));

            this.namePlate =
                this.namePlatePattern == null
                    ? Text.Empty
                    : new Text(
                        new Coord(
                            (short)(this.outerRegion.TopLeft.X + this.topLeft.Region.Width + 1),
                            this.outerRegion.TopLeft.Y),
                        this.namePlatePattern);
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
