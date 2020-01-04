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
        private const int INFINITE = -1;

        private static readonly object ActiveLock = new object();
        private static bool Active = false;

        private readonly Box<bool> eventThreadRunning;
        private readonly Thread eventThread;
        private readonly IntPtr handle;
        private readonly ConsoleCtrlHandlerDelegate consoleControlHandler;

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
            this.handle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(this.handle, out uint consoleMode);
            consoleMode &= ~ENABLE_QUICK_EDIT;
            SetConsoleMode(this.handle, consoleMode);

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
                            this.OnExitAccepted?.Invoke(this, new EventArgs());
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

        public event EventHandler<MouseMoveEventArgs>? OnMouseMove;

        public event EventHandler<MouseClickEventArgs>? OnLeftMouse;

        public event EventHandler<MouseClickEventArgs>? OnRightMouse;

        public event EventHandler<MouseWheelEventArgs>? OnVerticalMouseWheel;

        public event EventHandler<ConsoleKeyInfo>? OnKeyPressed;

        public event EventHandler<ExitRequestedEventArgs>? OnExitRequested;

        public event EventHandler? OnExitAccepted;

        private delegate bool ConsoleCtrlHandlerDelegate(ConsoleControlEventType CtrlType);

        public bool LeftMouseDown { get; private set; }

        public bool RightMouseDown { get; private set; }

        public void Dispose()
        {
            lock (this.eventThreadRunning)
            {
                this.eventThreadRunning.Value = false;
            }

            SetConsoleCtrlHandler(
                this.consoleControlHandler,
                false);

            CancelIoEx(this.handle, IntPtr.Zero);
            this.eventThread.Join();

            lock (Source.ActiveLock)
            {
                Source.Active = false;
            }
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
                    this.ProcessMouseEvent(@event.MouseEvent);
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
                    new ConsoleKeyInfo(
                        keyEvent.UnicodeChar,
                        keyEvent.VirtualKeyCode,
                        keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.ShiftPressed),
                        keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.LeftAltPressed)
                            || keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.RightAltPressed),
                        keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.LeftCtrlPressed)
                            || keyEvent.ControlKeyState.HasFlag(ConsoleControlKeyState.RightCtrlPressed)));
            }
        }

        ConsoleMouseEventInfo lastMouseEvent = default;
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
                this.OnVerticalMouseWheel?.Invoke(
                    this,
                    new MouseWheelEventArgs(
                        mouseEvent.MousePosition,
                        mouseEvent.ButtonState.HasFlag(ConsoleMouseButtonState.ScrollDown)));
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

        [Flags]
        private enum ConsoleMouseButtonState : int
        {
            /// <summary>
            /// The leftmost mouse button.
            /// </summary>
            FromLeft1stButtonPressed = 1,

            /// <summary>
            /// The rightmost mouse button.
            /// </summary>
            RightMostButtonPressed = 2,

            /// <summary>
            /// The second button from the left
            /// </summary>
            FromLeft2ndMouseButtonPressed = 4,

            /// <summary>
            /// The third button from the left
            /// </summary>
            FromLeft3rdMouseButtonPressed = 8,

            /// <summary>
            /// The fourth button from the left.
            /// </summary>
            FromLeft4thMouseButtonPressed = 16,

            /// <summary>
            /// For mouse wheel events, if this flag is set, the wheel was scrolled down. If cleared, the wheel was
            /// scrolled up.
            /// </summary>
            ScrollDown = unchecked((int)0xFF000000)
        }

        [Flags]
        private enum ConsoleMouseEventType
        {
            /// <summary>
            /// A mouse button was pressed or released
            /// </summary>
            MouseButton = 0,

            /// <summary>
            /// A change in mouse position occurred
            /// </summary>
            MouseMoved = 1,

            /// <summary>
            /// The second click of a double-click operation occurred.
            /// </summary>
            DoubleClick = 2,

            /// <summary>
            /// The vertical mouse wheel was rolled.
            /// </summary>
            MouseWheeled = 4,

            /// <summary>
            /// The horizontal mouse wheel was rolled.
            /// </summary>
            MouseHWheeled = 8
        }

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ConsoleMouseEventInfo
        {
            [FieldOffset(0)] private readonly Coord dwMousePosition;
            [FieldOffset(4)] private readonly int dwButtonState;
            [FieldOffset(8)] private readonly int dwControlKeyState;
            [FieldOffset(12)] private readonly int dwEventFlags;

            public Coord MousePosition => this.dwMousePosition;

            public ConsoleMouseButtonState ButtonState => (ConsoleMouseButtonState)this.dwButtonState;

            public ConsoleControlKeyState ControlKeyState => (ConsoleControlKeyState)this.dwControlKeyState;

            public ConsoleMouseEventType EventFlags => (ConsoleMouseEventType)this.dwEventFlags;
        }

        /// <summary>
        /// Console input event types.
        /// </summary>
        private enum ConsoleInputEventType : short
        {
            None = 0,
            KeyEvent = 1,
            MouseEvent = 2,
            WindowBufferSizeEvent = 4,
            MenuEvent = 8,
            FocusEvent = 16
        }

        [Flags]
        private enum ConsoleControlKeyState : int
        {
            /// <summary>
            /// Right Alt key is pressed
            /// </summary>
            RightAltPressed = 0x0001,

            /// <summary>
            /// Left Alt key is pressed
            /// </summary>
            LeftAltPressed = 0x0002,

            /// <summary>
            /// Right Ctrl key is pressed
            /// </summary>
            RightCtrlPressed = 0x0004,

            /// <summary>
            /// Left Ctrl key is pressed
            /// </summary>
            LeftCtrlPressed = 0x0008,

            /// <summary>
            /// The shift keys is pressed
            /// </summary>
            ShiftPressed = 0x0010,

            /// <summary>
            /// The number lock light is on
            /// </summary>
            NumLockOn = 0x0020,

            /// <summary>
            /// The scroll lock light is on
            /// </summary>
            ScrollLockOn = 0x0040,

            /// <summary>
            /// The caps lock light is on
            /// </summary>
            CapsLockOn = 0x0080,

            /// <summary>
            /// The key is enhanced
            /// </summary>
            EnhancedKey = 0x0100,

            /// <summary>
            /// DBCS for JPN: SBCS/DBCS mode
            /// </summary>
            NlsDbcsChar = 0x00010000,

            /// <summary>
            /// DBCS for JPN: Alphanumeric mode
            /// </summary>
            NlsAlphanumeric = 0x00000000,

            /// <summary>
            /// DBCS for JPN: Katakana mode
            /// </summary>
            NlsKatakana = 0x00020000,

            /// <summary>
            /// DBCS for JPN: Hiragana mode
            /// </summary>
            NlsHiragana = 0x00040000,

            /// <summary>
            /// DBCS for JPN: Roman/Noroman mode
            /// </summary>
            NlsRoman = 0x00400000,

            /// <summary>
            /// DBCS for JPN: IME conversion
            /// </summary>
            NlsImeConversion = 0x00800000,

            /// <summary>
            /// DBCS for JPN: IME enable/disable
            /// </summary>
            NlsImeDisable = 0x20000000
        }

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ConsoleKeyEventInfo
        {
            [FieldOffset(0)] private readonly int bKeyDown;
            [FieldOffset(4)] private readonly short wRepeatCount;
            [FieldOffset(6)] private readonly short wVirtualKeyCode;
            [FieldOffset(8)] private readonly short wVirtualScanCode;
            [FieldOffset(10)] private readonly char cUnicodeChar;
            [FieldOffset(10)] private readonly short wUnicodeChar;
            [FieldOffset(10)] private readonly byte bAsciiChar;
            [FieldOffset(12)] private readonly int dwControlKeyState;

            /// <summary>
            /// Gets a value indicating whether this is a key down or key up event.
            /// </summary>
            public bool KeyDown => this.bKeyDown != 0;

            /// <summary>
            /// Gets a value indicating that a key is being held down.
            /// </summary>
            public short RepeatCount => this.wRepeatCount;

            /// <summary>
            /// Gets a value that identifies the given key in a device-independent manner.
            /// </summary>
            public ConsoleKey VirtualKeyCode => (ConsoleKey)this.wVirtualKeyCode;

            /// <summary>
            /// Gets the hardware-dependent virtual scan code.
            /// </summary>
            public short VirtualScanCode => this.wVirtualScanCode;

            /// <summary>
            /// Gets the Unicode character for this key event.
            /// </summary>
            public char UnicodeChar => this.cUnicodeChar;

            /// <summary>
            /// Gets the ASCII key for this key event.
            /// </summary>
            public byte AsciiChar => this.bAsciiChar;

            /// <summary>
            /// Gets a value specifying the control key state for this key event.
            /// </summary>
            public ConsoleControlKeyState ControlKeyState => (ConsoleControlKeyState)this.dwControlKeyState;
        }

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ConsoleWindowBufferSizeEventInfo
        {
            [FieldOffset(0)]
            private readonly Coord dwSize;

            /// <summary>
            /// Gets a value indicating the size of the screen buffer, in character cell columns and rows.
            /// </summary>
            public Coord Size => this.dwSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ConsoleMenuEventInfo
        {
            [FieldOffset(0)]
            private int dwCommandId;

            /// <summary>
            /// The ID of the menu command.
            /// </summary>
            public int CommandId => this.dwCommandId;
        }

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ConsoleFocusEventInfo
        {
            [FieldOffset(0)]
            private readonly uint bSetFocus;

            /// <summary>
            /// Gets a value indicating whether focus is gained or lost.
            /// </summary>
            public bool SetFocus => this.bSetFocus != 0;
        }

        private enum ConsoleControlEventType : int
        {
            /// <summary>
            /// A CTRL+C signal was received, either from keyboard input or from a signal generated by the
            /// GenerateConsoleCtrlEvent function.
            /// </summary>
            CtrlC = 0,

            /// <summary>
            /// A CTRL+BREAK signal was received, either from keyboard input or from a signal generated by
            /// GenerateConsoleCtrlEvent.
            /// </summary>
            CtrlBreak = 1,

            /// <summary>
            /// A signal that the system sends to all processes attached to a console when the user closes the console
            /// (either by clicking Close on the console window's window menu, or by clicking the End Task button
            /// command from Task Manager).
            /// </summary>
            CtrlClose = 2,

            // 3 and 4 are reserved, per WinCon.h
            /// <summary>
            /// A signal that the system sends to all console processes when a user is logging off. 
            /// </summary>
            CtrlLogoff = 5,

            /// <summary>
            /// A signal that the system sends to all console processes when the system is shutting down. 
            /// </summary>
            CtrlShutdown = 6
        }

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ConsoleInputEventInfo
        {
            [FieldOffset(0)] public readonly ConsoleInputEventType EventType;

            /// <summary>
            /// Key event information if this is a keyboard event.
            /// </summary>
            [FieldOffset(4)] public readonly ConsoleKeyEventInfo KeyEvent;

            /// <summary>
            /// Mouse event information if this is a mouse event.
            /// </summary>
            [FieldOffset(4)] public readonly ConsoleMouseEventInfo MouseEvent;

            /// <summary>
            /// Window buffer size information if this is a window buffer size event.
            /// </summary>
            [FieldOffset(4)] public readonly ConsoleWindowBufferSizeEventInfo WindowBufferSizeEvent;

            /// <summary>
            /// Menu event information if this is a menu event.
            /// </summary>
            [FieldOffset(4)] public readonly ConsoleMenuEventInfo MenuEvent;

            /// <summary>
            /// Focus event information if this is a focus event.
            /// </summary>
            [FieldOffset(4)] public readonly ConsoleFocusEventInfo FocusEvent;
        }
    }
}
