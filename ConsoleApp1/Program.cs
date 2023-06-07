
using ConsoleApp1;
using ConsoleApp1.Utilities;
using ProjectOneClasses;
using ProjectOneClasses.Utilities;
using ProjectOneClasses.ValidityCriterias.External;
using ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;


public class Program
{
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

            pt.PrintRow(dataInfo.Name, fcm.Result.l, mc_fcm.Result.l, mc_fcm2.Result.l, mc_fcm3.Result.l, mc_fcm4.Result.l, mc_fcm5.Result.l);

            var fcm_sswc = new SSWC(data.X, data.C, /*new FakeFCM_Result(data.X, data.expect, data.C)*/fcm.Result).Index;
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

            var mc_fcm5_sswc = new SSWC(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_db = new DB(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_pbm = new PBM(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_rand = new Rand(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm5.Result).Index;

            var max_sswc = new[] { fcm_sswc, mc_fcm_sswc, mc_fcm2_sswc, mc_fcm3_sswc, mc_fcm4_sswc/*, mc_fcm5_sswc*/ }.Max();
            var min_db = new[] { fcm_db, mc_fcm_db, mc_fcm2_db, mc_fcm3_db, mc_fcm4_db/*, mc_fcm5_db*/ }.Min();
            var max_pbm = new[] {fcm_pbm, mc_fcm_pbm, mc_fcm2_pbm, mc_fcm3_pbm, mc_fcm4_pbm/*, mc_fcm5_pbm*/ }.Max();
            var max_accuracy = new[] {fcm_accuracy, mc_fcm_accuracy, mc_fcm2_accuracy, mc_fcm3_accuracy, mc_fcm4_accuracy/*, mc_fcm5_accuracy*/ }.Max();
            var max_rand = new[] {fcm_rand, mc_fcm_rand, mc_fcm2_rand, mc_fcm3_rand, mc_fcm4_rand/*, mc_fcm5_rand*/ }.Max();
            var max_jaccard = new[] {fcm_jaccard, mc_fcm_jaccard, mc_fcm2_jaccard, mc_fcm3_jaccard, mc_fcm4_jaccard/*, mc_fcm5_jaccard*/ }.Max();

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
    public static void SSMC_FCM_Benchmark()
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
        pt.PrintRow("", "sSMC-FCM (5%)", "sSMC-FCM (10%)", "sSMC-FCM (15%)", "sSMC-FCM (20%)", "sSMC-FCM (0%)", "sSMC-FCM (100%)");
        pt.PrintLine();
        foreach (var dataInfo in Data.datas)
        {
            //var dataInfo = datas[2];
            var data = dataInfo.aCIDb;
            var fcm = new sSMC_FCM(data.X, data.C, dataInfo.semiSupervised5);
            fcm._solve();
            var mc_fcm = new sSMC_FCM(data.X, data.C, dataInfo.semiSupervised10);
            mc_fcm._solve();
            var mc_fcm2 = new sSMC_FCM(data.X, data.C, dataInfo.semiSupervised15);
            mc_fcm2._solve();
            var mc_fcm3 = new sSMC_FCM(data.X, data.C, dataInfo.semiSupervised20);
            mc_fcm3._solve();
            var mc_fcm4 = new sSMC_FCM(data.X, data.C, Data.semiSupervised0);
            mc_fcm4._solve();
            var mc_fcm5 = new sSMC_FCM(data.X, data.C, dataInfo.semiSupervised100);
            mc_fcm5._solve();

            var r = fcm.Result as sSMC_FCM.sSMC_FCM_Result;
            var r2 = mc_fcm.Result as sSMC_FCM.sSMC_FCM_Result;
            var r3 = mc_fcm2.Result as sSMC_FCM.sSMC_FCM_Result;
            var r4 = mc_fcm3.Result as sSMC_FCM.sSMC_FCM_Result;
            var r5 = mc_fcm4.Result as sSMC_FCM.sSMC_FCM_Result;
            var r6 = mc_fcm5.Result as sSMC_FCM.sSMC_FCM_Result;

            pt.PrintRow(dataInfo.Name, $"{r.l} {r.M} {r.M2:F4}", $"{r2.l} {r2.M} {r2.M2:F4}", $"{r3.l} {r3.M} {r3.M2:F4}", $"{r4.l} {r4.M} {r4.M2:F4}", $"{r5.l} {r5.M} {r5.M2:F4}", $"{r6.l} {r6.M} {r6.M2:F4}");

            var fcm_sswc = new SSWC(data.X, data.C, /*new FakeFCM_Result(data.X, data.expect, data.C)*/fcm.Result).Index;
            var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
            var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
            var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
            var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
            var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;
            var fcm_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised5, data.C, fcm.Result).Index;

            var mc_fcm_sswc = new SSWC(data.X, data.C, mc_fcm.Result).Index;
            var mc_fcm_db = new DB(data.X, data.C, mc_fcm.Result).Index;
            var mc_fcm_pbm = new PBM(data.X, data.C, mc_fcm.Result).Index;
            var mc_fcm_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm.Result).Index;
            var mc_fcm_rand = new Rand(data.X, data.C, data.expect, mc_fcm.Result).Index;
            var mc_fcm_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm.Result).Index;
            var mc_fcm_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised10, data.C, mc_fcm.Result).Index;

            var mc_fcm2_sswc = new SSWC(data.X, data.C, mc_fcm2.Result).Index;
            var mc_fcm2_db = new DB(data.X, data.C, mc_fcm2.Result).Index;
            var mc_fcm2_pbm = new PBM(data.X, data.C, mc_fcm2.Result).Index;
            var mc_fcm2_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm2.Result).Index;
            var mc_fcm2_rand = new Rand(data.X, data.C, data.expect, mc_fcm2.Result).Index;
            var mc_fcm2_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm2.Result).Index;
            var mc_fcm2_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised15, data.C, mc_fcm2.Result).Index;

            var mc_fcm3_sswc = new SSWC(data.X, data.C, mc_fcm3.Result).Index;
            var mc_fcm3_db = new DB(data.X, data.C, mc_fcm3.Result).Index;
            var mc_fcm3_pbm = new PBM(data.X, data.C, mc_fcm3.Result).Index;
            var mc_fcm3_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm3.Result).Index;
            var mc_fcm3_rand = new Rand(data.X, data.C, data.expect, mc_fcm3.Result).Index;
            var mc_fcm3_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm3.Result).Index;
            var mc_fcm3_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised20, data.C, mc_fcm3.Result).Index;

            var mc_fcm4_sswc = new SSWC(data.X, data.C, mc_fcm4.Result).Index;
            var mc_fcm4_db = new DB(data.X, data.C, mc_fcm4.Result).Index;
            var mc_fcm4_pbm = new PBM(data.X, data.C, mc_fcm4.Result).Index;
            var mc_fcm4_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm4.Result).Index;
            var mc_fcm4_rand = new Rand(data.X, data.C, data.expect, mc_fcm4.Result).Index;
            var mc_fcm4_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm4.Result).Index;
            var mc_fcm4_trainAccuracy = new TrainAccuracy(data.X, Data.semiSupervised0, data.C, mc_fcm4.Result).Index;

            var mc_fcm5_sswc = new SSWC(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_db = new DB(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_pbm = new PBM(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_rand = new Rand(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised100, data.C, mc_fcm5.Result).Index;

            var max_sswc = new[] { fcm_sswc, mc_fcm_sswc, mc_fcm2_sswc, mc_fcm3_sswc, mc_fcm4_sswc/*, mc_fcm5_sswc*/ }.Max();
            var min_db = new[] { fcm_db, mc_fcm_db, mc_fcm2_db, mc_fcm3_db, mc_fcm4_db/*, mc_fcm5_db*/ }.Min();
            var max_pbm = new[] { fcm_pbm, mc_fcm_pbm, mc_fcm2_pbm, mc_fcm3_pbm, mc_fcm4_pbm/*, mc_fcm5_pbm*/ }.Max();
            var max_accuracy = new[] { fcm_accuracy, mc_fcm_accuracy, mc_fcm2_accuracy, mc_fcm3_accuracy, mc_fcm4_accuracy/*, mc_fcm5_accuracy*/ }.Max();
            var max_rand = new[] { fcm_rand, mc_fcm_rand, mc_fcm2_rand, mc_fcm3_rand, mc_fcm4_rand/*, mc_fcm5_rand*/ }.Max();
            var max_jaccard = new[] { fcm_jaccard, mc_fcm_jaccard, mc_fcm2_jaccard, mc_fcm3_jaccard, mc_fcm4_jaccard/*, mc_fcm5_jaccard*/ }.Max();
            var max_trainAccuracy = new[] { fcm_trainAccuracy, mc_fcm_trainAccuracy, mc_fcm2_trainAccuracy, mc_fcm3_trainAccuracy, mc_fcm4_trainAccuracy, mc_fcm5_trainAccuracy}.Max();

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
                });
            pt.PrintRow("    TRAIN ACC",
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

    public static void SSMC_FCM_VectorM2_Benchmark()
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
        pt.PrintRow("", "sSMC-FCM (5%)", "sSMC-FCM (10%)", "sSMC-FCM (15%)", "sSMC-FCM (20%)", "sSMC-FCM (0%)", "sSMC-FCM (100%)");
        pt.PrintLine();
        foreach (var dataInfo in Data.datas)
        {
            //var dataInfo = datas[2];
            var data = dataInfo.aCIDb;
            var fcm = new sSMC_FCM_VectorM2(data.X, data.C, dataInfo.semiSupervised5);
            fcm._solve();
            var mc_fcm = new sSMC_FCM_VectorM2(data.X, data.C, dataInfo.semiSupervised10);
            mc_fcm._solve();
            var mc_fcm2 = new sSMC_FCM_VectorM2(data.X, data.C, dataInfo.semiSupervised15);
            mc_fcm2._solve();
            var mc_fcm3 = new sSMC_FCM_VectorM2(data.X, data.C, dataInfo.semiSupervised20);
            mc_fcm3._solve();
            var mc_fcm4 = new sSMC_FCM_VectorM2(data.X, data.C, Data.semiSupervised0);
            mc_fcm4._solve();
            var mc_fcm5 = new sSMC_FCM_VectorM2(data.X, data.C, dataInfo.semiSupervised100);
            mc_fcm5._solve();

            var r = fcm.Result ;
            var r2 = mc_fcm.Result ;
            var r3 = mc_fcm2.Result ;
            var r4 = mc_fcm3.Result ;
            var r5 = mc_fcm4.Result ;
            var r6 = mc_fcm5.Result ;

            pt.PrintRow(dataInfo.Name, $"{r.l}", $"{r2.l}", $"{r3.l}", $"{r4.l}", $"{r5.l}", $"{r6.l}");

            var fcm_sswc = new SSWC(data.X, data.C, /*new FakeFCM_Result(data.X, data.expect, data.C)*/fcm.Result).Index;
            var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
            var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
            var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
            var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
            var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;
            var fcm_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised5, data.C, fcm.Result).Index;

            var mc_fcm_sswc = new SSWC(data.X, data.C, mc_fcm.Result).Index;
            var mc_fcm_db = new DB(data.X, data.C, mc_fcm.Result).Index;
            var mc_fcm_pbm = new PBM(data.X, data.C, mc_fcm.Result).Index;
            var mc_fcm_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm.Result).Index;
            var mc_fcm_rand = new Rand(data.X, data.C, data.expect, mc_fcm.Result).Index;
            var mc_fcm_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm.Result).Index;
            var mc_fcm_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised10, data.C, mc_fcm.Result).Index;

            var mc_fcm2_sswc = new SSWC(data.X, data.C, mc_fcm2.Result).Index;
            var mc_fcm2_db = new DB(data.X, data.C, mc_fcm2.Result).Index;
            var mc_fcm2_pbm = new PBM(data.X, data.C, mc_fcm2.Result).Index;
            var mc_fcm2_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm2.Result).Index;
            var mc_fcm2_rand = new Rand(data.X, data.C, data.expect, mc_fcm2.Result).Index;
            var mc_fcm2_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm2.Result).Index;
            var mc_fcm2_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised15, data.C, mc_fcm2.Result).Index;

            var mc_fcm3_sswc = new SSWC(data.X, data.C, mc_fcm3.Result).Index;
            var mc_fcm3_db = new DB(data.X, data.C, mc_fcm3.Result).Index;
            var mc_fcm3_pbm = new PBM(data.X, data.C, mc_fcm3.Result).Index;
            var mc_fcm3_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm3.Result).Index;
            var mc_fcm3_rand = new Rand(data.X, data.C, data.expect, mc_fcm3.Result).Index;
            var mc_fcm3_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm3.Result).Index;
            var mc_fcm3_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised20, data.C, mc_fcm3.Result).Index;

            var mc_fcm4_sswc = new SSWC(data.X, data.C, mc_fcm4.Result).Index;
            var mc_fcm4_db = new DB(data.X, data.C, mc_fcm4.Result).Index;
            var mc_fcm4_pbm = new PBM(data.X, data.C, mc_fcm4.Result).Index;
            var mc_fcm4_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm4.Result).Index;
            var mc_fcm4_rand = new Rand(data.X, data.C, data.expect, mc_fcm4.Result).Index;
            var mc_fcm4_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm4.Result).Index;
            var mc_fcm4_trainAccuracy = new TrainAccuracy(data.X, Data.semiSupervised0, data.C, mc_fcm4.Result).Index;

            var mc_fcm5_sswc = new SSWC(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_db = new DB(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_pbm = new PBM(data.X, data.C, mc_fcm5.Result).Index;
            var mc_fcm5_accuracy = new Accuracy(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_rand = new Rand(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_jaccard = new Jaccard(data.X, data.C, data.expect, mc_fcm5.Result).Index;
            var mc_fcm5_trainAccuracy = new TrainAccuracy(data.X, dataInfo.semiSupervised100, data.C, mc_fcm5.Result).Index;

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
                });
            pt.PrintRow("    TRAIN ACC",
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

    public static void ViewX()
    {
        {
            var dataInfo = Data.datas.FirstOrDefault(t => t.Name == "Iris");
            dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);
            
            foreach(var x in  dataInfo.aCIDb.X)
            {
                Console.WriteLine(x.Print());
            }
        }
    }

    public static void CustomBenchmark_1()
    {
        //Console.SetWindowSize(1900, 980);
        Data.LoadAllExampleData();
        PrintTable pt = new PrintTable(new ColumnStyle[] {
            new ColumnStyle(){Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 16},
        });

        var data = new ACIDbDeserializer(@"C:\Users\linh2\Downloads\features2_40.txt");
        var gen = new SemiSupervisedGenerator(@"C:\Users\linh2\Downloads\features2_40.txt.p100.ss");
        //var X_shuffled = data.X.OrderBy(x => Random.Shared.Next()).ToArray();
        //Console.WriteLine(X_shuffled.SequenceEqual(data.X));

        pt.PrintLine();
        pt.PrintRow("cycle", "SSWC", "DB", "PBM", "ACC", "RAND", "JACC");
        pt.PrintLine();
        //var supervised = gen.semiSupervised;
        var supervised = new Dictionary<int, int>()
        {
            [4] = 1,
            [5] = 1,
            [6] = 1,
            [22] = 0,
            [23] = 0,
            [24] = 0,

        };
        //var fcm = new MC_sSMC_FCM(data.X, data.C, supervised/*new Dictionary<int, int>()*/);
        var fcm = new MC_FCM(data.X, data.C);
        fcm._solve();

        

        var fcm_sswc = new SSWC(data.X, data.C, /*new FakeFCM_Result(data.X, data.expect, data.C)*/fcm.Result).Index;
        var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
        var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
        var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
        var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
        var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;


        pt.PrintRow(fcm.Result.l, fcm_sswc, fcm_db, fcm_pbm, fcm_accuracy, fcm_rand, fcm_jaccard);
        pt.PrintLine();

        //Console.WriteLine($"{fcm.Result.M} {fcm.Result.M2}");

        StringBuilder[] sb = new StringBuilder[data.C];
        int[] count = new int[data.C];

        for (int k = 0; k < data.C; k++)
        {
            sb[k] = new StringBuilder();
        }

        for (int i = 0; i < data.X.Count; i++)
        {
            int k_max = 0;
            for (int k = 1; k < data.C; k++)
            {
                if (fcm.Result.U[i][k] > fcm.Result.U[i][k_max]) k_max = k;
            }
            sb[k_max].Append($"{i} ");
            count[k_max]++;
        }
        for (int k = 0; k < data.C; k++)
        {
            Console.WriteLine("({0}) {1}", count[k], sb[k]);
        }
        Console.WriteLine();
    }

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();
    private static IntPtr ThisConsole = GetConsoleWindow();
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private const int HIDE = 0;
    private const int MAXIMIZE = 3;
    private const int MINIMIZE = 6;
    private const int RESTORE = 9;
    static void Main(string[] args)
    {
        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        ShowWindow(ThisConsole, MAXIMIZE);



        //Data.CreateSemiSupervisonOfAllData();
        //Data.Create_BestMCFCM_SemiSupervisonOfAllData();
        //MC_FCM_Tests.MC_FCM_DiffGenClusters_Benchmark();
        //MC_FCM_Tests.MC_SSMC_FCM_Benchmark();
        //MC_FCM_Tests.FCM_And_MC_FCM_BenchMark();
        //SSMC_FCM_Benchmark();
        //SSMC_FCM_VectorM2_Benchmark();

        CustomBenchmark_1();

        //ViewX();

        //Console.ReadLine();

        //var lines = File.ReadAllLines(@"C:\Users\linh2\Downloads\features2_40.txt");

        //for(int i = 0; i < lines.Length; i++)
        //{
        //    lines[i] += " " + (i / 20) + 1;

        //    //lines[i] = lines[i].Remove(lines[i].Length - 1, 1);
        //}

        //File.WriteAllLines(@"C:\Users\linh2\Downloads\features2_40.txt", lines);

        //var data = new ACIDbDeserializer(@"C:\Users\linh2\Downloads\features2_40.txt");
        //var gen = new SemiSupervisedGenerator(data.expect, data.expect.Count);
        //gen.WriteFile(@"C:\Users\linh2\Downloads\features2_40.txt.p100.ss");
    }


}