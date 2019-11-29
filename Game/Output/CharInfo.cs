using System;
using System.Runtime.InteropServices;

namespace Game.Output
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public readonly struct CharInfo
    {
        private static readonly CharAttributes DefaultAttributes =
            new CharColors(ConsoleColor.White, ConsoleColor.Black);

        [FieldOffset(0)] public readonly CharUnion Char;
        [FieldOffset(2)] public readonly CharAttributes Attributes;

        public CharInfo(CharUnion @char)
        {
            this.Char = @char;
            this.Attributes = DefaultAttributes;
        }

        public CharInfo(CharUnion @char, CharAttributes attributes)
        {
            this.Char = @char;
            this.Attributes = attributes;
        }

        public override string ToString()
        {
            return $"{this.Char.ToString()}";
        }
    }
}
