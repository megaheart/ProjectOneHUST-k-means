using System.Diagnostics.CodeAnalysis;
using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Utilities;

namespace ProjectOneClasses
{
    public delegate double[][] MC_FCM_InitializeCluster(IReadOnlyList<double[]> X, int C, double[] m, double epsilon);
    public class MC_FCM
    {
        public bool IsCompleted { get; private set; }
        protected double[] m = null;
        private IReadOnlyList<double[]> X;
        private int C;
        private double epsilon;
        int maxInterator;
        public MC_FCM_Result Result { get; private set; } = null;
        public MC_FCM([NotNull] IReadOnlyList<double[]> X, int C, double epsilon = 0.0001, int maxInterator = 200, double[] m = null)
        {
            if (C < 1) throw new Exception("C must be more than 0");
            if (C > X.Count) throw new Exception("C cannot be more than number of X");
            this.X = X;
            this.C = C;
            this.m = m;
            this.epsilon = epsilon;
            this.maxInterator = maxInterator;
        }
        public void Solve(Action<MC_FCM> callback)
        {
            Task task = Task.Run(_solve);
            task.ConfigureAwait(false);
            task.ContinueWith(t => callback(this));
        }

        public void _solve()
        {
            //Initialize input parameter
            const double ml = 1.1, mu = 4.1;
            double epsilon = this.epsilon;
            int dimension = X[0].Length;
            double[][] V;
            if (m == null)
            {
                if (ClustersGenerationMode == CGMode.MaxFuzzificationCoefficientGroups)
                {
                    GenerateFuzzificationCoefficientsAndFirstCClusters_MaxFuzzificationCoefficientGroups(X, C, ml, mu, epsilon, out m, out V);
                }
                else
                {
                    m = _GetFuzzificationCoefficients(X, C, ml, mu);
                    //B1
                    V = _GenerateFirstCClusters(X, C, m, epsilon);
                }
            }
            else
            {
                //B1
                V = _GenerateFirstCClusters(X, C, m, epsilon);
            }
            int n = X.Count;
            //int l = 0;
            double[][] U = new double[n][];
            //double[][] delta = new double[n][];
            for (int i = 0; i < n; i++)
            {
                U[i] = new double[C];

            }
            double[][] V_last = new double[C][];
            for (int i = 0; i < C; i++)
            {
                V_last[i] = new double[dimension];

            }
            //Main
            int l = 0;
            while (l < maxInterator)
            {
                l++;
                //B2: Update U[i][k]
                for (int i = 0; i < n; i++)
                {
                    double[] delta = new double[C];
                    double a = 1 / (m[i] - 1), b, c = 0;

                    int hasZeroDis = -1;

                    for (int k = 0; k < C; k++)
                    {
                        double d = GetSquareDistanse(X[i], V[k]);
                        if (d == 0)
                        {
                            hasZeroDis = k;
                            break;
                        }
                        b = Math.Pow(d, a);
                        delta[k] = b;
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
                //B3: Update V
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
            Result = new MC_FCM_Result(V, U, l);
        }
        public static double ScalarProduct(double[] x1, double[] x2)
        {
            double sum = 0;
            for (int i = 0; i < x1.Length; i++)
            {
                sum += x1[i] * x2[i];
            }
            return sum;

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
        public static double GetMedian(double[] l, bool isImmutable = true)
        {
            double[] _l;
            if (isImmutable)
            {
                _l = new double[l.Length];
                Array.Copy(l, _l, l.Length);
            }
            else
            {
                _l = l;
            }
            return _l.NthOrderStatistic((_l.Length - 1) / 2);
        }

        public static double[] _GetFuzzificationCoefficients(IReadOnlyList<double[]> X, int C, double ml, double mu)
        {
            int n = X.Count;
            int nn = n / C;
            PriorityQueue<double, double>[] priorityQueues = new System.Collections.Generic.PriorityQueue<double, double>[n];
            for (int i = 0; i < n; i++) priorityQueues[i] = new PriorityQueue<double, double>(nn);
            void pushToPriorityQueues(int i, double d)
            {
                if (priorityQueues[i].Count < nn) priorityQueues[i].Enqueue(d, -d);
                else priorityQueues[i].EnqueueDequeue(d, -d);
            }
            //Calculate the distance between two point in X list
            double[][] delta = new double[n - 1][];
            for (int i = 0; i < n - 1; i++)
            {
                delta[i] = new double[n - i - 1];
                for (int j = 0; j < delta[i].Length; j++)
                {
                    var d = Math.Sqrt(GetSquareDistanse(X[i], X[i + j + 1]));
                    delta[i][j] = d;
                    pushToPriorityQueues(i, d);
                    pushToPriorityQueues(i + j + 1, d);
                }
            }
            /*double getDelta(int i, int j)
            {
                if (i == j) return double.PositiveInfinity;
                if (i > j) return delta[j][i - j - 1];
                return delta[i][j - i - 1];
            }*/

            //Calculate density index of each point in X list
            double[] delta2 = new double[n];
            int delta2_minIdx = 0, delta2_maxIdx = 0;
            for (int i = 0; i < n; i++)
            {
                double sum = 0, d;
                while (priorityQueues[i].TryDequeue(out d, out _))
                {
                    sum += d;
                }
                delta2[i] = sum;
                //Get index of element which has max density index
                if (sum > delta2[delta2_maxIdx])
                {
                    delta2_maxIdx = i;
                }
                //Get index of element which has min density index
                if (sum < delta2[delta2_minIdx])
                {
                    delta2_minIdx = i;
                }
            }

            //Calculate power number of function (alpha)
            double delta2_min = delta2[delta2_minIdx], delta2_max_min = delta2[delta2_maxIdx] - delta2_min;
            double median = GetMedian(delta2);
            double alpha = Math.Log(0.5, (median - delta2_min) / delta2_max_min);
            //Calculate fuzzification coefficients
            double[] coefficients = new double[n];
            for (int i = 0; i < n; i++)
            {
                coefficients[i] = ml + (mu - ml) * Math.Pow((delta2[i] - delta2_min) / delta2_max_min, alpha);
            }

            return coefficients;
        }
        public struct XPoint
        {
            public int i;
            public double d;
            public XPoint(int i, double d)
            {
                this.i = i;
                this.d = d;
            }
        }
        public static void GenerateFuzzificationCoefficientsAndFirstCClusters_MaxFuzzificationCoefficientGroups(
            IReadOnlyList<double[]> X, int C, double ml, double mu, double epsilon, out double[] m, out double[][] V)
        {
            int n = X.Count;
            int nn = n / C;
            int dimension = X[0].Length;
            //Calculate density index of each point in X list
            double[] delta2 = new double[n];
            for (int i = 0; i < n; i++) delta2[i] = 0;
            PriorityQueue<XPoint, double>[] priorityQueues = new PriorityQueue<XPoint, double>[n];
            for (int i = 0; i < n; i++) priorityQueues[i] = new PriorityQueue<XPoint, double>(nn);
            double pushToPriorityQueues(int i, int xi, double d)
            {
                double dd = d;
                if (priorityQueues[i].Count < nn) priorityQueues[i].Enqueue(new XPoint(xi, d), -d);
                else
                {
                    XPoint x_d_max = priorityQueues[i].EnqueueDequeue(new XPoint(xi, d), -d);
                    dd -= x_d_max.d;
                }
                return dd;
            }
            //Calculate the distance between two point in X list
            /*double[][] delta = new double[n - 1][];
            double getDelta(int i, int j)
            {
                if (i == j) return double.PositiveInfinity;
                if (i > j) return delta[j][i - j - 1];
                return delta[i][j - i - 1];
            }*/
            for (int i = 0; i < n - 1; i++)
            {
                //delta[i] = new double[n - i - 1];
                for (int j = i + 1; j < n; j++)
                {
                    var d = Math.Sqrt(GetSquareDistanse(X[i], X[j]));
                    //delta[i][j] = d;
                    delta2[i] += pushToPriorityQueues(i, j, d);
                    delta2[j] += pushToPriorityQueues(j, i, d);
                }
            }

            XPoint[] delta3 = new XPoint[n];
            for (int i = 0; i < n; i++)
            {
                delta3[i].d = delta2[i];
                delta3[i].i = i;
            }
            Array.Sort(delta3, (a, b) =>
            {
                return b.d.CompareTo(a.d);
            });

            //Calculate power number of function (alpha)
            double delta2_min = delta3[n - 1].d, delta2_max_min = delta3[0].d - delta2_min;
            double median = GetMedian(delta2);
            double alpha = Math.Log(0.5, (median - delta2_min) / delta2_max_min);
            //Calculate fuzzification coefficients
            m = new double[n];
            for (int i = 0; i < n; i++)
            {
                m[i] = ml + (mu - ml) * Math.Pow((delta2[i] - delta2_min) / delta2_max_min, alpha);
            }

            //Generate d
            List<int> xis = new List<int>(C);
            int[] zIndexs = new int[n];
            for (int i = 0; i < n; i++) zIndexs[i] = n;
            double[][] maxGroupsV = new double[n][];
            int maxGroupSize;
            xis.Clear();
            for (int i = n - 1; i > -1 && xis.Count < C; i--)
            {
                int xi = delta3[i].i;
                if (zIndexs[xi] != n) continue;
                xis.Add(xi);
                maxGroupSize = 1;
                zIndexs[xi] = xi;
                maxGroupsV[xi] = new double[dimension];
                Array.Copy(X[xi], maxGroupsV[xi], dimension);
                XPoint p;
                // TODO: Check the correctness of this code
                priorityQueues[xi].TryDequeue(out _, out _);
                while (priorityQueues[xi].TryDequeue(out p, out _))
                {
                    if (zIndexs[p.i] != n) continue;
                    zIndexs[p.i] = xi;
                    maxGroupSize++;
                    for (int j = 0; j < dimension; ++j)
                    {
                        maxGroupsV[xi][j] += X[p.i][j];
                    }

                }
                if (maxGroupSize == 1)
                {
                    maxGroupsV[xi][0] += epsilon;
                }
                else
                    for (int j = 0; j < dimension; ++j)
                    {
                        maxGroupsV[xi][j] /= maxGroupSize;
                    }
            }
            V = new double[C][];
            for (int i = 0; i < C; ++i)
            {
                V[i] = maxGroupsV[xis[i]];
            }
        }
        public enum CGMode
        {
            KMeanPlusPlus,
            MaxFuzzificationCoefficients,
            MinFuzzificationCoefficients,
            ChooseCFromXClustersRandomly,
            StupidRandom,
            StupidRamdomWithPartition,
            AverageInPartition,
            //Choose max fuzzification coefficients groups
            MaxFuzzificationCoefficientGroups,
            Custom
        }
        public CGMode ClustersGenerationMode { get; set; } = CGMode.MaxFuzzificationCoefficients;
        public MC_FCM_InitializeCluster InitializeCluster = null;
        public double[][] _GenerateFirstCClusters(IReadOnlyList<double[]> X, int C, double[] m, double epsilon)
        {
            switch (ClustersGenerationMode)
            {
                case CGMode.KMeanPlusPlus:
                    return ClustersGenerator.KMeanPlusPlus(X, C, epsilon);
                case CGMode.AverageInPartition:
                    return ClustersGenerator.AverageInPartition(X, C, epsilon);

                case CGMode.ChooseCFromXClustersRandomly:
                    return ClustersGenerator.ChooseCFromXClustersRandomly(X, C, epsilon);

                case CGMode.StupidRandom:
                    return ClustersGenerator.StupidRandom(X, C, epsilon);

                case CGMode.StupidRamdomWithPartition:
                    return ClustersGenerator.StupidRamdomWithPartition(X, C, epsilon);
                case CGMode.MinFuzzificationCoefficients:
                    return _GenerateFirstCClusters_MinFuzzificationCoefficients(X, C, m, epsilon);
                case CGMode.Custom:
                    return InitializeCluster(X, C, m, epsilon);
                default:
                    return _GenerateFirstCClusters_MaxFuzzificationCoefficients(X, C, m, epsilon);

            }
        }
        public static double[][] _GenerateFirstCClusters_MinFuzzificationCoefficients(IReadOnlyList<double[]> X, int C, double[] m, double epsilon)
        {
            PriorityQueue<double[], double> priorityQueue = new PriorityQueue<double[], double>(C);
            int i = 0;
            for (i = 0; i < X.Count; i++)
            {
                if (priorityQueue.Count < C) priorityQueue.Enqueue(X[i], m[i]);
                else priorityQueue.EnqueueDequeue(X[i], m[i]);
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
