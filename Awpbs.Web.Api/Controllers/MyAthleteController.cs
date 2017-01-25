using CloudinaryDotNet;
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
    [RoutePrefix("api/MyAthlete")]
    public class MyAthleteController : ApiController
    {
        private ApplicationDbContext db;

        public MyAthleteController()
        {
            this.db = new ApplicationDbContext();
        }

        public Athlete Get()
        {
            var athlete = new UserProfileLogic(db).GetAthleteForUserName(User.Identity.Name);

            Athlete sanitized = new Athlete();
            athlete.CopyTo(sanitized, true);
            return sanitized;
        }

        [HttpPost]
        [Route("UseFacebookPicture")]
        public string UseFacebookPicture(bool use)
        {
            var logic = new UserProfileLogic(db);
            var athlete = logic.GetAthleteForUserName(User.Identity.Name);

            string pictureUrl = "";
            if (use == true && athlete.HasFacebookId)
                pictureUrl = "http://res.cloudinary.com/bestmybest/image/facebook/" + System.Web.HttpUtility.UrlEncode(athlete.FacebookId) + ".jpg";
            
            logic.UpdateAthletePicture(athlete.AthleteID, pictureUrl);
            return pictureUrl;
        }

        [HttpPost]
        [Route("UploadPicture")]
        public async Task<string> UploadPicture()
        {
            var logic = new UserProfileLogic(db);
            var athlete = logic.GetAthleteForUserName(User.Identity.Name);

            var file = await Request.Content.ReadAsStreamAsync();
            if (file.Length < 100)
                throw new Exception("file.Length=" + file.Length.ToString());
            if (file.Length > 1000000 * 100)
                throw new Exception("file.Length=" + file.Length.ToString());

            string cloudinaryAccount = System.Web.Configuration.WebConfigurationManager.AppSettings["CloudinaryAccount"];
            string cloudinaryKey = System.Web.Configuration.WebConfigurationManager.AppSettings["CloudinaryKey"];
            string cloudinarySecret = System.Web.Configuration.WebConfigurationManager.AppSettings["CloudinarySecret"];
            var cloudinary = new CloudinaryDotNet.Cloudinary(new CloudinaryDotNet.Account(cloudinaryAccount, cloudinaryKey, cloudinarySecret));

            var uploadResult = await cloudinary.UploadAsync(new CloudinaryDotNet.Actions.ImageUploadParams()
            {
                File = new CloudinaryDotNet.Actions.FileDescription("athlete" + athlete.AthleteID, file),
                Transformation = new Transformation().Width(500).Height(500).Crop("limit") // limit image size to 500x500 max
            });

            //string pictureUrl = uploadResult.Uri.ToString();
            string pictureUrl = "http://res.cloudinary.com/bestmybest/image/upload/" + uploadResult.PublicId + "." + uploadResult.Format;

            new UserProfileLogic(db).UpdateAthletePicture(athlete.AthleteID, pictureUrl);

            return pictureUrl;
        }
    }
}
