using System;
using System.Linq;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Text : IDrawable
    {
        private static short NewLineLength = (short)Environment.NewLine.Length;

        private readonly FormattedString content;
        private readonly CharColors? backgroundFill;

        private Rectangle rectangle;
        private ushort preceedingLinesSkipped;

        public Text(
            FormattedString content,
            IReadOnlyRegion constrainedTo,
            CharColors? backgroundFill = null)
            : this(content, new Region(constrainedTo), backgroundFill)
        {
        }

        public Text(
            FormattedString content,
            Region region,
            CharColors? backgroundFill = null)
        {
            this.content = content;
            this.Region = region;
            this.backgroundFill = backgroundFill;

            region.OnChanged +=
                (obj, e) =>
                {
                    // Only need to recalculate contents on resize
                    if (e.ChangeType == RegionChangeType.Resize || e.ChangeType == RegionChangeType.MoveAndResize)
                    {
                        this.Recalculate();
                    }
                };

            this.Recalculate();
        }

        public static Text Empty { get; } =
            new Text(
                FormattedString.Empty,
                new Region(
                    new Coord(0, 0),
                    new Coord(0, 0)));

        public Region Region { get; }

        public ushort TotalLines { get; private set; }

        public ushort PreceedingLinesSkipped
        {
            get => this.preceedingLinesSkipped;
            set
            {
                this.preceedingLinesSkipped = value;
                this.Recalculate();
            }
        }

        public void Draw(ISink sink)
        {
            this.rectangle.Draw(sink);
        }

        private void Recalculate()
        {
            this.TotalLines = 0;
            this.preceedingLinesSkipped = 0;
            this.rectangle = new Rectangle(new Coord(0, 0), new CharDelay[0, 0]);
            throw new NotImplementedException("TODO need to do line breaks n such");
        }
    }
}
