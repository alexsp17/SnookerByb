using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class LocationHelper
    {
        public VenueWebModel ClosestVenue
        {
            get;
            private set;
        }

        public Location Location
        {
            get;
            private set;
        }

        ILocationService locationSvc;
        WebService webService;

        public LocationHelper(ILocationService locationSvc, WebService webService)
        {
            this.locationSvc = locationSvc;
            this.webService = webService;
        }

        public void CheckForClosestVenue(EventHandler done)
        { 
            locationSvc.RequestLocationAsync((s1, e1) =>
            {
                onLocationReceived(done);
            }, false);
        }

        async void onLocationReceived(EventHandler done)
        {
            // new location?
            var newLocation = locationSvc.Location;
            if (newLocation == null || locationSvc.LocationServicesStatus != LocationServicesStatusEnum.Ok)
            {
                Location = null;
                ClosestVenue = null;
                done(this, EventArgs.Empty);
                return;
            }
            if (Location != null && Distance.Calculate(newLocation, Location).Meters < 500)
            {
                done(this, EventArgs.Empty);
                return; // same location as before
            }
            Location = newLocation;

            // find closest venue in cache
            VenueWebModel newClosestVenue = App.Cache.Venues.GetClosest(Location, Distance.FromMeters(500));
            if (newClosestVenue == null)
            {
                // find closest venue online
                var venues = await webService.FindSnookerVenues(newLocation, "", false, false);
                if (venues != null && venues.Count > 0)
                {
                    if (venues[0].DistanceInMeters < 500)
                        newClosestVenue = venues[0];
                }
            }
            ClosestVenue = newClosestVenue;

            done(this, EventArgs.Empty);
        }
    }
}
