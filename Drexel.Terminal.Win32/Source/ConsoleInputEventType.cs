namespace Drexel.Terminal.Source.Win32
{
    /// <summary>
    /// Console input event types.
    /// </summary>
    internal enum ConsoleInputEventType : short
    {
        None = 0,
        KeyEvent = 1,
        MouseEvent = 2,
        WindowBufferSizeEvent = 4,
        MenuEvent = 8,
        FocusEvent = 16
    }
}
