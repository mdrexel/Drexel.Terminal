using System;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents a source that can receive terminal input.
    /// </summary>
    public interface IReadOnlyTerminalSource
    {
        /// <summary>
        /// Gets a value indicating whether the left mouse button is currently held down.
        /// </summary>
        bool LeftMouseDown { get; }

        /// <summary>
        /// Gets a value indicating whether the right mouse button is currently held down.
        /// </summary>
        bool RightMouseDown { get; }

        /// <summary>
        /// Occurs when the left mouse button is raised or lowered.
        /// </summary>
        event EventHandler<MouseClickEventArgs>? OnLeftMouse;

        /// <summary>
        /// Occurs when the right mouse button is raised or lowered.
        /// </summary>
        event EventHandler<MouseClickEventArgs>? OnRightMouse;

        /// <summary>
        /// Occurs when the mouse moves.
        /// </summary>
        event EventHandler<MouseMoveEventArgs>? OnMouseMove;

        /// <summary>
        /// Occurs when the mouse wheel is scrolled.
        /// </summary>
        event EventHandler<MouseWheelEventArgs>? OnMouseWheel;

        /// <summary>
        /// Occurs when a terminal key is pressed.
        /// </summary>
        event EventHandler<TerminalKeyInfo>? OnKeyPressed;

        /// <summary>
        /// Occurs when an exit request has been accepted by this terminal.
        /// </summary>
        event EventHandler<ExitAcceptedEventArgs>? OnExitAccepted;

        /// <summary>
        /// Returns a task that will complete when an exit has been accepted, and all <see cref="OnExitAccepted"/>
        /// listeners listening on this instance have completed.
        /// </summary>
        /// <param name="cancellationToken">
        /// Allows the caller to cancel this task.
        /// </param>
        /// <returns>
        /// A task that will complete when an exit has been accepted, and all <see cref="OnExitAccepted"/> listeners
        /// listening on this instance have completed.
        /// </returns>
        Task DelayUntilExitAccepted(CancellationToken cancellationToken);
    }
}
