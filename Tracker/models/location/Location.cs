using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracker.models.location
{
    public class Location
    {
        public event LocationUpdateHandler OnUpdate;
        public event LocationChangeHandler OnChange;
        public double latitude { get; private set; }
        public double longitude{ get; private set; }
        public double altitude { get; private set; }
        private double _kiloMetersPerDeg = 111.196672;
        private long _time, _startTime;
        private double _movement = 0;
        private double _totalMovement = 0;
        public double totalMovement { get { return Math.Round(_totalMovement, 2); } private set { } }
        private double _speed = 0;
        public double speed { get { return Math.Round(_speed, 2); } private set { } }
        public double averageSpeed { get; private set; }
        public long runTime { get { return (long)TimeSpan.FromTicks(_time - _startTime).TotalSeconds; } private set { } }


        public Location()
        {

        }
        public Location(double latitude, double longitude, double altitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;
            this._time = this._startTime = DateTime.Now.Ticks;

        }

        public void setFirstLocation(double latitude, double longitude, double altitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;
            this._time = this._startTime = DateTime.Now.Ticks;
        }


        /**
         * Update and calculate movement, avgSpeed, speed 
         * 
         * param name="latitude"
         * param name="longitude"
         * param name="altitude" in meters npm
         * returns true 
         * 
         *
         * 
         */
        public async Task update(double latitude, double longitude, double altitude)
        {
            if (Double.IsNaN(latitude) || Double.IsNaN(longitude))
            {
                return;
            }
            double movement = Math.Sqrt(Math.Pow(latitude - this.latitude, 2) + Math.Pow((Math.Cos(this.latitude * Math.PI / 180) * (longitude - this.longitude)), 2));
            movement *= _kiloMetersPerDeg;
            //jeszcze gorki itp
            movement = Math.Sqrt(Math.Pow(movement, 2) + Math.Pow((altitude - this.altitude) / 1000, 2));

            this.calcSpeed(movement);

            this._movement = movement;
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;


            if (OnUpdate != null)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
 {
     OnUpdate(this, new EventArgs());

 }
 );

                if (OnChange != null && movement != 0)
                {
                    OnChange(this, new EventArgs());
                }
            }
        }
        private void calcSpeed(double movement)
        {

            long time = DateTime.Now.Ticks;
   
            if(time - this._time <= 1)
            {
                return;
            }

            this._speed = movement / TimeSpan.FromTicks(time - this._time).TotalHours;
            this._totalMovement += movement;
            this.averageSpeed = Math.Round(_totalMovement / TimeSpan.FromTicks(time - _startTime).TotalHours, 2);
            this._time = time;


        }


    }
    public delegate void LocationUpdateHandler(Location sender, EventArgs e);
    public delegate void LocationChangeHandler(Location sender, EventArgs e);
}
