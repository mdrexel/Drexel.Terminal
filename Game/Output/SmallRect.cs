using System.Runtime.InteropServices;

namespace Game.Output
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct SmallRect
    {
        public readonly short Left;
        public readonly short Top;
        public readonly short Right;
        public readonly short Bottom;

        public SmallRect(
            short left,
            short top,
            short right,
            short bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }
}
