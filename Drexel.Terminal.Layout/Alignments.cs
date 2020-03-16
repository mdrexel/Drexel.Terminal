namespace Drexel.Terminal.Layout
{
    /// <summary>
    /// Horizontal and vertical alignments.
    /// </summary>
    public struct Alignments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alignments"/> struct.
        /// </summary>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment.
        /// </param>
        /// <param name="verticalAlignment">
        /// The vertical alignment.
        /// </param>
        public Alignments(
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
        {
            this.HorizontalAlignment = horizontalAlignment;
            this.VerticalAlignment = verticalAlignment;
        }

        /// <summary>
        /// Gets the horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; }

        /// <summary>
        /// Gets the vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; }
    }
}
