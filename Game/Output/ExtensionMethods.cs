using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Game.Output
{
    internal static class ExtensionMethods
    {
        private static readonly string[] NewLines = new string[] { Environment.NewLine };

        ////public static CharInfo[,] ToCharInfo(this string value, CharColors colors)
        ////{
        ////    string[] split = value.Split(NewLines, StringSplitOptions.None);
        ////    int largestWidth = 0;
        ////    foreach (string component in split)
        ////    {
        ////        if (component.Length > largestWidth)
        ////        {
        ////            largestWidth = component.Length;
        ////        }
        ////    }

        ////    CharInfo[,] result = new CharInfo[split.Length, largestWidth];
        ////    for (int line = 0; line < split.Length; line++)
        ////    {
        ////        string component = split[line];
        ////        for (int counter = 0; counter < component.Length; counter++)
        ////        {
        ////            result[line, counter] = new CharInfo(component[counter], colors);
        ////        }
        ////    }

        ////    return result;
        ////}

        public static CharInfo[,] ToCharInfo(
            this string value,
            CharColors colors,
            short? maximumWidth = null,
            short? maximumHeight = null)
        {
            string[] lines = value.Split(NewLines, StringSplitOptions.None);
            short maxX = maximumWidth.HasValue ? maximumWidth.Value : (short)lines.Max(x => x.Length);

            string[][] values = lines.Select(x => x.Split(' ')).ToArray();
            short maxY = maximumHeight.HasValue ? maximumHeight.Value : (short)values.Length;

            CharInfo[,] result = new CharInfo[maxY, maxX];
            
            int yOffset = 0;
            foreach (string[] line in values)
            {
                if (yOffset > maxY)
                {
                    goto leave;
                }

                int xOffset = 0;
                for (int wordIndex = 0; wordIndex < line.Length; wordIndex++)
                {
                    string word = line[wordIndex];
                    bool noSpace = wordIndex == line.Length - 1
                        || (line[wordIndex + 1].Length + word.Length + 1 > maxX);

                    // If this word is too long to fit, then move on to the next line.
                    if (xOffset + word.Length > maxX)
                    {
                        yOffset++;
                        xOffset = 0;

                        if (yOffset > maxY)
                        {
                            goto leave;
                        }
                    }

                    // Write the word.
                    foreach (char @char in word)
                    {
                        result[yOffset, xOffset++] = new CharInfo(@char, colors);
                    }

                    // If we're supposed to include a space, include it.
                    if (!noSpace)
                    {
                        result[yOffset, xOffset] = new CharInfo(' ', colors);

                        if (++xOffset > maxX)
                        {
                            xOffset = 0;
                            yOffset++;

                            if (yOffset > maxY)
                            {
                                goto leave;
                            }
                        }
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
