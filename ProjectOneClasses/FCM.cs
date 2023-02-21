using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses
{
    public class FCM : MC_FCM
    {
        public FCM([NotNull] IReadOnlyList<double[]> X, int C, double m, double epsilon = 0.0001) : base(X, C, epsilon)
        {
            this.m = new double[X.Count];
            for(int i = 0; i < X.Count; i++)
            {
                this.m[i] = m;
            }
        }
    }
}
