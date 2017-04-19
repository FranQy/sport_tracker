using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tracker.models.db;
using Tracker.models.location;
using Tracker.models.routes;
using Tracker.models.Fb;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Db _db;
        private Images _images;


        public MainPage()
        {

            this.InitializeComponent();
            _db = Db.connect("tracker");
            this.NavigationCacheMode = NavigationCacheMode.Required;

            _images = new Images(new HttpClient());






        }

        private async void history_Loaded(object sender, RoutedEventArgs e)
        {
            var historyCollection = new ObservableCollection<routesListItem>();
            history.ItemsSource = historyCollection;


            var routes = _db.getAll<Route>("SELECT * FROM Route ORDER BY id DESC").Result;
            if (routes != null)
            {
                foreach (Route route in routes)
                {

                    var points = _db.getAll<RoutePoints>("SELECT * FROM RoutePoints WHERE id_parent=" + route.id.ToString()).Result;
                    if (points == null)
                    {
                        continue;
                    }
                    DateTime stamp = new DateTime(route.stamp);
                    TimeSpan time = new TimeSpan(0, 0, (int)route.time);

   
                    string timeText = "";
                    if (time.Hours > 0) timeText += time.Hours + "h";
                    if (time.Minutes > 0) timeText += time.Minutes + "min";
                    timeText += time.Seconds + "s";

                    string image = await _images.getUrl(points);
                    if (image == "")
                    {
                        continue;
                    }

                    try
                    {

                     
                        var routeItem = new routesListItem
                        {
                            id = route.id,
                            stamp = stamp.ToString("d.M.yyy H:m"),
                            image = new BitmapImage(new Uri(image, UriKind.Absolute)),
                            time = timeText,
                            speed = route.avgSpeed.ToString() + "km/h",
                            distance = route.distance.ToString() + "km",
                        };
                        historyCollection.Add(routeItem);
                    }
                    catch   { }

                }
            }


        }

        //private async void downloadImages()
        //{
        //    //  new Task(() =>
        //    //   {// image = '' or image is null
        //    var routesWithoutImages = _db.getAll<Route>("SELECT * FROM Route WHERE 1 or image is null or image = ''").Result;
        //    if (routesWithoutImages != null)
        //    {
        //        foreach (Route route in routesWithoutImages)
        //        {
        //            var points = _db.getAll<RoutePoints>("SELECT * FROM RoutePoints WHERE id_parent=" + route.id.ToString()).Result;
        //            string fileName = _images.download(points).Result;

        //            route.image = fileName;
        //            _db.update(route).Wait();
        //            //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //            //{
        //            // //   Image.SourceProperty = fileName;
        //            //});
        //        }
        //    }

        //    //   }).Start();

        //}

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //   Uri _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

            //     this.downloadImages();
            // this.createRoutesList();
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RoutePage));
        }




        private void history_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem as routesListItem;
            if (item != null)
            {
                Frame.Navigate(typeof(RouteView), new RouteViewPassedData { id = item.id });
            }
        }



    }
}
