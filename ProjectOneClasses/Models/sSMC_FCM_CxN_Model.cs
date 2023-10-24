using ProjectOneClasses.ResultTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Models
{
    public class sSMC_FCM_CxN_Model
    {
        public double[][] FuzzificationCoefficientsMatrix { get; private set; }
        public sSMC_FCM_Result Result { get; private set; }
        private sSMC_FCM sSMC_FCM;
        public void LearnFuzzificationCoefficientsMatrix([NotNull] IReadOnlyList<double[]> X, int C, IReadOnlyDictionary<int, int> Y, double M = 2, double alpha = 0.6, double epsilon = 0.0001)
        {
            if (Y == null)
            {
                throw new ArgumentNullException(nameof(Y));
            }
            if (X == null)
            {
                throw new ArgumentNullException(nameof(X));
            }
            if (Y.Count != X.Count)
            {
                throw new Exception("Supervision degree must be 100%");
            }

            sSMC_FCM = new sSMC_FCM(X, C, Y, M, alpha, epsilon);

            sSMC_FCM.PrepareVariables();
            //B1: Generate first clusters
            sSMC_FCM.GenerateFirstCClusters();
            //--- without supervision
            sSMC_FCM.UpdateU_NonSupervision();
            //Calculate M' (3.3)
            sSMC_FCM.GenerateM2();

            FuzzificationCoefficientsMatrix = new double[X.Count][];

            for (var i = 0; i < X.Count; i++)
            {
                FuzzificationCoefficientsMatrix[i] = new double[C];

                for (var j = 0; j < C; j++)
                {
                    FuzzificationCoefficientsMatrix[i][j] = M;
                }

                if(Y.TryGetValue(i, out var k))
                {
                    FuzzificationCoefficientsMatrix[i][k] = sSMC_FCM.M2;
                }
            }

        }

        public void Cluster()
        {
            //Main
            int l = 0;
            while (true)
            {
                l++;
                //B2: Update U[i][k]

                //--- without supervision
                sSMC_FCM.UpdateU_NonSupervision();

                //Calculate M' (3.3)
                if (l < 2)
                {
                    sSMC_FCM.GenerateM2();
                }

                //--- with supervision at k th cluster
                sSMC_FCM.UpdateU_Supervision();

                //B3: Update V
                sSMC_FCM.UpdateV();

                //B4: Check whether converging
                if (sSMC_FCM.CheckConverging()) break;
            }

            Result = sSMC_FCM.GetResult(l);
        }
    }
}
