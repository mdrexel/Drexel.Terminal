using System.Runtime.CompilerServices;

namespace Drexel.Terminal.Win32
{
    internal static class Utilities
    {
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