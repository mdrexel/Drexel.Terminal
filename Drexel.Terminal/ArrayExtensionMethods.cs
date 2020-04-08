using System.Runtime.CompilerServices;
using Drexel.Terminal.Internals;

namespace Drexel.Terminal
{
    /// <summary>
    /// Extension methods useful for working with arrays within the context of Drexel.Terminal.
    /// </summary>
    public static class ArrayExtensionMethods
    {
        /// <summary>
        /// Returns a <see cref="Coord"/> with horizontal and vertical positions equal to the width and height of the
        /// specified <paramref name="array"/>, respectively.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the array.
        /// </typeparam>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// A <see cref="Coord"/> with horizontal and vertical positions equal to the width and height of the
        /// specified <paramref name="array"/>, respectively.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Coord ToCoord<T>(this T[,] array)
        {
            return new Coord(array.GetWidth(), array.GetHeight());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T[,] Repeat<T>(this T[,] pattern, Coord newSize)
        {
            return pattern.Repeat(newSize.Y, newSize.X);
        }
    }
}
