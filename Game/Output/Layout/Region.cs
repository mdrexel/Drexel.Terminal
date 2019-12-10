using System;

namespace Game.Output.Layout
{
    public sealed class Region : IMoveOnlyRegion, IEquatable<Region>
    {
        private Coord topLeft;
        private Coord bottomRight;

        public Region(Coord topLeft, Coord bottomRight)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }

        public Coord TopLeft
        {
            get => this.topLeft;
            set
            {
                if (value == this.topLeft)
                {
                    return;
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(
                    this,
                    value,
                    this.bottomRight,
                    RegionChangeType.MoveAndResize);
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = value;

                if (this != oldRegion)
                {
                    this.OnChanged?.Invoke(
                        this,
                        new RegionChangedEventArgs(
                            oldRegion,
                            this,
                            RegionChangeType.MoveAndResize));
                }
            }
        }

        public Coord BottomRight
        {
            get => this.bottomRight;
            set
            {
                if (this.bottomRight == value)
                {
                    return;
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(
                    this,
                    this.topLeft,
                    value,
                    RegionChangeType.Resize);
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.bottomRight = value;

                if (this != oldRegion)
                {
                    this.OnChanged?.Invoke(
                        this,
                        new RegionChangedEventArgs(
                            oldRegion,
                            this,
                            RegionChangeType.Resize));
                }
            }
        }

        public short Width
        {
            get => (this.bottomRight - this.topLeft).X;
            set
            {
                RegionChangeType changeType;
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
                        changeType = RegionChangeType.Move;
                    }
                    else
                    {
                        changeType = RegionChangeType.MoveAndResize;
                    }
                }
                else
                {
                    newBottomRight = new Coord((short)(this.topLeft.X + value), this.bottomRight.Y);
                    changeType = RegionChangeType.Resize;
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(
                    this,
                    newTopLeft,
                    newBottomRight,
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
                RegionChangeType changeType;
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
                        changeType = RegionChangeType.Move;
                    }
                    else
                    {
                        changeType = RegionChangeType.MoveAndResize;
                    }
                }
                else
                {
                    newBottomRight = new Coord(this.bottomRight.X, (short)(this.topLeft.Y + value));
                    changeType = RegionChangeType.Resize;
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(
                    this,
                    newTopLeft,
                    newBottomRight,
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

        public bool Overlaps(Region other)
        {
            return this.topLeft.X < other.bottomRight.X
                && this.bottomRight.X > other.topLeft.X
                && this.topLeft.Y < other.bottomRight.Y
                && this.bottomRight.Y > other.topLeft.Y;
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

        public void Translate(Coord offset)
        {
            Coord newTopLeft = this.topLeft + offset;
            Coord newBottomRight = this.bottomRight + offset;

            RegionChangeEventArgs args = new RegionChangeEventArgs(
                this,
                newTopLeft,
                newBottomRight,
                RegionChangeType.Move);
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
                        RegionChangeType.Move));
            }
        }

        private Region Clone()
        {
            return new Region(this.topLeft, this.bottomRight);
        }
    }
}
