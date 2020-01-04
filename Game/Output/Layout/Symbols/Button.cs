using System;
using Game.Output.Primitives;

namespace Game.Output.Layout.Symbols
{
    public class Button : Symbol
    {
        private readonly Fill? background;

        private FormattedString text;
        private Label label;
        private bool inverted;

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
            this.Text = content;
            this.inverted = false;

            if (backgroundFill.HasValue)
            {
                this.background = new Fill(this.InnerRegion, backgroundFill.Value);
            }

            this.InnerRegion.OnChanged +=
                (obj, e) =>
                this.label.Region.TryTranslate(e.AfterChange.TopLeft - e.BeforeChange.TopLeft, out _);
        }

        public override bool CanBeFocused => true;

        public override bool CanBeMoved => true;

        public override bool CanBeResized => true;

        public FormattedString Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.label = new Label(this.InnerRegion.TopLeft, this.text);
                if (this.inverted)
                {
                    this.label.InvertColor();
                }

                this.LayoutManager.Draw(this.InnerRegion);
            }
        }

        public event EventHandler<ClickedEventArgs>? OnClicked;

        public override void FocusChanged(bool focused)
        {
            this.AppearanceEvent(focused);
        }

        public override void LeftMouseEvent(Coord coord, bool down)
        {
            if (!down)
            {
                this.OnClicked?.Invoke(this, new ClickedEventArgs(coord));
            }
        }

        public override void MouseEnteredSymbol(bool leftMouseDown, bool rightMouseDown)
        {
            this.AppearanceEvent(leftMouseDown);
        }

        public override void MouseExitedSymbol()
        {
            this.AppearanceEvent(false);
        }

        protected override void DrawInternal(ISink sink)
        {
            this.background?.Draw(sink);
            this.label.Draw(sink);
        }

        protected override void DrawInternal(ISink sink, Rectangle region)
        {
            this.background?.Draw(sink, region);
            this.label.Draw(sink, region);
        }

        protected override void InvertColorInternal()
        {
            this.label.InvertColor();
        }

        private void AppearanceEvent(bool wantToBeInverted)
        {
            if (wantToBeInverted && !this.inverted)
            {
                this.inverted = true;

                this.InvertColor();
                this.LayoutManager.Draw(this.Region);
            }
            else if (!wantToBeInverted && this.inverted)
            {
                this.inverted = false;

                this.InvertColor();
                this.LayoutManager.Draw(this.Region);
            }
        }
    }
}
