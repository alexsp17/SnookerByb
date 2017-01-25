using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Awpbs.Web
{
    public class SeederVenues
    {
        private readonly ApplicationDbContext db;

        public SeederVenues(ApplicationDbContext db)
        {
            this.db = db;
        }

        public List<Venue> LoadVenuesFromFile(System.IO.Stream file)
        {
            var reader = new System.IO.StreamReader(file);

            // first line is the header
            reader.ReadLine();

            // load venues line by line
            int lineNumber = 0;
            List<Venue> venues = new List<Venue>();
            for (; ; )
            {
                try
                {
                    lineNumber++;

                    var line = reader.ReadLine();
                    if (line == null)
                        break;

                    string[] strs = line.Split('\t');
                    if (strs.Length < 12)
                        throw new Exception("Incorrect format. lineNumber=" + lineNumber);

                    int id = 0;
                    int.TryParse(strs[0], out id);

                    string name = strs[2].Trim();
                    if (name.Length == 0)
                        throw new Exception("Empty name. lineNumber=" + lineNumber);

                    Country country = Country.Get(strs[3]);
                    if (country == null)
                        throw new Exception("Unknown country. lineNumber=" + lineNumber);

                    double lat = 0.0;
                    double lon = 0.0;
                    double? latitude = null;
                    double? longitude = null;
                    if (double.TryParse(strs[4], out lat))
                        latitude = lat;
                    if (double.TryParse(strs[5], out lon))
                        longitude = lon;

                    int? numberOf10ftTables = null;
                    if (strs[6].Length > 0 && strs[6].Trim() != "?")
                        numberOf10ftTables = int.Parse(strs[6]);

                    int? numberOf12ftTables = null;
                    if (strs[7].Length > 0 && strs[7].Trim() != "?")
                        numberOf12ftTables = int.Parse(strs[7]);

                    string address = strs[8].Trim();
                    if (address.StartsWith("\"") && address.EndsWith("\""))
                        address = address.Substring(1, address.Length - 2).Trim();

                    string phonenumber = strs[9].Trim();
                    if (phonenumber.StartsWith("\"") && phonenumber.EndsWith("\""))
                        phonenumber = phonenumber.Substring(1, phonenumber.Length - 2).Trim();

                    string website = strs[10].Trim();
                    if (website.StartsWith("\"") && website.EndsWith("\""))
                        website = website.Substring(1, website.Length - 2).Trim();

                    string poiid = strs[11].Trim();
                    if (poiid.StartsWith("\"") && poiid.EndsWith("\""))
                        poiid = poiid.Substring(1, poiid.Length - 2).Trim();

                    venues.Add(new Venue()
                    {
                        VenueID = id,
                        IsSnooker = strs[1] == "1",
                        Name = name,
                        Country = country.ThreeLetterCode,
                        Latitude = latitude,
                        Longitude = longitude,
                        NumberOf10fSnookerTables = numberOf10ftTables,
                        NumberOf12fSnookerTables = numberOf12ftTables,
                        Address = address,
                        PhoneNumber = phonenumber,
                        Website = website,
                        PoiID = poiid
                    });
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            if (venues.Count < 1)
                throw new Exception("No venues in the file.");
            return venues;
        }

        public void SaveVenuesToFile(List<Venue> venues, System.IO.Stream file)
        {
            var writer = new System.IO.StreamWriter(file);

            writer.WriteLine("ID\tSnooker\tName\tCountry\tLatitude\tLongitude\t10' tables\t12' tables\tAddress\tPhone number\tWebsite\tPOI id");

            foreach (var venue in venues)
            {
                string line =
                    venue.VenueID.ToString() + "\t" +
                    (venue.IsSnooker ? "1" : "0") + "\t" +
                    venue.Name + "\t" +
                    venue.Country + "\t" +
                    venue.Latitude.ToString() + "\t" +
                    venue.Longitude + "\t" +
                    (venue.NumberOf10fSnookerTables != null ? venue.NumberOf10fSnookerTables.Value.ToString() : "") + "\t" +
                    (venue.NumberOf12fSnookerTables != null ? venue.NumberOf12fSnookerTables.Value.ToString() : "") + "\t" +
                    (venue.HasAddress ? venue.Address : " ") + "\t" +
                    (venue.HasPhoneNumber ? venue.PhoneNumber : " ") + "\t" +
                    (venue.HasWebsite ? venue.Website : " ") + "\t" +
                    (venue.HasPOIid ? venue.PoiID : " ") + "\t";
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        public void PopulateVenuesIntoTheDatabase(List<Venue> venues, out int countNew, out int countModified, out int countError)
        {
            countNew = 0;
            countModified = 0;
            countError = 0;

            foreach (var venue in venues)
            {
                try
                {
                    Venue venueInDb = null;
                    if (venue.VenueID != 0)
                        venueInDb = db.Venues.Where(i => i.VenueID == venue.VenueID).FirstOrDefault();
                    if (venueInDb == null)
                        venueInDb = db.Venues.Where(i => i.Name == venue.Name && i.Latitude == venue.Latitude && i.Longitude == venue.Longitude).FirstOrDefault();

                    if (venueInDb == null)
                    {
                        venueInDb = new Venue()
                        {
                            Name = venue.Name,
                            Country = venue.Country,
                            Latitude = venue.Latitude,
                            Longitude = venue.Longitude,
                            IsSnooker = venue.IsSnooker,
                            CreatedByAthleteID = 0,
                            NumberOf10fSnookerTables = venue.NumberOf10fSnookerTables,
                            NumberOf12fSnookerTables = venue.NumberOf12fSnookerTables,
                            Address = venue.Address,
                            PhoneNumber = venue.PhoneNumber,
                            Website = venue.Website,
                            PoiID = venue.PoiID,
                            TimeCreated = DateTime.UtcNow
                        };
                        db.Venues.Add(venueInDb);
                        db.SaveChanges();
                        countNew++;
                    }
                    else if (venueInDb.Name != venue.Name || 
                            venueInDb.Country != venue.Country ||
                            venueInDb.Latitude != venue.Latitude ||
                            venueInDb.Longitude != venue.Longitude ||
                            venueInDb.IsSnooker != venue.IsSnooker ||
                            venueInDb.NumberOf10fSnookerTables != venue.NumberOf10fSnookerTables ||
                            venueInDb.NumberOf12fSnookerTables != venue.NumberOf12fSnookerTables ||
                            string.Compare(venueInDb.Address, venue.Address, true) != 0 ||
                            string.Compare(venueInDb.PhoneNumber, venue.PhoneNumber, true) != 0 ||
                            string.Compare(venueInDb.Website, venue.Website, true) != 0 ||
                            string.Compare(venueInDb.PoiID, venue.PoiID, true) != 0)
                    {
                        venueInDb.Name = venue.Name;
                        venueInDb.Country = venue.Country;
                        venueInDb.Latitude = venue.Latitude;
                        venueInDb.Longitude = venue.Longitude;
                        venueInDb.IsSnooker = venue.IsSnooker;
                        venueInDb.NumberOf10fSnookerTables = venue.NumberOf10fSnookerTables;
                        venueInDb.NumberOf12fSnookerTables = venue.NumberOf12fSnookerTables;
                        venueInDb.Address = venue.Address;
                        venueInDb.PhoneNumber = venue.PhoneNumber;
                        venueInDb.Website = venue.Website;
                        venueInDb.PoiID = venue.PoiID;
                        db.SaveChanges();
                        countModified++;
                    }

                    venue.VenueID = venueInDb.VenueID;
                }
                catch (Exception)
                {
                    countError++;
                }
            }
        }

        public class GoogleVerifyResults
        {
            public List<Venue> VenuesVerifiedWithGoogle { get; set; }
            public int CountErrors { get; set; }
        }

        public async Task<GoogleVerifyResults> VerifyVenuesWithGoogle(List<Venue> venues)
        {
            GoogleVerifyResults results = new GoogleVerifyResults();
            results.VenuesVerifiedWithGoogle = new List<Venue>();
            results.CountErrors = 0;

            GoogleGeocodingApi geocodingApi = new GoogleGeocodingApi();
            GooglePlacesApi placesApi = new GooglePlacesApi();

            for (int i = 0; i < venues.Count; ++i)
            {
                var venue = venues[i];
                geocodingApi.SleepToAvoidGoogleApiThreshold();

                // check for the location if not known
                if (venue.Location == null)
                {
                    try
                    {
                        var location = await geocodingApi.LocationFromAddress(venue.Address);
                        venue.Latitude = location.Latitude;
                        venue.Longitude = location.Longitude;
                    }
                    catch (Exception)
                    {
                        results.CountErrors++;
                        continue;
                    }
                }

                // check with Places API
                try
                {
                    // within 250meters
                    var pois = await placesApi.Search(venue.Location, 250, venue.Name);
                    if (pois.Count == 1)
                    {
                        POIWebModel poi = pois[0];
                        putPoiInfoIntoVenue(poi, venue, false);
                        results.VenuesVerifiedWithGoogle.Add(venue);
                        continue; // all good
                    }
                    if (pois.Count > 1)
                        continue; // ambigious

                    // within 20k
                    pois = await placesApi.Search(venue.Location, 20000, venue.Name);
                    if (pois.Count > 0)
                    {
                        POIWebModel poi = null;
                        if (pois.Count == 1)
                            poi = pois[0];
                        else if (pois[0].Name.Contains(venue.Name) || venue.Name.Contains(pois[0].Name))
                            poi = pois[0];
                        else if (pois[0].Name.Length >= 3 && venue.Name.Length >= 3 && pois[0].Name.Substring(0, 3).ToLower() == venue.Name.Substring(0, 3).ToLower())
                            poi = pois[0];

                        if (poi != null)
                        {
                            this.putPoiInfoIntoVenue(poi, venue, true);
                            results.VenuesVerifiedWithGoogle.Add(venue);
                            continue; // all good
                        }
                    }
                }
                catch (Exception)
                {
                    results.CountErrors++;
                    continue;
                }
            }

            return results;
        }

        private void putPoiInfoIntoVenue(POIWebModel poi, Venue venue, bool putLocation)
        {
            if (poi.HasPoiID)
                venue.PoiID = poi.PoiID;
            if (poi.HasPhone)
                venue.PhoneNumber = poi.Phone;
            if (poi.HasWebsite)
                venue.Website = poi.Website;
            if (poi.HasAddress)
                venue.Address = poi.Address;
            if (poi.HasPoiID)
                venue.PoiID = poi.PoiID;

            if (putLocation && poi.Location != null)
            {
                venue.Latitude = poi.Location.Latitude;
                venue.Longitude = poi.Location.Longitude;
            }
        }
    }
}
