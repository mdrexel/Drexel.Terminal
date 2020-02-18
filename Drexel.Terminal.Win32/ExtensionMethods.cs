using System.Runtime.CompilerServices;
using Drexel.Terminal.Sink;

namespace Drexel.Terminal.Win32
{
    internal static class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ConsoleCharAttributes ToCharAttributes(this TerminalColors colors)
        {
            return (ConsoleCharAttributes)(((ushort)colors.Background << 4) | (ushort)colors.Foreground);
        }
    }
}
