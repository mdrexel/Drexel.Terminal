using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Output.Primitives;

namespace Game.Output.Layout
{
    public sealed class Border : IDrawable
    {
        private readonly Region outerRegion;
        private readonly CharInfo[,]? topLeft;
        private readonly CharInfo[,]? topRight;
        private readonly CharInfo[,]? bottomLeft;
        private readonly CharInfo[,]? bottomRight;
        private readonly CharInfo[,]? leftStroke;
        private readonly CharInfo[,]? topStroke;
        private readonly CharInfo[,]? rightStroke;
        private readonly CharInfo[,]? bottomStroke;

        public Border(
            Region outerRegion,
            CharInfo[,]? topLeft = null,
            CharInfo[,]? topRight = null,
            CharInfo[,]? bottomLeft = null,
            CharInfo[,]? bottomRight = null,
            CharInfo[,]? leftStroke = null,
            CharInfo[,]? topStroke = null,
            CharInfo[,]? rightStroke = null,
            CharInfo[,]? bottomStroke = null)
        {
            this.outerRegion = outerRegion;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.leftStroke = leftStroke;
            this.topStroke = topStroke;
            this.rightStroke = rightStroke;
            this.bottomStroke = bottomStroke;

            this.outerRegion.OnChanged += (obj, e) => this.Recalculate();

            this.Recalculate();
        }

        private void Recalculate()
        {
        }

        public Region InnerRegion { get; }

        public void Draw(ISink sink)
        {
        }
    }
}
