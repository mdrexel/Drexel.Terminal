using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Source;

namespace Drexel.Terminal.Layout.Layouts.Symbols
{
    public class TextField : Symbol
    {
        private readonly object lockObject;
        private readonly Observable<TextFieldKeyAcceptedEventArgs> onKeyAccepted;
        private readonly Observable<string> onComplete;
        private readonly List<CharInfo> characters;

        private TerminalColors colors;
        private char background;
        private int scroll;

        public TextField(
            IResizeableRegion region,
            string name,
            TerminalColors colors,
            char background = '░')
            : base(region, name)
        {
            this.lockObject = new object();
            this.onKeyAccepted = new Observable<TextFieldKeyAcceptedEventArgs>();
            this.onComplete = new Observable<string>();
            this.colors = colors;
            this.background = background;

            this.characters = new List<CharInfo>();

            this.scroll = 0;

            this.AddDisposable(
                this.Region.OnChanged.Subscribe(
                    new Observer<RegionChangedEventArgs>(
                        x =>
                        {
                            lock (this.lockObject)
                            {
                                this.RequestRedraw(x.BeforeChange, x.AfterChange);
                            }
                        })));
        }

        public IObservable<TextFieldKeyAcceptedEventArgs> OnKeyAccepted => this.onKeyAccepted;

        public IObservable<string> OnComplete => this.onComplete;

        public override bool CanBeFocused => true;

        public override bool CapturesTabKey => true;

        public TerminalColors Colors
        {
            get => this.colors;
            set
            {
                lock (this.lockObject)
                {
                    this.colors = value;
                }

                this.RequestRedraw();
            }
        }

        public char Background
        {
            get => this.Background;
            set
            {
                lock (this.lockObject)
                {
                    this.background = value;
                    this.RequestRedraw();
                }
            }
        }

        public override void FocusChanged(bool focused)
        {
            if (focused)
            {
                this.RequestRedraw();
            }
        }

        public override void KeyPressed(TerminalKeyInfo keyInfo)
        {
            lock (this.lockObject)
            {
                if (keyInfo.Key == TerminalKey.Enter)
                {
                    this.onComplete.Next(new string(this.characters.Select(x => x.Character).ToArray()));
                }
                else if (keyInfo.Key == TerminalKey.Backspace && this.characters.Count > 0)
                {
                    this.characters.RemoveAt(this.characters.Count - 1);
                    if (this.characters.Count > this.Region.ActualWidth)
                    {
                        this.scroll--;
                    }
                    else
                    {
                        this.scroll = 0;
                    }
                }
                else if (keyInfo.Key == TerminalKey.Tab)
                {
                    TerminalKeyInfo spaceInfo =
                        new TerminalKeyInfo(
                            ' ',
                            TerminalKey.Spacebar,
                            (keyInfo.Modifiers & TerminalModifiers.Shift) != 0,
                            (keyInfo.Modifiers & TerminalModifiers.Alt) != 0,
                            (keyInfo.Modifiers & TerminalModifiers.Control) != 0);
                    this.AcceptKey(spaceInfo);
                    this.AcceptKey(spaceInfo);
                    this.AcceptKey(spaceInfo);
                    this.AcceptKey(spaceInfo);
                }
                else if (char.IsWhiteSpace(keyInfo.KeyChar)
                    || char.IsLetterOrDigit(keyInfo.KeyChar)
                    || char.IsPunctuation(keyInfo.KeyChar)
                    || char.IsSymbol(keyInfo.KeyChar))
                {
                    this.AcceptKey(keyInfo);
                }
                else if (keyInfo.Key == TerminalKey.LeftArrow)
                {
                    this.scroll--;
                    if (scroll < 0)
                    {
                        this.scroll = 0;
                    }
                }
                else if (keyInfo.Key == TerminalKey.RightArrow)
                {
                    this.scroll++;
                    if (scroll >= this.characters.Count)
                    {
                        this.scroll = Math.Max(0, this.characters.Count - 1);
                    }
                }
                else if (keyInfo.Key == TerminalKey.UpArrow)
                {
                    this.scroll -= this.Region.ActualWidth;
                    if (scroll < 0)
                    {
                        this.scroll = 0;
                    }
                }
                else if (keyInfo.Key == TerminalKey.DownArrow)
                {
                    this.scroll += this.Region.ActualWidth;
                    if (scroll >= this.characters.Count)
                    {
                        this.scroll = Math.Max(0, this.characters.Count - 1);
                    }
                }

                this.RequestRedraw();
            }
        }

        public void Clear()
        {
            lock (this.lockObject)
            {
                this.characters.Clear();
                this.RequestRedraw();
            }
        }

        public override void Draw(ITerminalSink sink, Rectangle window)
        {
            if (!this.Region.Overlaps(window))
            {
                return;
            }

            lock (this.lockObject)
            {
                CharInfo[,] array = new CharInfo[this.Region.ActualHeight, this.Region.ActualWidth];

                int index = scroll;
                Coord? inflection = null;
                for (short y = 0; y < this.Region.ActualHeight; y++)
                {
                    for (short x = 0; x < this.Region.ActualWidth; x++, index++)
                    {
                        if (index < this.characters.Count)
                        {
                            array[y, x] = this.characters[index];
                        }
                        else
                        {
                            if (!inflection.HasValue)
                            {
                                inflection = new Coord(x, y);
                            }

                            array[y, x] = new CharInfo(this.background, this.Colors);
                        }
                    }
                }

                if (!inflection.HasValue)
                {
                    inflection = this.Region.BottomRight;
                }
                else
                {
                    inflection = inflection.Value + this.Region.TopLeft;
                }

                sink.CursorPosition = inflection.Value;
                window = window - this.Region.TopLeft;
                window = new Rectangle(window.Left, window.Top, (short)(window.Right + 1), (short)(window.Bottom + 1));

                sink.Write(array, this.Region.TopLeft, window);
            }
        }

        private void AcceptKey(TerminalKeyInfo keyInfo)
        {
            TextFieldKeyAcceptedEventArgs args = new TextFieldKeyAcceptedEventArgs(
                keyInfo,
                this.characters.Count,
                this.colors);

            this.onKeyAccepted.Next(args);

            if (!args.Reject)
            {
                this.characters.Add(new CharInfo(args.AcceptedKey.KeyChar, args.Colors));
                if (this.characters.Count > this.Region.ActualWidth - 1)
                {
                    this.scroll = this.characters.Count - this.Region.ActualWidth + 1;
                }
                else
                {
                    this.scroll = 0;
                }
            }
        }
    }
}
