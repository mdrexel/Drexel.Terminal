using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public sealed class Fill : IDrawable
    {
        private CharDelay[,]? delayedContent;
        private CharInfo[,]? undelayedContent;

        public Fill(IReadOnlyRegion region, CharColors fill)
        {
            void Fill(IReadOnlyRegion fillToSize)
            {
                this.undelayedContent = new CharInfo[fillToSize.Height, fillToSize.Width];
                this.undelayedContent.Fill(new CharInfo(' ', fill));
            }

            this.Region = new Region(region);
            Fill(this.Region);

            this.Region.OnChanged +=
                (obj, e) =>
                {
                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                    {
                        Fill(e.AfterChange);
                    }
                };
        }

        public Fill(
            IReadOnlyRegion region,
            FormattedString pattern,
            CharColors? backgroundFill = null)
        {
            ContentValue contentValue = new ContentValue(pattern);
            this.Region = new Region(region);
            this.Region.OnChanged +=
                (obj, e) =>
                {
                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                    {
                        this.Recalculate(e.AfterChange, contentValue, backgroundFill);
                    }
                };

            this.Recalculate(this.Region, contentValue, backgroundFill);
        }

        internal Fill(
            FormattedString pattern,
            CharColors? backgroundFill = null)
        {
            ContentValue contentValue = new ContentValue(pattern);
            this.Region = new Region(Coord.Zero, contentValue.ToCoord());
            this.Region.OnChanged +=
                (obj, e) =>
                {
                    if (e.ChangeTypes.HasFlag(RegionChangeTypes.Resize))
                    {
                        this.Recalculate(e.AfterChange, contentValue, backgroundFill);
                    }
                };

            this.Recalculate(this.Region, contentValue, backgroundFill);
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

        public IRegion Region { get; }

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

        public void Draw(ISink sink, Rectangle window)
        {
            if (this.delayedContent is null)
            {
                sink.WriteRegion(
                    this.undelayedContent!,
                    this.Region.TopLeft,
                    window);
            }
            else
            {
                sink.WriteRegion(
                    this.delayedContent!,
                    this.Region.TopLeft,
                    window);
            }
        }

        private void Recalculate(
            IReadOnlyRegion sizeTo,
            ContentValue content,
            CharColors? backgroundFill)
        {
            if (content.Content.ContainsDelays)
            {
                this.delayedContent = content
                    .ToCharDelayArray(
                        backgroundFill.HasValue
                            ? new CharDelay(new CharInfo(' ', backgroundFill.Value), 0)
                            : default)
                    .Repeat(new Coord(sizeTo.Width, sizeTo.Height));
            }
            else
            {
                this.undelayedContent = content
                    .ToCharInfoArray(
                        backgroundFill.HasValue
                            ? new CharInfo(' ', backgroundFill.Value)
                            : default)
                    .Repeat(new Coord(sizeTo.Width, sizeTo.Height));
            }
        }
    }
}
