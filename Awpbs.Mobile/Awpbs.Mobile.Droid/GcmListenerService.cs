using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Graphics;
using Android.Util;
using System;

namespace Awpbs.Mobile.Droid
{
    [Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        public override void OnMessageReceived (string from, Bundle data)
        {
            var message = data.GetString ("message");
			var objectIDstr = data.GetString ("objectID");
			int objectID = 0;
			int.TryParse (objectIDstr, out objectID);
			Console.WriteLine ("MyGcmListenerService");
			Console.WriteLine ("MyGcmListenerService: message=" + message);
			Console.WriteLine ("MyGcmListenerService: objectID=" + objectID.ToString());

			App.MobileNotificationsService.AddLocalNotification ("Snooker Byb", message, objectID);
        }
    }
}