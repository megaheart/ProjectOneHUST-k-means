using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ValidityCriterias.External
{
    public class TrainAccuracy
    {
        public double Index { get; private set; }
        public TrainAccuracy(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, int C, IFCM_Result result)
        {
            if(Y.Count == 0)
            {
                Index = 0;
                return;
            }

            List<int>[] clusters = new List<int>[C];
            for (int i = 0; i < C; i++) clusters[i] = new List<int>(X.Count / C + 1);
            var U = result.U;

            foreach(var y in Y)
            {
                int i = y.Key, k = y.Value;

                int k_max = 0;
                for (int k_actual = 1; k_actual < C; k_actual++)
                {
                    if (U[i][k_actual] > U[i][k_max]) k_max = k_actual;
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
                    int ii = Y[i];
                    tmpA[ii]++;
                }
                int k_max = 0;
                for (int k = 1; k < C; k++)
                {
                    if (tmpA[k] > tmpA[k_max]) k_max = k;
                }

                foreach (var i in cluster)
                {
                    if (Y[i] != k_max)
                    {
                        notAsExpect++;
                        //notAsExpectPoints.Add(new _NotAsExpectPoint(X[i], expect[i], k_max));
                    }
                }
            }
            //Result = new _Result(X.Count - notAsExpect, notAsExpect, notAsExpectPoints);
            Index = 1.0 * (Y.Count - notAsExpect) / Y.Count;
        }
    }
}
