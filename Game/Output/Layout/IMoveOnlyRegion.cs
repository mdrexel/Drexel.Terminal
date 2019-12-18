using System;

namespace Game.Output.Layout
{
    public interface IMoveOnlyRegion : IReadOnlyRegion
    {
        bool Translate(Coord offset);

        bool MoveTo(Coord newTopLeft);

        event EventHandler<RegionChangeEventArgs>? OnChangeRequested;
    }
}
