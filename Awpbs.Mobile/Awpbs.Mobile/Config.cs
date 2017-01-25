using System;
using System.Collections.Generic;
using System.Text;
using Awpbs;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class Config
    {
        /// <summary>
        /// Set to true to use Production web service
        /// Set to false to use Development web service and turn development functions on
        /// </summary>
#if DEBUG
        public static readonly bool IsProduction = false;
#else
        public static readonly bool IsProduction = true;
#endif

#if __IOS__
        public static readonly bool IsIOS = true;
        public static readonly bool IsAndroid = false;
		public static readonly string AppStoreName = "App Store";
#endif
#if __ANDROID__
        public static readonly bool IsIOS = false;
        public static readonly bool IsAndroid = true;
		public static readonly string AppStoreName = "Google Play";
#endif

        public static Awpbs.MobileAppEnum App { get; private set; }
        public static string ProductName
        {
            get
            {
                if (App == MobileAppEnum.SnookerForVenues)
                    return "Snooker Byb for Venues";
                return "Snooker Byb";
            }
        }

        public static readonly string Athlete = "Snooker player";
        public static readonly string WebsiteName = "www.snookerbyb.com";
        public static readonly string DatabaseFileName = "BybDatabase.db";

        /// <summary>
        /// Set to true to clean up the database on start
        /// </summary>
        public const bool CleanUpTheDatabaseOnStart = false;

        public const string TestCredentials_Username = "test@test.com";
        public const string TestCredentials_Password = "testtest";
        
        // background colors
		public static readonly Color ColorBackgroundWhite = Color.White;
        public static readonly Color ColorBackground = Color.FromRgb(37, 36, 36);
        public static readonly Color ColorBackgroundLogo = Color.FromRgb(50, 50, 50);
        public static readonly Color ColorBlackBackground = Color.FromRgb(0, 0, 0);
        public static readonly Color ColorRedBackground = Color.FromRgb(219, 54, 56);
        public static readonly Color ColorGrayBackground = Color.FromRgb(240, 240, 240);

        // text colors
        public static readonly Color ColorTextOnBackground = Color.FromRgb(255, 255, 255);
        public static readonly Color ColorTextOnBackgroundGrayed = Color.FromRgb(140, 140, 140);
        public static readonly Color ColorGrayTextOnWhite = Color.FromRgb(147, 149, 152);
		public static readonly Color ColorBlackTextOnWhite = Color.Black;

		public static readonly Color ColorPageTitleBarTextNormal = ColorTextOnBackground;
		public static readonly Color ColorPageTitleBarTextAlert = Color.Red;

        // other colors
        public static readonly Color ColorGreen = Color.FromRgb(58, 181, 74);
        public static readonly Color ColorRed = Color.FromRgb(241, 89, 41);
        public static readonly Color ColorGray = Color.Gray;

        // font
        public static readonly string FontFamily = "Lato";
        public static readonly string FontFamilyBold = "Lato-Bold";

        // ball colors
        public static readonly List<Color> BallColors = new List<Color>
        {
            ColorGrayBackground,
            Color.FromRgb(220,51,56), // red
            Color.FromRgb(255,242,0), // yellow
            Color.FromRgb(57,181,74), // green
            Color.FromRgb(185, 122, 87), // brown
            Color.FromRgb(14,118,188), // blue
            Color.FromRgb(238,32,124), // pink
            Color.FromRgb(18,18,18), // black
        };

        // font sizes
        public static float SmallFontSize { get; private set; }
        public static float DefaultFontSize { get; private set; }
        public static float LargerFontSize { get; private set; }
        public static float VeryLargeFontSize { get; private set; }
		public static float SuperLargeFontSize { get; private set; }

        // spacing and sizes
        public static int TitleHeight { get; private set; }
        public static int OkCancelButtonsHeight { get; private set; }
        public static int OkCancelButtonsPadding { get; private set; }
        public static int LargeButtonsHeight { get; private set; }
        public static int PersonImageSize { get; private set; }
        public static int MyImageSize { get; private set; }
        public static int LikeImageSize { get; private set; }
        public static int CommentImageSize { get; private set; }
		public static int SpaceBetweenButtons { get; private set; }
        public static int RedArrowImageSize { get; private set; }

        // sizes of balls
        public static int SmallBallSize { get; private set; }
        public static int ExtraSmallBallSize { get; private set; }

        // device characteristics
        public static bool IsTablet { get; private set; }
        public static bool IsSimulator { get; set; }
        public static double DeviceScreenHeightInInches { get; private set; }
        public static string OSVersion { get; private set; }
        public static int OSVersionMajor
        {
            get
            {
                int version = 0;
                var strs = OSVersion.Split('.');
                if (strs.Length > 0)
                    int.TryParse(strs[0], out version);
                return version;
            }
        }

        public static void Init(Awpbs.MobileAppEnum app, bool isTablet, double deviceScreenHeightInInches, string osVersion)
        {
            App = app;

            IsTablet = isTablet;
            DeviceScreenHeightInInches = deviceScreenHeightInInches;
            OSVersion = osVersion;

            SmallFontSize = App == MobileAppEnum.SnookerForVenues ? 12 : (isTablet ? 10 : 8);       // small
            DefaultFontSize = App == MobileAppEnum.SnookerForVenues ? 18 : (isTablet ? 16 : 12.5f); // default
            LargerFontSize = App == MobileAppEnum.SnookerForVenues ? 23 : (isTablet ? 21 : 17);     // slighly larger than the default
			VeryLargeFontSize = App == MobileAppEnum.SnookerForVenues ? 28 : (isTablet ? 26 : 21);  // used in page titles
            SuperLargeFontSize = App == MobileAppEnum.SnookerForVenues ? 100 : (isTablet ? 31 : 25); // used in the frame score

            TitleHeight = isTablet ? 80 : 60;
            LargeButtonsHeight = isTablet ? 50 : 42;
            OkCancelButtonsPadding = isTablet ? 7 : 7;
            OkCancelButtonsHeight = LargeButtonsHeight;

            PersonImageSize = isTablet ? 70 : 50;
            MyImageSize = isTablet ? 140 : 100;

            SmallBallSize = isTablet ? 20 : 14;
            ExtraSmallBallSize = isTablet ? 14 : 12;

            LikeImageSize = isTablet ? 25 : 15;
            CommentImageSize = isTablet ? 23 : 13;

			SpaceBetweenButtons = IsAndroid ? 2 : 1; // on Android a space of 1 point between buttons is not visible (at least on some versions)

            RedArrowImageSize = IsTablet ? 15 : 12;
        }
    }
}
