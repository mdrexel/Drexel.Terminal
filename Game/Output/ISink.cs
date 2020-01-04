namespace Game.Output
{
    public interface ISink
    {
        void Write(CharUnion character);

        void Write(CharColors colors);

        void Write(CharInfo charInfo);

        void Write(CharDelay charDelay);

        void Write(CharUnion character, Coord destination);

        void Write(CharColors colors, Coord destination);

        void Write(CharInfo charInfo, Coord destination);

        void Write(CharDelay charDelay, Coord destination);

        void WriteRegion(CharInfo[,] buffer, Coord topLeft);

        void WriteRegion(CharDelay[,] buffer, Coord topLeft);

        void WriteRegion(CharInfo[,] buffer, Coord topLeft, Rectangle bufferRegion);

        void WriteRegion(CharDelay[,] buffer, Coord topLeft, Rectangle bufferRegion);
    }
}
