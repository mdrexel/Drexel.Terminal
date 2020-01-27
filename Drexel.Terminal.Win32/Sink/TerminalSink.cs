using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Terminal.Primitives;
using Microsoft.Win32.SafeHandles;

namespace Drexel.Terminal.Sink.Win32
{
    public sealed class TerminalSink : ITerminalSink, IDisposable
    {
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        private static Lazy<TerminalSink> backingInstance = new Lazy<TerminalSink>(
            () => new TerminalSink(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly SafeFileHandle handle;

        private TerminalSink(uint codePage = 65001)
        {
            SetConsoleOutputCP(codePage);

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

        public static TerminalSink Singleton => backingInstance.Value;

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

        public void Dispose()
        {
            backingInstance = new Lazy<TerminalSink>(
                () =>
                {
                    throw new InvalidOperationException("Cannot access terminal after singleton has been disposed.");
                });

            this.handle.Dispose();
        }

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

        private void SetCursorPosition(Coord destination)
        {
            SetConsoleCursorPosition(this.handle, destination);
        }
    }
}
