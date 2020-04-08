using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Drexel.Terminal.Internals;
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

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        /// <summary>
        /// Writes a region.
        /// </summary>
        /// <param name="hConsoleOutput">
        /// A handle to the console output.
        /// </param>
        /// <param name="lpBuffer">
        /// A pointer to the data to write.
        /// </param>
        /// <param name="dwBufferSize">
        /// The size of the data to write, given in terms of height/width.
        /// </param>
        /// <param name="dwBufferCoord">
        /// The BUFFER-space coordinate within the buffer to start writing data from.
        /// </param>
        /// <param name="lpWriteRegion">
        /// The SCREEN-space window to write data to.
        /// </param>
        /// <returns></returns>
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

        public bool WriteLine() => SetConsoleCursorPosition(
            this.outputHandle,
            new Coord(0, (short)(this.CursorPosition.Y + 1)));

        public bool Write(CharInfo charInfo)
        {
            bool success = true;
            if (charInfo != default)
            {
                success = this.Write(charInfo, this.CursorPosition);
            }

            return success && this.AdvanceCursor();
        }

        public bool Write(CharInfo[] buffer) => this.WriteAndAdvance(buffer, this.CursorPosition, true);

        public bool WriteLine(CharInfo[] buffer)
        {
            bool result = this.Write(buffer);
            return result && this.WriteLine();
        }

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
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            int x0 = line.TopLeft.X;
            int y0 = line.TopLeft.Y;
            int x1 = line.BottomRight.X;
            int y1 = line.BottomRight.Y;

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;

            CharInfo[][,] columns = new CharInfo[line.Pattern.GetWidth()][,];
            for (int counter = 0; counter < columns.Length; counter++)
            {
                columns[counter] = new CharInfo[line.Pattern.GetHeight(), 1];
                for (int position = 0; position < columns[counter].GetHeight(); position++)
                {
                    columns[counter][position, 0] = line.Pattern[position, counter];
                }
            }

            // TODO: Instead of calling this.Write directly, should this be filling a buffer, which we then write out
            // with a single call?
            bool success = true;
            for (int column = 0; true; Utilities.DivRem(++column, columns.Length, out column))
            {
                success &= this.Write(columns[column], new Coord((short)x0, (short)y0));
                if (x0 == x1 && y0 == y1)
                {
                    return success;
                }

                e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dy)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        public bool Write(Fill fill)
        {
            if (fill is null)
            {
                throw new ArgumentNullException(nameof(fill));
            }

            return this.Write(fill.GetFullSize(), fill.TopLeft);
        }

        public bool Write(Polygon polygon)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            this.outputHandle.Dispose();
        }

        internal void DisableResize()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                ////DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
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
                quotient == 0 ? remainder : bufferInfo.BufferWindow.HorizontalSpan];

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
                new Coord(0, destination.Y),
                new Rectangle(Coord.Zero, output.ToCoord()),
                advance,
                new Coord((short)remainder, (short)(destination.Y + quotient)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool WriteAndAdvance(
            CharInfo[,] buffer,
            Coord topLeft,
            Rectangle window, // Window is in buffer-space, not screen-space
            bool advance,
            Coord advanceToUndelayed)
        {
            (Coord windowTopLeft, Coord windowBottomRight) = window.Decompose();

            bool containsDelay = false;
            short height = buffer.GetHeight();
            short width = buffer.GetWidth();
            ConsoleCharInfo[,] output = new ConsoleCharInfo[height, width];
            bool needsFiltering = false;
            for (short yPos = windowTopLeft.Y; yPos < windowBottomRight.Y; yPos++)
            {
                for (short xPos = windowTopLeft.X; xPos < windowBottomRight.X; xPos++)
                {
                    CharInfo input = buffer[yPos, xPos];
                    containsDelay |= input.Delay > 0;
                    if (input.Character != default || input.Colors != default)
                    {
                        output[yPos, xPos] = new ConsoleCharInfo(input.Character, input.Colors.ToCharAttributes());
                    }
                    else
                    {
                        needsFiltering = true;
                    }
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
                        if (@char != default)
                        {
                            success &= this.Write(
                                @char.Char,
                                @char.Attributes,
                                new Coord((short)(topLeft.X + x), (short)(topLeft.Y + y)));
                        }

                        if (advance)
                        {
                            success &= this.AdvanceCursor();
                        }

                        ushort delay = buffer[y, x].Delay;
                        if (delay > 0)
                        {
                            Thread.Sleep(delay);
                        }
                    }
                }

                return success;
            }
            else
            {
                bool success = true;
                if (needsFiltering)
                {
                    // Because we want to treat values of default in the buffer as transparent, we need to subdivide
                    // the supplied buffer into chunks that don't include the default values. Then, we'll draw those
                    // sub-buffers, so that we never write a default value to the console.
                    List<(ConsoleCharInfo[,], Coord)> subBuffers = new List<(ConsoleCharInfo[,], Coord)>();

                    #region Really Bad Implementation
                    List<(Coord, Coord)> inflections = new List<(Coord, Coord)>();
                    for (short yPos = windowTopLeft.Y; yPos < windowBottomRight.Y; yPos++)
                    {
                        short lastNonDefault = -1;
                        for (short xPos = windowTopLeft.X; xPos < windowBottomRight.X; xPos++)
                        {
                            if (output[yPos, xPos] == default)
                            {
                                if (lastNonDefault != -1)
                                {
                                    inflections.Add((new Coord(lastNonDefault, yPos), new Coord(xPos, yPos)));
                                    lastNonDefault = -1;
                                }
                            }
                            else
                            {
                                if (lastNonDefault == -1)
                                {
                                    lastNonDefault = xPos;
                                }
                            }
                        }

                        if (lastNonDefault != -1)
                        {
                            inflections.Add((new Coord(lastNonDefault, yPos), new Coord(windowBottomRight.X, yPos)));
                        }
                    }

                    foreach ((Coord, Coord) inflection in inflections)
                    {
                        Coord size = inflection.Item2 - inflection.Item1;
                        ConsoleCharInfo[,] subBuffer = new ConsoleCharInfo[size.Y + 1, size.X];
                        for (int yIn = inflection.Item1.Y, yOut = 0; yIn < inflection.Item2.Y + 1; yIn++, yOut++)
                        {
                            for (int xIn = inflection.Item1.X, xOut = 0; xIn < inflection.Item2.X; xIn++, xOut++)
                            {
                                subBuffer[yOut, xOut] = output[yIn, xIn];
                            }
                        }

                        subBuffers.Add((subBuffer, inflection.Item1));
                    }
                    #endregion Really Bad Implementation

                    foreach ((ConsoleCharInfo[,], Coord) subBuffer in subBuffers)
                    {
                        unsafe
                        {
                            fixed (ConsoleCharInfo* pinned = subBuffer.Item1)
                            {
                                IntPtr pointer = (IntPtr)pinned;
                                Rectangle rect = new Rectangle(
                                    windowTopLeft + subBuffer.Item2 + topLeft,
                                    windowBottomRight + subBuffer.Item2 + topLeft);

                                success &= WriteConsoleOutputW(
                                    this.outputHandle,
                                    pointer,
                                    subBuffer.Item1.ToCoord(),
                                    Coord.Zero,
                                    in rect);
                            }
                        }
                    }
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
