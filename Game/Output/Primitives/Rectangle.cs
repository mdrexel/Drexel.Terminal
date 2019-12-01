using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Rectangle : IDrawable
    {
        private readonly CharDelay[,]? delayedContent;
        private readonly CharInfo[,]? undelayedContent;

        public Rectangle(Coord topLeft, CharDelay[,] content)
        {
            this.delayedContent = content;
            this.Region = new Region(topLeft, topLeft + content.ToCoord());
        }

        public Rectangle(Coord topLeft, CharInfo[,] content)
        {
            this.undelayedContent = content;
            this.Region = new Region(topLeft, topLeft + content.ToCoord());
        }

        public static Rectangle Empty { get; } = new Rectangle(new Coord(0, 0), new CharInfo[0, 0]);

        public IMoveOnlyRegion Region { get; }

        public void Draw(ISink sink)
        {
            if (this.delayedContent is null)
            {
                sink.WriteRegion(this.undelayedContent, this.Region.TopLeft);
            }
            else
            {
                for (short yPos = 0; yPos < this.delayedContent.GetHeight(); yPos++)
                {
                    for (short xPos = 0; xPos < this.delayedContent.GetWidth(); xPos++)
                    {
                        sink.Write(
                            this.delayedContent[yPos, xPos],
                            this.Region.TopLeft + new Coord(xPos, yPos));
                    }
                }
            }
        }
    }
}
