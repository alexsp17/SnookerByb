// MG: based on the HelloFacebook sample of "Facebook Android SDK"

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;

using Xamarin.Forms.Platform.Android;
using Xamarin.Facebook;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Facebook.AppEvents;
using Xamarin.Facebook.Login;


namespace Awpbs.Mobile.Droid
{
	[Activity(Label = "@string/app_name", Theme = "@style/BybTheme", WindowSoftInputMode = SoftInput.AdjustResize)]
    public class FacebookActivity : Activity, Xamarin.Facebook.IFacebookCallback
    {
        static readonly string[] LOGIN_PERMISSIONS = new[] { "public_profile", "email" };

        Button thisIsMeButton;
		Button cancelButton;

        ProfilePictureView profilePictureView;
        TextView greeting;
        ICallbackManager callbackManager;
        ProfileTracker profileTracker;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                FacebookSdk.SdkInitialize(this.ApplicationContext);

                callbackManager = CallbackManagerFactory.Create();
                LoginManager.Instance.RegisterCallback(callbackManager, this);

                LoginManager.Instance.LogInWithReadPermissions(this, LOGIN_PERMISSIONS);
            }
            catch (Exception e)
            {
                string failureInfo = e.ToString();
                Console.WriteLine("FacebookActivity::onCreate() EXCEPTION " + failureInfo);
            }

            SetContentView(Resource.Layout.facebookLoginPage);

            profileTracker = new CustomProfileTracker
            {
                HandleCurrentProfileChanged = (oldProfile, currentProfile) =>
                {
                    UpdateUI();
                }
            };

			// properties applied to all buttons on Facebook login page
			var padding = Awpbs.Mobile.Config.OkCancelButtonsPadding;
			var minHeight = Awpbs.Mobile.Config.OkCancelButtonsHeight;
			Typeface typeface = Typeface.CreateFromAsset(Resources.Assets, "fonts/Lato-Regular.ttf");

			// Profile Picture
            profilePictureView = FindViewById<ProfilePictureView>(Resource.Id.profilePicture);
			profilePictureView.SetMinimumHeight (minHeight);
			profilePictureView.SetPadding (padding, padding, padding, padding);

			// Greeting "Hello, Alex!", visible when connected to facebook account
            greeting = FindViewById<TextView>(Resource.Id.greeting);
			greeting.SetMinimumHeight(minHeight);
			greeting.SetPadding (padding, padding, padding, padding);
			greeting.SetTypeface (typeface, TypefaceStyle.Bold);
			greeting.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.LargerFontSize);

			//
			// Cancel and Ok buttons at the bottom of the page
			//
			cancelButton = FindViewById<Button>(Resource.Id.CancelButtonFb);
			cancelButton.SetBackgroundColor (Config.ColorBackgroundLogo.ToAndroid());
			cancelButton.SetMinimumHeight(minHeight);
			cancelButton.SetPadding (padding, padding, padding, padding);
			cancelButton.SetTypeface (typeface, TypefaceStyle.Bold);
			cancelButton.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
			cancelButton.Click += (sender, e) =>
			{
				this.Finish();
				App.FacebookService.OnLoginFailed();
			};

            thisIsMeButton = FindViewById<Button>(Resource.Id.thisIsMeButton);
			thisIsMeButton.SetBackgroundColor (Config.ColorRedBackground.ToAndroid());
			thisIsMeButton.SetMinimumHeight(minHeight);
			thisIsMeButton.SetPadding (padding, padding, padding, padding);
			thisIsMeButton.SetTypeface (typeface, TypefaceStyle.Bold);
			thisIsMeButton.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
            thisIsMeButton.Click += (sender, e) =>
            {
                this.Finish();
                App.FacebookService.OnLoginSucessful();
            };
				
		} // onCreate()

        protected override void OnResume()
        {
            base.OnResume();

            AppEventsLogger.ActivateApp(this);

            UpdateUI();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }


        protected override void OnPause()
        {
            base.OnPause();

            AppEventsLogger.DeactivateApp(this);
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

		/*
		protected override void OnStop()
		{
			profileTracker.StopTracking();
			base.OnStop();

			this.Finish();
		}
		*/

        protected override void OnDestroy()
        {
            base.OnDestroy();

            profileTracker.StopTracking();
        }

        private void UpdateUI()
        {
            /*
            if (AccessToken.CurrentAccessToken != null)
            {
                Console.WriteLine("AccessToken.CurrentAccessToken != NULL");
            }

            if (Profile.CurrentProfile != null)
            {
                Console.WriteLine("AccessToken.CurrentProfile != NULL");
            }
            */

            var enableButtons = AccessToken.CurrentAccessToken != null;

            var profile = Profile.CurrentProfile;

            if (enableButtons && profile != null)
            {
                profilePictureView.ProfileId = profile.Id;
                greeting.Text = GetString(Resource.String.hello_user, new Java.Lang.String(profile.FirstName));
            }
            else
            {
                profilePictureView.ProfileId = null;
                greeting.Text = null;
            }
        }

        /*
         * IFacebookCallback interface
         */
        public void OnCancel()
        {
			this.Finish();
        }


        public void OnError(FacebookException p0)
        {
            string exceptionText = p0.ToString();
            Console.WriteLine("IFacebookCallback: OnError(): " + exceptionText);
        }


        public void OnSuccess(Java.Lang.Object result)
        {
            LoginResult loginResult = result as LoginResult;
            Console.WriteLine("IFacebookCallback:OnSuccess: AccessToken.UserId" + loginResult.AccessToken.UserId);

            if (Profile.CurrentProfile != null)
            {
                var profile = Profile.CurrentProfile;
                string firstName = GetString(Resource.String.hello_user, new Java.Lang.String(profile.FirstName));

                Console.WriteLine("IFacebookCallback:OnSuccess(): name is " + firstName);
            }
            else
            {
                Console.WriteLine("IFacebookCallback:AccessToken.CurrentProfile is NULL");
            }
        }

    }

    class CustomProfileTracker : ProfileTracker
    {
        public delegate void CurrentProfileChangedDelegate(Profile oldProfile, Profile currentProfile);

        public CurrentProfileChangedDelegate HandleCurrentProfileChanged { get; set; }

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            var p = HandleCurrentProfileChanged;
            if (p != null)
                p(oldProfile, currentProfile);
        }
    }
}