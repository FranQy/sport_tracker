using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.System;

namespace Tracker.models.location
{
    public class Locator
    {
        private Geolocator _geolocator;
        public Location location { get; private set; }
        public event StatusChanged statusChanged; 

        public Locator()
        {
            _geolocator = new Geolocator();
            _geolocator.DesiredAccuracy = PositionAccuracy.High;
            _geolocator.MovementThreshold = 0;
            _geolocator.ReportInterval = 500;

            location = new Location();

            var result = this.connect();

        }



        private void onStatusChange(Geolocator sender, StatusChangedEventArgs args)
        {
            if(statusChanged != null)
            {
                statusChanged(sender, args);
            }
        }

        private async Task<int> connect()
        {


            try
            {
                Geoposition geoposition = await _geolocator.GetGeopositionAsync(
                      maximumAge: TimeSpan.FromSeconds(10),
                      timeout: TimeSpan.FromSeconds(5));
                location.setFirstLocation(geoposition.Coordinate.Point.Position.Latitude, geoposition.Coordinate.Point.Position.Longitude, geoposition.Coordinate.Point.Position.Altitude);
            }
            catch (Exception ex)
            {

                await Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));

                return 0;
            }
            _geolocator.PositionChanged += onChange;
            _geolocator.StatusChanged += onStatusChange;

            return 1;
        }


        public async void onChange(Geolocator locator, PositionChangedEventArgs oEventArgs)
        {
            double latitude = oEventArgs.Position.Coordinate.Point.Position.Latitude;
            double longitude = oEventArgs.Position.Coordinate.Point.Position.Longitude;
            double altitude = oEventArgs.Position.Coordinate.Point.Position.Altitude;
            await location.update(latitude, longitude, altitude);


        }
        public delegate void StatusChanged(Geolocator sender, StatusChangedEventArgs e);
    }

}
