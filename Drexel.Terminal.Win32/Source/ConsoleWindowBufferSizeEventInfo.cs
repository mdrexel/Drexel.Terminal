using System.Runtime.InteropServices;

namespace Drexel.Terminal.Source.Win32
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ConsoleWindowBufferSizeEventInfo
    {
        [FieldOffset(0)]
        private readonly Coord dwSize;

        /// <summary>
        /// Gets a value indicating the size of the screen buffer, in character cell columns and rows.
        /// </summary>
        public Coord Size => this.dwSize;
    }
}
