using FuzzyQuake.Lib;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuzzyQuake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VM viewModel = new VM();
        private EarthQuakeInferenceSystem quakeSystem = new EarthQuakeInferenceSystem();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void myMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Point mousePosition = e.GetPosition(this);
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            viewModel.Latitude = (float)pinLocation.Latitude;
            viewModel.Longitude = (float)pinLocation.Longitude;

            myPushPin.Location = pinLocation;
        }

        private void btnLoadCsv_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".csv";
            dlg.Filter = "csv files (*.csv)|*.csv|excel files (*.xls,*.xlsx)|*.xls*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value)
            {
                // Open document 
                string filename = dlg.FileName;
                viewModel.CSVPath = filename;
            }
        }

        private void calculateSeismicity_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(viewModel.CSVPath))
            {
                quakeSystem.ProvideInput(viewModel.CSVPath, viewModel.Latitude, viewModel.Longitude);
                viewModel.CurrentSeismicity = "Current seismicity: " + quakeSystem.EvaluateSeismicity();
                viewModel.WeekSeismicity = "Week seismicity: " + quakeSystem.EvaluateWeekSeismicity();
                viewModel.MonthSeismicity = "Month seismicity: " + quakeSystem.EvaluateMonthSeismicity();
                viewModel.SixMonthsSeismicity = "Six months seismicity: " + quakeSystem.EvaluateSixMonthsSeismicity();
            }
        }
    }

    public class VM : INotifyPropertyChanged
    {
        public VM()
        {
            Date = DateTime.Now;
        }

        private float longitude;
        public float Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Longitude"));
                }
            }
        }

        private float latitude;
        public float Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Latitude"));
                }
            }
        }


        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Date"));
                }
            }
        }

        private string csvPath;
        public string CSVPath
        {
            get
            {
                return csvPath;
            }
            set
            {
                csvPath = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CSVPath"));
                }
            }
        }

        private string currentSeismicity;
        public string CurrentSeismicity
        {
            get
            {
                return currentSeismicity;
            }
            set
            {
                currentSeismicity = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentSeismicity"));
                }
            }
        }

        private string weekSeismicity;
        public string WeekSeismicity
        {
            get
            {
                return weekSeismicity;
            }
            set
            {
                weekSeismicity = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("WeekSeismicity"));
                }
            }
        }

        private string monthSeismicity;
        public string MonthSeismicity
        {
            get
            {
                return monthSeismicity;
            }
            set
            {
                monthSeismicity = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("MonthSeismicity"));
                }
            }
        }

        private string sixMonthsSeismicity;
        public string SixMonthsSeismicity
        {
            get
            {
                return sixMonthsSeismicity;
            }
            set
            {
                sixMonthsSeismicity = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SixMonthsSeismicity"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
