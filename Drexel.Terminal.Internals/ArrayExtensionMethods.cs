using System.Runtime.CompilerServices;

namespace Drexel.Terminal.Internals
{
    /// <summary>
    /// Extension methods useful for working with arrays within the context of Drexel.Terminal.
    /// </summary>
    public static class ArrayExtensionMethods
    {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T[,] Repeat<T>(this T[,] pattern, short newHeight, short newWidth)
        {
            if (pattern.Length == 0)
            {
                return pattern;
            }

            short originalHeight = pattern.GetHeight();
            short originalWidth = pattern.GetWidth();

            T[,] result = new T[newHeight, newWidth];
            for (short y = 0; y < newHeight; y++)
            {
                for (short x = 0; x < newWidth; x++)
                {
                    result[y, x] = pattern[y % originalHeight, x % originalWidth];
                }
            }

            return result;
        }
    }
}
