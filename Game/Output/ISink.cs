namespace Game.Output
{
    public interface ISink
    {
        void Write(CharInfo charInfo, Coord destination);

        void Write(CharDelay charDelay, Coord destination);

        void WriteRegion(CharInfo[,] buffer, Coord topLeft);

        void WriteRegion(CharDelay[,] buffer, Coord topLeft);
    }
}
