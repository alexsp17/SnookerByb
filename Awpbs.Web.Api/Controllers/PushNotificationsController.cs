using PushSharp;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/PushNotifications")]
    public class PushNotificationsController : ApiController
    {
        private ApplicationDbContext db;

        public PushNotificationsController()
        {
            this.db = new ApplicationDbContext();
        }

        [Authorize]
        [HttpPost]
        [Route("RegisterDeviceToken")]
        public bool RegisterDeviceToken(RegisterDeviceTokenWebModel model)
        {
            int myAthleteID = new UserProfileLogic(db).GetAthleteIDForUserName(User.Identity.Name);
            new PushNotificationsLogic(db).RegisterDeviceToken(myAthleteID, model.Token, model.IsApple, model.IsAndroid, !model.IsNotProduction);
            return true;
        }

        /// <summary>
        /// just for testing - making sure that Apple's push notification certificate is installed
        /// </summary>
        [HttpGet]
        [Route("AppleCertificateName")]
        public string AppleCertificateName(bool? isProduction = null)
        {
            if (isProduction == null)
                isProduction = Config.IsProduction;

            //string certificateName = PushNotificationProcessor.TheProcessor.AppleCertficateName;
            var cert = PushNotificationProcessor.TheProcessor.LoadAppleCertificate(isProduction.Value);

            string text = "isProduction=" + isProduction + "  ";
            if (cert != null)
                text += "loaded: " + cert.FriendlyName + " ||||| Subject:" + cert.Subject;
            else
                text += "could not load : ?";

            return text;
        }

        [HttpGet]
        [Route("IsProduction")]
        public string IsProduction()
        {
            string text = "IsProduction = " + Config.IsProduction.ToString();
            return text;
        }
   }
}