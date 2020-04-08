using System;
using Drexel.Terminal.Sink;

namespace Drexel.Terminal.Primitives
{
    /// <summary>
    /// Represents a line.
    /// </summary>
    public sealed class Line
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="topLeft">
        /// The inclusive top-left coordinate from which the line should start being drawn.
        /// </param>
        /// <param name="bottomRight">
        /// The inclusive bottom-right coordinate to which the line should be drawn.
        /// </param>
        /// <param name="pattern">
        /// The pattern to use when drawing the line.
        /// </param>
        public Line(
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
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// <param name="left">
        /// The inclusive leftmost position from which the line should start being drawn.
        /// </param>
        /// <param name="top">
        /// The inclusive topmost position from which the line should start being drawn.
        /// </param>
        /// <param name="right">
        /// The inclusive rightmost position to which the line should be drawn.
        /// </param>
        /// <param name="bottom">
        /// The inclusive bottommost position to which the line should be drawn.
        /// </param>
        /// <param name="pattern">
        /// The pattern to use when drawing the line.
        /// </param>
        public Line(
            short left,
            short top,
            short right,
            short bottom,
            CharInfo[,] pattern)
        {
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

            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        /// <summary>
        /// Gets the inclusive top-left coordinate of this line.
        /// </summary>
        public Coord TopLeft { get; }

        /// <summary>
        /// Gets the inclusive bottom-right coordinate of this line.
        /// </summary>
        public Coord BottomRight { get; }

        /// <summary>
        /// Gets the pattern used when drawing this line.
        /// </summary>
        public CharInfo[,] Pattern { get; }
    }
}
