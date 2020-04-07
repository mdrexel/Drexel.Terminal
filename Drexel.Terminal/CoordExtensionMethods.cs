namespace Drexel.Terminal
{
    /// <summary>
    /// Extension methods for working with <see cref="Coord"/>s.
    /// </summary>
    public static class CoordExtensionMethods
    {
        /// <summary>
        /// Returns a <see cref="Coord"/> with the <see cref="Coord.X"/> and <see cref="Coord.Y"/> position of
        /// <paramref name="toInvert"/> <see cref="Coord"/> switched.
        /// </summary>
        /// <returns>
        /// A <see cref="Coord"/> with the X and Y position of this <see cref="Coord"/> switched.
        /// </returns>
        public static Coord Invert(this Coord toInvert)
        {
            return new Coord(toInvert.Y, toInvert.X);
        }
    }
}
