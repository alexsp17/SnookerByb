using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/Scores")]
    public class ScoresController : ApiController
    {
        private ApplicationDbContext db;

        public ScoresController()
        {
            this.db = new ApplicationDbContext();
        }

        [HttpGet]
        public List<Score> Get(int athleteID)
        {
            var athlete = db.Athletes.SingleOrDefault(i => i.AthleteID == athleteID);

            var scores = (from score in db.Scores
                          where score.AthleteAID == athleteID || score.AthleteBID == athleteID
                          where score.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined
                          where score.IsDeleted == false
                          orderby score.Date descending
                          select score).ToList();

            return (from i in scores
                    select i.Clone()).ToList();
        }

        [HttpGet]
        [Route("ScoresAtVenue")]
        public List<Score> ScoresAtVenue(int venueID)
        {
            var scores = (from score in db.Scores
                          where score.VenueID == venueID
                          where score.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined
                          where score.IsDeleted == false
                          orderby score.Date descending
                          select score).ToList();

            return (from i in scores
                    select i.Clone()).ToList();
        }

        [HttpGet]
        [Authorize]
        public List<Score> ScoresToConfirm()
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            var scores = (from r in db.Scores
                          where r.AthleteBConfirmation == (int)OpponentConfirmationEnum.NotYet
                          where r.AthleteBID == myAthleteID
                          where r.IsDeleted == false
                          orderby r.Date descending
                          select r).ToList();
            return (from i in scores
                    select i.Clone()).ToList();
        }

        [Route("ConfirmScore")]
        [HttpPost]
        [Authorize]
        public bool ConfirmScore(int scoreID, bool confirm)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);

            var score = db.Scores.Where(s => s.ScoreID == scoreID).Single();
            if (score.AthleteBID != myAthleteID)
                throw new Exception("Cannot confirm somebody else's score");

            score.AthleteBConfirmation = (int)(confirm ? OpponentConfirmationEnum.Confirmed : OpponentConfirmationEnum.Declined);
            db.SaveChanges();

            return true;
        }
    }
}