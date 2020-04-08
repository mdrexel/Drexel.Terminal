using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Source;

namespace Drexel.Terminal.Layout.Layouts
{
    public class LayoutManager : IDisposable
    {
        private readonly ITerminal terminal;
        private readonly List<IDisposable> subscriptions;

        private readonly LinkedList<Symbol> symbols;
        private readonly Dictionary<Symbol, LinkedListNode<Symbol>> symbolMapping;
        private readonly Dictionary<Symbol, List<IDisposable>> symbolSubscriptions;

        private bool active;
        private Symbol? lastMouseMove;

        public LayoutManager(
            ITerminal terminal,
            bool active)
        {
            this.terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));

            this.subscriptions =
                new List<IDisposable>()
                {
                    this.terminal.Source.OnKeyPressed.Subscribe(
                        new Observer<TerminalKeyInfo>(
                            x => this.KeyPressed(x))),
                    this.terminal.Source.Mouse.OnMove.Subscribe(
                        new Observer<MouseMoveEventArgs>(
                            x => this.MouseMoveEvent(x.PreviousPosition, x.CurrentPosition))),
                    this.terminal.Source.Mouse.LeftButton.OnButton.Subscribe(
                        new Observer<MouseClickEventArgs>(
                            x => this.LeftMouseEvent(x.Position, x.ButtonDown))),
                    this.terminal.Source.Mouse.RightButton.OnButton.Subscribe(
                        new Observer<MouseClickEventArgs>(
                            x => this.RightMouseEvent(x.Position, x.ButtonDown))),
                    this.terminal.Source.Mouse.OnScrollWheel.Subscribe(
                        new Observer<MouseWheelEventArgs>(
                            x => this.ScrollEvent(x.Position, x.Direction == MouseWheelDirection.Down)))
                };

            this.symbols = new LinkedList<Symbol>();
            this.symbolMapping = new Dictionary<Symbol, LinkedListNode<Symbol>>();
            this.symbolSubscriptions = new Dictionary<Symbol, List<IDisposable>>();

            this.lastMouseMove = null;
            this.active = active;
        }

        /// <summary>
        /// Gets the symbols contained by this layout manager.
        /// <br/><br/>
        /// Note that the symbols are returned in the order they will be drawn; i.e. back-to-front.
        /// </summary>
        public IReadOnlyCollection<Symbol> Symbols => this.symbols;

        /// <summary>
        /// Gets the currently-focused symbol, if one exists; otherwise, returns <see langword="null"/>.
        /// </summary>
        public Symbol? Focused { get; private set; }

        /// <summary>
        /// Gets or sets whether this layout manager is active. An active layout manager will draw symbols and process
        /// events; an inactive layout manager will not draw symbols, and will ignore events.
        /// </summary>
        public bool Active
        {
            get => this.active;
            set
            {
                this.active = value;
                if (!this.active)
                {
                    // When the layout becomes inactive, lose focus.
                    this.Focused?.FocusChanged(false);
                    this.Focused = null;
                }
                else
                {
                    // When the layout becomes active, re-draw.
                    this.Draw();
                }
            }
        }

        public void Add(Symbol symbol)
        {
            LinkedListNode<Symbol> node = this.symbols.AddLast(symbol);
            this.symbolMapping.Add(symbol, node);

            this.Subscribe(symbol);

            if (this.Active)
            {
                symbol.Draw(this.terminal.Sink);
            }
        }

        public void AddAbove(Symbol newSymbol, Symbol oldSymbol)
        {
            LinkedListNode<Symbol> node = this.symbols.AddAfter(this.symbolMapping[oldSymbol], newSymbol);
            this.symbolMapping.Add(newSymbol, node);

            if (this.Active)
            {
                while (!(node is null))
                {
                    node.Value.Draw(this.terminal.Sink, newSymbol.Region);
                    node = node.Next;
                }
            }
        }

        public void AddBelow(Symbol newSymbol, Symbol oldSymbol)
        {
            LinkedListNode<Symbol> node = this.symbols.AddBefore(this.symbolMapping[oldSymbol], newSymbol);
            this.symbolMapping.Add(newSymbol, node);

            if (this.Active)
            {
                while (!(node is null))
                {
                    node.Value.Draw(this.terminal.Sink, newSymbol.Region);
                    node = node.Next;
                }
            }
        }

        public void Remove(Symbol symbol)
        {
            LinkedListNode<Symbol> node = this.symbolMapping[symbol];
            this.symbols.Remove(node);
            this.symbolMapping.Remove(symbol);

            if (this.Active)
            {
                foreach (Symbol existing in this.symbols)
                {
                    existing.Draw(this.terminal.Sink, symbol.Region);
                }
            }
        }

        public void Draw()
        {
            if (this.Active)
            {
                foreach (Symbol symbol in this.symbols)
                {
                    symbol.Draw(this.terminal.Sink);
                }
            }
        }

        public void Draw(IReadOnlyRegion region)
        {
            if (this.Active)
            {
                Symbol? containedBy = this.symbols.Reverse().FirstOrDefault(x => x.Region.Contains(region));

                foreach (Symbol symbol in
                    object.ReferenceEquals(containedBy, null)
                        ? this.symbols
                        : this.symbols.SkipWhile(x => !object.ReferenceEquals(x, containedBy)))
                {
                    symbol.Draw(this.terminal.Sink, region);
                }
            }
        }

        public void KeyPressed(TerminalKeyInfo keyInfo)
        {
            if (!this.Active)
            {
                return;
            }

            // TODO: What if the symbol wants control of pressing tab (ex. text editor)?
            if (keyInfo.Key == TerminalKey.Tab)
            {
                if (this.Focused is null)
                {
                    this.Focused = this.symbols.FirstOrDefault(x => x.CanBeFocused);
                    this.Focused?.FocusChanged(true);
                }
                else
                {
                    LinkedListNode<Symbol> symbol = this.symbolMapping[this.Focused].Next;
                    while (symbol != null && !symbol.Value.CanBeFocused)
                    {
                        symbol = symbol.Next;
                    }

                    Symbol? newFocus;
                    if (symbol is null)
                    {
                        newFocus = this.symbols.FirstOrDefault(x => x.CanBeFocused);
                    }
                    else
                    {
                        newFocus = symbol.Value;
                    }

                    if (newFocus != this.Focused)
                    {
                        this.Focused.FocusChanged(false);
                        this.Focused = newFocus;
                        this.Focused?.FocusChanged(true);
                    }
                    else
                    {
                        // We cycled through the list of focusable symbols, and landed back on the current symbol.
                        // Un-focus the current symbol, so that you can "cycle" back to having nothing focused.
                        this.Focused.FocusChanged(false);
                        this.Focused = default;
                    }
                }
            }
            else
            {
                this.Focused?.KeyPressed(keyInfo);
            }
        }

        public void MouseMoveEvent(Coord oldPosition, Coord newPosition)
        {
            if (!this.Active)
            {
                return;
            }

            foreach (Symbol symbol in this.symbols.Reverse())
            {
                if (symbol.Region.Overlaps(newPosition))
                {
                    if (symbol != this.lastMouseMove)
                    {
                        this.lastMouseMove?.MouseExitedSymbol(oldPosition);
                        symbol.MouseEnteredSymbol(
                            newPosition,
                            this.terminal.Source.Mouse.LeftButton.Down,
                            this.terminal.Source.Mouse.RightButton.Down);

                        this.lastMouseMove = symbol;
                    }
                    else
                    {
                        // Only notify of a move event AFTER we've entered the symbol.
                        symbol.MouseMoveEvent(
                            oldPosition - symbol.Region.TopLeft,
                            newPosition - symbol.Region.TopLeft);
                    }

                    break;
                }
            }
        }

        public void LeftMouseEvent(Coord coord, bool down)
        {
            if (!this.Active)
            {
                return;
            }

            // Focus is lost when a mouse event occurs
            this.Focused?.FocusChanged(false);
            this.Focused = null;

            foreach (Symbol symbol in this.symbols.Reverse())
            {
                if (symbol.Region.Overlaps(coord))
                {
                    symbol.LeftMouseEvent(
                        coord - symbol.Region.TopLeft,
                        down);

                    break;
                }
            }
        }

        public void RightMouseEvent(Coord coord, bool down)
        {
            if (!this.Active)
            {
                return;
            }

            // Focus is lost when a mouse event occurs
            this.Focused?.FocusChanged(false);
            this.Focused = null;

            foreach (Symbol symbol in this.symbols.Reverse())
            {
                if (symbol.Region.Overlaps(coord))
                {
                    symbol.RightMouseEvent(
                        coord - symbol.Region.TopLeft,
                        down);

                    break;
                }
            }
        }

        public void ScrollEvent(Coord coord, bool down)
        {
            if (!this.Active)
            {
                return;
            }

            // Focus is lost when a scroll event occurs
            this.Focused?.FocusChanged(false);
            this.Focused = null;

            foreach (Symbol symbol in this.symbols.Reverse())
            {
                if (symbol.Region.Overlaps(coord))
                {
                    symbol.ScrollEvent(
                        coord - symbol.Region.TopLeft,
                        down);

                    break;
                }
            }
        }

        public void Dispose()
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (IDisposable disposable in this.subscriptions)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            else if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }
        }

        private void Subscribe(Symbol symbol)
        {
            List<IDisposable> subscriptions =
                new List<IDisposable>()
                {
                    symbol.OnRedrawRequested.Subscribe(
                        new Observer<SymbolRedrawEventArgs>(
                            x =>
                            {
                                foreach (IReadOnlyRegion region in x.ImpactedRegions)
                                {
                                this.Draw(region);
                                }
                            }))
                };

            this.symbolSubscriptions.Add(symbol, subscriptions);
        }

        private void Unsubscribe(Symbol symbol)
        {
            List<IDisposable> subscriptions = this.symbolSubscriptions[symbol];
            this.symbolSubscriptions.Remove(symbol);

            foreach (IDisposable subscription in subscriptions)
            {
                subscription.Dispose();
            }
        }
    }
}
