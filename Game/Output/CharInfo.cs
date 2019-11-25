using System.Runtime.InteropServices;

namespace Game.Output
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public readonly struct CharInfo
    {
        [FieldOffset(0)] public readonly CharUnion Char;
        [FieldOffset(2)] public readonly CharAttributes Attributes;

        public CharInfo(CharUnion @char, CharAttributes attributes)
        {
            this.Char = @char;
            this.Attributes = attributes;
        }

        public static int Size { get; } = Marshal.SizeOf<CharInfo>();
    }
}
