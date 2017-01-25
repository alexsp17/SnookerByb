using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class VenueWebModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int MetroID { get; set; }
        public string MetroName { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsSnooker { get; set; }
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
        public bool HasPOIid { get { return string.IsNullOrEmpty(PoiID) == false; } }

        public double DistanceInMeters { get; set; }
        public int LastContributorID { get; set; }
        public string LastContributorName { get; set; }
        public DateTime? LastContributorDate { get; set; }

        public Location Location
        {
            get
            {
                if (Latitude == null || Longitude == null)
                    return null;
                return new Location(Latitude.Value, Longitude.Value);
            }
        }

        public Distance Distance
        {
            get
            {
                return Distance.FromMeters(DistanceInMeters);
            }
        }

        public string PhoneNumberHtml
        {
            get
            {
                return PhoneNumberHelper.ToDialable(PhoneNumber, Country);
            }
        }

        public Venue ToVenue()
        {
            Venue venue = new Venue();
            venue.VenueID = this.ID;
            venue.Name = this.Name;
            venue.Country = this.Country;
            venue.MetroID = this.MetroID;
            venue.Latitude = this.Latitude;
            venue.Longitude = this.Longitude;
            venue.IsSnooker = this.IsSnooker;
            venue.NumberOf10fSnookerTables = this.NumberOf10fSnookerTables;
            venue.NumberOf12fSnookerTables = this.NumberOf12fSnookerTables;
            venue.Address = this.Address;
            venue.PhoneNumber = this.PhoneNumber;
            venue.Website = this.Website;
            venue.PoiID = this.PoiID;
            venue.IsInvalid = this.IsInvalid;
            return venue;
        }

        public static VenueWebModel FromVenue(Venue venue, Location location, Athlete lastContributor = null, DateTime? lastContributorDate = null)
        {
            VenueWebModel obj = new VenueWebModel();
            obj.ID = venue.VenueID;
            obj.Name = venue.Name;
            obj.Country = venue.Country;
            obj.MetroID = venue.MetroID ?? 0;
            obj.MetroName = venue.MetroID != null ? (venue.Metro != null ? venue.Metro.Name : "not loaded") : "";
            obj.Latitude = venue.Latitude;
            obj.Longitude = venue.Longitude;
            obj.IsSnooker = venue.IsSnooker;
            obj.NumberOf10fSnookerTables = venue.NumberOf10fSnookerTables;
            obj.NumberOf12fSnookerTables = venue.NumberOf12fSnookerTables;
            obj.Address = venue.Address;
            obj.PhoneNumber = venue.PhoneNumber;
            obj.Website = venue.Website;
            obj.PoiID = venue.PoiID;
            obj.IsInvalid = venue.IsInvalid;

            if (location != null && obj.Latitude != null && obj.Longitude != null)
                obj.DistanceInMeters = Awpbs.Distance.Calculate(location, new Location(obj.Latitude.Value, obj.Longitude.Value)).Meters;

            if (lastContributor != null)
            {
                obj.LastContributorID = lastContributor.AthleteID;
                obj.LastContributorName = lastContributor.Name;
                obj.LastContributorDate = lastContributorDate;
            }

            return obj;
        }
    }
}
