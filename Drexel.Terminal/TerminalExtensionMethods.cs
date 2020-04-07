﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Terminal.Sink;

namespace Drexel.Terminal
{
    /// <summary>
    /// Extension methods for working with <see cref="ITerminal"/>s.
    /// </summary>
    public static class TerminalExtensionMethods
    {
        /// <summary>
        /// Reads the next line of characters from the specified <paramref name="terminal"/>.
        /// </summary>
        /// <param name="terminal">
        /// The <see cref="ITerminal"/> to read the next line of characters from.
        /// </param>
        /// <param name="cancellationToken">
        /// Allows the caller to cancel this task.
        /// </param>
        /// <returns>
        /// The next line of characters from the specified <paramref name="terminal"/>.
        /// </returns>
        public static Task<string> ReadLineAsync(
            this ITerminal terminal,
            CancellationToken cancellationToken = default)
        {
            if (terminal is null)
            {
                throw new ArgumentNullException(nameof(terminal));
            }

            return terminal.Source.ReadLineAsync(
                x =>
                {
                    if (x.Key == TerminalKey.Backspace)
                    {
                        Coord backspace = terminal.Sink.GetReverseCursorPosition(terminal.Width);
                        terminal.Sink.Write(new CharInfo(' ', TerminalColors.Default), backspace);
                        terminal.Sink.CursorPosition = backspace;
                    }
                    else
                    {
                        terminal.Sink.Write(new CharInfo(x.KeyChar, TerminalColors.Default));
                    }
                },
                true,
                cancellationToken);
        }
    }
}
