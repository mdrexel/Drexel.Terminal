using System;

namespace Game.Output.Layout
{
    public sealed class RegionChangedEventArgs : EventArgs
    {
        public RegionChangedEventArgs(
            IReadOnlyRegion previousRegion,
            IReadOnlyRegion currentRegion,
            RegionChangeType changeType)
        {
            this.PreviousRegion = previousRegion;
            this.CurrentRegion = currentRegion;
            this.ChangeType = changeType;
        }

        public IReadOnlyRegion PreviousRegion { get; }

        public IReadOnlyRegion CurrentRegion { get; }

        public RegionChangeType ChangeType { get; }
    }
}
