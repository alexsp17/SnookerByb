using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Awpbs.Mobile.Droid
{
    public class LocationService_Android : Java.Lang.Object, ILocationService, ILocationListener
    {
        LocationManager locationManager;
        List<EventHandler> eventHandlersToFire = new List<EventHandler>();

        public DateTime? TimeLocationUpdated { get; private set; }
        public Location Location { get; private set; }

        public DateTime? TimeUpdated
        {
            get;
            private set;
        }

        public string FailureInfo
        {
            get
            {
                return failureInfo;
            }
        }
        private string failureInfo;

        public LocationServicesStatusEnum LocationServicesStatus
        {
            get;
            private set;
        }

        public LocationService_Android()
        {
            LocationServicesStatus = LocationServicesStatusEnum.Unknown;
        }

		public Location GetLastKnownLocationQuickly()
		{
			Console.WriteLine("LocationService.GetLastKnownLocationQuickly()");
			this.createLocationManager ();

			try
			{
				var location = locationManager.GetLastKnownLocation (LocationManager.PassiveProvider);
				if (location == null)
					return null;
				return new Location (location.Latitude, location.Longitude);
			}
			catch (Exception)
			{
				return null;
			}
		}

        public void RequestLocationAsync(EventHandler done, bool askPermissionIfNecessary)
        {
			Console.WriteLine("LocationService.RequestLocationAsync()");
			this.createLocationManager ();

            if (done != null)
                 eventHandlersToFire.Add(done);
            getLocation();
        }

        void getLocation()
        {
			this.failureInfo = "";
			DateTime timeInitiated = DateTime.Now;

			// try requesting the location
            try
            {
				string provider = LocationManager.PassiveProvider;//.GpsProvider;

                if (locationManager.IsProviderEnabled(provider))
					locationManager.RequestLocationUpdates (provider, 2000, 0, this);
                else
                    this.failureInfo = "provider not enabled";
			}
			catch (Exception exc)
			{
				TraceHelper.TraceException ("LocationService exception", exc);
				this.failureInfo = "exception requesting location";
			}

			var lastKnownLocation = locationManager.GetLastKnownLocation (LocationManager.PassiveProvider);

			// wait
			var waitTask = new System.Threading.Tasks.Task(() =>
			{
				System.Threading.Thread.Sleep(50);

				if (this.failureInfo != "") {
					this.fireEvents();
					return;
				}

				for (; ; )
				{
					DateTime now = DateTime.Now;
						
					if (TimeLocationUpdated != null && TimeLocationUpdated.Value >= timeInitiated)
					{
						Console.WriteLine("LocationService.getLocation() location updated");
						//note: events already fired
						return;
					}

//					if ((now - timeInitiated).TotalSeconds > 3 && Location != null && TimeLocationUpdated != null && (TimeLocationUpdated.Value - timeInitiated).TotalMinutes < 15)
//					{
//						Console.WriteLine("LocationService.getLocation() locationd didn't update, maybe because it didn't change since the last time.");
//						this.fireEvents();
//						return;
//					}

					if ((now - timeInitiated).TotalSeconds > 3 && lastKnownLocation != null)
					{
						this.Location = new Location(lastKnownLocation.Latitude, lastKnownLocation.Longitude);

						Console.WriteLine("LocationService.getLocation(): Timeout. Can use LastKnownLocation");
						this.failureInfo = "Timeout.";
						this.fireEvents();
						return;
					}
					if ((now - timeInitiated).TotalSeconds > 15)
					{
						Console.WriteLine("LocationService.getLocation(): Timeout. No LastKnownLocation");
						this.failureInfo = "Timeout.";
						this.fireEvents();
						return;
					}
					System.Threading.Thread.Sleep(200);
				}
			});
            waitTask.Start();
        }

		void createLocationManager()
		{
			if (this.locationManager != null)
				return;

			try
			{
				this.locationManager = MainActivity.The.GetSystemService(Context.LocationService) as LocationManager;
			}
			catch (Exception)
			{
				this.locationManager = null;
			}
		}

        void fireEvents()
        {
			if (eventHandlersToFire.Count == 0)
				return;
			
            Device.BeginInvokeOnMainThread(() =>
            {
                var list = eventHandlersToFire.ToList();
                foreach (var e in list)
                {
                    e(this, EventArgs.Empty);
                    eventHandlersToFire.Remove(e);
                }
            });
        }

        //
        // ILocationListener
        //

        public void OnLocationChanged(Android.Locations.Location location)
        {
            this.TimeLocationUpdated = DateTime.Now;
            this.Location = new Location() { Latitude = location.Latitude, Longitude = location.Longitude };
            this.LocationServicesStatus = LocationServicesStatusEnum.Ok;

            Console.WriteLine("OnLocationChanged: " +
                location.Provider.ToString() + " " +
                this.Location.Latitude.ToString() + " " +
                this.Location.Longitude.ToString());

			this.fireEvents ();

			try
			{
				locationManager.RemoveUpdates (this);
			}
			catch (Exception) {
			}
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }
    }
}
