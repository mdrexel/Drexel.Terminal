using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Enigma
{
    [DebuggerDisplay("{Name,nq}")]
    public class Rotor : Disk
    {
        private static readonly IReadOnlyDictionary<RotorModel, string> Names =
            new Dictionary<RotorModel, string>()
            {
                [RotorModel.I] = "I",
                [RotorModel.II] = "II",
                [RotorModel.III] = "III",
                [RotorModel.IV] = "IV",
                [RotorModel.V] = "V",
                [RotorModel.VI] = "VI",
                [RotorModel.VII] = "VII",
                [RotorModel.VIII] = "VIII",
                [RotorModel.Beta] = "Beta",
                [RotorModel.Gamma] = "Gamma"
            };

        private static readonly IReadOnlyDictionary<RotorModel, string> Mappings =
            new Dictionary<RotorModel, string>()
            {
                [RotorModel.I] = "ekmflgdqvzntowyhxuspaibrcj",
                [RotorModel.II] = "ajdksiruxblhwtmcqgznpyfvoe",
                [RotorModel.III] = "bdfhjlcprtxvznyeiwgakmusqo",
                [RotorModel.IV] = "esovpzjayquirhxlnftgkdcmwb",
                [RotorModel.V] = "vzbrgityupsdnhlxawmjqofeck",
                [RotorModel.VI] = "jpgvoumfyqbenhzrdkasxlictw",
                [RotorModel.VII] = "nzjhgrcxmyswboufaivlpekqdt",
                [RotorModel.VIII] = "fkqhtlxocbjspdzramewniuygv",
                [RotorModel.Beta] = "leyjvcnixwpbqmdrtakzgfuhos",
                [RotorModel.Gamma] = "fsokanuerhmbtiycwlqpzxvgjd"
            };

        private static readonly IReadOnlyDictionary<RotorModel, string> Turnovers =
            new Dictionary<RotorModel, string>()
            {
                [RotorModel.I] = "q",
                [RotorModel.II] = "e",
                [RotorModel.III] = "v",
                [RotorModel.IV] = "j",
                [RotorModel.V] = "z",
                [RotorModel.VI] = "zm",
                [RotorModel.VII] = "zm",
                [RotorModel.VIII] = "zm",
                [RotorModel.Beta] = "",
                [RotorModel.Gamma] = ""
            };

        private int initialPosition;
        private int ringPosition;

        public Rotor(RotorModel rotorModel)
            : base(Names[rotorModel], Mappings[rotorModel], Turnovers[rotorModel])
        {
            this.initialPosition = 0;
            this.ringPosition = 0;
        }

        public int InitialPosition
        {
            get => this.initialPosition;
            set
            {
                if (value < 0 || value > 25)
                {
                    throw new ArgumentException(
                        "Initial position must be between 0 and 25.",
                        nameof(value));
                }

                this.initialPosition = value;
            }
        }

        public int RingPosition
        {
            get => this.ringPosition;
            set
            {
                if (value < 0 || value > 25)
                {
                    throw new ArgumentException(
                        "Ring position must be between 0 and 25.",
                        nameof(value));
                }

                this.ringPosition = value;
            }
        }
    }
}
