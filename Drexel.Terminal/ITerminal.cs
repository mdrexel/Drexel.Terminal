using Drexel.Terminal.Sink;

namespace Drexel.Terminal
{
    /// <summary>
    /// Represents a terminal (sometimes also called a Command Prompt).
    /// </summary>
    public interface ITerminal : IReadOnlyTerminal
    {
        /// <summary>
        /// Gets the sink (outputs) associated with this terminal.
        /// </summary>
        ITerminalSink Sink { get; }

        /// <summary>
        /// Gets or sets the title of this terminal.
        /// </summary>
        new string Title { get; set; }

        /// <summary>
        /// Gets or sets the height of this terminal.
        /// </summary>
        new ushort Height { get; set; }

        /// <summary>
        /// Gets or sets the width of this terminal.
        /// </summary>
        new ushort Width { get; set; }
    }
}
