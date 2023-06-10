using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ResultTypes
{
    public interface IFCM_Result
    {
        public IReadOnlyList<double[]> V { get; }
        public IReadOnlyList<IReadOnlyList<double>> U { get; }
        public int l { get; }
        void printToConsole();
        //public double M { get; }
        //public double M2 { get; }
    }
}
