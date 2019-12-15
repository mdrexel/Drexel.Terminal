using System.Collections;
using System.Collections.Generic;

namespace Game.Output
{
    public sealed class Ranges : IReadOnlyList<Range>
    {
        private readonly IReadOnlyList<Range> ranges;

        public Ranges(IReadOnlyList<Range> ranges)
        {
            this.ranges = ranges;
        }

        public Range this[int index] => this.ranges[index];

        public int Count => this.ranges.Count;

        public Range GetRangeByIndex(ushort index)
        {
            foreach (Range range in this.ranges)
            {
                if (index >= range.StartIndexInclusive && index < range.EndIndexExclusive)
                {
                    return range;
                }
            }

            throw new KeyNotFoundException("No range containing the specified index exists.");
        }

        public Range GetNextRange(Range range)
        {
            return this.GetRangeByIndex(range.EndIndexExclusive);
        }

        public IEnumerator<Range> GetEnumerator() => this.ranges.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.ranges.GetEnumerator();
    }
}
