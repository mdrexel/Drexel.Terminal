using System;
using System.Linq;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public class Label : IDrawable
    {
        private static readonly string[] NewLines = new string[] { Environment.NewLine };

        private bool hasDelayedContent;
        private readonly CharDelay[,]? delayedContent;
        private readonly CharInfo[,]? undelayedContent;

        public Label(FormattedString content)
            : this(new Coord(0, 0), content)
        {
        }

        public Label(Coord topLeft, FormattedString content)
        {
            string[] lines = content.Value.Split(NewLines, StringSplitOptions.None);
            short height = (short)lines.Length;
            short width = (short)lines.Max(x => x.Length);

            this.hasDelayedContent = content.ContainsDelays;
            if (this.hasDelayedContent)
            {
                this.delayedContent = new CharDelay[height, width];
                int rangeIndex = 0;
                Range range = content.Ranges[rangeIndex++];
                for (int y = 0, index = 0; y < lines.Length; y++, index += Environment.NewLine.Length)
                {
                    string line = lines[y];
                    for (int x = 0; x < line.Length; x++, index++)
                    {
                        if (range.EndIndexExclusive == index)
                        {
                            range = content.Ranges[rangeIndex++];
                        }

                        this.delayedContent[y, x] = new CharDelay(
                            new CharInfo(line[x], range.Attributes),
                            range.Delay);
                    }
                }

                this.Region = new Region(topLeft, this.delayedContent.ToCoord() + topLeft);
            }
            else
            {
                this.undelayedContent = new CharInfo[height, width];
                int rangeIndex = 0;
                Range range = content.Ranges[rangeIndex++];
                for (int y = 0, index = 0; y < lines.Length; y++, index += Environment.NewLine.Length)
                {
                    string line = lines[y];
                    for (int x = 0; x < line.Length; x++, index++)
                    {
                        if (range.EndIndexExclusive == index)
                        {
                            range = content.Ranges[rangeIndex++];
                        }

                        this.undelayedContent[y, x] = new CharInfo(line[x], range.Attributes);
                    }
                }

                this.Region = new Region(topLeft, this.undelayedContent.ToCoord() + topLeft);
            }
        }

        internal Label(Coord topLeft, CharInfo[,] content)
        {
            this.undelayedContent = content;
            this.hasDelayedContent = false;
            this.Region = new Region(topLeft, content.ToCoord() + topLeft);
        }

        internal Label(Coord topLeft, CharDelay[,] content)
        {
            this.delayedContent = content;
            this.hasDelayedContent = true;
            this.Region = new Region(topLeft, content.ToCoord() + topLeft);
        }

        public static Label Empty { get; } = new Label(FormattedString.Empty);

        public IMoveOnlyRegion Region { get; private set; }

        public void Draw(ISink sink)
        {
            if (this.hasDelayedContent)
            {
                sink.WriteRegion(this.delayedContent, this.Region.TopLeft);
            }
            else
            {
                sink.WriteRegion(this.undelayedContent, this.Region.TopLeft);
            }
        }

        internal Label RepeatHorizontally(short width)
        {
            if (this.hasDelayedContent)
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatHorizontallyInternal(this.delayedContent, width));
            }
            else
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatHorizontallyInternal(this.undelayedContent, width));
            }
        }

        internal Label RepeatVertically(short height)
        {
            if (this.hasDelayedContent)
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatVerticallyInternal(this.delayedContent, height));
            }
            else
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatVerticallyInternal(this.undelayedContent, height));
            }
        }

        private static T[,] RepeatHorizontallyInternal<T>(T[,] pattern, short width)
        {
            short originalWidth = pattern.GetWidth();

            short height = pattern.GetHeight();
            T[,] result = new T[height, width];
            for (short y = 0; y < height; y++)
            {
                for (short x = 0; x < width; x++)
                {
                    result[y, x] = pattern[y, x % originalWidth];
                }
            }

            return result;
        }

        private static T[,] RepeatVerticallyInternal<T>(T[,] pattern, short height)
        {
            short originalHeight = pattern.GetHeight();

            short width = pattern.GetWidth();
            T[,] result = new T[height, width];
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
