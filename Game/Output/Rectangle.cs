using System;

namespace Game.Output
{
    public readonly struct Rectangle : IEquatable<Rectangle>
    {
        public readonly Coord TopLeft;
        public readonly Coord BottomRight;

        public Rectangle(Coord topLeft, Coord bottomRight)
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rectangle other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public bool Equals(Rectangle other)
        {
            return this.TopLeft == other.TopLeft
                && this.BottomRight == other.BottomRight;
        }

        public override int GetHashCode()
        {
            int hash = 5923;
            unchecked
            {
                hash = (hash * 31) + this.TopLeft.GetHashCode();
                hash = (hash * 31) + this.BottomRight.GetHashCode();
            }

            return hash;
        }
    }
}
