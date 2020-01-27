using System;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Represents a terminal key that was pressed, including modifier keys.
    /// </summary>
    public readonly struct TerminalKeyInfo : IEquatable<TerminalKeyInfo>
    {
        private static readonly HashSet<TerminalKey> CachedKeys =
            new HashSet<TerminalKey>(Enum.GetValues(typeof(TerminalKey)).Cast<TerminalKey>());

        /// <summary>
        /// The Unicode character associated with this key info.
        /// </summary>
        public readonly char KeyChar;

        /// <summary>
        /// The terminal key associated with this key info.
        /// </summary>
        public readonly TerminalKey Key;

        /// <summary>
        /// The modifiers associated with this key info.
        /// </summary>
        public readonly TerminalModifiers Modifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalKeyInfo"/> class.
        /// </summary>
        /// <param name="keyChar">
        /// The Unicode character associated with this instance.
        /// </param>
        /// <param name="key">
        /// The terminal key represented by this instance.
        /// </param>
        /// <param name="shift">
        /// Indicates whether the shift key was pressed.
        /// </param>
        /// <param name="alt">
        /// Indicates whether the alt key was pressed.
        /// </param>
        /// <param name="control">
        /// Indicates whether the control key was pressed.
        /// </param>
        public TerminalKeyInfo(
            char keyChar,
            TerminalKey key,
            bool shift,
            bool alt,
            bool control)
        {
            if (!TerminalKeyInfo.CachedKeys.Contains(key))
            {
                throw new ArgumentException("Unrecognized key.", nameof(key));
            }

            this.Key = key;
            this.KeyChar = keyChar;

            this.Modifiers = default;
            if (shift)
            {
                this.Modifiers |= TerminalModifiers.Shift;
            }

            if (alt)
            {
                this.Modifiers |= TerminalModifiers.Alt;
            }

            if (control)
            {
                this.Modifiers |= TerminalModifiers.Control;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> represent the same
        /// key info; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> represent the same key info;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(TerminalKeyInfo left, TerminalKeyInfo right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not represent the
        /// same key info; otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="left">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <param name="right">
        /// The <see cref="Rectangle"/> on the left side of the comparison.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> do not represent the same
        /// key info; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(TerminalKeyInfo left, TerminalKeyInfo right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether this instance and the specified <see cref="object"/> <paramref name="obj"/> are equal.
        /// </summary>
        /// <param name="obj">
        /// The object this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance is equal to the specified <paramref name="obj"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is TerminalKeyInfo other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this <see cref="TerminalKeyInfo"/> and the specified <see cref="TerminalKeyInfo"/>
        /// <paramref name="other"/> represent the same key info.
        /// </summary>
        /// <param name="other">
        /// The <see cref="TerminalKeyInfo"/> this instance should compare itself to.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if this instance and <paramref name="other"/> represent the same key info;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(TerminalKeyInfo other)
        {
            return this.KeyChar == other.KeyChar
                && this.Key == other.Key
                && this.Modifiers == other.Modifiers;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="TerminalKeyInfo"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="TerminalKeyInfo"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 39079;
            unchecked
            {
                hash = (hash * 31) + this.Modifiers.GetHashCode();
                hash = (hash * 31) + this.Key.GetHashCode();
                hash = (hash * 31) + this.KeyChar.GetHashCode();
            }

            return hash;
        }
    }
}
