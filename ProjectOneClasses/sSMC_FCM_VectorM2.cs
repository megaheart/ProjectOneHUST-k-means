using ProjectOneClasses.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses
{
    public class sSMC_FCM_VectorM2
    {
        private double M, alpha;
        private IReadOnlyList<double[]> X;
        private int C;
        private IReadOnlyDictionary<int, int> Y;
        private double epsilon;
        public sSMC_FCM_VectorM2([NotNull] IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> Y,
            double M = 2, double alpha = 0.6, double epsilon = 0.0001)
        {
            if (C < 1) throw new Exception("C must be more than 0");
            if (C > X.Count) throw new Exception("C cannot be more than number of X");
            this.X = X.ToImmutableArray();
            this.C = C;
            this.epsilon = epsilon;
            this.M = M;
            this.alpha = alpha;
            this.Y = Y.ToImmutableDictionary();
        }
        public class sSMC_FCM_VectorM2_Result : IFCM_Result
        {
            public IReadOnlyList<double[]> V { get; private set; }
            public IReadOnlyList<IReadOnlyList<double>> U { get; private set; }
            public int l { get; private set; }

            public double M { get; private set; }

            public double[] M2 { get; private set; }

            public sSMC_FCM_VectorM2_Result([NotNull] double[][] _V, [NotNull] double[][] _U, int l, double M, double[] M2)
            {
                this.V = _V;
                this.U = _U;
                this.l = l;
                this.M = M;
                this.M2 = M2;
            }
            public void printToConsole()
            {
                Console.WriteLine("Display V:");
                foreach (var row in V)
                {
                    string s = "", ss;
                    foreach (var col in row)
                    {
                        ss = col.ToString();
                        int n = 22 - ss.Length;
                        ss = string.Concat(Enumerable.Repeat(" ", n)) + ss;
                        s += ", " + ss;
                    }
                    s = s.Substring(2);

                    Console.WriteLine("({0})", s);
                }
                Console.WriteLine("Display U:");
                foreach (var row in U)
                {
                    foreach (var col in row)
                    {
                        Console.Write("{0,23}", col);
                    }
                    Console.WriteLine();
                }
            }
        }
        public sSMC_FCM_VectorM2_Result Result { get; private set; } = null;
        public void _solve()
        {
            //Initialize input parameter
            int dimension = X[0].Length;
            double[][] V;
            int n = X.Count;
            double[][] U = new double[n][];
            double[][] D = new double[n][];
            double[][] mu = new double[n][];
            //double[] sum_mu_i_j = new double[n];

            for (int i = 0; i < n; i++)
            {
                U[i] = new double[C];

            }
            double[][] V_last = new double[C][];
            for (int i = 0; i < C; i++)
            {
                V_last[i] = new double[dimension];

            }
            foreach (var i in Y.Keys)
            {
                D[i] = new double[C];
                mu[i] = new double[C];
            }
            double[] M2 = null;
            //B1: Generate first clusters
            V = _GenerateFirstCClusters(X, Y, C, epsilon);
            //var fcm = new MC_FCM(X, C);
            //fcm._solve();
            //V = fcm.Result.V.ToArray();

            //Main
            int l = 0;
            while (true)
            {
                l++;
                //B2: Update U[i][k]

                //--- without supervision
                for (int i = 0; i < n; i++)
                {
                    double[] delta = new double[C];
                    double a = 1 / (M - 1), b, c = 0;
                    int hasZeroDis = -1;

                    for (int k = 0; k < C; k++)
                    {
                        if (D[i] != null)
                        {
                            //Cache D[i][j] - distance between [with-supervision] point X[i] and cluster Y[k]
                            double d = GetSquareDistanse(X[i], V[k]);
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
                            double d = GetSquareDistanse(X[i], V[k]);
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
                //--- with supervision at k th cluster

                //Calculate M' (3.3)
                if (l < 2)
                {
                    //M2 = 3;
                    M2 = CalculateM2(Y/*, n, C*/, M, alpha, U);
                }
                

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
                    double aa = 1 / (M2[i] - 1), bb = (M2[i] - M) / (M2[i] - 1);
                    double right = Math.Pow(Math.Pow(D[i][k], 2) * M2[i], -aa);
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

                //B3: Update V
                for (int k = 0; k < C; k++)
                {
                    Array.Copy(V[k], 0, V_last[k], 0, dimension);// Store old V
                    double a = 0, _m;
                    for (int d = 0; d < dimension; d++)
                    {
                        V[k][d] = 0;
                    }
                    int s_k = -1;
                    for (int i = 0; i < n; i++)
                    {
                        //Calculate m(i, k) (2.8)
                        if (Y.TryGetValue(i, out s_k) && s_k == k)
                            _m = Math.Pow(U[i][k], M2[i]);
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


                //B4: Check whether converging
                bool isConverging = true;
                for (int k = 0; k < C; k++)
                {
                    double delta = Math.Sqrt(GetSquareDistanse(V[k], V_last[k]));
                    if (delta >= epsilon) isConverging = false;
                }
                if (isConverging) break;
            }

            Result = new sSMC_FCM_VectorM2_Result(V, U, l, M, M2);




        }
        public static double[] CalculateM2(IReadOnlyDictionary<int, int> Y/*, int n, int C*/, double M, double alpha, double[][] U)
        {
            double[] M2 = new double[U.Length];

            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;
                M2[i] = Math.Min(M2_PrecalculationTable.Instance[U[i][k]], 8);
            }

            return M2;
        }
        public static double GetSquareDistanse(double[] x1, double[] x2)
        {
            double sum = 0;
            for (int i = 0; i < x1.Length; i++)
            {
                sum += (x1[i] - x2[i]) * (x1[i] - x2[i]);
            }
            return sum;

        }
        public double[][] _GenerateFirstCClusters(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, int C, double epsilon)
        {
            return ClustersGenerator.sSMC_FCM_KMeanPlusPlus(X, Y, C, epsilon);

        }
    }
}
