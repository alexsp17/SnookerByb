using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Threading.Tasks;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/About")]
    public class AboutController : ApiController
    {
        private ApplicationDbContext db;

        public AboutController()
        {
            this.db = new ApplicationDbContext();
        }

        public BybApiAboutWebModel Get()
        {
            var model = new BybApiAboutWebModel();

            model.ThisApiVersionNumber = BybApiVersions.Current.Number;
            model.ThisApiVersionName = BybApiVersions.Current.VersionName;
            model.AppVersionLatestName = SnookerBybMobileVersions.Current.VersionName;
            model.AppVersionLatestNumber = SnookerBybMobileVersions.Current.Number;

            int versionUpgradeRecommended = 23;
            string strVersion = Environment.GetEnvironmentVariable("SNOOKER_BYB_VERSION_UPGRADE_RECOMMENDED");
            if (strVersion != null)
                int.TryParse(strVersion, out versionUpgradeRecommended);
            model.AppVersionUpgradeRecommended = versionUpgradeRecommended;

            int versionUpgradeRequired = 23;
            strVersion = Environment.GetEnvironmentVariable("SNOOKER_BYB_VERSION_UPGRADE_REQUIRED");
            if (strVersion != null)
                int.TryParse(strVersion, out versionUpgradeRequired);
            model.AppVersionUpgradeRequired = versionUpgradeRequired;

            return model;
        }

        [HttpPost]
        [Route("DeviceInfo")]
        public bool DeviceInfo(DeviceInfoWebModel info)
        {
            var me = new UserProfileLogic(db).GetAthleteForUserName(this.User.Identity.Name);

            db.DeviceInfos.Add(new Awpbs.DeviceInfo()
            {
                AthleteID = me.AthleteID,
                TimeCreated = DateTime.UtcNow,
                Platform = info.Platform,
                OSVersion = info.OSVersion,
            });
            db.SaveChanges();

            return true;
        }

        [HttpGet]
        [Route("Test1")]
        public async Task<string> Test1()
        {
            //string str = await new DeepLinkHelper().BuildLink_Athlete(12);
            //if (string.IsNullOrEmpty(str))
            //    return "Failed to build a link.";
            //return str;
            return "Test1---";
        }
    }
}