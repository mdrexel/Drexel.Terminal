using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Output
{
    public sealed class Source : IDisposable
    {
        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_QUICK_EDIT = 0x0040;

        private static readonly object ActiveLock = new object();
        private static bool Active = false;

        private readonly Box<bool> keyThreadRunning;
        private readonly Thread keyThread;

        public Source()
        {
            lock (Source.ActiveLock)
            {
                if (Source.Active)
                {
                    throw new InvalidOperationException("Can only have one source open at a time.");
                }

                Source.Active = true;
            }

            // Disable quick-edit mode (the ability to highlight regions of the console with the mouse)
            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(handle, out uint consoleMode);
            consoleMode &= ~ENABLE_QUICK_EDIT;
            SetConsoleMode(handle, consoleMode);

            Console.CancelKeyPress +=
                (obj, e) =>
                {
                    ExitRequestedEventArgs args = new ExitRequestedEventArgs();
                    this.OnExitRequested?.Invoke(this, args);
                    if (!args.Allow)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        this.OnExitAccepted?.Invoke(obj, e);
                    }
                };

            this.keyThreadRunning = new Box<bool>(true);
            this.keyThread =
                new Thread(
                    () =>
                    {
                        bool GetRunningState()
                        {
                            lock (this.keyThreadRunning)
                            {
                                return this.keyThreadRunning.Value;
                            }
                        }

                        while (GetRunningState())
                        {
                            try
                            {
                                while (true)
                                {
                                    this.OnKeyPressed?.Invoke(this, Console.ReadKey(true));
                                }
                            }
                            catch
                            {
                                // If the ReadKey is canceled by a disposal, it'll either throw an
                                // InvalidOperationException or an OperationCanceledException, which will cause the
                                // inner loop be broken. The Dispose call will have set keyThreadRunning to false, so
                                // the thread will be ready to join.
                            }
                        }
                    });
            this.keyThread.Start();
        }

        public event EventHandler<ConsoleKeyInfo>? OnKeyPressed;

        public event EventHandler<ExitRequestedEventArgs>? OnExitRequested;

        public event EventHandler? OnExitAccepted;

        public void Dispose()
        {
            lock (this.keyThreadRunning)
            {
                this.keyThreadRunning.Value = false;
            }

            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
            CancelIoEx(handle, IntPtr.Zero);
            this.keyThread.Join();

            lock (Source.ActiveLock)
            {
                Source.Active = false;
            }
        }

        public async Task DelayUntilExitAccepted(CancellationToken cancellationToken)
        {
            CancellationTokenSource cts;
            lock (this.keyThreadRunning)
            {
                if (!this.keyThreadRunning.Value)
                {
                    return;
                }
                else
                {
                    cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    this.OnExitAccepted +=
                        (obj, e) =>
                        {
                            cts.Cancel();
                        };

                }
            }

            try
            {
                await Task.Delay(-1, cts.Token);
            }
            catch (TaskCanceledException)
            {
                // An exception being thrown is expected.
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        private sealed class Box<T>
        {
            public Box(T value)
            {
                this.Value = value;
            }

            public T Value { get; set; }
        }
    }
}
