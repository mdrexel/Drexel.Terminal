using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents an event which is associated with an accepted "exit" request. The meaning of "exit" is left to the
    /// client. An accepted exit request cannot be canceled.
    /// </summary>
    public sealed class ExitAcceptedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitAcceptedEventArgs"/> class.
        /// </summary>
        public ExitAcceptedEventArgs()
        {
        }
    }
}
