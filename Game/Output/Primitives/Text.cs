using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Text : IDrawable
    {
        private readonly string content;
        private readonly IReadOnlyRegion constrainedTo;
        private readonly CharColors colors;
        private readonly CharColors? backgroundFill;

        private Rectangle rectangle;
        private bool leadingOverflow;
        private bool trailingOverflow;
        private ushort preceedingLinesSkipped;

        public Text(
            string content,
            CharColors colors,
            IReadOnlyRegion constrainedTo,
            CharColors? backgroundFill = null)
        {
            this.content = content;
            this.colors = colors;
            this.backgroundFill = backgroundFill;
            this.constrainedTo = constrainedTo;
            this.preceedingLinesSkipped = 0;

            constrainedTo.OnChanged += (obj, e) => this.Calculate();

            this.Calculate();
        }

        public ushort PreceedingLinesSkipped
        {
            get => this.preceedingLinesSkipped;
            set
            {
                this.preceedingLinesSkipped = value;
                this.Calculate();
            }
        }

        public bool LeadingOverflow => this.leadingOverflow;

        public bool TrailingOverflow => this.trailingOverflow;

        public void Draw(ISink sink)
        {
            this.rectangle.Draw(sink);
        }

        private void Calculate()
        {
            this.rectangle =
                new Rectangle(
                    this.constrainedTo.TopLeft,
                    this.content.ToCharInfo(
                        this.colors,
                        this.constrainedTo,
                        out this.leadingOverflow,
                        out this.trailingOverflow,
                        this.backgroundFill,
                        this.preceedingLinesSkipped));
        }
    }
}
