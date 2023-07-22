
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
using ProjectOneClasses.ResultTypes;
using ConsoleApp1.Tests;
using System.Diagnostics;
using ProjectOneClasses.Trials;

public class Program
{
    public static void ViewX()
    {
        {
            var dataInfo = Data.datas.FirstOrDefault(t => t.Name == "Iris");
            dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);

            //foreach(var x in  dataInfo.aCIDb.X)
            //{
            //    Console.WriteLine(x.Print());
            //}
            var data = dataInfo.aCIDb;
            var mc_fcm5 = new sSMC_FCM_Trial(data.X, data.C, 100, data.expect, 100);
            mc_fcm5._solve();
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

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        //Data.CreateSemiSupervisonOfAllData();
        //Data.Create_BestMCFCM_SemiSupervisonOfAllData();
        //MC_FCM_Tests.FCM_DiffGenClusters_Benchmark();
        //MC_FCM_Tests.MC_FCM_DiffGenClusters_Benchmark();
        //MC_FCM_Tests.MC_SSMC_FCM_Benchmark();
        //MC_FCM_Tests.FCM_And_MC_FCM_BenchMark();
        sSMC_FCM_Tests.SSMC_FCM_WithTrial_Benchmark();
        //sSMC_FCM_Tests.SSMC_FCM_Benchmark();
        //sSMC_FCM_Tests.SSMC_FCM_VectorM2_Benchmark();
        //MC_sSMC_FCM_Tests.MC_SSMC_FCM_WithTrial_Benchmark();
        //MC_sSMC_FCM_Tests.MC_SSMC_FCM_Benchmark();
        //CustomBenchmark_1();

        //ViewX();


        stopwatch.Stop();
        Console.WriteLine("Elapsed Time: {0} s", stopwatch.ElapsedMilliseconds / 1000.0);

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