using System;
using System.Collections.Generic;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Source;

namespace Drexel.Terminal.Layout.Layouts.Symbols
{
    public class TextField : Symbol
    {
        private readonly Observable<string> onComplete;
        private readonly List<char> characters;

        private TerminalColors colors;

        public TextField(
            IResizeableRegion region,
            string name,
            TerminalColors colors,
            char background)
            : base(region, name)
        {
            this.onComplete = new Observable<string>();
            this.characters = new List<char>();
        }

        public IObservable<string> OnComplete => this.onComplete;

        public override bool CanBeFocused => true;

        public TerminalColors Colors
        {
            get => this.colors;
            set
            {
                this.colors = value;
                this.RequestRedraw();
            }
        }

        public override void KeyPressed(TerminalKeyInfo keyInfo)
        {
            if (keyInfo.Key == TerminalKey.Enter)
            {
                this.onComplete.Next(new string(this.characters.ToArray()));
            }
            else if (keyInfo.Key == TerminalKey.Backspace && this.characters.Count > 0)
            {
                this.characters.RemoveAt(this.characters.Count - 1);
            }
            else if (char.IsWhiteSpace(keyInfo.KeyChar)
                || char.IsLetterOrDigit(keyInfo.KeyChar)
                || char.IsPunctuation(keyInfo.KeyChar))
            {
                this.characters.Add(keyInfo.KeyChar);
            }
        }

        public void Clear()
        {
            this.characters.Clear();
            this.RequestRedraw();
        }

        public override void Draw(ITerminalSink sink, Rectangle window)
        {
            if (!this.Region.Overlaps(window))
            {
                return;
            }

            CharInfo[,] array = new CharInfo[this.Region.ActualHeight, this.Region.ActualWidth];
            int index = 0;
            for (int y = 0; y < this.Region.ActualHeight; y++)
            {
                for (int x = 0; x < this.Region.ActualWidth; x++, index++)
                {
                    if (index < this.characters.Count)
                    {
                        array[y, x] = new CharInfo(this.characters[index], this.Colors);
                    }
                    else
                    {
                        array[y, x] = new CharInfo('▒', this.Colors);
                    }
                }
            }

            sink.Write(array, this.Region.TopLeft, window);
        }
    }
}
