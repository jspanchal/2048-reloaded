using System;

namespace Brain.Utils
{

    /// <summary>
    /// Simple random functions
    /// </summary>
    public static class RandomManager
    {
        private static Random _random;
        public static Random Random
        {
            get { return _random ?? (_random = new Random()); }
            set { _random = value; }
        }

        public static int Next()
        {
            return Random.Next();
        }

        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        public static void NextBytes(byte[] buffer)
        {
            Random.NextBytes(buffer);
        }

        public static double NextDouble()
        {
            return Random.NextDouble();
        }

        public static double NextDouble(double maxValue)
        {
            return Random.NextDouble() * maxValue;
        }

        public static double NextDouble(double minValue, double maxValue)
        {
            return (Random.NextDouble() * (maxValue - minValue)) + minValue;
        }

        public static bool Whatever()
        {
            return Random.Next(0, 2) > 0;
        }


        /// <summary>
        /// Shuffle the order of an array
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Next(i);
                if (j != i)
                {
                    T tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                }
            }
        }

    }
}
