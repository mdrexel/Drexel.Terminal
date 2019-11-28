﻿namespace Game.Output.Layout
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

        internal Border(
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
            this.InnerRegion = outerRegion;

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

        public Region InnerRegion { get; private set; }

        public void Draw(ISink sink)
        {
            if (this.topLeft != null)
            {
                sink.WriteRegion(
                    this.topLeft,
                    this.outerRegion.TopLeft.X,
                    this.outerRegion.TopLeft.Y);
            }

            if (this.topRight != null)
            {
                sink.WriteRegion(
                    this.topRight,
                    (short)(this.outerRegion.BottomRight.X - this.topRight.GetWidth()),
                    this.outerRegion.TopLeft.Y);
            }

            if (this.bottomLeft != null)
            {
                sink.WriteRegion(
                    this.bottomLeft,
                    this.outerRegion.TopLeft.X,
                    (short)(this.outerRegion.BottomRight.Y - this.bottomLeft.GetHeight()));
            }

            if (this.bottomRight != null)
            {
                sink.WriteRegion(
                    this.bottomRight,
                    (short)(this.outerRegion.BottomRight.X - this.bottomRight.GetWidth()),
                    (short)(this.outerRegion.BottomRight.Y - this.bottomRight.GetHeight()));
            }
        }

        private void Recalculate()
        {
            short largestTopOffset = Largest(
                this.topLeft?.GetHeight(),
                this.topRight?.GetHeight(),
                this.topStroke?.GetHeight());
            short largestLeftOffset = Largest(
                this.topLeft?.GetWidth(),
                this.topRight?.GetWidth(),
                this.leftStroke?.GetWidth());
            short largestBottomOffset = Largest(
                this.bottomLeft?.GetHeight(),
                this.bottomRight?.GetHeight(),
                this.bottomStroke?.GetHeight());
            short largestRightOffset = Largest(
                this.topRight?.GetWidth(),
                this.bottomRight?.GetWidth(),
                this.rightStroke?.GetWidth());

            this.InnerRegion = new Region(
                this.outerRegion.TopLeft + new Coord(largestLeftOffset, largestTopOffset),
                this.outerRegion.BottomRight - new Coord(largestRightOffset, largestBottomOffset));
        }

        private static short Largest(params short?[] values)
        {
            short largest = 0;
            foreach (short? value in values)
            {
                if (value.HasValue && value > largest)
                {
                    largest = value.Value;
                }
            }

            return largest;
        }
    }
}