using System;

namespace Drexel.Terminal.Layout
{
    public interface IReadOnlyRegion
    {
        /// <summary>
        /// Gets the inclusive top-left coordinate of the region.
        /// </summary>
        Coord TopLeft { get; }

        /// <summary>
        /// Gets the inclusive bottom-right coordinate of the region.
        /// </summary>
        Coord BottomRight { get; }

        /// <summary>
        /// Gets the mathematical width of the region.
        /// </summary>
        short Width { get; }

        /// <summary>
        /// Gets the mathematical width of the region.
        /// </summary>
        short Height { get; }

        event EventHandler<RegionChangedEventArgs>? OnChanged;

        bool Overlaps(IReadOnlyRegion region);

        bool Overlaps(Coord coord);

        bool Contains(IReadOnlyRegion region);
    }
}
