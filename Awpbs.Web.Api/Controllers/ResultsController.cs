using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/Results")]
    public class ResultsController : ApiController
    {
        private ApplicationDbContext db;

        public ResultsController()
        {
            this.db = new ApplicationDbContext();
        }

        [HttpGet]
        public List<ResultWebModel> Get(int athleteID)
        {
            var athlete = db.Athletes.SingleOrDefault(i => i.AthleteID == athleteID);

            var results = (from r in athlete.Results
                           where r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                           where r.IsNotAcceptedByAthleteYet == false
                           where r.IsDeleted == false
                           select r).ToList();
            return (from i in results
                    select ResultWebModel.FromResult(i)).ToList();
        }

        [Route("ResultsAtVenue")]
        [HttpGet]
        public List<ResultWebModel> ResultsAtVenue(int venueID)
        {
            var results = (from r in db.Results
                           where r.VenueID == venueID
                           where r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                           where r.IsDeleted == false
                           orderby r.Date descending
                           select r).ToList();
            return (from i in results
                    select ResultWebModel.FromResult(i)).ToList();
        }

        [Route("ResultsToConfirm")]
        [HttpGet]
        [Authorize]
        public List<ResultWebModel> ResultsToConfirm()
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            var results = (from r in db.Results
                           where r.OpponentConfirmation == (int)OpponentConfirmationEnum.NotYet
                           where r.OpponentAthleteID == myAthleteID
                           where r.IsNotAcceptedByAthleteYet == false
                           where r.IsDeleted == false
                           orderby r.Date descending
                           select r).ToList();
            return (from i in results
                    select ResultWebModel.FromResult(i)).ToList();
        }

        [Route("ResultsNotYetAcceptedByMe")]
        [HttpGet]
        [Authorize]
        public List<ResultWebModel> ResultsNotYetAcceptedByMe()
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            var results = (from r in db.Results
                           where r.IsNotAcceptedByAthleteYet == true
                           where r.IsDeleted == false
                           orderby r.Date descending
                           select r).ToList();
            var model = (from i in results
                         select ResultWebModel.FromResult(i)).ToList();
            return model;
        }

        [Route("AddResultNotYetAcceptedByAthlete")]
        [HttpPost]
        [Authorize]
        public bool AddResultNotYetAcceptedByAthlete(ResultWebModel _result)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            try
            {
                Result result = _result.ToResult();
                if (result.VenueID == 0)
                    result.VenueID = null;
                result.IsNotAcceptedByAthleteYet = true;
                if (result.OpponentAthleteID != myAthleteID)
                    throw new Exception("OpponentAthleteID must be you");
                if (result.AthleteID == myAthleteID)
                    throw new Exception("AthleteID cannot be you");

                db.Results.Add(result);
                db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [Route("AcceptResultNotYetAcceptedByMe")]
        [HttpPost]
        [Authorize]
        public bool AcceptResultNotYetAcceptedByMe(int resultID, bool accept)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            var result = db.Results.Where(i => i.ResultID == resultID).Single();
            if (result.IsNotAcceptedByAthleteYet == false || result.AthleteID != myAthleteID)
                return false;

            if (accept)
            {
                result.IsNotAcceptedByAthleteYet = false;
                result.OpponentConfirmation = (int)OpponentConfirmationEnum.Confirmed; // consider this confirmed by the opponent, because the opponent is the one who created this
            }
            else
                db.Results.Remove(result);

            db.SaveChanges();

            return true;
        }

        [Route("ConfirmResult")]
        [HttpPost]
        [Authorize]
        public bool ConfirmResult(int resultID, bool confirm)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            var result = db.Results.Where(r => r.ResultID == resultID).Single();
            if (result.OpponentAthleteID != myAthleteID)
                throw new Exception("Cannot confirm somebody else's result");

            result.OpponentConfirmation = (int)(confirm ? OpponentConfirmationEnum.Confirmed : OpponentConfirmationEnum.Declined);
            db.SaveChanges();

            return true;
        }
    }
}