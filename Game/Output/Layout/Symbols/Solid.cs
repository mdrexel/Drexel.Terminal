using Game.Output.Primitives;

namespace Game.Output.Layout.Symbols
{
    public class Solid : Symbol
    {
        private readonly Rectangle fill;

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
            this.fill = new Rectangle(this.InnerRegion, fill);
        }

        public override bool CanBeFocused => false;

        public override bool CanBeMoved => false;

        public override bool CanBeResized => false;

        protected override void DrawInternal(ISink sink)
        {
            this.fill.Draw(sink);
        }

        protected override void InvertColorInternal()
        {
            this.fill.InvertColor();
        }
    }
}
