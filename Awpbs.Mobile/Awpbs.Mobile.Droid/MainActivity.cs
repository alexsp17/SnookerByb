using System;

using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Support.V7.AppCompat;
using Android.Support.V7.App;

// for push notifications
//using Android.Gms.Common;

using XLabs.Ioc; // Using for SimpleContainer
using XLabs.Platform.Services.Geolocation; // Using for Geolocation
using XLabs.Platform.Device; // Using for Display

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.Android;
using XLabs.Forms;
using Java.Security;
using BranchXamarinSDK;
using RoundedBoxView.Forms.Plugin;

namespace Awpbs.Mobile.Droid
{
    [Activity (Label = "Snooker Byb", 
		Icon = "@drawable/icon", 
		MainLauncher = false, 
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, 
		WindowSoftInputMode = SoftInput.AdjustPan,
		ScreenOrientation = ScreenOrientation.Portrait,
        Exported = true
	)]
	[IntentFilter(
		new[] { Intent.ActionView },
		Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
		DataScheme = "snookerbyb" // this makes this activity to open when a user taps on a link like snookerbyb://xxx in another app (like an email app)
	)]
	//public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
        public static MainActivity The { get; private set; }
		private static bool initialized;

		static void handleAndroidException(object sender, RaiseThrowableEventArgs e)
		{
			Log.Error("INTERNAL DEBUG", "MainActivity::Unhandled Exception");
			e.Handled = true;

			var failureInfo = TraceHelper.ExceptionToString(e.Exception);
			Console.WriteLine("MainActivity::HandleAndroidException(): " + failureInfo);
		}

		static void initializeTheApp()
		{
			if (initialized)
				return;
			initialized = true;

			Console.WriteLine ("initializeTheApp --------------------------------------------------------------------------------------");

			// init Byb services
			App.Files = new Files_Android();
			App.KeyChain = new KeyChain_UnsecuredFile(Awpbs.Mobile.App.Files);
			App.ScorePronouncer = new Awpbs.Mobile.Droid.ScorePronouncer_Android();
			App.FacebookService = new FacebookService_Android();
			App.LocationService = new LocationService_Android();
			App.MobileNotificationsService = new MobileNotificationsService_Android ();

			// init XLabs
            var container = new SimpleContainer();
            container.Register<IGeolocator, Geolocator>();
            container.Register<XLabs.Platform.Device.IDevice>(t => XLabs.Platform.Device.AndroidDevice.CurrentDevice);
            container.Register<XLabs.Platform.Services.Media.IMediaPicker, XLabs.Platform.Services.Media.MediaPicker>();
            XLabs.Ioc.Resolver.SetResolver(container.GetResolver());

			// OS version
			string osVersion;
			try
			{
				osVersion = Build.VERSION.Release;
			}
			catch (Exception) {
				osVersion = "? exception";
			}

			// what's the device screen height?
			double deviceScreenHeightInInches;
			try
			{
				var device = Resolver.Resolve<IDevice>();
				deviceScreenHeightInInches = device.Display.ScreenHeightInches();
			}
			catch (Exception)
			{
				deviceScreenHeightInInches = 5;
			}
			System.Diagnostics.Debug.WriteLine("screen height is " + deviceScreenHeightInInches);

			Config.Init(MobileAppEnum.Snooker, Device.Idiom == TargetIdiom.Tablet, deviceScreenHeightInInches, osVersion);

			AndroidEnvironment.UnhandledExceptionRaiser += handleAndroidException; // note that it's static
		}

		// Ensure we get the updated link identifier when the app is opened from the
		// background with a new link.
		protected override void OnNewIntent(Intent intent)
		{
			BranchAndroid.GetInstance().SetNewUrl(intent.Data);
		}

		protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			Console.WriteLine("MainActivity: OnCreate() !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

			// Replace font with our own
     		TypefaceUtil.SetDefaultFont(this.ApplicationContext, "MONOSPACE", "fonts/Lato-Regular.ttf"); 

			global::Xamarin.Forms.Forms.Init(this, bundle);
			global::Xamarin.FormsMaps.Init (this, bundle);
			Refractored.XamForms.PullToRefresh.Droid.PullToRefreshLayoutRenderer.Init ();
			RoundedBoxView.Forms.Plugin.Droid.RoundedBoxViewRenderer.Init();

			// init Branch
			BranchAndroid.Init (this, "key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe", Intent.Data);

            The = this;
			initializeTheApp ();

			try
			{
            	// Start the registration intent service; try to get a token:
            	var intent = new Intent (this, typeof (RegistrationIntentService));
            	StartService (intent);
			}
			catch (Exception) {
			}

            LoadApplication(new Awpbs.Mobile.App());
		}

        protected override void OnStart()
        {
            base.OnStart(); // Always call the superclass first.
            Console.WriteLine("MainActivity: OnStart()");
        }

		protected override void OnResume()
        {
            base.OnResume(); // Always call the superclass first.
            Console.WriteLine("MainActivity: OnResume()");

            // seems like OnCreate() is now called as well as OnResume()
			processRemoteNotificationOrUrlIfNecessary("called from OnResume");

			App.Navigator.CheckApiVersionAndNotifyIfNeeded ();
        }

		protected override void OnPause()
        {
            base.OnPause(); // Always call the superclass first.
            Console.WriteLine("MainActivity: OnPause()");
        }

		protected override void OnStop()
        {
            base.OnPause(); // Always call the superclass first.
            Console.WriteLine("MainActivity: OnStop()");
        }

        void processRemoteNotificationOrUrlIfNecessary(string calledFrom)
        {
            Console.WriteLine("MainActivity: processRemoteNotificationIfNecessary() {0}", calledFrom);

			if (Intent == null)
                return;

			// check if remote notification has been received and process it
			if (Intent.Extras != null)
			{
				string noNotificationText = "no push notification";
				string notificationText = Intent.Extras.GetString ("pushText", noNotificationText);
				string objectIDstr = Intent.Extras.GetString ("pushObjectID", "");
				int objectID = 0;
				int.TryParse (objectIDstr, out objectID);
				Console.WriteLine ("processRemoteNotificationIfNecessary message " + notificationText);
				if (notificationText != noNotificationText)
				{
					PushNotificationMessage message = new PushNotificationMessage () { Text = notificationText, ObjectID = objectID };
					Awpbs.Mobile.App.Navigator.ProcessRemoteNotification (message, true);

					// make sure to avoid processing this if the app sleeps and then resumes
					Intent.RemoveExtra ("pushText");
					Intent.RemoveExtra ("pushObjectID");

					return;
				}
			}

			// check if onening from a UR
			if (Intent.Data != null && Intent.Data.EncodedAuthority != null)
			{
				string url = Intent.Data.ToString ();
				Console.WriteLine ("Intent.Data: " + url);
				App.Navigator.ProcessOpenUrl (url);

				return;
			}
        }

//        bool isPlayServicesAvailable ()
//        {
//            GoogleApiAvailability ga = GoogleApiAvailability.Instance;
//            var resultCode = ga.IsGooglePlayServicesAvailable(this);
//
//            // for testing
//            //resultCode = ConnectionResult.ServiceMissing;
//
//            if (resultCode != ConnectionResult.Success)
//            { 
//                if (ga.IsUserResolvableError (resultCode))
//                {
//
//                    // Give the user a chance to download the APK:
//                    //msgText.Text = ga.GetErrorString(resultCode);
//
//                    var msgString = ga.GetErrorString (resultCode);
//                    Console.WriteLine(msgString);
//                }
//                else
//                {
//                    Console.WriteLine("Sorry, this device is not supported");
//                    Finish ();
//                }
//                return false;
//            }
//            else
//            {
//                Console.WriteLine("Google Play Services is available.");
//                return true;
//            }
//        }
	}
}

