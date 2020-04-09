using System;

namespace Drexel.Terminal.Internals
{
    internal sealed class Disposable : IDisposable
    {
        private readonly Action? onDispose;

        public Disposable(Action? onDispose = null)
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            onDispose?.Invoke();
        }
    }
}
