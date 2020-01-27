using System.Runtime.InteropServices;

namespace Drexel.Terminal.Source.Win32
{
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ConsoleFocusEventInfo
    {
        [FieldOffset(0)]
        private readonly uint bSetFocus;

        /// <summary>
        /// Gets a value indicating whether focus is gained or lost.
        /// </summary>
        public bool SetFocus => this.bSetFocus != 0;
    }
}
