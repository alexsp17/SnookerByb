using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// </summary>
    public class Venue
    {
        public int VenueID { get; set; }
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? MetroID { get; set; }

        public bool IsSnooker { get; set; }
        public int? NumberOf10fSnookerTables { get; set; }
        public int? NumberOf12fSnookerTables { get; set; }

        public int CreatedByAthleteID { get; set; }
        public DateTime TimeCreated { get; set; }

        public string Country { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string PoiID { get; set; }
        public bool IsInvalid { get; set; }

        public bool HasAddress { get { return string.IsNullOrEmpty(Address) == false; } }
        public bool HasPhoneNumber { get { return string.IsNullOrEmpty(PhoneNumber) == false; } }
        public bool HasWebsite { get { return string.IsNullOrEmpty(Website) == false; } }
        public bool HasPOIid { get { return string.IsNullOrEmpty(PoiID) == false; } }

        public virtual Metro Metro { get; set; }
        public virtual List<Result> Results { get; set; }
        public virtual List<Score> Scores { get; set; }
        public virtual List<VenueEdit> VenueEdits { get; set; }
        public virtual List<GameHost> GameHosts { get; set; }

        public Location Location
        {
            get
            {
                if (Latitude == null || Longitude == null)
                    return null;
                return new Location(Latitude.Value, Longitude.Value);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
