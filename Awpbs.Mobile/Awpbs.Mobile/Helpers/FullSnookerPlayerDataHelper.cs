using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    public class FullSnookerPlayerData
    {
        public int AthleteID { get; set; }

		public bool InternetIssues { get; set; }

        public PersonFullWebModel Person { get; set; }
        public List<SnookerBreak> Breaks { get; set; }
        public List<SnookerMatchScore> Matches { get; set; }
        public List<SnookerVenuePlayed> VenuesPlayed { get; set; }
        public List<SnookerOpponent> Opponents { get; set; }

        public int? BestBreak
        {
            get
            {
                if (Breaks == null || Breaks.Count == 0)
                    return null;
                return Breaks.Max(i => i.Points);
            }
        }

        public int? BestFrame
        {
            get
            {
                if (Matches == null || Matches.Count == 0)
                    return null;
                var frames = (from i in Matches
                              where i.HasFrameScores
                              from f in i.FrameScores
                              select f).ToList();
                if (frames.Count == 0)
                    return null;
                return frames.Max(i => i.A);
            }
        }
    }

    public class FullSnookerPlayerDataHelper
    {
        public async Task<FullSnookerPlayerData> Load(int athleteID)
        {
            if (App.Repository.GetMyAthleteID() == athleteID)
                return await LoadForMe();
            else
                return await LoadForPlayer(athleteID);
        }

        public async Task<FullSnookerPlayerData> LoadForMe()
        {
            DateTime timeBegin = DateTime.Now;

            FullSnookerPlayerData data = new FullSnookerPlayerData();
            data.AthleteID = App.Repository.GetMyAthleteID();

            var results = App.Repository.GetResults(data.AthleteID, false);
            data.Breaks = (from i in results
                           select SnookerBreak.FromResult(i)).ToList();

            var scores = App.Repository.GetScores(false);
            data.Matches = (from i in scores
                            select SnookerMatchScore.FromScore(data.AthleteID, i)).ToList();

            await loadOpponents(data);
            await loadVenues(data);
            data.Person = await App.WebService.GetPersonByID(data.AthleteID);
			if (data.Person == null)
			{
				data.InternetIssues = true;

				var athlete = App.Repository.GetAthlete (data.AthleteID);
				data.Person = new PersonFullWebModel ();
				data.Person.CopyFrom(athlete);
			}

            this.putPlaceholdersIfInternetIssues(data);

			if (data.InternetIssues == false && data.Person != null)
				await App.Cache.LoadFromWebserviceIfNecessary_Metro(data.Person.MetroID);

            TraceHelper.TraceTimeSpan(timeBegin, DateTime.Now, "FullSnookerPlayerDataHelper.LoadForMe");
            return data;
        }

        public async Task<FullSnookerPlayerData> LoadForPlayer(int athleteID)
        {
            FullSnookerPlayerData data = new FullSnookerPlayerData();
            data.AthleteID = athleteID;

            var webResults = await App.WebService.GetResults(data.AthleteID);
			if (webResults != null)
			{
				var results = (from r in webResults
				               select r.ToResult ()).ToList ();
				data.Breaks = (from i in results
				               select SnookerBreak.FromResult (i)).ToList ();
			}
			else
			{
				data.InternetIssues = true;
			}

            var scores = await App.WebService.GetScores(data.AthleteID);
			if (scores != null)
			{
				data.Matches = (from i in scores
				                select SnookerMatchScore.FromScore (data.AthleteID, i)).ToList ();
			}
			else
			{
				data.InternetIssues = true;
			}

            await loadOpponents(data);
            await loadVenues(data);
            data.Person = await App.WebService.GetPersonByID(data.AthleteID);
			if (data.Person == null)
			{
				data.InternetIssues = true;
			}

            this.putPlaceholdersIfInternetIssues(data);
            return data;
        }

        void putPlaceholdersIfInternetIssues(FullSnookerPlayerData data)
        {
            if (!(data.Opponents == null && data.VenuesPlayed == null))
                return;

            if (data.Breaks != null)
                foreach (var b in data.Breaks)
                {
                    if (b.OpponentAthleteID > 0 && string.IsNullOrEmpty(b.OpponentName))
                        b.OpponentName = "name not loaded";
                    if (b.VenueID > 0 && string.IsNullOrEmpty(b.VenueName))
                        b.VenueName = "venue not loaded";
                }

            if (data.Matches != null)
                foreach (var m in data.Matches)
                {
                    if (m.OpponentAthleteID > 0 && string.IsNullOrEmpty(m.OpponentName))
                        m.OpponentName = "name not loaded";
                    if (m.VenueID > 0 && string.IsNullOrEmpty(m.VenueName))
                        m.VenueName = "venue not loaded";
                }
        }

        async Task loadOpponents(FullSnookerPlayerData data)
        {
            if (data.Breaks == null || data.Matches == null)
            {
                data.Opponents = null;
                return;
            }

            List<int> opponentIDs = new List<int>();
            List<int> opponentIDs1 = (from b in data.Breaks
                                      where b.OpponentAthleteID > 0
                                      select b.OpponentAthleteID).Distinct().ToList();
            opponentIDs.AddRange(opponentIDs1);

            List<int> opponentIDs2 = (from m in data.Matches
                                      where m.OpponentAthleteID > 0
                                      select m.OpponentAthleteID).Distinct().ToList();
            foreach (int id in opponentIDs2)
                if (opponentIDs.Contains(id) == false)
                    opponentIDs.Add(id);

            List<PersonBasicWebModel> friends = await App.WebService.GetFriendsOfAthlete(data.AthleteID);
            if (friends != null)
            {
                foreach (var friend in friends)
                    if (opponentIDs.Contains(friend.ID) == false)
                        opponentIDs.Add(friend.ID);
                App.Cache.People.Put(friends);
            }

            await App.Cache.LoadFromWebserviceIfNecessary_People(opponentIDs);

            foreach (var b in data.Breaks)
                if (b.OpponentAthleteID > 0)
                {
                    var person = App.Cache.People.Get(b.OpponentAthleteID);
                    b.OpponentName = person != null ? person.Name : "";
                }
            foreach (var match in data.Matches)
                if (match.OpponentAthleteID > 0)
                {
                    var person = App.Cache.People.Get(match.OpponentAthleteID);
                    match.OpponentName = person != null ? person.Name : "";
                }

            var people = App.Cache.People.Get(opponentIDs);
            if (people == null)
            {
                data.Opponents = null;
                return;
            }

            data.Opponents = new List<SnookerOpponent>();
            foreach (var person in people)
            {
                data.Opponents.Add(new SnookerOpponent()
                    {
                        Person = person,
                        CountDraws = data.Matches.Where(m => m.OpponentAthleteID == person.ID && m.MatchScoreA == m.MatchScoreB && m.OpponentConfirmation != OpponentConfirmationEnum.Declined).Count(),
                        CountLosses = data.Matches.Where(m => m.OpponentAthleteID == person.ID && m.MatchScoreA < m.MatchScoreB && m.OpponentConfirmation != OpponentConfirmationEnum.Declined).Count(),
                        CountWins = data.Matches.Where(m => m.OpponentAthleteID == person.ID && m.MatchScoreA > m.MatchScoreB && m.OpponentConfirmation != OpponentConfirmationEnum.Declined).Count()
                    });
            }
        }

        async Task loadVenues(FullSnookerPlayerData data)
        {
            if (data.Breaks == null || data.Matches == null)
            {
                data.VenuesPlayed = null;
                return;
            }

            List<int> ids = new List<int>();
            List<int> ids1 = (from b in data.Breaks
                              where b.VenueID > 0
                              select b.VenueID).Distinct().ToList();
            ids.AddRange(ids1);

            List<int> ids2 = (from m in data.Matches
                              where m.VenueID > 0
                              select m.VenueID).Distinct().ToList();
            foreach (int id in ids2)
                if (ids.Contains(id) == false)
                    ids.Add(id);

            await App.Cache.LoadFromWebserviceIfNecessary_Venues(ids);

            foreach (var b in data.Breaks)
                if (b.VenueID > 0)
                {
                    var venue = App.Cache.Venues.Get(b.VenueID);
                    b.VenueName = venue != null ? venue.Name : "";
                }
            foreach (var match in data.Matches)
                if (match.VenueID > 0)
                {
                    var venue = App.Cache.Venues.Get(match.VenueID);
                    match.VenueName = venue != null ? venue.Name : "";
                }

            var venues = App.Cache.Venues.Get(ids);
            if (venues == null)
            {
                data.VenuesPlayed = null;
                return;
            }

            data.VenuesPlayed = new List<SnookerVenuePlayed>();
            foreach (var venue in venues)
            {
                data.VenuesPlayed.Add(new SnookerVenuePlayed()
                    {
                        Venue = venue,
                        CountBests = data.Breaks.Where(i => i.VenueID == venue.ID).Count(),
                        CountMatches = data.Matches.Where(i => i.VenueID == venue.ID).Count()
                    });
            }
        }
    }
}
