using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class PeopleLogic
    {
        private const int maxLoadFromTheDatabase = 1000;
        //private const int maxCount = 30;

        private readonly ApplicationDbContext db;

        public PeopleLogic(ApplicationDbContext context)
        {
            this.db = context;
        }

        public PersonFullWebModel GetFull(int myAthleteID, int athleteID)
        {
            return this.getFull(myAthleteID, athleteID);
        }

        public PersonBasicWebModel GetBasic(int myAthleteID, int athleteID)
        {
            var query = db.Athletes.Where(i => i.AthleteID == athleteID);
            var list = getBasic(myAthleteID, query);
            if (list.Count == 1)
                return list[0];
            return null;
        }

        public List<PersonBasicWebModel> GetFriends(int myAthleteID, int athleteID)
        {
            bool confirmedOnly = myAthleteID != athleteID;
            var query = new FriendshipLogic(db).GetFriendsQuery(athleteID, confirmedOnly);
            return getBasic(myAthleteID, query, 1000);
        }

        public List<PersonBasicWebModel> GetPlayedAtVenue(int myAthleteID, int venueID)
        {
            var query = (from a in db.Athletes
                         where
                            a.Results.Where(r => r.VenueID == venueID && r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined).Count() > 0 || 
                            a.ScoreAs.Where(s => s.VenueID == venueID && s.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined).Count() > 0 ||
                            a.ScoreBs.Where(s => s.VenueID == venueID && s.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined).Count() > 0
                         select a);

            return getBasic(myAthleteID, query);
        }

        public List<PersonBasicWebModel> Find(int myAthleteID, string nameQuery, bool myFriendsOnly, Country country = null, int? metroID = null, bool includeMyself = false)
        {
            var query = (from a in db.Athletes
                         where includeMyself == true || a.AthleteID != myAthleteID
                         select a);
            if (myFriendsOnly)
                query = new FriendshipLogic(db).GetFriendsQuery(myAthleteID, false);

            if (string.IsNullOrEmpty(nameQuery) == false)
            {
                nameQuery = nameQuery.Trim();
                if (nameQuery != "")
                {
                    query = (from a in query//db.Athletes
                             where a.Name.Contains(nameQuery)
                             select a);
                }
            }

            if (country != null)
                query = (from a in query
                         where a.Country == country.ThreeLetterCode
                         select a);

            if (metroID != null)
                query = (from a in query
                         where a.MetroID == metroID.Value
                         select a);

            return this.getBasic(myAthleteID, query);
        }

        private List<PersonBasicWebModel> getBasic(int myAthleteID, IQueryable<Athlete> athletesQuery, int maxCount = PersonBasicWebModel.MaxItems)
        {
            var people = (from a in athletesQuery
                          let isFriend =
                                a.Friendships1.Where(f => f.Athlete2ID == myAthleteID && f.FriendshipStatus == (int)FriendshipStatusEnum.Confirmed).Count() > 0 ||
                                a.Friendships2.Where(f => f.Athlete1ID == myAthleteID && f.FriendshipStatus == (int)FriendshipStatusEnum.Confirmed).Count() > 0
                          let isFriendRequestSent =
                                a.Friendships1.Where(f => f.Athlete2ID == myAthleteID && f.FriendshipStatus == (int)FriendshipStatusEnum.Initiated).Count() > 0 ||
                                a.Friendships2.Where(f => f.Athlete1ID == myAthleteID && f.FriendshipStatus == (int)FriendshipStatusEnum.Initiated).Count() > 0
                          let metroName = db.Metros.Where(m => m.MetroID == a.MetroID).Select(m => m.Name).FirstOrDefault()
                          select new PersonBasicWebModel()
                          {
                              ID = a.AthleteID,
                              Name = a.Name,
                              MetroID = a.MetroID,
                              Metro = metroName,
                              DOB = a.DOB,
                              Gender = (GenderEnum)a.Gender,
                              Picture = a.Picture,
                              TimeCreated = a.TimeCreated,

                              IsFriend = isFriend,
                              IsFriendRequestSent = isFriendRequestSent,

                              SnookerAbout = a.SnookerAbout,
                          }).Take(maxLoadFromTheDatabase).ToList();

            if (people.Count <= maxCount)
                return people;

            // take random maxCount people
            List<PersonBasicWebModel> peopleRandom = new List<PersonBasicWebModel>();
            var random = new Random();
            for (int i = 0; i < maxCount; ++i)
            {
                var index = random.Next(people.Count - 1);
                if (index < 0 || index > people.Count)
                    break;
                var person = people[index];
                peopleRandom.Add(person);
                people.RemoveAt(index);
            }
            return peopleRandom;
        }

        private PersonFullWebModel getFull(int myAthleteID, int athleteID)
        {
            var _person = this.getBasic(myAthleteID, db.Athletes.Where(a => a.AthleteID == athleteID)).FirstOrDefault();
            if (_person == null)
                return null;

            var person = new PersonFullWebModel();
            _person.CopyTo(person);

            var athlete = (from a in db.Athletes.Include("Friendships1").Include("Friendships2").Include("ScoreAs").Include("ScoreBs").Include("Results")
                           where a.AthleteID == athleteID
                           select a).Single();

            // added in 01/25/2016:
            person.IsFriendRequestSentByThisPerson = false;
            if (person.IsFriend == false)
                if (athlete.Friendships1.Where(i => i.Athlete1ID == athleteID && i.FriendshipStatus == (int)FriendshipStatusEnum.Initiated).Count() == 1)
                    person.IsFriendRequestSentByThisPerson = true;

            // basic stats
            person.SnookerStats = new SnookerStatsModel();
            person.SnookerStats.CountContributions = (from i in athlete.VenueEdits
                                                      select i.VenueID).Distinct().Count();
            person.SnookerStats.CountBests = (from r in athlete.Results
                                              where r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                                              where r.IsDeleted == false
                                              select r).Count();
            person.SnookerStats.CountMatches =  (from s in athlete.ScoreAs
                                                 where s.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined
                                                 where s.IsDeleted == false
                                                 select s).Count()
                                                +
                                                (from s in athlete.ScoreBs
                                                 where s.AthleteBConfirmation == (int)OpponentConfirmationEnum.Confirmed
                                                 where s.IsDeleted == false
                                                 select s).Count();
            person.SnookerStats.BestBallCount = (from r in athlete.Results
                                                 where r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                                                 where r.IsDeleted == false
                                                 select r).Max(i => i.Count2) ?? 0;
            person.SnookerStats.BestPoints = (from r in athlete.Results
                                              where r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                                              where r.IsDeleted == false
                                              select r).Max(i => i.Count) ?? 0;

            // best frame score
            //var matchesA = (from i in athlete.ScoreAs
            //                select SnookerMatchScore.FromScore(myAthleteID, i));
            //var matchesB = (from i in athlete.ScoreBs
            //                where i.AthleteBConfirmation == (int)OpponentConfirmationEnum.Confirmed
            //                select SnookerMatchScore.FromScore(myAthleteID, i));
            List<SnookerMatchScore> matches = new List<SnookerMatchScore>();
            matches.AddRange(from i in athlete.ScoreAs
                             where i.IsDeleted == false
                             select SnookerMatchScore.FromScore(myAthleteID, i));
            matches.AddRange(from i in athlete.ScoreBs
                             where i.AthleteBConfirmation == (int)OpponentConfirmationEnum.Confirmed
                             where i.IsDeleted == false
                             select SnookerMatchScore.FromScore(myAthleteID, i));
            person.SnookerStats.BestFrameScore = (from m in matches
                                                  from f in m.FrameScores
                                                  select (int?)f.A).Max() ?? 0;
            //int maxA = (from m in matchesA
            //            from f in m.FrameScores
            //            select (int?)f.A).Max() ?? 0;
            //int maxB = (from m in matchesB
            //            from f in m.FrameScores
            //            select (int?)f.B).Max() ?? 0;
            //person.SnookerStats.BestFrameScore = System.Math.Max(maxA, maxB);

            // number of venues
            var venues1 = (from r in athlete.Results
                           where r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                           where r.IsDeleted == false
                           where r.VenueID != null
                           select r.VenueID.Value).Distinct().ToList();
            var venues2 = (from s in athlete.ScoreAs
                           where s.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined
                           where s.IsDeleted == false
                           where s.VenueID != null
                           select s.VenueID.Value).Distinct().ToList();
            var venues3 = (from s in athlete.ScoreBs
                           where s.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined
                           where s.IsDeleted == false
                           where s.VenueID != null
                           select s.VenueID.Value).Distinct().ToList();
            List<int> venues = venues1.ToList();
            foreach (var i in venues2)
                if (venues.Contains(i) == false)
                    venues.Add(i);
            foreach (var i in venues3)
                if (venues.Contains(i) == false)
                    venues.Add(i);
            person.SnookerStats.CountVenuesPlayed  = venues.Count();

            // number of opponents
            var opponents1 = (from r in athlete.Results
                              where r.OpponentAthleteID != null && r.OpponentAthleteID != 0
                              where r.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                              where r.IsDeleted == false
                              select r.OpponentAthleteID.Value).Distinct().ToList();
            var opponents2 = (from s in athlete.ScoreAs
                              where s.AthleteBID != 0
                              where s.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined
                              where s.IsDeleted == false
                              select s.AthleteBID).Distinct().ToList();
            var opponents3 = (from s in athlete.ScoreBs
                              where s.AthleteAID != 0
                              where s.AthleteBConfirmation != (int)OpponentConfirmationEnum.Declined
                              where s.IsDeleted == false
                              select s.AthleteAID).Distinct().ToList();
            List<int> opponents = opponents1.ToList();
            foreach (var i in opponents2)
                if (opponents.Contains(i) == false)
                    opponents.Add(i);
            foreach (var i in opponents3)
                if (opponents.Contains(i) == false)
                    opponents.Add(i);
            person.SnookerStats.CountOpponentsPlayed = opponents.Count;

            return person;
        }
    }
}
