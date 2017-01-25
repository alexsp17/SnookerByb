using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ObjCRuntime;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
//using BranchXamarinSDK;

namespace Awpbs.Mobile.iOS.Snooker2
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : XFormsApplicationDelegate // IBranchSessionInterface
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Console.WriteLine("!!!!! AppDelegate.FinishedLaunching");
            Console.WriteLine("(2) Device.Idiom=" + Device.Idiom.ToString());

            // init
            Awpbs.Mobile.iOS.AppInitCode.InitApp(MobileAppEnum.Snooker);

            // open up
            LoadApplication(new Awpbs.Mobile.App());

            // ask for notifications permissions
            Awpbs.Mobile.App.MobileNotificationsService.AskForNotificationsPermissions();

            // is the app opening because of a remote notification?
            if (options != null && options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
            {
                NSDictionary remoteNotification = options[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;

                // process this notification in a second (so that the app completed the initialization)
                System.Timers.Timer timer = new System.Timers.Timer(1000);
                timer.Start();
                timer.Elapsed += (s1, e1) =>
                {
                    timer.Stop();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            this.processRemoteNotification(remoteNotification, true);
                        }
                        catch (Exception exc)
                        {
                            App.Navigator.DisplayAlertRegular("error processing remote notification from FinishedLaunching - " + TraceHelper.ExceptionToString(exc));
                            return;
                        }
                    });
                };
            }

            return base.FinishedLaunching(app, options);
        }

        #region App lifecycle events

        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("!!!!! AppDelegate.OnActivated");

            App.Navigator.CheckApiVersionAndNotifyIfNeeded();

            // Call the 'ActivateApp' method to log an app event for use
            // in analytics and advertising reporting. This is optional
            Facebook.CoreKit.AppEvents.ActivateApp();

            //Branch branch = Branch.GetInstance();
            //branch.InitSessionAsync(this);

            base.OnActivated(application);
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            Console.WriteLine("!!!!! AppDelegate.OnResignActivation");

            //Branch branch = Branch.GetInstance();
            //await branch.CloseSessionAsync(this);

            base.OnResignActivation(uiApplication);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("!!!!! AppDelegate.DidEnterBackground");

            base.DidEnterBackground(application);
        }

        #endregion

        #region Remote notifications

        public override async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            string deviceTokenStr = deviceToken.ToString();
            Console.WriteLine(deviceTokenStr);

            bool deviceTokenAlreadyRegistered = string.Compare(deviceTokenStr, Awpbs.Mobile.App.KeyChain.RegisteredDeviceToken, false) == 0;
            if (deviceTokenAlreadyRegistered == false)
            {
                if (App.LoginAndRegistrationLogic.RegistrationStatus == RegistrationStatusEnum.Registered)
                {
                    // already logged-in as a user -> register the device token
                    if (await App.WebService.RegisterDeviceToken(deviceTokenStr, true, false) == true)
                        Awpbs.Mobile.App.KeyChain.RegisteredDeviceToken = deviceTokenStr;
                }
                else
                {
                    // not yet logged-in as a user -> register the device token once logged-in
                    Awpbs.Mobile.App.KeyChain.DeviceTokenToRegisterWhenLoggedIn = deviceTokenStr;
                }
            }

            // https://www.youtube.com/watch?v=m2txoLvosVw
            // https://mallibone.com/post/xamarin.ios-push-notifications-through-azure-push-notifications
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            try
            {
                bool isAwakening = application.ApplicationState == UIApplicationState.Inactive || application.ApplicationState == UIApplicationState.Background;
                this.processRemoteNotification(userInfo, isAwakening);
            }
            catch (Exception exc)
            {
                App.Navigator.DisplayAlertRegular("error in doOnRemoteNotification - " + exc.Message);
            }
        }

        void processRemoteNotification(NSDictionary remoteNotification, bool isAppStartingOrAwakening)
        {
            //string text = remoteNotification.ToString();
            //App.Navigator.NavPage.DisplayAlert("MESSAGE!", text, "Cancel");

            try
            {
                // message text
                var aps = (NSDictionary)remoteNotification["aps"];
                string text = aps["alert"].ToString();

                // "object ID"
                int objectID = 0;
                var custom = (NSObject)remoteNotification["id"];
                if (custom != null)
                {
                    string strObjectID = custom.ToString();
                    int.TryParse(strObjectID, out objectID);
                }

                //App.Navigator.NavPage.DisplayAlert("MESSAGE", "text=" + text + ", objectID=" + objectID, "Cancel");

                PushNotificationMessage message = new PushNotificationMessage() { Text = text, ObjectID = objectID };
                Awpbs.Mobile.App.Navigator.ProcessRemoteNotification(message, isAppStartingOrAwakening);

                // reset the badge (this also deleted notifications)
                //UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            }
            catch (Exception exc)
            {
                string exceptionText = TraceHelper.ExceptionToString(exc);
                App.Navigator.NavPage.DisplayAlert("MESSAGE", "EXCEPTION: " + exceptionText, "Cancel");
            }
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            App.Navigator.GoToEvents();
            App.Navigator.NavPage.DisplayAlert(notification.AlertTitle, notification.AlertBody, "OK");

            // reset the badge (this also deleted notifications)
            //UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        #endregion

        #region Universal links and App links

        /// <summary>
        /// this is call when the app is opening because a universal link is clicked (iOS 9 min)
        /// </summary>
        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            Console.WriteLine("!!!!! AppDelegate.ContinueUserActivity");
            if (userActivity.ActivityType == NSUserActivityType.BrowsingWeb)
                Console.WriteLine("ActivityType == BrowsingWeb");

            string url = userActivity.WebPageUrl.ToString();
            Console.WriteLine(url);

            int index = url.IndexOf("$deeplink_path=");
            if (index > 0)
            {
                // this is a link like https://bnc.lt/a/.../deeplink_path=athlete/2
                // convert this to https://snookerbyb.com/athlete/2
                string newUrl = url.Substring(index + 15, url.Length - index - 15);
                url = "https://snookerbyb.com/" + newUrl;
                Console.WriteLine("converted url to: " + url);
            }

            App.Navigator.ProcessOpenUrl(url);
            
            return true; // note: do NOT call the base
        }

        /// <summary>
        /// this is called when the app is opening because an App Link is clicked (prior iOS 9)
        /// </summary>
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            Console.WriteLine("!!!!! AppDelegate.OpenUrl");

            string urlStr = url.ToString().ToLower();
            Console.WriteLine("url=" + urlStr);

            if (urlStr.Contains("branch.lt") || urlStr.Contains("bnc.lt"))
            {
                //    BranchIOS.getInstance().SetNewUrl(url);
                return true;
            }

            if (urlStr.StartsWith("snookerbyb") || urlStr.StartsWith("https://snookerbyb.com"))
            {
				App.Navigator.ProcessOpenUrl(url.ToString());
				return true;
            }

            // We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
            return Facebook.CoreKit.ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        //IBranchSessionInterface implementation

        //public void InitSessionComplete(Dictionary<string, object> data)
        //{
        //    Console.WriteLine("!!!!! AppDelegate.InitSessionComplete. data.Count=" + data.Count());
        //    foreach (var item in data)
        //        Console.WriteLine("----- key=" + item.Key + "; value=" + item.Value);
        //    Console.WriteLine("done");

        //    if (data.ContainsKey("$deeplink_path"))
        //    {
        //        string url = (string)data["$deeplink_path"];
        //        App.Navigator.ProcessOpenUrl(url);
        //    }

        //    // Do something with the referring link data...
        //}

        //public void CloseSessionComplete()
        //{
        //    Console.WriteLine("!!!!! AppDelegate.CloseSessionComplete");

        //    // Handle any additional cleanup after the session is closed
        //}

        //public void SessionRequestError(BranchError error)
        //{
        //    Console.WriteLine("!!!!! AppDelegate.SessionRequestError");

        //    // Handle the error case here
        //}

        #endregion
    }
}