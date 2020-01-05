using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Output.Layout
{
    public class LayoutManager
    {
        internal readonly ISink sink;
        private readonly LinkedList<Symbol> symbols;
        private readonly Dictionary<Symbol, LinkedListNode<Symbol>> symbolMapping;
        private readonly Dictionary<Symbol, IReadOnlyRegion> constraints;

        private readonly Func<bool> leftMouseStateCallback;
        private readonly Func<bool> rightMouseStateCallback;

        private bool active;
        private Symbol? grabbed;
        private Symbol? lastMouseMove;

        public LayoutManager(
            ISink sink,
            Func<bool> leftMouseStateCallback,
            Func<bool> rightMouseStateCallback,
            bool active)
        {
            this.sink = sink;
            this.symbols = new LinkedList<Symbol>();
            this.symbolMapping = new Dictionary<Symbol, LinkedListNode<Symbol>>();
            this.constraints = new Dictionary<Symbol, IReadOnlyRegion>();

            this.leftMouseStateCallback = leftMouseStateCallback;
            this.rightMouseStateCallback = rightMouseStateCallback;

            this.grabbed = null;
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

            if (this.Active)
            {
                symbol.Draw(this.sink);
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
                    node.Value.Draw(this.sink, newSymbol.Region);
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
                    node.Value.Draw(this.sink, newSymbol.Region);
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
                    existing.Draw(this.sink, symbol.Region);
                }
            }
        }

        public void SetConstraint(Symbol symbol, IReadOnlyRegion region)
        {
            this.constraints[symbol] = region;
        }

        public void RemoveConstraint(Symbol symbol)
        {
            this.constraints.Remove(symbol);
        }

        public void Draw()
        {
            if (this.Active)
            {
                foreach (Symbol symbol in this.symbols)
                {
                    symbol.Draw(this.sink);
                }
            }
        }

        public void Draw(IReadOnlyRegion region)
        {
            if (this.Active)
            {
                foreach (Symbol symbol in this.symbols)
                {
                    symbol.Draw(this.sink, region);
                }
            }
        }

        public void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (!this.Active)
            {
                return;
            }

            // TODO: What if the symbol wants control of pressing tab (ex. text editor)?
            if (keyInfo.Key == ConsoleKey.Tab)
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

            if (this.grabbed != null)
            {
                Coord delta = newPosition - oldPosition;
                if (this.constraints.TryGetValue(this.grabbed, out IReadOnlyRegion constrainedTo))
                {
                    Region newRegion = new Region(this.grabbed.Region);
                    newRegion.TryTranslate(delta, out _);
                    if (!constrainedTo.Contains(newRegion))
                    {
                        return;
                    }
                }

                if (this.grabbed.Region.TryTranslate(delta, out IReadOnlyRegion beforeChange))
                {
                    // Re-draw the areas we just left, and the areas we're moving into
                    Region superset = new Region(
                        new Coord(
                            Math.Min(beforeChange.TopLeft.X, this.grabbed.Region.TopLeft.X),
                            Math.Min(beforeChange.TopLeft.Y, this.grabbed.Region.TopLeft.Y)),
                        new Coord(
                            Math.Max(beforeChange.BottomRight.X, this.grabbed.Region.BottomRight.X),
                            Math.Max(beforeChange.BottomRight.Y, this.grabbed.Region.BottomRight.Y)));
                    this.Draw(superset);
                }
            }
            else
            {
                foreach (Symbol symbol in this.symbols.Reverse())
                {
                    if (symbol.Region.Overlaps(newPosition))
                    {
                        if (symbol != this.lastMouseMove)
                        {
                            this.lastMouseMove?.MouseExitedSymbol();
                            symbol.MouseEnteredSymbol(
                                this.leftMouseStateCallback.Invoke(),
                                this.rightMouseStateCallback.Invoke());

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

            if (!down)
            {
                this.grabbed = null;
            }

            foreach (Symbol symbol in this.symbols.Reverse())
            {
                if (symbol.Border.TryGetComponentAt(
                    coord,
                    out BorderComponentType component,
                    out _))
                {
                    if (component == BorderComponentType.Center)
                    {
                        symbol.LeftMouseEvent(
                            coord - symbol.Region.TopLeft,
                            down);
                    }
                    else if (component == BorderComponentType.Top && symbol.CanBeMoved && down)
                    {
                        this.grabbed = symbol;
                    }
                    else if (symbol.CanBeResized)
                    {
                        // TOOD: impl resize
                    }

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
    }
}
