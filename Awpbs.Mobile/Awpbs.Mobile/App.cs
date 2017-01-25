using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
//using BranchXamarinSDK;

namespace Awpbs.Mobile
{
	public class App : Application//, IBranchSessionInterface
    {
        //#region IBranchSessionInterface implementation

        //public void InitSessionComplete(Dictionary<string, object> data)
        //{
        //    //throw new NotImplementedException ();
        //}

        //public void CloseSessionComplete()
        //{
        //    //throw new NotImplementedException ();
        //}

        //public void SessionRequestError(BranchError error)
        //{
        //    //throw new NotImplementedException ();
        //}

        //#endregion

        public static UserPreferences UserPreferences { get; set; }
        public static IFiles Files { get; set; }
        public static IKeyChain KeyChain { get; set; }
        public static Repository Repository { get; set; }
        public static WebService WebService { get; set; }
        public static CacheService Cache { get; set; }
        public static IScorePronouncer ScorePronouncer { get; set; }
        public static FacebookService FacebookService { get; set; }
        public static ILocationService LocationService { get; set; }
        public static ImagesService ImagesService { get; set; }
        public static NotificationsService NotificationsService { get; set; }
        public static SyncServiceBase Sync { get; set; }
        public static IMobileNotificationsService MobileNotificationsService { get; set; }

        public static LoginAndRegistrationLogic LoginAndRegistrationLogic { get; set; }
        public static Navigator Navigator { get; set; }

        public static DateTime TimeStarted { get; private set; }

		public App ()
		{
            TimeStarted = DateTime.UtcNow;

            // setup local database
            var databaseSetup = new DatabaseSetup();
            if (!databaseSetup.Install())
            {
                MainPage = new NavigationPage(new ErrorPage("Failed to install the database." + TraceHelper.ExceptionToString(databaseSetup.Exception)));
                return;
            }

            // init services
            UserPreferences = new UserPreferences();
            Repository = new Mobile.Repository();
            WebService = new Mobile.WebService(KeyChain);// Repository.GetAccessToken());
            Cache = new CacheService(WebService);
            ImagesService = new ImagesService();
            if (Config.App == MobileAppEnum.Snooker)
                Sync = new SyncServiceRegular(Repository, WebService);
            else
                Sync = new SyncServiceFVO(Repository, WebService);
            NotificationsService = new NotificationsService(WebService);
            LoginAndRegistrationLogic = new Mobile.LoginAndRegistrationLogic(FacebookService, Repository, WebService, KeyChain);
            Navigator = new Navigator(NotificationsService);

            // styles
            this.buildResources();

            // open a page
            if (LoginAndRegistrationLogic.RegistrationStatus == RegistrationStatusEnum.FirstStarted)
                LoginAndRegistrationLogic.StartSetup();
            else
                Navigator.OpenMainPage();
		}

		protected override void OnStart ()
		{
            base.OnStart();
			// Handle when your app starts
		}

		//protected override async void OnSleep ()
		//{
  //          base.OnSleep();

  //          //Branch branch = Branch.GetInstance();
  //          //await branch.CloseSessionAsync();
  //      }

		//protected override async void OnResume ()
		//{
  //          await base.OnResume();

  //          //Branch branch = Branch.GetInstance();
  //          //await branch.InitSessionAsync(this);
  //      }

        void buildResources()
        {
            var resources = new ResourceDictionary();
            this.addImplicitStyles(resources);
            this.addNamedStyles(resources);
            Application.Current.Resources = resources;
        }

        void addImplicitStyles(ResourceDictionary resources)
        {
            resources.Add(new Style(typeof(ContentPage))
            {
                Setters =
                {
                    new Setter { Property = ContentPage.PaddingProperty, Value = new Thickness(0, Device.OnPlatform(10, 0, 0), 0, 0) }
                }
            });
            resources.Add(new Style(typeof(ScrollView))
            {
                Setters =
                {
                    new Setter { Property = ScrollView.PaddingProperty, Value = new Thickness(20,10,20,30) }
                }
            });
            resources.Add(new Style(typeof(Frame))
            {
                Setters =
                {
                    new Setter { Property = Frame.HasShadowProperty, Value = false }
                }
            });
            resources.Add(new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Label.FontSizeProperty, Value = Config.DefaultFontSize },
                    new Setter { Property = Label.TextColorProperty, Value = Config.ColorBlackTextOnWhite },
                }
            });
            resources.Add(new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Button.FontSizeProperty, Value = Config.DefaultFontSize },
                }
            });
        }

        void addNamedStyles(ResourceDictionary resources)
        {
            // headers
//            resources.Add("Header1Style", new Style(typeof(Label))
//            {
//                Setters =
//                {
//                    new Setter { Property = Label.FontFamilyProperty, Value = Config.FontFamily },
//                    new Setter { Property = Label.FontSizeProperty, Value = Config.VeryLargeFontSize },
//                    new Setter { Property = Label.FontAttributesProperty, Value = Config.App == MobileAppEnum.SnookerForVenues ? FontAttributes.None : FontAttributes.Bold },
//                    new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
//				    new Setter { Property = Label.TextColorProperty, Value = Config.ColorBlackTextOnWhite },
//                }
//            });
//            resources.Add("Header2Style", new Style(typeof(Label))
//            {
//                Setters =
//                {
//                    new Setter { Property = Label.FontFamilyProperty, Value = Config.FontFamily },
//                    new Setter { Property = Label.FontSizeProperty, Value = Config.LargerFontSize },
//                    new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
//			        new Setter { Property = Label.TextColorProperty, Value = Config.ColorBlackTextOnWhite },
//                }
//            });

            // boxviews for padding
            resources.Add("BoxViewPadding1Style", new Style(typeof(Frame))
            {
                Setters =
                {
                    new Setter { Property = BoxView.HeightRequestProperty, Value = 20 },
                    new Setter { Property = BoxView.ColorProperty, Value = Color.Transparent }
                }
            });

            // buttons
            resources.Add("SimpleButtonStyle", new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Button.FontSizeProperty, Value = Config.DefaultFontSize },
                    new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },
                    new Setter { Property = Button.BackgroundColorProperty, Value = Color.Transparent },
                    new Setter { Property = Button.TextColorProperty, Value = Color.Black },
                }
            });
            resources.Add("BlackButtonStyle", new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Button.FontSizeProperty, Value = Config.DefaultFontSize },
                    new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },

                    new Setter { Property = Button.TextColorProperty, Value = Config.ColorTextOnBackground },
                    new Setter { Property = Button.BackgroundColorProperty, Value = Config.ColorBackgroundLogo },

                    new Setter { Property = Button.HeightRequestProperty, Value = Config.LargeButtonsHeight },
                    new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                    new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.Center },
                    
                    new Setter { Property = Button.BorderRadiusProperty, Value = 0 },
                }
            });
            resources.Add("LargeButtonStyle", new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Button.FontSizeProperty, Value = Config.DefaultFontSize },
                    new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },

                    new Setter { Property = Button.TextColorProperty, Value = Color.White },
                    new Setter { Property = Button.BackgroundColorProperty, Value = Config.ColorRedBackground },
                    
                    new Setter { Property = Button.HeightRequestProperty, Value = Config.LargeButtonsHeight },
                    new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                    new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.Center },

                    new Setter { Property = Button.BorderRadiusProperty, Value = 0 },
                }
            });
            resources.Add("LargeButtonOfSetWidthStyle", new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Button.FontSizeProperty, Value = Config.DefaultFontSize },
                    new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },

                    new Setter { Property = Button.TextColorProperty, Value = Color.White },
                    new Setter { Property = Button.BackgroundColorProperty, Value = Config.ColorRedBackground },

                    new Setter { Property = Button.HeightRequestProperty, Value = Config.LargeButtonsHeight },
                    new Setter { Property = Button.WidthRequestProperty, Value = 350 },
                    new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.Center },

                    new Setter { Property = Button.BorderRadiusProperty, Value = 0 },
                }
            });
            resources.Add("ConfirmButtonStyle", new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.BorderRadiusProperty, Value = 0 },
                    new Setter { Property = Button.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Button.FontSizeProperty, Value = Config.DefaultFontSize },
                    new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },
                    new Setter { Property = Button.BackgroundColorProperty, Value = Color.Transparent },
                    new Setter { Property = Button.TextColorProperty, Value = Config.ColorGreen },
                    new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.Center },
                }
            });
            resources.Add("DenyButtonStyle", new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.BorderRadiusProperty, Value = 0 },
                    new Setter { Property = Button.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Button.FontSizeProperty, Value = Config.DefaultFontSize },
                    new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },
                    new Setter { Property = Button.BackgroundColorProperty, Value = Color.Transparent },
                    new Setter { Property = Button.TextColorProperty, Value = Config.ColorRed },
                    new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.Center },
                }
            });

            // labels
            resources.Add("LabelOnBackgroundStyle", new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center },
                    new Setter { Property = Label.TextColorProperty, Value = Config.ColorTextOnBackground },
                    new Setter { Property = Label.FontSizeProperty, Value = Config.DefaultFontSize }
                }
            });
            resources.Add("LabelOnBackgroundGrayedStyle", new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.FontFamilyProperty, Value = Config.FontFamily },
                    new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center },
                    new Setter { Property = Label.TextColorProperty, Value = Config.ColorTextOnBackgroundGrayed },
                    new Setter { Property = Label.FontSizeProperty, Value = Config.DefaultFontSize }
                }
            });
        }
	}
}
