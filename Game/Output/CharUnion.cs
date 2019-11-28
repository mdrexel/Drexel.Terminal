using System.Runtime.InteropServices;

namespace Game.Output
{

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public readonly struct CharUnion
    {
        [FieldOffset(0)] public readonly char UnicodeChar;
        [FieldOffset(0)] public readonly byte AsciiChar;

        public CharUnion(char unicodeChar)
        {
            this.UnicodeChar = unicodeChar;

            // TODO: I'm only doing this because I'm too stupid to make unicode mode actually work.
            this.AsciiChar = (byte)unicodeChar;
        }

        public CharUnion(byte asciiChar)
        {
            this.AsciiChar = asciiChar;
            this.UnicodeChar = default;
        }

        public static implicit operator CharUnion(char @char)
        {
            return new CharUnion(@char);
        }
    }
}
