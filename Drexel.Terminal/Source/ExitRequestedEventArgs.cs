using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents an event which is associated with a request to "exit". The request can be denied by setting the
    /// <see cref="Allow"/> property to <see langword="false"/> on an instance of this class.
    /// </summary>
    public sealed class ExitRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitRequestedEventArgs"/> class.
        /// </summary>
        public ExitRequestedEventArgs()
        {
            this.Allow = true;
        }

        /// <summary>
        /// Gets or sets whether this exit request should be allowed. To cancel this exit request, set this property to
        /// <see langword="false"/>.
        /// </summary>
        public bool Allow { get; set; }
    }
}
