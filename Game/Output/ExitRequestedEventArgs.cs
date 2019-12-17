using System;

namespace Game.Output
{
    public sealed class ExitRequestedEventArgs : EventArgs
    {
        public ExitRequestedEventArgs()
        {
            this.Allow = true;
        }

        public bool Allow { get; set; }
    }
}
