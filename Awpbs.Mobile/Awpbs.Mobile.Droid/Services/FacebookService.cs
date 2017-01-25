using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
//using Org.Json;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace Awpbs.Mobile.Droid
{
    public class FacebookService_Android : FacebookService
    {
        public FacebookService_Android() {}

        public override bool StartLogin()
        {
            try
            {
                MainActivity.The.StartActivity(typeof(FacebookActivity));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public override bool Share(string text)
		public override void Share(string text, EventHandler<bool> done)
		{
			/* FIXME */
            try
            {
                return;
            }

            catch (Exception)
            {
                return;
            }
        }

        public override bool IsLoggedIn
        {
            get
            {
                try
                {
                    if (AccessToken.CurrentAccessToken != null)
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
                    return AccessToken.CurrentAccessToken.Token;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }
}
