using System;
using System.Runtime.InteropServices;

namespace Drexel.Terminal.Source.Win32
{

    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ConsoleKeyEventInfo
    {
        [FieldOffset(0)] private readonly int bKeyDown;
        [FieldOffset(4)] private readonly short wRepeatCount;
        [FieldOffset(6)] private readonly short wVirtualKeyCode;
        [FieldOffset(8)] private readonly short wVirtualScanCode;
        [FieldOffset(10)] private readonly char cUnicodeChar;
        [FieldOffset(10)] private readonly short wUnicodeChar;
        [FieldOffset(10)] private readonly byte bAsciiChar;
        [FieldOffset(12)] private readonly int dwControlKeyState;

        /// <summary>
        /// Gets a value indicating whether this is a key down or key up event.
        /// </summary>
        public bool KeyDown => this.bKeyDown != 0;

        /// <summary>
        /// Gets a value indicating that a key is being held down.
        /// </summary>
        public short RepeatCount => this.wRepeatCount;

        /// <summary>
        /// Gets a value that identifies the given key in a device-independent manner.
        /// </summary>
        public ConsoleKey VirtualKeyCode => (ConsoleKey)this.wVirtualKeyCode;

        /// <summary>
        /// Gets the hardware-dependent virtual scan code.
        /// </summary>
        public short VirtualScanCode => this.wVirtualScanCode;

        /// <summary>
        /// Gets the Unicode character for this key event.
        /// </summary>
        public char UnicodeChar => this.cUnicodeChar;

        /// <summary>
        /// Gets the ASCII key for this key event.
        /// </summary>
        public byte AsciiChar => this.bAsciiChar;

        /// <summary>
        /// Gets a value specifying the control key state for this key event.
        /// </summary>
        public ConsoleControlKeyState ControlKeyState => (ConsoleControlKeyState)this.dwControlKeyState;
    }
}
