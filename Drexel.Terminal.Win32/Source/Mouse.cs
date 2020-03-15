using System;
using Drexel.Terminal.Win32;

namespace Drexel.Terminal.Source.Win32
{
    internal sealed class Mouse : IMouse
    {
        public Mouse()
        {
            this.Position = default;
            this.OnMouseMove = new Observable<MouseMoveEventArgs>();
            this.OnMouseWheel = new Observable<MouseWheelEventArgs>();

            this.LeftButton = new MouseButton();
            this.MiddleButton = new MouseButton();
            this.RightButton = new MouseButton();
            this.Button4 = new MouseButton();
            this.Button5 = new MouseButton();
        }

        public Coord Position { get; set; }

        public Observable<MouseMoveEventArgs> OnMouseMove { get; }

        public Observable<MouseWheelEventArgs> OnMouseWheel { get; }

        public MouseButton LeftButton { get; }

        public MouseButton MiddleButton { get; }

        public MouseButton RightButton { get; }

        public MouseButton Button4 { get; }

        public MouseButton Button5 { get; }

        IObservable<MouseMoveEventArgs> IMouse.OnMove => this.OnMouseMove;

        IObservable<MouseWheelEventArgs> IMouse.OnScrollWheel => this.OnMouseWheel;

        IMouseButton IMouse.LeftButton => this.LeftButton;

        IMouseButton IMouse.MiddleButton => this.MiddleButton;

        IMouseButton IMouse.RightButton => this.RightButton;

        IMouseButton IMouse.Button4 => this.Button4;

        IMouseButton IMouse.Button5 => this.Button5;
    }
}
