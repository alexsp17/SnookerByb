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
    [RoutePrefix("api/Friends")]
    public class FriendsController : ApiController
    {
        private ApplicationDbContext db;

        public FriendsController()
        {
            this.db = new ApplicationDbContext();
        }

        public List<PersonBasicWebModel> Get()
        {
            var myAthlete = new UserProfileLogic(db).GetAthleteForUserName(User.Identity.Name);
            return new PeopleLogic(db).GetFriends(myAthlete.AthleteID, myAthlete.AthleteID);
        }

        [Route("GetFriendRequestsToMe")]
        [HttpGet]
        public List<FriendRequestWebModel> GetFriendRequestsToMe()
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);

            var list = (from f in db.Friendships
                        where f.FriendshipStatus == (int)FriendshipStatusEnum.Initiated
                        where f.Athlete2ID == me.AthleteID
                        orderby f.Athlete1.Name
                        select new FriendRequestWebModel() { FriendshipID = f.FriendshipID, PersonName = f.Athlete1.Name, AthleteID = f.Athlete1ID }).ToList();

            foreach (var item in list)
                if (string.IsNullOrEmpty(item.PersonName))
                    item.PersonName = "athlete#" + item.AthleteID;

            return list;
        }

        [Route("GetFriendRequestsByMe")]
        [HttpGet]
        public List<FriendRequestWebModel> GetFriendRequestsByMe()
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);

            var list = (from f in db.Friendships
                        where f.FriendshipStatus == (int)FriendshipStatusEnum.Initiated
                        where f.Athlete1ID == me.AthleteID
                        orderby f.Athlete2.Name
                        select new FriendRequestWebModel() { FriendshipID = f.FriendshipID, PersonName = f.Athlete2.Name, AthleteID = f.Athlete2ID }).ToList();

            foreach (var item in list)
                if (string.IsNullOrEmpty(item.PersonName))
                    item.PersonName = "athlete#" + item.AthleteID;

            return list;
        }

        [Route("RequestFriend")]
        [HttpPost]
        public async Task<bool> RequestFriend(int athleteID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            var athlete = db.Athletes.Single(i => i.AthleteID == athleteID);
            new FriendshipLogic(db).AddFriendship(me.AthleteID, athleteID);

            // send a push notification
            new PushNotificationsLogic(db).SendNotification(athleteID, PushNotificationMessage.BuildFriendRequest(me));
            PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();

            // send an email
            string linkToAthlete = new DeepLinkHelper().BuildLinkToAthlete(me.AthleteID);
            string linkToOpenByb = new DeepLinkHelper().BuildOpenBybLink_Athlete(me.AthleteID);
            string myName = me.NameOrUserName;
            //string myEmail = me.UserName;
            //if (string.IsNullOrEmpty(me.RealEmail) == false)
            //    myEmail = me.RealEmail;
            string html = string.Format(htmlMessage, myName, linkToAthlete, linkToOpenByb);
            await new EmailService().SendEmailToAthlete(athlete, "Friend Request", html);

            return true;
        }

        [Route("Unfriend")]
        [HttpPost]
        public bool Unfriend(int athleteID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            new FriendshipLogic(db).Unfriend(me.AthleteID, athleteID);

            return true;
        }

        [Route("AcceptFriendRequest")]
        [HttpPost]
        public bool AcceptFriendRequest(int friendshipID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            new FriendshipLogic(db).AcceptFriendRequest(friendshipID, me.AthleteID);

            return true;
        }

        [Route("AcceptFriendRequest2")]
        [HttpPost]
        public bool AcceptFriendRequest2(int athleteID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            new FriendshipLogic(db).AcceptFriendRequest2(athleteID, me.AthleteID);

            return true;
        }

        [Route("DeclineFriendRequest")]
        [HttpPost]
        public bool DeclineFriendRequest(int friendshipID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            new FriendshipLogic(db).DeclineFriendRequest(friendshipID, me.AthleteID);

            return true;
        }

        [Route("WithdrawFriendRequest")]
        [HttpPost]
        public bool WithdrawFriendRequest(int friendshipID)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            new FriendshipLogic(db).WithdrawFriendRequest(friendshipID, me.AthleteID);

            return true;
        }

        string htmlMessage =
@"<html>
<head>
</head>
<body>
    <p><span style=""color:gray;"">Friend request from: </span><a href=""{1}"">{0}</a></p>

    <p style=""color:gray;"">Tap on a device with Snooker Byb:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{2}"">Open in Byb</a>
    </p>
        
    <p>
        <span style=""color:gray;"">Snooker Byb Team:</span> This function is in beta. Please excuse our bugs. Feel free to shoot us a message with your suggestions and ideas: <a>team@snookerbyb.com</a>
    </p>
    
</body>
</html>";
    }
}