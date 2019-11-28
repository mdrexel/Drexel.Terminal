using System;

namespace Game.Output.Layout
{
    public interface IReadOnlyRegion
    {
        Coord TopLeft { get; }

        Coord BottomRight { get; }

        short Width { get; }

        short Height { get; }

        event EventHandler<RegionChangeEventArgs>? OnChangeRequested;

        event EventHandler<RegionChangedEventArgs>? OnChanged;
    }
}
