using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ProjectOneClasses;
using ProjectOneClasses.Models;
using ProjectOneClasses.ResultTypes;
using ProjectOneClasses.Trials;
using ProjectOneClasses.Utilities;
using ProjectOneClasses.ValidityCriterias.External;
using PythonInteractive;

namespace ConsoleApp1.Tests
{
    /// <summary>
    /// Train model with training data and labels, and return a model
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    /// <param name="trainData">Training data</param>
    /// <param name="C">Number of cluster</param>
    /// <param name="trainLabels">Training labels</param>
    /// <returns>a model after training</returns>
    public delegate T TrainFunction<T>(IReadOnlyList<double[]> trainData, int C, IReadOnlyList<int> trainLabels);
    /// <summary>
    /// Predict labels of test data with a model
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    /// <param name="model">Model after training and need testing</param>
    /// <param name="testData">Test data</param>
    /// <returns>Predicted labels of test data</returns>
    public delegate IReadOnlyList<int> PredictFunction<T>(T model, IReadOnlyList<double[]> testData);

    public class Model_Tests
    {
        public static void TestModel<T>(TrainFunction<T> train, PredictFunction<T> predict)
            where T:class
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

                Evaluation_Result meanResult = new Evaluation_Result(0, 0, 0, 0, 0, 0);

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
                    var trainLabels = trainIndexes.Select(x => expect[x]).ToList();
                    

                    var testData = testIndexes.Select(x => X[x]).ToList();
                    var testLabels = testIndexes.Select(x=> expect[x]).ToList();


                    //Train
                    T model = null;
                    try
                    {
                        model = train(trainData, data.C, trainLabels);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                        Debug.WriteLine(e);
                        continue;
                    }
                    //Test
                    var predictions = predict(model, testData);

                    if (model is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    else if (model is IAsyncDisposable asyncDisposable)
                    {
                        asyncDisposable.DisposeAsync();
                    }

                    var r = new Evaluation_Result(testData, data.C, testLabels, predictions);

                    meanResult += r;
                    combinationCount++;
                }

                meanResult /= combinationCount;

                pt.PrintRow(dataInfo.Name, meanResult.SSWC, meanResult.DB, meanResult.PBM, meanResult.Accuracy, meanResult.Rand, meanResult.Jaccard);

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

        public static void Test_sSMC_FCM_CxN_Predictable_Model()
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
                var predictions = testData.Select(x => model.Predict(new[] { x})).SelectMany(x => x).ToArray();
                return predictions;
            };

            Console.WriteLine("sSMC_FCM_CxN_Predictable_Model Benchmark\n");

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

        public static void Test_MC_sSMC_FCM_CxN_Predictable_Model()
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
                var predictions = testData.Select(x => model.Predict(new[] { x })).SelectMany(x => x).ToArray();
                return predictions;
            };

            Console.WriteLine("MC_sSMC_FCM_CxN_Predictable_Model Benchmark\n");

            TestModel(train, predict);
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

        public static void Test_SVC_Model()
        {
            TrainFunction<SVC> train = (trainData, C, trainLabels) =>
            {
                var model = new SVC();
                model.Train(trainData, C, trainLabels).Wait();
                return model;
            };

            PredictFunction<SVC> predict = (model, testData) =>
            {
                var predictions = model.Predict(testData).Result;
                return predictions;
            };

            Console.WriteLine("Support Vector Classification Model Benchmark\n");

            TestModel(train, predict);
        }
    }
}
