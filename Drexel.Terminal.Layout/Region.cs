using System;
using System.Diagnostics;
using Drexel.Terminal.Internals;

namespace Drexel.Terminal.Layout
{
    [DebuggerDisplay("{this.TopLeft,nq}, {this.BottomRight,nq}")]
    public sealed class Region : IResizeableRegion, IEquatable<Region>
    {
        private readonly Observable<RegionChangeEventArgs> onChangeRequested;
        private readonly Observable<RegionChangedEventArgs> onChanged;

        private Coord topLeft;
        private Coord bottomRight;

        public Region(Coord topLeft, Coord bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;

            this.onChangeRequested = new Observable<RegionChangeEventArgs>();
            this.onChanged = new Observable<RegionChangedEventArgs>();
        }

        public Coord TopLeft
        {
            get => this.topLeft;
            set => this.TrySetCorners(value, this.bottomRight, out _);
        }

        public Coord BottomRight
        {
            get => this.bottomRight;
            set => this.TrySetCorners(this.topLeft, value, out _);
        }

        public short MathWidth
        {
            get => (this.bottomRight - this.topLeft).X;
            set
            {
                RegionChangeTypes changeType;
                if (this.MathWidth == value)
                {
                    return;
                }

                Coord newTopLeft = this.topLeft;
                Coord newBottomRight = this.bottomRight;
                if (value < 0)
                {
                    newTopLeft = new Coord((short)(this.topLeft.X + value), this.topLeft.Y);
                    Coord newSize = newBottomRight - newTopLeft;
                    if (newSize.X == this.MathWidth)
                    {
                        // If our width is unchanged after the operation, it means they flipped us - we haven't
                        // resized, just translated to the left by our width.
                        changeType = RegionChangeTypes.Move;
                    }
                    else
                    {
                        changeType = RegionChangeTypes.Move | RegionChangeTypes.Resize;
                    }
                }
                else
                {
                    newBottomRight = new Coord((short)(this.topLeft.X + value), this.bottomRight.Y);
                    changeType = RegionChangeTypes.Resize;
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(
                    this,
                    new Region(newTopLeft, newBottomRight),
                    changeType);
                this.onChangeRequested.Next(args);
                if (args.Canceled)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;

                if (this != oldRegion)
                {
                    this.onChanged.Next(
                        new RegionChangedEventArgs(
                            oldRegion,
                            this,
                            changeType));
                }
            }
        }

        public short MathHeight
        {
            get => (this.bottomRight - this.topLeft).Y;
            set
            {
                RegionChangeTypes changeType;
                if (this.MathHeight == value)
                {
                    return;
                }

                Coord newTopLeft = this.topLeft;
                Coord newBottomRight = this.bottomRight;
                if (value < 0)
                {
                    newTopLeft = new Coord(this.topLeft.X, (short)(this.topLeft.Y + value));
                    Coord newSize = newBottomRight - newTopLeft;
                    if (newSize.Y == this.MathHeight)
                    {
                        // If our height is unchanged after the operation, it means they flipped us - we haven't
                        // resized, just translated upwards by our height.
                        changeType = RegionChangeTypes.Move;
                    }
                    else
                    {
                        changeType = RegionChangeTypes.Move | RegionChangeTypes.Resize;
                    }
                }
                else
                {
                    newBottomRight = new Coord(this.bottomRight.X, (short)(this.topLeft.Y + value));
                    changeType = RegionChangeTypes.Resize;
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(
                    this,
                    new Region(newTopLeft, newBottomRight),
                    changeType);
                this.onChangeRequested.Next(args);
                if (args.Canceled)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;

                if (this != oldRegion)
                {
                    this.onChanged.Next(
                        new RegionChangedEventArgs(
                            oldRegion,
                            this,
                            changeType));
                }
            }
        }

        public short ActualWidth
        {
            get => (short)(this.MathWidth + 1);
            set => this.MathWidth = (short)(value + 1);
        }

        public short ActualHeight
        {
            get => (short)(this.MathHeight + 1);
            set => this.MathHeight = (short)(value + 1);
        }

        public IObservable<RegionChangeEventArgs> OnChangeRequested => this.onChangeRequested;

        public IObservable<RegionChangedEventArgs> OnChanged => this.onChanged;

        public static bool operator ==(Region left, Region right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Region left, Region right)
        {
            return !(left == right);
        }

        public bool Overlaps(IReadOnlyRegion region)
        {
            return this.topLeft.X < region.BottomRight.X
                && this.bottomRight.X > region.TopLeft.X
                && this.topLeft.Y < region.BottomRight.Y
                && this.bottomRight.Y > region.TopLeft.Y;
        }

        public bool Overlaps(Coord coord)
        {
            return coord.X >= this.topLeft.X
                && coord.Y >= this.topLeft.Y
                && coord.X <= this.bottomRight.X
                && coord.Y <= this.bottomRight.Y;
        }

        public bool Contains(IReadOnlyRegion region)
        {
            return region.TopLeft.X >= this.topLeft.X
                && region.TopLeft.Y >= this.topLeft.Y
                && region.BottomRight.X <= this.bottomRight.X
                && region.BottomRight.Y <= this.bottomRight.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Region other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public bool Equals(Region other)
        {
            if (other is null)
            {
                return false;
            }

            return this.topLeft == other.topLeft
                && this.bottomRight == other.bottomRight;
        }

        public override int GetHashCode()
        {
            int hash = 54413;
            unchecked
            {
                hash = (hash * 31) + this.topLeft.GetHashCode();
                hash = (hash * 31) + this.bottomRight.GetHashCode();
            }

            return hash;
        }

        public bool TryMoveTo(Coord newTopLeft, out IReadOnlyRegion beforeChange)
        {
            if (this.CanMoveToInternal(newTopLeft, out Coord newBottomRight))
            {
                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;
                this.onChanged.Next(
                    new RegionChangedEventArgs(
                        oldRegion,
                        this,
                        RegionChangeTypes.Move));

                beforeChange = oldRegion;
                return true;
            }
            else
            {
                beforeChange = default!;
                return false;
            }
        }

        public bool TrySetCorners(Coord newTopLeft, Coord newBottomRight, out IReadOnlyRegion beforeChange)
        {
            if (this.CanSetCornersInternal(
                newTopLeft,
                newBottomRight,
                out newTopLeft,
                out newBottomRight,
                out RegionChangeTypes changeType))
            {
                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;
                this.onChanged.Next(
                    new RegionChangedEventArgs(
                        oldRegion,
                        this,
                        changeType));

                beforeChange = oldRegion;
                return true;
            }
            else
            {
                beforeChange = default!;
                return false;
            }
        }

        public bool CanSetCorners(Coord newTopLeft, Coord newBottomRight) =>
            this.CanSetCornersInternal(
                newTopLeft,
                newBottomRight,
                out _,
                out _,
                out _);

        public bool CanMoveTo(Coord newTopLeft) =>
            this.CanMoveToInternal(newTopLeft, out _);

        internal bool CanMoveToInternal(Coord newTopLeft, out Coord newBottomRight)
        {
            if (this.topLeft == newTopLeft)
            {
                newBottomRight = default;
                return false;
            }

            Coord delta = this.topLeft - newTopLeft;
            newBottomRight = this.bottomRight - delta;

            RegionChangeEventArgs args = new RegionChangeEventArgs(
                this,
                new Region(newTopLeft, newBottomRight),
                RegionChangeTypes.Move);
            this.onChangeRequested.Next(args);
            return args.Canceled;
        }

        internal bool CanSetCornersInternal(
            Coord newTopLeft,
            Coord newBottomRight,
            out Coord actualTopLeft,
            out Coord actualBottomRight,
            out RegionChangeTypes changeType)
        {
            short realNewTop = newTopLeft.Y;
            short realNewLeft = newTopLeft.X;
            short realNewBottom = newBottomRight.Y;
            short realNewRight = newBottomRight.X;
            if (newBottomRight.X < newTopLeft.X)
            {
                realNewLeft = newBottomRight.X;
                realNewRight = newTopLeft.X;
            }

            if (newBottomRight.Y < newTopLeft.Y)
            {
                realNewTop = newBottomRight.Y;
                realNewBottom = newTopLeft.Y;
            }

            actualTopLeft = new Coord(realNewLeft, realNewTop);
            actualBottomRight = new Coord(realNewRight, realNewBottom);

            if (this.topLeft == actualTopLeft && this.bottomRight == actualBottomRight)
            {
                changeType = default;
                return false;
            }

            if (this.topLeft == newTopLeft)
            {
                changeType = RegionChangeTypes.Resize;
            }
            else
            {
                Coord size = actualTopLeft - actualBottomRight;
                if (size.X == this.MathWidth && size.Y == this.MathHeight)
                {
                    changeType = RegionChangeTypes.Move;
                }
                else
                {
                    changeType = RegionChangeTypes.Move | RegionChangeTypes.Resize;
                }
            }

            RegionChangeEventArgs args = new RegionChangeEventArgs(
                this,
                new Region(actualTopLeft, actualBottomRight),
                changeType);
            this.onChangeRequested.Next(args);
            return args.Canceled;
        }

        private Region Clone()
        {
            return new Region(this.topLeft, this.bottomRight);
        }
    }
}
