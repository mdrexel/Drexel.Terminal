namespace Game.Output
{
    public interface IDrawable
    {
        void Draw(ISink sink);

        void InvertColor();
    }
}
