using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class FeedLogic
    {
        private readonly ApplicationDbContext db;

        public FeedLogic(ApplicationDbContext db)
        {
            this.db = db;
        }

        public int GetObjectIDFromComment(Comment comment)
        {
            switch ((NewsfeedItemTypeEnum)comment.ObjectType)
            {
                case NewsfeedItemTypeEnum.Post: return comment.PostID ?? 0;
                case NewsfeedItemTypeEnum.GameHost: return comment.GameHostID ?? 0;
                case NewsfeedItemTypeEnum.NewUser: return comment.NewUserID ?? 0;
                case NewsfeedItemTypeEnum.Result: return comment.ResultID ?? 0;
                case NewsfeedItemTypeEnum.Score: return comment.ScoreID ?? 0;
            }
            return 0;
        }

        public int MakePost(int athleteID, Country country, string text)
        {
            var athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();

            var post = new Post()
            {
                AthleteID = athleteID,
                Country = country.ThreeLetterCode,
                PostText = text,
                Time = DateTime.UtcNow,
            };
            db.Posts.Add(post);
            db.SaveChanges();

            return post.PostID;
        }

        public int MakePost(int athleteID, int metroID, string text)
        {
            var athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();
            var metro = db.Metros.Where(i => i.MetroID == metroID).Single();
            string country = metro.Country;
            if (string.IsNullOrEmpty(country))
                throw new Exception("unknown country");

            var post = new Post()
            {
                AthleteID = athleteID,
                Country = metro.Country,
                MetroID = metro.MetroID,
                PostText = text,
                Time = DateTime.UtcNow,
            };
            db.Posts.Add(post);
            db.SaveChanges();

            return post.PostID;
        }

        public void SetLike(int athleteID, NewsfeedItemTypeEnum objectType, int objectID, bool like)
        {
            var athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();

            var comment = new Comment()
            {
                AthleteID = athleteID,
                ObjectType = (int)objectType,
                CommentText = null,
                CommentType = (int)CommentTypeEnum.Like,
                Time = DateTime.UtcNow,
            };
            switch (objectType)
            {
                case NewsfeedItemTypeEnum.Post: comment.PostID = objectID; break;
                case NewsfeedItemTypeEnum.Result: comment.ResultID = objectID; break;
                case NewsfeedItemTypeEnum.Score: comment.ScoreID = objectID; break;
                case NewsfeedItemTypeEnum.GameHost: comment.GameHostID = objectID; break;
                case NewsfeedItemTypeEnum.NewUser: comment.NewUserID = objectID; break;
                default: throw new NotImplementedException("unknown objectType");
            }

            var existingComment = (from c in db.Comments
                                   where c.ObjectType == comment.ObjectType
                                   where c.AthleteID == comment.AthleteID
                                   where c.PostID == comment.PostID
                                   where c.ResultID == comment.ResultID
                                   where c.ScoreID == comment.ScoreID
                                   where c.GameHostID == comment.GameHostID
                                   where c.NewUserID == comment.NewUserID
                                   where c.CommentType == comment.CommentType
                                   select c).FirstOrDefault();

            if (existingComment != null)
                db.Comments.Remove(existingComment);
            if (like)
                db.Comments.Add(comment);
            db.SaveChanges();
        }

        public async Task<int> MakeComment(int athleteID, NewsfeedItemTypeEnum objectType, int objectID, string text, bool notifyAll)
        {
            var athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();

            // save the comment
            var comment = new Comment()
            {
                AthleteID = athleteID,
                ObjectType = (int)objectType,
                CommentText = text,
                Time = DateTime.UtcNow,
                CommentType = (int)CommentTypeEnum.Comment
            };
            switch (objectType)
            {
                case NewsfeedItemTypeEnum.Post: comment.PostID = objectID; break;
                case NewsfeedItemTypeEnum.Result: comment.ResultID = objectID; break;
                case NewsfeedItemTypeEnum.Score: comment.ScoreID = objectID; break;
                case NewsfeedItemTypeEnum.GameHost: comment.GameHostID = objectID; break;
                case NewsfeedItemTypeEnum.NewUser: comment.NewUserID = objectID; break;
                default: throw new NotImplementedException("unknown objectType");
            }
            db.Comments.Add(comment);
            db.SaveChanges();

            if (notifyAll)
            {
                if (objectType == NewsfeedItemTypeEnum.GameHost)
                    await this.notifyAllAttendees(objectID, athlete, text);
                else
                    await this.notifyAllInvolvedAthletes(athleteID, objectType, objectID, comment.CommentID, text);
            }

            return comment.CommentID;
        }

        public List<CommentWebModel> GetComments(NewsfeedItemTypeEnum objectType, int objectID)
        {
            int? postID = null;
            int? resultID = null;
            int? scoreID = null;
            int? gameHostID = null;
            int? newUserID = null;
            switch (objectType)
            {
                case NewsfeedItemTypeEnum.Post: postID = objectID; break;
                case NewsfeedItemTypeEnum.Result: resultID = objectID; break;
                case NewsfeedItemTypeEnum.Score: scoreID = objectID; break;
                case NewsfeedItemTypeEnum.GameHost: gameHostID = objectID; break;
                case NewsfeedItemTypeEnum.NewUser: newUserID = objectID; break;
                default: throw new NotImplementedException("unknown objectType");
            }

            var comments = (from i in db.Comments
                            where i.CommentType == (int)CommentTypeEnum.Comment
                            where i.ObjectType == (int)objectType
                            where i.PostID == postID
                            where i.ResultID == resultID
                            where i.ScoreID == scoreID
                            where i.GameHostID == gameHostID
                            where i.NewUserID == newUserID
                            orderby i.Time descending
                            select new CommentWebModel()
                            {
                                ID = i.CommentID,
                                AthleteID = i.AthleteID,
                                AthleteName = i.Athlete.Name,
                                AthletePicture = i.Athlete.Picture,
                                Text = i.CommentText,
                                Time = i.Time,
                            }).ToList();
            return comments;
        }

        public NewsfeedItemWebModel GetNewsfeedItem(int myAthleteID, NewsfeedItemTypeEnum objectType, int objectID)
        {
            List<NewsfeedItemWebModel> items = null;
            switch (objectType)
            {
                case NewsfeedItemTypeEnum.Post: items = this.getPosts(myAthleteID, db.Posts.Where(i => i.PostID == objectID)).ToList(); break;
                case NewsfeedItemTypeEnum.Result: items = this.getResults(myAthleteID, db.Results.Where(i => i.ResultID == objectID)).ToList(); break;
                case NewsfeedItemTypeEnum.Score: items = this.getScores(myAthleteID, db.Scores.Where(i => i.ScoreID == objectID)).ToList(); break;
                case NewsfeedItemTypeEnum.GameHost: items = this.getGameHosts(myAthleteID, db.GameHosts.Where(i => i.GameHostID == objectID)).ToList(); break;
                case NewsfeedItemTypeEnum.NewUser: items = this.getNewUsers(myAthleteID, db.Athletes.Where(i => i.AthleteID == objectID)).ToList(); break;
            }
            if (items == null || items.Count != 1)
                return null;
            return items[0];
        }

        public NewsfeedWebModel GetGlobalNewsfeed(int myAthleteID)
        {
            NewsfeedWebModel newsfeed = new NewsfeedWebModel();
            newsfeed.Items = new List<NewsfeedItemWebModel>();

            newsfeed.Items.AddRange(this.getPosts(myAthleteID,
                (from i in db.Posts
                 select i)));
            newsfeed.Items.AddRange(this.getResults(myAthleteID,
                (from i in db.Results
                 select i)));
            newsfeed.Items.AddRange(this.getScores(myAthleteID,
                (from i in db.Scores
                 select i)));
            newsfeed.Items.AddRange(this.getGameHosts(myAthleteID,
                (from i in db.GameHosts
                 where i.EventType != (int)EventTypeEnum.Private
                 select i)));
            newsfeed.Items.AddRange(this.getNewUsers(myAthleteID,
                (from i in db.Athletes
                 select i)));

            newsfeed.Items = newsfeed.Items.OrderByDescending(i => i.Time).Take(NewsfeedWebModel.MaxItems).ToList();
            return newsfeed;
        }

        public NewsfeedWebModel GetNewsfeedForCountry(int myAthleteID, Country country)
        {
            NewsfeedWebModel newsfeed = new NewsfeedWebModel();
            newsfeed.Items = new List<NewsfeedItemWebModel>();

            newsfeed.Items.AddRange(this.getPosts(myAthleteID,
                (from i in db.Posts
                 where i.Country == country.ThreeLetterCode
                 select i)));
            newsfeed.Items.AddRange(this.getResults(myAthleteID,
                (from i in db.Results
                 where i.Athlete.Country == country.ThreeLetterCode
                 select i)));
            newsfeed.Items.AddRange(this.getScores(myAthleteID,
                (from i in db.Scores
                 where i.AthleteA.Country == country.ThreeLetterCode
                 select i)));
            newsfeed.Items.AddRange(this.getGameHosts(myAthleteID,
                (from i in db.GameHosts
                 where i.EventType != (int)EventTypeEnum.Private
                 where i.Venue.Country == country.ThreeLetterCode
                 select i)));
            newsfeed.Items.AddRange(this.getNewUsers(myAthleteID,
                (from i in db.Athletes
                 where i.Country == country.ThreeLetterCode
                 select i)));

            newsfeed.Items = newsfeed.Items.OrderByDescending(i => i.Time).Take(NewsfeedWebModel.MaxItems).ToList();
            return newsfeed;
        }

        public NewsfeedWebModel GetNewsfeedForMetro(int myAthleteID, int metroID)
        {
            Metro metro = db.Metros.Where(i => i.MetroID == metroID).Single();
            string country = metro.Country;

            NewsfeedWebModel newsfeed = new NewsfeedWebModel();
            newsfeed.Items = new List<NewsfeedItemWebModel>();

            newsfeed.Items.AddRange(this.getPosts(myAthleteID,
                (from i in db.Posts
                 where i.MetroID == metroID || (i.MetroID == null && i.Country == country)
                 select i)));
            newsfeed.Items.AddRange(this.getResults(myAthleteID,
                (from i in db.Results
                 where i.Athlete.MetroID == metroID
                 select i)));
            newsfeed.Items.AddRange(this.getScores(myAthleteID,
                (from i in db.Scores
                 where i.AthleteA.MetroID == metroID
                 select i)));
            newsfeed.Items.AddRange(this.getGameHosts(myAthleteID,
                (from i in db.GameHosts
                 where i.EventType != (int)EventTypeEnum.Private
                 where i.Venue.MetroID == metroID
                 select i)));
            newsfeed.Items.AddRange(this.getNewUsers(myAthleteID,
                (from i in db.Athletes
                 where i.MetroID == metroID
                 select i)));

            newsfeed.Items = newsfeed.Items.OrderByDescending(i => i.Time).Take(NewsfeedWebModel.MaxItems).ToList();
            return newsfeed;
        }

        List<NewsfeedItemWebModel> getPosts(int myAthleteID, IQueryable<Post> query)
        {
            var list = (from i in query
                        orderby i.Time descending
                        select new NewsfeedItemWebModel()
                        {
                            ItemType = NewsfeedItemTypeEnum.Post,
                            ID = i.PostID,
                            AthleteID = i.AthleteID,
                            AthleteName = i.Athlete.Name,
                            AthletePicture = i.Athlete.Picture,
                            Text = i.PostText,
                            Time = i.Time,
                            CommentsCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Comment).Count(),
                            LikesCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like).Count(),
                            Liked = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like && c.AthleteID == myAthleteID).Count() > 0,
                        }).Take(NewsfeedWebModel.MaxItems).ToList();
            return list;
        }

        List<NewsfeedItemWebModel> getResults(int myAthleteID, IQueryable<Result> query)
        {
            var list = (from i in query
                        where i.Date != null
                        where i.IsDeleted == false
                        where i.IsNotAcceptedByAthleteYet == false
                        where i.OpponentConfirmation != (int)OpponentConfirmationEnum.Declined
                        //where i.OpponentConfirmation == (int)OpponentConfirmationEnum.Confirmed
                        where i.Count != null
                        orderby i.Date descending
                        select new NewsfeedItemWebModel()
                        {
                            ItemType = NewsfeedItemTypeEnum.Result,
                            ID = i.ResultID,
                            AthleteID = i.AthleteID,
                            AthleteName = i.Athlete.Name,
                            AthletePicture = i.Athlete.Picture,
                            Text = i.Count.Value.ToString(),
                            Time = i.Date.Value,
                            VenueID = i.VenueID ?? 0,
                            VenueName = i.VenueID != null ? i.Venue.Name : "",
                            CommentsCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Comment).Count(),
                            LikesCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like).Count(),
                            Liked = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like && c.AthleteID == myAthleteID).Count() > 0,
                        }).Take(NewsfeedWebModel.MaxItems).ToList();
            return list;
        }

        List<NewsfeedItemWebModel> getScores(int myAthleteID, IQueryable<Score> query)
        {
            var list = (from i in query
                        where i.Date != null
                        where i.IsDeleted == false
                        where i.IsUnfinished == false
                        where i.AthleteBConfirmation == (int)OpponentConfirmationEnum.Confirmed
                        orderby i.Date descending
                        select new NewsfeedItemWebModel()
                        {
                            ItemType = NewsfeedItemTypeEnum.Score,
                            ID = i.ScoreID,
                            AthleteID = i.AthleteAID,
                            AthleteName = i.AthleteA.Name,
                            AthletePicture = i.AthleteA.Picture,
                            Athlete2ID = i.AthleteBID,
                            Athlete2Name = i.AthleteB.Name,
                            Athlete2Picture = i.AthleteB.Picture,
                            Text = i.PointsA.ToString() + " : " + i.PointsB.ToString(),
                            Time = i.Date,
                            VenueID = i.VenueID ?? 0,
                            VenueName = i.VenueID != null ? i.Venue.Name : "",
                            CommentsCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Comment).Count(),
                            LikesCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like).Count(),
                            Liked = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like && c.AthleteID == myAthleteID).Count() > 0,
                        }).Take(NewsfeedWebModel.MaxItems).ToList();
            return list;
        }

        List<NewsfeedItemWebModel> getGameHosts(int myAthleteID, IQueryable<GameHost> query)
        {
            var list = (from i in query
                        orderby i.When descending
                        select new NewsfeedItemWebModel()
                        {
                            ItemType = NewsfeedItemTypeEnum.GameHost,
                            ID = i.GameHostID,
                            AthleteID = i.AthleteID,
                            AthleteName = i.Athlete.Name,
                            AthletePicture = i.Athlete.Picture,
                            Text = "An invite",
                            Time = i.TimeCreated,
                            TimeOfEvent = i.When,
                            TimeOfEventLocal = i.When_InLocalTimeZone,
                            VenueID = i.VenueID ?? 0,
                            VenueName = i.VenueID != null ? i.Venue.Name : "",
                            CommentsCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Comment).Count(),
                            LikesCount = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like).Count(),
                            Liked = i.Comments.Where(c => c.CommentType == (int)CommentTypeEnum.Like && c.AthleteID == myAthleteID).Count() > 0,
                        }).Take(NewsfeedWebModel.MaxItems).ToList();
            return list;
        }

        List<NewsfeedItemWebModel> getNewUsers(int myAthleteID, IQueryable<Athlete> query)
        {
            var list = (from i in query
                        orderby i.TimeCreated descending
                        let commentsCount = (from c in db.Comments
                                             where c.CommentType == (int)CommentTypeEnum.Comment
                                             where c.ObjectType == (int)NewsfeedItemTypeEnum.NewUser
                                             where c.NewUserID == i.AthleteID
                                             select c).Count()
                        let likesCount = (from c in db.Comments
                                          where c.CommentType == (int)CommentTypeEnum.Like
                                          where c.ObjectType == (int)NewsfeedItemTypeEnum.NewUser
                                          where c.NewUserID == i.AthleteID
                                          select c).Count()
                        let liked = (from c in db.Comments
                                     where c.CommentType == (int)CommentTypeEnum.Like
                                     where c.ObjectType == (int)NewsfeedItemTypeEnum.NewUser
                                     where c.NewUserID == i.AthleteID
                                     where c.AthleteID == myAthleteID
                                     select c).Count() > 0
                        select new NewsfeedItemWebModel()
                        {
                            ItemType = NewsfeedItemTypeEnum.NewUser,
                            ID = i.AthleteID,
                            AthleteID = i.AthleteID,
                            AthleteName = i.Name,
                            AthletePicture = i.Picture,
                            Text = "New user",
                            Time = i.TimeCreated,
                            CommentsCount = commentsCount,
                            LikesCount = likesCount,
                            Liked = liked,
                        }).Take(NewsfeedWebModel.MaxItems).ToList();
            return list;
        }

        async Task notifyAllInvolvedAthletes(int myAthleteID, NewsfeedItemTypeEnum objectType, int objectID, int commentID, string commentText)
        {
            try
            {
                var myAthlete = db.Athletes.Where(i => i.AthleteID == myAthleteID).Single();

                // list of athletes involved
                var item = this.GetNewsfeedItem(myAthleteID, objectType, objectID);
                if (item == null)
                    return;
                var comments = this.GetComments(objectType, objectID);
                List<int> athleteIDs = new List<int>();
                athleteIDs.Add(item.AthleteID);
                if (athleteIDs.Contains(item.Athlete2ID) == false)
                    athleteIDs.Add(item.Athlete2ID);
                foreach (var comment in comments)
                    if (athleteIDs.Contains(comment.AthleteID) == false)
                        athleteIDs.Add(comment.AthleteID);
                athleteIDs.Remove(0);
                athleteIDs.Remove(myAthleteID);

                foreach (int athleteID in athleteIDs)
                {
                    try
                    {
                        var athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();

                        // send push notification
                        new PushNotificationsLogic(db).SendNotification(athleteID, PushNotificationMessage.BuildComment(myAthlete, commentText, commentID));

                        // email
                        string subject = "'Byb' - " + athlete.Name + " commented on a tracked conversation";
                        string link1 = new DeepLinkHelper().BuildOpenBybLink_Comment(commentID);
                        string link2 = new DeepLinkHelper().BuildLinkToComment(commentID);
                        string html = string.Format(htmlMessage_Post, myAthlete.NameOrUserName, commentText, link1, link2);
                        await new EmailService().SendEmailToAthlete(athlete, subject, html);
                    }
                    catch (Exception)
                    {
                    }
                }
                PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
            }
            catch (Exception)
            {

            }
        }

        async Task notifyAllAttendees(int gameHostID, Athlete myAthlete, string commentText)
        {
            try
            {
                var gameHost = db.GameHosts.Where(i => i.GameHostID == gameHostID).Single();
                var venue = db.Venues.Where(i => i.VenueID == gameHost.VenueID).Single();

                // list of attendees and people that commented
                var athleteIDs = gameHost.GameHostInvites.Where(i => i.IsDeniedByInvitee == false).Select(i => i.AthleteID).ToList();
                if (athleteIDs.Contains(gameHost.AthleteID) == false)
                    athleteIDs.Add(gameHost.AthleteID);
                var commentAthleteIDs = gameHost.Comments.Select(i => i.AthleteID).ToList();
                foreach (int id in commentAthleteIDs)
                    if (athleteIDs.Contains(id) == false)
                        athleteIDs.Add(id);
                athleteIDs.Remove(0);
                athleteIDs.Remove(myAthlete.AthleteID);

                foreach (int athleteID in athleteIDs)
                {
                    try
                    {
                        var athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();

                        // send push notification
                        new PushNotificationsLogic(db).SendNotification(athleteID, PushNotificationMessage.BuildGameCommentMessage(myAthlete, commentText, gameHostID));

                        // send email
                        string when = gameHost.When_InLocalTimeZone.ToLongDateString() + " - " + gameHost.When_InLocalTimeZone.ToShortTimeString();
                        string subject = "'Byb' - " + myAthlete.Name + " commented on an invite";
                        string link1 = new DeepLinkHelper().BuildOpenBybLink_GameHost(gameHost.GameHostID);
                        string link2 = new DeepLinkHelper().BuildLinkToGameHost(gameHost.GameHostID);
                        string html = string.Format(htmlComment_GameHost, myAthlete.Name, gameHost.When_InLocalTimeZone, venue.Name, commentText, link1, link2);
                        await new EmailService().SendEmailToAthlete(athlete, subject, html);
                    }
                    catch (Exception)
                    {
                    }
                }

                PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
            }
            catch (Exception)
            {
            }
        }

        string htmlMessage_Post =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p>A player commented on a tracked conversation:</p>
    <p><span style=""color:gray;"">Player: </span><span style=""font-weight:bold"">{0}</span></p>
    <p><span style=""color:gray;"">Comment: </span><span style = ""font-weight:bold;text-color:black"">{1}</span></p>

    <br>

    <p>Open conversation in Snooker Byb:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{2}"">Open in Byb</a>
    </p>

    <p>If doesn't work, try this:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{3}"">Open in Byb</a>
    </p>
    <p>
        <span style=""color:gray;"">Snooker Byb Team</span>: This function is in early beta. Please excuse our bugs. Feel free to shoot us a message with your suggestions and ideas: <a>team@snookerbyb.com</a>
    </p>
    
</body>
</html>";

        string htmlComment_GameHost =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p><span style=""color:gray;"">Player: </span><span style=""font-weight:bold"">{0}</span></p>
    <p><span style=""color:gray;"">At: </span><span style = ""font-weight:bold"">{2}</span></p>
    <p><span style=""color:gray;"">When: </span><span style = ""font-weight:bold;text-color:black"">{1}</span></p>
    <p><span style=""color:gray;"">Comment: </span><span style = ""font-weight:bold;text-color:black"">{3}</span></p>

    <br>

    <p>Open in Snooker Byb:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{4}"">Open the Invite</a>
    </p>

    <p>If doesn't work, try this:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{5}"">Open the Invite</a>
    </p>
    <p>
        <span style=""color:gray;"">Snooker Byb Team</span>: This function is in early beta. Please excuse our bugs. Feel free to shoot us a message with your suggestions and ideas: <a>team@snookerbyb.com</a>
    </p>
</body>
</html>";

    }
}
