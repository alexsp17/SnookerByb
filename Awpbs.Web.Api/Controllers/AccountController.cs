using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Awpbs.Web.Api.Models;
using Awpbs.Web.Api.Providers;
using Awpbs.Web.Api.Results;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Awpbs.Web.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Route("ChangePin")]
        public async Task<ChangePinResultWebModel> ChangePin(ChangePinWebModel model)
        {
            string newPin = model.Pin;
            if (new AccessPinHelper().Validate(newPin) == false)
                throw new Exception("Bad pin");

            var db = new ApplicationDbContext();
            var logic = new UserProfileLogic(db);
            var myAthlete = logic.GetAthleteForUserName(User.Identity.Name);

            string oldPin = logic.GetPin(myAthlete.AthleteID);
            logic.SetPin(myAthlete.AthleteID, newPin);

            // try changing the password
            // this will only work if the old pin was the password, otherwise this will fail and the password will stay
            try
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), oldPin, newPin);
                if (result.Succeeded)
                    return new ChangePinResultWebModel() { PinChanged = true, PasswordChanged = true };
            }
            catch (Exception)
            {
            }

            return new ChangePinResultWebModel() { PinChanged = true, PasswordChanged = false };
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(Awpbs.RegisterWebModel model)
        {
            // temp: changing passwords on some accounts
            //try
            //{
            //    UserManager.ChangePassword("d786aeaf-6035-43eb-afb1-6a8c833f21cc", LoginHelper.BuildPasswordFromEmail_PreAugust2015("mg@omegawave.com"), LoginHelper.BuildLoginPasswordFromFacebookId("10153286890728926"));
            //    UserManager.ChangePassword("85747d20-d9c6-47b0-939f-b2279c0d9453", LoginHelper.BuildPasswordFromEmail_PreAugust2015("sergei.v.larionov@gmail.com"), LoginHelper.BuildLoginPasswordFromFacebookId("10102050416755296"));
            //    UserManager.ChangePassword("8958934d-ca86-420d-9116-c4082a59e9c1", LoginHelper.BuildPasswordFromEmail_PreAugust2015("ruta_sakaviciute@yahoo.com"), LoginHelper.BuildLoginPasswordFromFacebookId("10155681835300533"));
            //}
            //catch (Exception exc)
            //{
            //    int asdf = 15;
            //}

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // validate facebook's access token, if provided
            bool hasFacebook = string.IsNullOrEmpty(model.FacebookId) == false;
            BasicFacebookProfileInfo facebookProfileInfo = null;
            if (hasFacebook)
            {
                facebookProfileInfo = await FacebookAuthHelper.Validate(model.FacebookAccessToken);
                if (facebookProfileInfo == null)
                    throw new Exception("Couldn't validate Facebook's access token");
            }

            var db = new ApplicationDbContext();
            var logic = new UserProfileLogic(db);

            // make sure that there is no user by this email in the system
            var existingUsersWithThisEmail = (from i in db.Athletes
                                              where i.RealEmail != null
                                              where i.RealEmail.ToLower() == model.Email.ToLower()
                                              select i).ToList();
            if (existingUsersWithThisEmail.Count > 0)
                throw new Exception("There is already a user with this email");

            // create a user
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            
            // create athlete record
            string nameInFacebook = null;
            if (hasFacebook)
                nameInFacebook = model.Name;
            var athlete = logic.CreateAthleteForUserName(model.Email, model.FacebookId, nameInFacebook);
            logic.UpdateAthleteProfile(athlete.AthleteID, model.Name, (GenderEnum)model.Gender, model.DOB);
            if (hasFacebook && string.IsNullOrEmpty(facebookProfileInfo.EMail) == false)
                logic.UpdateRealEmail(athlete.AthleteID, facebookProfileInfo.EMail);

            // if the password fits the PIN format - use the password as the PIN
            if (new AccessPinHelper().Validate(model.Password))
            {
                logic.SetPin(athlete.AthleteID, model.Password);
            }

            // update picture
            string picture = null;
            if (hasFacebook)
                picture = "http://res.cloudinary.com/bestmybest/image/facebook/" + HttpUtility.UrlEncode(model.FacebookId) + ".jpg";
            if (picture != null)
                logic.UpdateAthletePicture(athlete.AthleteID, picture);

            return Ok();
        }

        // POST api/Account/RegisterFVO
        [AllowAnonymous]
        [Route("RegisterFVO")]
        public async Task<int?> RegisterFVO(Awpbs.RegisterWebModelFVO model)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            string password = model.Pin; // set pin as the password

            var db = new ApplicationDbContext();
            var logic = new UserProfileLogic(db);

            // make sure that there is no user by this email in the system
            var existingUsersWithThisEmail = (from i in db.Athletes
                                              where i.RealEmail != null
                                              where i.RealEmail.ToLower() == model.Email.ToLower()
                                              select i).ToList();
            if (existingUsersWithThisEmail.Count > 0)
                throw new Exception("There is already a user with this email");

            // create a user
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            IdentityResult result = await UserManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return null;
            }

            // create athlete record
            var athlete = logic.CreateAthleteForUserName(model.Email, null, null);
            athlete.Name = model.Name;
            var venue = db.Venues.Where(i => i.VenueID == model.VenueID).Single();
            athlete.Country = venue.Country;
            athlete.MetroID = venue.MetroID ?? 0;
            db.SaveChanges();

            // set the PIN
            logic.SetPin(athlete.AthleteID, model.Pin);

            return athlete.AthleteID;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("AthleteIDFromUserName")]
        //public int AthleteIDFromUserName(string userName)
        //{
        //    var db = new ApplicationDbContext();
        //    var user = new UserProfileLogic(db).GetAthleteForUserName(userName);
        //    return user.AthleteID;
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
    }
}
