using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents an event associated with a mouse movement.
    /// </summary>
    public sealed class MouseMoveEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseMoveEventArgs"/> class.
        /// </summary>
        /// <param name="previousPosition">
        /// The previous position (where the mouse was) in the input-space.
        /// </param>
        /// <param name="currentPosition">
        /// The current position (where the mouse is) in the input-space.
        /// </param>
        public MouseMoveEventArgs(Coord previousPosition, Coord currentPosition)
        {
            this.PreviousPosition = previousPosition;
            this.CurrentPosition = currentPosition;
        }

        /// <summary>
        /// Gets the previous position (where the mouse was) in the input-space.
        /// </summary>
        public Coord PreviousPosition { get; }

        /// <summary>
        /// Gets the current position (where the mouse is) in the input-space.
        /// </summary>
        public Coord CurrentPosition { get; }
    }
}
