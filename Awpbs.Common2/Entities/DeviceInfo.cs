using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// Represents information about a mobile device sent to the server
    /// </summary>
    public class DeviceInfo
    {
        public long DeviceInfoID { get; set; }
        public int AthleteID { get; set; }
        public DateTime TimeCreated { get; set; }
        public int Platform { get; set; }
        public string OSVersion { get; set; }

        public virtual Athlete Athlete { get; set; }

        public override string ToString()
        {
            return AthleteID.ToString() + ": " + ((MobilePlatformEnum)Platform).ToString() + ", OS version " + OSVersion;
        }
    }
}
