using System;

namespace Game.Output.Layout
{
    /// <summary>
    /// Represents the type of changes applied to a region.
    /// <br/><br/>
    /// Note that a single change can both move and resize; see <see cref="Enum.HasFlag(Enum)"/>.
    /// </summary>
    [Flags]
    public enum RegionChangeTypes
    {
        Move = 1,
        Resize = 2
    }
}
