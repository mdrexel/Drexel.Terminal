namespace Game.Output
{
    public interface ISink
    {
        void Write(CharInfo charInfo);

        void WriteRegion(CharInfo[,] buffer, Coord topLeft);
    }
}
