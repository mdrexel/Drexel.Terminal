using System.Diagnostics;
using Drexel.Terminal.Sink;

namespace Drexel.Terminal.Text
{
    [DebuggerDisplay("[{StartIndexInclusive,nq}, {EndIndexExclusive,nq})")]
    public readonly struct Range
    {
        public readonly ushort StartIndexInclusive;
        public readonly ushort EndIndexExclusive;
        public readonly TerminalColors Colors;
        public readonly int Delay;

        public Range(
            ushort startIndexInclusive,
            ushort endIndexExclusive,
            TerminalColors attributes,
            int delay = 0)
        {
            this.StartIndexInclusive = startIndexInclusive;
            this.EndIndexExclusive = endIndexExclusive;
            this.Colors = attributes;
            this.Delay = delay;
        }
    }
}
