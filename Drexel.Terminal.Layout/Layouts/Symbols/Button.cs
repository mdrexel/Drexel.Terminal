using System;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Text;

namespace Drexel.Terminal.Layout.Layouts.Symbols
{
    public class Button : Symbol
    {
        private readonly Observable<bool> onFired;
        private readonly Label backingLabel;

        public Button(
            IResizeableRegion region,
            string name,
            Catena content,
            Alignments alignments,
            TerminalColors? backgroundFill = null)
            : base(region, name)
        {
            this.onFired = new Observable<bool>();

            this.backingLabel = new Label(
                region,
                name + ".Content",
                content,
                alignments,
                backgroundFill);
        }

        public Catena Content
        {
            get => this.backingLabel.Content;
            set => this.backingLabel.Content = value;
        }

        public Alignments Alignments
        {
            get => this.backingLabel.Alignments;
            set => this.backingLabel.Alignments = value;
        }

        public TerminalColors? BackgroundFill
        {
            get => this.backingLabel.BackgroundFill;
            set => this.backingLabel.BackgroundFill = value;
        }

        public IObservable<bool> OnFired => this.onFired;

        public override bool CanBeFocused => true;

        public override bool CapturesTabKey => false;

        public override void LeftMouseEvent(Coord coord, bool down)
        {
            this.onFired.Next(down);
        }

        public override void Draw(ITerminalSink sink, Rectangle window)
        {
            this.backingLabel.Draw(sink, window);
        }

        public void Fire(bool down)
        {
            this.onFired.Next(down);
        }
    }
}
