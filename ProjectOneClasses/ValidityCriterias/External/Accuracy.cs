using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ValidityCriterias.External
{
    /// <summary>
    /// Greater index, better partition
    /// </summary>
    public class Accuracy
    {
        public class _NotAsExpectPoint
        {
            public _NotAsExpectPoint(double[] point, int expectClusterIndex, int actualClusterIndex/*, int expectClusterName, int actualClusterName*/)
            {
                Point = point;
                ExpectClusterIndex = expectClusterIndex;
                ActualClusterIndex = actualClusterIndex;
                //ExpectClusterName = expectClusterName;
                //ActualClusterName = actualClusterName;
            }

            public double[] Point { get; private set; }
            public int ExpectClusterIndex { get; private set; }
            public int ActualClusterIndex { get; private set; }
            //public int ExpectClusterName { get; private set; }
            //public int ActualClusterName { get; private set; }
        }
        public class _Result
        {
            public _Result(int asExpect, int notAsExpect, IReadOnlyList<_NotAsExpectPoint> notAsExpectPoints)
            {
                AsExpect = asExpect;
                NotAsExpect = notAsExpect;
                NotAsExpectPoints = notAsExpectPoints;
            }

            public int AsExpect { get; private set; }
            public int NotAsExpect { get; private set; }
            public IReadOnlyList<_NotAsExpectPoint> NotAsExpectPoints { get; private set; }
        }
        //public _Result Result { get; private set; }
        public double Index { get; private set; }

        public Accuracy(IReadOnlyList<double[]> X, int C, IReadOnlyList<int> expect, IFCM_Result result)
        {
            List<int>[] clusters = new List<int>[C];
            for (int i = 0; i < C; i++) clusters[i] = new List<int>(X.Count / C + 1);
            var U = result.U;
            for (int i = 0; i < X.Count; i++)
            {
                int k_max = 0;
                for (int k = 1; k < C; k++)
                {
                    if (U[i][k] > U[i][k_max]) k_max = k;
                }
                clusters[k_max].Add(i);
            }
            int[] tmpA = new int[C];
            int notAsExpect = 0;
            //var notAsExpectPoints = new List<_NotAsExpectPoint>();
            foreach (var cluster in clusters)
            {
                for (int i = 0; i < C; i++) tmpA[i] = 0;
                foreach (var i in cluster)
                {
                    int ii = expect[i];
                    tmpA[ii]++;
                }
                int k_max = 0;
                for (int k = 1; k < C; k++)
                {
                    if (tmpA[k] > tmpA[k_max]) k_max = k;
                }

                foreach (var i in cluster)
                {
                    if (expect[i] != k_max)
                    {
                        notAsExpect++;
                        //notAsExpectPoints.Add(new _NotAsExpectPoint(X[i], expect[i], k_max));
                    }
                }
            }
            //Result = new _Result(X.Count - notAsExpect, notAsExpect, notAsExpectPoints);
            Index = 1.0 * (X.Count - notAsExpect) / X.Count;
        }
    }
}
