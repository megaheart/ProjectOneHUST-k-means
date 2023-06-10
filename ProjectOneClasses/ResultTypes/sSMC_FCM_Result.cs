using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ResultTypes
{
    public class sSMC_FCM_Result : IFCM_Result
    {
        public IReadOnlyList<double[]> V { get; private set; }
        public IReadOnlyList<IReadOnlyList<double>> U { get; private set; }
        public int l { get; private set; }

        public double M { get; private set; }

        public double M2 { get; private set; }

        public sSMC_FCM_Result([NotNull] double[][] _V, [NotNull] double[][] _U, int l, double M, double M2)
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
}
