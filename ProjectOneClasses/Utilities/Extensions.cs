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
    }
}
