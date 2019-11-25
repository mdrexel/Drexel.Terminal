using System.Runtime.InteropServices;

namespace Game.Output
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Coord
    {
        public readonly short X;
        public readonly short Y;

        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    };
}
