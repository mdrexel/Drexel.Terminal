using System;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Primitives;
using Drexel.Terminal.Sink;

namespace Drexel.Terminal.Layout.Layouts.Symbols
{
    public class Solid : Symbol
    {
        private Fill backingFill;

        public Solid(
            IResizeableRegion region,
            string name,
            CharInfo[,] pattern)
            : base(region, name)
        {
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            this.AddDisposable(
                this.Region.OnChanged.Subscribe(
                    new Observer<RegionChangedEventArgs>(
                        x =>
                        {
                            this.backingFill = new Fill(x.AfterChange.TopLeft, x.AfterChange.BottomRight, pattern);
                            this.RequestRedraw(x.BeforeChange, x.AfterChange);
                        })));

            this.backingFill = new Fill(this.Region.TopLeft, this.Region.BottomRight, pattern);
        }

        public override bool CanBeFocused => false;

        public override bool CapturesTabKey => false;

        public override void Draw(ITerminalSink sink, Rectangle window)
        {
            if (!this.Region.Overlaps(window))
            {
                return;
            }

            sink.Write(
                this.backingFill.GetFullSize(),
                new Coord(window.Left, window.Top),
                window - this.Region.TopLeft);
        }
    }
}
