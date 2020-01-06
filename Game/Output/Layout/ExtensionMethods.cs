using System.Collections.Generic;

namespace Game.Output.Layout
{
    internal static class ExtensionMethods
    {
        public static IEnumerable<T> Reverse<T>(this LinkedList<T> list)
        {
            LinkedListNode<T> last = list.Last;
            while (!object.ReferenceEquals(last, null))
            {
                yield return last.Value;
                last = last.Previous;
            }
        }
    }
}
