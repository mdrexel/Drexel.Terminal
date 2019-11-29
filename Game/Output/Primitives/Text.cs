using System.Threading;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Text : IDrawable
    {
        private readonly IReadOnlyRegion constrainedTo;

        private CharInfo[,] buffer;
        private int[,]? delaysInMilliseconds;

        public Text(
            Coord topLeft,
            CharInfo[,] content,
            int[,]? delaysInMilliseconds = null)
        {
            this.buffer = content;
            this.constrainedTo = new Region(topLeft, topLeft + content.ToCoord());
            this.delaysInMilliseconds = delaysInMilliseconds;
        }

        public Text(
            string content,
            CharColors colors,
            IReadOnlyRegion constrainedTo,
            int[,]? delaysInMilliseconds = null)
        {
            this.constrainedTo = constrainedTo;
            this.delaysInMilliseconds = delaysInMilliseconds;

            void InnerCalculate()
            {
                this.buffer = content.ToCharInfo(colors, constrainedTo.Width, constrainedTo.Height);
            }

            constrainedTo.OnChanged += (obj, e) => InnerCalculate();

            InnerCalculate();
        }

        public static Text Empty { get; } = new Text(new Coord(0, 0), new CharInfo[0, 0]);

        public void Draw(ISink sink)
        {
            if (this.delaysInMilliseconds is null)
            {
                sink.WriteRegion(this.buffer, this.constrainedTo.TopLeft);
            }
            else
            {
                for (int yPos = 0; yPos < this.buffer.GetHeight(); yPos++)
                {
                    for (int xPos = 0; xPos < this.buffer.GetWidth(); xPos++)
                    {
                        CharInfo info = this.buffer[yPos, xPos];
                        int delay = this.delaysInMilliseconds[yPos, xPos];

                        sink.Write(info);
                        Thread.Sleep(delay);
                    }
                }
            }
        }
    }
}
