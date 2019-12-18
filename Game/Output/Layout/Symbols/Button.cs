using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Output.Primitives;

namespace Game.Output.Layout.Symbols
{
    public class Button : Symbol
    {
        private readonly Label label;
        private readonly Rectangle? background;

        public Button(
            LayoutManager layoutManager,
            Region region,
            BorderBuilder borderBuilder,
            string name,
            FormattedString content,
            CharColors? backgroundFill = null)
            : base(
                  layoutManager,
                  region,
                  borderBuilder,
                  name)
        {
            if (backgroundFill.HasValue)
            {
                this.background = new Rectangle(this.InnerRegion, backgroundFill.Value);
            }

            this.label = new Label(this.InnerRegion.TopLeft, content);
            this.InnerRegion.OnChanged +=
                (obj, e) =>
                this.label.Region.Translate(e.CurrentRegion.TopLeft - e.PreviousRegion.TopLeft);
        }

        public override bool CanBeFocused => true;

        public override bool CanBeMoved => true;

        public override bool CanBeResized => true;

        public override void FocusChanged(bool focused)
        {
            this.InvertColor();
            this.LayoutManager.Draw(this);
        }

        public override void LeftMouseEvent(Coord coord, bool down)
        {
            Random random = new Random();
            CharInfo[,] stuff = new CharInfo[this.Region.Height, this.Region.Width];
            for (int y = 0; y < stuff.GetHeight(); y++)
            {
                for (int x = 0; x < stuff.GetWidth(); x++)
                {
                    stuff[y, x] = new CharInfo(' ', CharColors.GetRandom(random));
                }
            }

            Rectangle garbage = new Rectangle(this.Region.TopLeft, stuff);
            garbage.Draw(this.LayoutManager.sink);
        }

        protected override void DrawInternal(ISink sink)
        {
            this.background?.Draw(sink);
            this.label.Draw(sink);
        }

        protected override void InvertColorInternal()
        {
            this.label.InvertColor();
        }
    }
}
