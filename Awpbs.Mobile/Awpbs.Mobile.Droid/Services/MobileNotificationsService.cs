using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Graphics;
using Android.Util;

namespace Awpbs.Mobile.Droid
{
	public class MobileNotificationsService_Android : IMobileNotificationsService
	{
		public MobileNotificationsService_Android ()
		{
		}

		public void AskForNotificationsPermissions()
		{
			// no need to do anything on Android
		}

		public void AddReminder(DateTime whenToFire, string title, string body)
		{
			// ???
		}

		public void AddLocalNotification(string title, string body, int objectID)
		{
			var context = Application.Context;

			// Set up a pending intent so that tapping the notifications returns to the app
			Intent intent = new Intent (context, typeof(MainActivity));
			intent.PutExtra ("pushText", body);
			intent.PutExtra ("pushObjectID", objectID.ToString());
			intent.SetFlags (ActivityFlags.NoHistory | ActivityFlags.ClearTask | ActivityFlags.NewTask); // these flags make the new activity the one and the only active activity, removing ability to navigate "back"
			const int pendingIntentId = 0; // only using one PendingIntent (ID = 0)
			PendingIntent pendingIntent = 
				PendingIntent.GetActivity (context, pendingIntentId, intent, PendingIntentFlags.UpdateCurrent);// 0);

			// Build the local notification
			Notification.Builder builder = new Notification.Builder (context)
				.SetContentIntent(pendingIntent)
				.SetContentTitle (title)
				.SetContentText (body)
				.SetSmallIcon (Resource.Drawable.icon);
			Notification notification = builder.Build();

			// Publish the local notification
			NotificationManager notificationManager = 
				context.GetSystemService (Context.NotificationService) as NotificationManager;
			int notificationId = new System.Random ().Next ();
			notificationManager.Notify (notificationId, notification);
		}
	}
}

