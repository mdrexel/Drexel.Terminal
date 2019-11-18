using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Enigma
{

    public class Plugboard : IReadOnlyCollection<KeyValuePair<char, char>>
    {
        private readonly Dictionary<char, char> mappings;

        public Plugboard()
        {
            this.mappings = new Dictionary<char, char>();
        }

        public char this[char a, bool inverted]
        {
            get
            {
                if (!inverted)
                {
                    if (this.mappings.TryGetValue(a, out char result))
                    {
                        return result;
                    }
                    else
                    {
                        return a;
                    }
                }
                else
                {
                    IReadOnlyList<KeyValuePair<char, char>> found =
                        this.mappings.Where(x => x.Value == a).ToArray();
                    if (found.Count > 0)
                    {
                        return found[0].Key;
                    }
                    else
                    {
                        return a;
                    }
                }
            }
        }

        public int Count => this.mappings.Count;

        public bool Add(char a, char b)
        {
            if (this.mappings.ContainsKey(a))
            {
                return false;
            }

            this.mappings[a] = b;
            this.mappings[b] = a;
            return true;
        }

        public bool Contains(char a)
        {
            return this.mappings.ContainsKey(a);
        }

        public IEnumerator<KeyValuePair<char, char>> GetEnumerator()
        {
            return this.mappings.GetEnumerator();
        }

        public bool Remove(char a)
        {
            if (this.mappings.TryGetValue(a, out char existing))
            {
                this.mappings.Remove(a);
                this.mappings.Remove(existing);
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
