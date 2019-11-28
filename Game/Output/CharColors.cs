using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Game.Output
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct CharColors
    {
        private static readonly IReadOnlyList<ConsoleColor> Colors = Enum
            .GetValues(typeof(ConsoleColor))
            .Cast<ConsoleColor>()
            .ToArray();

        private readonly CharAttributes attributes;

        public CharColors(ConsoleColor fg, ConsoleColor bg)
        {
            this.attributes = (CharAttributes)(((ushort)bg << 4) | (ushort)fg);
        }

        public static implicit operator CharAttributes(CharColors color)
        {
            return color.attributes;
        }

        public static CharColors Standard { get; } = new CharColors(ConsoleColor.White, ConsoleColor.Black);

        public static CharColors GetRandom(Random random)
        {
            ConsoleColor fg = Colors[random.Next(0, Colors.Count)];
            ConsoleColor bg = Colors[random.Next(0, Colors.Count)];

            return new CharColors(fg, bg);
        }
    }
}
