# Drexel.Terminal
A more powerful C# console API.

## Description
Typically, C# code interacts with the console using the `System.Console` class. `System.Console` was designed for ease-of-use, and does not expose many commonly desired console features (ex. color, writing regions of text, mouse support, etc). `Drexel.Terminal` is intended for these more advances console uses.

## Features
* Supports console mouse input _(if the target platform supports it)_
* Supports console colors
* Supports performantly writing blocks of characters
* Event-driven architecture using `System.IObserver<T>`

## Getting Started
1. Add a reference to the `Drexel.Terminal.*` NuGet package for your target platform:
  * `Drexel.Terminal.Win32`, to target the Windows Win32 Console API.
  * `Drexel.Terminal.Ansi`, to target multi-platform ANSI escape codes.
2. Use `TerminalInstance.GetInstanceAsync(...)` to retrieve a `TerminalInstance`
3. Use the `TerminalInstance`. If you'd like, you can also leverage:
  * `Drexel.Terminal.Text`, so that you don't need to convert `System.String`s to character arrays with your bare hands
  * `Drexel.Terminal.Layout`, to create GUI-like applications
