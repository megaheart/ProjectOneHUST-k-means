using ProjectOneClasses.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOneClasses.Models
{
    public class DistanceNearestModel
    {
        private List<double[]> trainedClusters;
        private List<int> trainedLabels;

        public void Train(List<double[]> data, int C, IReadOnlyDictionary<int, int> labels)
        {
            var labelsSet = new HashSet<int>(labels.Values);
            trainedLabels = labelsSet.ToList();
            for (int i = 0; i < labelsSet.Count; i++)
            {
                if (trainedLabels[i] < 0)
                {
                    throw new Exception("Labels must not be negative");
                }
            }
            var labelsMapping = trainedLabels.ToImmutableDictionary(x => x, x => trainedLabels.IndexOf(x));
            var labelsTransformed = labels.ToImmutableDictionary(x => x.Key, x => labelsMapping[x.Value]);
            for (int i = labelsSet.Count; i < C; i++)
            {
                int label = -(i - labelsSet.Count + 1);
                trainedLabels.Add(label);
            }
            var fcm = new sSMC_FCM(data, C, labelsTransformed);
            fcm._solve();
            trainedClusters = fcm.Result.V.ToList();
        }

        public int Predict(double[] example)
        {
            var distances = trainedClusters.Select(x => Calculation.EuclideanDistanseSquared(x, example)).ToList();
            var minDistance = distances.Min();
            var minDistanceIndex = distances.IndexOf(minDistance);
            return trainedLabels[minDistanceIndex];
        }
    }
}
