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
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
