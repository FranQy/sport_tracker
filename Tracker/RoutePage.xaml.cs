using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tracker.models.db;
using Tracker.models.location;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class RoutePage : Page
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        private Locator _locator;
        private long _runTimeSeconds = 0;
        private Db _db;
        private Route _route = new Route();
        private PositionStatus _positionStatus = PositionStatus.Disabled;
        private List<Task> allTasks = new List<Task>();
        public RoutePage()
        {
            this.InitializeComponent();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timerTick;


            _db = Db.connect("tracker");
            _locator = new Locator();
            _locator.location.OnUpdate += new LocationUpdateHandler(onLocationUpdated);
            _locator.location.OnChange += new LocationChangeHandler(onLocationChanged);

            _locator.statusChanged += onGpsStatusChanged;
        

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (!e.Handled && Frame.CurrentSourcePageType.FullName == "Tracker.RoutePage")
            {
                e.Handled = true;
            }
        }

        private void timerTick(object sender, object e)
        {
            _runTimeSeconds += 1;
            TimeSpan t = TimeSpan.FromSeconds(_runTimeSeconds);
            time.Text = t.ToString(@"h\:mm\:ss");
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("Navigated to routePage");

        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            
            foreach(var task in allTasks)
            {
                task.Wait();
            }
            Frame.Navigate(typeof(MainPage));
        }
        public void onLocationUpdated(Location location, EventArgs e)
        {
            if (_positionStatus != PositionStatus.Ready)
                return;


            distance.Text = location.totalMovement.ToString();
            altitude.Text = location.altitude.ToString();
            speed.Text = location.speed.ToString();
            avgSpeed.Text = location.averageSpeed.ToString();
        }
        public async void onLocationChanged(Location location, EventArgs e)
        {
          var task =  Task.Run( () =>
            {
                if (_positionStatus != PositionStatus.Ready)
                    return;


                _route.time = location.runTime;//_runTimeSeconds;
                _route.distance = location.totalMovement;
                _route.avgSpeed = location.averageSpeed;
                _db.update(_route).Wait();

                RoutePoints routePoint = new RoutePoints
                {
                    id_parent = _route.id,
                    altitude = location.altitude,
                    latitude = location.latitude,
                    longitude = location.longitude,
                    stamp = _runTimeSeconds,
                    speed = location.speed
                };

                 _db.insert(routePoint).Wait();
            });
            allTasks.Add(task);
        }

        private async void onGpsStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            _positionStatus = e.Status;
            if (_positionStatus == PositionStatus.Ready)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _timer.Start();
                    gpsStatus.Foreground = new SolidColorBrush(Color.FromArgb(100, 135, 169, 20));
                    gpsStatus.Text = "\ue802";
                });
                _route = new Route { stamp = DateTime.Now.Ticks };
                var insertedId = await _db.insert(_route);

                //if (insertedId == 0)
                //{
                  //  _db.insert(_route);
              //  }
            }
            else
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    gpsStatus.Foreground = new SolidColorBrush(Color.FromArgb(100, 202, 0, 0));
                    gpsStatus.Text = "\ue801";
                });
            }
        }


    }
}
