using System;
using Drexel.Terminal.Sink;

namespace Drexel.Terminal.Primitives
{
    /// <summary>
    /// Represents a region which should be filled with a pattern.
    /// </summary>
    public sealed class Fill
    {
        private CharInfo[,]? cached;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fill"/> class.
        /// </summary>
        /// <param name="topLeft">
        /// The inclusive top-left coordinate of the region to fill.
        /// </param>
        /// <param name="bottomRight">
        /// The inclusive bottom-right coordinate of the readion to fill.
        /// </param>
        /// <param name="pattern">
        /// The pattern to use when drawing the region.
        /// </param>
        public Fill(
            Coord topLeft,
            Coord bottomRight,
            CharInfo[,] pattern)
            : this(
                  topLeft.X,
                  topLeft.Y,
                  bottomRight.X,
                  bottomRight.Y,
                  pattern)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fill"/> class.
        /// </summary>
        /// <param name="left">
        /// The inclusive leftmost position of the region to fill.
        /// </param>
        /// <param name="top">
        /// The inclusive topmost position of the region to fill.
        /// </param>
        /// <param name="right">
        /// The inclusive rightmost position of the region to fill.
        /// </param>
        /// <param name="bottom">
        /// The inclusive bottommost position of the region to fill.
        /// </param>
        /// <param name="pattern">
        /// The pattern to use when drawing the region.
        /// </param>
        public Fill(
            short left,
            short top,
            short right,
            short bottom,
            CharInfo[,] pattern)
        {
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (left > right)
            {
                (left, right) = (right, left);
            }

            if (top > bottom)
            {
                (top, bottom) = (bottom, top);
            }

            this.TopLeft = new Coord(left, top);
            this.BottomRight = new Coord(right, bottom);

            this.Pattern = pattern;

            this.cached = null;
        }

        /// <summary>
        /// Gets the inclusive top-left coordinate of this fill.
        /// </summary>
        public Coord TopLeft { get; }

        /// <summary>
        /// Gets the inclusive bottom-right coordinate of this fill.
        /// </summary>
        public Coord BottomRight { get; }

        /// <summary>
        /// Gets the pattern used when drawing this fill.
        /// </summary>
        public CharInfo[,] Pattern { get; }

        /// <summary>
        /// Gets the full sized character array for drawing this fill.
        /// </summary>
        /// <returns>
        /// The full sized character array for drawing this fill.
        /// </returns>
        public CharInfo[,] GetFullSize()
        {
            if (this.cached is null)
            {
                this.cached = this.Pattern.Repeat(this.BottomRight - this.TopLeft + Coord.OneOffset);
            }

            return this.cached!;
        }
    }
}
