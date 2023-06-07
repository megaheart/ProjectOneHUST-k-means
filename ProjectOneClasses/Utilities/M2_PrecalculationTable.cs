using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Utilities
{
    public class M2_PrecalculationTable
    {
        public static readonly M2_PrecalculationTable Instance = new M2_PrecalculationTable(2, 0.6, 0.0001);

        private double[] _m2;
        public M2_PrecalculationTable(in double M, in double U_expect, in double epsilon)
        {
            InitFloatM2Table(1000, M, U_expect, epsilon);
            //InitIntegerM2Table(1000, M, U_expect);
        }

        public double this[double u]
        {
            get
            {
                int _u = (int)Math.Min(u * 1000, _m2.Length - 1);

                return _m2[_u];
            }
        }
        private void InitFloatM2Table(in int precision, in double M, in double U_expect, in double epsilon)
        {
            _m2 = new double[precision];
            double previousM2 = M;

            for (int i = precision - 1; i > -1; i--)
            {
                double u = i * 1.0 / precision;
                previousM2 = _m2[i] = CalculateM2Float(M, previousM2, U_expect, u, epsilon);
            }
        }

        double CalculateM2Float(double M, double startM2, double alpha, double Uik, double epsilon)
        {
            double right = M * Math.Pow((1 - alpha) / (1 / Uik - 1), M - 1);
            double M2_l = Math.Max(startM2, -1 / Math.Log(alpha)); // Start value of M2
            double left_l = M2_l * Math.Pow(alpha, M2_l - 1);
            if (left_l <= right)
            {
                return M2_l;
            }
            double M2_incr = 1;
            double M2_r = M2_l + M2_incr;
            double left_r = M2_r * Math.Pow(alpha, M2_r - 1);
            while (left_r > right)
            {
                M2_incr *= 2;
                M2_r = M2_l + M2_incr;
                left_r = M2_r * Math.Pow(alpha, M2_r - 1);
            }

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
        private void InitIntegerM2Table(in int precision, in double M, in double U_expect)
        {
            _m2 = new double[precision];
            double previousM2 = M;

            for (int i = precision - 1; i > -1; i--)
            {
                double u = i * 1.0 / precision;
                previousM2 = _m2[i] = CalculateM2Integer(M, previousM2, U_expect, u);
            }
        }

        double CalculateM2Integer(double M, double startM2, double alpha, double Uik)
        {
            double right = M * Math.Pow((1 - alpha) / (1 / Uik - 1), M - 1);
            double M2 = Math.Max(startM2, -1 / Math.Log(alpha)); // Start value of M2
            double left = M2 * Math.Pow(alpha, M2 - 1);

            while (left > right)
            {
                M2 += 1;
                left = M2 * Math.Pow(alpha, M2 - 1);
            }
            return M2;
        }
    }
}
