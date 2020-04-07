using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Terminal.Sink;
using Drexel.Terminal.Win32;
using Microsoft.Win32.SafeHandles;

namespace Drexel.Terminal.Source.Win32
{
    public sealed class TerminalSource : ITerminalSource
    {
        private const int STD_INPUT_HANDLE = -10;
        private const int INFINITE = -1;

        private readonly Box<bool> eventThreadRunning;
        private readonly Thread eventThread;
        private readonly SafeFileHandle inputHandle;
        private readonly SafeFileHandle inputStreamHandle;
        private readonly ConsoleCtrlHandlerDelegate consoleControlHandler;

        private readonly Observable<ExitRequestedEventArgs> onExitRequested;
        private readonly Observable<ExitAcceptedEventArgs> onExitAccepted;
        private readonly Observable<TerminalKeyInfo> onKeyPressed;
        private readonly Observable<TerminalKeyInfo> onKeyReleased;
        private readonly Observable<TerminalKeyInfo> onKeyPressedInternal;
        private readonly Observable<TerminalKeyInfo> onKeyReleasedInternal;
        private readonly Mouse mouse;

        private bool mouseEnabled;
        private bool keyboardEnabled;
        private bool keyboardSuppressed;
        private ConsoleMouseEventInfo lastMouseEvent;

        internal TerminalSource()
        {
            this.inputHandle = TerminalInstance.CreateFileW(
                "CONIN$",
                0x80000000,
                1,
                IntPtr.Zero,
                FileMode.Open,
                0,
                IntPtr.Zero);

            if (this.inputHandle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            this.inputStreamHandle = TerminalInstance.GetStdHandle(STD_INPUT_HANDLE);
            if (this.inputStreamHandle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            lastMouseEvent = default;
            this.mouse = new Mouse();

            this.onExitRequested = new Observable<ExitRequestedEventArgs>();
            this.onExitAccepted = new Observable<ExitAcceptedEventArgs>();
            this.onKeyPressed = new Observable<TerminalKeyInfo>();
            this.onKeyReleased = new Observable<TerminalKeyInfo>();

            this.onKeyPressedInternal = new Observable<TerminalKeyInfo>();
            this.onKeyReleasedInternal = new Observable<TerminalKeyInfo>();

            this.onKeyPressedInternal.Subscribe(
                new Observer<TerminalKeyInfo>(
                    x =>
                    {
                        if (!this.keyboardSuppressed)
                        {
                            this.onKeyPressed.Next(x);
                        }
                    },
                    x => this.onKeyPressed.Error(x),
                    () => this.onKeyPressed.Complete()));
            this.onKeyReleasedInternal.Subscribe(
                new Observer<TerminalKeyInfo>(
                    x =>
                    {
                        if (!this.keyboardSuppressed)
                        {
                            this.onKeyReleased.Next(x);
                        }
                    },
                    x => this.onKeyReleased.Error(x),
                    () => this.onKeyReleased.Complete()));

            this.consoleControlHandler =
                (consoleControlEventType) =>
                {
                    if (consoleControlEventType == ConsoleControlEventType.CtrlC
                        || consoleControlEventType == ConsoleControlEventType.CtrlBreak
                        || consoleControlEventType == ConsoleControlEventType.CtrlClose)
                    {
                        return this.RequestExit();
                    }
                    else
                    {
                        return false;
                    }
                };

            SetConsoleCtrlHandler(
                this.consoleControlHandler,
                true);

            this.mouseEnabled = false;
            this.keyboardEnabled = false;
            this.keyboardSuppressed = false;

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
                            while (this.ListenForEvents(out ConsoleInputEventInfo[] events))
                            {
                                foreach (ConsoleInputEventInfo @event in events)
                                {
                                    this.DispatchEventProcessing(@event);
                                }
                            }
                        }
                    });
            this.eventThread.Start();
        }

        public bool KeyboardEnabled
        {
            get => this.keyboardEnabled;
            set
            {
                if (this.keyboardEnabled == value)
                {
                    return;
                }

                this.keyboardEnabled = value;
            }
        }

        public bool MouseEnabled
        {
            get => this.mouseEnabled;
            set
            {
                if (this.mouseEnabled == value)
                {
                    return;
                }

                this.mouseEnabled = value;

                // Toggle quick-edit mode (the ability to highlight regions of the console with the mouse)
                GetConsoleMode(this.inputStreamHandle.DangerousGetHandle(), out uint consoleMode);
                SetConsoleMode(
                    this.inputStreamHandle.DangerousGetHandle(),
                    (uint)(((ConsoleInputModes)consoleMode) ^ ConsoleInputModes.ENABLE_QUICK_EDIT_MODE));
            }
        }

        public ConsoleCodePage CodePage
        {
            get => (ConsoleCodePage)GetConsoleCP();
            set => SetConsoleCP((uint)value);
        }

        public IObservable<ExitRequestedEventArgs> OnExitRequested => this.onExitRequested;

        public IObservable<TerminalKeyInfo> OnKeyPressed => this.onKeyPressed;

        public IObservable<TerminalKeyInfo> OnKeyReleased => this.onKeyReleased;

        public IObservable<ExitAcceptedEventArgs> OnExitAccepted => this.onExitAccepted;

        public IMouse Mouse => this.mouse;

        private delegate bool ConsoleCtrlHandlerDelegate(ConsoleControlEventType CtrlType);

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
                    this.OnExitAccepted.Subscribe(
                        new Observer<ExitAcceptedEventArgs>(
                            x => cts.Cancel(),
                            x => cts.Cancel(),
                            () => cts.Cancel()));
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

        public Task<string> ReadLineAsync(
            Action<TerminalKeyInfo>? echo = null,
            bool exclusive = true,
            CancellationToken cancellationToken = default)
        {
            if (!this.keyboardEnabled)
            {
                throw new InvalidOperationException(
                    "Can't perform reads if the keyboard is disabled.");
            }
            else if (this.keyboardSuppressed)
            {
                throw new InvalidOperationException(
                    "Can't perform reads if an exclusive read is already in progress.");
            }

            this.keyboardSuppressed = exclusive;

            TaskCompletionSource<string> result = new TaskCompletionSource<string>();
            List<char> characters = new List<char>();
            IDisposable subscription = this.onKeyPressedInternal.Subscribe(
                new Observer<TerminalKeyInfo>(
                    x =>
                    {
                        if (x.Key == TerminalKey.Enter)
                        {
                            result.SetResult(new string(characters.ToArray()));
                        }
                        else if (x.Key == TerminalKey.Backspace)
                        {
                            characters.RemoveAt(characters.Count - 1);
                            echo?.Invoke(x);
                        }
                        else if (char.IsLetterOrDigit(x.KeyChar)
                            || char.IsWhiteSpace(x.KeyChar)
                            || char.IsPunctuation(x.KeyChar))
                        {
                            characters.Add(x.KeyChar);
                            echo?.Invoke(x);
                        }
                    },
                    x => throw x,
                    () =>
                    {
                        result.SetResult(new string(characters.ToArray()));
                    }));

            cancellationToken.Register(
                () =>
                {
                    subscription.Dispose();
                    result.SetCanceled();
                });

            return result.Task.ContinueWith<string>(
                (task) =>
                {
                    subscription.Dispose();
                    this.keyboardSuppressed = false;
                    return task.Result;
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        public bool RequestExit()
        {
            ExitRequestedEventArgs requestedArgs = new ExitRequestedEventArgs();
            this.onExitRequested.Next(requestedArgs);

            if (requestedArgs.Allow)
            {
                ExitAcceptedEventArgs acceptedArgs = new ExitAcceptedEventArgs();
                this.onExitAccepted.Next(acceptedArgs);

                return true;
            }
            else
            {
                return false;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetConsoleCP();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(
            ConsoleCtrlHandlerDelegate HandlerRoutine,
            bool Add);

        [DllImport("kernel32.dll")]
        private static extern uint WaitForSingleObject(
            IntPtr hHandle,
            int dwMilliseconds);

        [DllImport("kernel32.dll")]
        private static extern uint WaitForSingleObject(
            SafeFileHandle hHandle,
            int dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(SafeFileHandle handle, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(
            IntPtr hConsoleHandle,
            out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(
            IntPtr hConsoleHandle,
            uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleInput(
            SafeFileHandle hConsoleInput,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] ConsoleInputEventInfo[] lpBuffer,
            int nLength,
            out int lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNumberOfConsoleInputEvents(
            SafeFileHandle hConsoleInput,
            out int lpcNumberOfEvents);

        internal void Dispose()
        {
            lock (this.eventThreadRunning)
            {
                this.eventThreadRunning.Value = false;
            }

            SetConsoleCtrlHandler(
                this.consoleControlHandler,
                false);

            CancelIoEx(this.inputHandle, IntPtr.Zero);

            this.inputHandle.Dispose();
            this.inputStreamHandle.Dispose();

            this.eventThread.Join();
        }

        private bool ListenForEvents(out ConsoleInputEventInfo[] events)
        {
            bool @continue = true;
            int unreadEventCount;
            try
            {
                // Wait until the console notifies us that at least one event has been received
                WaitForSingleObject(this.inputHandle, INFINITE);

                // Find out the number of console events waiting for us
                GetNumberOfConsoleInputEvents(this.inputHandle, out unreadEventCount);
            }
            catch (ObjectDisposedException)
            {
                // This gets thrown if we were listening for events when the terminal was disposed. This is fine,
                // because the event listener thread will exit after processing the last batch of events.
                @continue = false;
                unreadEventCount = 0;
            }

            if (unreadEventCount == 0)
            {
                events = Array.Empty<ConsoleInputEventInfo>();
            }
            else
            {
                // Allocate an array to read events into, and then read them
                events = new ConsoleInputEventInfo[unreadEventCount];
                ReadConsoleInput(
                    this.inputHandle,
                    events,
                    unreadEventCount,
                    out _);
            }

            return @continue;
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
                    if (this.mouseEnabled)
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
            if (!this.keyboardEnabled)
            {
                return;
            }

            TerminalKeyInfo keyInfo = new TerminalKeyInfo(
                keyEvent.UnicodeChar,
                (TerminalKey)keyEvent.VirtualKeyCode,
                keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.ShiftPressed),
                keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.LeftAltPressed)
                    || keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.RightAltPressed),
                keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.LeftCtrlPressed)
                    || keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.RightCtrlPressed));

            if (keyEvent.KeyDown)
            {
                this.onKeyPressedInternal.Next(keyInfo);
            }
            else
            {
                this.onKeyReleasedInternal.Next(keyInfo);
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
                this.mouse.OnMouseMove.Next(args);
            }

            ConsoleMouseButtonState delta = mouseEvent.ButtonState ^ lastMouseEvent.ButtonState;

            if (delta.HasFlag(ConsoleMouseButtonState.FromLeft1stButtonPressed))
            {
                this.mouse.LeftButton.OnButton.Next(
                    new MouseClickEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.FromLeft1stButtonPressed)));
            }

            if (delta.HasFlag(ConsoleMouseButtonState.FromLeft2ndMouseButtonPressed))
            {
                this.mouse.MiddleButton.OnButton.Next(
                    new MouseClickEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.FromLeft2ndMouseButtonPressed)));
            }

            if (delta.HasFlag(ConsoleMouseButtonState.FromLeft3rdMouseButtonPressed))
            {
                this.mouse.Button4.OnButton.Next(
                    new MouseClickEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.FromLeft3rdMouseButtonPressed)));
            }

            if (delta.HasFlag(ConsoleMouseButtonState.FromLeft4thMouseButtonPressed))
            {
                this.mouse.Button5.OnButton.Next(
                    new MouseClickEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.FromLeft4thMouseButtonPressed)));
            }

            if (delta.HasFlag(ConsoleMouseButtonState.RightMostButtonPressed))
            {
                this.mouse.RightButton.OnButton.Next(
                    new MouseClickEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.RightMostButtonPressed)));
            }

            if (mouseEvent.EventFlags.HasFlag(ConsoleMouseEventType.MouseWheeled))
            {
                this.mouse.OnMouseWheel.Next(
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
