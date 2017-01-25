using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    public class CacheHelper
    {
        public async Task LoadFromWebserviceIfNecessary_People(CacheService cache, List<Result> results, List<Score> scores)
        {
            List<int> ids = new List<int>();
            foreach (var result in results)
            {
                if (ids.Contains(result.AthleteID) == false)
                    ids.Add(result.AthleteID);
                if (result.OpponentAthleteID != null && ids.Contains(result.OpponentAthleteID.Value) == false)
                    ids.Add(result.OpponentAthleteID.Value);
            }
            foreach (var score in scores)
            {
                if (ids.Contains(score.AthleteAID) == false)
                    ids.Add(score.AthleteAID);
                if (ids.Contains(score.AthleteBID) == false)
                    ids.Add(score.AthleteBID);
            }

            await cache.LoadFromWebserviceIfNecessary_People(ids);
        }

        public void LoadNamesFromCache(CacheService cache, List<SnookerMatchScore> matches)
        {
            foreach (var match in matches)
            {
                var person = match.YourAthleteID > 0 ? cache.People.Get(match.YourAthleteID) : null;
                match.YourName = person != null ? person.Name : "";

                person = match.OpponentAthleteID > 0 ? cache.People.Get(match.OpponentAthleteID) : null;
                match.OpponentName = person != null ? person.Name : "";
            }
        }

        public void LoadNamesFromCache(CacheService cache, List<SnookerBreak> breaks)
        {
            foreach (var snookerBreak in breaks)
            {
                var person = snookerBreak.AthleteID > 0 ? cache.People.Get(snookerBreak.AthleteID) : null;
                snookerBreak.AthleteName = person != null ? person.Name : "";

                person = snookerBreak.OpponentAthleteID > 0 ? cache.People.Get(snookerBreak.OpponentAthleteID) : null;
                snookerBreak.OpponentName = person != null ? person.Name : "";
            }
        }
    }
}
