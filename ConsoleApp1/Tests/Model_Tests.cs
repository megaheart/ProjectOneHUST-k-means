using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ProjectOneClasses;
using ProjectOneClasses.Models;
using ProjectOneClasses.Trials;
using ProjectOneClasses.Utilities;

namespace ConsoleApp1.Tests
{

    public class Model_Tests
    {
        public static void Test_DistanceNearestModel()
        {
            Data.LoadAllExampleData();

            foreach (var dataInfo in Data.datas)
            {
                var data = dataInfo.aCIDb;
                var X = data.X;
                var expect = data.expect;

                var indexes = Enumerable.Range(0, X.Count).ToList();
                indexes.Shuffle();

                // Config k - cross validation
                int pieTrainCount = 4;
                int pieTestCount = 1;
                int pieCount = pieTrainCount + pieTestCount;
                var pies = new IEnumerable<int>[pieCount];
                int pieSize = X.Count / pieCount;

                for (int i = 0; i < pieCount - 1; i++)
                {
                    var pie = indexes.Skip(i * pieSize).Take(pieSize);
                    pies[i] = pie;
                }
                pies[pieCount - 1] = indexes.Skip((pieCount - 1) * pieSize);

                double meanAccuracy = 0;
                int combinationCount = 0;

                foreach (var combination in Calculation.GenerateAllCombinations<IEnumerable<int>>(pies, Math.Min(pieTrainCount, pieTestCount)))
                {
                    IEnumerable<int> trainIndexes = null;
                    IEnumerable<int> testIndexes = null;
                    int testIndexesIndex = 0;

                    for (int i = 0; i < pieCount; i++)
                    {
                        if (testIndexesIndex < combination.Count && combination[testIndexesIndex] == pies[i])
                        {
                            testIndexes = testIndexes == null ? pies[i] : testIndexes.Concat(pies[i]);
                            testIndexesIndex++;
                        }
                        else
                        {
                            trainIndexes = trainIndexes == null ? pies[i] : trainIndexes.Concat(pies[i]);
                        }
                    }

                    var trainData = trainIndexes.Select(x => X[x]).ToList();
                    var trainLabels = trainIndexes.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => expect[x.Item2]);
                    var testData = testIndexes.Select(x => X[x]).ToList();
                    var testLabels = testIndexes.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => expect[x.Item2]);

                    var model = new DistanceNearestModel();
                    model.Train(trainData, data.C, trainLabels);
                    var predictions = testData.Select(x => model.Predict(x)).ToList();
                    var predictionsDict = predictions.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => x.Item2);
                    var accuracy = Calculation.Accuracy(testLabels, predictionsDict);

                    meanAccuracy += accuracy;
                    combinationCount++;
                }

                Console.WriteLine($"{dataInfo.Name.PadRight(22)} = {Math.Round(meanAccuracy / combinationCount * 100, 6)}%({data.C})");


            }
        }
        public static void Test_sSMC_FCM_CxN_Model()
        {
            Data.LoadAllExampleData();
            var dataInfo = Data.datas[0];
            var data = dataInfo.aCIDb;
            sSMC_FCM_CxN_Model model = new sSMC_FCM_CxN_Model();
            var label = data.expect.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => x.Item2);

            model.LearnFuzzificationCoefficientsMatrix(data.X, data.C, label);

            model.Cluster();
        }

        public static void Test_MC_sSMC_FCM_CxN_Model()
        {
            Data.LoadAllExampleData();
            var dataInfo = Data.datas[0];
            var data = dataInfo.aCIDb;
            MC_sSMC_FCM_CxN_Model model = new MC_sSMC_FCM_CxN_Model();
            var label = data.expect.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => x.Item2);

            model.LearnFuzzificationCoefficientsMatrix(data.X, data.C, label);

            model.Cluster();
        }
    }
}
