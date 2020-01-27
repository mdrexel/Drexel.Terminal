using System;

namespace Drexel.Terminal.Source.Win32
{
    [Flags]
    internal enum ConsoleMouseEventType
    {
        /// <summary>
        /// A mouse button was pressed or released
        /// </summary>
        MouseButton = 0,

        /// <summary>
        /// A change in mouse position occurred
        /// </summary>
        MouseMoved = 1,

        /// <summary>
        /// The second click of a double-click operation occurred.
        /// </summary>
        DoubleClick = 2,

        /// <summary>
        /// The vertical mouse wheel was rolled.
        /// </summary>
        MouseWheeled = 4,

        /// <summary>
        /// The horizontal mouse wheel was rolled.
        /// </summary>
        MouseHWheeled = 8
    }
}
