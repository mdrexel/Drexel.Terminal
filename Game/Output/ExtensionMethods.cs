using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Game.Output.Layout;

namespace Game.Output
{
    internal static class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Coord ToCoord<T>(this T[,] array)
        {
            return new Coord(array.GetWidth(), array.GetHeight());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetWidth<T>(this T[,] array)
        {
            return (short)array.GetLength(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetHeight<T>(this T[,] array)
        {
            return (short)array.GetLength(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetWidth<T>(this T[,]? array, short @default)
        {
            if (array is null)
            {
                return default;
            }

            return (short)array.GetLength(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetHeight<T>(this T[,]? array, short @default)
        {
            if (array is null)
            {
                return default;
            }

            return (short)array.GetLength(0);
        }
    }
}
