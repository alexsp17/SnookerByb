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
    [RoutePrefix("api/Messages")]
    public class MessagesController : ApiController
    {
        private ApplicationDbContext db;

        public MessagesController()
        {
            this.db = new ApplicationDbContext();
        }

        string htmlMessage =
@"<html>
<head>
</head>
<body>
    <p style=""color:gray;""><i>Do not reply directly to this email, instead please follow the links below.</i></p>

    <p><span style=""color:gray;"">From: </span><a href=""{3}"">{0}</a></p>
    <p><span style=""color:gray;"">Message: </span><span style = ""font-weight:bold"">{1}</span></p>
    <p><span style=""color:gray;"">{0}'s email: </span>{2}</p>

    <br>

    <p style=""color:gray;"">Tap on a device with Snooker Byb:</p>
    <p style = ""margin-top:30px;margin-bottom:30px;"" >
        <a style=""font-weight:bold; background:#DB3638;color:white;padding: 15px 40px 15px 40px"" href=""{4}"">Open in Byb</a>
    </p>
        
    <p>
        <span style=""color:gray;"">Snooker Byb Team:</span> This function is in beta. Please excuse our bugs. Feel free to shoot us a message with your suggestions and ideas: <a>team@snookerbyb.com</a>
    </p>
    
</body>
</html>";

        [Route("Send")]
        [HttpPost]
        public async Task<bool> Send(int athleteID, string messageText, bool shareMyEmail)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);
            var athlete = db.Athletes.Single(i => i.AthleteID == athleteID);
            if (messageText == null)
                messageText = "";

            string linkToAthlete = new DeepLinkHelper().BuildLinkToAthlete(me.AthleteID);
            string linkToOpenByb = new DeepLinkHelper().BuildOpenBybLink_Athlete(me.AthleteID);

            // send an email
            string myName = me.NameOrUserName;
            string myEmail = me.UserName;
            if (string.IsNullOrEmpty(me.RealEmail) == false)
                myEmail = me.RealEmail;
            string html = string.Format(htmlMessage, myName, messageText, shareMyEmail ? myEmail : "notshared", linkToAthlete, linkToOpenByb);
            await new EmailService().SendEmailToAthlete(athlete, "Snooker Byb Message", html);

            // send a push notification
            new PushNotificationsLogic(db).SendNotification(athleteID, PushNotificationMessage.BuildPrivateMessage(me, messageText));
            PushNotificationProcessor.TheProcessor.PushAllPendingNotifications();

            return true;
        }
    }
}