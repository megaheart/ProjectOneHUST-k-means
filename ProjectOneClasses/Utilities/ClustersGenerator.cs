using System.Diagnostics.Metrics;

namespace ProjectOneClasses.Utilities
{
    public class ClustersGenerator
    {
        public static int ProporSelect(double[] vals, Random rnd)
        {
            // roulette wheel selection
            // on the fly technique
            // vals[] can't be all 0.0s
            int n = vals.Length;

            double sum = 0.0;
            for (int i = 0; i < n; ++i)
                sum += vals[i];

            double cumP = 0.0;  // cumulative prob
            double p = rnd.NextDouble();

            for (int i = 0; i < n; ++i)
            {
                cumP += (vals[i] / sum);
                if (cumP > p) return i;
            }
            return n - 1;  // last index
        }

        public static double[][] KMeanPlusPlus(IReadOnlyList<double[]> X, int C, double epsilon, double[][] mandatoryClusters = null)
        {
            //  k-means++ init using roulette wheel selection
            // clustering[] and means[][] exist
            int N = X.Count;
            int dim = X[0].Length;
            var rnd = Random.Shared;
            // select one data item index at random as 1st meaan

            int initK;

            double[][] V = new double[C][];

            if (mandatoryClusters == null || mandatoryClusters.Length == 0)
            {
                int idx = rnd.Next(0, N); // [0, N)

                for (int i = 0; i < C; i++) V[i] = new double[dim];

                for (int j = 0; j < dim; ++j)
                    V[0][j] = X[idx][j];

                initK = 1;
            }
            else
            {
                if (mandatoryClusters.Length > C) throw new ArgumentException(
                    $"{nameof(mandatoryClusters)} 's length must less then or equal to C.");
                initK = mandatoryClusters.Length;
                for (int i = 0; i < initK; i++)
                {
                    V[i] = new double[dim];
                    Array.Copy(mandatoryClusters[i], V[i], dim);
                }
                for (int i = initK; i < C; i++) V[i] = new double[dim];
            }
            

            for (int k = initK; k < C; ++k) // find each remaining mean
            {
                double[] dSquareds = new double[N]; // from each item to its closest mean

                for (int i = 0; i < N; ++i) // for each data item
                {
                    // compute distances from data[i] to each existing mean (to find closest)
                    double[] distances = new double[k]; // we currently have k means

                    for (int ki = 0; ki < k; ++ki)
                        distances[ki] = Calculation.EuclideanDistanseSquared(X[i], V[ki]);
                    dSquareds[i] = distances.Min();
                } // i

                // select an item far from its mean using roulette wheel
                // if an item has been used as a mean its distance will be 0
                // so it won't be selected

                int newMeanIdx = ProporSelect(dSquareds, rnd);

                Array.Copy(X[newMeanIdx], V[k], dim);
            } // k remaining means

            return V;
        }

        public static double[][] sSMC_FCM_KMeanPlusPlus(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, int C, double epsilon)
        {
            //  k-means++ init using roulette wheel selection
            // clustering[] and means[][] exist
            int N = X.Count;
            int dim = X[0].Length;
            var rnd = Random.Shared;

            double[][] V = new double[C][];

            for (int k = 0; k < C; k++) V[k] = new double[dim];

            int[] V_count = new int[C];

            int V_init_count = 0;

            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;

                if (V_count[k] == 0) V_init_count++;

                V_count[k]++;
                for (int j = 0; j < dim; j++)
                {
                    V[k][j] += X[i][j];
                }
            }

            int initK = 0;

            double[][] V_sorted = V;

            if (V_init_count > 0)
            {
                V_sorted = new double[C][];

                for (int k = 0; k < C; k++)
                {
                    if (V_count[k] > 0)
                    {
                        if (V_count[k] > 1)
                        {
                            for (int j = 0; j < dim; j++)
                            {
                                V[k][j] /= V_count[k];
                            }
                        }
                        V_sorted[initK++] = V[k];
                    }
                    else
                    {
                        V_sorted[V_init_count++] = V[k];
                    }
                }
            }

            if (initK == 0)
            {

                int idx = rnd.Next(0, N); // [0, N)

                for (int i = 0; i < C; i++) V_sorted[i] = new double[dim];

                Array.Copy(X[idx], V[0], dim);

                initK = 1;
            }

            for (int k = initK; k < C; ++k) // find each remaining mean
            {
                double[] dSquareds = new double[N]; // from each item to its closest mean

                for (int i = 0; i < N; ++i) // for each data item
                {
                    // compute distances from data[i] to each existing mean (to find closest)
                    double[] distances = new double[k]; // we currently have k means

                    for (int ki = 0; ki < k; ++ki)
                        distances[ki] = Calculation.EuclideanDistanseSquared(X[i], V_sorted[ki]);
                    dSquareds[i] = distances.Min();
                } // i

                // select an item far from its mean using roulette wheel
                // if an item has been used as a mean its distance will be 0
                // so it won't be selected

                int newMeanIdx = ProporSelect(dSquareds, rnd);

                Array.Copy(X[newMeanIdx], V_sorted[k], dim);
            } // k remaining means

            return V;
        }

        public static double[][] KMeanPlusPlus_HardSelect(IReadOnlyList<double[]> X, int C, double epsilon)
        {
            //  k-means++ init using roulette wheel selection
            // clustering[] and means[][] exist
            int N = X.Count;
            int dim = X[0].Length;
            var rnd = Random.Shared;
            // select one data item index at random as 1st meaan
            int idx = rnd.Next(0, N); // [0, N)

            double[][] V = new double[C][];

            for (int i = 0; i < C; i++) V[i] = new double[dim];

            for (int j = 0; j < dim; ++j)
                V[0][j] = X[idx][j];

            for (int k = 1; k < C; ++k) // find each remaining mean
            {
                //double[] dSquareds = new double[N]; // from each item to its closest mean

                var newMeanIdx = 0;
                double maxDSquared = 0;
                for (int i = 0; i < N; ++i) // for each data item
                {
                    // compute distances from data[i] to each existing mean (to find closest)
                    double[] distances = new double[k]; // we currently have k means

                    for (int ki = 0; ki < k; ++ki)
                        distances[ki] = Calculation.EuclideanDistanseSquared(X[i], V[ki]);
                    var dSquared = distances.Min();

                    if (dSquared > maxDSquared)
                    {
                        maxDSquared = dSquared;
                        newMeanIdx = i;
                    }

                } // i

                // select an item far from its mean using roulette wheel
                // if an item has been used as a mean its distance will be 0
                // so it won't be selected


                for (int j = 0; j < dim; ++j)
                    V[k][j] = X[newMeanIdx][j];
            } // k remaining means

            return V;
        }

        public static double[][] KMeanPlusPlus_FarthestFirst_HardSelect(IReadOnlyList<double[]> X, int C, double epsilon)
        {
            //  k-means++ init using roulette wheel selection
            // clustering[] and means[][] exist
            int N = X.Count;
            int dim = X[0].Length;
            var rnd = Random.Shared;
            // select one data item index at random as 1st meaan

            double[] mean = new double[dim];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < dim; ++j)
                    mean[j] += X[i][j];

            for (int j = 0; j < dim; ++j)
                mean[j] /= N;

            int idx = 0;
            double dSquaredToMean_max = Calculation.EuclideanDistanseSquared(X[0], mean);

            for (int i = 1; i < N; i++) {
                double dSquaredToMean = Calculation.EuclideanDistanseSquared(X[i], mean);

                if(dSquaredToMean > dSquaredToMean_max)
                {
                    dSquaredToMean_max = dSquaredToMean;
                    idx = i;
                }
            }

            double[][] V = new double[C][];

            for (int i = 0; i < C; i++) V[i] = new double[dim];

            for (int j = 0; j < dim; ++j)
                V[0][j] = X[idx][j];

            for (int k = 1; k < C; ++k) // find each remaining mean
            {
                //double[] dSquareds = new double[N]; // from each item to its closest mean

                var newMeanIdx = 0;
                double maxDSquared = 0;
                for (int i = 0; i < N; ++i) // for each data item
                {
                    // compute distances from data[i] to each existing mean (to find closest)
                    double[] distances = new double[k]; // we currently have k means

                    for (int ki = 0; ki < k; ++ki)
                        distances[ki] = Calculation.EuclideanDistanseSquared(X[i], V[ki]);
                    var dSquared = distances.Min();

                    if (dSquared > maxDSquared)
                    {
                        maxDSquared = dSquared;
                        newMeanIdx = i;
                    }

                } // i

                // select an item far from its mean using roulette wheel
                // if an item has been used as a mean its distance will be 0
                // so it won't be selected


                for (int j = 0; j < dim; ++j)
                    V[k][j] = X[newMeanIdx][j];
            } // k remaining means

            return V;
        }


        public static double[][] ChooseCFromXClustersRandomly(IReadOnlyList<double[]> X, int C, double epsilon)
        {

            bool[] isMatch = new bool[X.Count];
            double[][] V = new double[C][];
            double[] _x1;//, _x2;
            int l = X[0].Length, ii;
            Random r = new Random();
            for (int i = 0; i < C; i++)
            {
                _x1 = new double[l];
                ii = r.Next(0, X.Count - i);
                int j = 0;
                while (ii > -1)
                {
                    if (isMatch[j] == false)
                    {
                        if (ii == 0)
                        {
                            isMatch[j] = true;
                            Array.Copy(X[j], 0, _x1, 0, l);
                            _x1[0] += epsilon;
                            V[i] = _x1;
                            break;
                        }
                        ii--;
                    }
                    j++;
                }
            }
            return V;
        }
        public static double[][] StupidRandom(IReadOnlyList<double[]> X, int C, double epsilon)
        {
            Random r = new Random();
            int dimension = X[0].Length;
            double[][] V = new double[C][];
            double[] min = new double[dimension];
            double[] max = new double[dimension];
            for (int j = 0; j < dimension; j++)
            {
                min[j] = int.MaxValue;
                max[j] = int.MinValue;
            }
            for (int i = 0; i < X.Count; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    if (min[j] > X[i][j]) min[j] = X[i][j];
                    if (max[j] < X[i][j]) max[j] = X[i][j];
                }
            }
            for (int k = 0; k < C; k++)
            {
                V[k] = new double[dimension];
                for (int j = 0; j < dimension; j++) V[k][j] = min[j] + (max[j] - min[j]) * r.NextDouble();
            }
            return V;
        }
        public static double[][] StupidRamdomWithPartition(IReadOnlyList<double[]> X, int C, double epsilon)
        {
            Random r = new Random();
            int dimension = X[0].Length, partitionCount = X.Count / C, p = 0, to;
            double[][] V = new double[C][];
            double[] min = new double[dimension];
            double[] max = new double[dimension];

            for (int k = 1; k < C; k++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    min[j] = int.MaxValue;
                    max[j] = int.MinValue;
                }
                to = (p + 1) * partitionCount;
                for (int i = p * partitionCount; i < to; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        if (min[j] > X[i][j]) min[j] = X[i][j];
                        if (max[j] < X[i][j]) max[j] = X[i][j];
                    }
                }
                V[k] = new double[dimension];
                for (int j = 0; j < dimension; j++) V[k][j] = min[j] + (max[j] - min[j]) * r.NextDouble();
                ++p;
            }
            {
                int k = 0;
                for (int j = 0; j < dimension; j++)
                {
                    min[j] = int.MaxValue;
                    max[j] = int.MinValue;
                }
                for (int i = p * partitionCount; i < X.Count; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        if (min[j] > X[i][j]) min[j] = X[i][j];
                        if (max[j] < X[i][j]) max[j] = X[i][j];
                    }
                }
                V[k] = new double[dimension];
                for (int j = 0; j < dimension; j++) V[k][j] = min[j] + (max[j] - min[j]) * r.NextDouble();
            }
            return V;
        }
        public static double[][] AverageInPartition(IReadOnlyList<double[]> X, int C, double epsilon)
        {
            Random r = new Random();
            int dimension = X[0].Length, partitionCount = X.Count / C, p = 0, to;
            double[][] V = new double[C][];

            for (int k = 1; k < C; k++)
            {
                V[k] = new double[dimension];
                for (int j = 0; j < dimension; j++)
                {
                    V[k][j] = 0;
                }
                to = (p + 1) * partitionCount;
                for (int i = p * partitionCount; i < to; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        V[k][j] += X[i][j];
                    }
                }
                for (int j = 0; j < dimension; j++) V[k][j] /= partitionCount;
                ++p;
            }
            {
                int k = 0;
                V[k] = new double[dimension];
                for (int j = 0; j < dimension; j++)
                {
                    V[k][j] = 0;
                }
                for (int i = p * partitionCount; i < X.Count; i++)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        V[k][j] += X[i][j];
                    }
                }
                partitionCount = X.Count - p * partitionCount;
                for (int j = 0; j < dimension; j++) V[k][j] /= partitionCount;

            }
            return V;
        }

    }
}
