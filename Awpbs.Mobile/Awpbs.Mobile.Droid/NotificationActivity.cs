
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using Awpbs.Mobile;

namespace Awpbs.Mobile.Droid
{
    using System.Threading;

    using Android.App;
    using Android.OS;

	[Activity(Theme = "@style/Theme.NotificationActivity", MainLauncher = false, NoHistory = true)]
    public class NotificationActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //StartActivity(typeof(MainActivity));
            // add a way to deal with unhandled exceptions
            AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

			processRemoteNotificationIfNecessary ();
        }

		void processRemoteNotificationIfNecessary()
		{
			if (Intent == null)
				return; // this shouldn't be happening
			
			// check if remote notification has been received and process it
			string noNotificationText = "no push notification";
			string notificationText = Intent.Extras.GetString("pushText", noNotificationText);
			if (notificationText != noNotificationText)
			{
				PushNotificationMessage message = new PushNotificationMessage() { Text = notificationText };
				Awpbs.Mobile.App.Navigator.ProcessRemoteNotification(message, true);

				//this.StartActivity(typeof(Awpbs.Mobile.Droid.MainActivity));
			}
		}

        void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
			Log.Error("NotificationActivity Activity: INTERNAL DEBUG", "PLEASE HANDLE MY EXCEPTION!");
            e.Handled = true;

            var failureInfo = TraceHelper.ExceptionToString(e.Exception);
			Console.WriteLine("NotificationActivity::HandleAndroidException(): " + failureInfo);
        }

        protected override void OnResume()
        {
			Console.Write ("NotificationActivity - OnResume");
            base.OnResume();
        }

        protected override void OnPause()
        {
			Console.Write ("NotificationActivity - OnPause");
            base.OnPause();
        }

        protected override void OnStop()
        {
			Console.Write ("NotificationActivity - OnStop");
            base.OnStop();
            this.Finish();
        }

        protected override void OnDestroy()
        {
			Console.Write ("NotificationActivity - OnDestroy");
            base.OnDestroy();
        }

    }

}

