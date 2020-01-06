using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Text : IDrawable
    {
        private static readonly short NewLineLength = (short)Environment.NewLine.Length;
        private static readonly string[] NewLine = new string[] { Environment.NewLine };

        private readonly FormattedString content;
        private readonly CharColors? backgroundFill;

        private Fill rectangle;
        private IReadOnlyList<Line> lines;

        private ushort preceedingLinesSkipped;

        public Text(
            FormattedString content,
            IReadOnlyRegion constrainedTo,
            CharColors? backgroundFill = null)
            : this(content, new Region(constrainedTo), backgroundFill)
        {
        }

        public Text(
            FormattedString content,
            Region region,
            CharColors? backgroundFill = null)
        {
            this.content = content;
            this.backgroundFill = backgroundFill;
            this.preceedingLinesSkipped = 0;

            this.Region = region;

            region.OnChanged +=
                (obj, e) =>
                {
                    // Only need to recalculate contents on resize
                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                    {
                        this.ReflowContent(e.BeforeChange.Width != e.AfterChange.Width);
                    }
                };

            this.ReflowContent(true);
        }

        public static Text Empty { get; } =
            new Text(
                FormattedString.Empty,
                new Region(
                    new Coord(0, 0),
                    new Coord(0, 0)));

        public Region Region { get; }

        public ushort TotalLines { get; private set; }

        public ushort PreceedingLinesSkipped
        {
            get => this.preceedingLinesSkipped;
            private set
            {
                this.preceedingLinesSkipped = (ushort)Math.Min(value, this.TotalLines - this.MaximumVisibleLines);
            }
        }

        public ushort MaximumVisibleLines { get; private set; }

        public void Draw(ISink sink)
        {
            this.rectangle.Draw(sink);
        }

        public void Draw(ISink sink, Rectangle window)
        {
            this.rectangle.Draw(sink, window);
        }

        public void InvertColor()
        {
            this.rectangle.InvertColor();
        }

        public void AdjustLinesSkipped(int delta)
        {
            if (delta == 0)
            {
                return;
            }

            checked
            {
                if (delta < 0)
                {
                    delta = -delta;
                    if (delta > this.PreceedingLinesSkipped)
                    {
                        this.PreceedingLinesSkipped = 0;
                    }
                    else
                    {
                        this.PreceedingLinesSkipped -= (ushort)delta;
                    }
                }
                else
                {
                    this.PreceedingLinesSkipped += (ushort)delta;
                }
            }

            this.PopulateDrawBuffer();
        }

        private void ReflowContent(bool widthChanged)
        {
            this.MaximumVisibleLines = (ushort)this.Region.Height;

            if (widthChanged)
            {
                string[] rawLines = this.content.Value.Split(NewLine, StringSplitOptions.None);
                Line[] buffer = new Line[rawLines.Length];

                Line previous = new Line(string.Empty, (ushort)-NewLineLength);
                for (int counter = 0; counter < rawLines.Length; counter++)
                {
                    string raw = rawLines[counter];
                    Line line = new Line(raw, (ushort)(previous.StartOffset + previous.Content.Length + NewLineLength));
                    buffer[counter] = line;
                    previous = line;
                }

                List<Line> calculated = new List<Line>();
                short width = this.Region.Width;
                foreach (Line line in buffer)
                {
                    if (line.Content.Length <= width)
                    {
                        calculated.Add(line);
                    }
                    else
                    {
                        // Line is too wide to fit the current region - need to split it into multiple lines.
                        string[] words = line.Content.Split(' ');
                        ushort sumOfLengths = 0;
                        ushort startOffset = line.StartOffset;
                        int lastLineEndedOnIndex = 0;
                        for (int counter = 0; counter < words.Length; counter++)
                        {
                            string word = words[counter];
                            if (word.Length > width)
                            {
                                // Word is longer than an entire line. We need to:
                                // 1. Stop the current line and add it to the output list
                                // 2. Split the word into as many lines as would be *ENTIRELY FILLED* by it and add them to the list
                                // 3. Resume on the last partially-filled line
                                throw new NotImplementedException("Word too long");
                            }
                            else
                            {
                                sumOfLengths = (ushort)(sumOfLengths + word.Length);
                                bool writeLine = sumOfLengths == width || counter == words.Length - 1;
                                if (sumOfLengths > width)
                                {
                                    // Line is too long - write the line excluding the current word
                                    sumOfLengths = (ushort)(sumOfLengths - word.Length);
                                    counter--;
                                    writeLine = true;
                                }
                                else
                                {
                                    // Line isn't filled yet - keep going
                                    sumOfLengths++;
                                }

                                if (writeLine)
                                {
                                    // Line is filled - write the current line, including the current word
                                    Line newLine = new Line(
                                        string.Join(
                                            " ",
                                            words,
                                            Math.Max(0, lastLineEndedOnIndex),
                                            counter - lastLineEndedOnIndex + 1),
                                        startOffset);
                                    calculated.Add(newLine);

                                    startOffset += sumOfLengths;
                                    sumOfLengths = 0;
                                    lastLineEndedOnIndex = counter + 1;
                                }
                            }
                        }
                    }
                }

                this.lines = calculated;
                this.TotalLines = (ushort)this.lines.Count;

                // Intentionally set lines skipped to itself, so that overflow is evaluated
                this.PreceedingLinesSkipped = this.PreceedingLinesSkipped;
            }

            this.PopulateDrawBuffer();
        }

        private void PopulateDrawBuffer()
        {
            Fill Populate<T>(
                Func<char, Range, T> charFactory,
                Func<T[,], Fill> rectangleFactory)
            {
                T[,] output = new T[this.Region.Height, this.Region.Width];
                if (this.backgroundFill.HasValue)
                {
                    Range fakeRange = new Range(0, 0, this.backgroundFill.Value, 0);
                    for (int y = 0; y < this.Region.Height; y++)
                    {
                        for (int x = 0; x < this.Region.Width; x++)
                        {
                            output[y, x] = charFactory.Invoke(' ', fakeRange);
                        }
                    }
                }

                IEnumerator<Line> line = lines.Skip(this.PreceedingLinesSkipped).GetEnumerator();
                for (int y = 0; y < this.Region.Height && line.MoveNext(); y++)
                {
                    Range range = this.content.Ranges.GetRangeByIndex(line.Current.StartOffset);
                    for (int x = 0; x < line.Current.Content.Length; x++)
                    {
                        int index = x + line.Current.StartOffset;
                        if (index == range.EndIndexExclusive)
                        {
                            range = this.content.Ranges.GetNextRange(range);
                        }

                        output[y, x] = charFactory.Invoke(line.Current.Content[x], range);
                    }
                }

                return rectangleFactory.Invoke(output);
            }

            if (this.content.ContainsDelays)
            {
                this.rectangle = Populate(
                    (x, y) => new CharDelay(new CharInfo(x, y.Attributes), y.Delay),
                    x => new Fill(this.Region.TopLeft, x));
            }
            else
            {
                this.rectangle = Populate(
                    (x, y) => new CharInfo(x, y.Attributes),
                    x => new Fill(this.Region.TopLeft, x));
            }
        }

        [DebuggerDisplay("{StartOffset,nq}: {Content} ({Content.Length,nq})")]
        private class Line
        {
            public Line(string content, ushort startOffset)
            {
                this.Content = content;
                this.StartOffset = startOffset;
            }

            public string Content { get; }

            public ushort StartOffset { get; }
        }
    }
}
