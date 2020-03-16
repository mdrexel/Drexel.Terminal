using System;
using System.Diagnostics;

namespace Drexel.Terminal.Sink
{
    /// <summary>
    /// Represents a character and its associated information.
    /// </summary>
    [DebuggerDisplay("{Character,nq}")]
    public readonly struct CharInfo : IEquatable<CharInfo>
    {
        /// <summary>
        /// The character that should be written.
        /// </summary>
        public readonly char Character;

        /// <summary>
        /// The colors that should be used when writing this character.
        /// </summary>
        public readonly TerminalColors Colors;

        /// <summary>
        /// The delay that should occur after writing this character, in milliseconds.
        /// </summary>
        public readonly ushort Delay;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharInfo"/> struct.
        /// </summary>
        /// <param name="character">
        /// The character that should be written.
        /// </param>
        /// <param name="colors">
        /// The colors that should be used when writing this character.
        /// </param>
        public CharInfo(char character, TerminalColors colors)
            : this(character, colors, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharInfo"/> struct.
        /// </summary>
        /// <param name="character">
        /// The character that should be written.
        /// </param>
        /// <param name="colors">
        /// The colors that should be used when writing this character.
        /// </param>
        /// <param name="delay">
        /// The delay that should occur after writing this character, in milliseconds. A delay of 0 means no delay.
        /// </param>
        public CharInfo(char character, TerminalColors colors, ushort delay)
        {
            this.Character = character;
            this.Colors = colors;
            this.Delay = delay;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal;
        /// otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public static bool operator ==(CharInfo left, CharInfo right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal;
        /// otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="TerminalColors"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public static bool operator !=(CharInfo left, CharInfo right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Converts the specified <see langword="char"/> <paramref name="character"/> to a <see cref="CharInfo"/> with
        /// the default colors and no delay.
        /// </summary>
        /// <param name="character">
        /// The character to convert.
        /// </param>
        public static implicit operator CharInfo(char character)
        {
            return new CharInfo(character, TerminalColors.Default);
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
            if (obj is CharInfo asInfo)
            {
                return this.Equals(asInfo);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this <see cref="CharInfo"/> and the specified <see cref="CharInfo"/>
        /// <paramref name="other"/> are equal.
        /// </summary>
        /// <param name="other">
        /// The <see cref="TerminalColors"/> this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance and <paramref name="other"/> are equal; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(CharInfo other)
        {
            return this.Character == other.Character
                && this.Colors == other.Colors
                && this.Delay == other.Delay;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="CharInfo"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="CharInfo"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 66553;
            unchecked
            {
                hash = (hash * 31) + this.Delay.GetHashCode();
                hash = (hash * 31) + this.Colors.GetHashCode();
                hash = (hash * 31) + this.Character.GetHashCode();
            }

            return hash;
        }
    }
}
