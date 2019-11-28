namespace Game.Output.Layout
{
    public interface IMoveOnlyRegion : IReadOnlyRegion
    {
        new Coord TopLeft { get; set; }
    }
}
