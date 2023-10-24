using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Cores
{
    public class sSMC_FCM_Core
    {
        public static void UpdateU_NonSupervision(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, IReadOnlyList<double[]> V, double[][] D, double[][] U, double M)
        {
            int n = X.Count;
            int C = V.Count;
            for (int i = 0; i < n; i++)
            {
                double[] delta = new double[C];
                double a = 1 / (M - 1), b, c = 0;
                int hasZeroDis = -1;
                bool isSupervision = Y.ContainsKey(i);

                for (int k = 0; k < C; k++)
                {
                    if (isSupervision)
                    {
                        //Cache D[i][j] - distance between [with-supervision] point X[i] and cluster Y[k]
                        double d = Calculation.EuclideanDistanseSquared(X[i], V[k]);
                        if (d == 0)
                        {
                            hasZeroDis = k;
                            break;
                        }
                        D[i][k] = Math.Sqrt(d);
                        b = Math.Pow(d, a);
                    }
                    else
                    {
                        double d = Calculation.EuclideanDistanseSquared(X[i], V[k]);
                        if (d == 0)
                        {
                            hasZeroDis = k;
                            break;
                        }
                        b = Math.Pow(d, a);
                    }
                    //Calculate D(i, k)^(2 * a)
                    delta[k] = b;
                    //Calculate sigma sum
                    c += 1 / b;
                }

                if (hasZeroDis == -1)
                {
                    for (int k = 0; k < C; k++)
                    {
                        U[i][k] = 1 / (delta[k] * c); //(2.10)
                    }
                }
                else
                {
                    for (int k = 0; k < C; k++)
                    {
                        U[i][k] = 0;
                    }
                    U[i][hasZeroDis] = 1;
                }
            }
        }
        public static void UpdateU_Supervision(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, IReadOnlyList<double[]> V, double[][] D, double[][] U, double[][] mu, double M, double M2, double epsilon)
        {
            int n = X.Count;
            int C = V.Count;
            double aa = 1 / (M2 - 1), bb = (M2 - M) / (M2 - 1);

            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;
                double sum_mu_i_j = 0;
                //Get d_min (2.11)
                double dmin = D[i].Min();

                if (dmin == 0)
                {
                    for (int j = 0; j < C; j++)
                    {
                        U[i][j] = 0;
                    }
                    for (int j = 0; j < C; j++)
                    {
                        if (D[i][j] == 0)
                        {
                            U[i][j] = 1;
                            break;
                        }
                    }
                    continue;
                }

                //Calculate d(i, j) (2.11)
                for (int j = 0; j < C; j++)
                {
                    D[i][j] = D[i][j] / dmin;
                }
                //Calculate mu(i, j) with all j != k (2.12)
                double a = 1 / (M - 1), b;

                for (int j = 0; j < C; j++)
                {
                    if (j != k)
                    {
                        b = Math.Pow(Math.Pow(D[i][j], 2) * M, -a);
                        mu[i][j] = b;
                        sum_mu_i_j += b;
                    }
                }

                //Calculate mu(i, k) (2.13)
                double right = Math.Pow(Math.Pow(D[i][k], 2) * M2, -aa);
                double c = sum_mu_i_j;
                Func<double, double> func1 = x => (x / Math.Pow(x + c, bb)) - right;
                var solver = new SimpleOneVariableEquationSolver(func1, epsilon);
                solver.Solve();
                mu[i][k] = solver.Root;
                c += mu[i][k];
                //standardize U[i][j] (2.14)
                for (int j = 0; j < C; j++)
                {
                    U[i][j] = mu[i][j] / c;
                }
                if (Math.Abs(U[i].Sum() - 1) > epsilon)
                    throw new Exception("sum U[i][j] = 1 with all j = 1, ... , c, but now sum = " + U[i].Sum());
            }
        }
        public static void UpdateV(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, IReadOnlyList<double[]> V, IReadOnlyList<double[]> V_last, double[][] U, double M, double M2)
        {
            int n = X.Count;
            int C = V.Count;
            int dimension = V[0].Length;

            for (int k = 0; k < C; k++)
            {
                Array.Copy(V[k], 0, V_last[k], 0, dimension);// Store old V
                double a = 0, _m;
                for (int d = 0; d < dimension; d++)
                {
                    V[k][d] = 0;
                }
                
                for (int i = 0; i < n; i++)
                {
                    //Calculate m(i, k) (2.8)
                    if (Y.TryGetValue(i, out var s_k) && s_k == k)
                        _m = Math.Pow(U[i][k], M2);
                    else
                        _m = Math.Pow(U[i][k], M);

                    for (int d = 0; d < dimension; d++)
                    {
                        V[k][d] += _m * X[i][d];
                    }
                    a += _m;
                }
                for (int d = 0; d < dimension; d++)
                {
                    V[k][d] /= a;
                }
            }
        }
        public static bool CheckConverging(IReadOnlyList<double[]> V, IReadOnlyList<double[]> V_last, double epsilon)
        {
            int C = V.Count;
            for (int k = 0; k < C; k++)
            {
                double delta = Math.Sqrt(Calculation.EuclideanDistanseSquared(V[k], V_last[k]));
                if (delta >= epsilon) return false;
            }
            return true;
        }
        
        public static double CalculateM2(IReadOnlyDictionary<int, int> Y/*, int n, int C*/, double M, double alpha, double[][] U)
        {
            double u_min = 2;
            if (Y.Count > 0) { u_min = Y.Select(p => U[p.Key][p.Value]).Min(); }
            return M2_PrecalculationTable.Instance[u_min];
        }
        public static double[][] GenerateFirstCClusters(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, int C, double epsilon)
        {
            return ClustersGenerator.sSMC_FCM_KMeanPlusPlus(X, Y, C, epsilon);

        }
    }
}
