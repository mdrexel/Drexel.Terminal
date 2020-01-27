using System;

namespace Drexel.Terminal.Source.Win32
{
    [Flags]
    internal enum ConsoleMouseButtonState : int
    {
        /// <summary>
        /// The leftmost mouse button.
        /// </summary>
        FromLeft1stButtonPressed = 1,

        /// <summary>
        /// The rightmost mouse button.
        /// </summary>
        RightMostButtonPressed = 2,

        /// <summary>
        /// The second button from the left
        /// </summary>
        FromLeft2ndMouseButtonPressed = 4,

        /// <summary>
        /// The third button from the left
        /// </summary>
        FromLeft3rdMouseButtonPressed = 8,

        /// <summary>
        /// The fourth button from the left.
        /// </summary>
        FromLeft4thMouseButtonPressed = 16,

        /// <summary>
        /// For mouse wheel events, if this flag is set, the wheel was scrolled down. If cleared, the wheel was
        /// scrolled up.
        /// </summary>
        ScrollDown = unchecked((int)0xFF000000)
    }
}
