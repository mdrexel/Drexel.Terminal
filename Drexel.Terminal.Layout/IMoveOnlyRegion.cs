using System;

namespace Drexel.Terminal.Layout
{
    public interface IMoveOnlyRegion : IReadOnlyRegion
    {
        bool TryTranslate(Coord offset, out IReadOnlyRegion beforeChange);

        bool TryMoveTo(Coord newTopLeft, out IReadOnlyRegion beforeChange);

        event EventHandler<RegionChangeEventArgs>? OnChangeRequested;
    }
}
