
using ConsoleApp1;
using ProjectOneClasses;
using ProjectOneClasses.Utilities;
using ProjectOneClasses.ValidityCriterias.External;
using ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike;

public class Data
{
    public string Name { get; set; }
    public string Path { get; set; }
    public char Delimiter { get; set; } = ',';
    public ACIDbDeserializer aCIDb { get; set; } = null;
    public IReadOnlyDictionary<int, int> semiSupervised { get; set; } = null;
}
public class Program
{
    public static Data[] datas = new Data[] {
        new Data(){Name = "Iris", Path=@"C:\Users\linh2\Downloads\prj1\iris.data"},
        new Data(){Name = "Iris Shuffled", Path=@"C:\Users\linh2\Downloads\prj1\iris2.data"},
        new Data(){Name = "Wine", Path=@"C:\Users\linh2\Downloads\prj1\wine.data"},
        new Data(){Name = "Statlog(Heart)", Path=@"C:\Users\linh2\Downloads\prj1\heart.dat", Delimiter=' '},
        new Data(){Name = "Vertebral Column(2C)", Path=@"C:\Users\linh2\Downloads\prj1\column_2C.dat", Delimiter=' '},
        new Data(){Name = "Vertebral Column(3C)", Path=@"C:\Users\linh2\Downloads\prj1\column_3C.dat", Delimiter=' '},
        new Data(){Name = "Glass", Path=@"C:\Users\linh2\Downloads\prj1\glass.data"},
        new Data(){Name = "Bezdek Iris", Path=@"C:\Users\linh2\Downloads\prj1\bezdekIris.data"},
    };
    public static void MC_FCM_Validate()
    {
        Console.Clear();
        //Console.SetWindowSize(1900, 980);
        LoadAllExampleData();
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
        foreach (var dataInfo in datas)
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
        foreach (var dataInfo in datas)
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
    public static void FC_SSMC_FCM_Benchmark()
    {
        //Console.SetWindowSize(1900, 980);
        LoadAllExampleData();
        PrintTable pt = new PrintTable(new ColumnStyle[] {
            new ColumnStyle(){Width=20},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width=16},
            new ColumnStyle(){Align=ColumnAlign.Right, Width = 16},
        });

        pt.PrintLine();
        pt.PrintRow("", "FCM (Rand)", "MC-FCM (Rand)", "sSMC-FCM", "FC sSMC FCM");
        pt.PrintLine();
        foreach (var dataInfo in datas)
        {
            var data = dataInfo.aCIDb;
            var fcm = new FCM(data.X, data.C, 2);
            fcm.ClustersGenerationMode = MC_FCM.CGMode.StupidRandom;
            fcm._solve();
            var mc_fcm = new MC_FCM(data.X, data.C);
            mc_fcm.ClustersGenerationMode = MC_FCM.CGMode.StupidRandom;
            mc_fcm._solve();
            var mc_fcm2 = new sSMC_FCM(data.X, data.C, dataInfo.semiSupervised);
            mc_fcm2._solve();
            var mc_fcm3 = new MC_FCM(data.X, data.C);
            mc_fcm3._solve();

            pt.PrintRow(dataInfo.Name, fcm.Result.l, mc_fcm.Result.l, mc_fcm2.Result.l, mc_fcm3.Result.l);

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

            var max_sswc = new[] { fcm_sswc, mc_fcm_sswc, mc_fcm2_sswc, mc_fcm3_sswc }.Max();
            var min_db = new[] { fcm_db, mc_fcm_db, mc_fcm2_db, mc_fcm3_db }.Min();
            var max_pbm = new[] { fcm_pbm, mc_fcm_pbm, mc_fcm2_pbm, mc_fcm3_pbm }.Max();
            var max_accuracy = new[] { fcm_accuracy, mc_fcm_accuracy, mc_fcm2_accuracy, mc_fcm3_accuracy }.Max();
            var max_rand = new[] { fcm_rand, mc_fcm_rand, mc_fcm2_rand, mc_fcm3_rand }.Max();
            var max_jaccard = new[] { fcm_jaccard, mc_fcm_jaccard, mc_fcm2_jaccard, mc_fcm3_jaccard }.Max();

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
                });
            pt.PrintRow("    DB",
                new ColumnContent() { 
                    Content = fcm_db, 
                    Color = (fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White 
                },
                new ColumnContent() { 
                    Content = mc_fcm_db, 
                    Color = (mc_fcm_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White
                },
                new ColumnContent() { 
                    Content = mc_fcm2_db, 
                    Color = (mc_fcm2_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White 
                },
                new ColumnContent() { 
                    Content = mc_fcm3_db, 
                    Color = (mc_fcm3_db == min_db) ? ConsoleColor.Yellow : ConsoleColor.White 
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
                });

            pt.PrintRow("", "", "", "", "", "");
        }
        pt.PrintLine();
        Console.WriteLine();
    }
    public static void BenchMark()
    {
        Console.Clear();
        //Console.SetWindowSize(1900, 980);
        foreach (var dataInfo in datas)
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
        foreach (var dataInfo in datas)
        {
            var data = dataInfo.aCIDb;
            var fcm = new FCM(data.X, data.C, 2);
            fcm.ClustersGenerationMode = MC_FCM.CGMode.StupidRandom;
            fcm._solve();
            var mc_fcm = new MC_FCM(data.X, data.C);
            mc_fcm.ClustersGenerationMode = MC_FCM.CGMode.StupidRandom;
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
    public static void CreateSemiSupervisonOfAllData()
    {
        foreach (var dataInfo in datas)
        {
            dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);
            var gen = new SemiSupervisedGenerator(dataInfo.aCIDb.expect, 20);
            gen.WriteFile(dataInfo.Path + ".ss");
        }
            
    }
    public static void LoadAllExampleData()
    {
        foreach (var dataInfo in datas)
        {
            dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);
            var gen = new SemiSupervisedGenerator(dataInfo.Path + ".ss");
            dataInfo.semiSupervised = gen.semiSupervised;
        }

    }
    static void Main(string[] args)
    {
        FC_SSMC_FCM_Benchmark();
        //BenchMark();
        //Console.ReadLine();
    }


}