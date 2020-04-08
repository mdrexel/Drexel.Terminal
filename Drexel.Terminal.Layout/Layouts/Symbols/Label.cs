using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Text;

namespace Drexel.Terminal.Layout.Layouts.Symbols
{
    public class Label : Symbol
    {
        private static readonly short NewLineLength = (short)Environment.NewLine.Length;
        private static readonly string[] NewLine = new string[] { Environment.NewLine };

        private readonly Catena content;
        private readonly Alignments alignments;
        private readonly TerminalColors? backgroundFill;
        private readonly bool synchronousDraw;

        private readonly object cacheLock;
        private CharInfo[,] cached;
        private IReadOnlyList<TextLine> lines;

        private ushort preceedingLinesSkipped;

        public Label(
            IResizeableRegion region,
            string name,
            Catena content,
            Alignments alignments,
            TerminalColors? backgroundFill = null,
            bool synchronousDraw = true)
            : base(region, name)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.content = content;
            this.alignments = alignments;
            this.backgroundFill = backgroundFill;
            this.synchronousDraw = synchronousDraw;
            this.preceedingLinesSkipped = 0;

            this.cacheLock = new object();
            this.cached = null!;

            this.AddDisposable(
                region.OnChanged.Subscribe(
                    new Observer<RegionChangedEventArgs>(
                        x =>
                        {
                            lock (this.cacheLock)
                            {
                                if (x.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                                {
                                    this.MaximumVisibleLines = (ushort)x.AfterChange.ActualHeight;
                                    this.cached =
                                        this.Generate(x.BeforeChange.ActualWidth != x.AfterChange.ActualWidth);
                                }

                                this.RequestRedraw(x.BeforeChange, x.AfterChange);
                            }
                        })));

            this.cached = this.Generate(true);
        }

        public override bool CanBeFocused => false;

        public override bool CapturesTabKey => false;

        public ushort TotalLines { get; private set; }

        public ushort PreceedingLinesSkipped
        {
            get => this.preceedingLinesSkipped;
            private set
            {
                lock (this.cacheLock)
                {
                    this.preceedingLinesSkipped = (ushort)Math.Min(value, this.TotalLines - this.MaximumVisibleLines);
                    this.cached = this.Generate(false);
                    this.RequestRedraw();
                }
            }
        }

        public ushort MaximumVisibleLines { get; private set; }

        public override void Draw(ITerminalSink sink, Rectangle window)
        {
            if (!this.Region.Overlaps(window))
            {
                return;
            }

            lock (this.cacheLock)
            {
                Coord destination = this.Region.TopLeft;
                window = new Rectangle(
                    (short)(window.Left - destination.X),
                    (short)(window.Top - destination.Y),
                    (short)(window.Right + 1 - destination.X),
                    (short)(window.Bottom + 1 - destination.Y));

                if (this.synchronousDraw)
                {
                    sink.Write(this.cached!, destination, window);
                }
                else
                {
                    CharInfo[,] buffer = this.cached.CreateSameSizeArray<CharInfo, CharInfo>(default);
                    Array.Copy(this.cached, buffer, this.cached.Length);

                    Thread thread = new Thread(
                        x =>
                        {
                            var tuple =
                                ((ITerminalSink Sink, CharInfo[,] Buffer, Coord Destination, Rectangle Window))x;

                            tuple.Sink.Write(tuple.Buffer, tuple.Destination, tuple.Window);
                        });
                    thread.Start((sink, buffer, destination, window));
                }
            }
        }

        public void AdjustLinesSkipped(int delta)
        {
            if (delta == 0)
            {
                return;
            }

            lock (this.cacheLock)
            {
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

                this.cached = this.Generate(false);
                this.RequestRedraw();
            }
        }

        [DebuggerDisplay("{StartOffset,nq}: {Content} ({Content.Length,nq})")]
        private class TextLine
        {
            public TextLine(string content, ushort startOffset)
            {
                this.Content = content;
                this.StartOffset = startOffset;
            }

            public string Content { get; }

            public ushort StartOffset { get; }
        }

        private CharInfo[,] Generate(bool widthChanged)
        {
            this.MaximumVisibleLines = (ushort)this.Region.ActualHeight;

            if (widthChanged)
            {
                string[] rawLines = this.content.Value.Split(NewLine, StringSplitOptions.None);
                TextLine[] buffer = new TextLine[rawLines.Length];

                TextLine previous = new TextLine(string.Empty, (ushort)-NewLineLength);
                for (int counter = 0; counter < rawLines.Length; counter++)
                {
                    string raw = rawLines[counter];
                    TextLine line = new TextLine(raw, (ushort)(previous.StartOffset + previous.Content.Length + NewLineLength));
                    buffer[counter] = line;
                    previous = line;
                }

                List<TextLine> calculated = new List<TextLine>();
                short width = this.Region.ActualWidth;
                foreach (TextLine line in buffer)
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
                                    TextLine newLine = new TextLine(
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

                if (calculated.Any())
                {
                    int indexOfLastLine = calculated.Count - 1;
                    if (string.IsNullOrEmpty(calculated[indexOfLastLine].Content))
                    {
                        calculated.RemoveAt(indexOfLastLine);
                    }
                }

                this.lines = calculated;
                this.TotalLines = (ushort)this.lines.Count;

                // Intentionally set lines skipped to itself, so that overflow is evaluated
                this.PreceedingLinesSkipped = this.PreceedingLinesSkipped;
            }

            if (this.alignments.HorizontalAlignment == HorizontalAlignment.Left)
            {
                if (this.alignments.VerticalAlignment == VerticalAlignment.Top)
                {
                    return this.GenerateTopLeft();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Center)
                {
                    return this.GenerateCenterLeft();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return this.GenerateBottomLeft();
                }
            }
            else if (this.alignments.HorizontalAlignment == HorizontalAlignment.Center)
            {
                if (this.alignments.VerticalAlignment == VerticalAlignment.Top)
                {
                    return this.GenerateTopCenter();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Center)
                {
                    return this.GenerateCenterCenter();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return this.GenerateBottomCenter();
                }
            }
            else if (this.alignments.HorizontalAlignment == HorizontalAlignment.Right)
            {
                if (this.alignments.VerticalAlignment == VerticalAlignment.Top)
                {
                    return this.GenerateTopRight();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Center)
                {
                    return this.GenerateCenterRight();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return this.GenerateBottomRight();
                }
            }

            throw new InvalidOperationException("Unrecognized combination of alignments.");
        }

        private CharInfo[,] GenerateTopLeft()
        {
            CharInfo[,] output = new CharInfo[this.Region.ActualHeight, this.Region.ActualWidth];
            if (this.backgroundFill.HasValue)
            {
                for (int y = 0; y < this.Region.ActualHeight; y++)
                {
                    for (int x = 0; x < this.Region.ActualWidth; x++)
                    {
                        output[y, x] = new CharInfo(' ', this.backgroundFill.Value);
                    }
                }
            }

            IEnumerator<TextLine> line = lines.Skip(this.PreceedingLinesSkipped).GetEnumerator();
            for (int y = 0; y < this.Region.ActualHeight && line.MoveNext(); y++)
            {
                Range range = this.content.Ranges.GetRangeByIndex(line.Current.StartOffset);
                for (int x = 0; x < line.Current.Content.Length; x++)
                {
                    int index = x + line.Current.StartOffset;
                    if (index == range.EndIndexExclusive)
                    {
                        range = this.content.Ranges.GetNextRange(range);
                    }

                    output[y, x] = new CharInfo(line.Current.Content[x], range.Colors, range.Delay);
                }
            }

            return output;
        }

        private CharInfo[,] GenerateCenterLeft()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateBottomLeft()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateTopCenter()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateCenterCenter()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateBottomCenter()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateTopRight()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateCenterRight()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateBottomRight()
        {
            throw new NotImplementedException();
        }
    }
}
