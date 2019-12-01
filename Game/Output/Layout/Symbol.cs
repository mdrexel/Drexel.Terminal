namespace Game.Output.Layout
{
    public abstract class Symbol : IDrawable
    {
        private readonly Border border;

        public Symbol(
            Region region,
            BorderBuilder borderBuilder,
            string? name = null)
        {
            this.Region = region;

            BorderBuilder builder = borderBuilder;
            this.border = builder.Build(this.Region);
        }

        public Region Region { get; }

        protected IReadOnlyRegion InnerRegion => this.border.InnerRegion;

        public void Draw(ISink sink)
        {
            this.border.Draw(sink);
            this.DrawInternal(sink);
        }

        protected abstract void DrawInternal(ISink sink);
    }
}
