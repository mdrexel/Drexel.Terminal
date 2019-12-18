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
            CharInfo[,] content = new CharInfo[this.InnerRegion.Height, this.InnerRegion.Width];
            for (int y = 0; y < content.GetHeight(); y++)
            {
                for (int x = 0; x < content.GetWidth(); x++)
                {
                    content[y, x] = new CharInfo(' ', fill);
                }
            }

            this.fill = new Rectangle(this.InnerRegion.TopLeft, content);
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
