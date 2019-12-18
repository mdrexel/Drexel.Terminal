using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Output.Layout
{
    public class LayoutManager
    {
        internal readonly Sink sink;
        private readonly LinkedList<Symbol> symbols;
        private readonly Dictionary<Symbol, LinkedListNode<Symbol>> symbolMapping;

        private Symbol? grabbed;

        public LayoutManager(Sink sink)
        {
            this.sink = sink;
            this.symbols = new LinkedList<Symbol>();
            this.symbolMapping = new Dictionary<Symbol, LinkedListNode<Symbol>>();

            this.grabbed = null;
        }

        public Symbol? Focused { get; private set; }

        public void Add(Symbol symbol)
        {
            LinkedListNode<Symbol> node = this.symbols.AddLast(symbol);
            this.symbolMapping.Add(symbol, node);
            symbol.Draw(this.sink);
        }

        public void AddAbove(Symbol newSymbol, Symbol oldSymbol)
        {
            LinkedListNode<Symbol> node = this.symbols.AddAfter(this.symbolMapping[oldSymbol], newSymbol);
            this.symbolMapping.Add(newSymbol, node);

            while (!(node is null))
            {
                if (node.Value.Region.Overlaps(newSymbol.Region))
                {
                    node.Value.Draw(this.sink);
                }

                node = node.Next;
            }
        }

        public void AddBelow(Symbol newSymbol, Symbol oldSymbol)
        {
            LinkedListNode<Symbol> node = this.symbols.AddBefore(this.symbolMapping[oldSymbol], newSymbol);
            this.symbolMapping.Add(newSymbol, node);

            while (!(node is null))
            {
                if (node.Value.Region.Overlaps(newSymbol.Region))
                {
                    node.Value.Draw(this.sink);
                }

                node = node.Next;
            }
        }

        public void Remove(Symbol symbol)
        {
            this.symbols.Remove(symbol);
            if (this.symbolMapping.Remove(symbol))
            {
                foreach (Symbol existing in this.symbols)
                {
                    if (existing.Region.Overlaps(symbol.Region))
                    {
                        existing.Draw(this.sink);
                    }
                }
            }
        }

        public void Draw()
        {
            foreach (Symbol symbol in this.symbols)
            {
                symbol.Draw(this.sink);
            }
        }

        public void Draw(Symbol impacted)
        {
            foreach (Symbol symbol in this.symbols)
            {
                if (symbol.Region.Overlaps(impacted.Region))
                {
                    symbol.Draw(this.sink);
                }
            }
        }

        public void KeyPressed(ConsoleKeyInfo keyInfo)
        {
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
            if (this.grabbed != null)
            {
                this.grabbed.Region.Translate(newPosition - oldPosition);
                this.Draw(this.grabbed);
            }
            else
            {
                foreach (Symbol symbol in this.symbols.Reverse())
                {
                    if (symbol.Region.Overlaps(newPosition))
                    {
                        symbol.MouseMoveEvent(
                            oldPosition - symbol.Region.TopLeft,
                            newPosition - symbol.Region.TopLeft);

                        break;
                    }
                }
            }
        }

        public void LeftMouseEvent(Coord coord, bool down)
        {
            if (!down)
            {
                this.grabbed = null;
            }

            foreach (Symbol symbol in this.symbols.Reverse())
            {
                if (symbol.Region.Overlaps(coord))
                {
                    if (symbol.InnerRegion.Overlaps(coord))
                    {
                        symbol.LeftMouseEvent(
                            coord - symbol.Region.TopLeft,
                            down);
                    }
                    else if (symbol.CanBeMoved && down)
                    {
                        this.grabbed = symbol;
                    }
                    else if (symbol.CanBeResized)
                    {
                        // TODO: impl resize
                    }

                    break;
                }
            }
        }

        public void RightMouseEvent(Coord coord, bool down)
        {
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
