using System;
using System.Collections.Generic;
using System.Linq;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public class Label : IDrawable
    {
        private static readonly string[] NewLines = new string[] { Environment.NewLine };

        private readonly bool hasDelayedContent;
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
                Label.Process(
                    in this.delayedContent,
                    lines,
                    content.Ranges,
                    (x, y) => new CharDelay(
                        new CharInfo(x, y.Attributes),
                        y.Delay));
            }
            else
            {
                this.undelayedContent = new CharInfo[height, width];
                Label.Process(
                    in this.undelayedContent,
                    lines,
                    content.Ranges,
                    (x, y) => new CharInfo(x, y.Attributes));
            }

            this.Region = new Region(topLeft, new Coord(width, height) + topLeft);
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

        private static void Process<T>(
            in T[,] output,
            string[] lines,
            IReadOnlyList<Range> ranges,
            Func<char, Range, T> factory)
        {
            int rangeIndex = 0;
            Range range = ranges[rangeIndex++];
            for (int y = 0, index = 0; y < lines.Length; y++, index += Environment.NewLine.Length)
            {
                string line = lines[y];
                for (int x = 0; x < line.Length; x++, index++)
                {
                    if (range.EndIndexExclusive == index)
                    {
                        range = ranges[rangeIndex++];
                    }

                    output[y, x] = factory.Invoke(line[x], range);
                }
            }
        }
    }
}
