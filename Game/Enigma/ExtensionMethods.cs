using System.Collections.Generic;

namespace Game.Enigma
{
    internal static class ExtensionMethods
    {
        public static int IndexOf<T>(
            this IEnumerable<T> enumerable,
            T value,
            IEqualityComparer<T>? comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
            {
                for (int index = 0; enumerator.MoveNext(); index++)
                {
                    if (comparer.Equals(value, enumerator.Current))
                    {
                        return index;
                    }
                }
            }

            return -1;
        }
    }
}
