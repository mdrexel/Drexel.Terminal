using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents a source that can receive terminal input.
    /// </summary>
    public interface ITerminalSource : IReadOnlyTerminalSource
    {
        /// <summary>
        /// Gets an observable that occurs when an exit has been requested by this terminal.
        /// </summary>

        IObservable<ExitRequestedEventArgs>? OnExitRequested { get; }
    }
}
