using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Terminal;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Source;
using Drexel.Terminal.Text;
using Drexel.Terminal.Win32;
using System.Reactive;
using System.Reactive.Linq;
using Drexel.Terminal.Primitives;
using Drexel.Terminal.Layout.Layouts;
using Drexel.Terminal.Layout.Layouts.Symbols;
using Drexel.Terminal.Internals;
using Drexel.Terminal.Layout;

namespace Drexel.Game
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (TerminalInstance terminal = await TerminalInstance.GetInstanceAsync(default))
            {
                terminal.Source.KeyboardEnabled = true;
                terminal.Source.MouseEnabled = true;

                terminal.DisableResize();

                terminal.Sink.WriteLine("foo bar baz bazinga");

                LayoutManager manager = new LayoutManager(terminal, true);

                TextField textField = new TextField(
                        new Region(new Coord(5, 5), new Coord(20, 5)),
                        "Foo",
                        new TerminalColors(TerminalColor.White, TerminalColor.DarkGreen));

                manager.Add(textField);

                textField.OnComplete.Subscribe(
                    new Observer<string>(
                        x => terminal.Sink.Write(x, new Coord(7, 5))));

                await terminal.Source.DelayUntilExitAccepted(default);
            }
        }

        public static async Task<int> Foo(string[] args)
        {
            using (TerminalInstance terminal = await TerminalInstance.GetInstanceAsync(default))
            {
                terminal.Title = "Foo";
                terminal.Height = 12;
                terminal.Width = 40;

                terminal.SetCodePage(ConsoleCodePage.Utf8);

                terminal.Source.MouseEnabled = true;
                terminal.Source.KeyboardEnabled = true;

                terminal.Source.Mouse.OnMove.Subscribe(
                    x =>
                    {
                        terminal.Sink.Write($"Pos: {x.CurrentPosition.X}, {x.CurrentPosition.Y}  ", Coord.Zero);
                    });

                terminal.Source.Mouse.OnMove.Subscribe(
                    e =>
                    {
                        if (terminal.Source.Mouse.LeftButton.Down && terminal.Source.Mouse.RightButton.Down)
                        {
                            terminal.Sink.Write(
                                new CharInfo(' ', new TerminalColors(TerminalColor.Black, TerminalColor.Magenta)),
                                e.CurrentPosition);
                        }
                        else if (terminal.Source.Mouse.LeftButton.Down)
                        {
                            terminal.Sink.Write(
                                new CharInfo(' ', new TerminalColors(TerminalColor.Black, TerminalColor.Red)),
                                e.CurrentPosition);
                        }
                        else if (terminal.Source.Mouse.RightButton.Down)
                        {
                            terminal.Sink.Write(
                                new CharInfo(' ', new TerminalColors(TerminalColor.Black, TerminalColor.Blue)),
                                e.CurrentPosition);
                        }
                    });

                terminal.Source.OnKeyPressed.Subscribe(
                    e =>
                    {
                        if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar))
                        {
                            terminal.Sink.Write(new CharInfo(e.KeyChar, TerminalColors.Default));
                        }
                        else if (e.Key == TerminalKey.Spacebar)
                        {
                            terminal.Sink.Write();
                        }
                        else if (e.Key == TerminalKey.Enter)
                        {
                            terminal.Sink.WriteLine();
                        }
                        else if (e.Key == TerminalKey.UpArrow)
                        {
                            terminal.Sink.CursorPosition -= Coord.OneYOffset;
                        }
                        else if (e.Key == TerminalKey.DownArrow)
                        {
                            terminal.Sink.CursorPosition += Coord.OneYOffset;
                        }
                        else if (e.Key == TerminalKey.LeftArrow)
                        {
                            terminal.Sink.CursorPosition -= Coord.OneXOffset;
                        }
                        else if (e.Key == TerminalKey.RightArrow)
                        {
                            terminal.Sink.CursorPosition += Coord.OneXOffset;
                        }

                        ////terminal.Source.MouseEnabled = !terminal.Source.MouseEnabled;
                    });

                TerminalColors color = new TerminalColors(TerminalColor.Black, TerminalColor.Blue);
                terminal.Sink.Write(
                    new Fill(3, 3, 8, 8, new CharInfo[,] { { new CharInfo(' ', color) } }));
                terminal.Sink.Write(new Line(0, 0, 10, 10, new CharInfo[,] { { '*', '*', '*' }, { '~', '~', '~' } }));

                string test = await terminal.ReadLineAsync();

                await terminal.Source.DelayUntilExitAccepted(default);
            }

            return 0;
        }
    }
}
