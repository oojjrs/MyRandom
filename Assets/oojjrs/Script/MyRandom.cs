using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.oojjrs.Script
{
    public class MyRandom
    {
        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// [minValue, maxValue)
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue. maxValue must be greather than or equal to minValue.</param>
        /// <returns>A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.</returns>
        public static int Range(int minValue, int maxValue)
        {
            return Range(minValue, maxValue, RandomSource);
        }

        public static int Range(int minValue, int maxValue, Random random)
        {
            return random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a random floating-point number that is within a specified range.
        /// [minValue, maxValue]
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue. maxValue must be greather than or equal to minValue.</param>
        /// <returns>A single-precision floating point number that is greater than or equal to minValue, and less than or equal to maxValue.</returns>
        public static float Range(float minValue, float maxValue)
        {
            return (float)Range((double)minValue, maxValue, RandomSource);
        }

        public static float Range(float minValue, float maxValue, Random random)
        {
            return (float)Range((double)minValue, maxValue, random);
        }

        /// <summary>
        /// Returns a random floating-point number that is within a specified range.
        /// [minValue, maxValue]
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue. maxValue must be greather than or equal to minValue.</param>
        /// <returns>A double-precision floating point number that is greater than or equal to minValue, and less than or equal to maxValue.</returns>
        public static double Range(double minValue, double maxValue)
        {
            return Range(minValue, maxValue, RandomSource);
        }

        public static double Range(double minValue, double maxValue, Random random)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException();

            var result = (random.NextDouble() * (maxValue - (double)minValue)) + minValue;
            return (float)result;
        }

        /// <summary>
        /// Returns a randomly selected entry in an entries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of entries.</typeparam>
        /// <param name="entries">A sequence of values to invoke a Select function on.</param>
        /// <returns>A randomly selected entry in an entries.</returns>
        public static T Select<T>(IEnumerable<T> entries)
        {
            return Select(entries, RandomSource);
        }

        public static T Select<T>(IEnumerable<T> entries, Random random)
        {
            if (entries == default)
                return default;

            if (entries.Any() == false)
                return default;

            var length = entries.Count();
            if (length == 1)
                return entries.First();

            var index = random.Next(0, length);
            return entries.ElementAt(index);
        }

        /// <summary>
        /// Returns a N randomly selected entries in an entries.
        /// Returns Empty if count is less than or equal to 0, and returns the entries in random order if count is greater than or equal to the number of entries.
        /// </summary>
        /// <typeparam name="T">The type of the elements of entries.</typeparam>
        /// <param name="entries">A sequence of values to invoke a Select function on.</param>
        /// <param name="count">A number to select.</param>
        /// <returns>A N randomly selected entries.</returns>
        public static IEnumerable<T> Select<T>(IEnumerable<T> entries, int count)
        {
            return Select(entries, count, RandomSource);
        }

        public static IEnumerable<T> Select<T>(IEnumerable<T> entries, int count, Random random)
        {
            if (entries == default)
                return Enumerable.Empty<T>();

            if (count <= 0)
                return Enumerable.Empty<T>();

            if (entries.Any() == false)
                return Enumerable.Empty<T>();

            var length = entries.Count();
            if (length >= count)
                return Shuffle(entries, random);

            return entries.OrderBy(t => random.Next()).Take(count);
        }

        /// <summary>
        /// Returns a randomly selected entry in an entries.
        /// Each item has a probability of being selected equal to the item's float value from the sum of the floats of all items.
        /// If all float values are 1, it will have a value of 1/N, which works the same as Select<T>(entries).
        /// </summary>
        /// <typeparam name="(T, float)">The type of the elements of entries and probability of being selected.</typeparam>
        /// <param name="entries">A sequence of values to invoke a Select function on.</param>
        /// <returns>A randomly selected entry in an entries.</returns>
        public static T Select<T>(IEnumerable<(T, float)> entries)
        {
            return Select(entries, RandomSource);
        }

        public static T Select<T>(IEnumerable<(T, float)> entries, Random random)
        {
            if (entries == default)
                return default;

            if (entries.Any() == false)
                return default;

            var length = entries.Count();
            if (length == 1)
                return entries.First().Item1;

            var array = entries.ToArray();
            var sum = array.Sum(t => t.Item2);

            var accum = 0f;
            var point = Range(0f, 1f, random);
            return array.First(t =>
            {
                accum += t.Item2;
                return (accum / sum) >= point;
            }).Item1;
        }

        /// <summary>
        /// Returns a N randomly selected entries in an entries.
        /// Items already selected in entries can also be selected again.
        /// </summary>
        /// <typeparam name="T">The type of the elements of entries.</typeparam>
        /// <param name="entries">A sequence of values to invoke a Select function on.</param>
        /// <param name="count">A number to select.</param>
        /// <returns>A N randomly selected entries.</returns>
        public static IEnumerable<T> SelectWithRepetition<T>(IEnumerable<T> entries, int count)
        {
            return SelectWithRepetition(entries, count, RandomSource);
        }

        public static IEnumerable<T> SelectWithRepetition<T>(IEnumerable<T> entries, int count, Random random)
        {
            if (entries == default)
                return Enumerable.Empty<T>();

            if (count <= 0)
                return Enumerable.Empty<T>();

            if (entries.Any() == false)
                return Enumerable.Empty<T>();

            var entriesArray = entries.ToArray();
            var result = new List<T>();
            for (int i = 0; i < count; ++i)
            {
                var index = random.Next(0, entriesArray.Length);
                result.Add(entriesArray[index]);
            }
            return result;
        }

        /// <summary>
        /// Returns a N randomly selected entries in an entries.
        /// Items already selected in entries can also be selected again.
        /// Each item has a probability of being selected equal to the item's float value from the sum of the floats of all items.
        /// If all float values are 1, it will have a value of 1/N, which works the same as SelectWithRepetition<T>(entries, count).
        /// </summary>
        /// <typeparam name="(T, float)">The type of the elements of entries and probability of being selected.</typeparam>
        /// <param name="entries">A sequence of values to invoke a Select function on.</param>
        /// <param name="count">A number to select.</param>
        /// <returns>A N randomly selected entries.</returns>
        public static IEnumerable<T> SelectWithRepetition<T>(IEnumerable<(T, float)> entries, int count)
        {
            return SelectWithRepetition(entries, count, RandomSource);
        }

        public static IEnumerable<T> SelectWithRepetition<T>(IEnumerable<(T, float)> entries, int count, Random random)
        {
            if (entries == default)
                return Enumerable.Empty<T>();

            if (count <= 0)
                return Enumerable.Empty<T>();

            if (entries.Any() == false)
                return Enumerable.Empty<T>();

            var entriesArray = entries.ToArray();
            var sum = entriesArray.Sum(t => t.Item2);

            var result = new List<T>();
            for (int i = 0; i < count; ++i)
            {
                var accum = 0f;
                var point = Range(0f, 1f, random);
                var entry = entriesArray.First(t =>
                {
                    accum += t.Item2;
                    return (accum / sum) >= point;
                });
                result.Add(entry.Item1);
            }
            return result;
        }

        /// <summary>
        /// Returns the entries in random order.
        /// </summary>
        /// <typeparam name="T">The type of the elements of entries.</typeparam>
        /// <param name="entries">A sequence of values to invoke a Select function on.</param>
        /// <returns>The entries in random order.</returns>
        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> entries)
        {
            return Shuffle(entries, RandomSource);
        }

        public static IEnumerable<T> Shuffle<T>(IEnumerable<T> entries, Random random)
        {
            if (entries == default)
                return Enumerable.Empty<T>();

            if (entries.Any() == false)
                return Enumerable.Empty<T>();

            return entries.OrderBy(t => random.Next());
        }

        private static readonly Random RandomSource = new Random(DateTime.Now.GetHashCode());
    }
}
