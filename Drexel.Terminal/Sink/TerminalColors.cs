using System;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.Terminal.Sink
{
    /// <summary>
    /// Represents a foreground and background color.
    /// </summary>
    public readonly struct TerminalColors : IEquatable<TerminalColors>
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
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same
        /// foreground and background colors; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same foreground and
        /// background colors; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(TerminalColors left, TerminalColors right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not have the same
        /// foreground and background colors; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not have the same
        /// foreground and background colors; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(TerminalColors left, TerminalColors right)
        {
            return !left.Equals(right);
        }

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

        /// <summary>
        /// Determines whether this instance and the specified <see cref="object"/> <paramref name="obj"/> are equal.
        /// </summary>
        /// <param name="obj">
        /// The object this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance is equal to the specified <paramref name="obj"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is TerminalColors other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this <see cref="TerminalColors"/> and the specified <see cref="TerminalColors"/>
        /// <paramref name="other"/> have the same foreground and background colors.
        /// </summary>
        /// <param name="other">
        /// The <see cref="TerminalColors"/> this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance and <paramref name="other"/> have the same foreground and
        /// background colors; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(TerminalColors other)
        {
            return this.Foreground == other.Foreground
                && this.Background == other.Background;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="TerminalColors"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="TerminalColors"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 55381;
            unchecked
            {
                hash = (hash * 31) + this.Background.GetHashCode();
                hash = (hash * 31) + this.Foreground.GetHashCode();
            }

            return hash;
        }
    }
}
