using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Game.Output
{
    public sealed class Sink : IDisposable, ISink
    {
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        // TODO: This isn't thread safe, but it doesn't matter for this tiny game
        private static bool Active = false;

        private readonly SafeFileHandle handle;
        private readonly short height;
        private readonly short width;

        public Sink(
            string name,
            short height,
            short width,
            Action? onCancelKeyPress = null,
            uint codePage = 65001)
        {
            if (Sink.Active)
            {
                throw new InvalidOperationException("Can only have one sink open at a time.");
            }

            Sink.Active = true;

            this.height = height;
            this.width = width;

            SetConsoleOutputCP(codePage);
            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Console.BufferWidth = width;
            Console.BufferHeight = height;

            Console.CancelKeyPress +=
                (obj, e) =>
                {
                    bool? allowExit = this.OnExitRequested?.Invoke();
                    if (allowExit.HasValue && !allowExit.Value)
                    {
                        e.Cancel = true;
                    }
                    else if (onCancelKeyPress != null)
                    {
                        onCancelKeyPress.Invoke();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                };

            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            Console.CursorVisible = false;
            Console.Title = name;

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
                throw new InvalidOperationException("Failed to get safe handle of console output.");
            }
        }

        public delegate bool AllowExitDelegate();

        public event AllowExitDelegate? OnExitRequested;

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleOutputW(
          SafeFileHandle hConsoleOutput,
          IntPtr lpBuffer,////CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          in SmallRect lpWriteRegion);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleW(
            SafeFileHandle hConsoleOutput,
            string lpBuffer,
            uint nNumberOfCharsToWrite,
            out uint lpNumberOfCharsWritten,
            IntPtr lpReserved);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleOutputCharacter(
            SafeFileHandle hConsoleOutput,
            IntPtr lpCharacter,////char[] lpCharacter,
            int nLength,
            Coord dwWriteCoord,
            out int lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleOutputAttribute(
            SafeFileHandle hConsoleOutput,
            IntPtr lpAttribute,////CharAttributes[] lpAttribute,
            int nLength,
            Coord dwWriteCoord,
            out int lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetConsoleCursorPosition(
            SafeFileHandle hConsoleOutput,
            Coord dwCursorPosition);

        public void Write(string content)
        {
            WriteConsoleW(this.handle, content, (uint)content.Length, out _, (IntPtr)0);
        }

        public void Write(
            string content,
            CharColors colors,
            Coord origin,
            int millisecondsDelay,
            DelayMode delayMode)
        {
            SetConsoleCursorPosition(this.handle, origin);
            switch (delayMode)
            {
                case DelayMode.AtEnd:
                    foreach (char @char in content)
                    {
                        this.Write(@char, colors);
                    }

                    Thread.Sleep(millisecondsDelay);
                    break;
                case DelayMode.PerCharacter:
                    foreach (char @char in content)
                    {
                        this.Write(new CharDelay(new CharInfo(@char, colors), millisecondsDelay));
                    }

                    break;
                case DelayMode.PerWord:
                    string[] split = content.Split(' ');
                    for (int counter = 0; counter < split.Length; counter++)
                    {
                        this.Write(split[counter], colors);

                        if (counter < split.Length - 1)
                        {
                            this.Write(' ', colors);
                            Thread.Sleep(millisecondsDelay);
                        }
                    }

                    break;
            }
        }

        public void Write(string content, CharColors color)
        {
            foreach (char @char in content)
            {
                this.Write(@char, color);
            }
        }

        
        public void Write(CharDelay charDelay, Coord destination)
        {
            this.WriteInternal(charDelay.CharInfo, destination);
            Thread.Sleep(charDelay.DelayInMilliseconds);
        }

        public void Write(CharInfo charInfo, Coord destination)
        {
            this.WriteInternal(charInfo, destination);
        }

        public void Write(CharDelay charDelay)
        {
            this.Write(charDelay.CharInfo);
        }

        public void Write(CharInfo charInfo)
        {
            this.WriteInternal(charInfo, new Coord((short)Console.CursorLeft, (short)Console.CursorTop));
            this.AdvanceCursor();
        }

        public void Write(char @char, CharColors colors) =>
            this.Write(new CharInfo(@char, colors));

        public void WriteRegion(
            CharInfo[,] buffer,
            Coord topLeft)
        {
            Coord size = buffer.ToCoord();
            Coord offset = topLeft + size;
            SmallRect rect = new SmallRect(topLeft.X, topLeft.Y, offset.X, offset.Y);
            unsafe
            {
                fixed (CharInfo* pinned = buffer)
                {
                    IntPtr pointer = (IntPtr)pinned;
                    WriteConsoleOutputW(
                        this.handle,
                        pointer,
                        new Coord(size.X, size.Y),
                        new Coord(0, 0),
                        in rect);
                }
            }
        }

        public void WriteRegion(
            CharDelay[,] buffer,
            Coord topLeft)
        {
            this.SetCursorPosition(topLeft);
            for (int y = 0; y < buffer.GetHeight(); y++)
            {
                for (int x = 0; x < buffer.GetWidth(); x++)
                {
                    this.Write(buffer[y, x]);
                }
            }
        }

        public void Dispose()
        {
            Sink.Active = false;
            this.handle.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdvanceCursor()
        {
            if (!SetConsoleCursorPosition(
                this.handle,
                new Coord((short)(Console.CursorLeft + 1), (short)Console.CursorTop)))
            {
                SetConsoleCursorPosition(
                    this.handle,
                    new Coord(0, (short)(Console.CursorTop + 1)));
            }
        }

        private void SetCursorPosition(Coord destination)
        {
            SetConsoleCursorPosition(this.handle, destination);
        }

        private void WriteInternal(CharInfo charInfo, Coord destination)
        {
            unsafe
            {
                WriteConsoleOutputAttribute(
                    this.handle,
                    new IntPtr(&(charInfo.Attributes)),
                    1,
                    destination,
                    out _);
                WriteConsoleOutputCharacter(
                    this.handle,
                    new IntPtr(&(charInfo.Char)),
                    1,
                    destination,
                    out _);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly ref struct SmallRect
        {
            public readonly short Left;
            public readonly short Top;
            public readonly short Right;
            public readonly short Bottom;

            public SmallRect(
                short left,
                short top,
                short right,
                short bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }
    }
}
