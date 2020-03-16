using System;

namespace Drexel.Terminal.Layout
{
    /// <summary>
    /// Represents a requested change to an <see cref="IReadOnlyRegion"/>.
    /// <br/><br/>
    /// Note that the change has not yet been applied. The change can be canceled by setting <see cref="Cancel"/> to
    /// <see langword="true"/>.
    /// </summary>
    public sealed class RegionChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionChangeEventArgs"/> class.
        /// </summary>
        /// <param name="beforeChange">
        /// A region with properties equivalent to those without the change.
        /// </param>
        /// <param name="afterChange">
        /// A region with properties equivalent to those with the change.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="beforeChange"/> or <paramref name="afterChange"/> is <see langword="null"/>.
        /// </exception>
        public RegionChangeEventArgs(
            IReadOnlyRegion beforeChange,
            IReadOnlyRegion afterChange)
        {
            this.BeforeChange = beforeChange ?? throw new ArgumentNullException(nameof(beforeChange));
            this.AfterChange = afterChange ?? throw new ArgumentNullException(nameof(afterChange));

            RegionChangeTypes changeTypes = default;
            if (beforeChange.TopLeft != afterChange.TopLeft)
            {
                changeTypes |= RegionChangeTypes.Move;
            }

            if (beforeChange.Height != afterChange.Height
                || beforeChange.Width != afterChange.Width)
            {
                changeTypes |= RegionChangeTypes.Resize;
            }

            this.ChangeTypes = changeTypes;

            this.Canceled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionChangeEventArgs"/> class.
        /// </summary>
        /// <param name="beforeChange">
        /// A region with properties equivalent to those without the change.
        /// </param>
        /// <param name="afterChange">
        /// A region with properties equivalent to those with the change.
        /// </param>
        /// <param name="changeTypes">
        /// The types of changes encapsulated by this change event.
        /// </param>
        internal RegionChangeEventArgs(
            IReadOnlyRegion beforeChange,
            IReadOnlyRegion afterChange,
            RegionChangeTypes changeTypes)
        {
            this.BeforeChange = beforeChange;
            this.AfterChange = afterChange;
            this.ChangeTypes = changeTypes;

            this.Canceled = false;
        }

        /// <summary>
        /// Gets a region with properties before any change is applied.
        /// <br/><br/>
        /// Note that this instance may not be the region that triggered the event.
        /// </summary>
        public IReadOnlyRegion BeforeChange { get; }

        /// <summary>
        /// Gets a region with properties after any change is applied.
        /// <br/><br/>
        /// Note that this instance may not be the region that triggered the event.
        /// </summary>
        public IReadOnlyRegion AfterChange { get; }

        /// <summary>
        /// Gets the change types that have been applied.
        /// </summary>
        public RegionChangeTypes ChangeTypes { get; }

        /// <summary>
        /// Gets whether this change should be cancelled. 
        /// <br/><br/>
        /// To cancel a change, call <see cref="Cancel"/> or <see cref="Cancel(bool)"/>.
        /// </summary>
        public bool Canceled { get; private set; }

        /// <summary>
        /// Cancels this change.
        /// </summary>
        public void Cancel()
        {
            this.Canceled = true;
        }

        /// <summary>
        /// If <paramref name="condition"/> is <see langword="true"/>, cancels this change. Otherwise, has no effect.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public void Cancel(bool condition)
        {
            this.Canceled |= condition;
        }
    }
}
