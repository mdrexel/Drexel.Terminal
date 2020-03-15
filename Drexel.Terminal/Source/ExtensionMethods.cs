using System;
using System.Collections.Generic;

namespace Drexel.Terminal.Source
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the buttons available on <see cref="IMouse"/> in the predefined order of: left, right, middle,
        /// button 4, button 5.
        /// </summary>
        /// <param name="mouse">
        /// The mouse.
        /// </param>
        /// <returns>
        /// The buttons available on <see cref="IMouse"/> in the predefined order of: left, right, middle, button4,
        /// button 5.
        /// </returns>
        public static IEnumerable<IMouseButton> GetOrderedButtons(this IMouse mouse)
        {
            if (mouse is null)
            {
                throw new ArgumentNullException(nameof(mouse));
            }

            yield return mouse.LeftButton;
            yield return mouse.RightButton;
            yield return mouse.MiddleButton;
            yield return mouse.Button4;
            yield return mouse.Button5;
        }
    }
}
