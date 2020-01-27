using System.Runtime.CompilerServices;

namespace Drexel.Terminal
{
    /// <summary>
    /// Extension methods useful for working with arrays within the context of this package.
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

        /// <summary>
        /// Gets the width of the specified <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the array.
        /// </typeparam>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The width of the specified <paramref name="array"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetWidth<T>(this T[,] array)
        {
            checked
            {
                return (short)array.GetLength(1);
            }
        }

        /// <summary>
        /// Gets the height of the specified <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the array.
        /// </typeparam>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The height of the specified <paramref name="array"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetHeight<T>(this T[,] array)
        {
            checked
            {
                return (short)array.GetLength(0);
            }
        }
    }
}
