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
        }

        public Evaluation_Result(IReadOnlyList<double[]> X, int C, IReadOnlyList<int> expect, IReadOnlyList<int> predicts/*, int? precision = 6*/)
        {
            SSWC = new SSWC(X, C, predicts).Index;
            DB = new DB(X, C, predicts).Index;
            PBM = new PBM(X, C, predicts).Index;
            Accuracy = new Accuracy(X, C, expect, predicts).Index;
            Rand = new Rand(X, C, expect, predicts).Index;
            Jaccard = new Jaccard(X, C, expect, predicts).Index;
        }

        public Evaluation_Result(IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> expect, IReadOnlyDictionary<int, int> predicts/*, int? precision = 6*/)
        {
            SSWC = new SSWC(X, C, predicts).Index;
            DB = new DB(X, C, predicts).Index;
            PBM = new PBM(X, C, predicts).Index;
            Accuracy = new Accuracy(X, C, expect, predicts).Index;
            Rand = new Rand(X, C, expect, predicts).Index;
            Jaccard = new Jaccard(X, C, expect, predicts).Index;
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

        public static Evaluation_Result operator +(Evaluation_Result a, Evaluation_Result b)
        {
            return new Evaluation_Result(a.SSWC + b.SSWC, a.DB + b.DB, a.PBM + b.PBM, a.Accuracy + b.Accuracy, a.Rand + b.Rand, a.Jaccard + b.Jaccard);
        }

        public static Evaluation_Result operator /(Evaluation_Result a, int b)
        {
            return new Evaluation_Result(a.SSWC / b, a.DB / b, a.PBM / b, a.Accuracy / b, a.Rand / b, a.Jaccard / b);
        }
    }
}
