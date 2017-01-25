using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class VenueEditWebModel
    {
        public int VenueID { get; set; }
        public int? NumberOf10fSnookerTables { get; set; }
        public int? NumberOf12fSnookerTables { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string PoiID { get; set; }
        public bool IsInvalid { get; set; }

        public bool HasAddress { get { return string.IsNullOrEmpty(Address) == false; } }
        public bool HasPhoneNumber { get { return string.IsNullOrEmpty(PhoneNumber) == false; } }
        public bool HasWebsite { get { return string.IsNullOrEmpty(Website) == false; } }
        public bool HasPoiID { get { return string.IsNullOrEmpty(PoiID) == false; } }
    }
}
