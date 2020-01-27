using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents an event associated with the mouse wheel being scrolled.
    /// </summary>
    public sealed class MouseWheelEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseWheelEventArgs"/> class.
        /// </summary>
        /// <param name="position">
        /// The <see cref="Coord"/> in the input-space at which the event occurred.
        /// </param>
        /// <param name="direction">
        /// The <see cref="MouseWheelDirection"/> the mouse was scrolled in.
        /// </param>
        public MouseWheelEventArgs(Coord position, MouseWheelDirection direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the <see cref="Coord"/> in the input-space at which the event occurred.
        /// </summary>
        public Coord Position { get; }

        /// <summary>
        /// Gets the <see cref="MouseWheelDirection"/> the mouse was scrolled in.
        /// </summary>
        public MouseWheelDirection Direction { get; }
    }
}
