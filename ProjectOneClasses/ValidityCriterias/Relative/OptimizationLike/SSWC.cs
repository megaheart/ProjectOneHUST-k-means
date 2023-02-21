using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike
{
    /// <summary>
    /// Greater index, better partition
    /// </summary>
    public class SSWC
    {
        public double Index { get; private set; }
        public static double GetSquareDistanse(double[] x1, double[] x2)
        {
            double sum = 0;
            for (int i = 0; i < x1.Length; i++)
            {
                sum += (x1[i] - x2[i]) * (x1[i] - x2[i]);
            }
            return sum;

        }
        public SSWC(IReadOnlyList<double[]> X, int C/*, IReadOnlyList<int> expect*/, IFCM_Result result)
        {
            int dimension = X[0].Length;
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
            //Calculate centroids
            double[][] centroids = new double[C][];
            
            for (var i = 0; i < C; i++)
            {
                double[] c = new double[dimension];
                for (var j = 0; j < dimension; j++)
                {
                    c[j] = 0;
                }
                foreach (var x in clusters[i])
                {
                    for (var j = 0; j < dimension; j++)
                    {
                        c[j] += X[x][j];
                        //grandCentroid[j] += X[x][j];
                    }
                }
                for (var j = 0; j < dimension; j++)
                {
                    c[j] /= clusters[i].Count;
                }
                centroids[i] = c;
            }

            //Calculate s(x[i]) -> Calculate Index
            double sum = 0;
            for (var p = 0; p < C; p++)
            {
                //if (clusters[p].Count == 0) throw new Exception("Cluster must have least one element");
                if (clusters[p].Count <= 1)
                {
                    continue;
                }
                foreach (var j in clusters[p])
                {
                    double apj = Math.Sqrt(GetSquareDistanse(X[j], centroids[p]));
                    double bpj = double.MaxValue;
                    for (var q = 0; q < C; q++)
                    {
                        if(p == q) { continue; }
                        double dqj = Math.Sqrt(GetSquareDistanse(X[j], centroids[q]));
                        if(dqj < bpj) { 
                            bpj = dqj;
                        }
                    }
                    double s_xj = (bpj - apj) / Math.Max(bpj, apj);
                    sum += s_xj;
                }
            }

            Index = sum / X.Count;
        }
    }
}
