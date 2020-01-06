using System;
using System.Collections.Generic;
using System.Linq;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public class Label : IDrawable
    {
        private static readonly string[] NewLines = new string[] { Environment.NewLine };

        private readonly ContentValue content;
        private readonly Alignments alignments;
        private readonly CharColors? backgroundFill;

        private CharDelay[,]? delayedContent;
        private CharInfo[,]? undelayedContent;

        public Label(
            FormattedString content,
            Alignments alignments,
            CharColors? backgroundFill = null)
        {
            this.content = new ContentValue(content);
            this.alignments = alignments;
            this.backgroundFill = backgroundFill;

            this.Ctor(
                new Region(
                    new Coord(0, 0),
                    new Coord(this.content.Width, this.content.Height)));
        }

        public Label(
            IReadOnlyRegion region,
            Alignments alignments,
            FormattedString content,
            CharColors? backgroundFill = null)
        {
            this.content = new ContentValue(content);
            this.alignments = alignments;
            this.backgroundFill = backgroundFill;

            this.Ctor(region);
        }

        private Label(Coord topLeft, CharInfo[,] content, ContentValue originalContent)
        {
            this.content = originalContent;
            this.undelayedContent = content;
            this.Region = new Region(topLeft, content.ToCoord() + topLeft);
        }

        private Label(Coord topLeft, CharDelay[,] content, ContentValue originalContent)
        {
            this.content = originalContent;
            this.delayedContent = content;
            this.Region = new Region(topLeft, content.ToCoord() + topLeft);
        }

        public static Label Empty { get; } = new Label(FormattedString.Empty, Alignments.Default);

        public IMoveOnlyRegion Region { get; private set; }

        internal CharDelay[,]? DelayedContent => this.delayedContent;

        internal CharInfo[,]? UndelayedContent => this.undelayedContent;

        internal bool HasDelayedContent => this.content.Content.ContainsDelays;

        public void Draw(ISink sink)
        {
            if (this.HasDelayedContent)
            {
                sink.WriteRegion(this.delayedContent!, this.Region.TopLeft);
            }
            else
            {
                sink.WriteRegion(this.undelayedContent!, this.Region.TopLeft);
            }
        }

        public void Draw(ISink sink, Rectangle region)
        {
            if (this.HasDelayedContent)
            {
                sink.WriteRegion(
                    this.delayedContent!,
                    this.Region.TopLeft,
                    region);
            }
            else
            {
                sink.WriteRegion(
                    this.undelayedContent!,
                    this.Region.TopLeft,
                    region);
            }
        }

        public void InvertColor()
        {
            if (this.HasDelayedContent)
            {
                for (int y = 0; y < this.delayedContent!.GetHeight(); y++)
                {
                    for (int x = 0; x < this.delayedContent!.GetWidth(); x++)
                    {
                        this.delayedContent![y, x] = this.delayedContent[y, x].GetInvertedColor();
                    }
                }
            }
            else
            {
                for (int y = 0; y < this.undelayedContent!.GetHeight(); y++)
                {
                    for (int x = 0; x < this.undelayedContent!.GetWidth(); x++)
                    {
                        this.undelayedContent![y, x] = this.undelayedContent[y, x].GetInvertedColor();
                    }
                }
            }
        }

        internal Label RepeatHorizontally(short width)
        {
            if (this.HasDelayedContent)
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatHorizontallyInternal(this.delayedContent!, width),
                    this.content);
            }
            else
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatHorizontallyInternal(this.undelayedContent!, width),
                    this.content);
            }
        }

        internal Label RepeatVertically(short height)
        {
            if (this.HasDelayedContent)
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatVerticallyInternal(this.delayedContent!, height),
                    this.content);
            }
            else
            {
                return new Label(
                    this.Region.TopLeft,
                    RepeatVerticallyInternal(this.undelayedContent!, height),
                    this.content);
            }
        }

        private static T[,] RepeatHorizontallyInternal<T>(T[,] pattern, short width)
        {
            if (pattern.Length == 0)
            {
                return pattern;
            }

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
            if (pattern.Length == 0)
            {
                return pattern;
            }

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
            CharColors? backgroundFill,
            string[] lines,
            IReadOnlyList<Range> ranges,
            Func<char, Range, T> factory)
        {
            if (backgroundFill.HasValue)
            {
                Range fakeRange = new Range(0, 0, backgroundFill.Value);
                for (int y = 0; y < output.GetHeight(); y++)
                {
                    for (int x = 0; x < output.GetWidth(); x++)
                    {
                        output[y, x] = factory.Invoke(' ', fakeRange);
                    }
                }
            }

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

        private void Ctor(IReadOnlyRegion region)
        {
            if (this.content.Content.Value.Length == 0)
            {
                this.Region = new Region(region.TopLeft, region.TopLeft);
                this.undelayedContent = new CharInfo[0, 0];

                return;
            }

            this.Region = new Region(region);
            this.Region.OnChangeRequested +=
                (obj, e) =>
                {
                    if (e.AfterChange.Height < this.content.Height || e.AfterChange.Width < this.content.Width)
                    {
                        e.Cancel = true;
                    }
                };

            this.Region.OnChanged +=
                (obj, e) =>
                {
                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Move))
                    {
                        this.Region.TryMoveTo(e.AfterChange.TopLeft, out _);
                    }

                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                    {
                        this.Recalculate();
                    }
                };

            this.Recalculate();
        }

        private void Recalculate()
        {
            if (this.content.Content.ContainsDelays)
            {
                this.delayedContent = new CharDelay[this.content.Height, this.content.Width];
                Label.Process(
                    in this.delayedContent!,
                    this.backgroundFill,
                    this.content.Lines,
                    this.content.Content.Ranges,
                    (x, y) => new CharDelay(
                        new CharInfo(x, y.Attributes),
                        y.Delay));
            }
            else
            {
                this.undelayedContent = new CharInfo[this.content.Height, this.content.Width];
                Label.Process(
                    in this.undelayedContent!,
                    this.backgroundFill,
                    this.content.Lines,
                    this.content.Content.Ranges,
                    (x, y) => new CharInfo(x, y.Attributes));
            }
        }

        private readonly struct ContentValue
        {
            public readonly FormattedString Content;
            public readonly string[] Lines;
            public readonly short Height;
            public readonly short Width;

            public ContentValue(FormattedString content)
            {
                this.Content = content;

                this.Lines = content.Value.Split(NewLines, StringSplitOptions.None);
                this.Height = (short)this.Lines.Length;
                this.Width = (short)this.Lines.Max(x => x.Length);
            }
        }
    }
}
