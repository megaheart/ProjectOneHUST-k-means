using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonInteractive
{
    public class ANNClassifier : ModelCalling
    {
        private static readonly string programPath = ".\\Python\\ann_classifier_model.py";
        public ANNClassifier() : base(programPath)
        {
        }
    }
}
