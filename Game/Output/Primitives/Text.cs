using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Text : IDrawable
    {
        private readonly IReadOnlyRegion constrainedTo;

        private CharDelay[,]? delayedContent;
        private CharInfo[,]? undelayedContent;

        public Text(Coord topLeft, CharDelay[,] content)
        {
            this.delayedContent = content;
            this.constrainedTo = new Region(topLeft, topLeft + content.ToCoord());
        }

        public Text(Coord topLeft, CharInfo[,] content)
        {
            this.undelayedContent = content;
            this.constrainedTo = new Region(topLeft, topLeft + content.ToCoord());
        }

        public Text(
            string content,
            CharColors colors,
            IReadOnlyRegion constrainedTo,
            CharColors? backgroundFill = null)
        {
            this.constrainedTo = constrainedTo;

            void InnerCalculate()
            {
                this.undelayedContent = content.ToCharInfo(colors, constrainedTo, backgroundFill);
            }

            constrainedTo.OnChanged += (obj, e) => InnerCalculate();

            InnerCalculate();
        }

        public static Text Empty { get; } = new Text(new Coord(0, 0), new CharDelay[0, 0]);

        public void Draw(ISink sink)
        {
            if (this.delayedContent is null)
            {
                sink.WriteRegion(this.undelayedContent, this.constrainedTo.TopLeft);
            }
            else
            {
                for (short yPos = 0; yPos < this.delayedContent.GetHeight(); yPos++)
                {
                    for (short xPos = 0; xPos < this.delayedContent.GetWidth(); xPos++)
                    {
                        sink.Write(
                            this.delayedContent[yPos, xPos],
                            this.constrainedTo.TopLeft + new Coord(xPos, yPos));
                    }
                }
            }
        }
    }
}
