using System.Runtime.CompilerServices;

namespace Game.Output
{
    internal static class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteRegion(
            this ISink sink,
            CharInfo[,] buffer,
            Coord topLeft)
        {
            sink.WriteRegion(buffer, topLeft.X, topLeft.Y);
        }

        public static CharInfo[,] ToCharInfo(this string value, CharColors colors)
        {
            CharInfo[,] result = new CharInfo[1, value.Length];
            for (int counter = 0; counter < value.Length; counter++)
            {
                result[0, counter] = new CharInfo(value[counter], colors);
            }

            return result;
        }

        public static CharInfo[,] ToCharInfo(
            this string value,
            CharColors colors,
            short maximumWidth,
            short maximumHeight)
        {
            CharInfo[,] result = new CharInfo[maximumHeight, maximumWidth];
            for (int y = 0; y < maximumWidth; y++)
            {
                for (int x = 0; x < maximumWidth; x++)
                {
                    if (x + y > value.Length)
                    {
                        goto leave;
                    }

                    result[y, x] = new CharInfo(value[x + y], colors);
                }
            }

            leave: return result;
        }

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
