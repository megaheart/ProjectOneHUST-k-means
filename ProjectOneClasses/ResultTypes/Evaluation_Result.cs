using ProjectOneClasses.ValidityCriterias.External;
using ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ResultTypes
{
    public class Evaluation_Result
    {
        public double SSWC { get; private set; }
        public double DB { get; private set; }
        public double PBM { get; private set; }
        public double Accuracy { get; private set; }
        public double Rand { get; private set; }
        public double Jaccard { get; private set; }

        public Evaluation_Result(double sSWC, double dB, double pBM, double accuracy, double rand, double jaccard)
        {
            SSWC = sSWC;
            DB = dB;
            PBM = pBM;
            Accuracy = accuracy;
            Rand = rand;
            Jaccard = jaccard;
        }

        public Evaluation_Result(IReadOnlyList<double[]> X, int C, IReadOnlyList<int> expect, IFCM_Result result/*, int? precision = 6*/)
        {
            SSWC = new SSWC(X, C, result).Index;
            DB = new DB(X, C, result).Index;
            PBM = new PBM(X, C, result).Index;
            Accuracy = new Accuracy(X, C, expect, result).Index;
            Rand = new Rand(X, C, expect, result).Index;
            Jaccard = new Jaccard(X, C, expect, result).Index;

            //if(precision.HasValue) {
                
            //}
        }

        public Evaluation_Result Round(int precision)
        {
            double sSWC = Math.Round(SSWC, precision); 
            double dB = Math.Round(DB, precision); 
            double pBM = Math.Round(PBM, precision);
            double accuracy = Math.Round(Accuracy, precision);
            double rand = Math.Round(Rand, precision);
            double jaccard = Math.Round(Jaccard, precision);

            return new Evaluation_Result(sSWC, dB, pBM, accuracy, rand, jaccard);
        }
    }
}
