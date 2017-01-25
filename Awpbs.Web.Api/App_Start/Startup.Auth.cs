using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Awpbs.Web.Api.Providers;
using Awpbs.Web.Api.Models;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Awpbs.Web.Api
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new CustomApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(356*10),// TimeSpan.FromMinutes(2),
                AllowInsecureHttp = true,
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }

    /// <summary>
    /// Added logic to validate against directly provided Facebook's access tokens
    /// </summary>
    public class CustomApplicationOAuthProvider : ApplicationOAuthProvider
    {
        public CustomApplicationOAuthProvider(string publicClientId)
            : base(publicClientId)
        {
        }

        public override async Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            try
            {
                var db = new ApplicationDbContext();
                string userName = context.Identity.GetUserName();
                var athlete = new UserProfileLogic(db).GetAthleteForUserName(userName);
                if (athlete == null)
                    throw new Exception("athlete record doesn't exist!");

                // if this is a facebook account => make sure that the client provided a valid facebookToken
                if (athlete.HasFacebookId)
                {
                    string facebookToken = context.TokenEndpointRequest.Parameters.Get("facebookToken");
                    var facebookProfileInfo = await FacebookAuthHelper.Validate(facebookToken);
                    if (facebookProfileInfo == null)
                        throw new Exception("couldn't validate facebook token!");
                }
            }
            catch (Exception)
            {
                throw new Exception("Additional Byb validation failed");
            }

            await base.TokenEndpointResponse(context);
        }
    }

    public class BasicFacebookProfileInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
    }

    public class FacebookGraphMeResult
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
    }

    public class FacebookAuthHelper
    {
        /// <summary>
        /// Executes a facebook graph request using a provided faceook access token
        /// </summary>
        public static async Task<BasicFacebookProfileInfo> Validate(string facebookAccessToken)
        {
            try
            {
                string url = string.Format("https://graph.facebook.com/v2.4/me?access_token={0}&fields=id,name,email", facebookAccessToken);

                var request = HttpWebRequest.Create(url);
                request.Method = "GET";

                HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                string json;
                using (Stream responseStream = httpResponse.GetResponseStream())
                {
                    json = new StreamReader(responseStream).ReadToEnd();
                }
                FacebookGraphMeResult result = JsonConvert.DeserializeObject<FacebookGraphMeResult>(json);
                if (result == null || string.IsNullOrEmpty(result.ID))
                    return null;
                return new BasicFacebookProfileInfo()
                {
                    Id = result.ID,
                    Name = result.Name,
                    EMail = result.EMail
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
