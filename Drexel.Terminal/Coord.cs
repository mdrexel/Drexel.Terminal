using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Drexel.Terminal
{
    /// <summary>
    /// Represents a coordinate (a pair of a horizontal position and a vertical position).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Coord : IEquatable<Coord>
    {
        /// <summary>
        /// A coordinate representing a zero offset in both dimensions (0, 0).
        /// </summary>
        public static readonly Coord Zero = new Coord(0, 0);

        /// <summary>
        /// A coordinate representing a one offset in both dimensions (1, 1).
        /// </summary>
        public static readonly Coord OneOffset = new Coord(1, 1);

        /// <summary>
        /// A coordinate representing a one offset in the X dimension (1, 0).
        /// </summary>
        public static readonly Coord OneXOffset = new Coord(1, 0);

        /// <summary>
        /// A coordinate representing a one offset in the Y dimension (0, 1).
        /// </summary>
        public static readonly Coord OneYOffset = new Coord(0, 1);

        /// <summary>
        /// The horizontal position of this coordinate.
        /// </summary>
        public readonly short X;

        /// <summary>
        /// The vertical position of this coordinate.
        /// </summary>
        public readonly short Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Coord"/> struct.
        /// </summary>
        /// <param name="X">
        /// The horizontal position of this <see cref="Coord"/>.
        /// </param>
        /// <param name="Y">
        /// The vertical position of this <see cref="Coord"/>.
        /// </param>
        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Adds the specified <see cref="Coord"/>s <paramref name="left"/> and <paramref name="right"/> as if they
        /// were vectors.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Coord"/> on the left side of the summation.
        /// </param>
        /// <param name="right">
        /// The <see cref="Coord"/> on the right side of the summation.
        /// </param>
        /// <returns>
        /// The <see cref="Coord"/> resulting from adding the two <see cref="Coord"/> <paramref name="left"/> and
        /// <paramref name="right"/> together as if they were vectors.
        /// </returns>
        public static Coord operator +(Coord left, Coord right)
        {
            checked
            {
                return new Coord((short)(left.X + right.X), (short)(left.Y + right.Y));
            }
        }

        /// <summary>
        /// Subtracts the specified <see cref="Coord"/>s <paramref name="left"/> and <paramref name="right"/> as if
        /// they were vectors.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Coord"/> on the left side of the subtraction.
        /// </param>
        /// <param name="right">
        /// The <see cref="Coord"/> on the right side of the subtraction.
        /// </param>
        /// <returns>
        /// The <see cref="Coord"/> resulting from subtracting the two <see cref="Coord"/>s <paramref name="left"/> and
        /// <paramref name="right"/> as if they were vectors.
        /// </returns>
        public static Coord operator -(Coord left, Coord right)
        {
            checked
            {
                return new Coord((short)(left.X - right.X), (short)(left.Y - right.Y));
            }
        }

        /// <summary>
        /// Multiplies the specified <see cref="Coord"/>s <paramref name="left"/> and <paramref name="right"/> as if
        /// they were vectors.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Coord"/> on the left side of the multiplication.
        /// </param>
        /// <param name="right">
        /// The <see cref="Coord"/> on the right side of the multiplication.
        /// </param>
        /// <returns>
        /// The <see cref="Coord"/> resulting from multiplying the two <see cref="Coord"/>s <paramref name="left"/> and
        /// <paramref name="right"/> as if they were vectors.
        /// </returns>
        public static Coord operator *(Coord left, Coord right)
        {
            checked
            {
                return new Coord((short)(left.X * right.X), (short)(left.Y * right.Y));
            }
        }

        /// <summary>
        /// Multiplies the <see cref="Coord"/> <paramref name="right"/> by the specified scalar <paramref name="left"/>
        /// as if the <see cref="Coord"/> <paramref name="right"/> was a vector.
        /// </summary>
        /// <param name="left">
        /// The scalar value.
        /// </param>
        /// <param name="right">
        /// The <see cref="Coord"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Coord"/> resulting from the multiplication of the scalar <paramref name="left"/> and the
        /// <see cref="Coord"/> <paramref name="right"/> as if the <see cref="Coord"/> was a vector.
        /// </returns>
        public static Coord operator *(short left, Coord right)
        {
            checked
            {
                return new Coord((short)(left * right.X), (short)(left * right.Y));
            }
        }

        /// <summary>
        /// Multiplies the <see cref="Coord"/> <paramref name="left"/> by the specified scalar
        /// <paramref name="right"/> as if the <see cref="Coord"/> <paramref name="left"/> was a vector.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Coord"/>.
        /// </param>
        /// <param name="right">
        /// The scalar value.
        /// </param>
        /// <returns>
        /// The <see cref="Coord"/> resulting from the multiplication of the <see cref="Coord"/>
        /// <paramref name="left"/> and the scalar <paramref name="right"/> as if the <see cref="Coord"/> was a vector.
        /// </returns>
        public static Coord operator *(Coord left, short right)
        {
            checked
            {
                return new Coord((short)(left.X * right), (short)(left.Y * right));
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same
        /// horizontal and vertical position; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Coord"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="Coord"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same horizontal and
        /// vertical position; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Coord left, Coord right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not have the same
        /// horizontal and vertical position; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Coord"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="Coord"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not have the same
        /// horizontal and vertical position; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Coord left, Coord right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether this instance and the specified <see cref="object"/> <paramref name="obj"/> are equal.
        /// </summary>
        /// <param name="obj">
        /// The object this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance is equal to the specified <paramref name="obj"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Coord other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this <see cref="Coord"/> and the specified <see cref="Coord"/> <paramref name="other"/>
        /// have the same horizontal and vertical position.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Coord"/> this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance and <paramref name="other"/> have the same horizontal and vertical
        /// position; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(Coord other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        /// <summary>
        /// Returns a <see cref="Coord"/> with the <see cref="X"/> and <see cref="Y"/> position of this
        /// <see cref="Coord"/> flipped.
        /// </summary>
        /// <returns>
        /// A <see cref="Coord"/> with the X and Y position of this <see cref="Coord"/> flipped.
        /// </returns>
        public Coord Invert()
        {
            return new Coord(this.Y, this.X);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Coord"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="Coord"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 991;
            hash = (hash * 31) + this.X;
            hash = (hash * 31) + this.Y;

            return hash;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this <see cref="Coord"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this <see cref="Coord"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(
                "(",
                this.X.ToString(CultureInfo.InvariantCulture),
                ", ",
                this.Y.ToString(CultureInfo.InvariantCulture),
                ")");
        }
    };
}
