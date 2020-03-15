using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents a mouse button.
    /// </summary>
    public interface IMouseButton
    {
        /// <summary>
        /// Gets a value indicating whether the button is currently held down.
        /// </summary>
        bool Down { get; }

        /// <summary>
        /// Gets an observable that occurs when the button is raised or lowered.
        /// </summary>
        IObservable<MouseClickEventArgs>? OnButton { get; }
    }
}
