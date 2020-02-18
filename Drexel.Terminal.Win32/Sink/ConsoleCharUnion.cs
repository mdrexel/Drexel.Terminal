using System;
using System.Runtime.InteropServices;

namespace Drexel.Terminal.Sink.Win32
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public readonly struct ConsoleCharUnion : IEquatable<ConsoleCharUnion>
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

        public static bool operator ==(ConsoleCharUnion left, ConsoleCharUnion right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConsoleCharUnion left, ConsoleCharUnion right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is ConsoleCharUnion other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public bool Equals(ConsoleCharUnion other)
        {
            return this.AsciiChar == other.AsciiChar
                && this.UnicodeChar == other.UnicodeChar;
        }

        public override int GetHashCode()
        {
            int hash = 91237;
            unchecked
            {
                hash = (hash * 31) + this.AsciiChar.GetHashCode();
                hash = (hash * 31) + this.UnicodeChar.GetHashCode();
            }

            return hash;
        }

        public override string ToString()
        {
            return new string(this.UnicodeChar, 1);
        }
    }
}
