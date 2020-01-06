using System;
using System.Linq;

namespace Game.Output.Primitives
{
    internal readonly struct ContentValue
    {
        private static readonly string[] NewLines = new string[] { Environment.NewLine };

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

        public Coord ToCoord()
        {
            return new Coord(this.Width, this.Height);
        }

        public CharInfo[,] ToCharInfoArray(CharInfo? fill)
        {
            return this.ToCharArray(
                (x, y) => new CharInfo(x, y.Attributes),
                fill.HasValue
                    ? () => fill!.Value
                    : (Func<CharInfo>?)null);
        }

        public CharDelay[,] ToCharDelayArray(CharDelay? fill)
        {
            return this.ToCharArray(
                (x, y) => new CharDelay(
                    new CharInfo(x, y.Attributes),
                    y.Delay),
                fill.HasValue
                    ? () => fill!.Value
                    : (Func<CharDelay>?)null);
        }

        private T[,] ToCharArray<T>(
            Func<char, Range, T> factory,
            Func<T>? fillFactory = null)
        {
            T[,] output = new T[this.Height, this.Width];

            if (!object.ReferenceEquals(fillFactory, null))
            {
                T fill = fillFactory.Invoke();
                for (int y = 0; y < this.Height; y++)
                {
                    for (int x = 0; x < this.Width; x++)
                    {
                        output[y, x] = fill;
                    }
                }
            }

            int rangeIndex = 0;
            Range range = this.Content.Ranges[rangeIndex++];
            for (int y = 0, index = 0; y < this.Height; y++, index += Environment.NewLine.Length)
            {
                string line = this.Lines[y];
                for (int x = 0; x < line.Length; x++, index++)
                {
                    if (range.EndIndexExclusive == index)
                    {
                        range = this.Content.Ranges[rangeIndex++];
                    }

                    output[y, x] = factory.Invoke(line[x], range);
                }
            }

            return output;
        }
    }
}
