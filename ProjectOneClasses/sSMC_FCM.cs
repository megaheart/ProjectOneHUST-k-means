using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ProjectOneClasses.Cores;
using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Utilities;

namespace ProjectOneClasses
{
    public class sSMC_FCM
    {
        private double M, alpha;
        private IReadOnlyList<double[]> X;
        private int C;
        private IReadOnlyDictionary<int, int> Y;
        private double epsilon;
        int maxInterator;
        public sSMC_FCM([NotNull] IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> Y,
            double M = 2, double alpha = 0.6, double epsilon = 0.0001, int maxInterator = 200)
        {
            if (C < 1) throw new Exception("C must be more than 0");
            if (C > X.Count) throw new Exception("C cannot be more than number of X");
            this.X = X.ToImmutableArray();
            this.C = C;
            this.epsilon = epsilon;
            this.M = M;
            this.alpha = alpha;
            this.Y = Y.ToImmutableDictionary();
            this.n = X.Count;
            this.maxInterator = maxInterator;
        }
        
        public sSMC_FCM_Result Result { get; private set; } = null;

        double[][] U;
        double[][] D;
        double[][] mu;
        double[][] V_last;
        int dimension;
        int n;
        double[][] V;
        public double M2 { get; private set; }

        public void PrepareVariables()
        {
            U = new double[n][];
            for (int i = 0; i < n; i++)
            {
                U[i] = new double[C];

            }
            D = new double[n][];
            mu = new double[n][];
            dimension = X[0].Length;
            V_last = new double[C][];
            for (int i = 0; i < C; i++)
            {
                V_last[i] = new double[dimension];
            }

            foreach (var i in Y.Keys)
            {
                D[i] = new double[C];
                mu[i] = new double[C];
            }

            M2 = M;
        }

        public void GenerateFirstCClusters()
        {
            V = sSMC_FCM_Core.GenerateFirstCClusters(X, Y, C, epsilon);
        }

        public void UpdateU_NonSupervision()
        {
            sSMC_FCM_Core.UpdateU_NonSupervision(X, Y, V, D, U, M);
        }
        public void GenerateM2()
        {
            //M2 = 4;
            M2 = Math.Min(sSMC_FCM_Core.CalculateM2(Y/*, n, C*/, M, alpha, U), 8);
            //M2 = CalculateM2(Y/*, n, C*/, M, alpha, U);
        }
        public void UpdateU_Supervision()
        {
            sSMC_FCM_Core.UpdateU_Supervision(X, Y, V, D, U, mu, M, M2, epsilon);
        }
        public void UpdateV()
        {
            sSMC_FCM_Core.UpdateV(X, Y, V, V_last, U, M, M2);
        }
        public bool CheckConverging()
        {
            return sSMC_FCM_Core.CheckConverging(V, V_last, epsilon);
        }
        public sSMC_FCM_Result GetResult(int l)
        {
            Result = new sSMC_FCM_Result(V, U, l, M, M2);
            return Result;
        }

        public void _solve()
        {
            //Initialize input parameter
            //double[] sum_mu_i_j = new double[n];

            PrepareVariables();

            //B1: Generate first clusters
            GenerateFirstCClusters();

            //Main
            int l = 0;
            while (l < maxInterator)
            {
                l++;
                //B2: Update U[i][k]

                //--- without supervision
                UpdateU_NonSupervision();

                //Calculate M' (3.3)
                if (l < 2)
                {
                    GenerateM2();
                }

                //--- with supervision at k th cluster
                UpdateU_Supervision();

                //B3: Update V
                UpdateV();

                //B4: Check whether converging
                if (CheckConverging()) break;
            }

            GetResult(l);
        }
    }
}
