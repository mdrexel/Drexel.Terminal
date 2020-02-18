using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Drexel.Terminal.Primitives;
using Drexel.Terminal.Win32;
using Microsoft.Win32.SafeHandles;

namespace Drexel.Terminal.Sink.Win32
{
    public sealed class TerminalSink : ITerminalSink
    {
        private const int STD_OUTPUT_HANDLE = -11;

        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_CLOSE = 0xF060;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        private readonly SafeFileHandle outputHandle;
        private readonly SafeFileHandle outputStreamHandle;

        internal TerminalSink()
        {
            this.outputHandle = TerminalInstance.CreateFileW(
                "CONOUT$",
                0x40000000,
                2,
                IntPtr.Zero,
                FileMode.Open,
                0,
                IntPtr.Zero);

            if (this.outputHandle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            this.outputStreamHandle = TerminalInstance.GetStdHandle(STD_OUTPUT_HANDLE);
            if (this.outputStreamHandle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        public Coord CursorPosition
        {
            get =>this.ScreenBufferInfo.CursorPosition;
            set
            {
                if (!SetConsoleCursorPosition(this.outputStreamHandle, value))
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

        private ConsoleScreenBufferInfo ScreenBufferInfo
        {
            get
            {
                if (!GetConsoleScreenBufferInfo(this.outputStreamHandle, out ConsoleScreenBufferInfo info))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                return info;
            }
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
        private static extern bool WriteConsoleOutputCharacterW(
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

        public bool Write() => this.AdvanceCursor();

        public bool Write(CharInfo charInfo) => this.Write(charInfo, this.CursorPosition) && this.AdvanceCursor();

        public bool Write(CharInfo[] buffer) => this.WriteAndAdvance(buffer, this.CursorPosition, true);

        public bool Write(CharInfo charInfo, Coord destination) => this.Write(
            charInfo.Character,
            charInfo.Colors.ToCharAttributes(),
            destination);

        public bool Write(CharInfo[] buffer, Coord destination) => this.WriteAndAdvance(buffer, destination, false);

        public bool Write(CharInfo[,] buffer, Coord topLeft) => this.Write(
            buffer,
            topLeft,
            new Rectangle(Coord.Zero, buffer.ToCoord()));

        public bool Write(CharInfo[,] buffer, Coord topLeft, Rectangle window) => this.WriteAndAdvance(
            buffer,
            topLeft,
            window,
            false,
            Coord.Zero);

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

        internal void Dispose()
        {
            this.outputHandle.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Write(ConsoleCharUnion union, ConsoleCharAttributes attributes, Coord destination)
        {
            unsafe
            {
                return
                    WriteConsoleOutputCharacterW(
                        this.outputHandle,
                        new IntPtr(&union),
                        1,
                        destination,
                        out _)
                    && WriteConsoleOutputAttribute(
                        this.outputHandle,
                        new IntPtr(&attributes),
                        1,
                        destination,
                        out _);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool WriteAndAdvance(CharInfo[] buffer, Coord destination, bool advance)
        {
            ConsoleScreenBufferInfo bufferInfo = this.ScreenBufferInfo;
            int quotient = Utilities.DivRem(
                destination.X + buffer.Length,
                bufferInfo.BufferWindow.HorizontalSpan,
                out int remainder);

            CharInfo[,] output = new CharInfo[
                remainder != 0 ? quotient + 1 : quotient,
                bufferInfo.BufferWindow.HorizontalSpan];

            int height = output.GetHeight();
            int width = output.GetWidth();
            int index = 0;
            int xPos = destination.X;
            for (int yPos = 0; yPos < height; yPos++)
            {
                for (; xPos < width && index < buffer.Length; xPos++, index++)
                {
                    output[yPos, xPos] = buffer[index];
                }

                xPos = 0;
            }

            return this.WriteAndAdvance(
                output,
                destination,
                new Rectangle(Coord.Zero, output.ToCoord()),
                advance,
                new Coord((short)remainder, (short)(destination.Y + quotient)));
        }

        public bool WriteAndAdvance(
            CharInfo[,] buffer,
            Coord topLeft,
            Rectangle window,
            bool advance,
            Coord advanceToUndelayed)
        {
            (Coord windowTopLeft, Coord windowBottomRight) = window.Decompose();

            bool containsDelay = false;
            short height = buffer.GetHeight();
            short width = buffer.GetWidth();
            ConsoleCharInfo[,] output = new ConsoleCharInfo[height, width];
            for (short yPos = 0; yPos < height; yPos++)
            {
                for (short xPos = 0; xPos < width; xPos++)
                {
                    CharInfo input = buffer[yPos, xPos];
                    containsDelay |= input.Delay > 0;
                    output[yPos, xPos] = new ConsoleCharInfo(input.Character, input.Colors.ToCharAttributes());
                }
            }

            if (containsDelay)
            {
                short maxX = Math.Min(windowBottomRight.X, width);
                short maxY = Math.Min(windowBottomRight.Y, height);

                bool success = true;
                for (short y = windowTopLeft.Y; y < maxY; y++)
                {
                    for (short x = windowTopLeft.X; x < maxX; x++)
                    {
                        ConsoleCharInfo @char = output[y, x];
                        success &= this.Write(
                            @char.Char,
                            @char.Attributes,
                            new Coord((short)(topLeft.X + x), (short)(topLeft.Y + y)));
                        if (advance && @char != default)
                        {
                            success &= this.AdvanceCursor();
                        }

                        Thread.Sleep(buffer[y, x].Delay);
                    }
                }

                return success;
            }
            else
            {
                Coord adjustedTopLeft = topLeft + windowTopLeft;
                Coord adjustedBottomRight = topLeft + windowBottomRight;
                Rectangle rect = new Rectangle(
                    adjustedTopLeft.X,
                    adjustedTopLeft.Y,
                    adjustedBottomRight.X,
                    adjustedBottomRight.Y);

                bool success = true;
                unsafe
                {
                    fixed (ConsoleCharInfo* pinned = output)
                    {
                        IntPtr pointer = (IntPtr)pinned;
                        success &= WriteConsoleOutputW(
                            this.outputHandle,
                            pointer,
                            buffer.ToCoord(),
                            windowTopLeft,
                            in rect);
                    }
                }

                if (advance)
                {
                    success &= SetConsoleCursorPosition(this.outputHandle, advanceToUndelayed);
                }

                return success;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AdvanceCursor()
        {
            Coord currentPosition = this.CursorPosition;
            if (!SetConsoleCursorPosition(
                this.outputHandle,
                currentPosition + Coord.OneXOffset))
            {
                return SetConsoleCursorPosition(
                    this.outputHandle,
                    new Coord(0, (short)(currentPosition.Y + 1)));
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AdvanceCursor(int delta)
        {
            ConsoleScreenBufferInfo bufferInfo = this.ScreenBufferInfo;
            int quotient = Utilities.DivRem(
                bufferInfo.CursorPosition.X + delta,
                bufferInfo.BufferWindow.HorizontalSpan,
                out int remainder);

            Coord newPosition = new Coord((short)remainder, (short)(bufferInfo.CursorPosition.Y + quotient));
            return SetConsoleCursorPosition(this.outputHandle, newPosition);
        }
    }
}
