using System;

namespace Game.Output.Layout
{
    /// <summary>
    /// Represents a change to an <see cref="IReadOnlyRegion"/>.
    /// </summary>
    public sealed class RegionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionChangedEventArgs"/> class.
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
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="beforeChange"/> or <paramref name="afterChange"/> is <see langword="null"/>.
        /// </exception>
        public RegionChangedEventArgs(
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionChangedEventArgs"/> class.
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
        internal RegionChangedEventArgs(
            IReadOnlyRegion beforeChange,
            IReadOnlyRegion afterChange,
            RegionChangeTypes changeTypes)
        {
            this.BeforeChange = beforeChange;
            this.AfterChange = afterChange;
            this.ChangeTypes = changeTypes;
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
    }
}
