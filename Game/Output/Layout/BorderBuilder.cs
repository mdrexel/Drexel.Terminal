namespace Game.Output.Layout
{
    public sealed class BorderBuilder
    {
        private readonly FormattedString? defaultNamePlate;
        private readonly FormattedString? topLeft;
        private readonly FormattedString? topRight;
        private readonly FormattedString? bottomLeft;
        private readonly FormattedString? bottomRight;
        private readonly FormattedString? leftStroke;
        private readonly FormattedString? topStroke;
        private readonly FormattedString? rightStroke;
        private readonly FormattedString? bottomStroke;

        public BorderBuilder(
            FormattedString? defaultNamePlate = null,
            FormattedString? topLeft = null,
            FormattedString? topRight = null,
            FormattedString? bottomLeft = null,
            FormattedString? bottomRight = null,
            FormattedString? leftStroke = null,
            FormattedString? topStroke = null,
            FormattedString? rightStroke = null,
            FormattedString? bottomStroke = null)
        {
            this.defaultNamePlate = defaultNamePlate;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.leftStroke = leftStroke;
            this.topStroke = topStroke;
            this.rightStroke = rightStroke;
            this.bottomStroke = bottomStroke;
        }

        public Border Build(Region outerRegion, FormattedString? namePlate = null)
        {
            return new Border(
                outerRegion,
                namePlate ?? this.defaultNamePlate,
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
