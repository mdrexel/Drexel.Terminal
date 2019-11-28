using System;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Text : IDrawable
    {
        private readonly IReadOnlyRegion constrainedTo;

        private CharInfo[,] buffer;

        public Text(
            Coord topLeft,
            CharInfo[,] content)
        {
            this.buffer = content;
            this.constrainedTo = new Region(topLeft, topLeft + content.ToCoord());
        }

        public Text(
            string content,
            CharColors colors,
            IReadOnlyRegion constrainedTo)
        {
            this.constrainedTo = constrainedTo;

            void InnerCalculate()
            {
                if (content.Length <= constrainedTo.Width)
                {
                    CharInfo[,] working = new CharInfo[1, content.Length];
                    for (int counter = 0; counter < content.Length; counter++)
                    {
                        this.buffer[1, counter] = new CharInfo(content[counter], colors);
                    }

                    buffer = working;
                }
                else
                {
                    throw new NotImplementedException(
                        "Haven't implemented line break magicks");
                }
            }

            constrainedTo.OnChanged += (obj, e) => InnerCalculate();

            InnerCalculate();
        }

        public static Text Empty { get; } = new Text(new Coord(0, 0), new CharInfo[0, 0]);

        public void Draw(ISink sink)
        {
            sink.WriteRegion(
                this.buffer,
                this.constrainedTo.TopLeft.X,
                this.constrainedTo.TopLeft.Y);
        }
    }
}
