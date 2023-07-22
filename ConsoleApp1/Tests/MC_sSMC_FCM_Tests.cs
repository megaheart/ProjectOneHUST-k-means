using ProjectOneClasses.ResultTypes;
using ProjectOneClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOneClasses.Trials;

namespace ConsoleApp1.Tests
{
    public static class MC_sSMC_FCM_Tests
    {
        public static void MC_SSMC_FCM_WithTrial_Benchmark()
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
                new ColumnStyle(){Align=ColumnAlign.Right, Width = 17},
            });

            pt.PrintLine();
            pt.PrintRow("", "MC-sSMC-FCM(5%)", "MC-sSMC-FCM(10%)", "MC-sSMC-FCM(15%)", "MC-sSMC-FCM(20%)", "MC-sSMC-FCM(0%)", "MC-sSMC-FCM(100%)");
            pt.PrintLine();
            foreach (var dataInfo in Data.datas)
            {
                //var dataInfo = datas[2];
                var data = dataInfo.aCIDb;
                var fcm = new MC_sSMC_FCM_Trial(data.X, data.C, 5, data.expect, 100);
                fcm._solve();
                var mc_fcm = new MC_sSMC_FCM_Trial(data.X, data.C, 10, data.expect, 100);
                mc_fcm._solve();
                var mc_fcm2 = new MC_sSMC_FCM_Trial(data.X, data.C, 15, data.expect, 100);
                mc_fcm2._solve();
                var mc_fcm3 = new MC_sSMC_FCM_Trial(data.X, data.C, 20, data.expect, 100);
                mc_fcm3._solve();
                var mc_fcm4 = new MC_sSMC_FCM_Trial(data.X, data.C, 0, data.expect, 100);
                mc_fcm4._solve();
                var mc_fcm5 = new MC_sSMC_FCM_Trial(data.X, data.C, 100, data.expect, 100);
                mc_fcm5._solve();


                int min_L = new[] { fcm.AverageL, mc_fcm.AverageL, mc_fcm2.AverageL, mc_fcm3.AverageL, mc_fcm4.AverageL, mc_fcm5.AverageL }.Min();

                pt.PrintRow(dataInfo.Name,
                    new ColumnContent()
                    {
                        Content = fcm.AverageL,
                        Color = (fcm.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm.AverageL,
                        Color = (mc_fcm.AverageL == min_L) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
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
                var er1 = mc_fcm.Evaluation_Result.Round(6);
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
                var fcm_trainAccuracy = er.TrainAccuracy;

                var mc_fcm_sswc = er1.SSWC;
                var mc_fcm_db = er1.DB;
                var mc_fcm_pbm = er1.PBM;
                var mc_fcm_accuracy = er1.Accuracy;
                var mc_fcm_rand = er1.Rand;
                var mc_fcm_jaccard = er1.Jaccard;
                var mc_fcm_trainAccuracy = er1.TrainAccuracy;

                var mc_fcm2_sswc = er2.SSWC;
                var mc_fcm2_db = er2.DB;
                var mc_fcm2_pbm = er2.PBM;
                var mc_fcm2_accuracy = er2.Accuracy;
                var mc_fcm2_rand = er2.Rand;
                var mc_fcm2_jaccard = er2.Jaccard;
                var mc_fcm2_trainAccuracy = er2.TrainAccuracy;

                var mc_fcm3_sswc = er3.SSWC;
                var mc_fcm3_db = er3.DB;
                var mc_fcm3_pbm = er3.PBM;
                var mc_fcm3_accuracy = er3.Accuracy;
                var mc_fcm3_rand = er3.Rand;
                var mc_fcm3_jaccard = er3.Jaccard;
                var mc_fcm3_trainAccuracy = er3.TrainAccuracy;

                var mc_fcm4_sswc = er4.SSWC;
                var mc_fcm4_db = er4.DB;
                var mc_fcm4_pbm = er4.PBM;
                var mc_fcm4_accuracy = er4.Accuracy;
                var mc_fcm4_rand = er4.Rand;
                var mc_fcm4_jaccard = er4.Jaccard;
                var mc_fcm4_trainAccuracy = er4.TrainAccuracy;

                var mc_fcm5_sswc = er5.SSWC;
                var mc_fcm5_db = er5.DB;
                var mc_fcm5_pbm = er5.PBM;
                var mc_fcm5_accuracy = er5.Accuracy;
                var mc_fcm5_rand = er5.Rand;
                var mc_fcm5_jaccard = er5.Jaccard;
                var mc_fcm5_trainAccuracy = er5.TrainAccuracy;

                var max_sswc = new[] { fcm_sswc, mc_fcm_sswc, mc_fcm2_sswc, mc_fcm3_sswc, mc_fcm4_sswc/*, mc_fcm5_sswc*/ }.Max();
                var min_db = new[] { fcm_db, mc_fcm_db, mc_fcm2_db, mc_fcm3_db, mc_fcm4_db/*, mc_fcm5_db*/ }.Min();
                var max_pbm = new[] { fcm_pbm, mc_fcm_pbm, mc_fcm2_pbm, mc_fcm3_pbm, mc_fcm4_pbm/*, mc_fcm5_pbm*/ }.Max();
                var max_accuracy = new[] { fcm_accuracy, mc_fcm_accuracy, mc_fcm2_accuracy, mc_fcm3_accuracy, mc_fcm4_accuracy/*, mc_fcm5_accuracy*/ }.Max();
                var max_rand = new[] { fcm_rand, mc_fcm_rand, mc_fcm2_rand, mc_fcm3_rand, mc_fcm4_rand/*, mc_fcm5_rand*/ }.Max();
                var max_jaccard = new[] { fcm_jaccard, mc_fcm_jaccard, mc_fcm2_jaccard, mc_fcm3_jaccard, mc_fcm4_jaccard/*, mc_fcm5_jaccard*/ }.Max();
                var max_trainAccuracy = new[] { fcm_trainAccuracy, mc_fcm_trainAccuracy, mc_fcm2_trainAccuracy, mc_fcm3_trainAccuracy, mc_fcm4_trainAccuracy, mc_fcm5_trainAccuracy }.Max();

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
                    new ColumnContent()
                    {
                        Content = mc_fcm_db,
                        Color = (mc_fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
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
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_accuracy,
                        Color = (mc_fcm5_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    }); pt.PrintRow("    TRAIN ACC",
                new ColumnContent()
                {
                    Content = fcm_trainAccuracy,
                    Color = (fcm_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm_trainAccuracy,
                    Color = (mc_fcm_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm2_trainAccuracy,
                    Color = (mc_fcm2_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm3_trainAccuracy,
                    Color = (mc_fcm3_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm4_trainAccuracy,
                    Color = (mc_fcm4_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm5_trainAccuracy,
                    Color = (mc_fcm5_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
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
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_jaccard,
                        Color = (mc_fcm5_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    });

                pt.PrintRow("", "", "", "", "", "", "");
            }
            pt.PrintLine();
            Console.WriteLine();
        }
        public static void MC_SSMC_FCM_Benchmark()
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
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 17},
        });

            pt.PrintLine();
            pt.PrintRow("", "MC-sSMC-FCM(5%)", "MC-sSMC-FCM(10%)", "MC-sSMC-FCM(15%)", "MC-sSMC-FCM(20%)", "MC-sSMC-FCM(0%)", "MC-sSMC-FCM(100%)");
            pt.PrintLine();
            foreach (var dataInfo in Data.datas)
            {
                //var dataInfo = datas[2];
                var data = dataInfo.aCIDb;
                var fcm = new MC_sSMC_FCM(data.X, data.C, dataInfo.semiSupervised5);
                fcm._solve();
                var mc_fcm = new MC_sSMC_FCM(data.X, data.C, dataInfo.semiSupervised10);
                mc_fcm._solve();
                var mc_fcm2 = new MC_sSMC_FCM(data.X, data.C, dataInfo.semiSupervised15);
                mc_fcm2._solve();
                var mc_fcm3 = new MC_sSMC_FCM(data.X, data.C, dataInfo.semiSupervised20);
                mc_fcm3._solve();
                var mc_fcm4 = new MC_sSMC_FCM(data.X, data.C, Data.semiSupervised0);
                mc_fcm4._solve();
                var mc_fcm5 = new MC_sSMC_FCM(data.X, data.C, dataInfo.semiSupervised100);
                mc_fcm5._solve();


                var r = fcm.Result;
                var r2 = mc_fcm.Result;
                var r3 = mc_fcm2.Result;
                var r4 = mc_fcm3.Result;
                var r5 = mc_fcm4.Result;
                var r6 = mc_fcm5.Result;

                pt.PrintRow(dataInfo.Name, fcm.Result.l, mc_fcm.Result.l, mc_fcm2.Result.l, mc_fcm3.Result.l, mc_fcm4.Result.l, mc_fcm5.Result.l);

                var er = new sSMC_FCM_Evaluation_Result(data.X, data.C, dataInfo.semiSupervised5, data.expect, r).Round(6);
                var er1 = new sSMC_FCM_Evaluation_Result(data.X, data.C, dataInfo.semiSupervised10, data.expect, r2).Round(6);
                var er2 = new sSMC_FCM_Evaluation_Result(data.X, data.C, dataInfo.semiSupervised15, data.expect, r3).Round(6);
                var er3 = new sSMC_FCM_Evaluation_Result(data.X, data.C, dataInfo.semiSupervised20, data.expect, r4).Round(6);
                var er4 = new sSMC_FCM_Evaluation_Result(data.X, data.C, Data.semiSupervised0, data.expect, r5).Round(6);
                var er5 = new sSMC_FCM_Evaluation_Result(data.X, data.C, dataInfo.semiSupervised100, data.expect, r6).Round(6);

                var fcm_sswc = er.SSWC;
                var fcm_db = er.DB;
                var fcm_pbm = er.PBM;
                var fcm_accuracy = er.Accuracy;
                var fcm_rand = er.Rand;
                var fcm_jaccard = er.Jaccard;
                var fcm_trainAccuracy = er.TrainAccuracy;

                var mc_fcm_sswc = er1.SSWC;
                var mc_fcm_db = er1.DB;
                var mc_fcm_pbm = er1.PBM;
                var mc_fcm_accuracy = er1.Accuracy;
                var mc_fcm_rand = er1.Rand;
                var mc_fcm_jaccard = er1.Jaccard;
                var mc_fcm_trainAccuracy = er1.TrainAccuracy;

                var mc_fcm2_sswc = er2.SSWC;
                var mc_fcm2_db = er2.DB;
                var mc_fcm2_pbm = er2.PBM;
                var mc_fcm2_accuracy = er2.Accuracy;
                var mc_fcm2_rand = er2.Rand;
                var mc_fcm2_jaccard = er2.Jaccard;
                var mc_fcm2_trainAccuracy = er2.TrainAccuracy;

                var mc_fcm3_sswc = er3.SSWC;
                var mc_fcm3_db = er3.DB;
                var mc_fcm3_pbm = er3.PBM;
                var mc_fcm3_accuracy = er3.Accuracy;
                var mc_fcm3_rand = er3.Rand;
                var mc_fcm3_jaccard = er3.Jaccard;
                var mc_fcm3_trainAccuracy = er3.TrainAccuracy;

                var mc_fcm4_sswc = er4.SSWC;
                var mc_fcm4_db = er4.DB;
                var mc_fcm4_pbm = er4.PBM;
                var mc_fcm4_accuracy = er4.Accuracy;
                var mc_fcm4_rand = er4.Rand;
                var mc_fcm4_jaccard = er4.Jaccard;
                var mc_fcm4_trainAccuracy = er4.TrainAccuracy;

                var mc_fcm5_sswc = er5.SSWC;
                var mc_fcm5_db = er5.DB;
                var mc_fcm5_pbm = er5.PBM;
                var mc_fcm5_accuracy = er5.Accuracy;
                var mc_fcm5_rand = er5.Rand;
                var mc_fcm5_jaccard = er5.Jaccard;
                var mc_fcm5_trainAccuracy = er5.TrainAccuracy;

                var max_sswc = new[] { fcm_sswc, mc_fcm_sswc, mc_fcm2_sswc, mc_fcm3_sswc, mc_fcm4_sswc/*, mc_fcm5_sswc*/ }.Max();
                var min_db = new[] { fcm_db, mc_fcm_db, mc_fcm2_db, mc_fcm3_db, mc_fcm4_db/*, mc_fcm5_db*/ }.Min();
                var max_pbm = new[] { fcm_pbm, mc_fcm_pbm, mc_fcm2_pbm, mc_fcm3_pbm, mc_fcm4_pbm/*, mc_fcm5_pbm*/ }.Max();
                var max_accuracy = new[] { fcm_accuracy, mc_fcm_accuracy, mc_fcm2_accuracy, mc_fcm3_accuracy, mc_fcm4_accuracy/*, mc_fcm5_accuracy*/ }.Max();
                var max_rand = new[] { fcm_rand, mc_fcm_rand, mc_fcm2_rand, mc_fcm3_rand, mc_fcm4_rand/*, mc_fcm5_rand*/ }.Max();
                var max_jaccard = new[] { fcm_jaccard, mc_fcm_jaccard, mc_fcm2_jaccard, mc_fcm3_jaccard, mc_fcm4_jaccard/*, mc_fcm5_jaccard*/ }.Max();
                var max_trainAccuracy = new[] { fcm_trainAccuracy, mc_fcm_trainAccuracy, mc_fcm2_trainAccuracy, mc_fcm3_trainAccuracy, mc_fcm4_trainAccuracy, mc_fcm5_trainAccuracy }.Max();

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
                    new ColumnContent()
                    {
                        Content = mc_fcm_db,
                        Color = (mc_fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                    },
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
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_accuracy,
                        Color = (mc_fcm5_accuracy == max_accuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                    }); pt.PrintRow("    TRAIN ACC",
                new ColumnContent()
                {
                    Content = fcm_trainAccuracy,
                    Color = (fcm_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm_trainAccuracy,
                    Color = (mc_fcm_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm2_trainAccuracy,
                    Color = (mc_fcm2_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm3_trainAccuracy,
                    Color = (mc_fcm3_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm4_trainAccuracy,
                    Color = (mc_fcm4_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent()
                {
                    Content = mc_fcm5_trainAccuracy,
                    Color = (mc_fcm5_trainAccuracy == max_trainAccuracy) ? ConsoleColor.Yellow : ConsoleColor.White
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
                    },
                    new ColumnContent()
                    {
                        Content = mc_fcm5_jaccard,
                        Color = (mc_fcm5_jaccard == max_jaccard) ? ConsoleColor.Yellow : ConsoleColor.White
                    });

                pt.PrintRow("", "", "", "", "", "", "");
            }
            pt.PrintLine();
            Console.WriteLine();
        }
    }
}
