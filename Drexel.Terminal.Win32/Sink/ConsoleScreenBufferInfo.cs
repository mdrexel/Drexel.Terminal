using System.Runtime.InteropServices;
using Drexel.Terminal.Win32;

namespace Drexel.Terminal.Sink.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ConsoleScreenBufferInfo
    {
        [Explicit] private readonly Coord dwSize;
        [Explicit] private readonly Coord dwCursorPosition;
        [Explicit] private readonly ushort wAttributes;
        [Explicit] private readonly Rectangle srWindow;
        [Explicit] private readonly Coord dwMaximumWindowSize;

        public Coord Size => this.dwSize;

        public Coord CursorPosition => this.dwCursorPosition;

        public Rectangle BufferWindow => this.srWindow;

        public Coord MaximumWindowSize => this.dwMaximumWindowSize;
    }
}
