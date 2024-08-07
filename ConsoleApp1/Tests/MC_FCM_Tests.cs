﻿using ProjectOneClasses;
using ProjectOneClasses.Trials;
using ProjectOneClasses.Utilities;
using ProjectOneClasses.ValidityCriterias.External;
using ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike;

namespace ConsoleApp1.Tests
{
    internal static class MC_FCM_Tests
    {
        public static void FCM_And_MC_FCM_BenchMark()
        {
            Console.Clear();
            //Console.SetWindowSize(1900, 980);
            foreach (var dataInfo in Data.datas)
                dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);
            PrintTable pt = new PrintTable(new ColumnStyle[] {
            new ColumnStyle(){Width=20},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 16},
            });

            pt.PrintLine();
            pt.PrintRow("", "FCM (Rand)", "MC-FCM (Rand)", "MC-FCM (Max FC)", "MC-FCM (Min FC)", "MC-FCM (Max FCG)");
            pt.PrintLine();
            foreach (var dataInfo in Data.datas)
            {
                var data = dataInfo.aCIDb;
                var fcm = new FCM(data.X, data.C, 2);
                fcm.ClustersGenerationMode = MC_FCM.CGMode.KMeanPlusPlus;
                fcm._solve();
                var mc_fcm = new MC_FCM(data.X, data.C);
                mc_fcm.ClustersGenerationMode = MC_FCM.CGMode.KMeanPlusPlus;
                mc_fcm._solve();
                var mc_fcm2 = new MC_FCM(data.X, data.C);
                mc_fcm2.ClustersGenerationMode = MC_FCM.CGMode.MaxFuzzificationCoefficients;
                mc_fcm2._solve();
                var mc_fcm3 = new MC_FCM(data.X, data.C);
                mc_fcm3.ClustersGenerationMode = MC_FCM.CGMode.MinFuzzificationCoefficients;
                mc_fcm3._solve();
                var mc_fcm4 = new MC_FCM(data.X, data.C);
                mc_fcm4.ClustersGenerationMode = MC_FCM.CGMode.MaxFuzzificationCoefficientGroups;
                mc_fcm4._solve();
                pt.PrintRow(dataInfo.Name, fcm.Result.l, mc_fcm.Result.l, mc_fcm2.Result.l, mc_fcm3.Result.l, mc_fcm4.Result.l);

                var fcm_sswc = new SSWC(data.X, data.C, fcm.Result).Index;
                var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
                var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
                var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
                var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
                var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;

                var mc_fcm_sswc = new SSWC(data.X, data.C, mc_fcm.Result).Index;
                var mc_fcm_db = new DB(data.X, data.C, mc_fcm.Result).Index;
                var mc_fcm_pbm = new PBM(data.X, data.C, mc_fcm.Result).Index;
                var mc_fcm_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm.Result).Index;
                var mc_fcm_rand = new Rand(data.X, data.C, data.expect, mc_fcm.Result).Index;
                var mc_fcm_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm.Result).Index;

                var mc_fcm2_sswc = new SSWC(data.X, data.C, mc_fcm2.Result).Index;
                var mc_fcm2_db = new DB(data.X, data.C, mc_fcm2.Result).Index;
                var mc_fcm2_pbm = new PBM(data.X, data.C, mc_fcm2.Result).Index;
                var mc_fcm2_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm2.Result).Index;
                var mc_fcm2_rand = new Rand(data.X, data.C, data.expect, mc_fcm2.Result).Index;
                var mc_fcm2_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm2.Result).Index;

                var mc_fcm3_sswc = new SSWC(data.X, data.C, mc_fcm3.Result).Index;
                var mc_fcm3_db = new DB(data.X, data.C, mc_fcm3.Result).Index;
                var mc_fcm3_pbm = new PBM(data.X, data.C, mc_fcm3.Result).Index;
                var mc_fcm3_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm3.Result).Index;
                var mc_fcm3_rand = new Rand(data.X, data.C, data.expect, mc_fcm3.Result).Index;
                var mc_fcm3_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm3.Result).Index;

                var mc_fcm4_sswc = new SSWC(data.X, data.C, mc_fcm4.Result).Index;
                var mc_fcm4_db = new DB(data.X, data.C, mc_fcm4.Result).Index;
                var mc_fcm4_pbm = new PBM(data.X, data.C, mc_fcm4.Result).Index;
                var mc_fcm4_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm4.Result).Index;
                var mc_fcm4_rand = new Rand(data.X, data.C, data.expect, mc_fcm4.Result).Index;
                var mc_fcm4_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm4.Result).Index;

                var max_sswc = new[] { fcm_sswc, mc_fcm_sswc, mc_fcm2_sswc, mc_fcm3_sswc, mc_fcm4_sswc }.Max();
                var min_db = new[] { fcm_db, mc_fcm_db, mc_fcm2_db, mc_fcm3_db, mc_fcm4_db }.Min();
                var max_pbm = new[] { fcm_pbm, mc_fcm_pbm, mc_fcm2_pbm, mc_fcm3_pbm, mc_fcm4_pbm }.Max();
                var max_accuracy = new[] { fcm_accuracy, mc_fcm_accuracy, mc_fcm2_accuracy, mc_fcm3_accuracy, mc_fcm4_accuracy }.Max();
                var max_rand = new[] { fcm_rand, mc_fcm_rand, mc_fcm2_rand, mc_fcm3_rand, mc_fcm4_rand }.Max();
                var max_jaccard = new[] { fcm_jaccard, mc_fcm_jaccard, mc_fcm2_jaccard, mc_fcm3_jaccard, mc_fcm4_jaccard }.Max();

                pt.PrintRow("    SSWC",
                    new ColumnContent()
                    {
                        Content = fcm_sswc,
                        Color = (fcm_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm_sswc,
                        Color = (mc_fcm_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm2_sswc,
                        Color = (mc_fcm2_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_sswc,
                        Color = (mc_fcm3_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_sswc,
                        Color = (mc_fcm4_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    DB",
                    new ColumnContent() { Content = fcm_db, Color = (fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White },
                    new ColumnContent() { Content = mc_fcm_db, Color = (mc_fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White },
                    new ColumnContent() { Content = mc_fcm2_db, Color = (mc_fcm2_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White },
                    new ColumnContent() { Content = mc_fcm3_db, Color = (mc_fcm3_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White },
                    new ColumnContent() { Content = mc_fcm4_db, Color = (mc_fcm4_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White });
                pt.PrintRow("    PBM",
                    new ColumnContent()
                    {
                        Content = fcm_pbm,
                        Color = (fcm_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm_pbm,
                        Color = (mc_fcm_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm2_pbm,
                        Color = (mc_fcm2_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_pbm,
                        Color = (mc_fcm3_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_pbm,
                        Color = (mc_fcm4_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    ACC",
                    new ColumnContent()
                    {
                        Content = fcm_accuracy,
                        Color = (fcm_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm_accuracy,
                        Color = (mc_fcm_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm2_accuracy,
                        Color = (mc_fcm2_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_accuracy,
                        Color = (mc_fcm3_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_accuracy,
                        Color = (mc_fcm4_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    RAND",
                    new ColumnContent()
                    {
                        Content = fcm_rand,
                        Color = (fcm_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm_rand,
                        Color = (mc_fcm_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm2_rand,
                        Color = (mc_fcm2_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_rand,
                        Color = (mc_fcm3_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_rand,
                        Color = (mc_fcm4_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    JACC",
                    new ColumnContent()
                    {
                        Content = fcm_jaccard,
                        Color = (fcm_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm_jaccard,
                        Color = (mc_fcm_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm2_jaccard,
                        Color = (mc_fcm2_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_jaccard,
                        Color = (mc_fcm3_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_jaccard,
                        Color = (mc_fcm4_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    });

                pt.PrintRow("", "", "", "", "", "");
            }
            pt.PrintLine();
            Console.WriteLine();
        }
        public static void MC_FCM_Validate()
        {
            Console.Clear();
            //Console.SetWindowSize(1900, 980);
            Data.LoadAllExampleData();
            PrintTable pt = new PrintTable(new ColumnStyle[] {
            new ColumnStyle(){Width = 24},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=12},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=12},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=12},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 12},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 12},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 12},
        });

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("FCM");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            pt.PrintLine();
            pt.PrintRow("", "SSWC", "PBM", "DB", "Accuracy", "Jaccard", "Rand");
            pt.PrintLine();
            foreach (var dataInfo in Data.datas)
            {
                var data = dataInfo.aCIDb;
                var fcm = new FCM(data.X, data.C, 2);
                fcm._solve();
                var sswc = Math.Round(new SSWC(data.X, data.C, fcm.Result).Index, 6);
                var db = Math.Round(new DB(data.X, data.C, fcm.Result).Index, 6);
                var pbm = Math.Round(new PBM(data.X, data.C, fcm.Result).Index, 6);
                var accuracy = Math.Round(new Accuracy(data.X, data.C, data.expect, fcm.Result).Index, 6);
                var rand = Math.Round(new Rand(data.X, data.C, data.expect, fcm.Result).Index, 6);
                var jaccard = Math.Round(new Jaccard(data.X, data.C, data.expect, fcm.Result).Index, 6);
                //int n = data.X.Count;
                pt.PrintRow(dataInfo.Name, sswc, pbm, db, accuracy, jaccard, rand);

            }
            pt.PrintLine();
            Console.WriteLine();


            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write("MC_FCM");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            pt.PrintLine();
            pt.PrintRow("", "SSWC", "PBM", "DB", "Accuracy", "Jaccard", "Rand");
            pt.PrintLine();
            foreach (var dataInfo in Data.datas)
            {
                var data = dataInfo.aCIDb;
                var mc_fcm = new MC_FCM(data.X, data.C);
                mc_fcm._solve();
                var sswc = Math.Round(new SSWC(data.X, data.C, mc_fcm.Result).Index, 6);
                var db = Math.Round(new DB(data.X, data.C, mc_fcm.Result).Index, 6);
                var pbm = Math.Round(new PBM(data.X, data.C, mc_fcm.Result).Index, 6);
                var accuracy = Math.Round(new Accuracy(data.X, data.C, data.expect, mc_fcm.Result).Index, 6);
                var rand = Math.Round(new Rand(data.X, data.C, data.expect, mc_fcm.Result).Index, 6);
                var jaccard = Math.Round(new Jaccard(data.X, data.C, data.expect, mc_fcm.Result).Index, 6);
                //int n = data.X.Count;
                pt.PrintRow(dataInfo.Name, sswc, pbm, db, accuracy, jaccard, rand);

            }
            pt.PrintLine();
        }
        public static void MC_FCM_DiffGenClusters_Benchmark()
        {
            //Console.SetWindowSize(1900, 980);
            Data.LoadAllExampleData();
            PrintTable pt = new PrintTable(new ColumnStyle[] {
                new ColumnStyle(){Width=20},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                new ColumnStyle(){Align=ColumnAlign.Right, Width = 16},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                //new ColumnStyle(){Align=ColumnAlign.Right, Width = 17},
            });
            Console.WriteLine("MC-FCM Different Cluster Generation Benchmark");
            Console.WriteLine("");
            pt.PrintLine();
            pt.PrintRow("", "Choose K", "KMeanPP HS", "KMeanPP FF HS", "Max FC Group", "KMeanPP");
            pt.PrintLine();
            foreach (var dataInfo in Data.datas)
            {
                //var dataInfo = datas[2];
                var data = dataInfo.aCIDb;


                var fcm = new MC_FCM_Trial(data.X, data.C, MC_FCM.CGMode.ChooseCFromXClustersRandomly,
                    null, data.expect, 100);
                fcm._solve();

                //var mc_fcm = new MC_FCM_Trial(data.X, data.C, MC_FCM.CGMode.StupidRandom,
                //    null, data.expect, 100);
                //mc_fcm._solve();

                var mc_fcm2 = new MC_FCM_Trial(data.X, data.C, MC_FCM.CGMode.Custom,
                    (X, C, m, e) =>
                    {
                        return ClustersGenerator.KMeanPlusPlus_HardSelect(X, C, e);
                    }, data.expect, 100);
                mc_fcm2._solve();

                var mc_fcm3 = new MC_FCM_Trial(data.X, data.C, MC_FCM.CGMode.Custom,
                    (X, C, m, e) =>
                    {
                        return ClustersGenerator.KMeanPlusPlus_FarthestFirst_HardSelect(X, C, e);
                    }, data.expect, 1);
                mc_fcm3._solve();

                var mc_fcm4 = new MC_FCM_Trial(data.X, data.C, MC_FCM.CGMode.MaxFuzzificationCoefficientGroups,
                    (X, C, m, e) =>
                    {
                        return ClustersGenerator.KMeanPlusPlus_HardSelect(X, C, e);
                    }, data.expect, 1);
                mc_fcm4._solve();

                var mc_fcm5 = new MC_FCM_Trial(data.X, data.C, MC_FCM.CGMode.KMeanPlusPlus,
                    null, data.expect, 100);
                mc_fcm5._solve();

                int min_L = new[] { fcm.AverageL/*, mc_fcm.AverageL*/, mc_fcm2.AverageL, mc_fcm3.AverageL, mc_fcm4.AverageL, mc_fcm5.AverageL }.Min();

                pt.PrintRow(dataInfo.Name,
                    new ColumnContent()
                    {
                        Content = fcm.AverageL,
                        Color = (fcm.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm.AverageL,
                    //    Color = (mc_fcm.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2.AverageL,
                        Color = (mc_fcm2.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3.AverageL,
                        Color = (mc_fcm3.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4.AverageL,
                        Color = (mc_fcm4.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5.AverageL,
                        Color = (mc_fcm5.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    });

                var er = fcm.Evaluation_Result.Round(6);
                //var er1 = mc_fcm.Evaluation_Result.Round(6);
                var er2 = mc_fcm2.Evaluation_Result.Round(6);
                var er3 = mc_fcm3.Evaluation_Result.Round(6);
                var er4 = mc_fcm4.Evaluation_Result.Round(6);
                var er5 = mc_fcm5.Evaluation_Result.Round(6);

                var fcm_sswc = er.SSWC;
                var fcm_db = er.DB;
                var fcm_pbm = er.PBM;
                var fcm_accuracy = er.Accuracy;
                var fcm_rand = er.Rand;
                var fcm_jaccard = er.Jaccard;

                //var mc_fcm_sswc = er1.SSWC;
                //var mc_fcm_db = er1.DB;
                //var mc_fcm_pbm = er1.PBM;
                //var mc_fcm_accuracy = er1.Accuracy;
                //var mc_fcm_rand = er1.Rand;
                //var mc_fcm_jaccard = er1.Jaccard;

                var mc_fcm2_sswc = er2.SSWC;
                var mc_fcm2_db = er2.DB;
                var mc_fcm2_pbm = er2.PBM;
                var mc_fcm2_accuracy = er2.Accuracy;
                var mc_fcm2_rand = er2.Rand;
                var mc_fcm2_jaccard = er2.Jaccard;

                var mc_fcm3_sswc = er3.SSWC;
                var mc_fcm3_db = er3.DB;
                var mc_fcm3_pbm = er3.PBM;
                var mc_fcm3_accuracy = er3.Accuracy;
                var mc_fcm3_rand = er3.Rand;
                var mc_fcm3_jaccard = er3.Jaccard;

                var mc_fcm4_sswc = er4.SSWC;
                var mc_fcm4_db = er4.DB;
                var mc_fcm4_pbm = er4.PBM;
                var mc_fcm4_accuracy = er4.Accuracy;
                var mc_fcm4_rand = er4.Rand;
                var mc_fcm4_jaccard = er4.Jaccard;

                var mc_fcm5_sswc = er5.SSWC;
                var mc_fcm5_db = er5.DB;
                var mc_fcm5_pbm = er5.PBM;
                var mc_fcm5_accuracy = er5.Accuracy;
                var mc_fcm5_rand = er5.Rand;
                var mc_fcm5_jaccard = er5.Jaccard;

                var max_sswc = new[] { fcm_sswc/*, mc_fcm_sswc*/, mc_fcm2_sswc, mc_fcm3_sswc, mc_fcm4_sswc, mc_fcm5_sswc }.Max();
                var min_db = new[] { fcm_db, mc_fcm2_db, mc_fcm3_db, mc_fcm4_db, mc_fcm5_db }.Min();
                var max_pbm = new[] { fcm_pbm, mc_fcm2_pbm, mc_fcm3_pbm, mc_fcm4_pbm, mc_fcm5_pbm }.Max();
                var max_accuracy = new[] { fcm_accuracy, mc_fcm2_accuracy, mc_fcm3_accuracy, mc_fcm4_accuracy, mc_fcm5_accuracy }.Max();
                var max_rand = new[] { fcm_rand, mc_fcm2_rand, mc_fcm3_rand, mc_fcm4_rand, mc_fcm5_rand }.Max();
                var max_jaccard = new[] { fcm_jaccard, mc_fcm2_jaccard, mc_fcm3_jaccard, mc_fcm4_jaccard, mc_fcm5_jaccard }.Max();

                pt.PrintRow("    SSWC",
                    new ColumnContent()
                    {
                        Content = fcm_sswc,
                        Color = (fcm_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_sswc,
                    //    Color = (mc_fcm_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_sswc,
                        Color = (mc_fcm2_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_sswc,
                        Color = (mc_fcm3_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_sswc,
                        Color = (mc_fcm4_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_sswc,
                        Color = (mc_fcm5_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    DB",
                    new ColumnContent()
                    {
                        Content = fcm_db,
                        Color = (fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_db,
                    //    Color = (mc_fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_db,
                        Color = (mc_fcm2_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_db,
                        Color = (mc_fcm3_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_db,
                        Color = (mc_fcm4_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_db,
                        Color = (mc_fcm5_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    PBM",
                    new ColumnContent()
                    {
                        Content = fcm_pbm,
                        Color = (fcm_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_pbm,
                    //    Color = (mc_fcm_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_pbm,
                        Color = (mc_fcm2_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_pbm,
                        Color = (mc_fcm3_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_pbm,
                        Color = (mc_fcm4_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_pbm,
                        Color = (mc_fcm5_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    ACC",
                    new ColumnContent()
                    {
                        Content = fcm_accuracy,
                        Color = (fcm_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_accuracy,
                    //    Color = (mc_fcm_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_accuracy,
                        Color = (mc_fcm2_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_accuracy,
                        Color = (mc_fcm3_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_accuracy,
                        Color = (mc_fcm4_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_accuracy,
                        Color = (mc_fcm5_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    RAND",
                    new ColumnContent()
                    {
                        Content = fcm_rand,
                        Color = (fcm_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_rand,
                    //    Color = (mc_fcm_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_rand,
                        Color = (mc_fcm2_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_rand,
                        Color = (mc_fcm3_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_rand,
                        Color = (mc_fcm4_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_rand,
                        Color = (mc_fcm5_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    JACC",
                    new ColumnContent()
                    {
                        Content = fcm_jaccard,
                        Color = (fcm_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_jaccard,
                    //    Color = (mc_fcm_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_jaccard,
                        Color = (mc_fcm2_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_jaccard,
                        Color = (mc_fcm3_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm4_jaccard,
                        Color = (mc_fcm4_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_jaccard,
                        Color = (mc_fcm5_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    });

                pt.PrintRow("", "", "", "", "", ""/*, ""*/);
            }
            pt.PrintLine();
            Console.WriteLine();
        }

        public static void FCM_DiffGenClusters_Benchmark()
        {
            //Console.SetWindowSize(1900, 980);
            Data.LoadAllExampleData();
            PrintTable pt = new PrintTable(new ColumnStyle[] {
                new ColumnStyle(){Width=20},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                new ColumnStyle(){Align=ColumnAlign.Right, Width = 16},
                //new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
                //new ColumnStyle(){Align=ColumnAlign.Right, Width = 17},
            });
            Console.WriteLine("FCM Different Cluster Generation Benchmark");
            Console.WriteLine("");
            pt.PrintLine();
            pt.PrintRow("", "Choose K", "KMeanPP HS", "KMeanPP FF HS", "KMeanPP");
            pt.PrintLine();
            foreach (var dataInfo in Data.datas)
            {
                //var dataInfo = datas[2];
                var data = dataInfo.aCIDb;

                var fcm = new FCM_Trial(data.X, data.C, 2, MC_FCM.CGMode.ChooseCFromXClustersRandomly,
                    null, data.expect, 100);
                fcm._solve();

                //var mc_fcm = new FCM_Trial(data.X, data.C, 2, MC_FCM.CGMode.StupidRandom,
                //    null, data.expect, 100);
                //mc_fcm._solve();

                var mc_fcm2 = new FCM_Trial(data.X, data.C, 2, MC_FCM.CGMode.Custom,
                    (X, C, m, e) =>
                    {
                        return ClustersGenerator.KMeanPlusPlus_HardSelect(X, C, e);
                    }, data.expect, 100);
                mc_fcm2._solve();

                var mc_fcm3 = new FCM_Trial(data.X, data.C, 2, MC_FCM.CGMode.Custom,
                   (X, C, m, e) =>
                   {
                       return ClustersGenerator.KMeanPlusPlus_FarthestFirst_HardSelect(X, C, e);
                   }, data.expect, 1);
                mc_fcm3._solve();

                //var mc_fcm4 = new FCM(data.X, data.C, 2);
                //mc_fcm4.ClustersGenerationMode = MC_FCM.CGMode.MaxFuzzificationCoefficientGroups;
                //mc_fcm4._solve();

                var mc_fcm5 = new FCM_Trial(data.X, data.C, 2, MC_FCM.CGMode.KMeanPlusPlus,
                    null, data.expect, 100);
                mc_fcm5._solve();


                int min_L = new[] { fcm.AverageL/*, mc_fcm.AverageL*/, mc_fcm2.AverageL, mc_fcm3.AverageL, mc_fcm5.AverageL }.Min();

                pt.PrintRow(dataInfo.Name,
                    new ColumnContent()
                    {
                        Content = fcm.AverageL,
                        Color = (fcm.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm.AverageL,
                    //    Color = (mc_fcm.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2.AverageL,
                        Color = (mc_fcm2.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3.AverageL,
                        Color = (mc_fcm3.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = r5.l,
                    //    Color = (r5.l == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm5.AverageL,
                        Color = (mc_fcm5.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    });

                var er = fcm.Evaluation_Result.Round(6);
                //var er1 = mc_fcm.Evaluation_Result.Round(6);
                var er2 = mc_fcm2.Evaluation_Result.Round(6);
                var er3 = mc_fcm3.Evaluation_Result.Round(6);
                //var er4 = mc_fcm4.Evaluation_Result.Round(6);
                var er5 = mc_fcm5.Evaluation_Result.Round(6);

                var fcm_sswc = er.SSWC;
                var fcm_db = er.DB;
                var fcm_pbm = er.PBM;
                var fcm_accuracy = er.Accuracy;
                var fcm_rand = er.Rand;
                var fcm_jaccard = er.Jaccard;

                //var mc_fcm_sswc = er1.SSWC;
                //var mc_fcm_db = er1.DB;
                //var mc_fcm_pbm = er1.PBM;
                //var mc_fcm_accuracy = er1.Accuracy;
                //var mc_fcm_rand = er1.Rand;
                //var mc_fcm_jaccard = er1.Jaccard;

                var mc_fcm2_sswc = er2.SSWC;
                var mc_fcm2_db = er2.DB;
                var mc_fcm2_pbm = er2.PBM;
                var mc_fcm2_accuracy = er2.Accuracy;
                var mc_fcm2_rand = er2.Rand;
                var mc_fcm2_jaccard = er2.Jaccard;

                var mc_fcm3_sswc = er3.SSWC;
                var mc_fcm3_db = er3.DB;
                var mc_fcm3_pbm = er3.PBM;
                var mc_fcm3_accuracy = er3.Accuracy;
                var mc_fcm3_rand = er3.Rand;
                var mc_fcm3_jaccard = er3.Jaccard;

                //var mc_fcm4_sswc = new SSWC(data.X, data.C, mc_fcm4.Result).Index;
                //var mc_fcm4_db = new DB(data.X, data.C, mc_fcm4.Result).Index;
                //var mc_fcm4_pbm = new PBM(data.X, data.C, mc_fcm4.Result).Index;
                //var mc_fcm4_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm4.Result).Index;
                //var mc_fcm4_rand = new Rand(data.X, data.C, data.expect, mc_fcm4.Result).Index;
                //var mc_fcm4_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm4.Result).Index;

                var mc_fcm5_sswc = er5.SSWC;
                var mc_fcm5_db = er5.DB;
                var mc_fcm5_pbm = er5.PBM;
                var mc_fcm5_accuracy = er5.Accuracy;
                var mc_fcm5_rand = er5.Rand;
                var mc_fcm5_jaccard = er5.Jaccard;

                var max_sswc = new[] { fcm_sswc/*, mc_fcm_sswc*/, mc_fcm2_sswc, mc_fcm3_sswc/*, mc_fcm4_sswc*/, mc_fcm5_sswc }.Max();
                var min_db = new[] { fcm_db/*, mc_fcm_db*/, mc_fcm2_db, mc_fcm3_db, mc_fcm5_db }.Min();
                var max_pbm = new[] { fcm_pbm/*, mc_fcm_pbm*/, mc_fcm2_pbm, mc_fcm3_pbm, mc_fcm5_pbm }.Max();
                var max_accuracy = new[] { fcm_accuracy/*, mc_fcm_accuracy*/, mc_fcm2_accuracy, mc_fcm3_accuracy, mc_fcm5_accuracy }.Max();
                var max_rand = new[] { fcm_rand/*, mc_fcm_rand*/, mc_fcm2_rand, mc_fcm3_rand, mc_fcm5_rand }.Max();
                var max_jaccard = new[] { fcm_jaccard/*, mc_fcm_jaccard*/, mc_fcm2_jaccard, mc_fcm3_jaccard, mc_fcm5_jaccard }.Max();

                pt.PrintRow("    SSWC",
                    new ColumnContent()
                    {
                        Content = fcm_sswc,
                        Color = (fcm_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_sswc,
                    //    Color = (mc_fcm_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_sswc,
                        Color = (mc_fcm2_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_sswc,
                        Color = (mc_fcm3_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm4_sswc,
                    //    Color = (mc_fcm4_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm5_sswc,
                        Color = (mc_fcm5_sswc == max_sswc) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    DB",
                    new ColumnContent()
                    {
                        Content = fcm_db,
                        Color = (fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_db,
                    //    Color = (mc_fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_db,
                        Color = (mc_fcm2_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_db,
                        Color = (mc_fcm3_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm4_db,
                    //    Color = (mc_fcm4_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm5_db,
                        Color = (mc_fcm5_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    PBM",
                    new ColumnContent()
                    {
                        Content = fcm_pbm,
                        Color = (fcm_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_pbm,
                    //    Color = (mc_fcm_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_pbm,
                        Color = (mc_fcm2_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_pbm,
                        Color = (mc_fcm3_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm4_pbm,
                    //    Color = (mc_fcm4_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm5_pbm,
                        Color = (mc_fcm5_pbm == max_pbm) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    ACC",
                    new ColumnContent()
                    {
                        Content = fcm_accuracy,
                        Color = (fcm_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_accuracy,
                    //    Color = (mc_fcm_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_accuracy,
                        Color = (mc_fcm2_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_accuracy,
                        Color = (mc_fcm3_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm4_accuracy,
                    //    Color = (mc_fcm4_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm5_accuracy,
                        Color = (mc_fcm5_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    RAND",
                    new ColumnContent()
                    {
                        Content = fcm_rand,
                        Color = (fcm_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_rand,
                    //    Color = (mc_fcm_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_rand,
                        Color = (mc_fcm2_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_rand,
                        Color = (mc_fcm3_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm4_rand,
                    //    Color = (mc_fcm4_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm5_rand,
                        Color = (mc_fcm5_rand == max_rand) ? ConsoleColor.Yellow : ConsoleColor.White
                    });
                pt.PrintRow("    JACC",
                    new ColumnContent()
                    {
                        Content = fcm_jaccard,
                        Color = (fcm_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm_jaccard,
                    //    Color = (mc_fcm_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm2_jaccard,
                        Color = (mc_fcm2_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm3_jaccard,
                        Color = (mc_fcm3_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    //new ColumnContent()
                    //{
                    //    Content = mc_fcm4_jaccard,
                    //    Color = (mc_fcm4_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    //},
                    new ColumnContent()
                    {
                        Content = mc_fcm5_jaccard,
                        Color = (mc_fcm5_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    });

                pt.PrintRow("", "", "", "", ""/*, ""*//*, ""*/);
            }
            pt.PrintLine();
            Console.WriteLine();
        }
    }
}
