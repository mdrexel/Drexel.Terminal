using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Sink.Win32;
using Drexel.Terminal.Source;
using Drexel.Terminal.Source.Win32;
using Microsoft.Win32.SafeHandles;

namespace Drexel.Terminal.Win32
{
    public sealed class TerminalInstance : ITerminal, IDisposable
    {
        private static readonly SemaphoreSlim ActiveSemaphore = new SemaphoreSlim(1, 1);

        private readonly SafeFileHandle handle;
        private readonly Action releaseCallback;
        private bool isDisposed;

        private TerminalInstance(Action releaseCallback)
        {
            this.handle = CreateFile(
                "CONOUT$",
                0x40000000,
                2,
                IntPtr.Zero,
                FileMode.Open,
                0,
                IntPtr.Zero);

            if (this.handle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            this.releaseCallback = releaseCallback;

            this.Source = new TerminalSource(this.handle);
            this.Sink = new TerminalSink(this.handle);

            this.isDisposed = false;
        }

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
            set
            {
                Console.WindowHeight = value;
                Console.BufferHeight = value;
            }
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
            set
            {
                Console.WindowWidth = value;
                Console.BufferWidth = value;
            }
        }

        ITerminalSource IReadOnlyTerminal.Source => this.Source;

        ITerminalSink ITerminal.Sink => this.Sink;

        string IReadOnlyTerminal.Title => this.Title;

        ushort IReadOnlyTerminal.Height => this.Height;

        ushort IReadOnlyTerminal.Width => this.Width;

        public static async Task<TerminalInstance> GetSingletonAsync(CancellationToken cancellationToken)
        {
            await TerminalInstance.ActiveSemaphore.WaitAsync(cancellationToken);
            return new TerminalInstance(() => TerminalInstance.ActiveSemaphore.Release());
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                this.Source.Dispose();
                this.handle.Dispose();
                this.releaseCallback.Invoke();
            }
        }
    }
}
