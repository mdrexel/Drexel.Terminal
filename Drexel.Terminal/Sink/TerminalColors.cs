using System;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.Terminal.Sink
{
    /// <summary>
    /// Represents a foreground and background color.
    /// </summary>
    public readonly struct TerminalColors
    {
        private static readonly IReadOnlyList<TerminalColor> CachedColors = Enum
            .GetValues(typeof(TerminalColor))
            .Cast<TerminalColor>()
            .ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalColors"/> struct.
        /// </summary>
        /// <param name="foreground">
        /// The foreground (text) color.
        /// </param>
        /// <param name="background">
        /// The background color.
        /// </param>
        public TerminalColors(TerminalColor foreground, TerminalColor background)
        {
            this.Foreground = foreground;
            this.Background = background;
        }

        /// <summary>
        /// Gets the default colors of white text on a black background. Note that this default color may not match the
        /// default color of the terminal in use; it is only provided for convenience, for where it is not desirable to
        /// instantiate a new instance of the <see cref="TerminalColors"/> struct.
        /// </summary>
        public static TerminalColors Default { get; } = new TerminalColors(TerminalColor.White, TerminalColor.Black);

        /// <summary>
        /// Gets the foreground color.
        /// </summary>
        public TerminalColor Foreground { get; }

        /// <summary>
        /// Gets the background color.
        /// </summary>
        public TerminalColor Background { get; }

        /// <summary>
        /// Gets a random combination of colors.
        /// </summary>
        /// <param name="random">
        /// The <see cref="Random"/> to use when selecting colors.
        /// </param>
        /// <returns>
        /// A <see cref="TerminalColors"/> containing random colors.
        /// </returns>
        public static TerminalColors GetRandom(Random random)
        {
            TerminalColor foreground = CachedColors[random.Next(0, CachedColors.Count)];
            TerminalColor background = CachedColors[random.Next(0, CachedColors.Count)];

            return new TerminalColors(foreground, background);
        }
    }
}
