using System;

namespace Drexel.Terminal.Sink
{
    /// <summary>
    /// Extension methods for working with <see cref="ITerminalSink"/>s.
    /// </summary>
    public static class TerminalSinkExtensionMethods
    {
        /// <summary>
        /// Gets the <see cref="Coord"/> that the <see cref="ITerminalSink.CursorPosition"/> of <paramref name="sink"/>
        /// would need to be set to in order to reverse the cursor's movement by one position.
        /// </summary>
        /// <param name="sink">
        /// The sink to retrieve the cursor position of.
        /// </param>
        /// <param name="width">
        /// The width of the terminal.
        /// </param>
        /// <returns>
        /// The <see cref="Coord"/> that the <see cref="ITerminalSink.CursorPosition"/> of <paramref name="sink"/>
        /// would need to be set to in order to reverse the cursor's movement by one position.
        /// </returns>
        public static Coord GetReverseCursorPosition(this ITerminalSink sink, ushort width)
        {
            if (sink is null)
            {
                throw new ArgumentNullException(nameof(sink));
            }

            Coord currentPosition = sink.CursorPosition;
            if (currentPosition.X - 1 < 0)
            {
                return new Coord((short)(width - 1), (short)(currentPosition.Y - 1));
            }
            else
            {
                return new Coord((short)(currentPosition.X - 1), currentPosition.Y);
            }
        }
    }
}
