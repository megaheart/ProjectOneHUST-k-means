using ProjectOneClasses.Models;
using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Utilities;
using PythonInteractive;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Tests
{
    public record Model_PredictUntrainedClass_Test_Data(List<double[]> trainData, List<int> trainLabels, int C, List<double[]> testTrainedClassData, List<int> testTrainedClassLabels, List<double[]> testUntrainedClassData, List<int> testUntrainedClassLabels)
    {
        public IReadOnlyList<double[]> TrainData { get => trainData; }
        public IReadOnlyList<int> TrainLabels { get => trainLabels; }
        public int C { get => C; }
        public IReadOnlyList<double[]> TestTrainedClassData { get => testTrainedClassData; }
        public IReadOnlyList<int> TestTrainedClassLabels { get => testTrainedClassLabels; }
        public IReadOnlyList<double[]> TestUntrainedClassData { get => testUntrainedClassData; }
        public IReadOnlyList<int> TestUntrainedClassLabels { get => testUntrainedClassLabels; }

    }
    public class Model_PredictUntrainedClass_Test
    {
        public static List<List<int>> GroupDataIndexesByLabel(IReadOnlyList<double[]> data, IReadOnlyList<int> labels, int C)
        {
            var dataByLabel = new List<List<int>>(C);
            for (int i = 0; i < C; i++)
            {
                dataByLabel.Add(new List<int>());
            }
            for (int i = 0; i < data.Count; i++)
            {
                dataByLabel[labels[i]].Add(i);
            }
            return dataByLabel;
        }

        public static Model_PredictUntrainedClass_Test_Data SplitData(IEnumerable<int> trainClasses, IEnumerable<int> testClasses, List<List<int>> dataIndexesByLabel, IReadOnlyList<double[]> data, IReadOnlyList<int> labels, double testPercent = 0.2)
        {
            int C = dataIndexesByLabel.Count;

            var trainClassDataIndexes = new List<int>();
            foreach (var trainClass in trainClasses)
            {
                trainClassDataIndexes.AddRange(dataIndexesByLabel[trainClass]);
            }
            trainClassDataIndexes.Shuffle();

            var untrainedClassDataIndexes = new List<int>();
            foreach (var testClass in testClasses)
            {
                untrainedClassDataIndexes.AddRange(dataIndexesByLabel[testClass]);
            }
            untrainedClassDataIndexes.Shuffle();

            var trainCount = (int)(trainClassDataIndexes.Count * (1 - testPercent));
            var untrainedCount = Math.Min(trainClassDataIndexes.Count - trainCount, untrainedClassDataIndexes.Count);

            var trainData = trainClassDataIndexes.Take(trainCount).Select(i => data[i]).ToList();
            var trainLabels = trainClassDataIndexes.Take(trainCount).Select(i => labels[i]).ToList();

            var testTrainedClassData = trainClassDataIndexes.Skip(trainCount).Select(i => data[i]).ToList();
            var testTrainedClassLabels = trainClassDataIndexes.Skip(trainCount).Select(i => labels[i]).ToList();

            var testUntrainedClassData = untrainedClassDataIndexes.Take(untrainedCount).Select(i => data[i]).ToList();
            var testUntrainedClassLabels = untrainedClassDataIndexes.Take(untrainedCount).Select(i => labels[i]).ToList();

            return new Model_PredictUntrainedClass_Test_Data(trainData, trainLabels, C, testTrainedClassData, testTrainedClassLabels, testUntrainedClassData, testUntrainedClassLabels);
        }

        public static void TestModel<T>(TrainFunction<T> train, PredictFunction<T> predict)
            where T : class
        {
            Data.LoadAllExampleData();

            PrintTable pt = new PrintTable(new ColumnStyle[] {
                new ColumnStyle(){Width=20},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=10},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=10},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=10},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=10},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=10},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=10},
            });

            pt.PrintLine();
            pt.PrintRow("", "SSWC", "DB", "PBM", "ACC", "RAND", "JACC");
            pt.PrintLine();

            foreach (var dataInfo in Data.datas)
            {
                var data = dataInfo.aCIDb;
                var X = data.X;
                var expect = data.expect;

                var dataIndexesByLabel = GroupDataIndexesByLabel(X, expect, data.C);

                var testPercent = 0.2;

                Evaluation_Result meanTrainedResult = new Evaluation_Result(0, 0, 0, 0, 0, 0);
                Evaluation_Result meanUntrainedResult = new Evaluation_Result(0, 0, 0, 0, 0, 0);

                int combinationCount = 0;

                var pies = Enumerable.Range(0, data.C).ToList();
                var k = (int)Math.Ceiling(data.C * 2.0 / 3);

                if(k < 2) continue;

                foreach (var combination in Calculation.GenerateAllCombinations(pies, k))
                {
                    var trainedClasses = combination;
                    var untrainedClasses = new List<int>();
                    int trainedClassIndex = 0;

                    for (int i = 0; i < data.C; i++)
                    {
                        if (trainedClassIndex < combination.Count && combination[trainedClassIndex] == i)
                        {
                            trainedClassIndex++;
                        }
                        else
                        {
                            untrainedClasses.Add(i);
                        }
                    }

                    var test_data = SplitData(trainedClasses, untrainedClasses, dataIndexesByLabel, X, expect, testPercent);


                    //Train
                    T model = null;
                    try
                    {
                        model = train(test_data.TrainData, data.C, test_data.TrainLabels);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                        Debug.WriteLine(e);
                        continue;
                    }
                    //Test
                    var trainedDataPredictions = predict(model, test_data.TestTrainedClassData);
                    var untrainedDataPredictions = predict(model, test_data.TestUntrainedClassData);

                    if (model is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    else if (model is IAsyncDisposable asyncDisposable)
                    {
                        asyncDisposable.DisposeAsync();
                    }

                    var trainedResult = new Evaluation_Result(test_data.TestTrainedClassData, data.C, test_data.TestTrainedClassLabels, trainedDataPredictions);
                    var untrainedResult = new Evaluation_Result(test_data.TestUntrainedClassData, data.C, test_data.TestUntrainedClassLabels, untrainedDataPredictions);

                    meanTrainedResult += trainedResult;
                    meanUntrainedResult += untrainedResult;
                    combinationCount++;
                }

                meanTrainedResult /= combinationCount;
                meanUntrainedResult /= combinationCount;

                pt.PrintRow(dataInfo.Name, "", "", "", "", "", "");
                pt.PrintRow("   Trained Class", meanTrainedResult.SSWC, meanTrainedResult.DB, meanTrainedResult.PBM, meanTrainedResult.Accuracy, meanTrainedResult.Rand, meanTrainedResult.Jaccard);
                pt.PrintRow("   Untrained Class", meanUntrainedResult.SSWC, meanUntrainedResult.DB, meanUntrainedResult.PBM, meanUntrainedResult.Accuracy, meanUntrainedResult.Rand, meanUntrainedResult.Jaccard);

                pt.PrintLine();
            }
        }

        public static void Test_DistanceNearestModel()
        {
            TrainFunction<DistanceNearestModel> train = (trainData, C, trainLabels) =>
            {
                var model = new DistanceNearestModel();

                var trainLabelMap = trainLabels.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => x.Item2);

                model.Train(trainData.ToList(), C, trainLabelMap);

                return model;
            };

            PredictFunction<DistanceNearestModel> predict = (model, testData) =>
            {
                var predictions = testData.Select(x => model.Predict(x)).ToList();
                return predictions;
            };

            Console.WriteLine("DistanceNearestModel Benchmark\n");

            TestModel(train, predict);
        }

        public static void Test_MultiPredict_sSMC_FCM_CxN_Predictable_Model()
        {
            TrainFunction<sSMC_FCM_CxN_Predictable_Model> train = (trainData, C, trainLabels) =>
            {
                var model = new sSMC_FCM_CxN_Predictable_Model();

                var trainLabelMap = trainLabels.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => x.Item2);

                model.LearnFuzzificationCoefficientsMatrix(trainData, C, trainLabelMap);

                return model;
            };

            PredictFunction<sSMC_FCM_CxN_Predictable_Model> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData);
                return predictions;
            };

            Console.WriteLine("sSMC_FCM_CxN_Predictable_Model MultiPredict Benchmark\n");

            TestModel(train, predict);
        }

        public static void Test_MultiPredict_MC_sSMC_FCM_CxN_Predictable_Model()
        {
            TrainFunction<MC_sSMC_FCM_CxN_Predictable_Model> train = (trainData, C, trainLabels) =>
            {
                var model = new MC_sSMC_FCM_CxN_Predictable_Model();

                var trainLabelMap = trainLabels.Select((x, i) => new Tuple<int, int>(i, x)).ToImmutableDictionary(x => x.Item1, x => x.Item2);

                model.LearnFuzzificationCoefficientsMatrix(trainData, C, trainLabelMap);

                return model;
            };

            PredictFunction<MC_sSMC_FCM_CxN_Predictable_Model> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData);
                return predictions;
            };

            Console.WriteLine("MC_sSMC_FCM_CxN_Predictable_Model MultiPredict Benchmark\n");

            TestModel(train, predict);
        }

        public static void Test_ANN_Classifier_Model()
        {
            TrainFunction<ANNClassifier> train = (trainData, C, trainLabels) =>
            {
                var model = new ANNClassifier();
                model.Train(trainData, C, trainLabels).Wait();
                return model;
            };

            PredictFunction<ANNClassifier> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData).Result;
                return predictions;
            };

            Console.WriteLine("ANN Classifier Model Benchmark\n");

            TestModel(train, predict);
        }

        public static void Test_Decision_Tree_Model()
        {
            TrainFunction<DecisionTreeModel> train = (trainData, C, trainLabels) =>
            {
                var model = new DecisionTreeModel();
                model.Train(trainData, C, trainLabels).Wait();
                return model;
            };

            PredictFunction<DecisionTreeModel> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData).Result;
                return predictions;
            };

            Console.WriteLine("Decision Tree Model Benchmark\n");

            TestModel(train, predict);
        }
        public static void Test_Kmeans_Model()
        {
            TrainFunction<KmeansModel> train = (trainData, C, trainLabels) =>
            {
                var model = new KmeansModel();
                model.Train(trainData, C, trainLabels).Wait();
                return model;
            };

            PredictFunction<KmeansModel> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData).Result;
                return predictions;
            };

            Console.WriteLine("Kmeans Model Benchmark\n");

            TestModel(train, predict);
        }
        public static void Test_KNN_Model()
        {
            TrainFunction<KNNModel> train = (trainData, C, trainLabels) =>
            {
                var model = new KNNModel();
                model.Train(trainData, C, trainLabels).Wait();
                return model;
            };

            PredictFunction<KNNModel> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData).Result;
                return predictions;
            };

            Console.WriteLine("KNN Model Benchmark\n");

            TestModel(train, predict);
        }

        public static void Test_Random_Forest_Model()
        {
            TrainFunction<RandomForestModel> train = (trainData, C, trainLabels) =>
            {
                var model = new RandomForestModel();
                model.Train(trainData, C, trainLabels).Wait();
                return model;
            };

            PredictFunction<RandomForestModel> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData).Result;
                return predictions;
            };

            Console.WriteLine("Random Forest Model Benchmark\n");

            TestModel(train, predict);
        }
    }
}
