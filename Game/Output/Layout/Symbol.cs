using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Output.Layout
{
    public abstract class Symbol
    {
        public Symbol(
            string name,
            Region region,
            BorderBuilder borderBuilder)
        {
            this.Region = region;
        }

        public Region Region { get; }

        protected Region InnerRegion { get; }
    }
}
