using Game.Output.Primitives;

namespace Game.Output.Layout.Symbols
{
    public class Solid : Symbol
    {
        private readonly Fill fill;

        public Solid(
            LayoutManager layoutManager,
            Region region,
            BorderBuilder borderBuilder,
            string name,
            CharColors fill)
            : base(
                  layoutManager,
                  region,
                  borderBuilder,
                  name)
        {
            this.fill = new Fill(this.InnerRegion, fill);
        }

        public Solid(
            LayoutManager layoutManager,
            Region region,
            BorderBuilder borderBuilder,
            string name,
            FormattedString fill,
            CharColors? backgroundFill = null)
            : base(
                  layoutManager,
                  region,
                  borderBuilder,
                  name)
        {
            this.fill = new Fill(this.InnerRegion, fill, backgroundFill);
        }

        public override bool CanBeFocused => false;

        public override bool CanBeMoved => false;

        public override bool CanBeResized => false;

        protected override void DrawInternal(ISink sink)
        {
            this.fill.Draw(sink);
        }

        protected override void DrawInternal(ISink sink, Rectangle region)
        {
            this.fill.Draw(sink, region);
        }

        protected override void InvertColorInternal()
        {
            this.fill.InvertColor();
        }
    }
}
