using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents an attached mouse.
    /// </summary>
    public interface IMouse
    {
        /// <summary>
        /// Gets the current mouse position in the input-space.
        /// </summary>
        Coord Position { get; }

        /// <summary>
        /// Gets an observable that occurs when the mouse moves.
        /// </summary>
        IObservable<MouseMoveEventArgs> OnMouseMove { get; }

        /// <summary>
        /// Gets an observable that occurs when the mouse wheel is scrolled.
        /// </summary>
        IObservable<MouseWheelEventArgs> OnMouseWheel { get; }

        /// <summary>
        /// Gets the left mouse button.
        /// </summary>
        IMouseButton LeftButton { get; }

        /// <summary>
        /// Gets the middle mouse button.
        /// </summary>
        IMouseButton MiddleButton { get; }

        /// <summary>
        /// Gets the right mouse button.
        /// </summary>
        IMouseButton RightButton { get; }

        /// <summary>
        /// Gets button 4. Button 4 (also known as XButton1, or simply the "back button") may not be available on all
        /// systems.
        /// </summary>
        IMouseButton Button4 { get; }

        /// <summary>
        /// Gets button 5. Button 5 (also known as XButton2, or simply the "forward button") may not be available on
        /// all systems.
        /// </summary>
        IMouseButton Button5 { get; }
    }
}
