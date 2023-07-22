using ProjectOneClasses.Utilities;
using ProjectOneClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Data
    {
        static string DataSetFolder = System.IO.Path.GetFullPath("DataSet");
        public static Data[] datas = new Data[] {
            new Data(){Name = "Iris", Path=$"{DataSetFolder}\\iris.data"},
            new Data(){Name = "Iris Shuffled", Path=$"{DataSetFolder}\\iris2.data"},
            new Data(){Name = "Wine", Path=$"{DataSetFolder}\\wine.data"},
            new Data(){Name = "Statlog(Heart)", Path=$"{DataSetFolder}\\heart.dat", Delimiter=' '},
            new Data(){Name = "Vertebral Column(2C)", Path=$"{DataSetFolder}\\column_2C.dat", Delimiter=' '},
            new Data(){Name = "Vertebral Column(3C)", Path=$"{DataSetFolder}\\column_3C.dat", Delimiter=' '},
            new Data(){Name = "Glass", Path=$"{DataSetFolder}\\glass.data"},
            new Data(){Name = "Bezdek Iris", Path=$"{DataSetFolder}\\bezdekIris.data"},
        };
        public static void CreateSemiSupervisonOfAllData()
        {
            foreach (var dataInfo in datas)
            {
                dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);
                for (int i = 1; i < 5; i++)
                {
                    int v = i * 5;
                    int x = dataInfo.aCIDb.expect.Count * v / 100;
                    var gen = new SemiSupervisedGenerator(dataInfo.aCIDb.expect, x);
                    gen.WriteFile(dataInfo.Path + $".p{v}.ss");
                }
                var gen100 = new SemiSupervisedGenerator(dataInfo.aCIDb.expect, dataInfo.aCIDb.expect.Count);
                gen100.WriteFile(dataInfo.Path + $".p100.ss");
            }

        }

        public static void Create_BestMCFCM_SemiSupervisonOfAllData()
        {
            foreach (var dataInfo in datas)
            {
                dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);
                var mc_fcm = new MC_FCM(dataInfo.aCIDb.X, dataInfo.aCIDb.C);
                mc_fcm._solve();
                var gen = new ChooseBestAccuracyPointForSemiSupervision(dataInfo.aCIDb.X, dataInfo.aCIDb.C, dataInfo.aCIDb.expect, mc_fcm.Result);
                gen.SaveAsSemiSupervisionFile(new int[] { 5, 10, 15, 20 }, System.IO.Path.GetFileName(dataInfo.Path), System.IO.Path.GetDirectoryName(dataInfo.Path));
            }

        }

        public static void LoadAllExampleData()
        {
            foreach (var dataInfo in datas)
            {
                dataInfo.aCIDb = new ACIDbDeserializer(dataInfo.Path, dataInfo.Delimiter);
                var gen = new SemiSupervisedGenerator(dataInfo.Path + ".p5.ss");
                dataInfo.semiSupervised5 = gen.semiSupervised;
                gen = new SemiSupervisedGenerator(dataInfo.Path + ".p10.ss");
                dataInfo.semiSupervised10 = gen.semiSupervised;
                gen = new SemiSupervisedGenerator(dataInfo.Path + ".p15.ss");
                dataInfo.semiSupervised15 = gen.semiSupervised;
                gen = new SemiSupervisedGenerator(dataInfo.Path + ".p20.ss");
                dataInfo.semiSupervised20 = gen.semiSupervised;
                gen = new SemiSupervisedGenerator(dataInfo.Path + ".p100.ss");
                dataInfo.semiSupervised100 = gen.semiSupervised;
            }

        }


        public string Name { get; set; }
        public string Path { get; set; }
        public char Delimiter { get; set; } = ',';
        public ACIDbDeserializer aCIDb { get; set; } = null;
        public static IReadOnlyDictionary<int, int> semiSupervised0 { get; private set; } = new Dictionary<int, int>();
        public IReadOnlyDictionary<int, int> semiSupervised5 { get; set; } = null;
        public IReadOnlyDictionary<int, int> semiSupervised10 { get; set; } = null;
        public IReadOnlyDictionary<int, int> semiSupervised15 { get; set; } = null;
        public IReadOnlyDictionary<int, int> semiSupervised20 { get; set; } = null;
        public IReadOnlyDictionary<int, int> semiSupervised100 { get; set; } = null;
    }
}
