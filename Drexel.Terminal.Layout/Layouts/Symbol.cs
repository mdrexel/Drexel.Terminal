using System;
using System.Collections.Generic;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Source;

namespace Drexel.Terminal.Layout.Layouts
{
    public abstract class Symbol : IDisposable
    {
        private readonly Observable<SymbolRedrawEventArgs> onRedrawRequested;
        private readonly List<IDisposable> subscriptions;

        private bool isDisposed;

        protected Symbol(IResizeableRegion region, string name)
        {
            this.Region = region ?? throw new ArgumentNullException(nameof(region));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));

            this.onRedrawRequested = new Observable<SymbolRedrawEventArgs>();

            this.subscriptions =
                new List<IDisposable>()
                {
                    this.Region.OnChanged.Subscribe(
                        new Observer<RegionChangedEventArgs>(
                            x => this.RequestRedraw(new IReadOnlyRegion[] { x.BeforeChange, x.AfterChange })))
                };

            this.isDisposed = false;
        }

        public string Name { get; }

        public IResizeableRegion Region { get; }

        public abstract bool CanBeFocused { get; }

        public abstract bool CapturesTabKey { get; }

        public IObservable<SymbolRedrawEventArgs> OnRedrawRequested => this.onRedrawRequested;

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.DisposeInternal();

                foreach (IDisposable disposable in this.subscriptions)
                {
                    disposable.Dispose();
                }

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

        protected virtual void DisposeInternal()
        {
        }

        protected void RequestRedraw()
        {
            this.onRedrawRequested.Next(new SymbolRedrawEventArgs(this.Region));
        }

        protected void RequestRedraw(params IReadOnlyRegion[] impactedRegionParams)
        {
            this.onRedrawRequested.Next(new SymbolRedrawEventArgs(impactedRegionParams));
        }

        protected void RequestRedraw(IReadOnlyList<IReadOnlyRegion> impactedRegions)
        {
            this.onRedrawRequested.Next(new SymbolRedrawEventArgs(impactedRegions));
        }

        protected void AddDisposable(IDisposable disposable)
        {
            this.subscriptions.Add(disposable);
        }
    }
}
