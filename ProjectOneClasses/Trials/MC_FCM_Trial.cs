using ProjectOneClasses.ResultTypes;
using System.Diagnostics.CodeAnalysis;

namespace ProjectOneClasses.Trials
{
    public class MC_FCM_Trial
    {
        private IReadOnlyList<double[]> X;
        private int C;
        private double epsilon;
        private int trialTimes;
        IReadOnlyList<int> expect;
        MC_FCM.CGMode clustersGenerationMode;
        MC_FCM_InitializeCluster mC_FCM_InitializeCluster;
        public MC_FCM_Trial([NotNull] IReadOnlyList<double[]> X, int C,
            MC_FCM.CGMode clustersGenerationMode,
            MC_FCM_InitializeCluster mC_FCM_InitializeCluster,
            IReadOnlyList<int> expect, int trialTimes, double epsilon = 0.0001)
        {
            if (trialTimes < 1) throw new ArgumentOutOfRangeException($"{nameof(trialTimes)} must be positive.");
            this.X = X;
            this.C = C;
            this.epsilon = epsilon;
            this.trialTimes = trialTimes;
            this.expect = expect;
            this.clustersGenerationMode = clustersGenerationMode;
            this.mC_FCM_InitializeCluster = mC_FCM_InitializeCluster;
        }
        public Evaluation_Result Evaluation_Result { get; private set; }
        public int AverageL { get; private set; }
        public void _solve()
        {
            double sSWC = 0, dB = 0, pBM = 0, accuracy = 0, rand = 0, jaccard = 0;
            int l = 0;
            for (int i = 0; i < trialTimes; i++)
            {
                var mc_fcm = new MC_FCM(X, C, epsilon);
                mc_fcm.ClustersGenerationMode = clustersGenerationMode;
                mc_fcm.InitializeCluster = mC_FCM_InitializeCluster;
                mc_fcm._solve();

                l += mc_fcm.Result.l;

                var er = new Evaluation_Result(X, C, expect, mc_fcm.Result);
                sSWC += er.SSWC;
                dB += er.DB;
                pBM += er.PBM;
                accuracy += er.Accuracy;
                rand += er.Rand;
                jaccard += er.Jaccard;
            }

            sSWC /= trialTimes;
            dB /= trialTimes;
            pBM /= trialTimes;
            accuracy /= trialTimes;
            rand /= trialTimes;
            jaccard /= trialTimes;
            l /= trialTimes;

            AverageL = l;
            Evaluation_Result = new Evaluation_Result(sSWC, dB, pBM, accuracy, rand, jaccard);
        }
    }
}
