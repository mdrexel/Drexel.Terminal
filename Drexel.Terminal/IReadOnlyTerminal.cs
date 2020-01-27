using Drexel.Terminal.Source;

namespace Drexel.Terminal
{
    /// <summary>
    /// Represents a read-only terminal (sometimes also called a Command Prompt).
    /// </summary>
    /// <seealso cref="ITerminal"/>
    public interface IReadOnlyTerminal
    {
        /// <summary>
        /// Gets the source (inputs) associated with this terminal.
        /// </summary>
        ITerminalSource Source { get; }

        /// <summary>
        /// Gets the title of this terminal.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the height of this terminal.
        /// </summary>
        public ushort Height { get; }

        /// <summary>
        /// Gets the width of this terminal.
        /// </summary>
        public ushort Width { get; }
    }
}
