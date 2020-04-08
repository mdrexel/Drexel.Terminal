using System;
using System.Collections.Generic;

namespace Drexel.Terminal.Layout.Layouts
{
    public sealed class SymbolRedrawEventArgs
    {
        public SymbolRedrawEventArgs(IReadOnlyList<IReadOnlyRegion> impactedRegions)
        {
            this.ImpactedRegions = impactedRegions ?? throw new ArgumentNullException(nameof(impactedRegions));
        }

        public SymbolRedrawEventArgs(params IReadOnlyRegion[] impectedRegionParams)
            : this(impactedRegions: impectedRegionParams)
        {
        }

        public IReadOnlyList<IReadOnlyRegion> ImpactedRegions { get; }
    }
}
