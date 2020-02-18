using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Terminal;
using Drexel.Terminal.Sink;
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
                ////terminal.Sink.CursorPosition = new Coord(19, 3);
                ////terminal.Sink.Write("漢字");
                terminal.Sink.Write(
                    new Catena(
                        "The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.",
                        TerminalColors.Default,
                        100),
                    new Coord(19, 3));
                ////terminal.Sink.Write(new string('a', 41));

                terminal.Source.OnKeyPressed +=
                    (obj, e) =>
                    {
                        ////terminal.Source.MouseEnabled = !terminal.Source.MouseEnabled;
                        terminal.Sink.Write(new CharInfo(e.KeyChar, TerminalColors.Default));
                    };

                await terminal.Source.DelayUntilExitAccepted(default);
            }

            return 0;
        }
    }
}
