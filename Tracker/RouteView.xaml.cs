using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tracker.models.db;
using Tracker.models.Fb;
using Tracker.models.routes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteView : Page
    {
        private Db _db;
        private Images _images;
        private Fb _fb = new Fb();
        private Route _route;
        private List<RoutePoints> _points;
        private string _img;

        public RouteView()
        {
            this.InitializeComponent();
            _db = Db.connect("tracker");
            _images = new Images(new HttpClient());

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (!e.Handled && Frame.CurrentSourcePageType.FullName == "Tracker.RouteView")
            {
                e.Handled = true;
                Frame.Navigate(typeof(MainPage));
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var data = e.Parameter as RouteViewPassedData;
            await getData(data.id);
            assemblyChart();
            createView();
        }

        private async Task<bool> getData(int id)
        {
            _route = _db.getRow<Route>("SELECT * FROM Route WHERE id=" + id.ToString() + " ORDER BY id DESC").Result;
            if (_route == null)
                return false;

            _points = _db.getAll<RoutePoints>("SELECT * FROM RoutePoints WHERE id_parent=" + _route.id.ToString()).Result;
            _img = await _images.getUrl(_points);
            return true;
        }

        private void assemblyChart()
        {
            List<ChartPoint> chartPointsLatitude = new List<ChartPoint>();
            List<ChartPoint> chartPointsSpeed = new List<ChartPoint>();


            foreach (var point in _points)
            {
                chartPointsLatitude.Add(new ChartPoint { val = (int)point.latitude, stamp = point.stamp });
                chartPointsSpeed.Add(new ChartPoint { val = (int)point.speed, stamp = point.stamp });
            }
            try
            {
                (chartLatitude.Series[0] as LineSeries).ItemsSource = chartPointsLatitude;
            }
            catch { }

            try
            {
                (chartSpeed.Series[0] as LineSeries).ItemsSource = chartPointsSpeed;
            }
            catch { }
        }

        private void createView()
        {

            var imageUri = new Uri(_img, UriKind.Absolute);
            BitmapImage bmi = new BitmapImage();
            bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmi.UriSource = imageUri;
            map.Source = bmi;


            TimeSpan time = new TimeSpan(0, 0, (int)_route.time);

            string timeText = "";
            if (time.Hours > 0) timeText += time.Hours + "h";
            if (time.Minutes > 0) timeText += time.Minutes + "min";
            timeText += time.Seconds + "s";

            distance.Text = _route.distance.ToString() + "km";
            avgSpeed.Text = _route.avgSpeed.ToString() + "km/h";
            timeOf.Text = timeText;
        }
        private void fbBtn_Click(object sender, RoutedEventArgs e)
        {
            _fb.share(new PostParams { description = "BestTracker", link = _img, caption = "BestTracker", picture = _img, name = "BestTracker" });
        }
    }
    public class ChartPoint
    {
        public int val { get; set; }
        public long stamp { get; set; }
    }

    public class RouteViewPassedData
    {
        public int id;
    }
}
