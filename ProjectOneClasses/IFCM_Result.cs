using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses
{
    public interface IFCM_Result
    {
        public IReadOnlyList<double[]> V { get; }
        public IReadOnlyList<IReadOnlyList<double>> U { get; }
    }
}
