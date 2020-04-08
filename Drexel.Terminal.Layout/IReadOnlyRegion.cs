using System;

namespace Drexel.Terminal.Layout
{
    /// <summary>
    /// Represents a two-dimensional region.
    /// </summary>
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
        short MathWidth { get; }

        /// <summary>
        /// Gets the mathematical width of the region.
        /// </summary>
        short MathHeight { get; }

        /// <summary>
        /// Gets the actual width of this region.
        /// </summary>
        short ActualWidth { get; }

        /// <summary>
        /// Gets the actual height of this region.
        /// </summary>
        short ActualHeight { get; }

        event EventHandler<RegionChangedEventArgs>? OnChanged;

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="IReadOnlyRegion"/> <paramref name="region"/>
        /// overlaps with this region.
        /// </summary>
        /// <param name="region">
        /// The region to check for an overlap with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this region overlaps with the specified region; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        bool Overlaps(IReadOnlyRegion region);

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="Coord"/> <paramref name="coord"/> overlaps with
        /// this region.
        /// </summary>
        /// <param name="coord">
        /// The coordinate to check for an overlap with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this region overlaps with the specified coordinate; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        bool Overlaps(Coord coord);

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="IReadOnlyRegion"/> <paramref name="region"/> is
        /// entirely contained within this region.
        /// </summary>
        /// <param name="region">
        /// The region to check against this region.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified region is entirely contained within this region; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        bool Contains(IReadOnlyRegion region);
    }
}
