using System.Runtime.InteropServices;

namespace Drexel.Terminal.Win32
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public readonly struct ConsoleCharUnion
    {
        [FieldOffset(0)] public readonly char UnicodeChar;
        [FieldOffset(0)] public readonly byte AsciiChar;

        public ConsoleCharUnion(char unicodeChar)
        {
            this.UnicodeChar = unicodeChar;

            // TODO: I'm only doing this because I'm too stupid to make unicode mode actually work.
            this.AsciiChar = (byte)unicodeChar;
        }

        public ConsoleCharUnion(byte asciiChar)
        {
            this.AsciiChar = asciiChar;
            this.UnicodeChar = default;
        }

        public static implicit operator ConsoleCharUnion(char @char)
        {
            return new ConsoleCharUnion(@char);
        }

        public override string ToString()
        {
            return new string(this.UnicodeChar, 1);
        }
    }
}
