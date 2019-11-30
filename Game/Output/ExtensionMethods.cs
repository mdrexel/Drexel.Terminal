using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Game.Output.Layout;

namespace Game.Output
{
    internal static class ExtensionMethods
    {
        private static readonly string[] NewLines = new string[] { Environment.NewLine };

        public static CharInfo[,] ToCharInfo(
            this string value,
            CharColors colors,
            IReadOnlyRegion region,
            CharColors? backgroundFill = null) =>
            value.ToCharInfo(
                colors,
                backgroundFill,
                region.Width,
                region.Height);

        public static CharInfo[,] ToCharInfo(
            this string value,
            CharColors colors,
            CharColors? backgroundFill = null,
            short? maximumWidth = null,
            short? maximumHeight = null)
        {
            string[] lines = value.Split(NewLines, StringSplitOptions.None);
            short maxX = maximumWidth.HasValue ? maximumWidth.Value : (short)lines.Max(x => x.Length);

            string[][] values = lines.Select(x => x.Split(' ')).ToArray();
            short maxY = maximumHeight.HasValue ? maximumHeight.Value : (short)values.Length;

            CharInfo[,] result = new CharInfo[maxY, maxX];

            if (backgroundFill.HasValue)
            {
                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        result[y, x] = new CharInfo(' ', backgroundFill.Value);
                    }
                }
            }

            int yOffset = 0;
            foreach (string[] line in values)
            {
                if (yOffset >= maxY)
                {
                    goto leave;
                }

                int xOffset = 0;
                for (int wordIndex = 0; wordIndex < line.Length; wordIndex++)
                {
                    string word = line[wordIndex];

                    // If this word is too long to fit, then move on to the next line.
                    if (xOffset + word.Length > maxX)
                    {
                        yOffset++;
                        xOffset = 0;

                        if (yOffset >= maxY)
                        {
                            goto leave;
                        }
                    }

                    bool noSpace = wordIndex == line.Length - 1
                        || (xOffset + word.Length + line[wordIndex + 1].Length > maxX);

                    // Write the word.
                    foreach (char @char in word)
                    {
                        result[yOffset, xOffset++] = new CharInfo(@char, colors);
                    }

                    // If we're supposed to include a space, include it.
                    if (!noSpace)
                    {
                        if (xOffset >= maxX)
                        {
                            xOffset = 0;
                            yOffset++;

                            if (yOffset >= maxY)
                            {
                                goto leave;
                            }
                        }

                        result[yOffset, xOffset++] = new CharInfo(' ', colors);
                    }
                }

                yOffset++;
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
