﻿using System;

namespace Game.Output.Layout
{
    public sealed class Region : IReadOnlyRegion
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
                RegionChangeEventArgs args = new RegionChangeEventArgs(this, value, this.bottomRight);
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = value;
                this.OnChanged?.Invoke(this, new RegionChangedEventArgs(oldRegion, this));
            }
        }

        public Coord BottomRight
        {
            get => this.bottomRight;
            set
            {
                RegionChangeEventArgs args = new RegionChangeEventArgs(this, this.topLeft, value);
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.bottomRight = value;
                this.OnChanged?.Invoke(this, new RegionChangedEventArgs(oldRegion, this));
            }
        }

        public short Width
        {
            get => (this.bottomRight - this.topLeft).X;
            set
            {
                Coord newTopLeft = this.topLeft;
                Coord newBottomRight = this.bottomRight;
                if (value < 0)
                {
                    newTopLeft = new Coord((short)(this.topLeft.X + value), this.topLeft.Y);
                }
                else
                {
                    newBottomRight = new Coord((short)(this.topLeft.X + value), this.bottomRight.Y);
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(this, newTopLeft, newBottomRight);
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;
                this.OnChanged?.Invoke(this, new RegionChangedEventArgs(oldRegion, this));
            }
        }

        public short Height
        {
            get => (this.bottomRight - this.topLeft).Y;
            set
            {
                Coord newTopLeft = this.topLeft;
                Coord newBottomRight = this.bottomRight;
                if (value < 0)
                {
                    newTopLeft = new Coord(this.topLeft.X, (short)(this.topLeft.Y + value));
                }
                else
                {
                    newBottomRight = new Coord(this.bottomRight.X, (short)(this.topLeft.Y + value));
                }

                RegionChangeEventArgs args = new RegionChangeEventArgs(this, newTopLeft, newBottomRight);
                this.OnChangeRequested?.Invoke(this, args);
                if (args.Cancel)
                {
                    return;
                }

                Region oldRegion = this.Clone();
                this.topLeft = newTopLeft;
                this.bottomRight = newBottomRight;
                this.OnChanged?.Invoke(this, new RegionChangedEventArgs(oldRegion, this));
            }
        }

        public event EventHandler<RegionChangeEventArgs>? OnChangeRequested;

        public event EventHandler<RegionChangedEventArgs>? OnChanged;

        public bool Overlaps(Region other)
        {
            return this.topLeft.X < other.bottomRight.X
                && this.bottomRight.X > other.topLeft.X
                && this.topLeft.Y < other.bottomRight.Y
                && this.bottomRight.Y > other.topLeft.Y;
        }

        public void Translate(Coord vector)
        {
            Coord newTopLeft = this.topLeft + vector;
            Coord newBottomRight = this.bottomRight + vector;

            RegionChangeEventArgs args = new RegionChangeEventArgs(this, newTopLeft, newBottomRight);
            this.OnChangeRequested?.Invoke(this, args);
            if (args.Cancel)
            {
                return;
            }

            Region oldRegion = this.Clone();
            this.topLeft = newTopLeft;
            this.bottomRight = newBottomRight;
            this.OnChanged?.Invoke(this, new RegionChangedEventArgs(oldRegion, this));
        }

        private Region Clone()
        {
            return new Region(this.topLeft, this.bottomRight);
        }
    }
}