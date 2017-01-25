using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// Represents an ID for a mobile device
    /// </summary>
    public class DeviceToken
    {
        public int DeviceTokenID { get; set; }
        public int AthleteID { get; set; }
        public string Token { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool IsApple { get; set; }
        public bool IsAndroid { get; set; }
        public bool IsProduction { get; set; }

        public virtual Athlete Athlete { get; set; }
        public virtual List<PushNotification> PushNotifications { get; set; }

        public override string ToString()
        {
            return Token;
        }
    }
}
