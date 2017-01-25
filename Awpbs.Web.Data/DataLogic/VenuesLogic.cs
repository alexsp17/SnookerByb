using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class VenuesLogic
    {
        private readonly ApplicationDbContext db;

        public VenuesLogic(ApplicationDbContext context)
        {
            this.db = context;
        }

        public VenueWebModel Get(int id)
        {
            var venue = db.Venues.Include("Metro").Where(i => i.VenueID == id).Single();
            var model = VenueWebModel.FromVenue(venue, null, null);
            this.loadLastContributorToModel(model);
            return model;
        }

        private void loadLastContributorToModel(VenueWebModel model)
        {
            model.LastContributorID = 0;
            model.LastContributorName = "";
            model.LastContributorDate = null;

            var lastEdit = (from i in db.VenueEdits.Include("Athlete")
                            where i.VenueID == model.ID
                            orderby i.Time descending
                            select i).FirstOrDefault();
            if (lastEdit != null)
            {
                model.LastContributorID = lastEdit.Athlete.AthleteID;
                model.LastContributorName = lastEdit.Athlete.NameOrUnknown;
                model.LastContributorDate = lastEdit.Time;
            }
        }

        public List<VenueWebModel> GetAllVenuesInCountry(string country)
        {
            var query = (from i in db.Venues
                         where i.Country == country
                         select i);
            var items = (from i in query
                         let lastEdit = i.VenueEdits.OrderByDescending(e => e.Time).FirstOrDefault()
                         let lastContributor = lastEdit != null ? lastEdit.Athlete : null
                         let lastContributorDate = lastEdit != null ? (DateTime?)lastEdit.Time : null
                         //let lastContributor = i.VenueEdits.OrderByDescending(e => e.Time).Select(e => e.Athlete).FirstOrDefault()
                         select new { Venue = i, LastContributor = lastContributor, LastContributorDate = lastContributorDate }).Take(100).ToList();
            var venues = (from i in items
                          orderby i.Venue.Name
                          select VenueWebModel.FromVenue(i.Venue, null, i.LastContributor, i.LastContributorDate)).ToList();

            return venues;
        }

        public List<VenueWebModel> FindVenues(
            Location location, bool searchLargerRadiusIfNeeded,
            string nameQuery, 
            bool snookerVenues,
            bool requireSnookerTables, bool require12ftSnookerTables)
        {
            try
            {
                List<int> kmAway = new List<int> { 100, 1000, 10000 };
                if (string.IsNullOrEmpty(nameQuery) == false && nameQuery.Length > 1)
                    kmAway.Add(1000000);

                //List<VenueWebModel> venuesToReturn = new List<VenueWebModel>();

                foreach (int km in kmAway)
                {
                    var query = (from i in db.Venues.Include("Metro")
                                 where i.IsSnooker == snookerVenues
                                 select i);
                    if (string.IsNullOrEmpty(nameQuery) == false)
                    {
                        query = (from i in db.Venues.Include("Metro")
                                 where i.IsSnooker == snookerVenues
                                 where i.Name.Contains(nameQuery)
                                 select i);

                        //var list = query.ToList();
                        //int df1345 = 1;
                    }

                    if (require12ftSnookerTables == true)
                        query = (from i in query
                                 where i.NumberOf12fSnookerTables > 0
                                 select i);
                    else if (requireSnookerTables == true)
                        query = (from i in query
                                 where i.NumberOf10fSnookerTables > 0 || i.NumberOf12fSnookerTables > 0
                                 select i);

                    if (location != null)
                    {
                        Location location2 = location.OffsetRoughly(km * 1000, km * 1000);
                        double latRange = System.Math.Abs(location2.Latitude - location.Latitude);
                        double lonRange = System.Math.Abs(location2.Longitude - location.Longitude);

                        query = (from i in query
                                 where i.Latitude != null && i.Longitude != null
                                 where i.Latitude > location.Latitude - latRange && i.Latitude < location.Latitude + latRange
                                 where i.Longitude > location.Longitude - lonRange && i.Longitude < location.Longitude + lonRange
                                 orderby System.Math.Abs(i.Latitude.Value - location.Latitude) + System.Math.Abs(i.Longitude.Value - location.Longitude)
                                 select i);
                    }

                    var items = (from i in query
                                 let lastEdit = i.VenueEdits.OrderByDescending(e => e.Time).FirstOrDefault()
                                 let lastContributor = lastEdit != null ? lastEdit.Athlete : null
                                 let lastContributorDate = lastEdit != null ? (DateTime?)lastEdit.Time : null
                                 //let lastContributor = i.VenueEdits.OrderByDescending(e => e.Time).Select(e => e.Athlete).FirstOrDefault()
                                 select new { Venue = i, LastContributor = lastContributor, LastContributorDate = lastContributorDate }).Take(100).ToList();
                    var venues = (from i in items
                                  select VenueWebModel.FromVenue(i.Venue, location, i.LastContributor, i.LastContributorDate)).ToList();

                    if (location == null || searchLargerRadiusIfNeeded == false)
                        return venues;
                    if (venues.Count > 1)
                        return venues;
                    if (venues.Count == 1 && km == kmAway.Last())
                        return venues;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return new List<VenueWebModel>();
        }

        public List<VenueWebModel> FindVenues2(
            Location location,
            double radiusInMeters,
            Country country,
            string nameQuery,
            bool snookerVenues)
        {
            try
            {
                var query = (from i in db.Venues.Include("Metro")
                             where i.IsSnooker == snookerVenues
                             select i);
                if (string.IsNullOrEmpty(nameQuery) == false)
                {
                    query = (from i in db.Venues.Include("Metro")
                             where i.IsSnooker == snookerVenues
                             where i.Name.Contains(nameQuery)
                             select i);
                }

                if (country != null)
                    query = (from i in query
                             where i.Metro.Country == country.ThreeLetterCode
                             select i);

                if (location != null)
                {
                    Location location2 = location.OffsetRoughly(radiusInMeters * 1.5, radiusInMeters * 1.5);
                    double latRange = System.Math.Abs(location2.Latitude - location.Latitude);
                    double lonRange = System.Math.Abs(location2.Longitude - location.Longitude);

                    query = (from i in query
                             where i.Latitude != null && i.Longitude != null
                             where i.Latitude > location.Latitude - latRange && i.Latitude < location.Latitude + latRange
                             where i.Longitude > location.Longitude - lonRange && i.Longitude < location.Longitude + lonRange
                             orderby System.Math.Abs(i.Latitude.Value - location.Latitude) + System.Math.Abs(i.Longitude.Value - location.Longitude)
                             select i);
                }

                var items = (from i in query
                             let lastEdit = i.VenueEdits.OrderByDescending(e => e.Time).FirstOrDefault()
                             let lastContributor = lastEdit != null ? lastEdit.Athlete : null
                             let lastContributorDate = lastEdit != null ? (DateTime?)lastEdit.Time : null
                             //let lastContributor = i.VenueEdits.OrderByDescending(e => e.Time).Select(e => e.Athlete).FirstOrDefault()
                             select new { Venue = i, LastContributor = lastContributor, LastContributorDate = lastContributorDate }).Take(1000).ToList();
                var venues = (from i in items
                              select VenueWebModel.FromVenue(i.Venue, location, i.LastContributor, i.LastContributorDate)).ToList();
                venues = (from i in venues
                          orderby i.DistanceInMeters
                          select i).ToList();

                return venues;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Venue RegisterVenue(Venue venue, int athleteID)
        {
            // make sure this venue hasn't been registered yet
            double range = 0.5;
            var existingVenues = (from v in db.Venues
                                  where v.Latitude > venue.Latitude - range && v.Latitude < venue.Latitude + range
                                  where v.Longitude > venue.Longitude - range && v.Longitude < venue.Longitude + range
                                  where v.IsSnooker == venue.IsSnooker
                                  where string.Compare(v.Name, venue.Name, true) == 0
                                  select v).ToList();
            if (existingVenues.Count > 0)
                throw new Exception("A venue by this name at this location already exists");

            // clean-up/validate the country name
            string countryName = venue.Country;
            if (string.IsNullOrEmpty(countryName))
                throw new Exception("Country is not specified");
            Country country = Country.Get(countryName);
            if (country == null)
                throw new Exception("Unknown country");
            countryName = country.ThreeLetterCode;

            // create a row in the db
            var createdVenue = new Venue()
            {
                Name = venue.Name,
                MetroID = venue.MetroID,
                Latitude = venue.Latitude,
                Longitude = venue.Longitude,
                Country = countryName,
                IsSnooker = venue.IsSnooker,
                TimeCreated = DateTime.UtcNow,
                CreatedByAthleteID = athleteID,

                NumberOf10fSnookerTables = venue.NumberOf10fSnookerTables,
                NumberOf12fSnookerTables = venue.NumberOf12fSnookerTables
            };
            db.Venues.Add(createdVenue);
            db.SaveChanges();

            return createdVenue;
        }
    }
}
