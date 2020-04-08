////using System;
////using System.Collections.Generic;
////using Drexel.Terminal.Internals;
////using Drexel.Terminal.Sink;

////namespace Drexel.Terminal.Layout.Layouts.Symbols
////{
////    public sealed class Border : Symbol
////    {
////        private readonly Symbol symbol;

////        private Border(Symbol symbol, string name)
////            : base(symbol.Region, name)
////        {
////            this.symbol = symbol;

////            this.AddDisposable(
////                this.Region.OnChangeRequested.Subscribe(
////                    new Observer<RegionChangeEventArgs>(
////                        x =>
////                        {
////                            if (x.ChangeTypes == RegionChangeTypes.Move)
////                            {
////                                Coord offset = x.AfterChange.TopLeft - x.BeforeChange.TopLeft;
////                                if (this.symbol.Region.CanTranslateBy(offset))
////                                {
////                                    x.Cancel();
////                                }
////                            }
////                            else
////                            {
////                                // intentional compiler error to force me to fix this
////                                ();
////                            }
////                        })));
////        }

////        public override bool CanBeFocused => this.symbol.CanBeFocused;

////        public static Border Create(
////            Symbol symbol,
////            string name,
////            BorderStyle style,
////            BorderModes modes)
////        {
////            Border border = new Border(symbol, name);

////            return border;
////        }

////        public override void Draw(ITerminalSink sink, Rectangle window)
////        {
////            // TODO draw the borders
////            this.symbol.Draw(sink, window);
////        }
////    }
////}
