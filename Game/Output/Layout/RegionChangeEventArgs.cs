﻿using System;

namespace Game.Output.Layout
{
    public sealed class RegionChangeEventArgs : EventArgs
    {
        public RegionChangeEventArgs(
            IReadOnlyRegion region,
            Coord requestedTopLeft,
            Coord requestedBottomRight)
        {
            this.Region = region;
            this.RequestedTopLeft = requestedTopLeft;
            this.RequestedBottomRight = requestedBottomRight;

            this.Cancel = false;
        }

        public IReadOnlyRegion Region { get; }

        public Coord RequestedTopLeft { get; }

        public Coord RequestedBottomRight { get; }

        /// <summary>
        /// Gets or sets whether this event should be cancelled. Set to <see langword="true"/> to cancel the requested
        /// resize event.
        /// </summary>
        public bool Cancel { get; set; }
    }
}