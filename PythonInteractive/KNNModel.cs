using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonInteractive
{
    public class KNNModel : ModelCalling
    {
        private static readonly string programPath = ".\\Python\\knn_model.py";
        public KNNModel() : base(programPath)
        {
        }
    }
}
