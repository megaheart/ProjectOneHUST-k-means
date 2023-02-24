using ProjectOneClasses.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneHUST
{
    public class ACIData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ACIDbDeserializer aCIDb { get; set; } = null;
    }
}
