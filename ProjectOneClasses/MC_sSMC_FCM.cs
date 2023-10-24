using ProjectOneClasses.Cores;
using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static ProjectOneClasses.MC_FCM;

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
            this.X = X.ToImmutableArray();
            this.C = C;
            this.epsilon = epsilon;
            this.alpha = alpha;
            this.Y = Y.ToImmutableDictionary();
        }
        
        public MC_sSMC_FCM_Result Result { get; private set; } = null;
        int dimension;
        double[][] V;
        double[][] U;
        double[][] D;
        double[][] mu;
        double[] sum_mu_i_j;
        double[][] V_last;
        const double ml = 1.1, _mu = 4.1;
        public double[] m {  get; set; }
        public double[] M2 { get; set; }
        int n;

        public void PrepareVariables()
        {
            dimension = X[0].Length;
            n = X.Count;
            U = new double[n][];
            D = new double[n][];
            mu = new double[n][];//mu mean μ
            sum_mu_i_j = new double[n];

            for (int i = 0; i < n; i++)
            {
                U[i] = new double[C];

            }
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
        }

        public void GenerateFirstCClustersAndFuzzificationCoefficients()
        {
            //GenerateFuzzificationCoefficientsAndFirstCClusters_ExpandPointsNearestCluster(X, Y, C, ml, _mu, epsilon, out m, out V);
            double[] m;
            MC_sSMC_FCM_Core.GenerateFCAndFirstCClusters_MaxFuzzificationCoefficientGroups(X, Y, C, ml, _mu, epsilon, out m, out V);
            this.m = m;
            M2 = new double[n];
            Array.Copy(m, M2, m.Length);
        }

        public void UpdateU_NonSupervision()
        {
            MC_sSMC_FCM_Core.UpdateU_NonSupervision(X, Y, V, D, U, m);
        }
        public void GenerateM2()
        {
            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;
                M2[i] = MC_sSMC_FCM_Core.CalculateM2(Y, m[i], alpha, U[i][k], epsilon);
            }
        }
        public void UpdateU_Supervision()
        {
            MC_sSMC_FCM_Core.UpdateU_Supervision(X, Y, V, D, U, mu, m, M2, sum_mu_i_j, epsilon);
        }
        public void UpdateV()
        {
            MC_sSMC_FCM_Core.UpdateV(X, Y, V, V_last, U, m, M2);
        }
        public bool CheckConverging()
        {
            return MC_sSMC_FCM_Core.CheckConverging(V, V_last, epsilon);
        }
        public MC_sSMC_FCM_Result GetResult(int l)
        {
            Result = new MC_sSMC_FCM_Result(V, U, l);
            return Result;
        }

        public void _solve()
        {
            //Initialize input parameter
            PrepareVariables();
            //B0: Generate m + B1: Generate first clusters
            GenerateFirstCClustersAndFuzzificationCoefficients();

            //Main
            int l = 0;
            while (true)
            {
                l++;
                //B2: Update U[i][k]

                //--- without supervision
                UpdateU_NonSupervision();
                //--- with supervision at k th cluster

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
