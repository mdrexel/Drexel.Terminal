using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Drexel.Terminal.Internals
{
    internal sealed class Observable<T> : IObservable<T>, IDisposable
    {
        private readonly LinkedList<Subscription> subscriptions;
        private readonly List<LinkedListNode<Subscription>> deadSubscriptions;

        private object closedLock;
        private bool closed;

        public Observable()
        {
            this.subscriptions = new LinkedList<Subscription>();
            this.deadSubscriptions = new List<LinkedListNode<Subscription>>();

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
                    this.RemoveSubscriptions();

                    foreach (Subscription subscription in this.subscriptions.ToArray())
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
                    this.RemoveSubscriptions();

                    foreach (Subscription subscription in this.subscriptions.ToArray())
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
                    this.RemoveSubscriptions();

                    Exception? error = null;
                    foreach (Subscription subscription in this.subscriptions.ToArray())
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

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveSubscriptions()
        {
            foreach (LinkedListNode<Subscription> subscription in this.deadSubscriptions)
            {
                this.subscriptions.Remove(subscription);
            }

            this.deadSubscriptions.Clear();
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
                    this.parent.deadSubscriptions.Add(this.node);
                }
            }
        }
    }
}
