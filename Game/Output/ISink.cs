namespace Game.Output
{
    public interface ISink
    {
        void WriteRegion(
            CharInfo[,] buffer,
            short left,
            short top);
    }
}
