using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace Awpbs.Mobile.iOS
{
    public class MobileNotificationsService_iOS : IMobileNotificationsService
    {
        public void AddReminder(DateTime whenToFire, string title, string body)
        {
            addLocalNotification(whenToFire, title, body);
        }

        public void AddLocalNotification(string title, string body, int objectID)
        {
            addLocalNotification(DateTime.Now, title, body);
        }

        void addLocalNotification(DateTime whenToFire, string title, string body)
        {
            // setup the notification
            var notification = new UILocalNotification();
            notification.FireDate = DateTimeHelper.DateTimeToNSDate(whenToFire);
            notification.AlertTitle = title;
            notification.AlertBody = body;
            notification.SoundName = UILocalNotification.DefaultSoundName;
            //notification.ApplicationIconBadgeNumber = 3; // modify the badge

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public void AskForNotificationsPermissions()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    null);

                UIApplication.SharedApplication.RegisterUserNotificationSettings(notificationSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }
    }
}
