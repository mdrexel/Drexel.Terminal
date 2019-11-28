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

        public static Coord operator +(Coord left, Coord right)
        {
            return new Coord((short)(left.X + right.X), (short)(left.Y + right.Y));
        }

        public static Coord operator -(Coord left, Coord right)
        {
            return new Coord((short)(left.X - right.X), (short)(left.Y - right.Y));
        }

        public static Coord operator *(Coord left, Coord right)
        {
            return new Coord((short)(left.X * right.X), (short)(left.Y * right.Y));
        }
    };
}
