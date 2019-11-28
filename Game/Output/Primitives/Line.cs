namespace Game.Output.Primitives
{
    public sealed class Line : IDrawable
    {
        public Line(Coord start, Coord end, CharInfo[,] pattern)
        {
            this.Start = start;
            this.End = end;
            this.Pattern = pattern;
        }

        public Coord Start { get; }

        public Coord End { get; }

        public CharInfo[,] Pattern { get; }

        public void Draw(ISink sink)
        {
            throw new System.NotImplementedException();
        }
    }
}
