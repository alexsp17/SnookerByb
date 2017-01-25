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
    [RoutePrefix("api/GameHosts")]
    public class GameHostsController : ApiController
    {
        private ApplicationDbContext db;

        public GameHostsController()
        {
            this.db = new ApplicationDbContext();
        }

        [Authorize]
        [HttpGet]
        [Route("MyGameHosts")]
        public List<GameHostWebModel> GetMyGameHosts(bool includePast)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            return new GameHostsLogic(db).GetMyGameHosts(me.AthleteID, includePast);
        }

        [Authorize]
        [HttpGet]
        [Route("GameHostsAtVenue")]
        public List<GameHostWebModel> GameHostsAtVenue(int venueID, bool includePast)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            return new GameHostsLogic(db).GetGameHostsAtVenue(venueID, includePast, me.AthleteID);
        }

        [Authorize]
        [HttpPost]
        [Route("NewGameHost")]
        public int NewGameHost(NewGameHostWebModel model)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);

            if ((model.When - DateTime.UtcNow).TotalHours < -1)
                throw new Exception("when is in the past");

            GameHost gameHost = new GameHost()
            {
                AthleteID = me.AthleteID,
                TimeCreated = DateTime.UtcNow,
                VenueID = model.VenueID,
                Visibility = model.Visibility,
                When = model.When,
                When_InLocalTimeZone = model.When_InLocalTimeZone
            };
            db.GameHosts.Add(gameHost);
            db.SaveChanges();

            return gameHost.GameHostID;
        }

        [Authorize]
        [HttpPost]
        [Route("ChangeLimitOnNumberOfPlayers")]
        public bool ChangeLimitOnNumberOfPlayers(int gameHostID, int limit)
        {
            if (limit < 0)
                throw new Exception("limit=" + limit);

            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            var gameHost = db.GameHosts.Where(i => i.GameHostID == gameHostID).Single();
            if (gameHost.AthleteID != me.AthleteID)
                throw new Exception("cannot edit someone else's event");

            gameHost.LimitOnNumberOfPlayers = limit;
            db.SaveChanges();

            return true;
        }

        [Authorize]
        [HttpPost]
        [Route("NewGameHost2")]
        public async Task<int> NewGameHost2(NewGameHostWebModel2 model)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            var venue = db.Venues.SingleOrDefault(i => i.VenueID == model.VenueID);
            List<Athlete> playersToInvite = new List<Athlete>();
            if (model.Invitees != null)
            {
                var ids = model.Invitees.Distinct().ToList();
                if (ids.Count > 100)
                    throw new ApplicationException("Too many athletes are being invited.");
                foreach (var id in ids)
                    playersToInvite.Add(db.Athletes.Single(i => i.AthleteID == id));
            }

            if ((model.When - DateTime.UtcNow).TotalHours < -1)
                throw new Exception("when is in the past");

            // create the gamehost
            GameHost gameHost = new GameHost()
            {
                AthleteID = me.AthleteID,
                TimeCreated = DateTime.UtcNow,
                VenueID = model.VenueID,
                Visibility = 0,//model.Visibility,
                When = model.When,
                When_InLocalTimeZone = model.When_InLocalTimeZone,
                LimitOnNumberOfPlayers = model.LimitOnNumberOfPlayers,
                EventType = (int)model.EventType,
            };
            db.GameHosts.Add(gameHost);

            // create the invites
            foreach (var athlete in playersToInvite)
            {
                GameHostInvite invite = new GameHostInvite()
                {
                    AthleteID = athlete.AthleteID,
                    GameHost = gameHost,
                    IsApprovedByHost = true,
                    TimeCreated = DateTime.UtcNow
                };
                db.GameHostInvites.Add(invite);
            }

            db.SaveChanges();

            // add the comment
            if (string.IsNullOrEmpty(model.Comments) == false)
            {
                await new FeedLogic(db).MakeComment(me.AthleteID, NewsfeedItemTypeEnum.GameHost, gameHost.GameHostID, model.Comments, false);
            }

            // send the invites
            foreach (var athlete in playersToInvite)
            {
                await sendAnInvite(me, athlete, gameHost, venue, model.Comments);
            }

            return gameHost.GameHostID;
        }

        [Authorize]
        [HttpPost]
        [Route("DeleteGameHost")]
        public bool DeleteGameHost(int gameHostID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            var gameHost = db.GameHosts.Single(i => i.GameHostID == gameHostID);

            // can delete?
            bool canDelete = me.IsAdmin;
            if (gameHost.AthleteID == me.AthleteID)
                canDelete = true;
            if (canDelete == false)
                throw new Exception("Cannot delete. No rights.");

            // delete
            var comments = db.Comments.Where(i => i.GameHostID == gameHostID).ToList();
            foreach (var comment in comments)
                db.Comments.Remove(comment);
            db.GameHosts.Remove(gameHost);
            db.SaveChanges();

            return true;
        }

        [Authorize]
        [HttpPost]
        [Route("NewGameHostInvite")]
        public async Task<int> NewGameHostInvite(NewGameHostInviteWebModel model)
        {
            int gameHostID = model.GameHostID;
            int athleteID = model.AthleteID;

            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            var athlete = db.Athletes.Single(i => i.AthleteID == athleteID);
            if (athlete == null || me.AthleteID == athleteID)
                throw new Exception("athleteID is invalid");

            GameHost gameHost = db.GameHosts.Include("Venue").Single(i => i.GameHostID == gameHostID);
            if (gameHost.GameHostInvites.Where(i => i.AthleteID == athleteID).Count() > 0)
                throw new Exception("an invite for this athlete already exists");

            GameHostInvite invite = new GameHostInvite()
            {
                AthleteID = athleteID,
                GameHost = gameHost,
                IsApprovedByHost = true,
                TimeCreated = DateTime.UtcNow
            };
            db.GameHostInvites.Add(invite);
            db.SaveChanges();

            await sendAnInvite(me, athlete, gameHost, gameHost.Venue, "");

            return invite.GameHostInviteID;
        }

        [Authorize]
        [HttpPost]
        [Route("DeclineInvitation")]
        public async Task<bool> DeclineInvitation(int gameHostID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);

            GameHost gameHost = db.GameHosts.Include("Athlete").Include("Venue").Where(i => i.GameHostID == gameHostID).Single();
            GameHostInvite invite = gameHost.GameHostInvites.Where(i => i.AthleteID == me.AthleteID).First();

            invite.IsApprovedByInvitee = false;
            invite.IsDeniedByInvitee = true;
            db.SaveChanges();

            await this.sendInviteDeclined(me, gameHost.Athlete, gameHost, gameHost.Venue);

            return true;
        }

        [Authorize]
        [HttpPost]
        [Route("AskToJoinGameHost")]
        public async Task<int> AskToJoinGameHost(int gameHostID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            GameHost gameHost = db.GameHosts.Single(i => i.GameHostID == gameHostID);
            if (gameHost.AthleteID == me.AthleteID)
                throw new Exception("Cannot ask yourself");

            GameHostInvite invite = gameHost.GameHostInvites.Where(i => i.AthleteID == me.AthleteID).FirstOrDefault();
            if (invite != null && invite.IsApprovedByHost == true)
            {
                // already invited by the host - mark as approved by invitee
                invite.IsApprovedByInvitee = true;
                invite.IsDeniedByInvitee = false;
                db.SaveChanges();

                await this.sendInviteAccepted(me, gameHost.Athlete, gameHost, gameHost.Venue);

                return invite.GameHostInviteID;
            }
            else if (invite != null && invite.IsApprovedByHost == false)
            {
                // the invitee already invited himself, the host hasn't accepted yet
                return invite.GameHostInviteID;
            }
            else
            {
                // create an invitation object
                invite = new GameHostInvite()
                {
                    AthleteID = me.AthleteID,
                    GameHost = gameHost,
                    IsApprovedByHost = false,
                    IsApprovedByInvitee = true,
                    TimeCreated = DateTime.UtcNow
                };
                db.GameHostInvites.Add(invite);
                db.SaveChanges();

                // email
                await this.emailWantsToPlay(me, gameHost.Athlete, gameHost, gameHost.Venue);

                return invite.GameHostInviteID;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ApproveInvitee")]
        public async Task<bool> ApproveInvitee(int gameHostID, int inviteeID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            GameHost gameHost = db.GameHosts.Include("Athlete").Include("Venue").Single(i => i.GameHostID == gameHostID);
            if (gameHost.AthleteID != me.AthleteID)
                throw new Exception("Not your game host!");

            GameHostInvite invite = gameHost.GameHostInvites.Where(i => i.AthleteID == inviteeID).FirstOrDefault();
            if (invite == null)
                throw new Exception("Not found");
            if (invite.IsApprovedByInvitee == false || invite.IsDeniedByInvitee == true)
                throw new Exception("IsApprovedByInvitee == false");
            Athlete athleteInvitee = db.Athletes.Where(i => i.AthleteID == invite.AthleteID).Single();

            invite.IsApprovedByHost = true;
            db.SaveChanges();

            await this.sendHostApproves(me, athleteInvitee, gameHost, gameHost.Venue);

            return true;
        }

        [HttpGet]
        public GameHostWebModel Get(int id)
        {
            Athlete me = null;
            if (this.User.Identity.IsAuthenticated)
                me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            return new GameHostsLogic(db).Get(id, me != null ? me.AthleteID : 0);
        }

        [HttpGet]
        [Route("Find")]
        [Authorize]
        public List<GameHostWebModel> Find(bool friendsOnly, double? latitude = null, double? longitude = null)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);

            Location location = null;
            if (latitude != null && longitude != null)
                location = new Location(latitude.Value, longitude.Value);
            Distance distance = Distance.FromMiles(30);

            return new GameHostsLogic(db).FindGameHosts(me.AthleteID, location, distance, friendsOnly, DateTime.UtcNow, DateTime.UtcNow.AddDays(1000));
        }

        string htmlMessage_Invitation =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p><span style=""color:gray;"">From: </span><span style=""font-weight:bold"">{0}</span></p>
    <p><span style=""color:gray;"" >At: </span><span style = ""font-weight:bold"">{2}</span></p>
    <p><span style=""color:gray;"">When: </span><span style = ""font-weight:bold;text-color:black"">{1}</span></p>
    <p><span style=""color:gray;"">Type: </span><span style = ""font-weight:bold;text-color:black"">{6}</span></p>
    <p><span style=""color:gray;"">Comments: </span><span style = ""font-weight:bold;text-color:black"">{7}</span></p>

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

        string htmlMessage_InviteAccepted =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p><span style=""color:gray;"">Accepted by: </span><span style=""font-weight:bold"">{0}</span></p>
    <p><span style=""color:gray;"" >At: </span><span style = ""font-weight:bold"">{2}</span></p>
    <p><span style=""color:gray;"">When: </span><span style = ""font-weight:bold;text-color:black"">{1}</span></p>

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

        string htmlMessage_WantsToPlay =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p><span style=""color:gray;"">Player: </span><span style=""font-weight:bold"">{0}</span></p>
    <p><span style=""color:gray;"" >At: </span><span style = ""font-weight:bold"">{2}</span></p>
    <p><span style=""color:gray;"">When: </span><span style = ""font-weight:bold;text-color:black"">{1}</span></p>

    <p>To accept or decline - open in Snooker Byb:</p>
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

        string htmlMessage_InviteDeclined =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p><span style=""color:gray;"">Declined by: </span><span style=""font-weight:bold"">{0}</span></p>
    <p><span style=""color:gray;"" >At: </span><span style = ""font-weight:bold"">{2}</span></p>
    <p><span style=""color:gray;"">When: </span><span style = ""font-weight:bold;text-color:black"">{1}</span></p>

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

        string htmlMessage_HostApproves =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p><span style=""color:gray;"">From: </span><span style=""font-weight:bold"">{0}</span></p>
    <p><span style=""color:gray;"" >At: </span><span style = ""font-weight:bold"">{2}</span></p>
    <p><span style=""color:gray;"">When: </span><span style = ""font-weight:bold;text-color:black"">{1}</span></p>

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

        async Task sendAnInvite(Athlete me, Athlete athlete, GameHost gameHost, Venue venue, string comments)
        {
            // send an email
            string myName = me.NameOrUnknown;
            string myEmail = me.UserName;
            if (string.IsNullOrEmpty(me.RealEmail) == false)
                myEmail = me.RealEmail;
            string when = gameHost.When_InLocalTimeZone.ToLongDateString() + " - " + gameHost.When_InLocalTimeZone.ToShortTimeString();
            string subject = "'Byb Invite' For a Game of Snooker";
            string type = (gameHost.EventType == (int)EventTypeEnum.Private ? "Private event" : "Public event") + (gameHost.LimitOnNumberOfPlayers > 0 ? (", max. " + gameHost.LimitOnNumberOfPlayers.ToString() + " can join") : "");
            string link1 = new DeepLinkHelper().BuildOpenBybLink_GameHost(gameHost.GameHostID);
            string link2 = new DeepLinkHelper().BuildLinkToGameHost(gameHost.GameHostID);
            string html = string.Format(htmlMessage_Invitation, myName, when, venue.Name, gameHost.GameHostID.ToString(), link1, link2, type, comments ?? "");
            await new EmailService().SendEmailToAthlete(athlete, subject, html);

            // send a push notification
            new PushNotificationsLogic(db).SendNotification(athlete.AthleteID, 
                PushNotificationMessage.BuildGameMessage(PushNotificationMessageTypeEnum.GameInvite, me, gameHost.GameHostID));
            PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
        }

        async Task sendHostApproves(Athlete me, Athlete inviteeAthlete, GameHost gameHost, Venue venue)
        {
            // send an email
            string myName = me.NameOrUnknown;
            string myEmail = me.UserName;
            if (string.IsNullOrEmpty(me.RealEmail) == false)
                myEmail = me.RealEmail;
            string when = gameHost.When_InLocalTimeZone.ToLongDateString() + " - " + gameHost.When_InLocalTimeZone.ToShortTimeString();
            string subject = "'Byb' - Approved";
            string link1 = new DeepLinkHelper().BuildOpenBybLink_GameHost(gameHost.GameHostID);
            string link2 = new DeepLinkHelper().BuildLinkToGameHost(gameHost.GameHostID);
            string html = string.Format(htmlMessage_HostApproves, myName, when, venue.Name, gameHost.GameHostID.ToString(), myEmail, link1, link2);
            await new EmailService().SendEmailToAthlete(inviteeAthlete, subject, html);

            // send a push notification
            new PushNotificationsLogic(db).SendNotification(inviteeAthlete.AthleteID,
                PushNotificationMessage.BuildGameMessage(PushNotificationMessageTypeEnum.GameApprovedByHost, me, gameHost.GameHostID));
            PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
        }

        async Task sendInviteAccepted(Athlete inviteeAthlete, Athlete hostAthlete, GameHost gameHost, Venue venue)
        {
            // send an email
            string inviteeName = inviteeAthlete.NameOrUnknown;
            string inviteeEmail = inviteeAthlete.UserName;
            if (string.IsNullOrEmpty(inviteeAthlete.RealEmail) == false)
                inviteeEmail = inviteeAthlete.RealEmail;
            string when = gameHost.When_InLocalTimeZone.ToLongDateString() + " - " + gameHost.When_InLocalTimeZone.ToShortTimeString();
            string subject = "'Byb Invite' Accepted by " + inviteeName;
            string link1 = new DeepLinkHelper().BuildOpenBybLink_GameHost(gameHost.GameHostID);
            string link2 = new DeepLinkHelper().BuildLinkToGameHost(gameHost.GameHostID);
            string html = string.Format(htmlMessage_InviteAccepted, inviteeName, when, venue.Name, gameHost.GameHostID.ToString(), link1, link2);
            await new EmailService().SendEmailToAthlete(hostAthlete, subject, html);

            // send a push notification
            new PushNotificationsLogic(db).SendNotification(hostAthlete.AthleteID,
                PushNotificationMessage.BuildGameMessage(PushNotificationMessageTypeEnum.GameInviteAccepted, inviteeAthlete, gameHost.GameHostID));
            PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
        }

        async Task sendInviteDeclined(Athlete inviteeAthlete, Athlete hostAthlete, GameHost gameHost, Venue venue)
        {
            // send an email
            string inviteeName = inviteeAthlete.NameOrUnknown;
            string inviteeEmail = inviteeAthlete.UserName;
            if (string.IsNullOrEmpty(inviteeAthlete.RealEmail) == false)
                inviteeEmail = inviteeAthlete.RealEmail;
            string when = gameHost.When_InLocalTimeZone.ToLongDateString() + " - " + gameHost.When_InLocalTimeZone.ToShortTimeString();
            string subject = "'Byb Invite' Declined by " + inviteeName;
            string link1 = new DeepLinkHelper().BuildOpenBybLink_GameHost(gameHost.GameHostID);
            string link2 = new DeepLinkHelper().BuildLinkToGameHost(gameHost.GameHostID);
            string html = string.Format(htmlMessage_InviteDeclined, inviteeName, when, venue.Name, gameHost.GameHostID.ToString(), link1, link2);
            await new EmailService().SendEmailToAthlete(hostAthlete, subject, html);

            // send a push notification
            new PushNotificationsLogic(db).SendNotification(hostAthlete.AthleteID,
                PushNotificationMessage.BuildGameMessage(PushNotificationMessageTypeEnum.GameInviteDeclined, inviteeAthlete, gameHost.GameHostID));
            PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
        }

        async Task emailWantsToPlay(Athlete inviteeAthlete, Athlete hostAthlete, GameHost gameHost, Venue venue)
        {
            // send an email
            string inviteeName = inviteeAthlete.NameOrUnknown;
            string inviteeEmail = inviteeAthlete.UserName;
            if (string.IsNullOrEmpty(inviteeAthlete.RealEmail) == false)
                inviteeEmail = inviteeAthlete.RealEmail;
            string when = gameHost.When_InLocalTimeZone.ToLongDateString() + " - " + gameHost.When_InLocalTimeZone.ToShortTimeString();
            string subject = "'Byb' - " + inviteeName + " wants to join your game";
            string link1 = new DeepLinkHelper().BuildOpenBybLink_GameHost(gameHost.GameHostID);
            string link2 = new DeepLinkHelper().BuildLinkToGameHost(gameHost.GameHostID);
            string html = string.Format(htmlMessage_WantsToPlay, inviteeName, when, venue.Name, gameHost.GameHostID.ToString(), link1, link2);
            await new EmailService().SendEmailToAthlete(hostAthlete, subject, html);

            // send a push notification
            new PushNotificationsLogic(db).SendNotification(hostAthlete.AthleteID,
                PushNotificationMessage.BuildGameMessage(PushNotificationMessageTypeEnum.GameWantsToBeInvited, inviteeAthlete, gameHost.GameHostID));
            PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();
        }
    }
}