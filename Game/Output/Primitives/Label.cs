using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Output.Layout;

namespace Game.Output.Primitives
{
    public class Label : IDrawable
    {
        private readonly Coord topLeft;
        private readonly FormattedString content;

        private readonly CharDelay[,]? delayedContent;
        private readonly CharInfo[,]? undelayedContent;

        public Label(FormattedString content)
        {
            this.topLeft = new Coord(0, 0);
            this.content = content;

            this.Calculate();
        }

        public Label(Coord topLeft, FormattedString content)
        {
            this.topLeft = topLeft;
            this.content = content;

            this.Calculate();
        }

        public static Label Empty { get; } = new Label(FormattedString.Empty);

        public IMoveOnlyRegion Region { get; private set; }

        public void Draw(ISink sink)
        {
            sink.WriteRegion()
        }

        private void Calculate()
        {


            this.Region = new Region(this.topLeft, this.topLeft);
            throw new NotImplementedException("Need to actually create the [,] content");
        }
    }
}
