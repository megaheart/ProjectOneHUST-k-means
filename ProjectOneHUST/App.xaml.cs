using ProjectOneClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using ProjectOneClasses.Utilities;
using System.Windows.Markup;
using ProjectOneClasses.ValidityCriterias.External;

namespace ProjectOneHUST
{
    public class IntSmallerComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y - x;
        }
    }
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //var X = new double[7][]
            //{
            //    new double[2]{6, 6},
            //    new double[2]{5.5, 7},
            //    new double[2]{5.4, 5.4},
            //    new double[2]{0, 0},
            //    new double[2]{3, 3},
            //    new double[2]{1, 1},
            //    new double[2]{0.6, -1.2},
            //};
            //var m = new FCM(X, 2, 2);
            //m._solve();
            //m.Result.printToConsole();

            var dataDic = new Dictionary<string, ACIDbDeserializer>{
                ["iris.data"] = new ACIDbDeserializer(@"C:\Users\linh2\Downloads\prj1\iris.data"),
                ["iris2.data"] = new ACIDbDeserializer(@"C:\Users\linh2\Downloads\prj1\iris2.data") 
            };

            var modes = Enum.GetValues<MC_FCM.CGMode>();
            foreach (var pair in dataDic)
            {
                var data = pair.Value;
                foreach(var mode in modes)
                {
                    var m = new MC_FCM(data.X, data.C);
                    m.ClustersGenerationMode = mode;
                    m._solve();
                    var validation = new Accuracy(data.X, data.C, data.expect, m.Result);
                    int n = data.X.Count;
                    //double asExpectRate = 100.0 * validation.Result.AsExpect / n;
                    //double notAsExpectRate = 100.0 * validation.Result.NotAsExpect / n;
                    //Console.ForegroundColor = ConsoleColor.DarkBlue;
                    //Console.Write("MC_FCM ");
                    //Console.ForegroundColor = ConsoleColor.White;
                    //Console.Write("[db: ", pair.Key, mode);
                    //Console.ForegroundColor = ConsoleColor.Magenta;
                    //Console.Write(pair.Key);
                    //Console.ForegroundColor = ConsoleColor.White;
                    //Console.Write(", GCMode: ", pair.Key, mode);
                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.Write(mode);
                    //Console.ForegroundColor = ConsoleColor.White;
                    //Console.WriteLine("]: ");
                    //Console.Write("cycle Count: {0, 4}, ", m.Result.l);
                    //Console.Write("as Expect: {0, 10} ~ {1, 10:F6}%, ", validation.Result.AsExpect, asExpectRate);
                    //Console.WriteLine("not as Expect: {0, 10} ~ {1, 10:F6}%", validation.Result.NotAsExpect, notAsExpectRate);
                }
            }
            foreach (var pair in dataDic)
            {
                var data = pair.Value;
                foreach (var mode in modes)
                {
                    var m = new FCM(data.X, data.C, 2);
                    m.ClustersGenerationMode = mode;
                    m._solve();
                    var validation = new Accuracy(data.X, data.C, data.expect, m.Result);
                    int n = data.X.Count;
                    double asExpectRate = 100.0 * validation.Result.AsExpect / n;
                    double notAsExpectRate = 100.0 * validation.Result.NotAsExpect / n;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("FCM ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("[db: ", pair.Key, mode);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(pair.Key);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(", GCMode: ", pair.Key, mode);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(mode);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("]: ");
                    Console.Write("cycle Count: {0, 4}, ", m.Result.l);
                    Console.Write("as Expect: {0, 10} ~ {1, 10:F6}%, ", validation.Result.AsExpect, asExpectRate);
                    Console.WriteLine("not as Expect: {0, 10} ~ {1, 10:F6}%", validation.Result.NotAsExpect, notAsExpectRate);
                }
            }
            App.Current.Shutdown();

            //MainWindow = new MainWindow();
            //MainWindow.Show();
        }
    }
}
