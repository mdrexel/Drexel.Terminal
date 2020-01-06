using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public class Label : IDrawable
    {
        private readonly ContentValue content;
        private readonly Alignments alignments;
        private readonly CharColors? backgroundFill;

        private CharDelay[,]? delayedContent;
        private CharInfo[,]? undelayedContent;

        public Label(
            FormattedString content,
            Alignments alignments,
            CharColors? backgroundFill = null)
        {
            this.content = new ContentValue(content);
            this.alignments = alignments;
            this.backgroundFill = backgroundFill;

            this.Ctor(
                new Region(
                    new Coord(0, 0),
                    new Coord(this.content.Width, this.content.Height)));
        }

        public Label(
            IReadOnlyRegion region,
            Alignments alignments,
            FormattedString content,
            CharColors? backgroundFill = null)
        {
            this.content = new ContentValue(content);
            this.alignments = alignments;
            this.backgroundFill = backgroundFill;

            this.Ctor(region);
        }

        public static Label Empty { get; } = new Label(FormattedString.Empty, Alignments.Default);

        public IMoveOnlyRegion Region { get; private set; }

        internal bool HasDelayedContent => this.content.Content.ContainsDelays;

        public void Draw(ISink sink)
        {
            if (this.HasDelayedContent)
            {
                sink.WriteRegion(this.delayedContent!, this.Region.TopLeft);
            }
            else
            {
                sink.WriteRegion(this.undelayedContent!, this.Region.TopLeft);
            }
        }

        public void Draw(ISink sink, Rectangle window)
        {
            if (this.HasDelayedContent)
            {
                sink.WriteRegion(
                    this.delayedContent!,
                    this.Region.TopLeft,
                    window);
            }
            else
            {
                sink.WriteRegion(
                    this.undelayedContent!,
                    this.Region.TopLeft,
                    window);
            }
        }

        public void InvertColor()
        {
            if (this.HasDelayedContent)
            {
                for (int y = 0; y < this.delayedContent!.GetHeight(); y++)
                {
                    for (int x = 0; x < this.delayedContent!.GetWidth(); x++)
                    {
                        this.delayedContent![y, x] = this.delayedContent[y, x].GetInvertedColor();
                    }
                }
            }
            else
            {
                for (int y = 0; y < this.undelayedContent!.GetHeight(); y++)
                {
                    for (int x = 0; x < this.undelayedContent!.GetWidth(); x++)
                    {
                        this.undelayedContent![y, x] = this.undelayedContent[y, x].GetInvertedColor();
                    }
                }
            }
        }

        private void Ctor(IReadOnlyRegion region)
        {
            if (this.content.Content.Value.Length == 0)
            {
                this.Region = new Region(region.TopLeft, region.TopLeft);
                this.undelayedContent = new CharInfo[0, 0];

                return;
            }

            this.Region = new Region(region);
            this.Region.OnChangeRequested +=
                (obj, e) =>
                {
                    e.Cancel(e.AfterChange.Height < this.content.Height
                        || e.AfterChange.Width < this.content.Width);
                };

            this.Region.OnChanged +=
                (obj, e) =>
                {
                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Move))
                    {
                        this.Region.TryMoveTo(e.AfterChange.TopLeft, out _);
                    }

                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                    {
                        this.Recalculate();
                    }
                };

            this.Recalculate();
        }

        private void Recalculate()
        {
            if (this.content.Content.ContainsDelays)
            {
                this.delayedContent = this.content.ToCharDelayArray(
                    this.backgroundFill.HasValue
                        ? new CharDelay(new CharInfo(' ', this.backgroundFill!.Value), 0)
                        : default);
            }
            else
            {
                this.undelayedContent = this.content.ToCharInfoArray(
                    this.backgroundFill.HasValue
                        ? new CharInfo(' ', this.backgroundFill!.Value)
                        : default);
            }
        }
    }
}
