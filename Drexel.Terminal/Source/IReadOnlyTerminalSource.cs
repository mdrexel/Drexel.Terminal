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
        /// Gets an observable that occurs when a terminal key is pressed.
        /// </summary>
        IObservable<TerminalKeyInfo> OnKeyPressed { get; }

        /// <summary>
        /// Gets an observable that occurs when a terminal key is released.
        /// </summary>
        IObservable<TerminalKeyInfo> OnKeyReleased { get; }

        /// <summary>
        /// Gets an observable that occurs when an exit request has been accepted by this terminal.
        /// </summary>
        IObservable<ExitAcceptedEventArgs> OnExitAccepted { get; }

        /// <summary>
        /// Gets the mouse attached to this terminal source.
        /// </summary>
        IMouse Mouse { get; }

        /// <summary>
        /// Returns a task that will complete when an exit has been accepted, and all <see cref="OnExitAccepted"/>
        /// observers observing this instance have completed.
        /// </summary>
        /// <param name="cancellationToken">
        /// Allows the caller to cancel this task.
        /// </param>
        /// <returns>
        /// A task that will complete when an exit has been accepted, and all <see cref="OnExitAccepted"/> observers
        /// observing this instance have completed.
        /// </returns>
        Task DelayUntilExitAccepted(CancellationToken cancellationToken);
    }
}
