using System;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents a combination of terminal modifier keys.
    /// </summary>
    [Flags]
    public enum TerminalModifiers
    {
        /// <summary>
        /// The left or right "alt" key.
        /// </summary>
        Alt = 0x1,

        /// <summary>
        /// The left or right "shift" key.
        /// </summary>
        Shift = 0x2,

        /// <summary>
        /// The left or right "control" key.
        /// </summary>
        Control = 0x4
    }
}
