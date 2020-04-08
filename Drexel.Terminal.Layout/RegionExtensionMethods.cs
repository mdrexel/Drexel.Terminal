using System;

namespace Drexel.Terminal.Layout
{
    /// <summary>
    /// Extension methods for working with regions.
    /// </summary>
    public static class RegionExtensionMethods
    {
        /// <summary>
        /// Returns a value indicating whether <paramref name="region"/> can be translated by the specified
        /// <paramref name="offset"/>.
        /// </summary>
        /// <param name="region">
        /// The region to check.
        /// </param>
        /// <param name="offset">
        /// The offset to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="region"/> can be translated by the specified
        /// <paramref name="offset"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool CanTranslateBy(this IMoveOnlyRegion region, Coord offset)
        {
            if (region is null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            return region.CanMoveTo(region.TopLeft + offset);
        }

        /// <summary>
        /// Tries to translate <paramref name="region"/> by the specified <paramref name="offset"/>, and returns a
        /// value indicating whether the translation was successful. If the translation was successful,
        /// <see langword="true"/> will be returned, and <paramref name="beforeChange"/> will be set to a region
        /// equivalent to <paramref name="region"/> before the translation was applied.
        /// </summary>
        /// <param name="region">
        /// The region to try to translate.
        /// </param>
        /// <param name="offset">
        /// The offset by which to translate this region.
        /// </param>
        /// <param name="beforeChange">
        /// If the translation succeeds, a region equivalent to this region before the translation was applied.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the translation succeeded; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryTranslateBy(this IMoveOnlyRegion region, Coord offset, out IReadOnlyRegion beforeChange)
        {
            if (region is null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            return region.TryMoveTo(region.TopLeft + offset, out beforeChange);
        }
    }
}
