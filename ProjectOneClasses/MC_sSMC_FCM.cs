using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectOneClasses.MC_FCM;
using static ProjectOneClasses.sSMC_FCM;

namespace ProjectOneClasses
{
    public class MC_sSMC_FCM
    {
        private IReadOnlyList<double[]> X;
        private int C;
        private IReadOnlyDictionary<int, int> Y;
        private double epsilon, alpha;
        public MC_sSMC_FCM([NotNull] IReadOnlyList<double[]> X, int C, 
            [NotNull] IReadOnlyDictionary<int, int> Y,
            double alpha = 0.6, double epsilon = 0.0001)
        {
            if (C < 1) throw new Exception("C must be more than 0");
            if (C > X.Count) throw new Exception("C cannot be more than number of X");
            this.X = X;
            this.C = C;
            this.epsilon = epsilon;
            this.alpha = alpha;
            this.Y = Y;
        }
        public class MC_sSMC_FCM_Result : IFCM_Result
        {
            public IReadOnlyList<double[]> V { get; private set; }
            public IReadOnlyList<IReadOnlyList<double>> U { get; private set; }
            public int l { get; private set; }
            public MC_sSMC_FCM_Result([NotNull] double[][] _V, [NotNull] double[][] _U, int l)
            {
                this.V = _V;
                this.U = _U;
                this.l = l;
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
        public MC_sSMC_FCM_Result Result { get; private set; } = null;

        public void _solve()
        {
            //Initialize input parameter
            int dimension = X[0].Length;
            double[][] V;
            int n = X.Count;
            double[][] U = new double[n][];
            double[][] D = new double[n][];
            double[][] mu = new double[n][];//mu mean μ
            double[] sum_mu_i_j = new double[n];

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

            const double ml = 1.1, _mu = 4.1;
            //B0: Generate m
            double[] m = _GetFuzzificationCoefficients(X, C, ml, _mu);
            //B1: Generate first clusters
            V = _GenerateFirstCClusters(X, C, m, epsilon);



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
                    double a = 1 / (m[i] - 1), b, c = 0;
                    for (int k = 0; k < C; k++)
                    {
                        if (D[i] != null)
                        {
                            //Cache D[i][j] - distance between [with-supervision] point X[i] and cluster Y[k]
                            b = GetSquareDistanse(X[i], V[k]);
                            D[i][k] = Math.Sqrt(b);
                            b = Math.Pow(b, a);
                        }
                        else
                        {
                            b = Math.Pow(GetSquareDistanse(X[i], V[k]), a);
                        }
                        //Calculate D(i, k)^(2 * a)
                        delta[k] = b;
                        //Calculate sigma sum
                        c += 1 / b;
                    }
                    for (int k = 0; k < C; k++)
                    {
                        U[i][k] = 1 / (delta[k] * c); //(2.10)
                    }
                }
                //--- with supervision at k th cluster

                foreach (var y in Y)
                {
                    int i = y.Key, k = y.Value;
                    //Get d_min (2.11)
                    double dmin = D[i].Min();
                    //Calculate d(i, j) (2.11)
                    for (int j = 0; j < C; j++)
                    {
                        D[i][j] = D[i][j] / dmin;
                    }
                    //Calculate mu(i, j) with all j != k (2.12)
                    double a = 1 / (m[i] - 1), b;

                    for (int j = 0; j < C; j++)
                    {
                        if (j != k)
                        {
                            b = Math.Pow(Math.Pow(D[i][j], 2) * m[i], -a);
                            mu[i][j] = b;
                            sum_mu_i_j[i] += b;
                        }
                    }
                }
                //Calculate M' (3.3)

                double[] M2 = new double[n];

                foreach (var y in Y)
                {
                    int i = y.Key, k = y.Value;
                    //Calculate M' (3.3)
                    double m2 = M2[i] = CalculateM2(Y/*, n, C*/, m[i], alpha, U);
                    double aa = 1 / (m2 - 1), bb = (m2 - m[i]) / (m2 - 1);
                    //Calculate mu(i, k) (2.13)
                    double right = Math.Pow(Math.Pow(D[i][k], 2) * m2, -aa);
                    double c = sum_mu_i_j[i];
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
                            _m = Math.Pow(U[i][k], m[i]);

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


            Result = new MC_sSMC_FCM_Result(V, U, l);
        }
        public static double CalculateM2(IReadOnlyDictionary<int, int> Y/*, int n, int C*/, double M, double alpha, double[][] U)
        {
            double right_min = int.MaxValue;
            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;
                double right = M * Math.Pow((1 - alpha) / ((1 / U[i][k]) - 1), M - 1);
                if (right < right_min) right_min = right;
            }
            double M2 = Math.Max(M, -1 / Math.Log(alpha)); // Start value of M2
            double left = M2 * Math.Pow(alpha, M2 - 1);
            while (left > right_min)
            {
                M2 *= 2;
                left = M2 * Math.Pow(alpha, M2 - 1);
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
        public double[][] _GenerateFirstCClusters(IReadOnlyList<double[]> X, int C, double[] m, double epsilon)
        {
            return _GenerateFirstCClusters_MaxFuzzificationCoefficients(X, C, m, epsilon);
        }
        public static double[][] _GenerateFirstCClusters_MaxFuzzificationCoefficients(IReadOnlyList<double[]> X, int C, double[] m, double epsilon)
        {
            PriorityQueue<double[], double> priorityQueue = new PriorityQueue<double[], double>(C);
            int i = 0;
            for (i = 0; i < X.Count; i++)
            {
                if (priorityQueue.Count < C) priorityQueue.Enqueue(X[i], -m[i]);
                else priorityQueue.EnqueueDequeue(X[i], -m[i]);
            }
            double[][] V = new double[C][];
            double[] _x1, _x2;
            int l;
            for (i = 0; i < C; i++)
            {
                _x1 = priorityQueue.Dequeue();
                l = _x1.Length;
                _x2 = new double[l];
                Array.Copy(_x1, 0, _x2, 0, l);
                _x2[0] += epsilon;
                V[i] = _x2;
            }
            return V;
        }
    }
}
