using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.ValidityCriterias.External;
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
    public class PBM
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

        private void CalculateIndex(IReadOnlyList<double[]> X, int C, List<int>[] clusters)
        {
            int dimension = X[0].Length;
            //Calculate centroids
            double[][] centroids = new double[C][];
            //Grand centroid
            double[] grandCentroid = new double[dimension];
            for (var j = 0; j < dimension; j++)
            {
                grandCentroid[j] = 0;
            }
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
                        grandCentroid[j] += X[x][j];
                    }
                }
                for (var j = 0; j < dimension; j++)
                {
                    c[j] /= clusters[i].Count;
                }
                centroids[i] = c;
            }
            for (var j = 0; j < dimension; j++)
            {
                grandCentroid[j] /= X.Count;
            }
            // Calculate E1 - the sum of distances between the objects and the grand mean of the data
            double E1 = 0;
            for (int i = 0; i < X.Count; i++)
            {
                E1 += Math.Sqrt(GetSquareDistanse(X[i], grandCentroid));
            }
            // Calculate EK - the sum of within-group distances
            double EK = 0;
            for (var l = 0; l < C; l++)
            {
                foreach (var i in clusters[l])
                {
                    EK += Math.Sqrt(GetSquareDistanse(X[i], centroids[l]));
                }
            }
            // Calculate DK - the maximum distance between group centroids
            double DK = 0;
            for (var l = 0; l < C; l++)
            {
                for (var m = l + 1; m < C; m++)
                {
                    double d = Math.Sqrt(GetSquareDistanse(centroids[l], centroids[m]));
                    if (d > DK) DK = d;
                }
            }
            //Calculate index
            Index = (E1 * DK) / (EK * C);
            Index = Index * Index;
        }

        public PBM(IReadOnlyList<double[]> X, int C, IReadOnlyList<int> predicts)
        {
            var clusters = Utils.PredictionsToClusters(C, predicts);

            // Calculate index
            CalculateIndex(X, C, clusters);
        }

        public PBM(IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> predicts)
        {
            var clusters = Utils.PredictionsToClusters(C, predicts);

            // Calculate index
            CalculateIndex(X, C, clusters);
        }

        public PBM(IReadOnlyList<double[]> X, int C/*, IReadOnlyList<int> expect*/, IFCM_Result result)
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

            // Calculate index
            CalculateIndex(X, C, clusters);

        }
    }
}
