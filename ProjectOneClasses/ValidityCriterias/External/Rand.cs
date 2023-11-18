using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOneClasses.ResultTypes;

namespace ProjectOneClasses.ValidityCriterias.External
{
    /// <summary>
    /// Greater index, better partition (<= 1)
    /// </summary>
    public class Rand
    {
        public double Index { get; private set; }

        private void CalculateIndex(IReadOnlyList<double[]> X, IReadOnlyList<int> expect, IReadOnlyList<int> actual)
        {
            int a, b, c, d;
            a = b = c = d = 0;
            for (int i = 0; i < X.Count; i++)
            {
                for (int j = i + 1; j < X.Count; j++)
                {
                    bool hasSameClassInExpect = expect[i] == expect[j];//belonging to the same class in R - expect
                    bool inSameClusterInResult = actual[i] == actual[j];//belonging to the same cluster in Q - result
                    if (hasSameClassInExpect)
                    {
                        if (inSameClusterInResult)
                        {
                            a++;
                        }
                        else
                        {
                            b++;
                        }
                    }
                    else
                    {
                        if (inSameClusterInResult)
                        {
                            c++;
                        }
                        else
                        {
                            d++;
                        }
                    }
                }
            }

            Index = (a + d) * 1.0 / (a + b + c + d);
        }

        public Rand(IReadOnlyList<double[]> X, int C, IReadOnlyList<int> expect, IReadOnlyList<int> actual)
        {
            CalculateIndex(X, expect, actual);
        }

        public Rand(IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> expect, IReadOnlyDictionary<int, int> actual)
        {
            var expectList = Utils.DictionaryToList(expect);
            var actualList = Utils.DictionaryToList(actual);

            CalculateIndex(X, expectList, actualList);
        }

        public Rand(IReadOnlyList<double[]> X, int C, IReadOnlyList<int> expect, IFCM_Result result)
        {
            int[] actual = new int[X.Count];//Point X[i] is belonging to cluster whose index in V = actual[i]
            var U = result.U;
            for (int i = 0; i < X.Count; i++)
            {
                int k_max = 0;
                for (int k = 1; k < C; k++)
                {
                    if (U[i][k] > U[i][k_max]) k_max = k;
                }
                actual[i] = k_max;
            }
            
            CalculateIndex(X, expect, actual);
        }
    }
}
