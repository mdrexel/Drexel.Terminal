using Drexel.Terminal.Sink;
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
        /// Reads the next line of characters from this terminal source. If <paramref name="echo"/> is not
        /// <see langword="null"/>, the characters read from this source will be written back to the supplied sink. If
        /// <paramref name="exclusive"/> is <see langword="true"/>, any key-related observables on this terminal source
        /// (ex. <see cref="IReadOnlyTerminalSource.OnKeyPressed"/>) will be suppressed.
        /// </summary>
        /// <param name="echo">
        /// The terminal sink to echo characters to, and the width of the terminal sink.
        /// </param>
        /// <param name="exclusive">
        /// Indicates whether key-related observables on this terminal source should be suppressed.
        /// </param>
        /// <returns>
        /// The next line of characters from this terminal source.
        /// </returns>
        public Task<string> ReadLineAsync((ITerminalSink Sink, ushort Width)? echo = null, bool exclusive = true);

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
