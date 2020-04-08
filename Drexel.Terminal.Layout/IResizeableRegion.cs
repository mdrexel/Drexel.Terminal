namespace Drexel.Terminal.Layout
{
    /// <summary>
    /// Represents a resizeable two-dimensional region.
    /// </summary>
    public interface IResizeableRegion : IMoveOnlyRegion
    {
        /// <summary>
        /// Gets or sets the inclusive top-left coordinate of the region.
        /// </summary>
        new Coord TopLeft { get; set; }

        /// <summary>
        /// Gets or sets the inclusive bottom-right coordinate of the region.
        /// </summary>
        new Coord BottomRight { get; set; }

        /// <summary>
        /// Gets or sets the mathematical width of this region.
        /// </summary>
        new short MathWidth { get; set; }

        /// <summary>
        /// Gets or sets the mathematical height of this region.
        /// </summary>
        new short MathHeight { get; set; }

        /// <summary>
        /// Gets or sets the actual width of this region.
        /// </summary>
        new short ActualWidth { get; set; }

        /// <summary>
        /// Gets or sets the actual height of this region.
        /// </summary>
        new short ActualHeight { get; set; }

        /// <summary>
        /// Returns a value indicating whether this region can have its top-left and bottom-right corners set to the
        /// specified <paramref name="newTopLeft"/> and <paramref name="newBottomRight"/>.
        /// </summary>
        /// <param name="newTopLeft">
        /// The top-left to check against.
        /// </param>
        /// <param name="newBottomRight">
        /// The bottom-right to check against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the corners can be set to the specified values; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        bool CanSetCorners(Coord newTopLeft, Coord newBottomRight);

        /// <summary>
        /// Tries to set the top-left and bottom-right corners of this region to the specified
        /// <paramref name="newTopLeft"/> and <paramref name="newBottomRight"/>, and returns a value indicating whether
        /// setting the corners was successful. If setting the corners was successful, <see langword="true"/> will be
        /// returned, and <paramref name="beforeChange"/> will be set to a region equivalent to this region before the
        /// corners were set.
        /// </summary>
        /// <param name="newTopLeft">
        /// The new top-left coordinate of this region.
        /// </param>
        /// <param name="newBottomRight">
        /// The new bottom-right coordinate of this region.
        /// </param>
        /// <param name="beforeChange">
        /// If setting the corners succeeds, a region equivalent to this region before the translation was applied.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if setting the corners succeeded; otherwise, <see langword="false"/>.
        /// </returns>
        bool TrySetCorners(Coord newTopLeft, Coord newBottomRight, out IReadOnlyRegion beforeChange);
    }
}
