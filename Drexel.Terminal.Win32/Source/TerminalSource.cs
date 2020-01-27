using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Terminal.Source.Win32
{
    public sealed class TerminalSource : ITerminalSource, IDisposable
    {
        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_QUICK_EDIT = 0x0040;
        private const int INFINITE = -1;

        private static Lazy<TerminalSource> backingInstance = new Lazy<TerminalSource>(
            () => new TerminalSource(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly bool gui;
        private readonly Box<bool> eventThreadRunning;
        private readonly Thread eventThread;
        private readonly IntPtr handle;
        private readonly ConsoleCtrlHandlerDelegate consoleControlHandler;

        private ConsoleMouseEventInfo lastMouseEvent;

        private TerminalSource(bool gui = true)
        {
            lastMouseEvent = default;
            this.gui = gui;

            this.handle = GetStdHandle(STD_INPUT_HANDLE);

            if (gui)
            {
                // Disable quick-edit mode (the ability to highlight regions of the console with the mouse)
                GetConsoleMode(this.handle, out uint consoleMode);
                consoleMode &= ~ENABLE_QUICK_EDIT;
                SetConsoleMode(this.handle, consoleMode);
            }

            this.consoleControlHandler =
                (consoleControlEventType) =>
                {
                    if (consoleControlEventType == ConsoleControlEventType.CtrlC
                        || consoleControlEventType == ConsoleControlEventType.CtrlBreak)
                    {
                        ExitRequestedEventArgs args = new ExitRequestedEventArgs();
                        this.OnExitRequested?.Invoke(this, args);
                        if (args.Allow)
                        {
                            this.OnExitAccepted?.Invoke(this, new ExitAcceptedEventArgs());
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                };

            SetConsoleCtrlHandler(
                this.consoleControlHandler,
                true);

            this.OnLeftMouse += (obj, e) => this.LeftMouseDown = e.ButtonDown;
            this.OnRightMouse += (obj, e) => this.RightMouseDown = e.ButtonDown;

            this.eventThreadRunning = new Box<bool>(true);
            this.eventThread =
                new Thread(
                    () =>
                    {
                        bool GetRunningState()
                        {
                            lock (this.eventThreadRunning)
                            {
                                return this.eventThreadRunning.Value;
                            }
                        }

                        while (GetRunningState())
                        {
                            try
                            {
                                while (true)
                                {
                                    ConsoleInputEventInfo[] events = this.ListenForEvents();
                                    foreach (ConsoleInputEventInfo @event in events)
                                    {
                                        try
                                        {
                                            this.DispatchEventProcessing(@event);
                                        }
                                        catch
                                        {
                                            // Processing an event shouldn't emit an exception, but just in case...
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                // This shouldn't ever happen, but just in case, re-listen for events
                            }
                        }
                    });
            this.eventThread.Start();
        }

        public static TerminalSource Singleton => backingInstance.Value;

        public bool LeftMouseDown { get; private set; }

        public bool RightMouseDown { get; private set; }

        public event EventHandler<ExitRequestedEventArgs>? OnExitRequested;

        public event EventHandler<MouseClickEventArgs>? OnLeftMouse;

        public event EventHandler<MouseClickEventArgs>? OnRightMouse;

        public event EventHandler<MouseMoveEventArgs>? OnMouseMove;

        public event EventHandler<MouseWheelEventArgs>? OnMouseWheel;

        public event EventHandler<TerminalKeyInfo>? OnKeyPressed;

        public event EventHandler<ExitAcceptedEventArgs>? OnExitAccepted;

        private delegate bool ConsoleCtrlHandlerDelegate(ConsoleControlEventType CtrlType);

        public void Dispose()
        {
            backingInstance = new Lazy<TerminalSource>(
                () =>
                {
                    throw new InvalidOperationException("Cannot access terminal after singleton has been disposed.");
                });

            lock (this.eventThreadRunning)
            {
                this.eventThreadRunning.Value = false;
            }

            SetConsoleCtrlHandler(
                this.consoleControlHandler,
                false);

            CancelIoEx(this.handle, IntPtr.Zero);
            this.eventThread.Join();
        }

        public async Task DelayUntilExitAccepted(CancellationToken cancellationToken)
        {
            CancellationTokenSource cts;
            lock (this.eventThreadRunning)
            {
                if (!this.eventThreadRunning.Value)
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
        private static extern bool SetConsoleCtrlHandler(
            ConsoleCtrlHandlerDelegate HandlerRoutine,
            bool Add);

        [DllImport("kernel32.dll")]
        private static extern uint WaitForSingleObject(
            IntPtr hHandle,
            int dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleInput(
            IntPtr hConsoleInput,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] ConsoleInputEventInfo[] lpBuffer,
            int nLength,
            out int lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNumberOfConsoleInputEvents(
            IntPtr hConsoleInput,
            out int lpcNumberOfEvents);

        private ConsoleInputEventInfo[] ListenForEvents()
        {
            // Wait until the console notifies us that at least one event has been received
            WaitForSingleObject(this.handle, INFINITE);

            // Find out the number of console events waiting for us
            GetNumberOfConsoleInputEvents(this.handle, out int unreadEventCount);

            // Allocate an array to read events into, and then read them
            ConsoleInputEventInfo[] values = new ConsoleInputEventInfo[unreadEventCount];
            ReadConsoleInput(
                this.handle,
                values,
                unreadEventCount,
                out _);

            return values;
        }

        private void DispatchEventProcessing(ConsoleInputEventInfo @event)
        {
            switch (@event.EventType)
            {
                case ConsoleInputEventType.None:
                    // This shouldn't ever happen, but just in case...
                    break;
                case ConsoleInputEventType.FocusEvent:
                    break;
                case ConsoleInputEventType.KeyEvent:
                    this.ProcessKeyEvent(@event.KeyEvent);
                    break;
                case ConsoleInputEventType.MenuEvent:
                    break;
                case ConsoleInputEventType.MouseEvent:
                    if (this.gui)
                    {
                        this.ProcessMouseEvent(@event.MouseEvent);
                    }

                    break;
                case ConsoleInputEventType.WindowBufferSizeEvent:
                    break;
                default:
                    throw new NotImplementedException(
                        "Unrecognized console input event type.");
            }
        }

        private void ProcessKeyEvent(ConsoleKeyEventInfo keyEvent)
        {
            if (keyEvent.KeyDown)
            {
                this.OnKeyPressed?.Invoke(
                    this,
                    new TerminalKeyInfo(
                        keyEvent.UnicodeChar,
                        (TerminalKey)keyEvent.VirtualKeyCode,
                        keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.ShiftPressed),
                        keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.LeftAltPressed)
                            || keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.RightAltPressed),
                        keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.LeftCtrlPressed)
                            || keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.RightCtrlPressed)));
            }
        }

        private void ProcessMouseEvent(ConsoleMouseEventInfo mouseEvent)
        {
            if (mouseEvent.MousePosition != this.lastMouseEvent.MousePosition)
            {
                MouseMoveEventArgs args =
                    new MouseMoveEventArgs(
                        this.lastMouseEvent.MousePosition,
                        mouseEvent.MousePosition);
                this.OnMouseMove?.Invoke(this, args);
            }

            ConsoleMouseButtonState delta = mouseEvent.ButtonState ^ lastMouseEvent.ButtonState;

            if (delta.HasFlag(ConsoleMouseButtonState.FromLeft1stButtonPressed))
            {
                this.OnLeftMouse?.Invoke(
                    this,
                    new MouseClickEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.FromLeft1stButtonPressed)));
            }

            if (delta.HasFlag(ConsoleMouseButtonState.RightMostButtonPressed))
            {
                this.OnRightMouse?.Invoke(
                    this,
                    new MouseClickEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.RightMostButtonPressed)));
            }

            if (mouseEvent.EventFlags.HasFlag(ConsoleMouseEventType.MouseWheeled))
            {
                this.OnMouseWheel?.Invoke(
                    this,
                    new MouseWheelEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.ScrollDown)
                            ? MouseWheelDirection.Down
                            : MouseWheelDirection.Up));
            }

            this.lastMouseEvent = mouseEvent;
        }

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
