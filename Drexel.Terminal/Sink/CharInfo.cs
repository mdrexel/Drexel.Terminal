using System.Diagnostics;

namespace Drexel.Terminal.Sink
{
    /// <summary>
    /// Represents a character and its associated information.
    /// </summary>
    [DebuggerDisplay("{Character,nq}")]
    public readonly struct CharInfo
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
    }
}
