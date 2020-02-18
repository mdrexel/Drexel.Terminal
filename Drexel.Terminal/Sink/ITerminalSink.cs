using Drexel.Terminal.Primitives;

namespace Drexel.Terminal.Sink
{
    /// <summary>
    /// Represents a sink that can emit terminal output.
    /// </summary>
    public interface ITerminalSink
    {
        /// <summary>
        /// Gets or sets the position of the cursor.
        /// </summary>
        Coord CursorPosition { get; set; }

        /// <summary>
        /// Advances the cursor.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is if the cursor would be located outside the writeable
        /// area of this sink.
        /// </returns>
        bool Write();

        /// <summary>
        /// Advances the cursor to the start of the next line.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is if the cursor would be located outside the writeable
        /// area of this sink.
        /// </returns>
        bool WriteLine();

        /// <summary>
        /// Writes the specified <see cref="CharInfo"/> <paramref name="charInfo"/> and advances the cursor.
        /// </summary>
        /// <param name="charInfo">
        /// The <see cref="CharInfo"/> to write.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is if the specified <paramref name="charInfo"/> would be
        /// written outside the writeable area of this sink.
        /// </returns>
        bool Write(CharInfo charInfo);

        /// <summary>
        /// Writes the specified <see cref="T:CharInfo[]"/> <paramref name="buffer"/> and advances the cursor.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="T:CharInfo[]"/> to write.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is if the specified <paramref name="buffer"/> extends past
        /// the writeable area of this sink.
        /// </returns>
        bool Write(CharInfo[] buffer);

        /// <summary>
        /// Writes the specified <see cref="T:CharInfo[]"/> <paramref name="buffer"/> and advances the cursor to the
        /// start of the next line.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="T:CharInfo[]"/> to write.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is if the specified <paramref name="buffer"/> extends past
        /// the writeable area of this sink.
        /// </returns>
        bool WriteLine(CharInfo[] buffer);

        /// <summary>
        /// Writes the specified <see cref="CharInfo"/> <paramref name="charInfo"/> at the coordinate specified by the
        /// <see cref="Coord"/> <paramref name="destination"/>, if possible. If the write operation completed, returns
        /// <see langword="true"/>; otherwise, returns <see langword="false"/>. The most common reason for an
        /// incomplete write operation is if the specified <paramref name="destination"/> is outside the writeable area
        /// of this sink.
        /// </summary>
        /// <param name="charInfo">
        /// The <see cref="CharInfo"/> to write.
        /// </param>
        /// <param name="destination">
        /// The destination to write <paramref name="charInfo"/> to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is if the specified <paramref name="destination"/> is
        /// outside the writeable area of this sink.
        /// </returns>
        bool Write(CharInfo charInfo, Coord destination);

        /// <summary>
        /// Writes the specified <see cref="T:CharInfo[]"/> <paramref name="buffer"/> at the coordinate specified by
        /// the <see cref="Coord"/> <paramref name="destination"/>, if possible. If the write operation completed,
        /// returns <see langword="true"/>; otherwise, returns <see langword="false"/>. The most common reason for an
        /// incomplete write operation is if the specified <paramref name="buffer"/> extends past the writeable area of
        /// this sink.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="T:CharInfo[]"/> to write.
        /// </param>
        /// <param name="destination">
        /// The destination to start writing <paramref name="buffer"/> from.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is if the specified <paramref name="buffer"/> extends past
        /// the writeable area of this sink.
        /// </returns>
        bool Write(CharInfo[] buffer, Coord destination);

        /// <summary>
        /// Writes the specified <see cref="T:CharInfo[,]"/> <paramref name="buffer"/> starting from (inclusive)
        /// the coordinate specified by the <see cref="Coord"/> <paramref name="topLeft"/>, if possible. If the write
        /// operation completed, returns <see langword="true"/>; otherwise, returns <see langword="false"/>. The most
        /// common reason for an incomplete write operation is because some or all of <paramref name="buffer"/> was
        /// outside the writeable area of this sink.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="T:CharInfo[,]"/> to write.
        /// </param>
        /// <param name="topLeft">
        /// The top-left coordinate from which to begin writing <paramref name="buffer"/> (inclusive).
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is because some or all of <paramref name="buffer"/> was
        /// outside the writeable area of this sink.
        /// </returns>
        bool Write(CharInfo[,] buffer, Coord topLeft);

        /// <summary>
        /// Writes the specified <see cref="T:CharInfo[,]"/> <paramref name="buffer"/> starting from (inclusive)
        /// the coordinate specified by the <see cref="Coord"/> <paramref name="topLeft"/> after applying the specified
        /// <see cref="Rectangle"/> <paramref name="window"/>, if possible. Applying <paramref name="window"/> means
        /// that only the values of <paramref name="buffer"/> that fall within <paramref name="window"/> will be
        /// written to the sink. <paramref name="window"/> is given in absolute coordinates, and is not relative to
        /// <paramref name="topLeft"/> or <paramref name="buffer"/>. If the write operation completed, returns
        /// <see langword="true"/>; otherwise, returns <see langword="false"/>. The most common reason for an
        /// incomplete write operation is because some or all of <paramref name="buffer"/> was outside
        /// <paramref name="window"/>.
        /// </summary>
        /// <param name="buffer">
        /// The <see cref="T:CharInfo[,]"/> to write.
        /// </param>
        /// <param name="topLeft">
        /// The top-left coordinate from which to begin writing <paramref name="buffer"/> (inclusive).
        /// </param>
        /// <param name="window">
        /// The window to apply to the write operation
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is because some or all of <paramref name="buffer"/> was
        /// outside the writeable area of this sink after applying <paramref name="window"/>.
        /// </returns>
        bool Write(CharInfo[,] buffer, Coord topLeft, Rectangle window);

        /// <summary>
        /// Writes the specified <see cref="Line"/> <paramref name="line"/>. If the write operation completed, returns
        /// <see langword="true"/>; otherwise, returns <see langword="false"/>. The most common reason for an
        /// incomplete write operation is because some or all of <paramref name="line"/> was outside the writeable area
        /// of this sink.
        /// </summary>
        /// <param name="line">
        /// The <see cref="Line"/> to write.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is because some or all of <paramref name="line"/> was
        /// outside the writeable area of this sink.
        /// </returns>
        bool Write(Line line);

        /// <summary>
        /// Writes the specified <see cref="Fill"/> <paramref name="fill"/>. If the write operation completed, returns
        /// <see langword="true"/>; otherwise, returns <see langword="false"/>. The most common reason for an
        /// incomplete write operation is because some or all of <paramref name="fill"/> was outside the writeable area
        /// of this sink.
        /// </summary>
        /// <param name="fill">
        /// The <see cref="Fill"/> to write.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is because some or all of <paramref name="fill"/> was
        /// outside the writeable area of this sink.
        /// </returns>
        bool Write(Fill fill);

        /// <summary>
        /// Writes the specified <see cref="Polygon"/> <paramref name="polygon"/>. If the write operation completed,
        /// returns <see langword="true"/>; otherwise, returns <see langword="false"/>. The most common reason for an
        /// incomplete write operation is because some or all of <paramref name="polygon"/> was outside the writeable
        /// area of this sink.
        /// </summary>
        /// <param name="polygon">
        /// The <see cref="Polygon"/> to write.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the write operation completed; otherwise, <see langword="false"/>. The most
        /// common reason for an incomplete write operation is because some or all of <paramref name="polygon"/> was
        /// outside the writeable area of this sink.
        /// </returns>
        bool Write(Polygon polygon);
    }
}
