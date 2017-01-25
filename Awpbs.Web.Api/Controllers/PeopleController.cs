using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/People")]
    public class PeopleController : ApiController
    {
        private ApplicationDbContext db;

        public PeopleController()
        {
            this.db = new ApplicationDbContext();
        }

        public PersonFullWebModel Get(int athleteID)
        {
            int myAthleteID = 0;
            if (User.Identity.IsAuthenticated == true)
                myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            return new PeopleLogic(db).GetFull(myAthleteID, athleteID);
        }

        public List<PersonBasicWebModel> Get(string athleteIDs)
        {
            int myAthleteID = 0;
            if (User.Identity.IsAuthenticated == true)
                myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            string[] strIDs = athleteIDs.Split(',');

            List<PersonBasicWebModel> people = new List<PersonBasicWebModel>();
            foreach (string strID in strIDs)
            {
                int id = int.Parse(strID);
                var person = new PeopleLogic(db).GetBasic(myAthleteID, id);   // consider doing in one query!!!!!
                people.Add(person);
            }

            return people;
        }

        [HttpGet]
        [Route("Find")]
        public List<PersonBasicWebModel> Find(string nameQuery, string country, int? metroID, bool friendsOnly = false, bool includeMyself = false)
        {
            Country countryObj = null;
            if (string.IsNullOrEmpty(country) == false)
            {
                countryObj = Country.Get(country);
                if (countryObj == null)
                    throw new Exception("Unknown country");
            }

            if (metroID == 0)
                metroID = null;

            int myAthleteID = 0;
            if (User.Identity.IsAuthenticated == true)
                myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            return new PeopleLogic(db).Find(myAthleteID, nameQuery, friendsOnly, countryObj, metroID, includeMyself);
        }

        [HttpGet]
        [Route("Friends")]
        public List<PersonBasicWebModel> Friends(int athleteID)
        {
            int myAthleteID = 0;
            if (User.Identity.IsAuthenticated == true)
                myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            return new PeopleLogic(db).GetFriends(myAthleteID, athleteID);
        }

        [HttpGet]
        [Route("PlayedAtVenue")]
        public List<PersonBasicWebModel> PlayedAtVenue(int venueID)
        {
            int myAthleteID = 0;
            if (User.Identity.IsAuthenticated == true)
                myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            return new PeopleLogic(db).GetPlayedAtVenue(myAthleteID, venueID);
        }

        [Authorize]
        [HttpGet]
        [Route("HasPin")]
        public bool HasPin(int athleteID)
        {
            var db = new ApplicationDbContext();
            var athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();
            bool hasPin = string.IsNullOrEmpty(athlete.Pin) == false;
            return hasPin;
        }

        [Authorize]
        [HttpPost]
        [Route("VerifyPin")]
        public bool VerifyPin(VerifyPinWebModel model)
        {
            var db = new ApplicationDbContext();
            bool verified = new UserProfileLogic(db).VerifyPin(model.AthleteID, model.Pin);
            return verified;
        }
    }
}