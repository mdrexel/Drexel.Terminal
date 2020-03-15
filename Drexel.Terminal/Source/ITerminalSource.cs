using System;
using System.Threading.Tasks;

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
        /// Requests that an exit take place. The meaning of "exit" is left to the client. The returned task completes
        /// once the requested exit has either completed or been denied. The result of the task indicates whether
        /// the exit occurred: <see langword="true"/> if the exit was accepted, or <see langword="false"/> if the exit
        /// was denied.
        /// </summary>
        /// <returns>
        /// A task that completes once the requested exit has either completed or been denied. The result of the task
        /// indicates whether the exit occurred: <see langword="true"/> if the exit was accepted, or
        /// <see langword="false"/> if the exit was denied.
        /// </returns>
        Task<bool> RequestExitAsync();
    }
}
