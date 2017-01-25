using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public interface IMobileNotificationsService
    {
        void AskForNotificationsPermissions();
        void AddReminder(DateTime whenToFire, string title, string body);
		void AddLocalNotification(string title, string body, int objectID);
    }
}
