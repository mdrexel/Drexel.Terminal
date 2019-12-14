using System.Diagnostics;

namespace Game.Output
{
    [DebuggerDisplay("[{StartIndexInclusive,nq}, {EndIndexExclusive,nq})")]
    public readonly struct Range
    {
        public readonly ushort StartIndexInclusive;
        public readonly ushort EndIndexExclusive;
        public readonly CharAttributes Attributes;
        public readonly int Delay;

        public Range(
            ushort startIndexInclusive,
            ushort endIndexExclusive,
            CharAttributes attributes,
            int delay = 0)
        {
            this.StartIndexInclusive = startIndexInclusive;
            this.EndIndexExclusive = endIndexExclusive;
            this.Attributes = attributes;
            this.Delay = delay;
        }
    }
}
