using System.Runtime.CompilerServices;

namespace Drexel.Terminal.Internals
{
    internal static class Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[,] RepeatHorizontally<T>(this T[,] pattern, short width)
        {
            if (pattern.Length == 0)
            {
                return pattern;
            }

            short originalWidth = pattern.GetWidth();

            short height = pattern.GetHeight();
            T[,] result = new T[height, width];
            for (short y = 0; y < height; y++)
            {
                for (short x = 0; x < width; x++)
                {
                    result[y, x] = pattern[y, x % originalWidth];
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[,] RepeatVertically<T>(this T[,] pattern, short height)
        {
            if (pattern.Length == 0)
            {
                return pattern;
            }

            short originalHeight = pattern.GetHeight();

            short width = pattern.GetWidth();
            T[,] result = new T[height, width];
            for (short y = 0; y < height; y++)
            {
                for (short x = 0; x < width; x++)
                {
                    result[y, x] = pattern[y % originalHeight, x];
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[,] Fill<T>(this T[,] array, T value)
        {
            for (int y = 0; y < array.GetHeight(); y++)
            {
                for (int x = 0; x < array.GetWidth(); x++)
                {
                    array[y, x] = value;
                }
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static U[,] CreateSameSizeArray<T, U>(this T[,] array, U defaultValue)
        {
            U[,] result = new U[array.GetHeight(), array.GetWidth()];
            result.Fill(defaultValue);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DivRem(int numerator, int divisor, out int remainder)
        {
            int quotient = numerator / divisor;
            remainder = numerator - (quotient * divisor);

            return quotient;
        }
    }
}