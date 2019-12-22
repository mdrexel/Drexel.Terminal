namespace Game.Output.Layout
{
    public sealed class BorderBuilder
    {
        private readonly bool ignoreNamePlates;
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
            this.ignoreNamePlates = false;
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

        public BorderBuilder(
            bool ignoreNamePlates,
            FormattedString? topLeft = null,
            FormattedString? topRight = null,
            FormattedString? bottomLeft = null,
            FormattedString? bottomRight = null,
            FormattedString? leftStroke = null,
            FormattedString? topStroke = null,
            FormattedString? rightStroke = null,
            FormattedString? bottomStroke = null)
        {
            this.ignoreNamePlates = ignoreNamePlates;
            this.defaultNamePlate = null;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottomRight = bottomRight;
            this.leftStroke = leftStroke;
            this.topStroke = topStroke;
            this.rightStroke = rightStroke;
            this.bottomStroke = bottomStroke;
        }

        public static BorderBuilder Empty { get; } = new BorderBuilder(true);

        public static BorderBuilder CreateExtraThickWindowStyle(
            CharColors borderColors,
            FormattedString? defaultNamePlate = null)
        {
            return new BorderBuilder(
                defaultNamePlate,
                topLeft: new FormattedString("╔═╦\r\n║ ║\r\n╠═╬", borderColors),
                topRight: new FormattedString("╦═╗\r\n║ ║\r\n╬═╣", borderColors),
                bottomLeft: new FormattedString("╠═╬\r\n╚═╩", borderColors),
                bottomRight: new FormattedString("╬═╣\r\n╩═╝", borderColors),
                leftStroke: new FormattedString("║ ║", borderColors),
                topStroke: new FormattedString("═\r\n\r\n═", borderColors),
                rightStroke: new FormattedString("║ ║", borderColors),
                bottomStroke: new FormattedString("═\r\n═", borderColors));
        }

        public static BorderBuilder CreateExtraThickStyle(CharColors borderColors)
        {
            return new BorderBuilder(
                true,
                topLeft: new FormattedString("╔═╦\r\n╠═╬", borderColors),
                topRight: new FormattedString("╦═╗\r\n╬═╣", borderColors),
                bottomLeft: new FormattedString("╠═╬\r\n╚═╩", borderColors),
                bottomRight: new FormattedString("╬═╣\r\n╩═╝", borderColors),
                leftStroke: new FormattedString("║ ║", borderColors),
                topStroke: new FormattedString("═\r\n═", borderColors),
                rightStroke: new FormattedString("║ ║", borderColors),
                bottomStroke: new FormattedString("═\r\n═", borderColors));
        }

        public static BorderBuilder CreateThickStyle(CharColors borderColors)
        {
            return new BorderBuilder(
                true,
                topLeft: new FormattedString("╔", borderColors),
                topRight: new FormattedString("╗", borderColors),
                bottomLeft: new FormattedString("╚", borderColors),
                bottomRight: new FormattedString("╝", borderColors),
                leftStroke: new FormattedString("║", borderColors),
                topStroke: new FormattedString("═", borderColors),
                rightStroke: new FormattedString("║", borderColors),
                bottomStroke: new FormattedString("═", borderColors));
        }

        public static BorderBuilder CreateThinWindowStyle(CharColors borderColors)
        {
            return new BorderBuilder(
                topLeft: new FormattedString("┌\r\n│\r\n├", borderColors),
                topRight: new FormattedString("┐\r\n│\r\n┤", borderColors),
                bottomLeft: new FormattedString("└", borderColors),
                bottomRight: new FormattedString("┘", borderColors),
                leftStroke: new FormattedString("│", borderColors),
                topStroke: new FormattedString("─\r\n\r\n─", borderColors),
                rightStroke: new FormattedString("│", borderColors),
                bottomStroke: new FormattedString("─", borderColors));
        }

        public static BorderBuilder CreateThinStyle(CharColors borderColors)
        {
            return new BorderBuilder(
                true,
                topLeft: new FormattedString("┌", borderColors),
                topRight: new FormattedString("┐", borderColors),
                bottomLeft: new FormattedString("└", borderColors),
                bottomRight: new FormattedString("┘", borderColors),
                leftStroke: new FormattedString("│", borderColors),
                topStroke: new FormattedString("─", borderColors),
                rightStroke: new FormattedString("│", borderColors),
                bottomStroke: new FormattedString("─", borderColors));
        }

        public Border Build(Region outerRegion, FormattedString? namePlate = null)
        {
            return new Border(
                outerRegion,
                this.ignoreNamePlates ? null : namePlate ?? this.defaultNamePlate,
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
