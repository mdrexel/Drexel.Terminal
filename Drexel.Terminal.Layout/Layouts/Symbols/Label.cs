using System;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Text;

namespace Drexel.Terminal.Layout.Layouts.Symbols
{
    public class Label : Symbol
    {
        private readonly Catena content;
        private readonly Alignments alignments;

        private readonly object cacheLock;
        private CharInfo[,] cached;

        public Label(
            IResizeableRegion region,
            string name,
            Catena content,
            Alignments alignments)
            : base(region, name)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.content = content;
            this.alignments = alignments;

            this.cacheLock = new object();
            this.cached = null!;

            this.AddDisposable(
                region.OnChanged.Subscribe(
                    new Observer<RegionChangedEventArgs>(
                        x =>
                        {
                            lock (this.cacheLock)
                            {
                                this.cached = null!;
                                this.RequestRedraw(x.BeforeChange, x.AfterChange);
                            }
                        })));
        }

        public override bool CanBeFocused => false;

        public override bool CapturesTabKey => false;

        public override void Draw(ITerminalSink sink, Rectangle window)
        {
            if (this.cached is null)
            {
            }

            Coord destination = new Coord(window.Left, window.Top);
            sink.Write(this.cached!, destination, window - destination);
        }

        private CharInfo[,] Generate()
        {
            if (this.alignments.HorizontalAlignment == HorizontalAlignment.Left)
            {
                if (this.alignments.VerticalAlignment == VerticalAlignment.Top)
                {
                    return this.GenerateTopLeft();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Center)
                {
                    return this.GenerateCenterLeft();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return this.GenerateBottomLeft();
                }
            }
            else if (this.alignments.HorizontalAlignment == HorizontalAlignment.Center)
            {
                if (this.alignments.VerticalAlignment == VerticalAlignment.Top)
                {
                    return this.GenerateTopCenter();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Center)
                {
                    return this.GenerateCenterCenter();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return this.GenerateBottomCenter();
                }
            }
            else if (this.alignments.HorizontalAlignment == HorizontalAlignment.Right)
            {
                if (this.alignments.VerticalAlignment == VerticalAlignment.Top)
                {
                    return this.GenerateTopRight();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Center)
                {
                    return this.GenerateCenterRight();
                }
                else if (this.alignments.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return this.GenerateBottomRight();
                }
            }

            throw new InvalidOperationException("Unrecognized combination of alignments.");
        }

        private CharInfo[,] GenerateTopLeft()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateCenterLeft()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateBottomLeft()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateTopCenter()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateCenterCenter()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateBottomCenter()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateTopRight()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateCenterRight()
        {
            throw new NotImplementedException();
        }

        private CharInfo[,] GenerateBottomRight()
        {
            throw new NotImplementedException();
        }
    }
}
