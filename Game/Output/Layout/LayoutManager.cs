using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Output.Layout
{
    public class LayoutManager
    {
        private readonly Sink sink;
        private readonly List<Symbol> symbols;

        public LayoutManager(Sink sink)
        {
            this.sink = sink;
        }

        public void AddSymbol(Symbol symbol)
        {

        }

        public void RemoveSymbol(Symbol symbol)
        {
            // 1. Zero out the region of the symbol
            // 2. For each symbol that overlaps with the removed symbol, re-draw it
        }
    }
}
