using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.oojjrs.Script
{
    public class MyRandom
    {
        public static int Range(int min, int max)
        {
            return Range(min, max, RandomSource);
        }

        public static int Range(int min, int max, Random random)
        {
            return random.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return (float)Range((double)min, max, RandomSource);
        }

        public static float Range(float min, float max, Random random)
        {
            return (float)Range((double)min, max, random);
        }

        public static double Range(double min, double max)
        {
            return Range(min, max, RandomSource);
        }

        public static double Range(double min, double max, Random random)
        {
            var result = (random.NextDouble() * (max - (double)min)) + min;
            return (float)result;
        }

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
            if (length == count)
                return Shuffle(entries, random);

            return entries.OrderBy(t => random.Next()).Take(count);
        }

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

        public static IEnumerable<T> SelectInDuplicate<T>(IEnumerable<T> entries, int count)
        {
            return SelectInDuplicate(entries, count, RandomSource);
        }

        public static IEnumerable<T> SelectInDuplicate<T>(IEnumerable<T> entries, int count, Random random)
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

        public static IEnumerable<T> SelectInDuplicate<T>(IEnumerable<(T, float)> entries, int count)
        {
            return SelectInDuplicate(entries, count, RandomSource);
        }

        public static IEnumerable<T> SelectInDuplicate<T>(IEnumerable<(T, float)> entries, int count, Random random)
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
