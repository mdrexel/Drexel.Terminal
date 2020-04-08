using System;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Source;

namespace Drexel.Terminal.Layout.Layouts
{
    public abstract class Symbol : IDisposable
    {
        private readonly Observable<Symbol> onRedrawRequested;

        private bool isDisposed;

        protected Symbol(IResizeableRegion region, string name)
        {
            this.Region = region ?? throw new ArgumentNullException(nameof(region));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));

            this.onRedrawRequested = new Observable<Symbol>();

            this.isDisposed = false;
        }

        public string Name { get; }

        public IResizeableRegion Region { get; }

        public abstract bool CanBeFocused { get; }

        public IObservable<Symbol> OnRedrawRequested { get; }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.DisposeInternal();

                this.onRedrawRequested.Complete();
                this.isDisposed = true;
            }
        }

        public abstract void Draw(ITerminalSink sink, Rectangle window);

        public virtual void KeyPressed(TerminalKeyInfo keyInfo)
        {
        }

        public virtual void MouseMoveEvent(Coord oldPosition, Coord newPosition)
        {
        }

        public virtual void LeftMouseEvent(Coord coord, bool down)
        {
        }

        public virtual void MouseEnteredSymbol(Coord enterCoord, bool leftMouseDown, bool rightMouseDown)
        {
        }

        public virtual void MouseExitedSymbol(Coord exitCoord)
        {
        }

        public virtual void RightMouseEvent(Coord coord, bool down)
        {
        }

        public virtual void ScrollEvent(Coord coord, bool down)
        {
        }

        public virtual void FocusChanged(bool focused)
        {
        }

        protected void RequestRedraw()
        {
            this.onRedrawRequested.Next(this);
        }

        protected virtual void DisposeInternal()
        {
        }
    }
}
