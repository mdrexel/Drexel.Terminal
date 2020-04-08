using System;
using Drexel.Terminal.Sink;

namespace Drexel.Terminal.Layout.Layouts
{
    public static class SymbolExtensionMethods
    {
        public static void Draw(this Symbol symbol, ITerminalSink sink)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink));
            }

            symbol.Draw(sink, new Rectangle(symbol.Region.TopLeft, symbol.Region.BottomRight));
        }

        public static void Draw(this Symbol symbol, ITerminalSink sink, IReadOnlyRegion region)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink));
            }

            if (region is null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            symbol.Draw(sink, new Rectangle(region.TopLeft, region.BottomRight));
        }
    }
}
