namespace Game.Output.Layout
{
    public sealed class BorderBuilder
    {
        private readonly CharInfo[,]? namePlate;
        private readonly CharInfo[,]? topLeft;
        private readonly CharInfo[,]? topRight;
        private readonly CharInfo[,]? bottomLeft;
        private readonly CharInfo[,]? bottomRight;
        private readonly CharInfo[,]? leftStroke;
        private readonly CharInfo[,]? topStroke;
        private readonly CharInfo[,]? rightStroke;
        private readonly CharInfo[,]? bottomStroke;

        public BorderBuilder(
            CharInfo[,]? namePlate = null,
            CharInfo[,]? topLeft = null,
            CharInfo[,]? topRight = null,
            CharInfo[,]? bottomLeft = null,
            CharInfo[,]? bottomRight = null,
            CharInfo[,]? leftStroke = null,
            CharInfo[,]? topStroke = null,
            CharInfo[,]? rightStroke = null,
            CharInfo[,]? bottomStroke = null)
        {
            this.namePlate = namePlate;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.leftStroke = leftStroke;
            this.topStroke = topStroke;
            this.rightStroke = rightStroke;
            this.bottomStroke = bottomStroke;
        }

        public Border Build(Region outerRegion)
        {
            return new Border(
                outerRegion,
                this.namePlate,
                this.topLeft,
                this.topRight,
                this.bottomLeft,
                this.bottomRight,
                this.leftStroke,
                this.topStroke,
                this.rightStroke,
                this.bottomStroke);
        }
    }
}
