using System.Runtime.InteropServices;

namespace Drexel.Terminal.Win32
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    internal readonly struct ConsoleCharInfo
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

        public ConsoleCharInfo GetInvertedColor()
        {
            return new ConsoleCharInfo(this.Char, this.Attributes ^ ConsoleCharAttributes.COMMON_LVB_REVERSE_VIDEO);
        }

        public override string ToString()
        {
            return $"{this.Char.ToString()}";
        }
    }
}
