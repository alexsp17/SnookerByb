using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class RecentDataHelper
    {
        Repository repository;

        public RecentDataHelper(Repository repository)
        {
            this.repository = repository;
        }

        public Result GetRecentResult(double upToHoursAgo)
        {
            var res = repository.GetLatestResult();
            if (res == null || res.Date == null)
                return null;
            if (res.Date > DateTime.Now)
                return null;
            if ((DateTime.Now - res.Date.Value).TotalHours > upToHoursAgo)
                return null;
            return res;
        }

        public Score GetRecentScore(double upToHoursAgo)
        {
            var score = repository.GetLatestScore();
            if (score == null || score.Date == null)
                return null;
            if (score.Date > DateTime.Now)
                return null;
            if ((DateTime.Now - score.Date).TotalHours > upToHoursAgo)
                return null;
            return score;
        }

        public int? GetRecentOpponent(double upToHoursAgo)
        {
            Result recentResult = this.GetRecentResult(upToHoursAgo);
            Score recentScore = this.GetRecentScore(upToHoursAgo);
            if (recentResult == null && recentScore == null)
                return null;

            if (recentScore == null)
                return recentResult.OpponentAthleteID;

            int myAthleteID = repository.GetMyAthleteID();
            if (recentResult == null)
                return recentScore.AthleteAID == myAthleteID ? recentScore.AthleteBID : recentScore.AthleteAID;

            if (recentResult.Date.Value > recentScore.Date)
                return recentResult.OpponentAthleteID;
            else
                return recentScore.AthleteAID == myAthleteID ? recentScore.AthleteBID : recentScore.AthleteAID;
        }

        public int? GetRecentVenue(double upToHoursAgo)
        {
            Result recentResult = this.GetRecentResult(upToHoursAgo);
            Score recentScore = this.GetRecentScore(upToHoursAgo);
            if (recentResult == null && recentScore == null)
                return null;

            if (recentScore == null)
                return recentResult.VenueID;
            if (recentResult == null)
                return recentScore.VenueID;
            if (recentScore.VenueID == null)
                return recentResult.VenueID;
            if (recentResult.VenueID == null)
                return recentScore.VenueID;

            if (recentResult.Date.Value > recentScore.Date)
                return recentResult.VenueID;
            else
                return recentScore.VenueID;
        }

        public int? GetRecentType1(double upToHoursAgo)
        {
            Result recentResult = this.GetRecentResult(upToHoursAgo);
            Score recentScore = this.GetRecentScore(upToHoursAgo);
            if (recentResult == null && recentScore == null)
                return null;

            if (recentScore == null)
                return recentResult.Type1;
            if (recentResult == null)
                return recentScore.Type1;
            if (recentScore.Type1 == null)
                return recentResult.Type1;
            if (recentResult.Type1 == null)
                return recentScore.Type1;

            if (recentResult.Date.Value > recentScore.Date)
                return recentResult.Type1;
            else
                return recentScore.Type1;
        }
    }
}
