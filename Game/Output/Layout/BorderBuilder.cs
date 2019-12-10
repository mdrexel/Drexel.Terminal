using Game.Output.Primitives;

namespace Game.Output.Layout
{
    public sealed class BorderBuilder
    {
        private readonly FormattedString? namePlate;
        private readonly FormattedString? topLeft;
        private readonly FormattedString? topRight;
        private readonly FormattedString? bottomLeft;
        private readonly FormattedString? bottomRight;
        private readonly FormattedString? leftStroke;
        private readonly FormattedString? topStroke;
        private readonly FormattedString? rightStroke;
        private readonly FormattedString? bottomStroke;

        public BorderBuilder(
            FormattedString? namePlate = null,
            FormattedString? topLeft = null,
            FormattedString? topRight = null,
            FormattedString? bottomLeft = null,
            FormattedString? bottomRight = null,
            FormattedString? leftStroke = null,
            FormattedString? topStroke = null,
            FormattedString? rightStroke = null,
            FormattedString? bottomStroke = null)
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
