using System;
using System.Diagnostics;

namespace Game.Output.Layout
{
    [DebuggerDisplay("{this.TopLeft,nq}, {this.BottomRight,nq}")]
    public sealed class Region : IRegion, IEquatable<Region>
    {
        private Coord topLeft;
        private Coord bottomRight;

        public Region(Coord topLeft, Coord bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public Region(IReadOnlyRegion region)
        {
            this.topLeft = region.TopLeft;
            this.bottomRight = region.BottomRight;

            region.OnChanged +=
                (obj, e) =>
                {
                    RegionChangeTypes changeType;
                    if (e.AfterChange.TopLeft == this.topLeft)
                    {
                        if (e.AfterChange.BottomRight == this.bottomRight)
                        {
                            return;
                        }
                        else
                        {
                            changeType = RegionChangeTypes.Resize;
                        }
                    }
                    else
                    {
                        Coord newSize = new Coord(e.AfterChange.Width, e.AfterChange.Height);
                        Coord currentSize = new Coord(this.Width, this.Height);

                        Coord delta = currentSize - newSize;
                        if (delta.X == 0 && delta.Y == 0)
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
                        e.AfterChange,
                        changeType);
                    this.OnChangeRequested?.Invoke(obj, args);
                    if (args.Cancel)
                    {
                        return;
                    }

                    Region oldRegion = this.Clone();
                    this.topLeft = e.AfterChange.TopLeft;
                    this.bottomRight = e.AfterChange.BottomRight;

                    if (this != oldRegion)
                    {
                        this.OnChanged?.Invoke(
                            this,
                            new RegionChangedEventArgs(
                                oldRegion,
                                this,
                                changeType));
                    }
                };
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

        public short Width
        {
            get => (this.bottomRight - this.topLeft).X;
            set
            {
                RegionChangeTypes changeType;
                if (this.Width == value)
                {
                    return;
                }

                Coord newTopLeft = this.topLeft;
                Coord newBottomRight = this.bottomRight;
                if (value < 0)
                {
                    newTopLeft = new Coord((short)(this.topLeft.X + value), this.topLeft.Y);
                    Coord newSize = newBottomRight - newTopLeft;
                    if (newSize.X == this.Width)
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
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;

                if (this != oldRegion)
                {
                    this.OnChanged?.Invoke(
                        this,
                        new RegionChangedEventArgs(
                            oldRegion,
                            this,
                            changeType));
                }
            }
        }

        public short Height
        {
            get => (this.bottomRight - this.topLeft).Y;
            set
            {
                RegionChangeTypes changeType;
                if (this.Height == value)
                {
                    return;
                }

                Coord newTopLeft = this.topLeft;
                Coord newBottomRight = this.bottomRight;
                if (value < 0)
                {
                    newTopLeft = new Coord(this.topLeft.X, (short)(this.topLeft.Y + value));
                    Coord newSize = newBottomRight - newTopLeft;
                    if (newSize.Y == this.Height)
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
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;

                if (this != oldRegion)
                {
                    this.OnChanged?.Invoke(
                        this,
                        new RegionChangedEventArgs(
                            oldRegion,
                            this,
                            changeType));
                }
            }
        }

        public event EventHandler<RegionChangeEventArgs>? OnChangeRequested;

        public event EventHandler<RegionChangedEventArgs>? OnChanged;

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
            if (this.topLeft == newTopLeft)
            {
                beforeChange = default!;
                return false;
            }

            Coord delta = this.topLeft - newTopLeft;
            Coord newBottomRight = this.bottomRight - delta;

            RegionChangeEventArgs args = new RegionChangeEventArgs(
                this,
                new Region(newTopLeft, newBottomRight),
                RegionChangeTypes.Move);
            this.OnChangeRequested?.Invoke(this, args);
            if (args.Cancel)
            {
                beforeChange = default!;
                return false;
            }

            Region oldRegion = this.Clone();
            this.topLeft = newTopLeft;
            this.bottomRight = newBottomRight;
            this.OnChanged?.Invoke(
                this,
                new RegionChangedEventArgs(
                    oldRegion,
                    this,
                    RegionChangeTypes.Move));

            beforeChange = oldRegion;
            return true;
        }

        public bool TryTranslate(Coord offset, out IReadOnlyRegion beforeChange)
        {
            if (offset == Coord.Zero)
            {
                beforeChange = default!;
                return false;
            }

            Coord newTopLeft = this.topLeft + offset;
            Coord newBottomRight = this.bottomRight + offset;

            RegionChangeEventArgs args = new RegionChangeEventArgs(
                this,
                new Region(newTopLeft, newBottomRight),
                RegionChangeTypes.Move);
            this.OnChangeRequested?.Invoke(this, args);
            if (args.Cancel)
            {
                beforeChange = default!;
                return false;
            }

            Region oldRegion = this.Clone();
            this.topLeft = newTopLeft;
            this.bottomRight = newBottomRight;
            this.OnChanged?.Invoke(
                this,
                new RegionChangedEventArgs(
                    oldRegion,
                    this,
                    RegionChangeTypes.Move));

            beforeChange = oldRegion;
            return true;
        }

        public bool TrySetCorners(Coord newTopLeft, Coord newBottomRight, out IReadOnlyRegion beforeChange) =>
            this.TrySetCorners(
                newTopLeft,
                newBottomRight,
                true,
                out beforeChange);

        internal bool SimulateRequestChange(
            Coord newTopLeft,
            Coord newBottomRight)
        {
            RegionChangeEventArgs args = new RegionChangeEventArgs(
                this,
                new Region(newTopLeft, newBottomRight));
            this.OnChangeRequested?.Invoke(this, args);
            return args.Cancel;
        }

        internal bool TrySetCorners(
            Coord newTopLeft,
            Coord newBottomRight,
            bool allowCancel,
            out IReadOnlyRegion beforeChange)
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

            newTopLeft = new Coord(realNewLeft, realNewTop);
            newBottomRight = new Coord(realNewRight, realNewBottom);

            if (this.topLeft == newTopLeft && this.bottomRight == newBottomRight)
            {
                beforeChange = default!;
                return false;
            }

            RegionChangeTypes changeType;
            if (this.topLeft == newTopLeft)
            {
                changeType = RegionChangeTypes.Resize;
            }
            else
            {
                Coord size = newTopLeft - newBottomRight;
                if (size.X == this.Width && size.Y == this.Height)
                {
                    changeType = RegionChangeTypes.Move;
                }
                else
                {
                    changeType = RegionChangeTypes.Move | RegionChangeTypes.Resize;
                }
            }

            if (allowCancel)
            {
                RegionChangeEventArgs args = new RegionChangeEventArgs(
                    this,
                    new Region(newTopLeft, newBottomRight),
                    changeType);
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    beforeChange = default!;
                    return false;
                }
            }

            Region oldRegion = this.Clone();
            this.topLeft = newTopLeft;
            this.bottomRight = newBottomRight;
            this.OnChanged?.Invoke(
                this,
                new RegionChangedEventArgs(
                    oldRegion,
                    this,
                    changeType));

            beforeChange = oldRegion;
            return true;
        }

        private Region Clone()
        {
            return new Region(this.topLeft, this.bottomRight);
        }
    }
}
