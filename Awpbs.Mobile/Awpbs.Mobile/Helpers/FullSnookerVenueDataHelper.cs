using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    public class FullSnookerVenueData
    {
        public int VenueID { get; set; }

        public VenueWebModel Venue { get; set; }
        public List<SnookerBreak> Breaks { get; set; }
        public List<SnookerMatchScore> Matches { get; set; }
        public List<PersonBasicWebModel> People { get; set; }
        public List<GameHostWebModel> GameHosts { get; set; }

        public List<GameHostWebModel> GameHostsInThePast { get; set; }
        public List<GameHostWebModel> GameHostsInTheFuture { get; set; }

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

    public class FullSnookerVenueDataHelper
    {
        public async Task<FullSnookerVenueData> Load(int venueID)
        {
            FullSnookerVenueData data = new FullSnookerVenueData();
            data.VenueID = venueID;

            VenueWebModel venue = await App.WebService.GetVenue(venueID);
            if (venue == null)
                return null;
            data.Venue = venue;

            App.Cache.Venues.Put(venue);

            var webResults = await App.WebService.GetResultsAtVenue(venueID);
            if (webResults != null)
            {
                var results = (from r in webResults
                               select r.ToResult()).ToList();
                data.Breaks = (from i in results
                               select SnookerBreak.FromResult(i)).ToList();
            }

            var scores = await App.WebService.GetScoresAtVenue(venueID);
            if (scores != null)
                data.Matches = (from i in scores
                                select SnookerMatchScore.FromScore(0, i)).ToList();

            var gameHosts = await App.WebService.GetGameHostsAtVenue(venueID, true);
            if (gameHosts != null)
            {
                data.GameHosts = (from i in gameHosts
                                  orderby i.When descending
                                  select i).ToList();
                data.GameHostsInThePast = (from i in gameHosts
                                           where i.When < DateTime.UtcNow
                                           orderby i.When descending
										   select i).ToList();
                data.GameHostsInTheFuture = (from i in gameHosts
                                             where i.When >= DateTime.UtcNow
                                             orderby i.When ascending
                                             select i).ToList();
            }

            await loadPeople(data);

            this.putPlaceholdersIfInternetIssues(data);
            return data;
        }

        void putPlaceholdersIfInternetIssues(FullSnookerVenueData data)
        {
            if (data.Breaks != null)
                foreach (var b in data.Breaks)
                {
                    if (b.AthleteID > 0 && string.IsNullOrEmpty(b.AthleteName))
                        b.AthleteName = "name not loaded";
                    if (b.OpponentAthleteID > 0 && string.IsNullOrEmpty(b.OpponentName))
                        b.OpponentName = "name not loaded";
                    if (b.VenueID > 0 && string.IsNullOrEmpty(b.VenueName))
                        b.VenueName = "venue not loaded";
                }

            if (data.Matches != null)
                foreach (var m in data.Matches)
                {
                    if (m.YourAthleteID > 0 && string.IsNullOrEmpty(m.YourName))
                        m.YourName = "name not loaded";
                    if (m.OpponentAthleteID > 0 && string.IsNullOrEmpty(m.OpponentName))
                        m.OpponentName = "name not loaded";
                    if (m.VenueID > 0 && string.IsNullOrEmpty(m.VenueName))
                        m.VenueName = "venue not loaded";
                }
        }

        async Task loadPeople(FullSnookerVenueData data)
        {
            if (data.Breaks == null || data.Matches == null)
            {
                data.People = null;
                return;
            }

            List<int> peopleIDs = new List<int>();

            List<int> peopleids1 = (from b in data.Breaks
                                    where b.AthleteID > 0
                                    select b.AthleteID).Distinct().ToList();
            List<int> peopleids2 = (from b in data.Breaks
                                    where b.OpponentAthleteID > 0
                                    select b.OpponentAthleteID).Distinct().ToList();
            List<int> peopleids3 = (from b in data.Matches
                                    where b.YourAthleteID > 0
                                    select b.YourAthleteID).Distinct().ToList();
            List<int> peopleids4 = (from b in data.Matches
                                    where b.OpponentAthleteID > 0
                                    select b.OpponentAthleteID).Distinct().ToList();

            peopleIDs.AddRange(peopleids1);
            foreach (var id in peopleids2)
                if (peopleIDs.Contains(id) == false)
                    peopleIDs.Add(id);
            foreach (var id in peopleids3)
                if (peopleIDs.Contains(id) == false)
                    peopleIDs.Add(id);
            foreach (var id in peopleids4)
                if (peopleIDs.Contains(id) == false)
                    peopleIDs.Add(id);

            await App.Cache.LoadFromWebserviceIfNecessary_People(peopleIDs);
            data.People = App.Cache.People.Get(peopleIDs);

            if (data.People != null)
            {
                foreach (var b in data.Breaks)
                {
                    PersonBasicWebModel person = b.AthleteID > 0 ? App.Cache.People.Get(b.AthleteID) : null;
                    if (person != null)
                        b.AthleteName = person.Name;
                    person = b.OpponentAthleteID > 0 ? App.Cache.People.Get(b.OpponentAthleteID) : null;
                    if (person != null)
                        b.OpponentName = person.Name;
                }
                foreach (var m in data.Matches)
                {
                    PersonBasicWebModel person = m.YourAthleteID > 0 ? App.Cache.People.Get(m.YourAthleteID) : null;
                    if (person != null)
                        m.YourName = person.Name;
                    person = m.OpponentAthleteID > 0 ? App.Cache.People.Get(m.OpponentAthleteID) : null;
                    if (person != null)
                        m.OpponentName = person.Name;
                }
            }
        }
    }
}
