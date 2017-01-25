using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class Navigator
    {
        public BybNavigationPage NavPage
        {
            get;
            private set;
        }

        public MainSnookerPage RootPage
        {
            get { return this.rootPage; }
        }

        NotificationsService notificationsService;

        MainSnookerPage rootPage;
        FVOMainPage fvoMainPage;
        ToolbarItem toolBarMenuItem2;

        public Navigator(NotificationsService notificationsService)
        {
            this.notificationsService = notificationsService;

            this.notificationsService.SubscribeToLoadedEvent((s, e) =>
            {
                this.UpdateNavigationMenuButtons(null);
            });
        }

		public async Task<bool> ProcessOpenUrl(string url)
		{
			if (App.LoginAndRegistrationLogic.RegistrationStatus != RegistrationStatusEnum.Registered)
				return false;

            // for URL Schemas: 2015/09/25: read this: http://riccardo-moschetti.org/2014/10/03/opening-a-mobile-app-from-a-link-the-xamarin-way-url-schemas/
            // for Universal links: https://developer.apple.com/library/ios/documentation/General/Conceptual/AppSearch/UniversalLinks.html

            url = url.ToLower();

            if (!(url.StartsWith("https://snookerbyb.com") ||  // universal links
                  url.StartsWith("snookerbyb.com") || // universal links
                  url.StartsWith("snookerbyb:"))) // app links / URL schemas
                return false;

            try
            {
				// remove everything after "?"
				int indexQ = url.IndexOf('?');
				if (indexQ >= 0)
					url = url.Substring(0, indexQ);
				
				// parse the id
				int index = url.LastIndexOf("/");
				if (index < 0)
					return false;
				string strID = url.Substring(index + 1, url.Length - index - 1);
				int id;
				if (!int.TryParse(strID, out id))
					return false;

				if (url.Contains("/athlete") || url.Contains("/player"))
				{
					await this.GoToPersonProfile(id);
					return true;
				}
                if (url.Contains("/venue"))
                {
                    await this.GoToVenueProfile(id);
                    return true;
                }
                else if (url.Contains("/comment"))
                {
                    this.OpenNewsfeedItemPage(id);
                    return true;
                }
				else if (url.Contains("/gamehost") || url.Contains("/invite"))
				{
					bool isExpectedToAcceptInvitation = false;
					//if (url.Contains("/gamehost/accept"))
					//	isExpectedToAcceptInvitation = true;

					var page = new GameHostPage(isExpectedToAcceptInvitation);
					await this.NavPage.Navigation.PushModalAsync(page);
					await page.OpenGameHost(id);
					return true;
				}
                else if (url.Contains("/sync"))
                {
                    this.StartSyncAndCheckForNotifications();
                    await this.GoToMyProfile(ProfilePersonStateEnum.Matches, false);
                    return true;
                }
				else
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public Page GetOpenedPage(Type type)
		{
			var page = this.GetOpenedModalPage (type);
			if (page != null)
				return page;
			return this.GetOpenedNonmodalPage (type);
		}

		public Page GetOpenedModalPage(Type type)
		{
			foreach (Page page in this.NavPage.Navigation.ModalStack)
				if (page.GetType().Name == type.Name)
					return page;
			return null;
		}

		public Page GetOpenedNonmodalPage(Type type)
		{
			var page = this.NavPage.Navigation.NavigationStack.LastOrDefault();
			if (page == null)
				return null;
			if (page.GetType().Name == type.Name)
				return page;
			//foreach (Page page in this.NavPage.Navigation.NavigationStack)
			//	if (page.GetType().Name == type.Name)
			//		return page;
			return null;
		}

        public async void ProcessRemoteNotification(PushNotificationMessage message, bool isAppStartingOrAwakening)
        {
			Console.WriteLine("Navigator.ProcessRemoteNotification - message.Text=" + message.Text);
			Console.WriteLine("Navigator.ProcessRemoteNotification - message.ObjectID=" + message.ObjectID);

            PushNotificationMessageTypeEnum? messageType = PushNotificationMessage.ParseType(message.Text);

            if (messageType == PushNotificationMessageTypeEnum.FriendRequest)
            {
                if (isAppStartingOrAwakening)
                    CheckNotificationsAndOpenNotificationsPage();
                else
                    notificationsService.CheckForNotificationsIfNecessary();
            }
            else if (messageType == PushNotificationMessageTypeEnum.PrivateMessage)
            {
                if (message.ObjectID > 0)
                    await GoToPersonProfile(message.ObjectID);
                await NavPage.DisplayAlert("Byb Message", message.Text, "OK");
            }
            else if (messageType == PushNotificationMessageTypeEnum.GameInvite)
            {
                if (isAppStartingOrAwakening == false)
                    await NavPage.DisplayAlert("Byb Game Invite", "You were invited for a game of snooker by somebody.", "OK");
                await GoToEvents();
            }
            else if (messageType == PushNotificationMessageTypeEnum.Comment && message.ObjectID > 0)
            {
                OpenNewsfeedItemPage(message.ObjectID);
            }
            else
            {
                await GoToEvents();
            }
        }

        public void OpenNewsfeedItemPage(int commentID)
        {
            PleaseWaitPage pleaseWaitPage = new PleaseWaitPage();
            pleaseWaitPage.StatusText = "Loading.";
            pleaseWaitPage.ShowCancelButton = true;
            NavPage.Navigation.PushModalAsync(pleaseWaitPage);
            pleaseWaitPage.CancelButtonClicked += (s1, e1) =>
            {
                NavPage.Navigation.PopModalAsync();
                pleaseWaitPage = null;
            };

            Task task = new Task(async () =>
            {
                var item = await App.WebService.GetNewsfeedItemFromCommentID(commentID);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    // close the wait page
                    await NavPage.Navigation.PopModalAsync();

                    // open newsfeed item page
                    if (item == null)
                    {
                        await this.GoToCommunity();
                    }
                    else
                    {
                        NewsfeedItemPage page = new NewsfeedItemPage(item, true);
                        await NavPage.Navigation.PushModalAsync(page);
                    }
                });
            });
            task.Start();
        }

		public async void ShowInternalOptions()
		{
			string strAbout = "About";
			string strTestMessage = "Send yourself a message";
			string strWebserviceInfo = "Webservice info";
            string strTest1 = "Test";
			string strUnregister = "Unregister";

			string strInput = await NavPage.DisplayActionSheet ("Byb - Internal", "Cancel", null, strAbout, strWebserviceInfo, strTestMessage, strUnregister, strTest1);

			if (strInput == strAbout) {
				InfoPage page = new InfoPage ();
				await NavPage.Navigation.PushModalAsync (page);
			}

			if (strInput == strWebserviceInfo) {
				string text = "";
				text += "Last exception url: ";
				if (App.WebService.LastExceptionUrl != null)
					text += App.WebService.LastExceptionUrl;
				else
					text += "none";

				text += "\r\n\r\nLast exception: ";
				if (App.WebService.LastException == null)
					text += "null";
				else
					text += TraceHelper.ExceptionToString (App.WebService.LastException);

				InfoPage page = new InfoPage ();
				page.Text = text;
				await NavPage.Navigation.PushModalAsync (page);
			}

			if (strInput == strTestMessage) {
				string msg = string.Format("Message from '{0}' : {1}", "Miguel", "Some message!!!");
				App.MobileNotificationsService.AddLocalNotification("Byb", msg, 0);
				return;
			}

			if (strInput == strUnregister)
			{
				string fileName = System.IO.Path.Combine (App.Files.GetWritableFolder (), Config.DatabaseFileName);
				App.Files.DeleteFile (fileName);
				await NavPage.DisplayAlert ("Byb", "The local database deleted. Pease force-close the app now.", "Done");
				return;
			}

            if (strInput == strTest1 && Config.IsProduction == false)
            {
                OpenNewsfeedItemPage(1098);
                return;
            }

			//var athlete = App.Repository.GetMyAthlete();
			//					athlete.Country = "";
			//					athlete.MetroID = 0;
			//					App.Repository.UpdateAthlete(athlete);
			//					App.Navigator.DisplayAlertRegular("Set athlete's metro no empty. Try Edit Profile now.");
		}

        public void OpenLoginPage()
        {
            rootPage = null;

            NavPage = new BybNavigationPage(new SetupStartPage());
            App.Current.MainPage = NavPage;
        }

        public void OpenReloginPage(ReloginPage reloginPage)
        {
            rootPage = null;

            NavPage = new BybNavigationPage(reloginPage);
            App.Current.MainPage = NavPage;
        }

        public void OpenMainPage()
        {
            if (Config.App == MobileAppEnum.SnookerForVenues)
            {
                /// Snooker Byb For Venues
                /// 

                if (this.fvoMainPage != null)
                    return;

                this.fvoMainPage = new FVOMainPage();

                // create a "NavigationPage"
                this.NavPage = new BybNavigationPage(this.fvoMainPage);
                this.NavPage.BarBackgroundColor = Config.ColorBackground;
                this.NavPage.BarTextColor = Config.ColorPageTitleBarTextNormal;

                // make the "NavigationPage" the main page of the app
                App.Current.MainPage = this.NavPage;
            }
            else
            {
                /// Snooker Byb
                /// 

                if (this.rootPage != null)
                    return; // already open

                this.rootPage = new MainSnookerPage();
                this.rootPage.InitAsRecord();

                // create a "NavigationPage"
                this.NavPage = new BybNavigationPage(this.rootPage);
                this.NavPage.BarBackgroundColor = Config.ColorBackground;
                this.NavPage.BarTextColor = Config.ColorPageTitleBarTextNormal;

                // create a toolbar
                this.toolBarMenuItem2 = new ToolbarItem()
                {
                    Icon = new FileImageSource() { File = "alert1.png" },
                    Order = ToolbarItemOrder.Primary,
                    Command = new Command(() =>
                    {
                        this.OpenNotificationsPage();
                    }),
                };
                this.NavPage.ToolbarItems.Add(toolBarMenuItem2);

                // make the "NavigationPage" the main page of the app
                App.Current.MainPage = this.NavPage;

                App.Sync.StatusChanged += (s1, e1) =>
                {
                    if (e1.Completed == true)
                        doOnSyncCompleted(e1);
                };
            }

			CheckApiVersionAndNotifyIfNeeded ();
        }

        async void doOnSyncCompleted(SyncStatus syncStatus)
        {
            if (rootPage == null)
                return;
            if (NavPage.CurrentPage != rootPage)
                return;

            int myAthleteID = App.Repository.GetMyAthleteID();
			if (this.rootPage.State == MainSnookerPage.StateEnum.Person && this.rootPage.AthleteID == myAthleteID)
			{
				bool forceReload = true;
				await this.GoToMyProfile (ProfilePersonStateEnum.Unknown, forceReload);
			}

            // update the title of the root page
            if (App.Sync.LastSyncResults != null && App.Sync.LastSyncResults.Result == SyncResultEnum.Ok)
                rootPage.Title = "sync ok";
            else
                rootPage.Title = "sync error. Internet issues?";

            // reset the title of the root page back
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 2000;
            timer.Elapsed += (s1,e1) => {
                Device.BeginInvokeOnMainThread(() => {
                    timer.Stop();
                    rootPage.UpdateTheTitle();
                });
            };
            timer.Start();
        }

        public void DisplayAlertRegular(string text)
        {
            App.Navigator.NavPage.DisplayAlert("Byb", text, "OK");
        }

        public async Task DisplayAlertRegularAsync(string text)
        {
            await App.Navigator.NavPage.DisplayAlert("Byb", text, "OK");
        }

//		public async Task DisplayAlertRegularWithDelay(string text)
//		{
//			System.Timers.Timer timer = new System.Timers.Timer ();
//			timer.Interval = 1000;
//			timer.Elapsed += (s1, e1) => {
//				Device.BeginInvokeOnMainThread(() => {
//					timer.Stop();
//					App.Navigator.NavPage.DisplayAlert("Byb", text, "OK");
//				});
//			};
//			timer.Start ();
//		}

        public void DisplayAlertError(string text)
        {
            App.Navigator.NavPage.DisplayAlert("Error", text, "OK");
        }

        public async Task DisplayAlertErrorAsync(string text)
        {
            await App.Navigator.NavPage.DisplayAlert("Error", text, "OK");
        }

        public void StartSyncAndCheckForNotifications()
        {
            App.Navigator.NavPage.CurrentPage.Title = "syncing with snookerbyb.com...";
            App.Sync.StartAsync();
            notificationsService.CheckForNotificationsIfNecessary();
        }

		public async void OpenProfileEditPage(bool showCancelButton, bool showSecurityPanel)
        {
			if (this.GetOpenedModalPage (typeof(EditProfilePage)) != null)
				return;
			
			EditProfilePage dialogProfile = new EditProfilePage(showCancelButton, showSecurityPanel);
            await this.NavPage.Navigation.PushModalAsync(dialogProfile);
			await dialogProfile.Init ();
            dialogProfile.UserClickedOkOrCancel += async (s1, e1) =>
            {
                await App.Navigator.NavPage.Navigation.PopModalAsync();
                App.Navigator.OpenMainPage();
                await App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Unknown, true);
            };
        }

        public void OpenProfileImageEditPage()
        {
			if (this.GetOpenedPage (typeof(ProfileImageEditPage)) != null)
				return;
			
            ProfileImageEditPage dialog = new ProfileImageEditPage();
            NavPage.Navigation.PushModalAsync(dialog);
            dialog.Disappearing += async (s1, e1) =>
            {
                await GoToMyProfile(ProfilePersonStateEnum.Unknown, true);
            };
        }

        public void OpenProfileImageViewPage(PersonFullWebModel person)
        {
			if (this.GetOpenedPage (typeof(ProfileImageViewPage)) != null)
				return;
			
            ProfileImageViewPage page = new ProfileImageViewPage(person);
            NavPage.Navigation.PushAsync(page);
        }

        public async void CheckNotificationsAndOpenNotificationsPage()
        {
            PleaseWaitPage pleaseWaitPage = new PleaseWaitPage();
            pleaseWaitPage.StatusText = "Loading notifications.";
            pleaseWaitPage.ShowCancelButton = true;
            await NavPage.Navigation.PushAsync(pleaseWaitPage);
            pleaseWaitPage.CancelButtonClicked += (s1, e1) =>
            {
                NavPage.Navigation.PopAsync();
                pleaseWaitPage = null;
            };

            notificationsService.CheckForNotifications((s1, e1) =>
            {
                if (pleaseWaitPage == null)
                    return;
                NavPage.Navigation.PopAsync();
                OpenNotificationsPage();
            });
        }

        public void OpenNotificationsPage()
        {
            if (notificationsService.Data == null || notificationsService.Data.InternetIssues)
            {
                DisplayAlertRegular("Notifications not loaded. Internet issues?");
                return;
            }

			if (this.GetOpenedPage (typeof(NotificationsPage)) != null)
				return;

            NotificationsPage page = new NotificationsPage(notificationsService.Data);
            NavPage.Navigation.PushAsync(page);
            page.Disappearing += (s1, e1) =>
                {
                    notificationsService.FireLoadedEvent();
                };

            this.UpdateNavigationMenuButtons(false);
        }

        public async Task GoToRecord()
        {
            if (rootPage == null)
                return;

            await NavPage.PopToRootAsync();
            rootPage.InitAsRecord();

            this.UpdateNavigationMenuButtons(true);
        }

        public async Task GoToEvents()
        {
            if (rootPage == null)
                return;

            await NavPage.PopToRootAsync();
            rootPage.InitAsEvents();

            this.UpdateNavigationMenuButtons(true);
        }

        public async Task GoToCommunity()
        {
            if (rootPage == null)
                return;

			TraceHelper.TraceInfoForResponsiveness("GoToCommunity - Starting");
            await NavPage.PopToRootAsync();
			TraceHelper.TraceInfoForResponsiveness("GoToCommunity - Popped pages");
            rootPage.InitAsCommunity();
			TraceHelper.TraceInfoForResponsiveness("GoToCommunity - InitAsCommunity finished");

            this.UpdateNavigationMenuButtons(true);
			TraceHelper.TraceInfoForResponsiveness("GoToCommunity - Done");
        }

		public async Task GoToMyProfile(ProfilePersonStateEnum state, bool forceReload)
        {
            if (rootPage == null)
                return;

            await NavPage.PopToRootAsync();
			rootPage.InitAsMyProfile(state, forceReload);

            this.UpdateNavigationMenuButtons(true);
        }

        public async Task GoToPersonProfile(int athleteID, ProfilePersonStateEnum state = ProfilePersonStateEnum.Unknown)
        {
			var openedPage = this.GetOpenedPage(typeof(MainSnookerPage)) as MainSnookerPage;
			if (openedPage != null && openedPage.State == MainSnookerPage.StateEnum.Person && openedPage.AthleteID == athleteID)
				return;
			
            var newPage = new MainSnookerPage();
            newPage.InitAsPersonProfile(athleteID, state);
            await NavPage.PushAsync(newPage);

            this.UpdateNavigationMenuButtons(true);
        }

        public async Task GoToVenueProfile(int venueID)
        {
			var openedPage = this.GetOpenedPage(typeof(MainSnookerPage)) as MainSnookerPage;
			if (openedPage != null && openedPage.State == MainSnookerPage.StateEnum.Venue && openedPage.VenueID == venueID)
				
				return;
			
            var newPage = new MainSnookerPage();
            newPage.InitAsVenueProfile(venueID);
            await NavPage.PushAsync(newPage);

            this.UpdateNavigationMenuButtons(true);
        }

        public async void DoOnVenueEdited(int venueID)
        {
            await App.Navigator.NavPage.PopAsync();
            await App.Navigator.NavPage.PopAsync();

            foreach (var page in NavPage.Navigation.NavigationStack)
            {
                MainSnookerPage aPage = page as MainSnookerPage;
                if (aPage != null)
                    aPage.DoOnVenueEdited(venueID);
            }
        }

        //public async void GoToAdd()
        //{
        //    string actionAddBreak = "Add a Break";
        //    string actionAddMatch = "Add a Match";
        //    string actionGameHost = "Host a Game";
        //    string actionUpdateLatestMatch = "Edit the Unfinished Match";

        //    string action = "";

        //    // latest snooker match
        //    Score latestScore = App.Repository.GetLatestScore();
        //    SnookerMatchScore unfinishedMatch = null;
        //    if (latestScore != null && latestScore.IsUnfinished == true)
        //    {
        //        unfinishedMatch = SnookerMatchScore.FromScore(App.Repository.GetMyAthleteID(), latestScore);
        //        var opponent = App.Cache.People.Get(unfinishedMatch.OpponentAthleteID);
        //        unfinishedMatch.OpponentName = opponent != null ? opponent.Name : "not loaded";
        //        var venue = App.Cache.Venues.Get(unfinishedMatch.VenueID);
        //        unfinishedMatch.VenueName = venue != null ? venue.Name : "not loaded";
        //    }

        //    if (unfinishedMatch != null)
        //    {
        //        action = await NavPage.DisplayActionSheet("+", "Cancel", null, actionAddBreak, actionAddMatch, actionGameHost, actionUpdateLatestMatch);
        //    }
        //    else
        //    {
        //        action = await NavPage.DisplayActionSheet("+", "Cancel", null, actionAddBreak, actionAddMatch, actionGameHost);
        //    }

        //    if (action == actionAddBreak)
        //    {
        //        //await NavPage.Navigation.PushModalAsync(new NewSnookerBreakPage(null));
        //    }
        //    else if (action == actionAddMatch)
        //    {
        //        await NavPage.Navigation.PushModalAsync(new NewSnookerMatchPage());
        //    }
        //    else if (action == actionGameHost)
        //    {
        //        await NavPage.Navigation.PushModalAsync(new NewGameHostPage());
        //    }
        //    else if (action == actionUpdateLatestMatch)
        //    {
        //        await NavPage.Navigation.PushModalAsync(new NewSnookerMatchPage(unfinishedMatch));
        //    }
        //}

        public void OpenBrowserApp(string url)
        {
            Device.OpenUri(new Uri(url));
        }

        public void OpenPhoneCallApp(string phone)
        {
            string url = "tel:" + phone;
            Device.OpenUri(new Uri(url));
        }

        public void OpenMapsApp(Location location, string name, string address)
        {
            string latitude = location.Latitude.ToString();
            string longitude = location.Longitude.ToString();

            string url = string.Format("http://maps.apple.com/?daddr={0},{1}&saddr={2}", latitude, longitude, Uri.EscapeUriString("Current Location"));
            if (string.IsNullOrEmpty(address) == false)
                url = string.Format("http://maps.apple.com/?daddr={0}&saddr={1}", address, Uri.EscapeUriString("Current Location"));

            Device.OpenUri(new Uri(url));

//#if __IOS__
//            var request = string.Format("http://maps.apple.com/maps?q={2}@{0},{1}", latitude, longitude, Uri.EscapeUriString(name));
//            Device.OpenUri(new Uri(request));
//            //UIKit.UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(request));
//#elif __ANDROID__
//            var request = string.Format("geo:0,0?q={0},{1}({2})", latitude, longitude, Uri.EscapeUriString(name));
//            Device.OpenUri(new Uri(request));
//#else
//            throw new Exception("No device type compile-time directive found");
//#endif
        }

        DateTime lastTimeNotifiedAboutTheUpdate = new DateTime(2000, 1, 1);
        DateTime lastTimeSentDeviceInfo = new DateTime(2000, 1, 1);

        public void CheckApiVersionAndNotifyIfNeeded()
        {
			if (App.LoginAndRegistrationLogic.RegistrationStatus != RegistrationStatusEnum.Registered)
				return;
			
            Task.Run(async () =>
            {
                BybApiAboutWebModel aboutApi = await App.WebService.About();
                if (aboutApi == null)
                    return; // no internet access

                // show "upgrade required" if necessary
                int currentVersionNumber = SnookerBybMobileVersions.Current.Number;
                if (currentVersionNumber <= aboutApi.AppVersionUpgradeRequired)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UpgradeRequiredPage dlg = new UpgradeRequiredPage();
                        NavPage.Navigation.PushModalAsync(dlg);
                    });
                }
                else if (currentVersionNumber <= aboutApi.AppVersionUpgradeRecommended)
                {
                    if ((DateTime.Now - lastTimeNotifiedAboutTheUpdate).TotalDays > 1)
                    {
                        lastTimeNotifiedAboutTheUpdate = DateTime.Now;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Navigator.DisplayAlertRegular("A new version of the Snooker Byb has been released. We recommend updating to the newest version.");
                        });
                    }
                }

				// send "device info" to the server
				if ((DateTime.Now - lastTimeSentDeviceInfo).TotalHours > 1)
				{
					if (await App.WebService.SendDeviceInfo() == true)
						lastTimeSentDeviceInfo = DateTime.Now;
				}
            });
        }

        public void UpdateNavigationMenuButtons(bool? makeVisible = null)
        {
            if (makeVisible == false)
            {
                this.toolBarMenuItem2.Icon = null;
                return;
            }

            if (notificationsService == null || notificationsService.Data == null)
                return;

            if (notificationsService.Data.NotificationsCount <= 0)
            {
				this.NavPage.BarTextColor = Config.ColorPageTitleBarTextNormal;
                this.toolBarMenuItem2.Icon = new FileImageSource() { File = "alert1.png" };
            }
            else
            {
                this.NavPage.BarTextColor = Config.ColorPageTitleBarTextAlert;
                this.toolBarMenuItem2.Icon = new FileImageSource() { File = "alert2.png" };
            }
        }

        public void SetRootPageTitleToLoading()
        {
            this.rootPage.Title = "... loading ...";
        }

        public void SetRootPageTitleToNormal()
        {
            this.rootPage.UpdateTheTitle();
        }
    }
}
