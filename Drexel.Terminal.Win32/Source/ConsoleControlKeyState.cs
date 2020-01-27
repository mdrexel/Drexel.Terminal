using System;

namespace Drexel.Terminal.Source.Win32
{
    [Flags]
    internal enum ConsoleControlKeyState : int
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
}
