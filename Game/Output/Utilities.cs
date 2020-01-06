namespace Game.Output
{
    public static class Utilities
    {
        public static T[,] Fill<T>(this T[,] array, T value)
        {
            for (int y = 0; y < array.GetHeight(); y++)
            {
                for (int x = 0; x < array.GetWidth(); x++)
                {
                    array[y, x] = value;
                }
            }

            return array;
        }

        public static U[,] CreateSameSizeArray<T, U>(this T[,] array, U defaultValue)
        {
            U[,] result = new U[array.GetHeight(), array.GetWidth()];
            result.Fill(defaultValue);

            return result;
        }
    }
}
