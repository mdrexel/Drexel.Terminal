using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Collections.Generic;

namespace Game.Enigma.Models
{
    public class EnigmaM3
    {
        private readonly IReadOnlyList<Rotor> rotors;

        private long charactersTranslated;

        public EnigmaM3(
            RotorModel rotor1,
            RotorModel rotor2,
            RotorModel rotor3,
            ReflectorModel reflector)
        {
            this.Rotor1 = new Rotor(rotor1);
            this.Rotor2 = new Rotor(rotor2);
            this.Rotor3 = new Rotor(rotor3);
            this.Reflector = new Reflector(reflector);

            this.EntryRotor = EntryRotor.EtwABCDEF;
            this.Plugboard = new Plugboard();

            this.rotors = new Rotor[] { this.Rotor1, this.Rotor2, this.Rotor3 };
            this.charactersTranslated = 0;
        }

        public EntryRotor EntryRotor { get; }

        public Rotor Rotor1 { get; }

        public Rotor Rotor2 { get; }

        public Rotor Rotor3 { get; }

        public Reflector Reflector { get; }

        public Plugboard Plugboard { get; }

        public char PerformTranslate(char content, out bool shouldBeFollowedBySpace)
        {
            // Collect slots, rotors, positions and rings for current translation
            int[] positions = new int[]
            {
                this.Rotor1.InitialPosition,
                this.Rotor2.InitialPosition,
                this.Rotor3.InitialPosition
            };
            int[] rings = new int[]
            {
                this.Rotor1.RingPosition,
                this.Rotor2.RingPosition,
                this.Rotor3.RingPosition
            };

            static int mod(int x, int modulo)
            {
                modulo = Math.Abs(modulo);
                int buffer = x % modulo;
                return buffer < 0 ? buffer + modulo : buffer;
            }

            static int map(
                int charIndex,
                IReadOnlyList<char> mappings,
                int position,
                int ringSetting,
                bool inverted)
            {
                // Apply ring setting
                int offset = mod(position - ringSetting, 26);

                // Map character
                int indexAfterOffset = mod(charIndex + offset, 26);

                int mappedCharacter = !inverted
                  ? mappings[indexAfterOffset] - 97
                  : mappings.IndexOf((char)(97 + indexAfterOffset));

                int revertAppliedOffset = mod(mappedCharacter - offset, 26);

                return revertAppliedOffset;
            }

            static bool rotorAtTurnover(Rotor rotor, int position)
            {
                char positionChar = (char)(97 + mod(position, 26));
                return rotor.Turnovers.Contains(positionChar);
            }

            int value = content;

            if (value >= 65 && value <= 90)
            {
                // Read uppercase character
                value = value - 65;
            }
            else if (value >= 97 && value <= 122)
            {
                // Read lowercase character
                value = value - 97;
            }
            else if (value == ' ')
            {
                // Preserve spaces, we'll strip them later
                shouldBeFollowedBySpace = false;
                return ' ';
            }
            else
            {
                // This is a foreign character
                shouldBeFollowedBySpace = (++this.charactersTranslated % 5) == 0;
                return '*';
            }

            // Engage lever driven wheel-turnover mechanism
            {
                bool stepped = false;
                int i = 1;
                while (!stepped && i < rotors.Count)
                {
                    stepped = rotorAtTurnover(rotors[i], positions[i]);
                    if (stepped)
                    {
                        // Shift current rotor, if it is not the last one (rotated later)
                        if (i != rotors.Count - 1)
                        {
                            positions[i]++;
                        }

                        // Shift rotor on its left
                        positions[i - 1]++;
                    }

                    i++;
                }
            }

            // Shift fast rotor at every turn
            positions[positions.Length - 1]++;

            // Wire characters through the plugboard
            value = this.Plugboard[(char)value, false];

            // Through the entry
            value = map(
                value,
                this.EntryRotor.Mappings,
                0,
                0,
                false);

            // Through the rotors (from right to left)
            for (int rotor = rotors.Count - 1; rotor >= 0; rotor--)
            {
                value = map(
                    value,
                    rotors[rotor].Mappings,
                    positions[rotor],
                    rings[rotor],
                    false);
            }

            // Through the reflector
            value = map(
                value,
                this.Reflector.Mappings,
                0,
                0,
                false);

            // Through the inverted rotors (from left to right)
            for (int rotor = 0; rotor < rotors.Count; rotor++)
            {
                value = map(
                    value,
                    rotors[rotor].Mappings,
                    positions[rotor],
                    rings[rotor],
                    true);
            }

            // Through the inverted entry
            value = map(
                value,
                this.EntryRotor.Mappings,
                0,
                0,
                true);

            // Through the inverted plugboard
            value = this.Plugboard[(char)value, true];

            this.Rotor1.InitialPosition = positions[0];
            this.Rotor2.InitialPosition = positions[1];
            this.Rotor3.InitialPosition = positions[2];

            // Translate char index back to code point and return it
            shouldBeFollowedBySpace = (++this.charactersTranslated % 5) == 0;
            return (char)(value + 97);
        }

        public string PerformTranslate(string content, out bool shouldBeFollowedBySpace)
        {
            // Collect slots, rotors, positions and rings for current translation
            IReadOnlyList<Rotor> rotors = new Rotor[]
            {
                this.Rotor1,
                this.Rotor2,
                this.Rotor3
            };
            int[] positions = new int[]
            {
                this.Rotor1.InitialPosition,
                this.Rotor2.InitialPosition,
                this.Rotor3.InitialPosition
            };
            int[] rings = new int[]
            {
                this.Rotor1.RingPosition,
                this.Rotor2.RingPosition,
                this.Rotor3.RingPosition
            };

            static int mod(int x, int modulo)
            {
                modulo = Math.Abs(modulo);
                int buffer = x % modulo;
                return buffer < 0 ? buffer + modulo : buffer;
            }

            static int map(
                int charIndex,
                IReadOnlyList<char> mappings,
                int position,
                int ringSetting,
                bool inverted)
            {
                // Apply ring setting
                int offset = mod(position - ringSetting, 26);

                // Map character
                int indexAfterOffset = mod(charIndex + offset, 26);

                int mappedCharacter = !inverted
                  ? mappings[indexAfterOffset] - 97
                  : mappings.IndexOf((char)(97 + indexAfterOffset));

                int revertAppliedOffset = mod(mappedCharacter - offset, 26);

                return revertAppliedOffset;
            }

            static bool rotorAtTurnover(Rotor rotor, int position)
            {
                char positionChar = (char)(97 + mod(position, 26));
                return rotor.Turnovers.Contains(positionChar);
            }

            // Go through each content code point
            char[] result = content
                .Select(
                    (codePoint, index) =>
                    {
                        int value = 0;

                        if (codePoint >= 65 && codePoint <= 90)
                        {
                            // Read uppercase character
                            value = codePoint - 65;
                        }
                        else if (codePoint >= 97 && codePoint <= 122)
                        {
                            // Read lowercase character
                            value = codePoint - 97;
                        }
                        else if (codePoint == ' ')
                        {
                            // Preserve spaces, we'll strip them later
                            return ' ';
                        }
                        else
                        {
                            // This is a foreign character
                            return '*';
                        }

                        // Engage lever driven wheel-turnover mechanism
                        {
                            bool stepped = false;
                            int i = 1;
                            while (!stepped && i < rotors.Count)
                            {
                                stepped = rotorAtTurnover(rotors[i], positions[i]);
                                if (stepped)
                                {
                                    // Shift current rotor, if it is not the last one (rotated later)
                                    if (i != rotors.Count - 1)
                                    {
                                        positions[i]++;
                                    }

                                    // Shift rotor on its left
                                    positions[i - 1]++;
                                }

                                i++;
                            }
                        }

                        // Shift fast rotor at every turn
                        positions[positions.Length - 1]++;

                        // Wire characters through the plugboard
                        value = this.Plugboard[(char)value, false];

                        // Through the entry
                        value = map(
                            value,
                            this.EntryRotor.Mappings,
                            0,
                            0,
                            false);

                        // Through the rotors (from right to left)
                        for (int rotor = rotors.Count - 1; rotor >= 0; rotor--)
                        {
                            value = map(
                                value,
                                rotors[rotor].Mappings,
                                positions[rotor],
                                rings[rotor],
                                false);
                        }

                        // Through the reflector
                        value = map(
                            value,
                            this.Reflector.Mappings,
                            0,
                            0,
                            false);

                        // Through the inverted rotors (from left to right)
                        for (int rotor = 0; rotor < rotors.Count; rotor++)
                        {
                            value = map(
                                value,
                                rotors[rotor].Mappings,
                                positions[rotor],
                                rings[rotor],
                                true);
                        }

                        // Through the inverted entry
                        value = map(
                            value,
                            this.EntryRotor.Mappings,
                            0,
                            0,
                            true);

                        // Through the inverted plugboard
                        value = this.Plugboard[(char)value, true];

                        // Translate char index back to code point and return it
                        return (char)(value + 97);
                    })
                .ToArray();

            this.Rotor1.InitialPosition = positions[0];
            this.Rotor2.InitialPosition = positions[1];
            this.Rotor3.InitialPosition = positions[2];

            StringBuilder builder = new StringBuilder();
            int counter = 0;
            int written = 0;
            long real = 0;
            while (counter < result.Length)
            {
                char value = result[counter];
                if (value != ' ')
                {
                    builder.Append(result[counter]);
                    real++;
                }

                if (++written == 5)
                {
                    written = 0;
                    builder.Append(' ');
                }

                counter++;
            }

            this.charactersTranslated += real;
            shouldBeFollowedBySpace = (real % 5) == 0;
            return builder.ToString();
        }
    }
}
