using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonInteractive
{
    public class SVC : ModelCalling
    {
        private static readonly string programPath = ".\\Python\\multi_svm_model.py";
        public SVC() : base(programPath)
        {
        }
    }
}
