using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Output
{
    public static class Utilities
    {
        public static U[,] CreateSameSizeArray<T, U>(this T[,] array, U defaultValue)
        {
            // Why, Microsoft? Why is .NET Standard 2.1 not available on Framework? Array.Fill would do this for us...
            U[,] result = new U[array.GetHeight(), array.GetWidth()];
            for (int y = 0; y < result.GetHeight(); y++)
            {
                for (int x = 0; x < result.GetWidth(); x++)
                {
                    result[y, x] = defaultValue;
                }
            }

            return result;
        }
    }
}
