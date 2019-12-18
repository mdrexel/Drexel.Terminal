namespace Game.Output
{
    public readonly struct CharDelay
    {
        public readonly CharInfo CharInfo;
        public readonly int DelayInMilliseconds;

        public CharDelay(CharInfo charInfo, int delayInMilliseconds)
        {
            this.CharInfo = charInfo;
            this.DelayInMilliseconds = delayInMilliseconds;
        }

        public CharDelay GetInvertedColor()
        {
            return new CharDelay(this.CharInfo.GetInvertedColor(), this.DelayInMilliseconds);
        }
    }
}
