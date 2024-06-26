﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ResultTypes
{
    public class FakeFCM_Result : IFCM_Result
    {
        private double[][] _V = null;
        private double[][] _U = null;
        public IReadOnlyList<double[]> V { get => _V; }

        public IReadOnlyList<IReadOnlyList<double>> U { get => _U; }
        public int l { get => Random.Shared.Next(1, 200); }
        public FakeFCM_Result(IReadOnlyList<double[]> X, IReadOnlyList<int> expect, int C)
        {
            int dimension = X[0].Length;
            _V = new double[C][];
            int[] V_count = new int[C];
            _U = new double[expect.Count][];
            for (int i = 0; i < C; i++)
            {
                _V[i] = new double[dimension];
            }
            for (int i = 0; i < expect.Count; i++)
            {
                _U[i] = new double[C];
                for (int j = 0; j < dimension; j++)
                {
                    _V[expect[i]][j] += X[i][j];
                }
                V_count[expect[i]]++;
                _U[i][expect[i]] = 1;
            }

            for (int i = 0; i < C; i++)
            {
                if (V_count[i] > 1)
                {
                    for (int j = 0; j < dimension; j++)
                    {
                        _V[i][j] /= V_count[i];
                    }
                }

            }
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
}
