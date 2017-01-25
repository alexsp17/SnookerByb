using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public enum ProfilePersonStateEnum
    {
        Unknown = 0,
        Breaks,
        Matches,
        Opponents,
        Venues
    }

    public class ProfilePersonControl : Grid
    {
		public event Action LoadStarted;
		public event Action<bool> LoadCompleted;
		
        public int AthleteID { get; private set; }
        public bool IsMyAthlete { get; private set; }

		public string AthleteName
		{
			get
			{
				if (this.fullPlayerData == null || this.fullPlayerData.Person == null)
					return null;
				return this.fullPlayerData.Person.Name;
			}
		}

        FullSnookerPlayerData fullPlayerData;

        ProfilePersonInfoControl infoControl;

        Grid panelMe;
        Grid panelNotMe;

        Button buttonSendFriendRequest;
        Label labelFriendship;
        Button buttonInvite;
        Button buttonMessage;
        Button buttonSync;

        ListOfSnookerBreaksControl listOfBreaksControl;
        ListOfSnookerMatchesControl listOfMatchesControl;
        ListOfOpponentsControl listOfOpponents;
        ListOfVenuesPlayedControl listOfVenues;

        BybButtonWithNumber buttonBreaks;
        BybButtonWithNumber buttonMatches;
        BybButtonWithNumber buttonOpponents;
        BybButtonWithNumber buttonVenues;

        public ProfilePersonStateEnum State
        {
            get
            {
                if (this.listOfMatchesControl.IsVisible)
                    return ProfilePersonStateEnum.Matches;
                if (this.listOfOpponents.IsVisible)
                    return ProfilePersonStateEnum.Opponents;
                if (this.listOfVenues.IsVisible)
                    return ProfilePersonStateEnum.Venues;
                return ProfilePersonStateEnum.Breaks;
            }
            set
            {
                if (value == ProfilePersonStateEnum.Breaks)
                {
                    this.listOfMatchesControl.IsVisible = false;
                    this.listOfOpponents.IsVisible = false;
                    this.listOfVenues.IsVisible = false;
                    this.listOfBreaksControl.IsVisible = true;
                }
                else if (value == ProfilePersonStateEnum.Matches)
                {
                    this.listOfBreaksControl.IsVisible = false;
                    this.listOfOpponents.IsVisible = false;
                    this.listOfVenues.IsVisible = false;
                    this.listOfMatchesControl.IsVisible = true;
                }
                else if (value == ProfilePersonStateEnum.Opponents)
                {
                    this.listOfBreaksControl.IsVisible = false;
                    this.listOfMatchesControl.IsVisible = false;
                    this.listOfVenues.IsVisible = false;
                    this.listOfOpponents.IsVisible = true;
                }
                else if (value == ProfilePersonStateEnum.Venues)
                {
                    this.listOfBreaksControl.IsVisible = false;
                    this.listOfMatchesControl.IsVisible = false;
                    this.listOfOpponents.IsVisible = false;
                    this.listOfVenues.IsVisible = true;
                }

                this.buttonBreaks.IsSelected = value == ProfilePersonStateEnum.Breaks;
                this.buttonMatches.IsSelected = value == ProfilePersonStateEnum.Matches;
                this.buttonOpponents.IsSelected = value == ProfilePersonStateEnum.Opponents;
                this.buttonVenues.IsSelected = value == ProfilePersonStateEnum.Venues;

                this.fillVisibleControlsWithExistingData();
            }
        }

		public void FillAsync(Athlete athlete, bool onlyIfItsBeenAwhile)
		{
			this.IsMyAthlete = true;
			this.AthleteID = athlete.AthleteID;

			this.panelMe.IsVisible = true;
			this.panelNotMe.IsVisible = false;

			this.infoControl.SetImage(athlete.Picture);

			this.loadDataAsyncAndFill(onlyIfItsBeenAwhile);
		}

		public void FillAsync(int personID, bool onlyIfItsBeenAwhile)
		{
			var myAthlete = App.Repository.GetMyAthlete();
			if (personID == myAthlete.AthleteID)
			{
				this.FillAsync(myAthlete, onlyIfItsBeenAwhile);
				return;
			}

			this.IsMyAthlete = false;
			this.AthleteID = personID;

			this.panelMe.IsVisible = false;
			this.panelNotMe.IsVisible = true;

			this.loadDataAsyncAndFill(onlyIfItsBeenAwhile);
		}

		public void RefillAsync(bool onlyIfItsBeenAwhile)
		{
			this.loadDataAsyncAndFill(onlyIfItsBeenAwhile);
		}

        public ProfilePersonControl()
        {
			this.Padding = new Thickness(0);
            this.BackgroundColor = Config.ColorBackground;
            this.ColumnSpacing = 0;
            this.RowSpacing = 0;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;

            /// info panel
            /// 
            this.infoControl = new ProfilePersonInfoControl() { Padding = new Thickness(0, 0, 0, 15), HorizontalOptions = LayoutOptions.FillAndExpand };
            this.infoControl.ClickedOnBestBreak += (s, e) =>
            {
                this.State = ProfilePersonStateEnum.Breaks;
                this.listOfBreaksControl.Sort(SnookerBreakSortEnum.ByPoints);
            };
            this.infoControl.ClickedOnBestFrame += (s, e) =>
            {
                this.State = ProfilePersonStateEnum.Matches;
                this.listOfMatchesControl.Sort(SnookerMatchSortEnum.ByBestFrame);
            };
            this.infoControl.ClickedOnContributions += (s, e) =>
            {
                App.Navigator.DisplayAlertRegular("Verifying snooker venues is an example of a contribution.");
            };
            this.infoControl.ClickedOnAbout += (s, e) =>
            {
                if (IsMyAthlete == true)
                    App.Navigator.OpenProfileEditPage(true, true);
            };

            /// panel "Me"
            /// 
            Button buttonEditProfile = new BybButton() { Text = "Edit profile", Style = (Style)App.Current.Resources["BlackButtonStyle"], FontFamily = Config.FontFamily, TextColor = Config.ColorTextOnBackground };
            buttonEditProfile.Clicked += buttonEditProfile_Clicked;
            this.buttonSync = new BybButton() { Text = "Sync now", Style = (Style)App.Current.Resources["BlackButtonStyle"], FontFamily = Config.FontFamily, TextColor = Config.ColorTextOnBackground };
            this.buttonSync.Clicked += buttonSync_Clicked;
            this.panelMe = new Grid()
            {
                Padding = new Thickness(10, 0, 10, 10),
				ColumnSpacing = Config.SpaceBetweenButtons,
                RowSpacing = 0,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto)},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                }
            };
            this.panelMe.Children.Add(buttonSync, 0, 0);
            this.panelMe.Children.Add(buttonEditProfile, 1, 0);

            /// panel "Not me"
            /// 
            this.panelNotMe = new Grid()
            {
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(0, 0, 0, 10),
                ColumnSpacing = 0,
                RowSpacing = 0,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto)},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition { Width = new GridLength(10, GridUnitType.Absolute)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(10, GridUnitType.Absolute)},
                }
            };
            this.buttonSendFriendRequest = new BybButton() { Text = "Friend", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            this.buttonSendFriendRequest.Clicked += buttonSendFriendRequest_Clicked;
            this.buttonInvite = new BybButton() { Text = "Invite", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            this.buttonInvite.Clicked += buttonInvite_Clicked;
            this.buttonMessage = new BybButton() { Text = "Message", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            this.buttonMessage.Clicked += buttonMessage_Clicked;
            this.labelFriendship = new BybLabel() { Text = "-", TextColor = Config.ColorTextOnBackgroundGrayed, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
            this.labelFriendship.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    this.labelFriendship_Tapped();
                }),
                NumberOfTapsRequired = 1
            });
            this.panelNotMe.Children.Add(this.buttonMessage, 1, 0);
            this.panelNotMe.Children.Add(this.buttonInvite, 3, 0);
            this.panelNotMe.Children.Add(this.labelFriendship, 5, 0);
            this.panelNotMe.Children.Add(this.buttonSendFriendRequest, 5, 0);

            /// Tabs: Breaks / matches / opponents
            /// 

            this.buttonBreaks = new BybButtonWithNumber("Breaks") { HeightRequest = 55 };
            buttonBreaks.Clicked += (s, e) => { this.State = ProfilePersonStateEnum.Breaks; };
            this.buttonMatches = new BybButtonWithNumber("Matches") { HeightRequest = 55 };
            buttonMatches.Clicked += (s, e) => { this.State = ProfilePersonStateEnum.Matches; };
            this.buttonOpponents = new BybButtonWithNumber("Opponents") { HeightRequest = 55 };
            buttonOpponents.Clicked += (s, e) => { this.State = ProfilePersonStateEnum.Opponents; };
            this.buttonVenues = new BybButtonWithNumber("Venues") { HeightRequest = 55 };
            buttonVenues.Clicked += (s, e) => { this.State = ProfilePersonStateEnum.Venues; };

            Grid gridWithButtons = new Grid()
            {
                BackgroundColor = Config.ColorBackgroundWhite,//Config.ColorGrayBackground,
                Padding = new Thickness(0, 0, 0, 0),
                ColumnSpacing = 0,
                RowSpacing = 0,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height = new GridLength(55, GridUnitType.Auto)},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                }
            };
            gridWithButtons.Children.Add(buttonBreaks, 0, 0);
            gridWithButtons.Children.Add(buttonMatches, 1, 0);
            gridWithButtons.Children.Add(buttonOpponents, 2, 0);
            gridWithButtons.Children.Add(buttonVenues, 3, 0);

            StackLayout panelContent = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Config.ColorGrayBackground,
            };

            // breaks
            this.listOfBreaksControl = new ListOfSnookerBreaksControl()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsVisible = true
            };
            this.listOfBreaksControl.UserWantsToEditRes += ctrl_UserWantsToEditBreak;
            this.listOfBreaksControl.UserWantsToDeleteRes += ctrl_UserWantsToDeleteBreak;
            panelContent.Children.Add(this.listOfBreaksControl);

            // matches
            this.listOfMatchesControl = new ListOfSnookerMatchesControl()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsVisible = false
            };
            this.listOfMatchesControl.UserWantsToDeleteScore += ctrl_UserWantsToDeleteScore;
            this.listOfMatchesControl.UserWantsToEditScore += ctrl_UserWantsToEditScore;
            this.listOfMatchesControl.UserWantsToViewScore += ctrl_UserWantsToViewScore;
            panelContent.Children.Add(this.listOfMatchesControl);

            // friends
            this.listOfOpponents = new ListOfOpponentsControl()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsVisible = false
            };
            panelContent.Children.Add(this.listOfOpponents);

            // venues
            this.listOfVenues = new ListOfVenuesPlayedControl()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsVisible = false
            };
            panelContent.Children.Add(this.listOfVenues);


            /// Grid
            /// 
            this.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
            };
            this.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
            };
            this.Children.Add(this.infoControl, 0, 0);
            this.Children.Add(this.panelMe, 0, 1);
            this.Children.Add(this.panelNotMe, 0, 1);
            this.Children.Add(gridWithButtons, 0, 2);
            this.Children.Add(panelContent, 0, 3);

            this.State = ProfilePersonStateEnum.Breaks;
        }

        void buttonEditProfile_Clicked(object sender, EventArgs e)
        {
            if (IsMyAthlete == false)
                return;

            App.Navigator.OpenProfileEditPage(true, true);
        }

        void buttonSync_Clicked(object sender, EventArgs e)
        {
            App.Navigator.StartSyncAndCheckForNotifications();
        }

        async void buttonSendFriendRequest_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.CurrentPage.IsBusy = true;
            bool ok = await App.WebService.RequestFriend(this.AthleteID);
            App.Navigator.NavPage.CurrentPage.IsBusy = false;

            if (ok == false)
            {
                App.Navigator.DisplayAlertError("Failed to send a friend request. Internet issues?");
                return;
            }

            this.buttonSendFriendRequest.IsVisible = false;
            this.labelFriendship.IsVisible = true;
            this.labelFriendship.Text = "Friend request sent";
        }

		DateTime timeLastLoad = DateTime.MinValue;

		void loadDataAsyncAndFill(bool onlyIfItsBeenAwhile)
        {
            if (this.AthleteID < -1)
                return;

			if (onlyIfItsBeenAwhile & (DateTime.Now - timeLastLoad).TotalMinutes < 2)
				return;

            this.infoControl.FillWaiting();
            this.buttonBreaks.Number = null;
            this.buttonMatches.Number = null;
            this.buttonOpponents.Number = null;
            this.buttonVenues.Number = null;

			if (this.LoadStarted != null)
				this.LoadStarted ();

			new Task(async () =>
			{
				timeLastLoad = DateTime.Now;
            	this.fullPlayerData = await new FullSnookerPlayerDataHelper().Load(AthleteID);

				Device.BeginInvokeOnMainThread(() =>
				{
					this.fillOther();
					this.fillButtons ();
					this.infoControl.Fill(fullPlayerData, IsMyAthlete);
					this.fillVisibleControlsWithExistingData();

					if (this.LoadCompleted != null)
						this.LoadCompleted(this.fullPlayerData != null && this.fullPlayerData.InternetIssues == false);
				});
			}).Start();
        }

		void fillButtons()
		{
			if (fullPlayerData == null)
				return;
			
			if (fullPlayerData.Breaks != null)
				this.buttonBreaks.Number = fullPlayerData.Breaks.Count;
			if (fullPlayerData.Matches != null)
				this.buttonMatches.Number = fullPlayerData.Matches.Count;
			if (fullPlayerData.Opponents != null)
				this.buttonOpponents.Number = fullPlayerData.Opponents.Count;
			if (fullPlayerData.VenuesPlayed != null)
				this.buttonVenues.Number = fullPlayerData.VenuesPlayed.Count;
		}

		void fillOther()
		{
			if (this.fullPlayerData == null || this.fullPlayerData.Person == null)
				return;

			var person = this.fullPlayerData.Person;

			this.labelFriendship.IsVisible = person.IsFriend || person.IsFriendRequestSent == true;
			this.buttonSendFriendRequest.IsVisible = person.IsFriend == false && person.IsFriendRequestSent == false;
			if (person.IsFriend == true)
			{
				this.labelFriendship.TextColor = Config.ColorTextOnBackgroundGrayed;
				this.labelFriendship.Text = "your friend";
			}
			else if (person.IsFriendRequestSentByThisPerson == true)
			{
				this.labelFriendship.TextColor = Color.Red;
				this.labelFriendship.Text = "Accept as friend?";
			}
			else
			{
				this.labelFriendship.TextColor = Config.ColorTextOnBackgroundGrayed;
				this.labelFriendship.Text = "friend request sent";
			}

			this.infoControl.SetImage(person.Picture);
		}

        void fillVisibleControlsWithExistingData()
        {
            if (fullPlayerData == null)
                return;
			
			if (this.listOfBreaksControl.IsVisible && this.listOfBreaksControl.AllBreaks != fullPlayerData.Breaks)
			{
				this.listOfBreaksControl.Type = this.IsMyAthlete ? ListTypeEnum.PrimaryAthlete : ListTypeEnum.NotPrimaryAthlete;// .IsForPrimaryAthlete = this.IsMyAthlete;
				this.listOfBreaksControl.Fill (fullPlayerData.Breaks);
			}
			if (this.listOfMatchesControl.IsVisible && this.listOfMatchesControl.AllMatches != fullPlayerData.Matches)
			{
				this.listOfMatchesControl.Type = IsMyAthlete ? ListTypeEnum.PrimaryAthlete : ListTypeEnum.NotPrimaryAthlete;//.IsForPrimaryAthlete = this.IsMyAthlete;
				this.listOfMatchesControl.Fill(fullPlayerData.Matches);
			}
			if (this.listOfOpponents.IsVisible && this.listOfOpponents.AllOpponents != fullPlayerData.Opponents)
                this.listOfOpponents.Fill(fullPlayerData.Opponents);
			if (this.listOfVenues.IsVisible && this.listOfVenues.AllVenues != fullPlayerData.VenuesPlayed)
                this.listOfVenues.Fill(fullPlayerData.VenuesPlayed);
        }

        void ctrl_UserWantsToEditBreak(object sender, SnookerEventArgs e)
        {
            if (this.IsMyAthlete == false)
                return;

            SnookerBreak snookerBreak = e.SnookerBreak;
            if (snookerBreak.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
            {
                App.Navigator.DisplayAlertRegular("Cannot edit a confirmed break.");
                return;
            }

            var page = new RecordBreakPage(snookerBreak, false, true);
            App.Navigator.NavPage.Navigation.PushModalAsync(page);
            page.Done += async (s1, e1) =>
            {
                snookerBreak = e1;
                if (snookerBreak == null)
                    return;

                // update
                Result result = App.Repository.GetResult(snookerBreak.ID);
                result.TimeModified = DateTimeHelper.GetUtcNow();
                result.OpponentConfirmation = (int)OpponentConfirmationEnum.NotYet;
                snookerBreak.PostToResult(result);
                App.Repository.UpdateResult(result);

                await App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Breaks, true);
                App.Navigator.StartSyncAndCheckForNotifications();
            };
        }

        async void ctrl_UserWantsToDeleteBreak(object sender, SnookerEventArgs e)
        {
            if (this.IsMyAthlete == false)
                return;

            SnookerBreak snookerBreak = e.SnookerBreak;

            bool ok = await App.Navigator.NavPage.DisplayAlert("Byb", "Delete break " + snookerBreak.ToString() + " ?", "Yes, delete", "Cancel");
            if (ok == false)
                return;

            App.Repository.SetIsDeletedOnResult(snookerBreak.ID, true);
            App.Navigator.StartSyncAndCheckForNotifications();

			this.loadDataAsyncAndFill(false);
        }

        void ctrl_UserWantsToViewScore(object sender, SnookerEventArgs e)
        {
            var me = App.Repository.GetMyAthlete();

            SnookerMatchScore match = e.MatchScore;
            match.YourName = "Unknown";
            if (match.YourAthleteID == me.AthleteID)
            {
                match.YourName = me.Name;
                match.YourPicture = me.Picture;
            }
            else
            {
                var person = App.Cache.People.Get(match.YourAthleteID);
                if (person != null)
                {
                    match.YourName = person.Name;
                    match.YourPicture = person.Picture;
                }
            }
			if (match.OpponentAthleteID > 0) 
			{
				var person = App.Cache.People.Get (match.OpponentAthleteID);
				if (person != null)
					match.OpponentPicture = person.Picture;
			}

			var page = new RecordMatchPage (match, true);// NewMatch2Page(match, true);
            App.Navigator.NavPage.Navigation.PushModalAsync(page);
        }

        void ctrl_UserWantsToEditScore(object sender, SnookerEventArgs e)
        {
            var me = App.Repository.GetMyAthlete();

            SnookerMatchScore match = e.MatchScore;
            match.YourName = "Unknown";
            if (match.YourAthleteID != me.AthleteID)
            {
                App.Navigator.DisplayAlertRegular("Cannot edit someone else's match.");
                return;
            }
            if (match.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
            {
                App.Navigator.DisplayAlertRegular("Cannot edit a confirmed match.");
                return;
            }
            match.YourName = me.Name;
            match.YourPicture = me.Picture;
			if (match.OpponentAthleteID > 0) 
			{
				var person = App.Cache.People.Get (match.OpponentAthleteID);
				if (person != null)
					match.OpponentPicture = person.Picture;
			}

			var page = new RecordMatchPage (match, false);// NewMatch2Page(match, false);
            App.Navigator.NavPage.Navigation.PushModalAsync(page);
        }

        async void ctrl_UserWantsToDeleteScore(object sender, SnookerEventArgs e)
        {
            if (this.IsMyAthlete == false)
                return;

            SnookerMatchScore match = e.MatchScore;

            bool ok = await App.Navigator.NavPage.DisplayAlert("Byb", "Delete match " + match.ToString() + " ?", "Yes, delete", "Cancel");
            if (ok == false)
                return;

            App.Repository.SetIsDeletedOnScore(match.ID, true);
            App.Navigator.StartSyncAndCheckForNotifications();

			this.loadDataAsyncAndFill(false);
        }

        async private void buttonMessage_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage (typeof(SendMessagePage)) != null)
				return;
			if (this.fullPlayerData == null || this.fullPlayerData.Person == null)
				return;
			
			SendMessagePage page = new SendMessagePage(this.fullPlayerData.Person);
            await App.Navigator.NavPage.Navigation.PushAsync(page);
        }

        async void buttonInvite_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage(typeof(NewGameHostPage)) != null)
				return;
			if (this.fullPlayerData == null || this.fullPlayerData.Person == null)
				return;
			
            NewGameHostPage page = new NewGameHostPage();
			page.AddPerson(this.fullPlayerData.Person);
            await App.Navigator.NavPage.Navigation.PushModalAsync(page);
        }

        async void labelFriendship_Tapped()
        {
			if (this.fullPlayerData == null || this.fullPlayerData.Person == null)
				return;
			var person = this.fullPlayerData.Person;
			
			if (person.IsFriendRequestSentByThisPerson == true)
            {
                // accept friendship

				if (await App.WebService.AcceptFriendRequest2(person.ID))
                {
                    await App.Navigator.NavPage.PopAsync();
					await App.Navigator.GoToPersonProfile(person.ID);
                }
                else
                {
                    App.Navigator.DisplayAlertError("Couldn't unfriend. Internet issues?");
                }
            }
            else
            {
                // unfriend?

                string strUnfriend = "Unfriend";

                string answer = await App.Navigator.NavPage.DisplayActionSheet("Your friend", null, null, strUnfriend, "Cancel");

                if (answer == strUnfriend)
                {
                    if (await App.Navigator.NavPage.DisplayAlert("Byb", "Unfriend a person", "Unfriend " + person.Name, "Cancel") == true)
                    {
                        if (await App.WebService.Unfriend(person.ID))
                        {
                            await App.Navigator.NavPage.PopAsync();
                            await App.Navigator.GoToPersonProfile(person.ID);
                        }
                        else
                        {
                            App.Navigator.DisplayAlertError("Couldn't unfriend. Internet issues?");
                        }
                    }
                }
            }
        }
    }
}
