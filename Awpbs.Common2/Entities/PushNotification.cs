using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// Represents a single notification sent to a mobile device
    /// </summary>
    public class PushNotification
    {
        public long PushNotificationID { get; set; }
        public int DeviceTokenID { get; set; }
        public DateTime TimeCreated { get; set; }
        public int Status { get; set; }
        public string NotificationText { get; set; }
        public int? ObjectID1 { get; set; }
        public int? ObjectID2 { get; set; }
        public bool IsProduction { get; set; }

        public virtual DeviceToken DeviceToken { get; set; }
    }
}
