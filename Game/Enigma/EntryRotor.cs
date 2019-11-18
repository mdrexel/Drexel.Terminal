using System.Diagnostics;

namespace Game.Enigma
{
    [DebuggerDisplay("{Name,nq}")]
    public class EntryRotor : Disk
    {
        private EntryRotor(string name, string mappings, string turnovers)
            : base(name, mappings, turnovers)
        {
        }

        public static EntryRotor EtwABCDEF { get; } = new EntryRotor("ETW-ABCDEF", "abcdefghijklmnopqrstuvwxyz", "");

        public static EntryRotor EtwQWERTZ { get; } = new EntryRotor("ETW-QWERTZ", "jwulcmnohpqzyxiradkegvbtsf", "");
    }
}
