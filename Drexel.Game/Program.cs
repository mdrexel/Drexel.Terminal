using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Terminal.Sink;
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

                terminal.Source.OnKeyPressed +=
                    (obj, e) =>
                    {
                        terminal.Source.MouseEnabled = !terminal.Source.MouseEnabled;
                        ////terminal.Sink.Write(new CharInfo(e.KeyChar, TerminalColors.Default));
                    };

                await terminal.Source.DelayUntilExitAccepted(default);
            }

            return 0;
        }
    }
}
