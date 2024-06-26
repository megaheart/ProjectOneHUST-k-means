﻿using ProjectOneClasses.Cores;
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
    public class MC_sSMC_FCM_CxN_Predictable_Model
    {
        private IReadOnlyList<double[]> X;
        private int C;
        private IReadOnlyDictionary<int, int> Y;
        private double epsilon, alpha;

        int dimension;
        double[][] V;
        const double ml = 1.1, _mu = 4.1;
        private double[] m;
        public double[] M2;
        int n;

        int maxInterator = 200;

        public void LearnFuzzificationCoefficientsMatrix([NotNull] IReadOnlyList<double[]> X, int C,
            [NotNull] IReadOnlyDictionary<int, int> Y,
            double alpha = 0.6, double epsilon = 0.0001)
        {
            if (Y == null)
            {
                throw new ArgumentNullException(nameof(Y));
            }
            if (X == null)
            {
                throw new ArgumentNullException(nameof(X));
            }

            if (C < 1) throw new Exception("C must be more than 0");
            if (C > X.Count) throw new Exception("C cannot be more than number of X");

            this.X = X.ToImmutableArray();
            this.C = C;
            this.epsilon = epsilon;
            this.alpha = alpha;
            this.Y = Y.ToImmutableDictionary();
            this.n = X.Count;
            this.dimension = X[0].Length;

            MC_sSMC_FCM_Core.GenerateFCAndFirstCClusters_MaxFuzzificationCoefficientGroups(X, Y, C, ml, _mu, epsilon, out m, out V);

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

            MC_sSMC_FCM_Core.UpdateU_NonSupervision(X, Y, V, D, U, m);

            M2 = new double[n];

            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;
                M2[i] = MC_sSMC_FCM_Core.CalculateM2(Y, m[i], alpha, U[i][k], epsilon);
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

            double[] m = new double[n];
            Array.Copy(this.m, m, this.m.Length);
            for(int i = this.m.Length; i < n; i++)
            {
                m[i] = ml;
            }

            double[][] D = new double[n][];
            var mu = new double[n][];
            foreach (var i in Y.Keys)
            {
                D[i] = new double[C];
                mu[i] = new double[C];
            }

            var V_last = new double[C][];
            for (int i = 0; i < C; i++)
            {
                V_last[i] = new double[dimension];

            }

            var sum_mu_i_j = new double[n];

            //Main
            int l = 0;
            while (l < maxInterator)
            {
                l++;
                //B2: Update U[i][k]

                //--- without supervision
                MC_sSMC_FCM_Core.UpdateU_NonSupervision(X, Y, V, D, U, m);
                //--- with supervision at k th cluster

                //--- with supervision at k th cluster
                MC_sSMC_FCM_Core.UpdateU_Supervision(X, Y, V, D, U, mu, m, M2, sum_mu_i_j, epsilon);

                //B3: Update V
                MC_sSMC_FCM_Core.UpdateV(X, Y, V, V_last, U, m, M2);

                //B4: Check whether converging
                if (MC_sSMC_FCM_Core.CheckConverging(V, V_last, epsilon)) break;
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
