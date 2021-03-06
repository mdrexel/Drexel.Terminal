﻿using System;
using System.Diagnostics;

namespace Game.Output.Layout
{
    [DebuggerDisplay("{name,nq}")]
    public abstract class Symbol : IDrawable
    {
        private readonly string name;
        private readonly Border border;

        public Symbol(
            LayoutManager layoutManager,
            Region region,
            BorderBuilder borderBuilder,
            string name)
        {
            this.LayoutManager = layoutManager;
            this.Region = region;
            this.border = borderBuilder.Build(this.Region);
            this.name = name;
        }

        public Region Region { get; }

        public IReadOnlyRegion InnerRegion => this.border.InnerRegion;

        public IReadOnlyBorder Border => this.border;

        public abstract bool CanBeFocused { get; }

        public abstract bool CanBeMoved { get; }

        public abstract bool CanBeResized { get; }

        protected LayoutManager LayoutManager { get; }

        public void Draw(ISink sink)
        {
            this.border.Draw(sink);
            this.DrawInternal(sink);
        }

        public void Draw(ISink sink, Rectangle window)
        {
            this.border.Draw(sink, window);
            this.DrawInternal(sink, window);
        }

        public void Draw(ISink sink, IReadOnlyRegion region)
        {
            if (this.InnerRegion.Contains(region))
            {
                this.DrawInternal(
                    sink,
                    new Rectangle(
                        region.TopLeft - this.InnerRegion.TopLeft,
                        region.BottomRight - this.InnerRegion.TopLeft - Coord.OneOffset));
            }
            else if (this.border.OuterRegion.Overlaps(region))
            {
                this.Draw(sink);
            }
        }

        public void InvertColor()
        {
            this.border.InvertColor();
            this.InvertColorInternal();
        }

        public virtual void KeyPressed(ConsoleKeyInfo keyInfo)
        {
        }

        public virtual void MouseMoveEvent(Coord oldPosition, Coord newPosition)
        {
        }

        public virtual void LeftMouseEvent(Coord coord, bool down)
        {
        }

        public virtual void MouseEnteredSymbol(Coord enterCoord, bool leftMouseDown, bool rightMouseDown)
        {
        }

        public virtual void MouseExitedSymbol(Coord exitCoord)
        {
        }

        public virtual void RightMouseEvent(Coord coord, bool down)
        {
        }

        public virtual void ScrollEvent(Coord coord, bool down)
        {
        }

        public virtual void FocusChanged(bool focused)
        {
        }

        protected abstract void DrawInternal(ISink sink);

        protected abstract void DrawInternal(ISink sink, Rectangle window);

        protected abstract void InvertColorInternal();
    }
}
