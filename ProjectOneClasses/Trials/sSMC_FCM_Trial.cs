using ProjectOneClasses.ResultTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Trials
{
    public class sSMC_FCM_Trial
    {
        private IReadOnlyList<double[]> X;
        private int C;
        IReadOnlyDictionary<int, int> Y;
        private double epsilon;
        private int trialTimes;
        IReadOnlyList<int> expect;
        double M; 
        double alpha;
        public sSMC_FCM_Trial([NotNull] IReadOnlyList<double[]> X, int C,
            IReadOnlyDictionary<int, int> Y,
            IReadOnlyList<int> expect, int trialTimes, double M = 2, double alpha = 0.6, double epsilon = 0.0001)
        {
            if (trialTimes < 1) throw new ArgumentOutOfRangeException($"{nameof(trialTimes)} must be positive.");
            this.X = X;
            this.C = C;
            this.Y = Y;
            this.epsilon = epsilon;
            this.trialTimes = trialTimes;
            this.expect = expect;
            this.M = M;
            this.alpha = alpha;
            
        }
        public sSMC_FCM_Evaluation_Result Evaluation_Result { get; private set; }
        public int AverageL { get; private set; }
        public void _solve()
        {
            double sSWC = 0, dB = 0, pBM = 0, accuracy = 0, trainAccuracy = 0, rand = 0, jaccard = 0;
            int l = 0;
            for (int i = 0; i < trialTimes; i++)
            {
                var mc_fcm = new sSMC_FCM(X, C, Y, M, alpha, epsilon);
                mc_fcm._solve();

                l += mc_fcm.Result.l;

                var er = new sSMC_FCM_Evaluation_Result(X, C, Y, expect, mc_fcm.Result);
                sSWC += er.SSWC;
                dB += er.DB;
                pBM += er.PBM;
                accuracy += er.Accuracy;
                trainAccuracy += er.TrainAccuracy;
                rand += er.Rand;
                jaccard += er.Jaccard;
            }

            sSWC /= trialTimes;
            dB /= trialTimes;
            pBM /= trialTimes;
            accuracy /= trialTimes;
            trainAccuracy /= trialTimes;
            rand /= trialTimes;
            jaccard /= trialTimes;
            l /= trialTimes;

            AverageL = l;
            Evaluation_Result = new sSMC_FCM_Evaluation_Result(sSWC, dB, pBM, accuracy, trainAccuracy, rand, jaccard);
        }
    }
}
