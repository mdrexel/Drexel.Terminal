using System.Runtime.CompilerServices;

namespace Game.Output
{
    internal static class ExtensionMethods
    {
        public static void WriteRegion(
            this ISink sink,
            CharInfo[,]? buffer,
            Coord topLeft)
        {
            if (!(buffer is null))
            {
                sink.WriteRegion(buffer, topLeft.X, topLeft.Y);
            }
        }

        public static void WriteRegion(
            this ISink sink,
            CharInfo[,]? buffer,
            short left,
            short top)
        {
            if (!(buffer is null))
            {
                sink.WriteRegion(buffer, left, top);
            }
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
    }
}
