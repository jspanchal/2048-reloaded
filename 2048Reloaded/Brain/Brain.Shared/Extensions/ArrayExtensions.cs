using System.Collections.Generic;
using Brain.Utils;

namespace Brain.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Flatten a multi dimentional array into a single array
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this T[,] map) where T : new()
        {
            var list = new List<T>();
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    list.Add(map[row, col]);
                }
            }
            return list;
        }


        /// <summary>
        /// Return a random member of an array
        /// </summary>
        public static T Random<T>(this T[] array)
        {
            if (array.Length <= 0) return default(T);
            if (array.Length == 1) return array[0];
            return array[RandomManager.Next(array.Length)];
        }


        /// <summary>
        /// Swap two array members
        /// </summary>
        public static void Swap<T>(this T[] array, int from, int to)
        {
            var tmp = array[to];
            array[to] = array[from];
            array[from] = tmp;
        }
    }
}
