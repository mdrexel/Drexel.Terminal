using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Terminal.Win32;

namespace Drexel.Game
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            TerminalInstance terminal = await TerminalInstance.GetSingletonAsync(default);

            terminal.Title = "Foo";
            terminal.Height = 12;
            terminal.Width = 40;

            await terminal.Source.DelayUntilExitAccepted(default);

            return 0;
        }
    }
}
