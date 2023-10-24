using ProjectOneClasses.ResultTypes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectOneClasses.Trials
{
    public class sSMC_FCM_Trial
    {
        private IReadOnlyList<double[]> X;
        private int C;
        //IReadOnlyDictionary<int, int> Y;
        private int semiSupervisedDegree;
        private double epsilon;
        private int trialTimes;
        IReadOnlyList<int> expect;
        double M; 
        double alpha;
        public sSMC_FCM_Trial([NotNull] IReadOnlyList<double[]> X, int C,
            //IReadOnlyDictionary<int, int> Y,
            int semiSupervisedDegree,
            IReadOnlyList<int> expect, int trialTimes, double M = 2, double alpha = 0.6, double epsilon = 0.0001)
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
            this.M = M;
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

            int processorCount = Environment.ProcessorCount / 2;

            int completedTrial = 0;

            var lockObj = new object();
            //var lockTrial = new object();

            void ThreadWork()
            {
                while(completedTrial < trialTimes)
                {
                    Interlocked.Increment(ref completedTrial);

                    //Generate the semi-supervised set
                    IReadOnlyDictionary<int, int> Y;

                    lock(gen)
                    {
                        Y = gen.Generate().ToImmutableDictionary();
                    }
                    //Solve the problem with sSMC-FCM
                    var mc_fcm = new sSMC_FCM(X, C, Y, M, alpha, epsilon);
                    mc_fcm._solve();

                    Interlocked.Add(ref l, mc_fcm.Result.l);

                    var er = new sSMC_FCM_Evaluation_Result(X, C, Y, expect, mc_fcm.Result);

                    
                    lock(lockObj)
                    {
                        sSWC += er.SSWC;
                        dB += er.DB;
                        pBM += er.PBM;
                        accuracy += er.Accuracy;
                        trainAccuracy += er.TrainAccuracy;
                        rand += er.Rand;
                        jaccard += er.Jaccard;
                    }   


                }
            }

            List<Thread> threads = new List<Thread>(processorCount);

            for (int i = 0; i < processorCount; i++)
            {
                var t = new Thread(ThreadWork);
                threads.Add(t);
                t.Start();
            }

            foreach (var t in threads)
            {
                t.Join();
            }

            sSWC /= completedTrial;
            dB /= completedTrial;
            pBM /= completedTrial;
            accuracy /= completedTrial;
            trainAccuracy /= completedTrial;
            rand /= completedTrial;
            jaccard /= completedTrial;
            l /= completedTrial;

            AverageL = l;
            Evaluation_Result = new sSMC_FCM_Evaluation_Result(sSWC, dB, pBM, accuracy, trainAccuracy, rand, jaccard);
        }
    }
}
