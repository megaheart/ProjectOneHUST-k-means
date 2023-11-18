using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonInteractive
{
    public class RandomForestModel : ModelCalling
    {
        private static readonly string programPath = ".\\Python\\random_forest_model.py";
        public RandomForestModel() : base(programPath)
        {
        }
    }
}
