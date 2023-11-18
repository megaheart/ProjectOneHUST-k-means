using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Utilities
{
    public static class Extensions
    {
        public static IList Shuffle([NotNull] this IList list)
        {
            int N = list.Count;
            for (int i = 0; i < N; i++)
            {
                int j = Random.Shared.Next(0, N);
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            return list;
        }
        public static int IndexOfMax<T>([NotNull] this IReadOnlyList<T> list)
            where T : IComparable<T>
        {
            int maxIndex = 0;
            T maxValue = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].CompareTo(maxValue) > 0)
                {
                    maxIndex = i;
                    maxValue = list[i];
                }
            }

            return maxIndex;
        }
        public static IEnumerable RandomItems<T>([NotNull] this T list, int count)
            where T : IList
        {
            if (count > list.Count)
            {
                throw new Exception("Count cannot be more than list count");
            }

            if (count < 0)
            {
                throw new Exception("Count cannot be less than 0");
            }

            var indexes = Enumerable.Range(0, list.Count).ToList();
            indexes.Shuffle();
            for (int i = 0; i < count; i++)
            {
                yield return list[indexes[i]];
            }
        }
    }
}
