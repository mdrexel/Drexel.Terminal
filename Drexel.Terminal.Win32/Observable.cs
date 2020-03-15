using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Drexel.Terminal.Win32
{
    internal sealed class Observable<T> : IObservable<T>, IDisposable
    {
        private readonly LinkedList<Subscription> subscriptions;

        private object closedLock;
        private bool closed;

        public Observable()
        {
            this.subscriptions = new LinkedList<Subscription>();

            this.closedLock = new object();
            this.closed = false;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return new Subscription(this, observer);
        }

        public void Next(T value)
        {
            lock (this.closedLock)
            {
                this.ThrowIfClosed();
                lock (this.subscriptions)
                {
                    foreach (Subscription subscription in this.subscriptions)
                    {
                        try
                        {
                            subscription.Observer.OnNext(value);
                        }
                        catch (Exception e)
                        {
                            this.Error(e);
                            break;
                        }
                    }
                }
            }
        }

        public void Error(Exception error)
        {
            lock (this.closedLock)
            {
                this.ThrowIfClosed();
                lock (this.subscriptions)
                {
                    foreach (Subscription subscription in this.subscriptions)
                    {
                        try
                        {
                            subscription.Observer.OnError(error);
                        }
                        catch (Exception e)
                        {
                            error = e;
                        }
                    }
                }

                this.closed = true;
            }
        }

        public void Complete()
        {
            lock (this.closedLock)
            {
                this.ThrowIfClosed();
                lock (this.subscriptions)
                {
                    Exception? error = null;
                    foreach (Subscription subscription in this.subscriptions)
                    {
                        try
                        {
                            if (error is null)
                            {
                                subscription.Observer.OnCompleted();
                            }
                            else
                            {
                                subscription.Observer.OnError(error);
                            }
                        }
                        catch (Exception e)
                        {
                            error = e;
                        }
                    }
                }

                this.closed = true;
            }
        }

        public void Dispose()
        {
            lock (this.closedLock)
            {
                if (this.closed)
                {
                    return;
                }

                this.Complete();
                this.closed = true;
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfClosed()
        {
            if (this.closed)
            {
                throw new InvalidOperationException("Cannot operate on a complete/errored-out observable.");
            }
        }

        private sealed class Subscription : IDisposable
        {
            private readonly Observable<T> parent;
            private readonly LinkedListNode<Subscription> node;

            public Subscription(
                Observable<T> parent,
                IObserver<T> observer)
            {
                this.parent = parent;
                this.Observer = observer;

                lock (this.parent.subscriptions)
                {
                    this.node = this.parent.subscriptions.AddLast(this);
                }
            }

            public IObserver<T> Observer { get; }

            public void Dispose()
            {
                lock (this.parent.subscriptions)
                {
                    this.parent.subscriptions.Remove(this.node);
                }
            }
        }
    }
}
