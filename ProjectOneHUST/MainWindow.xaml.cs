using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ProjectOneClasses;
using ProjectOneClasses.Utilities;
using ProjectOneClasses.ValidityCriterias.External;
using ProjectOneClasses.ValidityCriterias.Relative.OptimizationLike;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProjectOneHUST
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int algorithm = 0;
        ObservableCollection<ValidationResultItem> ValidationResults;
        public MainWindow()
        {
            InitializeComponent();
            ValidationResults = new ObservableCollection<ValidationResultItem>();
            Validation_Result.ItemsSource = ValidationResults;
            Overview_Panel.Visibility = Visibility.Visible;
            Result_Panel.Visibility = Visibility.Collapsed;
        }
        ACIDbDeserializer aCIDb = null;
        private void Button_Click_ImportInputFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "aci db (*.data;*.dat)|*.data;*.dat|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "DataSet");
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    aCIDb = new ACIDbDeserializer(openFileDialog.FileName);
                    InputFilePathTxt.Text = openFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MaterialDesignThemes.Wpf.DialogHost
                        .Show("Đầu vào phải là file aci database với các chiều của các điểm cách nhau bởi dấu phẩy `,`"
                            + " hoặc dấu khoảng trắng ` `.");
                }
            }

        }

        private void RadioButton_Checked_SelectAlgorithms(object sender, RoutedEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            string context = btn.DataContext.ToString();
            algorithm = Convert.ToInt32(context);
        }

        private void Button_Click_RunAlgorithm(object sender, RoutedEventArgs e)
        {

            if(aCIDb == null)
            {
                MaterialDesignThemes.Wpf.DialogHost
                        .Show("Vui lòng chọn file đầu vào.");
                return;
            }

            Button btn = (Button)sender;
            ButtonProgressAssist.SetIsIndicatorVisible(btn, true);
            btn.IsEnabled = false;
            LoadingBar.Visibility = Visibility.Visible;
            Thread thread = null;
            if (algorithm == 0)
            {
                //FCM-------------------------------------------------------------
                //----------------------------------------------------------------
                MC_FCM.CGMode cGMode = MC_FCM.CGMode.StupidRandom;
                if (ClusterGenOptions_FCM.SelectedIndex == 1)
                {
                    cGMode = MC_FCM.CGMode.MaxFuzzificationCoefficientGroups;
                }
                else if (ClusterGenOptions_FCM.SelectedIndex == 2)
                {
                    cGMode = MC_FCM.CGMode.KMeanPlusPlus;
                }
                thread = new Thread(new ParameterizedThreadStart(a =>
                {
                    lock (this.aCIDb)
                    {
                        var data = this.aCIDb;
                        var fcm = new FCM(data.X, data.C, 2);
                        fcm.ClustersGenerationMode = cGMode;
                        fcm._solve();
                        CycleTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            CycleTxt.Text = fcm.Result.l.ToString();
                        }));
                        StringBuilder sbClustersTxt = new StringBuilder();
                        for (var i = 0; i < fcm.Result.V.Count; i++)
                        {
                            if (i != 0)
                                sbClustersTxt.AppendLine();
                            var cluster = fcm.Result.V[i];
                            foreach (var vi in cluster)
                            {
                                sbClustersTxt.Append(vi.ToString("N6") + ", ");
                            }
                            if (cluster.Length > 0)
                            {
                                sbClustersTxt.Remove(sbClustersTxt.Length - 2, 2);
                            }
                        }
                        ClustersTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            ClustersTxt.Text = sbClustersTxt.ToString();
                        }));
                        var fcm_sswc = new SSWC(data.X, data.C, fcm.Result).Index;
                        var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
                        var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
                        var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ValidationResults.Clear();
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "SSWC",
                                Index = fcm_sswc
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "DB",
                                Index = fcm_db
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "PBM",
                                Index = fcm_pbm
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Accuracy",
                                Index = fcm_accuracy
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Rand",
                                Index = fcm_rand
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Jaccard",
                                Index = fcm_jaccard
                            });
                        }));


                        btn.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonProgressAssist.SetIsIndicatorVisible(btn, false);
                            btn.IsEnabled = true;
                        }));
                        LoadingBar.Dispatcher.Invoke(new Action(() =>
                        {
                            LoadingBar.Visibility = Visibility.Hidden;
                        }));

                        Overview_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Overview_Panel.Visibility = Visibility.Collapsed;
                        }));
                        Result_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Result_Panel.Visibility = Visibility.Visible;
                        }));
                    }
                }));
            }
            else if (algorithm == 1)
            {
                //MC_FCM-------------------------------------------------------------
                //----------------------------------------------------------------
                MC_FCM.CGMode cGMode = MC_FCM.CGMode.StupidRandom;
                if (ClusterGenOptions_MC_FCM.SelectedIndex == 1)
                {
                    cGMode = MC_FCM.CGMode.MaxFuzzificationCoefficientGroups;
                }
                else if (ClusterGenOptions_FCM.SelectedIndex == 2)
                {
                    cGMode = MC_FCM.CGMode.KMeanPlusPlus;
                }
                thread = new Thread(new ParameterizedThreadStart(a =>
                {
                    lock (this.aCIDb)
                    {
                        var data = this.aCIDb;
                        var fcm = new MC_FCM(data.X, data.C);
                        fcm.ClustersGenerationMode = cGMode;
                        fcm._solve();
                        CycleTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            CycleTxt.Text = fcm.Result.l.ToString();
                        }));
                        StringBuilder sbClustersTxt = new StringBuilder();
                        for (var i = 0; i < fcm.Result.V.Count; i++)
                        {
                            if (i != 0)
                                sbClustersTxt.AppendLine();
                            var cluster = fcm.Result.V[i];
                            foreach (var vi in cluster)
                            {
                                sbClustersTxt.Append(vi.ToString("N6") + ", ");
                            }
                            if (cluster.Length > 0)
                            {
                                sbClustersTxt.Remove(sbClustersTxt.Length - 2, 2);
                            }
                        }
                        ClustersTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            ClustersTxt.Text = sbClustersTxt.ToString();
                        }));
                        var fcm_sswc = new SSWC(data.X, data.C, fcm.Result).Index;
                        var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
                        var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
                        var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ValidationResults.Clear();
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "SSWC",
                                Index = fcm_sswc
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "DB",
                                Index = fcm_db
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "PBM",
                                Index = fcm_pbm
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Accuracy",
                                Index = fcm_accuracy
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Rand",
                                Index = fcm_rand
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Jaccard",
                                Index = fcm_jaccard
                            });
                        }));


                        btn.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonProgressAssist.SetIsIndicatorVisible(btn, false);
                            btn.IsEnabled = true;
                        }));
                        LoadingBar.Dispatcher.Invoke(new Action(() =>
                        {
                            LoadingBar.Visibility = Visibility.Hidden;
                        }));

                        Overview_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Overview_Panel.Visibility = Visibility.Collapsed;
                        }));
                        Result_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Result_Panel.Visibility = Visibility.Visible;
                        }));
                    }
                }));
            }
            else if (algorithm == 2)
            {
                //sSMC_FCM-------------------------------------------------------------
                //----------------------------------------------------------------
                int percent = (int)checked(SupervisionDegreeInput_sSMC.Value);
                int x = aCIDb.expect.Count * percent / 100;
                var gen = new SemiSupervisedGenerator(aCIDb.expect, x);
                var semiSupervised = gen.semiSupervised;
                thread = new Thread(new ParameterizedThreadStart(a =>
                {
                    lock (this.aCIDb)
                    {
                        var data = this.aCIDb;
                        var fcm = new sSMC_FCM(data.X, data.C, semiSupervised);
                        fcm._solve();
                        CycleTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            CycleTxt.Text = fcm.Result.l.ToString();
                        }));
                        StringBuilder sbClustersTxt = new StringBuilder();
                        for (var i = 0; i < fcm.Result.V.Count; i++)
                        {
                            if (i != 0)
                                sbClustersTxt.AppendLine();
                            var cluster = fcm.Result.V[i];
                            foreach (var vi in cluster)
                            {
                                sbClustersTxt.Append(vi.ToString("N6") + ", ");
                            }
                            if (cluster.Length > 0)
                            {
                                sbClustersTxt.Remove(sbClustersTxt.Length - 2, 2);
                            }
                        }
                        ClustersTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            ClustersTxt.Text = sbClustersTxt.ToString();
                        }));
                        var fcm_sswc = new SSWC(data.X, data.C, fcm.Result).Index;
                        var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
                        var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
                        var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ValidationResults.Clear();
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "SSWC",
                                Index = fcm_sswc
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "DB",
                                Index = fcm_db
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "PBM",
                                Index = fcm_pbm
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Accuracy",
                                Index = fcm_accuracy
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Rand",
                                Index = fcm_rand
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Jaccard",
                                Index = fcm_jaccard
                            });
                        }));


                        btn.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonProgressAssist.SetIsIndicatorVisible(btn, false);
                            btn.IsEnabled = true;
                        }));
                        LoadingBar.Dispatcher.Invoke(new Action(() =>
                        {
                            LoadingBar.Visibility = Visibility.Hidden;
                        }));

                        Overview_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Overview_Panel.Visibility = Visibility.Collapsed;
                        }));
                        Result_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Result_Panel.Visibility = Visibility.Visible;
                        }));
                    }
                }));
            }
            else if (algorithm == 3)
            {
                //FC_sSMC_FCM-------------------------------------------------------------
                //----------------------------------------------------------------
                int percent = (int)checked(SupervisionDegreeInput_MC_sSMC.Value);
                int x = aCIDb.expect.Count * percent / 100;
                var gen = new SemiSupervisedGenerator(aCIDb.expect, x);
                var semiSupervised = gen.semiSupervised;
                thread = new Thread(new ParameterizedThreadStart(a =>
                {
                    lock (this.aCIDb)
                    {
                        var data = this.aCIDb;
                        var fcm = new MC_sSMC_FCM(data.X, data.C, semiSupervised);
                        fcm._solve();
                        CycleTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            CycleTxt.Text = fcm.Result.l.ToString();
                        }));
                        StringBuilder sbClustersTxt = new StringBuilder();
                        for (var i = 0; i < fcm.Result.V.Count; i++)
                        {
                            if (i != 0)
                                sbClustersTxt.AppendLine();
                            var cluster = fcm.Result.V[i];
                            foreach (var vi in cluster)
                            {
                                sbClustersTxt.Append(vi.ToString("N6") + ", ");
                            }
                            if (cluster.Length > 0)
                            {
                                sbClustersTxt.Remove(sbClustersTxt.Length - 2, 2);
                            }
                        }
                        ClustersTxt.Dispatcher.Invoke(new Action(() =>
                        {
                            ClustersTxt.Text = sbClustersTxt.ToString();
                        }));
                        var fcm_sswc = new SSWC(data.X, data.C, fcm.Result).Index;
                        var fcm_db = new DB(data.X, data.C, fcm.Result).Index;
                        var fcm_pbm = new PBM(data.X, data.C, fcm.Result).Index;
                        var fcm_accuracy = new Accuracy(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_rand = new Rand(data.X, data.C, data.expect, fcm.Result).Index;
                        var fcm_jaccard = new Jaccard(data.X, data.C, data.expect, fcm.Result).Index;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ValidationResults.Clear();
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "SSWC",
                                Index = fcm_sswc
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "DB",
                                Index = fcm_db
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "PBM",
                                Index = fcm_pbm
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Accuracy",
                                Index = fcm_accuracy
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Rand",
                                Index = fcm_rand
                            });
                            ValidationResults.Add(new ValidationResultItem()
                            {
                                Name = "Jaccard",
                                Index = fcm_jaccard
                            });
                        }));


                        btn.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonProgressAssist.SetIsIndicatorVisible(btn, false);
                            btn.IsEnabled = true;
                        }));
                        LoadingBar.Dispatcher.Invoke(new Action(() =>
                        {
                            LoadingBar.Visibility = Visibility.Hidden;
                        }));

                        Overview_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Overview_Panel.Visibility = Visibility.Collapsed;
                        }));
                        Result_Panel.Dispatcher.Invoke(new Action(() =>
                        {
                            Result_Panel.Visibility = Visibility.Visible;
                        }));
                    }
                }));
            }

            if (thread != null)
            {
                thread.Start();
            }
            else
            {
                ButtonProgressAssist.SetIsIndicatorVisible(btn, false);
                btn.IsEnabled = true;
                LoadingBar.Visibility = Visibility.Hidden;
            }

        }

        private void Button_Click_CopyClustersInfo(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ClustersTxt.Text);
        }

        private void Button_Click_CopyContext(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string s = button.DataContext.ToString();
            Clipboard.SetText(s.Remove(s.IndexOf('.') + 7));
        }
    }
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
