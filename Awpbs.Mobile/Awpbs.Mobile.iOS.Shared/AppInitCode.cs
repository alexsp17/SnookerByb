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

namespace Awpbs.Mobile.iOS
{
    public static class AppInitCode
    {
        public static void InitApp(MobileAppEnum appType)
        {
            // init XLabs
            var container = new XLabs.Ioc.SimpleContainer();
            container.Register<XLabs.Platform.Device.IDevice>(t => XLabs.Platform.Device.AppleDevice.CurrentDevice);
            container.Register<XLabs.Platform.Services.Media.IMediaPicker, XLabs.Platform.Services.Media.MediaPicker>();
            XLabs.Ioc.Resolver.SetResolver(container.GetResolver());

            // forms Init
            Forms.Init();
            Xamarin.FormsMaps.Init();
            Refractored.XamForms.PullToRefresh.iOS.PullToRefreshLayoutRenderer.Init();
            RoundedBoxView.Forms.Plugin.iOSUnified.RoundedBoxViewRenderer.Init();

            // OS version
            string osVersion;
            try
            {
                osVersion = UIDevice.CurrentDevice.SystemVersion;
            }
            catch (Exception)
            {
                osVersion = "? exception";
            }

            // what's the device screen height?
            double deviceScreenHeightInInches;
            double deviceScreenWidthInInches;
            try
            {
                var device = Resolver.Resolve<IDevice>();
                deviceScreenHeightInInches = device.ScreenHeightInches();
                deviceScreenWidthInInches = device.ScreenWidthInches();
            }
            catch (Exception)
            {
                deviceScreenHeightInInches = 5;
                deviceScreenWidthInInches = 2;
            }

            // is simulator?
            try
            {
                Config.IsSimulator = ObjCRuntime.Runtime.Arch == Arch.SIMULATOR;
            }
            catch (Exception)
            {
                Config.IsSimulator = false;
            }

            // is it a Tablet?
            bool isTablet = false;
            Console.WriteLine("AppInitCode - Device.Idiom=" + Device.Idiom.ToString());
            Console.WriteLine("AppInitCode - deviceScreenHeightInInches=" + deviceScreenHeightInInches.ToString() + ", deviceScreenWidthInInches=" + deviceScreenWidthInInches.ToString());
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                isTablet = true;
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                isTablet = false;
            }
            else
            {
                isTablet = deviceScreenHeightInInches > 7 || deviceScreenWidthInInches > 7;
            }

            // init configuration
            Config.Init(appType, isTablet, deviceScreenHeightInInches, osVersion);

            // init services
            Awpbs.Mobile.App.Files = new Awpbs.Mobile.iOS.Files_iOS();
            Awpbs.Mobile.App.KeyChain = new KeyChain_UnsecuredFile(Awpbs.Mobile.App.Files);
            Awpbs.Mobile.App.ScorePronouncer = new Awpbs.Mobile.iOS.ScorePronouncer_iOS();
            Awpbs.Mobile.App.FacebookService = new Awpbs.Mobile.iOS.FacebookService_iOS();
            Awpbs.Mobile.App.LocationService = new Awpbs.Mobile.iOS.LocationService_iOS();
            Awpbs.Mobile.App.MobileNotificationsService = new Awpbs.Mobile.iOS.MobileNotificationsService_iOS();

            // branch init
            //NSUrl url = null;
            //if ((options != null) && options.ContainsKey(UIApplication.LaunchOptionsUrlKey))
            //{
            //    url = (NSUrl)options.ValueForKey(UIApplication.LaunchOptionsUrlKey);
            //    Console.WriteLine("AppDelegate.FinishedLaunching");
            //}
            //BranchXamarinSDK.BranchIOS.Init("key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe", url);
            //Branch branch = Branch.GetInstance();
            //branch.InitSessionAsync(this);
        }
    }
}
