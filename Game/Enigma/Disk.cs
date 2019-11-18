using System.Collections.Generic;
using System.Linq;
using Drexel.Collections.Generic;

namespace Game.Enigma
{
    public abstract class Disk
    {
        private protected Disk(
            string name,
            string mappings,
            string turnovers)
        {
            this.Name = name;
            this.Mappings = mappings.ToList();
            this.Turnovers = new SetAdapter<char>(new HashSet<char>(turnovers));
        }

        public string Name { get; }

        public IReadOnlyList<char> Mappings { get; }

        public IReadOnlyInvariantSet<char> Turnovers { get; }
    }
}
