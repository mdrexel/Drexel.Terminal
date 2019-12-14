using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Game.Output
{
    [DebuggerDisplay("{Value,nq}")]
    public sealed class FormattedString
    {
        private const string SpanStart = "<span ";
        private static readonly char[] Space = new char[] { ' ' };

        public FormattedString(
            string value,
            CharColors defaultColors,
            int defaultDelay = 0)
        {
            //// Foo bar <span fg="" bg="">baz bazinga bazongo</span> diddle doodle piddle poodle;
            List<Range> ranges = new List<Range>();
            this.Ranges = Ranges;
            if (value.Length == 0)
            {
                goto exit;
            }

            int startIndex = 0;
            while (true)
            {
                // 1. Find the next '<span '
                //    a. If not found, goto exit
                int startOfNextSpan = value.IndexOf("<span ", startIndex);
                if (startOfNextSpan < 0)
                {
                    goto exit;
                }

                // 2. Find the next >
                //    a. If not found, goto exit
                int endOfNextSpan = value.IndexOf('>', startOfNextSpan);
                if (endOfNextSpan < 0)
                {
                    goto exit;
                }

                int lengthOfNextSpan = endOfNextSpan - startOfNextSpan + 1;

                // 3. Find the next </span>
                //    b. If not found, goto exit
                const string closeOfSpan = "</span>";
                int closeOfNextSpan = value.IndexOf(closeOfSpan, endOfNextSpan);
                int closeOfNextSpanLength = closeOfSpan.Length;
                if (closeOfNextSpan < 0)
                {
                    goto exit;
                }

                // 4. Copy the '<span ...>'
                string copy = value.Substring(startOfNextSpan, lengthOfNextSpan);

                // 5. Remove the '</span>'
                value = value.Remove(closeOfNextSpan, closeOfNextSpanLength);

                // 6. Remove the '<span ...>'
                value = value.Remove(startOfNextSpan, lengthOfNextSpan);

                // 7. Parse the copied '<span ...>',
                //    a. Don't forget to correct for the removal of the '<span ...>', or else the indices will be wrong!
                Range range = ParseSpan(
                    copy,
                    (ushort)startOfNextSpan, // the start is unchanged after the removal
                    (ushort)(closeOfNextSpan - lengthOfNextSpan),
                    defaultColors,
                    defaultDelay);

                // 8. Add the Range to the list
                ranges.Add(range);

                // 9. Goto 1
                startIndex = range.EndIndexExclusive;
            }

        exit:
            LinkedList<Range> linkedList = new LinkedList<Range>(ranges);
            if (ranges.Count == 0)
            {
                linkedList.AddFirst(
                    new Range(
                        0,
                        (ushort)value.Length,
                        defaultColors,
                        defaultDelay));
            }
            else if (linkedList.First.Value.StartIndexInclusive != 0)
            {
                linkedList.AddFirst(
                    new Range(
                        0,
                        linkedList.First.Value.StartIndexInclusive,
                        defaultColors,
                        defaultDelay));
            }

            if (ranges.Count != 0)
            {
                // We already explcitly ensured the first value reaches the next span. Now, we just need to check:
                // * Current range's start == next range's end
                // * If there is no next range, next range's end must equal value's length. Otherwise, add a new range.
                LinkedListNode<Range>? current = linkedList.First;
                while (current != null)
                {
                    LinkedListNode<Range> next = current.Next;
                    if (next == null && current.Value.EndIndexExclusive != value.Length)
                    {
                        linkedList.AddAfter(
                            current,
                            new Range(
                                current.Value.EndIndexExclusive,
                                (ushort)value.Length,
                                defaultColors,
                                defaultDelay));

                        current = null;
                    }
                    else if (next != null && current.Value.EndIndexExclusive != next.Value.StartIndexInclusive)
                    {
                        linkedList.AddAfter(
                            current,
                            new Range(
                                current.Value.EndIndexExclusive,
                                next.Value.StartIndexInclusive,
                                defaultColors,
                                defaultDelay));

                        current = current.Next;
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
            }

            this.Value = value;
            this.Ranges = linkedList.ToList();
            this.ContainsDelays = this.Ranges.Any(x => x.Delay > 0);
        }

        public static FormattedString Empty { get; } = new FormattedString(string.Empty, CharColors.Standard);

        public string Value { get; }

        public IReadOnlyList<Range> Ranges { get; }

        public bool ContainsDelays { get; }

        public static implicit operator FormattedString(string value)
        {
            return new FormattedString(value, CharColors.Standard);
        }

        ////<span fg="Blue" bg="Red" fg=Green>
        private static Range ParseSpan(
            string token,
            ushort startIndexInclusive,
            ushort endIndexExclusive,
            CharColors defaultColors,
            int defaultDelay)
        {
            token = token.Remove(token.Length - 1).Substring(SpanStart.Length);

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string[] pairs = token.Split(Space, StringSplitOptions.RemoveEmptyEntries);
            foreach (string pair in pairs)
            {
                string[] components = pair.Split('=');
                string key = components[0];
                if (!result.TryGetValue(key, out List<string> values))
                {
                    values = new List<string>();
                    result[key] = values;
                }

                if (components.Length == 2)
                {
                    string value = components[1];
                    if (value.Length > 1)
                    {
                        if (value[0] == '"' && value[value.Length - 1] == '"')
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                    }

                    values.Add(value);
                }
            }

            ConsoleColor fg = defaultColors.Foreground;
            ConsoleColor bg = defaultColors.Background;
            int dtime = defaultDelay;
            foreach (KeyValuePair<string, List<string>> kvp in result)
            {
                switch (kvp.Key)
                {
                    case "fg":
                        if (!Enum.TryParse<ConsoleColor>(kvp.Value[0], out fg))
                        {
                            throw new InvalidOperationException(
                                $"Unrecognized color '{kvp.Value[0]}'");
                        }

                        break;
                    case "bg":
                        if (!Enum.TryParse<ConsoleColor>(kvp.Value[0], out bg))
                        {
                            throw new InvalidOperationException(
                                $"Unrecognized color '{kvp.Value[0]}'");
                        }

                        break;
                    case "dtime":
                        dtime = int.Parse(kvp.Value[0]);

                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Unrecognized key '{kvp.Key}'");
                }
            }

            return new Range(
                startIndexInclusive,
                endIndexExclusive,
                new CharColors(fg, bg),
                dtime);
        }
    }
}
