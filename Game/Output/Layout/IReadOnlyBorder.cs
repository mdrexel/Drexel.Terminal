namespace Game.Output.Layout
{
    public interface IReadOnlyBorder : IDrawable
    {
        IReadOnlyRegion InnerRegion { get; }

        IReadOnlyRegion OuterRegion { get; }

        IReadOnlyRegion GetComponent(BorderComponentType component);
    }
}