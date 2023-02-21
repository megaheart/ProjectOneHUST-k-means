using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses
{
    public class SimpleOneVariableEquationSolver
    {
        private Func<double, double> function;
        
        private double start;
        private double end;
        private double epsilon_X;
        //private double epsilon_Y;
        private double root;
        public double Root { get { return root; } }
        public bool SolveSuccessfully { get { return double.IsNaN(root); } }
        public SimpleOneVariableEquationSolver([NotNull] Func<double, double> function, double epsilon_X/*, double epsilon_Y*/, double start, double end)
        {
            this.function = function;
            if (epsilon_X <= 0 /*|| epsilon_Y <= 0*/) throw new Exception("epsilon can't less than or equal 0.");
            this.epsilon_X = epsilon_X;
            //this.epsilon_Y = epsilon_Y;
            this.start = start;
            this.end = end;
        }
        public SimpleOneVariableEquationSolver([NotNull] Func<double, double> function, double epsilon_X/*, double epsilon_Y*/)
        {
            this.function = function;
            if (epsilon_X <= 0 /*|| epsilon_Y <= 0*/) throw new Exception("epsilon can't less than or equal 0.");
            this.epsilon_X = epsilon_X;
            //this.epsilon_Y = epsilon_Y;
            this.start = double.MinValue;
            this.end = double.MaxValue;
        }
        public void Solve()
        {
            double l = start;
            double r = end;
            double mid, lf, rf, mf;
            lf = function(l);
            rf = function(r);
            while(double.IsNaN(lf))
            {
                l /= 2;
                lf = function(l);
            }
            while (double.IsNaN(rf))
            {
                r /= 2;
                rf = function(r);
            }
            if (lf * rf > 0)
            {
                root = double.NaN;
                return;
            }
            else if(lf * rf == 0)
            {
                if(Math.Abs(lf) <= double.Epsilon)
                {
                    root = l;
                    return;
                }
                else if (Math.Abs(rf) <= double.Epsilon)
                {
                    root = r;
                    return;
                }
                else
                {
                    root = double.NaN;
                    return;
                }
            }
            do
            {
                mid = (r + l) / 2;
                lf = function(l);
                mf = function(mid);
                rf = function(r);
                if (Math.Abs(mf) == 0)
                {
                    root = mid;
                    return;
                }
                else if (mf * lf > 0)
                {
                    l = mid;
                }
                else if(mf * rf > 0)
                {
                    r = mid;
                }
            }
            while (r - l > epsilon_X);

            root = (l + r) / 2;

        }
    }
}
