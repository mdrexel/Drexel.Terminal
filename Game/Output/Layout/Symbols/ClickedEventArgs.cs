using System;

namespace Game.Output.Layout.Symbols
{
    public sealed class ClickedEventArgs : EventArgs
    {
        public ClickedEventArgs(Coord coord)
        {
            this.Coord = coord;
        }

        public Coord Coord { get; }
    }
}
