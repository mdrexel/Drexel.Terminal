using System.Runtime.InteropServices;

namespace Drexel.Terminal.Source.Win32
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ConsoleMouseEventInfo
    {
        [FieldOffset(0)] private readonly Coord dwMousePosition;
        [FieldOffset(4)] private readonly int dwButtonState;
        [FieldOffset(8)] private readonly int dwControlKeyState;
        [FieldOffset(12)] private readonly int dwEventFlags;

        public Coord MousePosition => this.dwMousePosition;

        public ConsoleMouseButtonState ButtonState => (ConsoleMouseButtonState)this.dwButtonState;

        public ConsoleControlKeyState ControlKeyState => (ConsoleControlKeyState)this.dwControlKeyState;

        public ConsoleMouseEventType EventFlags => (ConsoleMouseEventType)this.dwEventFlags;
    }
}
