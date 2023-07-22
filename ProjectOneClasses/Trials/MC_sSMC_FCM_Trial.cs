using ProjectOneClasses.ResultTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Trials
{
    public class MC_sSMC_FCM_Trial
    {
        private IReadOnlyList<double[]> X;
        private int C;
        //IReadOnlyDictionary<int, int> Y;
        private int semiSupervisedDegree;
        private double epsilon;
        private int trialTimes;
        IReadOnlyList<int> expect;
        //double M;
        double alpha;
        public MC_sSMC_FCM_Trial([NotNull] IReadOnlyList<double[]> X, int C,
            //IReadOnlyDictionary<int, int> Y,
            int semiSupervisedDegree,
            IReadOnlyList<int> expect, int trialTimes/*, double M = 2*/, double alpha = 0.6, double epsilon = 0.0001)
        {
            if (trialTimes < 1) throw new ArgumentOutOfRangeException($"{nameof(trialTimes)} must be positive.");
            if (semiSupervisedDegree < 0 || semiSupervisedDegree > 100)
                throw new ArgumentOutOfRangeException($"{nameof(trialTimes)} must be between 0 and 100.");

            if (semiSupervisedDegree != 0)
            {
                int minimumTrialTimes = 100 / semiSupervisedDegree;
                if (100 % semiSupervisedDegree != 0)
                {
                    minimumTrialTimes++;
                }
                if (trialTimes < minimumTrialTimes)
                    throw new ArgumentOutOfRangeException($"{nameof(trialTimes)} must >= {minimumTrialTimes} (=ceiling(100 / semiSupervisedDegree).");

            }

            this.X = X;
            this.C = C;
            //this.Y = Y;
            this.epsilon = epsilon;
            this.trialTimes = trialTimes;
            this.expect = expect;
            //this.M = M;
            this.alpha = alpha;
            this.semiSupervisedDegree = semiSupervisedDegree;

        }
        public sSMC_FCM_Evaluation_Result Evaluation_Result { get; private set; }
        public int AverageL { get; private set; }
        public void _solve()
        {
            double sSWC = 0, dB = 0, pBM = 0, accuracy = 0, trainAccuracy = 0, rand = 0, jaccard = 0;
            int l = 0;
            int semiSupervisedCount = X.Count * semiSupervisedDegree / 100;
            var gen = new GenerateSemiSupervisedSetForTrial(C, semiSupervisedCount, expect);

            for (int i = 0; i < trialTimes; i++)
            {
                //Generate the semi-supervised set
                var Y = gen.Generate();
                //Solve the problem with sSMC-FCM
                var mc_fcm = new MC_sSMC_FCM(X, C, Y, alpha, epsilon);
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
