using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Rectangle : IDrawable
    {
        private readonly CharInfo[,] pattern;

        public Rectangle(Coord topLeft, CharInfo[,] pattern)
        {
            this.pattern = pattern;
            this.Region = new Region(topLeft, topLeft + new Coord(pattern.GetWidth(), pattern.GetHeight()));
        }

        public static Rectangle Empty { get; } = new Rectangle(new Coord(0, 0), new CharInfo[0, 0]);

        public IMoveOnlyRegion Region { get; }

        public void Draw(ISink sink)
        {
            sink.WriteRegion(this.pattern, this.Region.TopLeft);
        }
    }
}
