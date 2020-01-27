using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents an event associated with a mouse click.
    /// </summary>
    public sealed class MouseClickEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseClickEventArgs"/> class.
        /// </summary>
        /// <param name="position">
        /// The <see cref="Coord"/> in the input-space at which the event occurred.
        /// </param>
        /// <param name="buttonDown">
        /// <see langword="true"/> if the mouse button was lowered as part of this event. <see langword="false"/> if
        /// the mouse button was raised as part of this event.
        /// </param>
        public MouseClickEventArgs(Coord position, bool buttonDown)
        {
            this.Position = position;
            this.ButtonDown = buttonDown;
        }

        /// <summary>
        /// Gets the <see cref="Coord"/> in the input-space at which the event occurred.
        /// </summary>
        public Coord Position { get; }

        /// <summary>
        /// Gets a value indicating whether the mouse was raised or lowered as part of this event.
        /// <see langword="true"/> if the mouse button was lowered as part of this event. <see langword="false"/> if
        /// the mouse button was raised as part of this event.
        /// </summary>
        public bool ButtonDown { get; }
    }
}
