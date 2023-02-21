using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike
{
    /// <summary>
    /// Lower index, better partition
    /// </summary>
    public class DB
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
        public DB(IReadOnlyList<double[]> X, int C/*, IReadOnlyList<int> expect*/, IFCM_Result result)
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
                    }
                }
                for (var j = 0; j < dimension; j++)
                {
                    c[j] /= clusters[i].Count;
                }
                centroids[i] = c;
            }
            //Calculate d(l)
            double[] dl = new double[C];
            for (var l = 0; l < C; l++)
            {
                double sum = 0;
                foreach (var x in clusters[l])
                {
                    sum += Math.Sqrt(GetSquareDistanse(X[x], centroids[l]));
                }
                dl[l] = sum / clusters[l].Count;
            }
            //Calculate d(l, m)
            double[][] dlm = new double[C][];
            for (var i = 0; i < C; i++)
            {
                dlm[i] = new double[C];
            }
            for (var l = 0; l < C; l++)
            {
                for (var m = l + 1; m < C; m++)
                {
                    dlm[l][m] = dlm[m][l] = Math.Sqrt(GetSquareDistanse(centroids[l], centroids[m]));
                }
            }
            //Calculate DB
            double sum2 = 0;            
            for (var l = 0; l < C; l++)
            {
                //Calculate Dl
                double Dl = 0;
                for (var m = 0; m < C; m++)
                {
                    if (l == m) continue;
                    double Dlm = (dl[l] + dl[m]) / dlm[l][m];
                    if (Dl < Dlm) Dl = Dlm;
                }
                sum2 += Dl;
            }
            Index = sum2 / C;
        }
    }
}
