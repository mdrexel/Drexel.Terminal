using System;

namespace Game.Output
{
    public sealed class MouseClickEventArgs : EventArgs
    {
        public MouseClickEventArgs(Coord position, bool buttonDown)
        {
            this.Position = position;
            this.ButtonDown = buttonDown;
        }

        public Coord Position { get; }

        public bool ButtonDown { get; }
    }
}
