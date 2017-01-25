////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////
////using Android.App;
////using Android.Content;
////using Android.OS;
////using Android.Runtime;
////using Android.Util;
////using Android.Views;
////using Android.Widget;
////using Android.Locations;
////using System.Threading.Tasks;
////
////using Xamarin.Forms;
////
////using Android.Gms.Common;
////using Android.Gms.Common.Apis;
////using Android.Gms.Location;
//
//using System;
//using System.Collections.Generic;
//using System.Linq;
//
//using Android.App;
//using Android.Content;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.OS;
//using Android.Gms.Common.Apis;
//using Android.Gms.Location;
//using System.Threading.Tasks;
//
//namespace Awpbs.Mobile.Droid
//{
//	public class LocationService_GooglePlay : ILocationService, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener, ILocationListener//Googleapiclient IGooglePlayServicesClientConnectionCallbacks, IGooglePlayServicesClientOnConnectionFailedListener//, Java.Lang.Object, ILocationListener
//    {
//		public void OnLocationChanged (Android.Locations.Location location)
//		{
//		}
//
//		public void OnConnectionFailed (Android.Gms.Common.ConnectionResult result)
//		{
//		}
//
//		public void Dispose ()
//		{
//		}
//
//		public IntPtr Handle {
//			get {
//				return IntPtr.Zero;
//			}
//		}
//
//        //private LocationManager locationManager;
//		IGoogleApiClient googleApiClient;
//        List<EventHandler> eventHandlersToFire = new List<EventHandler>();
//
//        public DateTime? TimeLocationUpdated { get; private set; }
//        public Location Location { get; private set; }
//        //public int eventIdx { get; private set; }
//
//        public DateTime? TimeUpdated
//        {
//            get;
//            private set;
//        }
//
//        public string FailureInfo
//        {
//            get
//            {
//                return failureInfo;
//            }
//        }
//        private string failureInfo;
//
//        public LocationServicesStatusEnum LocationServicesStatus
//        {
//            get;
//            private set;
//        }
//
//		public LocationService_GooglePlay()
//        {
//            LocationServicesStatus = LocationServicesStatusEnum.Unknown;
//            //eventIdx = 0;
//        }
//
//		public Location GetLastKnownLocationQuickly()
//		{
//			initialize ();
//			return null;
//			//this.createLocationManager ();
//			//var location = locationManager.GetLastKnownLocation (LocationManager.PassiveProvider);
//			//if (location == null)
//			//	return null;
//			//return new Location (location.Latitude, location.Longitude);
//		}
//
//        public void RequestLocationAsync(EventHandler done, bool askPermissionIfNecessary)
//        {
//            if (done != null)
//            {
//                //eventIdx++;
//				//
//                //Console.WriteLine("LocationService.RequestLocationAsync(): add eventHandler, eventIdx++ " + eventIdx);
//                eventHandlersToFire.Add(done);
//            }
//
//            Console.WriteLine("LocationService.RequestLocationAsync()");
//			initialize ();
//			RequestLocationUpdates ();
//            //getLocation();
//        }
//
//		void initialize()
//		{
//			if (googleApiClient != null)
//				return;
//
//			try
//
//			{
//				var builder = new GoogleApiClientBuilder (Application.Context);
//				builder = builder.AddApi (Android.Gms.Location.LocationServices.API);
//				builder = builder.AddConnectionCallbacks (this);
//				builder = builder.AddOnConnectionFailedListener (this);
//				googleApiClient = builder.Build ();
//				googleApiClient.Connect();
//			}
//			catch (Exception exc) {
//				int asdf = 15;
//			}
//		}
//
//		async Task RequestLocationUpdates ()
//		{
//			// Describe our location request
//			var locationRequest = new LocationRequest ()
//				.SetInterval (10000)
//				.SetFastestInterval (1000)
//				.SetPriority (LocationRequest.PriorityHighAccuracy);
//
//			// Check to see if we can request updates first
//			if (await CheckLocationAvailability (locationRequest)) {
//
//				// Request updates
//				await LocationServices.FusedLocationApi.RequestLocationUpdates (googleApiClient,
//					locationRequest, 
//					this);
//			}
//		}
//
//		async Task<bool> CheckLocationAvailability (LocationRequest locationRequest)
//		{
//			// Build a new request with the given location request
//			var locationSettingsRequest = new LocationSettingsRequest.Builder ()
//				.AddLocationRequest (locationRequest)
//				.Build ();
//
//			// Ask the Settings API if we can fulfill this request
//			var locationSettingsResult = await LocationServices.SettingsApi.CheckLocationSettingsAsync (googleApiClient, locationSettingsRequest);
//
//
//			// If false, we might be able to resolve it by showing the location settings 
//			// to the user and allowing them to change the settings
//			if (!locationSettingsResult.Status.IsSuccess) {
//
//				//if (locationSettingsResult.Status.StatusCode == LocationSettingsStatusCodes.ResolutionRequired)
//				//	locationSettingsResult.Status.StartResolutionForResult(this, 101);
//				//else 
//				//	Toast.MakeText (this, "Location Services Not Available for the given request.", ToastLength.Long).Show ();
//
//				return false;
//			}
//
//			return true;
//		}
//
//		////Interface methods
//
//		public void OnConnected (Bundle bundle)
//		{
//			// This method is called when we connect to the LocationClient. We can start location updated directly form
//			// here if desired, or we can do it in a lifecycle method, as shown above 
//
//			// You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
//			//Log.Info("LocationClient", "Now connected to client");
//		}
//
//		public void OnDisconnected ()
//		{
//			// This method is called when we disconnect from the LocationClient.
//
//			// You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
//		    //Log.Info("LocationClient", "Now disconnected from client");
//		}
//
//		public void OnConnectionSuspended (int i)
//		{
//
//		}
//
//
//
////        private void getLocation()
////        {
////            try
////            {
////                this.failureInfo = "";
////                DateTime timeInitiated = DateTime.Now;
////
////                Console.WriteLine("GetLocation(): Enter time " + timeInitiated);
////
////                var waitTask = new System.Threading.Tasks.Task(() =>
////                    {
////                        Console.WriteLine("GetLocation(): inside waitTask");
////
////                        for (; ; )
////                        {
////                            if (TimeLocationUpdated != null && TimeLocationUpdated.Value >= timeInitiated)
////                            {
////                                Console.WriteLine("LocationService.getLocation() location updated");
////                                return;
////                            }
////
////							DateTime now = DateTime.Now;
////							if ((now - timeInitiated).TotalSeconds > 3 && Location != null && TimeLocationUpdated != null && (TimeLocationUpdated.Value - timeInitiated).TotalMinutes < 5)
////							{
////								Console.WriteLine("LocationService.getLocation() locationd didn't update, maybe because it didn't change since the last time. ");
////								this.fireEvents();
////								return;
////							}
////
////                            if ((now - timeInitiated).TotalSeconds > 20)
////                            {
////                                Console.WriteLine("LocationService.getLocation(): Timeout");
////                                this.failureInfo = "Timeout.";
////                                return;
////                            }
////                            System.Threading.Thread.Sleep(200);
////                        }
////                    });
////                
////                if (this.locationManager == null)
////                    this.locationManager = MainActivity.The.GetSystemService(Context.LocationService) as LocationManager;
////
////                // TODO: use NetworkProvider as an alternative?
////                string provider = LocationManager.GpsProvider;
////
////                if (locationManager.IsProviderEnabled(provider))
////                {
////                    Console.WriteLine("LocationService.getLocation(): RequestLocationUpdates()");
////                    locationManager.RequestLocationUpdates (provider, 2000, 1, this);
////                }
////                else
////                {
////                    Console.WriteLine("LocationService.getLocation(): provider not enabled()");
////
////                    this.failureInfo = "provider not enabled";
////
////                    return;
////                }
////
////                waitTask.Start();
////            }
////            catch (Exception exc)
////            {
////                failureInfo = TraceHelper.ExceptionToString(exc);
////            }
////        }
////
////		void createLocationManager()
////		{
////			if (this.locationManager != null)
////				return;
////			this.locationManager = MainActivity.The.GetSystemService(Context.LocationService) as LocationManager;
////		}
////
////        void fireEvents()
////        {
////            try
////            {
////                int processedEvents = 0;
////
////                Console.WriteLine("LocationService", " fireEvents() Enter");
////                Device.BeginInvokeOnMainThread(() =>
////                {
////                    var list = eventHandlersToFire.ToList();
////                    foreach (var e in list)
////                    {
////                        eventIdx--;
////
////                        Console.WriteLine("LocationService::fireEvents(), processEvent " + processedEvents + " eventIdx  " + eventIdx);
////
////                        e(this, EventArgs.Empty);
////                        eventHandlersToFire.Remove(e);
////                        processedEvents++;
////                    }
////                });
////                Console.WriteLine("LocationService::fireEvents() Exit, processed " + processedEvents);
////
////            }
////            catch (Exception exc)
////            {
////
////                failureInfo = TraceHelper.ExceptionToString(exc);
////
////                Console.WriteLine("Exception fireEvents: ", failureInfo);
////
////                return;
////            }
////        }
////
////        //
////        // ILocationListener
////        //
////
////        public void OnLocationChanged(Android.Locations.Location location)
////        {
////            TimeLocationUpdated = DateTime.Now;
////            this.Location = new Location() { Latitude = location.Latitude, Longitude = location.Longitude };
////
////            if (this.Location != null)
////                LocationServicesStatus = LocationServicesStatusEnum.Ok;
////
////            Console.WriteLine("console OnLocationChanged2: " +
////                location.Provider.ToString() + " " +
////                this.Location.Latitude.ToString() + " " +
////                this.Location.Longitude.ToString());
////
////            var list = this.eventHandlersToFire.ToList();
////
////            if (list.Count > 0)
////            {
////                Console.WriteLine("OnLocationChanged: LocationService has events, call fireEvents()");
////                this.fireEvents();
////            }
////        }
////
////        public void OnProviderDisabled(string provider)
////        {
////        }
////
////        public void OnProviderEnabled(string provider)
////        {
////        }
////
////        public void OnStatusChanged(string provider, Availability status, Bundle extras)
////        {
////        }
//    }
//}
