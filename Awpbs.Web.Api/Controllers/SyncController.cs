using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/Sync")]
    public class SyncController : ApiController
    {
        private ApplicationDbContext db;

        public SyncController()
        {
            this.db = new ApplicationDbContext();
        }

        private Athlete getAthlete()
        {
            string userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                throw new Exception("userName is null");
            
            var athlete = new UserProfileLogic(db).GetAthleteForUserName(userName);
            return athlete;
        }

        [HttpPost]
        [Route("SyncScores")]
        public List<Score> SyncScores(List<Score> scoresOnMobile)
        {
            Athlete athlete = this.getAthlete();

            // scores on mobile
            List<Score> scoresOnMobile1 = (from s in scoresOnMobile
                                           where s.AthleteAID == athlete.AthleteID || s.AthleteBID == athlete.AthleteID // just in case
                                           select s).ToList();
            foreach (var score in scoresOnMobile1)
                if (score.VenueID == 0)
                    score.VenueID = null;

            // scores on the server
            List<Score> scoresOnServer = (from s in db.Scores
                                          where s.AthleteAID == athlete.AthleteID || (s.AthleteBID == athlete.AthleteID && s.AthleteBConfirmation == (int)OpponentConfirmationEnum.Confirmed)
                                          select s).ToList();

            // list of friends
            FriendshipLogic friendshipLogic = new FriendshipLogic(db);
            List<int> friends = friendshipLogic.GetFriendsQuery(athlete.AthleteID, true).Select(i => i.AthleteID).ToList();

            List<Score> newScoresConfirmed = new List<Score>();
            List<Score> newScoresNotYetConfirmed = new List<Score>();

            // update scores on the server
            foreach (var scoreOnMobile in scoresOnMobile1)
            {
                var scoreOnServer = (from r in scoresOnServer
                                     where r.Guid == scoreOnMobile.Guid
                                     select r).FirstOrDefault();

                if (scoreOnMobile.IsDeleted)
                {
                    if (scoreOnServer == null)
                        continue; // no need to store deleted scores on the server

                    if ((scoreOnMobile.TimeModified - scoreOnServer.TimeModified).TotalSeconds > 1.0)
                    {
                        // note: deleted on the mobile more recently than changed on the server => do something on the server

                        if (scoreOnServer.AthleteAID == athlete.AthleteID)
                        {
                            if (scoreOnServer.AthleteBID > 0 && scoreOnServer.AthleteBConfirmation == (int)OpponentConfirmationEnum.Confirmed)
                            {
                                // reverse A&B and make it "declined"
                                scoreOnServer.AthleteA = scoreOnServer.AthleteB = null;
                                scoreOnServer.ReverseAandB();
                                scoreOnServer.AthleteBConfirmation = (int)OpponentConfirmationEnum.Declined;
                                db.SaveChanges();
                                continue;
                            }
                        }
                        else
                        {
                            // simply consider this a "declined" score
                            scoreOnServer.AthleteBConfirmation = (int)OpponentConfirmationEnum.Declined;
                            db.SaveChanges();
                            continue;
                        }
                    }
                }

                if (scoreOnServer == null)
                {
                    // new result on the mobile -> add on the server

                    if (friends.Contains(scoreOnMobile.AthleteBID))
                    {
                        // auto-confirm for friends
                        scoreOnMobile.AthleteBConfirmation = (int)OpponentConfirmationEnum.Confirmed;
                        newScoresConfirmed.Add(scoreOnMobile);
                    }
                    else
                    {
                        scoreOnMobile.AthleteBConfirmation = (int)OpponentConfirmationEnum.NotYet;
                        newScoresNotYetConfirmed.Add(scoreOnMobile);
                    }

                    // add on the server
                    db.Scores.Add(scoreOnMobile);
                    db.SaveChanges();
                    continue;
                }

                if (scoreOnServer.AthleteAID != athlete.AthleteID)
                    continue; // do not edit opponents matches

                if (scoreOnServer.IsDifferent(scoreOnMobile, false))
                {
                    scoreOnServer.AthleteBConfirmation = (int)OpponentConfirmationEnum.NotYet;

                    // update on the server
                    scoreOnServer.IsDeleted = scoreOnMobile.IsDeleted;
                    scoreOnServer.AthleteBID = scoreOnMobile.AthleteBID;
                    scoreOnServer.Date = scoreOnMobile.Date;
                    scoreOnServer.PointsA = scoreOnMobile.PointsA;
                    scoreOnServer.PointsB = scoreOnMobile.PointsB;
                    scoreOnServer.Type1 = scoreOnMobile.Type1;
                    scoreOnServer.VenueID = scoreOnMobile.VenueID;
                    for (int i = 0; i < scoreOnMobile.InnerPointsA.Length; ++i)
                    {
                        scoreOnServer.InnerPointsA[i] = scoreOnMobile.InnerPointsA[i];
                        scoreOnServer.InnerPointsB[i] = scoreOnMobile.InnerPointsB[i];
                    }
                    scoreOnServer.ExtraData = scoreOnMobile.ExtraData;
                    db.SaveChanges();
                    continue;
                }
            }

            var scores = (from s in db.Scores
                          where s.AthleteAID == athlete.AthleteID || (s.AthleteBID == athlete.AthleteID && s.AthleteBConfirmation == (int)OpponentConfirmationEnum.Confirmed)
                          where s.IsDeleted == false // do not send deleted items back to the mobile
                          select s).ToList();

            // email people
            if (newScoresConfirmed.Count > 0 || newScoresNotYetConfirmed.Count > 0)
                Task.Run(async () =>
                {
                    await emailPeopleAboutNewScores(athlete, newScoresConfirmed, newScoresNotYetConfirmed);
                });

            return (from s in scores
                    select s.Clone()).ToList();
        }

        [HttpPost]
        [Route("SyncResults")]
        public List<ResultWebModel> SyncResults(List<ResultWebModel> resultsOnMobile)
        {
            Athlete athlete = this.getAthlete();
            
            // results on mobile
            List<Result> resultsOnMobile1 = (from r in resultsOnMobile
                                             select r.ToResult()).ToList();
            foreach (var result in resultsOnMobile1)
                if (result.AthleteID != athlete.AthleteID)
                    throw new Exception("athleteID !");
            foreach (var result in resultsOnMobile1)
                if (result.VenueID == 0)
                    result.VenueID = null;

            // results on the server
            List<Result> resultsOnServer = db.Results.Where(i => i.AthleteID == athlete.AthleteID).ToList();

            // update results on the server
            foreach (var resultOnMobile in resultsOnMobile1)
            {
                var resultOnServer = (from r in resultsOnServer
                                      where r.Guid == resultOnMobile.Guid
                                      select r).FirstOrDefault();

                if (resultOnServer == null)
                {
                    // note. new result on the mobile -> add on the server

                    if (resultOnMobile.VenueID == 0)
                        resultOnMobile.VenueID = null;
                    resultOnMobile.OpponentConfirmation = (int)OpponentConfirmationEnum.NotYet;

                    db.Results.Add(resultOnMobile);
                    db.SaveChanges();
                    continue;
                }

                if (resultOnServer.IsDifferent(resultOnMobile) &&
                    (resultOnMobile.TimeModified - resultOnServer.TimeModified).TotalSeconds > 1)
                {
                    // update on the server
                    resultOnServer.Count = resultOnMobile.Count;
                    resultOnServer.Count2 = resultOnMobile.Count2;
                    resultOnServer.Time = resultOnMobile.Time;
                    resultOnServer.Date = resultOnMobile.Date;
                    resultOnServer.Notes = resultOnMobile.Notes;
                    resultOnServer.TimeModified = resultOnMobile.TimeModified;
                    resultOnServer.VenueID = resultOnMobile.VenueID;
                    resultOnServer.Type1 = resultOnMobile.Type1;
                    resultOnServer.OpponentAthleteID = resultOnMobile.OpponentAthleteID;
                    resultOnServer.Details1 = resultOnMobile.Details1;
                    resultOnServer.IsDeleted = resultOnMobile.IsDeleted;
                    if (resultOnServer.IsDeleted == false)
                        resultOnServer.OpponentConfirmation = (int)OpponentConfirmationEnum.NotYet; // once edited, a result should be confirmed/declined by the opponent again
                    db.SaveChanges();
                    continue;
                }
            }

            var results = (from i in db.Results
                           where i.AthleteID == athlete.AthleteID
                           where i.IsDeleted == false // do not send deleted items back to the mobile
                           select i).ToList();

            List<ResultWebModel> models = (from i in results
                                           select ResultWebModel.FromResult(i)).ToList();

            return models;
        }

        [HttpPost]
        [Route("SyncMyAthlete")]
        public Athlete SyncMyAthlete(Athlete athleteOnMobile)
        {
            Athlete athlete = this.getAthlete();
            if (athlete.AthleteID != athleteOnMobile.AthleteID)
                throw new Exception("AthleteID!");

            if (athlete.IsDifferent(athleteOnMobile) == false)
                return null;

            if ((athlete.TimeModified - athleteOnMobile.TimeModified).TotalSeconds > 1)//athlete.TimeModified > athleteOnMobile.TimeModified)
            {
                return athlete; // send web athlete back to the mobile
            }
            else
            {
                // update the server's record
                athleteOnMobile.CopyTo(athlete, false);
                athlete.TimeModified = athlete.TimeModified;
                db.SaveChanges();
                return null;
            }
        }

        [HttpPost]
        [Route("PersistScoresFVO")]
        public List<Score> PersistScoresFVO(List<Score> scores)
        {
            // TO DO: !!!! Verify that the logged-in user can do this...

            List<Score> scoresPersisted = new List<Score>();

            foreach (var score in scores)
            {
                // check if already persisted
                if (db.Scores.Where(i => i.Guid == score.Guid).Count() > 0)
                {
                    scoresPersisted.Add(score);
                    continue;
                }

                // do both players have a PIN?
                bool pinsAreOk = (from i in db.Athletes
                                  where i.AthleteID == score.AthleteAID || i.AthleteID == score.AthleteBID
                                  where i.Pin != null && i.Pin != ""
                                  select i).Count() == 2;
                if (pinsAreOk)
                    score.AthleteBConfirmation = (int)OpponentConfirmationEnum.Confirmed;

                // save
                db.Scores.Add(score);
                db.SaveChanges();
                scoresPersisted.Add(score);
            }

            return scoresPersisted;
        }

        [HttpPost]
        [Route("PersistResultsFVO")]
        public List<ResultWebModel> PersistResultsFVO(List<ResultWebModel> results)
        {
            // TO DO: !!!! Verify that the logged-in user can do this...

            List<ResultWebModel> resultsPersisted = new List<ResultWebModel>();

            foreach (var result in results)
            {
                // check if already persisted
                if (db.Results.Where(i => i.Guid == result.Guid).Count() > 0)
                {
                    resultsPersisted.Add(result);
                    continue;
                }

                // do both players have a PIN?
                bool pinsAreOk = (from i in db.Athletes
                                  where i.AthleteID == result.AthleteID || i.AthleteID == result.OpponentAthleteID
                                  where i.Pin != null && i.Pin != ""
                                  select i).Count() == 2;
                if (pinsAreOk)
                    result.OpponentConfirmation = OpponentConfirmationEnum.Confirmed;
                else
                    result.OpponentConfirmation = OpponentConfirmationEnum.NotYet;

                // save
                var entity = result.ToResult();
                db.Results.Add(entity);
                db.SaveChanges();
                result.ResultID = entity.ResultID;
                resultsPersisted.Add(result);
            }

            return resultsPersisted;
        }

        async Task emailPeopleAboutNewScores(Athlete me, List<Score> newScoresConfirmed, List<Score> newScoresNotYetConfirmed)
        {
            try
            {
                ApplicationDbContext db = new ApplicationDbContext();

                string link1 = new DeepLinkHelper().BuildLinkToSync(me.AthleteID);
                string link2 = new DeepLinkHelper().BuildOpenBybLink_Sync(me.AthleteID);
                string myName = me.NameOrUserName;

                foreach (Score score in newScoresConfirmed)
                {
                    var athlete = db.Athletes.Single(i => i.AthleteID == score.AthleteBID);

                    // build the email
                    string strScore = score.PointsA + " : " + score.PointsB;
                    if (score.PointsA < score.PointsB)
                        strScore += " (you won)";
                    else if (score.PointsA > score.PointsB)
                        strScore += " (you lost)";
                    string html = string.Format(htmlMessage_NewConfirmedScore, myName, strScore, score.Date.ToShortDateString(), link1, link2);

                    // send the email
                    await new EmailService().SendEmailToAthlete(athlete, "New match recorded and auto-confirmed", html);
                }

                foreach (Score score in newScoresNotYetConfirmed)
                {
                    var athlete = db.Athletes.Single(i => i.AthleteID == score.AthleteBID);

                    // build the email
                    string strScore = score.PointsA + " : " + score.PointsB;
                    if (score.PointsA < score.PointsB)
                        strScore += " (you won)";
                    else if (score.PointsA > score.PointsB)
                        strScore += " (you lost)";
                    string html = string.Format(htmlMessage_NewNotConfirmedScore, myName, strScore, score.Date.ToShortDateString(), link1, link2);

                    // send the email
                    await new EmailService().SendEmailToAthlete(athlete, "New match recorded & needs your confirmation", html);
                }
            }
            catch (Exception)
            {
            }
        }

        string htmlMessage_NewConfirmedScore =
@"<html>
<head>
</head>
<body>
    <p>New match recorded by your friend against you. The match was auto-approved because <span style=""font-weight:bold"">{0}</span> is your friend.</p>

    <p><span style=""color:gray;"" >Score: </span><span style = ""font-weight:bold"">{1}</span></p>
    <p><span style=""color:gray;"">When played: </span><span style = ""font-weight:bold;text-color:black"">{2}</span></p>

    <p>Open in Snooker Byb:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{3}"">Open</a>
    </p>

    <p>If doesn't work, try this:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{4}"">Open</a>
    </p>
    <p>
        <span style=""color:gray;"">Snooker Byb Team</span>: This function is in early beta. Please excuse our bugs. Feel free to shoot us a message with your suggestions and ideas: <a>team@snookerbyb.com</a>
    </p>
</body>
</html>";
        string htmlMessage_NewNotConfirmedScore =
@"<html>
<head>
</head>
<body>
    <p>New match recorded by <span style=""font-weight:bold"">{0}</span> against you. Open Snooker Byb app and tap on the icon in the right-top corner to confirm this match.</p>

    <p style=""color:gray;"">Tip: 'Friend' this person to get your matches automatically confirmed.</p>

    <p><span style=""color:gray;"" >Score: </span><span style = ""font-weight:bold"">{1}</span></p>
    <p><span style=""color:gray;"">When played: </span><span style = ""font-weight:bold;text-color:black"">{2}</span></p>

    <p>Open in Snooker Byb:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{3}"">Open</a>
    </p>

    <p>If doesn't work, try this:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{4}"">Open</a>
    </p>
    <p>
        <span style=""color:gray;"">Snooker Byb Team</span>: This function is in early beta. Please excuse our bugs. Feel free to shoot us a message with your suggestions and ideas: <a>team@snookerbyb.com</a>
    </p>
</body>
</html>";
    }
}