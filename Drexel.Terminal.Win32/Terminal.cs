using System;
using System.Threading;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Sink.Win32;
using Drexel.Terminal.Source;
using Drexel.Terminal.Source.Win32;

namespace Drexel.Terminal.Win32
{
    public sealed class Terminal : ITerminal, IDisposable
    {
        private static Lazy<Terminal> backingInstance =
            new Lazy<Terminal>(
                () => new Terminal(TerminalSource.Singleton, TerminalSink.Singleton),
                LazyThreadSafetyMode.ExecutionAndPublication);

        private Terminal(TerminalSource source, TerminalSink sink)
        {
            this.Source = source;
            this.Sink = sink;
        }

        public static Terminal Singleton => backingInstance.Value;

        public TerminalSource Source { get; }

        public TerminalSink Sink { get; }

        public string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }

        public ushort Height
        {
            get
            {
                checked
                {
                    return (ushort)Console.BufferHeight;
                }
            }
            set => Console.BufferHeight = value;
        }

        public ushort Width
        {
            get
            {
                checked
                {
                    return (ushort)Console.BufferWidth;
                }
            }
            set => Console.BufferWidth = value;
        }

        ITerminalSource IReadOnlyTerminal.Source => this.Source;

        ITerminalSink ITerminal.Sink => this.Sink;

        string IReadOnlyTerminal.Title => this.Title;

        ushort IReadOnlyTerminal.Height => this.Height;

        ushort IReadOnlyTerminal.Width => this.Width;

        public void Dispose()
        {
            this.Source.Dispose();
            this.Sink.Dispose();

            backingInstance = new Lazy<Terminal>(
                () =>
                {
                    throw new InvalidOperationException("Cannot access terminal after singleton has been disposed.");
                });
        }
    }
}
