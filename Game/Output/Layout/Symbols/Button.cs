using System;
using Game.Output.Primitives;

namespace Game.Output.Layout.Symbols
{
    public class Button : Symbol
    {
        private readonly Fill? background;

        private FormattedString text;
        private Alignments alignments;
        private Label label;
        private bool inverted;

        public Button(
            LayoutManager layoutManager,
            Region region,
            BorderBuilder borderBuilder,
            string name,
            FormattedString content,
            Alignments alignments,
            CharColors? backgroundFill = null)
            : base(
                  layoutManager,
                  region,
                  borderBuilder,
                  name)
        {
            this.text = content;
            this.alignments = alignments;
            this.inverted = false;

            if (backgroundFill.HasValue)
            {
                this.background = new Fill(this.InnerRegion, backgroundFill.Value);
            }

            this.Recalculate();

            this.InnerRegion.OnChanged +=
                (obj, e) =>
                this.label.Region.TryTranslate(e.AfterChange.TopLeft - e.BeforeChange.TopLeft, out _);
        }

        public override bool CanBeFocused => true;

        public override bool CanBeMoved => true;

        public override bool CanBeResized => false;

        public FormattedString Text
        {
            get => this.text;
            set
            {
                if (this.text == value)
                {
                    return;
                }

                this.text = value;
                this.Recalculate();
            }
        }

        public Alignments Alignments
        {
            get => this.alignments;
            set
            {
                if (this.alignments == value)
                {
                    return;
                }

                this.alignments = value;
                this.Recalculate();
            }
        }

        public event EventHandler<ClickedEventArgs>? OnClicked;

        public override void FocusChanged(bool focused)
        {
            this.InvertEvent(focused);
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
            this.InvertEvent(leftMouseDown);
        }

        public override void MouseExitedSymbol()
        {
            this.InvertEvent(false);
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

        private void InvertEvent(bool wantToBeInverted)
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

        private void Recalculate()
        {
            this.label = new Label(
                this.InnerRegion,
                this.alignments,
                this.text);
            if (this.inverted)
            {
                this.label.InvertColor();
            }

            this.LayoutManager.Draw(this.InnerRegion);
        }
    }
}
