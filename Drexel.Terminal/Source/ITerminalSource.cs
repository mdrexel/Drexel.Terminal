using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents a source that can receive terminal input.
    /// </summary>
    public interface ITerminalSource : IReadOnlyTerminalSource
    {
        /// <summary>
        /// Gets an observable that occurs when an exit has been requested by this terminal. The meaning of "exit" is
        /// left to the client.
        /// </summary>
        IObservable<ExitRequestedEventArgs> OnExitRequested { get; }

        /// <summary>
        /// Requests that an exit take place. The meaning of "exit" is left to the client. The returned
        /// <see langword="bool"/> indicates whether the exit has either completed or been denied:
        /// <see langword="true"/> if the exit was accepted, or <see langword="false"/> if the exit
        /// was denied.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the exit was accepted, or <see langword="false"/> if the exit was denied.
        /// </returns>
        bool RequestExit();
    }
}
