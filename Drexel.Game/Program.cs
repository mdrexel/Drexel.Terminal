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

namespace Drexel.Game
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            using (TerminalInstance terminal = await TerminalInstance.GetInstanceAsync(default))
            {
                terminal.Title = "Foo";
                terminal.Height = 12;
                terminal.Width = 40;

                terminal.SetCodePage(ConsoleCodePage.Utf8);

                terminal.Source.MouseEnabled = true;

                terminal.Source.Mouse.OnMouseMove.Subscribe(
                    new Observer<MouseMoveEventArgs>(
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
                    }));

                terminal.Source.OnKeyPressed.Subscribe(
                    new Observer<TerminalKeyInfo>(
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
                    }));

                await terminal.Source.DelayUntilExitAccepted(default);
            }

            return 0;
        }
    }
}
