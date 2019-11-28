using System;

namespace Game.Output.Layout
{
    public sealed class RegionChangedEventArgs : EventArgs
    {
        public RegionChangedEventArgs(IReadOnlyRegion oldRegion, IReadOnlyRegion newRegion)
        {
            this.OldRegion = oldRegion;
            this.NewRegion = newRegion;
        }

        public IReadOnlyRegion OldRegion { get; }

        public IReadOnlyRegion NewRegion { get; }
    }
}
