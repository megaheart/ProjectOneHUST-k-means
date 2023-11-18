using ProjectOneClasses.Cores;
using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Models
{
    public class sSMC_FCM_CxN_Predictable_Model
    {
        private double M, alpha;
        private IReadOnlyList<double[]> X;
        private int C;
        private IReadOnlyDictionary<int, int> Y;
        private double epsilon;
        int dimension;
        int n;
        double[][] V;
        public double M2;
        public double[][] FuzzificationCoefficientsMatrix { get; private set; }
        int maxInterator = 200;

        public void LearnFuzzificationCoefficientsMatrix([NotNull] IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> Y, double M = 2, double alpha = 0.6, double epsilon = 0.0001)
        {
            if (Y == null)
            {
                throw new ArgumentNullException(nameof(Y));
            }
            if (X == null)
            {
                throw new ArgumentNullException(nameof(X));
            }
            if (Y.Count != X.Count)
            {
                throw new Exception("Supervision degree must be 100%");
            }

            this.X = X.ToImmutableArray();
            this.C = C;
            this.epsilon = epsilon;
            this.M = M;
            this.alpha = alpha;
            this.Y = Y.ToImmutableDictionary();
            this.n = X.Count;
            this.dimension = X[0].Length;

            var U = new double[n][];
            for (int i = 0; i < n; i++)
            {
                U[i] = new double[C];

            }

            var D = new double[n][];
            foreach (var i in Y.Keys)
            {
                D[i] = new double[C];
            }

            //B1: Generate first clusters
            V = sSMC_FCM_Core.GenerateFirstCClusters(X, Y, C, epsilon);

            //B2: Update U[i][k]
            //--- without supervision
            sSMC_FCM_Core.UpdateU_NonSupervision(X, Y, V, D, U, M);

            //Calculate M' (3.3)
            M2 = Math.Min(sSMC_FCM_Core.CalculateM2(Y/*, n, C*/, M, alpha, U), 8);

            FuzzificationCoefficientsMatrix = new double[X.Count][];

            for (var i = 0; i < X.Count; i++)
            {
                FuzzificationCoefficientsMatrix[i] = new double[C];

                for (var j = 0; j < C; j++)
                {
                    FuzzificationCoefficientsMatrix[i][j] = M;
                }

                if (Y.TryGetValue(i, out var k))
                {
                    FuzzificationCoefficientsMatrix[i][k] = M2;
                }
            }

        }

        public IReadOnlyList<int> Predict(IReadOnlyList<double[]> examples)
        {
            int n = this.n + examples.Count;

            double[][] V = new double[C][];
            for (int i = 0; i < C; i++)
            {
                V[i] = new double[dimension];
                Array.Copy(this.V[i], V[i], dimension);
            }

            double[][] X = new double[n][];
            Array.Copy(this.X.ToArray(), X, this.X.Count);
            Array.Copy(examples.ToArray(), 0, X, this.X.Count, examples.Count);

            double[][] U = new double[n][];
            for (int i = 0; i < n; i++)
            {
                U[i] = new double[C];
            }

            double[][] D = new double[n][];
            for (int i = 0; i < n; i++)
            {
                D[i] = new double[C];
            }

            double[][] mu = new double[n][];
            for (int i = 0; i < n; i++)
            {
                mu[i] = new double[C];
            }

            var V_last = new double[C][];
            for (int i = 0; i < C; i++)
            {
                V_last[i] = new double[dimension];
            }

            int l = 0;
            while (l < maxInterator)
            {
                l++;
                //B2: Update U[i][k]

                //--- without supervision
                sSMC_FCM_Core.UpdateU_NonSupervision(X, Y, V, D, U, M);

                //--- with supervision at k th cluster
                sSMC_FCM_Core.UpdateU_Supervision(X, Y, V, D, U, mu, M, M2, epsilon);

                //B3: Update V
                sSMC_FCM_Core.UpdateV(X, Y, V, V_last, U, M, M2);

                //B4: Check whether converging
                if (sSMC_FCM_Core.CheckConverging(V, V_last, epsilon)) break;
            }

            var predictedLabels = new int[examples.Count];

            for (int i = this.n, j = 0; i < n; i++, j++)
            {
                var k = U[i].IndexOfMax();
                predictedLabels[j] = k;
            }

            return predictedLabels;
        }
    }
}
