using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Facebook.CoreKit;
using UIKit;
using Foundation;

namespace Awpbs.Mobile.iOS
{
    public class FacebookService_iOS : FacebookService
    {
        public FacebookService_iOS()
        {
            this.init();
        }


        private static bool isSdkInitialized = false;

        private void init()
        {
            if (isSdkInitialized == true)
                return;
            try
            {
                // This is false by default,
                // If you set true, you can handle the user profile info once is logged into FB with the Profile.Notifications.ObserveDidChange notification,
                // If you set false, you need to get the user Profile info by hand with a GraphRequest
                Facebook.CoreKit.Profile.EnableUpdatesOnAccessTokenChange(true);

                Facebook.CoreKit.Settings.AppID = "1433294136990775";
                Facebook.CoreKit.Settings.DisplayName = Config.ProductName;
                //if (Config.App == MobileAppEnum.SnookerForVenues)
                //    Facebook.CoreKit.Settings.AppUrlSchemeSuffix = "fvo";
                //Facebook.CoreKit.Settings.

                isSdkInitialized = true;
            }
            catch (Exception)
            {
            }
        }

        public override bool StartLogin()
        {
            try
            {
                List<string> readPermissions = new List<string> { "public_profile", "email" };
                new Facebook.LoginKit.LoginManager().LogInWithReadPermissions(readPermissions.ToArray(), onLoginManagerRequestTokenHandler);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        async System.Threading.Tasks.Task doAGraphReqest_Status(string text, string publishToken, EventHandler<bool> done)
        {
            try
            {
                BasicFacebookProfileInfo facebookProfile = await this.GetBasicProfileInfo();

                var parameters = new NSDictionary("message", text);
                string userID = facebookProfile.Id;
                string graphPath = "/" + userID + "/feed";

                var request = new Facebook.CoreKit.GraphRequest(graphPath, parameters, publishToken, null, "POST");
                var requestConnection = new GraphRequestConnection();
                requestConnection.AddRequest(request, (connection, result1, error1) => {
                    if (error1 != null)
                    {
                        done(this, false);
                        return;
                    }

                    string id = (result1 as NSDictionary)["post_id"].ToString();
                });
                requestConnection.Start();
            }
            catch (Exception)
            {
                done(this, false);
            }
        }

        async System.Threading.Tasks.Task doAGraphReqest_Photo(string text, string publishToken, EventHandler<bool> done)
        {
            try
            {
                BasicFacebookProfileInfo facebookProfile = await this.GetBasicProfileInfo();

                var parameters = new NSDictionary("picture", UIImage.FromFile("logo-512x512.png").AsPNG(), "caption", text);
                string userID = facebookProfile.Id;
                string graphPath = "/" + userID + "/photos";

                var request = new Facebook.CoreKit.GraphRequest(graphPath, parameters, publishToken, null, "POST");
                var requestConnection = new GraphRequestConnection();
                requestConnection.AddRequest(request, (connection, result1, error1) => {
                    if (error1 != null)
                    {
                        done(this, false);
                        return;
                    }

                    string id = (result1 as NSDictionary)["post_id"].ToString();
                });
                requestConnection.Start();
            }
            catch (Exception)
            {
                done(this, false);
            }
        }

        public override void Share(string text, EventHandler<bool> done)
        {
            try
            {
                var login = new Facebook.LoginKit.LoginManager();
                login.LogInWithPublishPermissions(new[] { "publish_actions" }, async (result, error) => 
                {
                    if (error != null)
                    {
                        done(this, false);
                        return;
                    }

                    // Handle if the user cancelled the request
                    if (result.IsCancelled)
                    {
                        done(this, false);
                        return;
                    }

                    string publishToken = result.Token.TokenString;
                    await doAGraphReqest_Status(text, publishToken, done);
                });
                //Facebook.ShareKit.ShareOpenGraphAction share = Facebook.ShareKit.ShareOpenGraphAction.Action("like", )
            }
            catch (Exception)
            {
                done(this, false);
            }
        }

        public void onLoginManagerRequestTokenHandler(Facebook.LoginKit.LoginManagerLoginResult result, Foundation.NSError error)
        {
            if (result.IsCancelled || this.IsLoggedIn == false)
                this.OnLoginFailed();
            else
                this.OnLoginSucessful();
        }

        public override bool IsLoggedIn
        {
            get
            {
                try
                {
                    if (Facebook.CoreKit.AccessToken.CurrentAccessToken != null)
                        return true;
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public override string FacebookAccessToken
        {
            get
            {
                try
                {
                    return Facebook.CoreKit.AccessToken.CurrentAccessToken.TokenString;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}