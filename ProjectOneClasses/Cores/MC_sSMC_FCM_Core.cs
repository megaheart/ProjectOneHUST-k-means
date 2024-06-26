﻿using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectOneClasses.MC_FCM;

namespace ProjectOneClasses.Cores
{
    public class MC_sSMC_FCM_Core
    {
        public static void UpdateU_NonSupervision(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, IReadOnlyList<double[]> V, double[][] D, double[][] U, double[] m)
        {
            int n = X.Count;
            int C = V.Count;

            for (int i = 0; i < n; i++)
            {
                double[] delta = new double[C];
                double a = 1 / (m[i] - 1), b, c = 0;

                int hasZeroDis = -1;

                for (int k = 0; k < C; k++)
                {
                    if (D[i] != null)
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
        public static void UpdateU_Supervision(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, IReadOnlyList<double[]> V, double[][] D, double[][] U, double[][] mu, double[] m, double[] M2,double[] sum_mu_i_j, double epsilon)
        {
            int n = X.Count;
            int C = V.Count;

            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;

                sum_mu_i_j[i] = 0;
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


            foreach (var y in Y)
            {
                int i = y.Key, k = y.Value;
                double m2 = M2[i];

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
        }
        public static void UpdateV(IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, IReadOnlyList<double[]> V, IReadOnlyList<double[]> V_last, double[][] U, double[] m, double[] M2)
        {
            int n = X.Count;
            int C = V.Count;
            int dimension = X[0].Length;

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

       
        public static double CalculateM2(IReadOnlyDictionary<int, int> Y, double M,
            double alpha, double Uik, double epsilon, double m2Max = 8)
        {
            double right = M * Math.Pow((1 - alpha) / ((1 / Uik) - 1), M - 1);
            //double M2 = Math.Max(M, -1 / Math.Log(alpha)); // Start value of M2
            //double left = M2 * Math.Pow(alpha, M2 - 1);
            //while (left > right)
            //{
            //    M2 += 1;
            //    left = M2 * Math.Pow(alpha, M2 - 1);
            //}
            //return M2;

            double M2_l = Math.Max(M, -1 / Math.Log(alpha)); // Start value of M2
            double left_l = M2_l * Math.Pow(alpha, M2_l - 1);
            if (left_l <= right)
            {
                return M2_l;
            }
            //double M2_incr = 1;
            //double M2_r = M2_l + M2_incr;
            double M2_r = m2Max;
            double left_r = M2_r * Math.Pow(alpha, M2_r - 1);

            if (left_r > right) return M2_r;

            //while (left_r > right)
            //{
            //    M2_incr *= 2;
            //    M2_r = M2_l + M2_incr;
            //    left_r = M2_r * Math.Pow(alpha, M2_r - 1);
            //}

            while ((M2_r - M2_l) > epsilon)
            {
                double M2_mid = (M2_l + M2_r) / 2;
                double left_mid = M2_mid * Math.Pow(alpha, M2_mid - 1);
                if (left_mid > right)
                {
                    M2_l = M2_mid;
                }
                else
                {
                    M2_r = M2_mid;
                }
            }
            return M2_r;
        }
        public static void GenerateFCAndFirstCClusters_MaxFuzzificationCoefficientGroups(
            IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, int C, double ml, double mu, double epsilon, out double[] m, out double[][] V)
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
                    var d = Math.Sqrt(Calculation.EuclideanDistanseSquared(X[i], X[j]));
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

            //create cluster for Supervised point
            {
                int[] V_count = new int[C];
                int[] supervisedGroupIndexs = new int[C];
                for (int k = 0; k < C; k++)
                {
                    supervisedGroupIndexs[k] = n;
                }
                foreach (var y in Y)
                {
                    int i = y.Key, k = y.Value;
                    double[] supervisedGroup;
                    if (supervisedGroupIndexs[k] == n)
                    {
                        supervisedGroup = new double[dimension];
                        Array.Copy(X[i], supervisedGroup, dimension);
                        xis.Add(i);
                        zIndexs[i] = i;
                        V_count[k] = 1;
                        supervisedGroupIndexs[k] = i;
                        maxGroupsV[i] = supervisedGroup;
                    }
                    else
                    {
                        supervisedGroup = maxGroupsV[supervisedGroupIndexs[k]];
                        zIndexs[i] = supervisedGroupIndexs[k];
                        V_count[k]++;
                        for (int j = 0; j < dimension; j++)
                        {
                            supervisedGroup[j] += X[i][j];
                        }
                    }

                }
                for (int k = 0; k < C; k++)
                {
                    int i = supervisedGroupIndexs[k];
                    if (i != n)
                    {
                        double[] supervisedGroup = maxGroupsV[i];
                        if (V_count[k] == 1)
                        {
                            for (int j = 0; j < dimension; j++)
                            {
                                supervisedGroup[j] += 2 * epsilon;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < dimension; j++)
                            {
                                supervisedGroup[j] /= V_count[k];
                            }
                        }
                    }
                }
            }

            //create cluster for non-Supervised point
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

            if(xis.Count != C)
            {
                throw new Exception("Not enough point to create C clusters");
            }

            V = new double[C][];
            for (int i = 0; i < C; ++i)
            {
                V[i] = maxGroupsV[xis[i]];
            }
        }

        public static void GenerateFuzzificationCoefficientsAndFirstCClusters_ExpandPointsNearestCluster(
            IReadOnlyList<double[]> X, IReadOnlyDictionary<int, int> Y, int C, double ml, double mu, double epsilon, out double[] m, out double[][] V)
        {
            int n = X.Count;
            int nn = n / C;
            int nn_80percent = nn * 80 / 100;
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
                    var d = Math.Sqrt(Calculation.EuclideanDistanseSquared(X[i], X[j]));
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

            //create cluster for Supervised point
            {
                int[] V_count = new int[C];
                int[] supervisedGroupIndexs = new int[C];
                for (int k = 0; k < C; k++)
                {
                    supervisedGroupIndexs[k] = n;
                }
                foreach (var y in Y)
                {
                    int i = y.Key, k = y.Value;
                    double[] supervisedGroup;
                    if (supervisedGroupIndexs[k] == n)
                    {
                        supervisedGroup = new double[dimension];
                        Array.Copy(X[i], supervisedGroup, dimension);
                        xis.Add(i);
                        zIndexs[i] = i;
                        V_count[k] = 1;
                        supervisedGroupIndexs[k] = i;
                        maxGroupsV[i] = supervisedGroup;
                    }
                    else
                    {
                        supervisedGroup = maxGroupsV[supervisedGroupIndexs[k]];
                        zIndexs[i] = supervisedGroupIndexs[k];
                        V_count[k]++;
                        for (int j = 0; j < dimension; j++)
                        {
                            supervisedGroup[j] += X[i][j];
                        }
                    }

                }

                PriorityQueue<XPoint, double>[] nearestClusters = new PriorityQueue<XPoint, double>[C];//Not optimized memory
                void pushToNearestCluster(int i, int xi, double d)
                {
                    //double dd = d;
                    if (nearestClusters[i].Count < nn_80percent) priorityQueues[i].Enqueue(new XPoint(xi, d), -d);
                    else
                    {
                        XPoint x_d_max = nearestClusters[i].EnqueueDequeue(new XPoint(xi, d), -d);
                        //dd -= x_d_max.d;
                    }
                    //return dd;
                }

                for (int k = 0; k < C; k++)
                {
                    int i = supervisedGroupIndexs[k];
                    if (i != n)
                    {
                        double[] supervisedGroup = maxGroupsV[i];
                        if (V_count[k] > 1)
                        {
                            for (int j = 0; j < dimension; j++)
                            {
                                supervisedGroup[j] /= V_count[k];
                            }

                        }
                        //else //if (V_count[k] == 1)
                        //{
                        //    for (int j = 0; j < dimension; j++)
                        //    {
                        //        supervisedGroup[j] += 2 * epsilon;
                        //    }
                        //}
                        if (V_count[k] < nn_80percent)
                            nearestClusters[k] = new PriorityQueue<XPoint, double>();
                    }
                }

                foreach (var y in Y)
                {
                    int i = y.Key, k = y.Value;
                    int zIdx = zIndexs[i];
                    double[] supervisedGroup = maxGroupsV[zIdx];

                    if (V_count[k] < nn_80percent)
                        pushToNearestCluster(k, i, Calculation.EuclideanDistanseSquared(supervisedGroup, X[i]));

                }

                for (int k = 0; k < C; k++)
                {
                    int i = supervisedGroupIndexs[k];
                    if (i != n)
                    {
                        double[] supervisedGroup = maxGroupsV[i];
                        if (V_count[k] < nn_80percent)
                        {
                            if (V_count[k] > 1)
                            {
                                for (int j = 0; j < dimension; j++)
                                {
                                    supervisedGroup[j] *= V_count[k];
                                }
                            }
                            Stack<int> nearests = new Stack<int>();
                            while (nearestClusters[k].TryDequeue(out var p, out _))
                            {
                                nearests.Push(p.i);
                            }
                            while (V_count[k] < nn_80percent && nearests.TryPop(out var nearest))
                            {
                                while (priorityQueues[nearest].TryDequeue(out var p, out _))
                                {
                                    if (zIndexs[p.i] != n) continue;
                                    zIndexs[p.i] = i;
                                    V_count[k]++;
                                    for (int j = 0; j < dimension; ++j)
                                    {
                                        maxGroupsV[i][j] += X[p.i][j];
                                    }

                                }
                            }

                            if (V_count[k] > 1)
                            {
                                for (int j = 0; j < dimension; j++)
                                {
                                    supervisedGroup[j] /= V_count[k];
                                }

                            }
                        }


                        if (V_count[k] == 1)
                        {
                            for (int j = 0; j < dimension; j++)
                            {
                                supervisedGroup[j] += 2 * epsilon;
                            }
                        }
                    }
                }

            }

            //create cluster for non-Supervised point
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
    }
}
