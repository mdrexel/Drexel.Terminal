namespace Game.Output.Primitives
{
    public sealed class Alignments
    {
        public Alignments(
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
        {
            this.HorizontalAlignment = horizontalAlignment;
            this.VerticalAlignment = verticalAlignment;
        }

        public static Alignments Default { get; } = new Alignments(HorizontalAlignment.Left, VerticalAlignment.Top);

        public HorizontalAlignment HorizontalAlignment { get; }

        public VerticalAlignment VerticalAlignment { get; }
    }
}
