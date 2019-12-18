using System;

namespace Game.Output
{
    public sealed class MouseWheelEventArgs : EventArgs
    {
        public MouseWheelEventArgs(Coord position, bool down)
        {
            this.Position = position;
            this.Down = down;
        }

        public Coord Position { get; }

        public bool Down { get; }
    }
}
