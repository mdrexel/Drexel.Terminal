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
            out bool leadingOverflow,
            out bool trailingOverflow,
            CharColors? backgroundFill = null,
            ushort preceedingLinesSkipped = 0) =>
            value.ToCharInfo(
                colors,
                out leadingOverflow,
                out trailingOverflow,
                backgroundFill,
                preceedingLinesSkipped,
                region.Width,
                region.Height);

        public static CharInfo[,] ToCharInfo(
            this string value,
            CharColors colors,
            out bool leadingOverflow,
            out bool trailingOverflow,
            CharColors? backgroundFill = null,
            ushort preceedingLinesSkipped = 0,
            short? maximumWidth = null,
            short? maximumHeight = null)
        {
            leadingOverflow = preceedingLinesSkipped > 0;
            trailingOverflow = false;

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

            short yOffset = 0;
            CharInfo spaceInfo = new CharInfo(' ', colors);
            foreach (string[] line in values)
            {
                if (yOffset >= maxY)
                {
                    goto leave;
                }

                short xOffset = 0;
                for (int wordIndex = 0; wordIndex < line.Length; wordIndex++)
                {
                    string word = line[wordIndex];

                    // If this word is too long to fit, then move on to the next line.
                    if (xOffset + word.Length > maxX)
                    {
                        yOffset++;
                        xOffset = 0;

                        if (preceedingLinesSkipped < yOffset && yOffset >= maxY)
                        {
                            goto leave;
                        }
                    }

                    bool noSpace = wordIndex == line.Length - 1
                        || (xOffset + word.Length + line[wordIndex + 1].Length > maxX);

                    // Write the word.
                    if (preceedingLinesSkipped < yOffset)
                    {
                        foreach (char @char in word)
                        {
                            result[yOffset, xOffset++] = new CharInfo(@char, colors);
                        }
                    }

                    // If we're supposed to include a space, include it.
                    if (!noSpace)
                    {
                        if (maxX < xOffset)
                        {
                            xOffset = 0;
                            yOffset++;

                            if (preceedingLinesSkipped < yOffset && maxY < yOffset )
                            {
                                goto leave;
                            }
                        }

                        if (preceedingLinesSkipped < yOffset)
                        {
                            result[yOffset, xOffset++] = spaceInfo;
                        }
                    }
                }

                yOffset++;
            }

            goto exit;

            leave: trailingOverflow = true;
            exit: return result;
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
