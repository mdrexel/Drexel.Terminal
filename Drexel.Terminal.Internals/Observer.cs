using System;

namespace Drexel.Terminal.Internals
{
    internal sealed class Observer<T> : IObserver<T>
    {
        private readonly Action<T>? onNext;
        private readonly Action<Exception>? onError;
        private readonly Action? onCompleted;

        public Observer(
            Action<T>? onNext = null,
            Action<Exception>? onError = null,
            Action? onCompleted = null)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public void OnCompleted() => this.onCompleted?.Invoke();

        public void OnError(Exception error) => this.onError?.Invoke(error);

        public void OnNext(T value) => this.onNext?.Invoke(value);
    }
}
