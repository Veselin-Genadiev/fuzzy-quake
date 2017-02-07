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
using System.Windows.Controls.Primitives;
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
        private bool isDrawing = false;
        private Location center;

        private VM viewModel = new VM();
        private EarthQuakeInferenceSystem quakeSystem;


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void StartDrawing(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = "Stop Drawing";

            //Capture the current center of the map. We will use this to lock the map view.
            center = myMap.Center;

            //Add map events
            myMap.MouseLeftButtonDown += MouseTouchStartHandler;
            myMap.TouchDown += MouseTouchStartHandler;
            myMap.MouseMove += MouseTouchMoveHandler;
            myMap.TouchMove += MouseTouchMoveHandler;
            myMap.MouseLeftButtonUp += MouseTouchEndHandler;
            myMap.TouchUp += MouseTouchEndHandler;
            myMap.ViewChangeOnFrame += MyMap_ViewChangeOnFrame;
        }

        private void StopDrawing(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = "Start Drawing";

            //Remove map events
            myMap.MouseLeftButtonDown -= MouseTouchStartHandler;
            myMap.TouchDown += MouseTouchStartHandler;
            myMap.MouseMove -= MouseTouchMoveHandler;
            myMap.TouchMove -= MouseTouchMoveHandler;
            myMap.MouseLeftButtonUp -= MouseTouchEndHandler;
            myMap.TouchUp += MouseTouchEndHandler;
            myMap.ViewChangeOnFrame -= MyMap_ViewChangeOnFrame;
        }

        private void MouseTouchStartHandler(object sender, object e)
        {
            //Optional: Remove any already drawn polygons.
            //MyMap.Children.Clear();

            Location startLoc = GetMouseTouchLocation(e);

            //Get the initial location where the user pressed the mouse down.
            if (startLoc != null)
            {

                //Create a polygon that has four corners, all of which are the starting location.
                mapPolygon.Locations = new LocationCollection()
                {
                    startLoc,
                    startLoc,
                    startLoc,
                    startLoc
                };

                isDrawing = true;
            }
        }

        private void MyMap_ViewChangeOnFrame(object sender, Microsoft.Maps.MapControl.WPF.MapEventArgs e)
        {
            //If drawing keep reseting the center to the original center value when we entered drawing mode. 
            //This will disable panning of the map when we click and drag. 
            myMap.Center = center;

            //Optional: Disable rotation of map, useful when using touch.
            myMap.Heading = 0;
        }

        private void MouseTouchMoveHandler(object sender, object e)
        {
            if (isDrawing)
            {
                Location currentLoc = GetMouseTouchLocation(e);

                //Get the location where muse is.
                if (currentLoc != null)
                {
                    var firstLoc = mapPolygon.Locations[0];

                    //Update locations 1 - 3 of polygon so as to create a rectangle.
                    mapPolygon.Locations[1] = new Location(firstLoc.Latitude, currentLoc.Longitude);
                    mapPolygon.Locations[2] = currentLoc;
                    mapPolygon.Locations[3] = new Location(currentLoc.Latitude, firstLoc.Longitude);
                }
            }
        }

        private void MouseTouchEndHandler(object sender, object e)
        {
            //Update drawing flag so that polygon isn't updated when mouse is moved.
            isDrawing = false;
            viewModel.IsDrawingChecked = false;

            //The rectangle is drawn, grab it's locations and do something with them.
        }

        private Location GetMouseTouchLocation(object e)
        {
            Location loc = null;

            if (e is MouseEventArgs)
            {
                myMap.TryViewportPointToLocation((e as MouseEventArgs).GetPosition(myMap), out loc);
            }
            else if (e is TouchEventArgs)
            {
                myMap.TryViewportPointToLocation((e as TouchEventArgs).GetTouchPoint(myMap).Position, out loc);
            }

            return loc;
        }

        private void btnLoadCsv_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".csv";
            dlg.Filter = "csv files (*.csv)|*.csv|excel files (*.xls,*.xlsx)|*.xls*";

            Nullable<bool> result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filename = dlg.FileName;
                viewModel.CSVPath = filename;
            }
        }

        private void calculateSeismicity_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(viewModel.CSVPath))
            {
                viewModel.LatitudeStart = (float)mapPolygon.Locations.Select(l => l.Latitude).Min();
                viewModel.LongitudeStart = (float)mapPolygon.Locations.Select(l => l.Longitude).Min();
                viewModel.LatitudeEnd = (float)mapPolygon.Locations.Select(l => l.Latitude).Max();
                viewModel.LongitudeEnd = (float)mapPolygon.Locations.Select(l => l.Longitude).Max();

                quakeSystem = new EarthQuakeInferenceSystem(viewModel.Date);
                quakeSystem.ProvideInput(viewModel.CSVPath, viewModel.LatitudeStart, viewModel.LongitudeStart, viewModel.LatitudeEnd, viewModel.LongitudeEnd);
                viewModel.CurrentSeismicity = "Current seismicity: " + quakeSystem.EvaluateSeismicity();
                viewModel.WeekSeismicity = "Week seismicity: " + quakeSystem.EvaluateWeekSeismicity();
                viewModel.MonthSeismicity = "Month seismicity: " + quakeSystem.EvaluateMonthSeismicity();
            }
        }
    }

    public class VM : INotifyPropertyChanged
    {
        public VM()
        {
            Date = DateTime.Now;
        }

        private float longitudeStart;
        public float LongitudeStart
        {
            get
            {
                return longitudeStart;
            }
            set
            {
                longitudeStart = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LongitudeStart"));
                }
            }
        }

        private float latitudeStart;
        public float LatitudeStart
        {
            get
            {
                return latitudeStart;
            }
            set
            {
                latitudeStart = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LatitudeStart"));
                }
            }
        }

        private float longitudeEnd;
        public float LongitudeEnd
        {
            get
            {
                return longitudeEnd;
            }
            set
            {
                longitudeEnd = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LongitudeEnd"));
                }
            }
        }

        private float latitudeEnd;
        public float LatitudeEnd
        {
            get
            {
                return latitudeEnd;
            }
            set
            {
                latitudeEnd = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LatitudeEnd"));
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

        private bool isDrawingChecked;
        public bool IsDrawingChecked
        {
            get
            {
                return isDrawingChecked;
            }
            set
            {
                isDrawingChecked = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDrawingChecked"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
