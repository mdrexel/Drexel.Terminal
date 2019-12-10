using System;

namespace Game.Output.Layout
{
    public interface IMoveOnlyRegion : IReadOnlyRegion
    {
        void Translate(Coord offset);

        void MoveTo(Coord newTopLeft);

        event EventHandler<RegionChangeEventArgs>? OnChangeRequested;
    }
}
