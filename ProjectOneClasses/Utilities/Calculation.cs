using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Utilities
{
    public class Calculation
    {
        public static double EuclideanDistanseSquared(double[] x1, double[] x2)
        {
            double sum = 0;
            for (int i = 0; i < x1.Length; i++)
            {
                sum += (x1[i] - x2[i]) * (x1[i] - x2[i]);
            }
            return sum;

        }

        public static IEnumerable<IList<T>> GenerateAllCombinations<T>(IList<T> set, int k)
        {
            if (k > set.Count)
                throw new ArgumentException("k cannot be greater than set size", "k");

            var indexes = Enumerable.Range(0, k).ToArray();

            do
            {
                yield return indexes.Select(i => set[i]).ToArray();

                for (int i = k - 1; i > -1; i--)
                {
                    if (indexes[i] < set.Count - k + i)
                    {
                        indexes[i]++;
                        for (int j = i + 1; j < k; j++)
                        {
                            indexes[j] = indexes[j - 1] + 1;
                        }
                        break;
                    }
                    else if (i == 0)
                    {
                        indexes = null;
                        break;
                    }
                }
            } while (indexes != null);
        }

    }
}
