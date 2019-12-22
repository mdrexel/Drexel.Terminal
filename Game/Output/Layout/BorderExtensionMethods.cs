namespace Game.Output.Layout
{
    public static class BorderExtensionMethods
    {
        private readonly static BorderComponentType[,] ComponentTypes =
            new BorderComponentType[,]
            {
                { BorderComponentType.TopLeft, BorderComponentType.Top, BorderComponentType.TopRight },
                { BorderComponentType.Left, BorderComponentType.Center, BorderComponentType.Right },
                { BorderComponentType.BottomLeft, BorderComponentType.Bottom, BorderComponentType.BottomRight }
            };

        public static bool TryGetComponentAt(
            this IReadOnlyBorder border,
            Coord coord,
            out BorderComponentType component,
            out IReadOnlyRegion componentRegion)
        {
            if (!border.OuterRegion.Overlaps(coord))
            {
                goto fail;
            }

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    BorderComponentType type = ComponentTypes[y, x];
                    IReadOnlyRegion region = border.GetComponent(type);
                    if (region.Overlaps(coord))
                    {
                        component = type;
                        componentRegion = region;
                        return true;
                    }
                }
            }

            fail:
            component = default;
            componentRegion = default;
            return false;
        }
    }
}
