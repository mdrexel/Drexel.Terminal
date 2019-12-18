using System;

namespace Game.Output
{
    public sealed class MouseMoveEventArgs : EventArgs
    {
        public MouseMoveEventArgs(Coord previousPosition, Coord currentPosition)
        {
            this.PreviousPosition = previousPosition;
            this.CurrentPosition = currentPosition;
        }

        public Coord PreviousPosition { get; }

        public Coord CurrentPosition { get; }
    }
}
