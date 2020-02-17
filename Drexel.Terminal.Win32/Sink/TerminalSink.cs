using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Drexel.Terminal.Primitives;
using Drexel.Terminal.Win32;
using Microsoft.Win32.SafeHandles;

namespace Drexel.Terminal.Sink.Win32
{
    public sealed class TerminalSink : ITerminalSink
    {
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        private readonly SafeFileHandle handle;

        internal TerminalSink(SafeFileHandle handle)
        {
            this.handle = handle;
        }

        public Coord CursorPosition
        {
            get
            {
                if (!GetConsoleScreenBufferInfo(this.handle, out ConsoleScreenBufferInfo info))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                return info.CursorPosition;
            }
            set
            {
                if (!SetConsoleCursorPosition(this.handle, value))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
        }

        public ConsoleCodePage CodePage
        {
            get => (ConsoleCodePage)GetConsoleOutputCP();
            set => SetConsoleOutputCP((uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write()
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

        public void Write(CharInfo charInfo)
        {
            throw new NotImplementedException();
        }

        public bool Write(CharInfo charInfo, Coord destination)
        {
            throw new NotImplementedException();
        }

        public bool Write(CharInfo[,] buffer, Coord topLeft)
        {
            throw new NotImplementedException();
        }

        public bool Write(CharInfo[,] buffer, Coord topLeft, Rectangle window)
        {
            throw new NotImplementedException();
        }

        public bool Write(Line line)
        {
            throw new NotImplementedException();
        }

        public bool Write(Fill fill)
        {
            throw new NotImplementedException();
        }

        public bool Write(Polygon polygon)
        {
            throw new NotImplementedException();
        }

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetConsoleOutputCP();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleOutputW(
          SafeFileHandle hConsoleOutput,
          IntPtr lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          in Rectangle lpWriteRegion);

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
            IntPtr lpCharacter,
            int nLength,
            Coord dwWriteCoord,
            out int lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool WriteConsoleOutputAttribute(
            SafeFileHandle hConsoleOutput,
            IntPtr lpAttribute,
            int nLength,
            Coord dwWriteCoord,
            out int lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool SetConsoleCursorPosition(
            SafeFileHandle hConsoleOutput,
            Coord dwCursorPosition);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfo(
            SafeFileHandle hConsoleOutput,
            out ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);
    }
}
