using System.Runtime.InteropServices;

namespace Drexel.Terminal.Source.Win32
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ConsoleInputEventInfo
    {
        [FieldOffset(0)] public readonly ConsoleInputEventType EventType;

        /// <summary>
        /// Key event information if this is a keyboard event.
        /// </summary>
        [FieldOffset(4)] public readonly ConsoleKeyEventInfo KeyEvent;

        /// <summary>
        /// Mouse event information if this is a mouse event.
        /// </summary>
        [FieldOffset(4)] public readonly ConsoleMouseEventInfo MouseEvent;

        /// <summary>
        /// Window buffer size information if this is a window buffer size event.
        /// </summary>
        [FieldOffset(4)] public readonly ConsoleWindowBufferSizeEventInfo WindowBufferSizeEvent;

        /// <summary>
        /// Menu event information if this is a menu event.
        /// </summary>
        [FieldOffset(4)] public readonly ConsoleMenuEventInfo MenuEvent;

        /// <summary>
        /// Focus event information if this is a focus event.
        /// </summary>
        [FieldOffset(4)] public readonly ConsoleFocusEventInfo FocusEvent;
    }
}
