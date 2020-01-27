using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents a source that can receive terminal input.
    /// </summary>
    public interface ITerminalSource : IReadOnlyTerminalSource
    {
        /// <summary>
        /// Occurs when an exit has been requested by this terminal.
        /// </summary>

        event EventHandler<ExitRequestedEventArgs>? OnExitRequested;
    }
}
