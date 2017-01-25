using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLocation;
using UIKit;
using Xamarin.Forms;

namespace Awpbs.Mobile.iOS
{
    public class LocationService_iOS : ILocationService
    {
        public Location Location
        {
            get;
            private set;
        }

        public DateTime? TimeUpdated
        {
            get;
            private set;
        }

        public string FailureInfo
        {
            get;
            private set;
        }

        public LocationServicesStatusEnum LocationServicesStatus
        {
            get;
            private set;
        }

        CoreLocation.CLLocationManager locationManager;
        List<EventHandler> eventHandlersToFire = new List<EventHandler>();

        public LocationService_iOS()
        {
            LocationServicesStatus = LocationServicesStatusEnum.Unknown;
        }

        public Location GetLastKnownLocationQuickly()
        {
            return null; // not available on iOS as far as I know. investigate!
        }

        public void RequestLocationAsync(EventHandler done, bool askPermissionIfNecessary)
        {
            if (done != null)
                eventHandlersToFire.Add(done);
            requestLocationAsync(true);
        }

        void requestLocationAsync(bool askPermissionIfNecessary)
        {
            // is not authorized?
            if (CoreLocation.CLLocationManager.Status == CLAuthorizationStatus.Restricted ||
                CoreLocation.CLLocationManager.Status == CLAuthorizationStatus.Denied)
            {
                Task.Run(() =>
                {
                    Location = null; // location is not known
                    LocationServicesStatus = LocationServicesStatusEnum.NotAuthorized;
                    FailureInfo = "Not authorized to use location services";
                    fireEvents();
                });
                return;
            }

            // initialize
            initLocationManager();

            // request permission
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0) == true &&
                CoreLocation.CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
            {
                if (askPermissionIfNecessary == false)
                    return; // don't do anything if the caller doesn't want to ask for permission

                locationManager.AuthorizationChanged += (s, e) =>
                {
                    if (e.Status == CLAuthorizationStatus.NotDetermined)
                        return; // in progress

                    if (e.Status == CLAuthorizationStatus.Authorized || e.Status == CLAuthorizationStatus.AuthorizedAlways || e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
                    {
                        LocationServicesStatus = LocationServicesStatusEnum.Ok;
                        this.tryCheckingNow();
                    }
                    else
                    {
                        Location = null; // location is not known
                        LocationServicesStatus = LocationServicesStatusEnum.NotAuthorized;
                        FailureInfo = "Permission to use use location services not given";
                        fireEvents();
                    }
                };
                locationManager.RequestWhenInUseAuthorization();
                return;
            }

            // check location
            this.tryCheckingNow();
        }

        void tryCheckingNow()
        {
            if (CLLocationManager.LocationServicesEnabled == false)
            {
                Location = null; // location is not known
                LocationServicesStatus = LocationServicesStatusEnum.NotAuthorized;
                FailureInfo = "Not authorized to use location services (2)";
                fireEvents();
                return;
            }

            DateTime timeStarted = DateTime.Now;
            Task.Run(async () =>
            {
                for (; ; )
                {
                    await Task.Delay(100);

                    if (TimeUpdated != null && TimeUpdated.Value >= timeStarted)
                    {
                        // stop listening
                        // note: events had already been fired
                        locationManager.StopUpdatingLocation();
                        return;
                    }

                    if (TimeUpdated != null && (DateTime.Now - TimeUpdated.Value).TotalMinutes < 5 && Location != null)
                    {
                        // location already known. no need to wait.
                        fireEvents();
                        return;
                    }

                    if ((DateTime.Now - timeStarted).TotalSeconds > 5)
                    {
                        FailureInfo = "Timeout";
                        fireEvents();
                        return;
                    }
                }
            });

            // start listening
            locationManager.StartUpdatingLocation();
        }

        void fireEvents()
        {
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

        void doOnLocationUpdated(Location location)
        {
            this.TimeUpdated = DateTime.Now;
            this.Location = location;
            this.FailureInfo = "";
            if (this.Location != null)
                LocationServicesStatus = LocationServicesStatusEnum.Ok;
            this.fireEvents();
        }

        void initLocationManager()
        {
            if (locationManager != null)
                return;

            locationManager = new CoreLocation.CLLocationManager();
            locationManager.DesiredAccuracy = 500; //500m

            if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
            {
                locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    var location = e.Locations[e.Locations.Length - 1];
                    this.doOnLocationUpdated(new Location(location.Coordinate.Latitude, location.Coordinate.Longitude));
                };
            }
            else
            {
#pragma warning disable 618
                // this won't be called on iOS 6 (deprecated)
                locationManager.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) =>
                {
                    var location = e.NewLocation;
                    this.doOnLocationUpdated(new Location(location.Coordinate.Latitude, location.Coordinate.Longitude));
                };
#pragma warning restore 618
            }
        }
    }
}