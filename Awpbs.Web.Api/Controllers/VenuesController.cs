using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/Venues")]
    public class VenuesController : ApiController
    {
        private ApplicationDbContext db;

        public VenuesController()
        {
            this.db = new ApplicationDbContext();
        }

        public VenueWebModel Get(int id)
        {
            return new VenuesLogic(db).Get(id);
        }

        public IEnumerable<VenueWebModel> Get(string ids)
        {
            string[]strIDs = ids.Split(',');

            List<VenueWebModel> venues = new List<VenueWebModel>();
            foreach (string strID in strIDs)
            {
                int id = int.Parse(strID);
                venues.Add(new VenuesLogic(db).Get(id));
            }

            return venues;
        }

        //[HttpGet]
        //[Route("Find", Name = "Find")]
        //public IEnumerable<VenueWebModel> Find(double? latitude, double? longitude, string searchQuery, bool snooker)
        //{
        //    Location location = null;
        //    if (latitude != null && longitude != null) 
        //        location = new Location(latitude.Value, longitude.Value);

        //    List<Venue> venues = new VenuesLogic(db).FindVenues(location, true, searchQuery, snooker, false, false);
        //    return (from i in venues
        //            select VenueWebModel.FromVenue(i, location)).ToList();
        //}

        [HttpGet]
        [Route("FindSnooker", Name = "FindSnooker")]
        public IEnumerable<VenueWebModel> FindSnooker(double? latitude, double? longitude, string searchQuery, bool requireSnookerTables, bool require12ftSnookerTables)
        {
            Location location = null;
            if (latitude != null && longitude != null)
                location = new Location(latitude.Value, longitude.Value);

            List<VenueWebModel> venues = new VenuesLogic(db).FindVenues(location, true, searchQuery, true, requireSnookerTables, require12ftSnookerTables);
            return venues;
        }

        [HttpGet]
        [Route("FindSnooker2", Name = "FindSnooker2")]
        public FindVenuesWebModel FindSnooker2(int radiusInMeters, string country, int maxCount, string searchQuery = "", double? latitude = null, double? longitude = null)
        {
            Location location = null;
            if (latitude != null && longitude != null)
                location = new Location(latitude.Value, longitude.Value);
            Country countryObj = Country.Get(country);

            // load venues
            var loadedVenues = new VenuesLogic(db).FindVenues2(location, radiusInMeters, countryObj, searchQuery, true);
            int allVenuesCount = loadedVenues.Count;

            // keep up to maxCount venues
            List<VenueWebModel> venues;
            if (allVenuesCount <= maxCount)
            {
                venues = loadedVenues;
            }
            else
            {
                venues = new List<VenueWebModel>();
                Random random = new Random();
                for (int i = 0; i < maxCount; ++i)
                {
                    int index = random.Next(loadedVenues.Count - 1);
                    venues.Add(loadedVenues[index]);
                    loadedVenues.RemoveAt(index);
                }
            }

            FindVenuesWebModel model = new FindVenuesWebModel();
            model.Venues = venues;
            if (location != null)
                model.ClosestMetro = new MetrosLogic(db).GetClosestMetro(location);
            model.TotalCountAvailable = allVenuesCount;

            return model;
        }

        [Authorize]
        public int Post([FromBody]VenueWebModel venue)
        {
            var athlete = new UserProfileLogic(db).GetAthleteForUserName(User.Identity.Name);

            var metros = new MetrosLogic(db).GetMetrosAround(venue.Location);
            if (metros.Count == 0)
                throw new Exception("No metros around this location: " + venue.Location.ToString());
            var metro = metros[0];
            venue.MetroID = metro.ID;
            venue.Country = metro.Country;

            Venue venueToCreate = venue.ToVenue();
            Venue createdVenue = new VenuesLogic(db).RegisterVenue(venueToCreate, athlete.AthleteID);
            return createdVenue.VenueID;
        }

        [HttpPost]
        [Authorize]
        [Route("VerifyOrEditVenue")]
        public bool VerifyOrEditVenue(VenueEditWebModel venueEdit)
        {
            var athlete = new UserProfileLogic(db).GetAthleteForUserName(User.Identity.Name);
            var venue = db.Venues.Single(i => i.VenueID == venueEdit.VenueID);

            string backup = string.Format("{0}|||||{1}|||||{2}|||||{3}|||||{4}|||||{5}|||||{6}",
                venue.PhoneNumber ?? "", venue.Website ?? "", venue.Address ?? "", venue.PoiID ?? "", venue.NumberOf10fSnookerTables ?? -1, venue.NumberOf12fSnookerTables ?? -1, venue.IsInvalid);

            bool isEdited = false;
            if (venueEdit.NumberOf10fSnookerTables != null && venue.NumberOf10fSnookerTables != venueEdit.NumberOf10fSnookerTables)
            {
                isEdited = true;
                venue.NumberOf10fSnookerTables = venueEdit.NumberOf10fSnookerTables;
            }
            if (venueEdit.NumberOf12fSnookerTables != null && venue.NumberOf12fSnookerTables != venueEdit.NumberOf12fSnookerTables)
            {
                isEdited = true;
                venue.NumberOf12fSnookerTables = venueEdit.NumberOf12fSnookerTables;
            }
            if (venueEdit.HasAddress != venue.HasAddress)
            {
                isEdited = true;
                venue.Address = venueEdit.Address;
            }
            if (venueEdit.HasWebsite != venue.HasWebsite)
            {
                isEdited = true;
                venue.Website = venueEdit.Website;
            }
            if (venueEdit.HasPhoneNumber != venue.HasPhoneNumber)
            {
                isEdited = true;
                venue.PhoneNumber = venueEdit.PhoneNumber;
            }
            if (venueEdit.HasPoiID != venue.HasPOIid)
            {
                isEdited = true;
                venue.PoiID = venueEdit.PoiID;
            }
            if (venueEdit.IsInvalid != venue.IsInvalid)
            {
                isEdited = true;
                venue.IsInvalid = venueEdit.IsInvalid;
            }

            VenueEditTypeEnum type = VenueEditTypeEnum.ConfirmOnly;
            if (isEdited)
                type = VenueEditTypeEnum.EditedMeta;

            VenueEdit edit = new VenueEdit()
            {
                AthleteID = athlete.AthleteID,
                VenueID = venue.VenueID,
                Time = DateTime.UtcNow,
                Type = (int)type,
                Backup = backup
            };
            db.VenueEdits.Add(edit);
            db.SaveChanges();

            return true;
        }
    }
}