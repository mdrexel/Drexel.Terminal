using System.Runtime.InteropServices;

namespace Drexel.Terminal.Source.Win32
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ConsoleMenuEventInfo
    {
        [FieldOffset(0)]
        private readonly int dwCommandId;

        /// <summary>
        /// The ID of the menu command.
        /// </summary>
        public int CommandId => this.dwCommandId;
    }
}
