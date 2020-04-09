using Drexel.Terminal.Sink;
using Drexel.Terminal.Source;

namespace Drexel.Terminal.Layout.Layouts.Symbols
{
    public class TextFieldKeyAcceptedEventArgs
    {
        public TextFieldKeyAcceptedEventArgs(
            TerminalKeyInfo acceptedKey,
            int indexInField,
            TerminalColors colors)
        {
            this.AcceptedKey = acceptedKey;
            this.IndexInField = indexInField;
            this.Colors = colors;
            this.Reject = false;
        }

        public TerminalKeyInfo AcceptedKey { get; set; }

        public int IndexInField { get; }

        public TerminalColors Colors { get; set; }

        public bool Reject { get; set; }
    }
}
