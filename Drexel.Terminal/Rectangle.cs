using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Drexel.Terminal
{
    /// <summary>
    /// Represents a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Rectangle : IEquatable<Rectangle>
    {
        /// <summary>
        /// The leftmost position of the rectangle (inclusive).
        /// </summary>
        public readonly short Left;

        /// <summary>
        /// The topmost position of the rectangle (inclusive).
        /// </summary>
        public readonly short Top;

        /// <summary>
        /// The rightmost position of the rectangle (inclusive).
        /// </summary>
        public readonly short Right;

        /// <summary>
        /// The bottommost position of the rectangle (inclusive).
        /// </summary>
        public readonly short Bottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct, using the supplied coordinates.
        /// </summary>
        /// <param name="topLeft">
        /// The top-left of the rectangle (inclusive).
        /// </param>
        /// <param name="bottomRight">
        /// The bottom-right of the rectangle (inclusive).
        /// </param>
        public Rectangle(Coord topLeft, Coord bottomRight)
            : this(
                  topLeft.X,
                  topLeft.Y,
                  bottomRight.X,
                  bottomRight.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct, using the supplied bounds.
        /// </summary>
        /// <param name="left">
        /// The leftmost position of the rectangle (inclusive).
        /// </param>
        /// <param name="top">
        /// The topmost position of the rectangle (inclusive).
        /// </param>
        /// <param name="right">
        /// The rightmost position of the rectangle (inclusive).
        /// </param>
        /// <param name="bottom">
        /// The bottommost position of the rectangle (inclusive).
        /// </param>
        public Rectangle(
            short left,
            short top,
            short right,
            short bottom)
        {
            (this.Left, this.Right) =
                left > right
                    ? (right, left)
                    : (left, right);
            (this.Top, this.Bottom) =
                top > bottom
                    ? (bottom, top)
                    : (top, bottom);
        }

        /// <summary>
        /// Gets the mathematical height of the rectangle.
        /// </summary>
        public ushort Height => (ushort)(this.Bottom - this.Top);

        /// <summary>
        /// Gets the mathematical width of the rectangle.
        /// </summary>
        public ushort Width => (ushort)(this.Right - this.Left);

        /// <summary>
        /// Gets the horizontal span of the rectangle. This is the mathematical width plus one.
        /// </summary>
        public ushort HorizontalSpan => (ushort)(this.Width + 1);

        /// <summary>
        /// Gets the vertical span of the rectangle. This is the mathematical height plus one.
        /// </summary>
        public ushort VerticalSpan => (ushort)(this.Height + 1);

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same
        /// bounds; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same bounds;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not have the same
        /// bounds; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not have the same bounds;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Decomposes this <see cref="Rectangle"/> into the <see cref="Coord"/>s representing its bounds.
        /// </summary>
        /// <returns>
        /// The <see cref="Coord"/>s representing this <see cref="Rectangle"/>s bounds.
        /// </returns>
        public (Coord TopLeft, Coord BottomRight) Decompose()
        {
            return (new Coord(this.Left, this.Top), new Coord(this.Right, this.Bottom));
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
            if (obj is Rectangle other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this <see cref="Rectangle"/> and the specified <see cref="Rectangle"/>
        /// <paramref name="other"/> have the same bounds.
        /// </summary>
        /// <param name="other">
        /// The <see cref="Rectangle"/> this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance and <paramref name="other"/> have the same bounds; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(Rectangle other)
        {
            return this.Left == other.Left
                && this.Right == other.Right
                && this.Top == other.Top
                && this.Bottom == other.Bottom;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Rectangle"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="Rectangle"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 5923;
            unchecked
            {
                hash = (hash * 31) + this.Left;
                hash = (hash * 31) + this.Top;
                hash = (hash * 31) + this.Right;
                hash = (hash * 31) + this.Bottom;
            }

            return hash;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this <see cref="Rectangle"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this <see cref="Rectangle"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(
                "(",
                this.Left.ToString(CultureInfo.InvariantCulture),
                ", ",
                this.Top.ToString(CultureInfo.InvariantCulture),
                "), (",
                this.Right.ToString(CultureInfo.InvariantCulture),
                ", ",
                this.Bottom.ToString(CultureInfo.InvariantCulture),
                ")");
        }
    }
}
