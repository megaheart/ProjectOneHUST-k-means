using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonInteractive
{
    public class KmeansModel : ModelCalling
    {
        private static readonly string programPath = ".\\Python\\kmean_model.py";
        public KmeansModel() : base(programPath)
        {
        }
    }
}
