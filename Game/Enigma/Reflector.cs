using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Enigma
{
    [DebuggerDisplay("{Name,nq}")]
    public class Reflector : Disk
    {
        private static readonly IReadOnlyDictionary<ReflectorModel, string> Names =
            new Dictionary<ReflectorModel, string>()
            {
                [ReflectorModel.UkwA] = "UKW A",
                [ReflectorModel.UkwB] = "UKW B",
                [ReflectorModel.UkwC] = "UKW C",
                [ReflectorModel.UkwBThin] = "UKW B Thin",
                [ReflectorModel.UkwCThin] = "UKW C Thin"
            };

        private static readonly IReadOnlyDictionary<ReflectorModel, string> Mappings =
            new Dictionary<ReflectorModel, string>()
            {
                [ReflectorModel.UkwA] = "ejmzalyxvbwfcrquontspikhgd",
                [ReflectorModel.UkwB] = "yruhqsldpxngokmiebfzcwvjat",
                [ReflectorModel.UkwC] = "fvpjiaoyedrzxwgctkuqsbnmhl",
                [ReflectorModel.UkwBThin] = "enkqauywjicopblmdxzvfthrgs",
                [ReflectorModel.UkwCThin] = "rdobjntkvehmlfcwzaxgyipsuq"
            };

        public Reflector(ReflectorModel reflectorModel)
            : base(Names[reflectorModel], Mappings[reflectorModel], string.Empty)
        {
        }
    }
}
