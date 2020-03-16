namespace Drexel.Terminal.Layout
{
    public interface IRegion : IMoveOnlyRegion
    {
        new Coord TopLeft { get; set; }

        new Coord BottomRight { get; set; }

        new short Width { get; set; }

        new short Height { get; set; }

        bool TrySetCorners(Coord newTopLeft, Coord newBottomRight, out IReadOnlyRegion beforeChange);
    }
}
