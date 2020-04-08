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

        public IReadOnlyList<IReadOnlyRegion> ImpactedRegions { get; }
    }
}
