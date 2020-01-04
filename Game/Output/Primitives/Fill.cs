using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Fill : IDrawable
    {
        private readonly FormattedString? fill;
        private readonly CharColors? backgroundFill;

        private CharDelay[,]? delayedContent;
        private CharInfo[,]? undelayedContent;

        public Fill(IReadOnlyRegion region, CharColors fill)
        {
            this.Region = new Region(region);
            this.undelayedContent = new CharInfo[region.Height, region.Width];
            for (int y = 0; y < this.undelayedContent.GetHeight(); y++)
            {
                for (int x = 0; x < this.undelayedContent.GetWidth(); x++)
                {
                    this.undelayedContent[y, x] = new CharInfo(' ', fill);
                }
            }
        }

        public Fill(
            IReadOnlyRegion region,
            FormattedString fill,
            CharColors? backgroundFill = null)
        {
            this.fill = fill;
            this.backgroundFill = backgroundFill;

            this.Region = new Region(region);
            this.Region.OnChanged +=
                (obj, e) =>
                {
                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                    {
                        this.Recalculate(e.AfterChange);
                    }
                };

            this.Recalculate(this.Region);
        }

        internal Fill(Coord topLeft, CharDelay[,] content)
        {
            this.delayedContent = content;
            this.Region = new Region(topLeft, topLeft + content.ToCoord());
        }

        internal Fill(Coord topLeft, CharInfo[,] content)
        {
            this.undelayedContent = content;
            this.Region = new Region(topLeft, topLeft + content.ToCoord());
        }

        public static Fill Empty { get; } = new Fill(new Coord(0, 0), new CharInfo[0, 0]);

        public IMoveOnlyRegion Region { get; }

        public void InvertColor()
        {
            if (this.delayedContent is null)
            {
                for (int y = 0; y < this.undelayedContent!.GetHeight(); y++)
                {
                    for (int x = 0; x < this.undelayedContent!.GetWidth(); x++)
                    {
                        this.undelayedContent![y, x] = this.undelayedContent[y, x].GetInvertedColor();
                    }
                }
            }
            else
            {
                for (int y = 0; y < this.delayedContent.GetHeight(); y++)
                {
                    for (int x = 0; x < this.delayedContent.GetWidth(); x++)
                    {
                        this.delayedContent[y, x] = this.delayedContent[y, x].GetInvertedColor();
                    }
                }
            }
        }

        public void Draw(ISink sink)
        {
            if (this.delayedContent is null)
            {
                sink.WriteRegion(this.undelayedContent!, this.Region.TopLeft);
            }
            else
            {
                sink.WriteRegion(this.delayedContent!, this.Region.TopLeft);
            }
        }

        public void Draw(ISink sink, Rectangle region)
        {
            if (this.delayedContent is null)
            {
                sink.WriteRegion(
                    this.undelayedContent!,
                    this.Region.TopLeft,
                    region);
            }
            else
            {
                sink.WriteRegion(
                    this.delayedContent!,
                    this.Region.TopLeft,
                    region);
            }
        }

        private void Recalculate(IReadOnlyRegion region)
        {
            Label label = new Label(this.fill!, this.backgroundFill);
            label.RepeatHorizontally(region.Width);
            label.RepeatVertically(region.Height);

            if (label.HasDelayedContent)
            {
                this.delayedContent = label.DelayedContent;
            }
            else
            {
                this.undelayedContent = label.UndelayedContent;
            }
        }
    }
}
