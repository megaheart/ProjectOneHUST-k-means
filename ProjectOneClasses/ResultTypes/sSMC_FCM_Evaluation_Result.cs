using ProjectOneClasses.ValidityCriterias.External;
using ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.ResultTypes
{
    public class sSMC_FCM_Evaluation_Result : Evaluation_Result
    {
        public double TrainAccuracy { get; private set; }
        public sSMC_FCM_Evaluation_Result(
            double sSWC, double dB, double pBM, double accuracy,
            double trainAccuracy, double rand, double jaccard
            ):base(sSWC, dB, pBM, accuracy, rand, jaccard)
        {
            TrainAccuracy = trainAccuracy;
        }
        public sSMC_FCM_Evaluation_Result(
            IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> Y,
            IReadOnlyList<int> expect, 
            IFCM_Result result/*, int? precision = 6*/):base(X, C, expect, result)
        {
            TrainAccuracy = new TrainAccuracy(X, Y, C, result).Index;
        }
        public new sSMC_FCM_Evaluation_Result Round(int precision)
        {
            double sSWC = Math.Round(SSWC, precision);
            double dB = Math.Round(DB, precision);
            double pBM = Math.Round(PBM, precision);
            double accuracy = Math.Round(Accuracy, precision);
            double trainAccuracy = Math.Round(TrainAccuracy, precision);
            double rand = Math.Round(Rand, precision);
            double jaccard = Math.Round(Jaccard, precision);

            return new sSMC_FCM_Evaluation_Result(sSWC, dB, pBM, accuracy, trainAccuracy, rand, jaccard);
        }
    }
}
