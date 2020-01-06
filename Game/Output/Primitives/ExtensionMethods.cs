using System;

namespace Game.Output.Primitives
{
    internal static class ExtensionMethods
    {
        public static T[,] Repeat<T>(this T[,] pattern, short height, short width)
        {
            if (pattern.Length == 0)
            {
                return pattern;
            }

            short originalHeight = pattern.GetHeight();
            short originalWidth = pattern.GetWidth();

            T[,] result = new T[height, width];
            for (short y = 0; y < height; y++)
            {
                for (short x = 0; x < width; x++)
                {
                    result[y, x] = pattern[y % originalHeight, x % originalWidth];
                }
            }

            return result;
        }

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
    }
}
