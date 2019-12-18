using System;

namespace Game.Output.Layout
{
    public interface IReadOnlyRegion
    {
        Coord TopLeft { get; }

        Coord BottomRight { get; }

        short Width { get; }

        short Height { get; }

        event EventHandler<RegionChangedEventArgs>? OnChanged;

        bool Overlaps(IReadOnlyRegion region);

        bool Overlaps(Coord coord);

        bool Contains(IReadOnlyRegion region);
    }
}
