using System;
using System.Runtime.InteropServices;

namespace Drexel.Terminal.Sink.Win32
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    internal readonly struct ConsoleCharInfo : IEquatable<ConsoleCharInfo>
    {
        [FieldOffset(0)] public readonly ConsoleCharUnion Char;
        [FieldOffset(2)] public readonly ConsoleCharAttributes Attributes;

        public ConsoleCharInfo(ConsoleCharUnion @char)
        {
            this.Char = @char;
            this.Attributes = default;
        }

        public ConsoleCharInfo(ConsoleCharUnion @char, ConsoleCharAttributes attributes)
        {
            this.Char = @char;
            this.Attributes = attributes;
        }

        public static bool operator ==(ConsoleCharInfo left, ConsoleCharInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConsoleCharInfo left, ConsoleCharInfo right)
        {
            return !(left == right);
        }

        public ConsoleCharInfo GetInvertedColor()
        {
            return new ConsoleCharInfo(this.Char, this.Attributes ^ ConsoleCharAttributes.COMMON_LVB_REVERSE_VIDEO);
        }

        public override bool Equals(object obj)
        {
            if (obj is ConsoleCharInfo other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public bool Equals(ConsoleCharInfo other)
        {
            return this.Char == other.Char
                && this.Attributes == other.Attributes;
        }

        public override int GetHashCode()
        {
            int hash = 93151;
            unchecked
            {
                hash = (hash * 31) + this.Attributes.GetHashCode();
                hash = (hash * 31) + this.Char.GetHashCode();
            }

            return hash;
        }

        public override string ToString()
        {
            return $"{this.Char.ToString()}";
        }
    }
}
