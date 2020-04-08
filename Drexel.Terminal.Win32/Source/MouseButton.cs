using System;
using Drexel.Terminal.Internals;

namespace Drexel.Terminal.Source.Win32
{
    internal sealed class MouseButton : IMouseButton
    {
        public MouseButton()
        {
            this.Down = false;
            this.OnButton = new Observable<MouseClickEventArgs>();

            this.OnButton.Subscribe(new Observer<MouseClickEventArgs>(x => this.Down = x.ButtonDown));
        }

        public bool Down { get; private set; }

        public Observable<MouseClickEventArgs> OnButton { get; }

        IObservable<MouseClickEventArgs> IMouseButton.OnButton => this.OnButton;
    }
}
