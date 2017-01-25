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

            // init
            Awpbs.Mobile.iOS.AppInitCode.InitApp(MobileAppEnum.SnookerForVenues);

            // open up
            LoadApplication(new Awpbs.Mobile.App());

            // ask for notifications permissions
            //Awpbs.Mobile.App.MobileNotificationsService.AskForNotificationsPermissions();

            return base.FinishedLaunching(app, options);
        }

        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("!!!!! AppDelegate.OnActivated");

            App.Navigator.CheckApiVersionAndNotifyIfNeeded();

            // Call the 'ActivateApp' method to log an app event for use
            // in analytics and advertising reporting. This is optional
            //Facebook.CoreKit.AppEvents.ActivateApp();

            base.OnActivated(application);
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            Console.WriteLine("!!!!! AppDelegate.OnResignActivation");

            base.OnResignActivation(uiApplication);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("!!!!! AppDelegate.DidEnterBackground");

            base.DidEnterBackground(application);
        }
    }
}