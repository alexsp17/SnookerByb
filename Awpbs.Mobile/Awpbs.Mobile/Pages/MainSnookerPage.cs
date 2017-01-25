using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    /// <summary>
    /// Can show either "record", "profile", "community", or "events"
    /// </summary>
	public class MainSnookerPage : ContentPage
	{
        public enum StateEnum
        {
            Record,
            Person,
            Venue,
            Community,
            Events,
        }

        public StateEnum State
        {
            get
            {
                if (rootForRecord != null && rootForRecord.IsVisible)
                    return StateEnum.Record;
                if (rootForEvents != null && rootForEvents.IsVisible)
                    return StateEnum.Events;
                if (rootForCommunity != null && rootForCommunity.IsVisible)
                    return StateEnum.Community;
                if (profileVenueControl != null && profileVenueControl.IsVisible)
                    return StateEnum.Venue;
                return StateEnum.Person;
            }
        }

        public int? AthleteID
        {
            get
            {
                if (this.profilePersonControl != null)
                    return this.profilePersonControl.AthleteID;
                return null;
            }
        }

		public int? VenueID
		{
			get
			{
				if (this.profileVenueControl != null)
					return this.profileVenueControl.VenueID;
				return null;
			}
		}

        Grid grid;

        View rootForRecord;
        View rootForProfilePerson;
        View rootForProfileVenue;
        View rootForCommunity;
        View rootForEvents;

        RecordControl recordControl;
        ProfilePersonControl profilePersonControl;
        ProfileVenueControl profileVenueControl;
        CommunityControl communityControl;
        EventsControl eventsControl;

		BybButtonWithImage buttonRecord;
		BybButtonWithImage buttonProfile;
		BybButtonWithImage buttonCommunity;
		BybButtonWithImage buttonEvents;

        public MainSnookerPage()
		{
            this.init();

            App.NotificationsService.CheckForNotificationsIfNecessary(true);
        }

        public void UpdateTheTitle()
        {
			string title = "";
			
			if (State == StateEnum.Record)
				title = "Record Breaks & Matches";
			else if (State == StateEnum.Events)
				title = "Events";
			else if (State == StateEnum.Community)
				title = "Community";
			else if (State == StateEnum.Venue)
			{
				if (this.profileVenueControl != null && profileVenueControl.FullVenueData != null && profileVenueControl.FullVenueData.Venue != null)
					title = profileVenueControl.FullVenueData.Venue.Name ?? "-";
			}
			else if (State == StateEnum.Person)
					title = (this.profilePersonControl != null) ? (this.profilePersonControl.AthleteName ?? "") : "";

			this.Title = title;
        }

        public void InitAsRecord()
        {
			TraceHelper.TraceInfoForResponsiveness("InitAsRecord Begin +++++++++++");
			
            if (this.recordControl == null)
            {
                this.recordControl = new RecordControl();
                this.rootForRecord = this.recordControl;
                grid.Children.Add(this.rootForRecord);
            }
            this.rootForRecord.IsVisible = true;
            this.recordControl.DoOnOpen();

            if (this.rootForProfileVenue != null)
                this.rootForProfileVenue.IsVisible = false;
            if (this.rootForProfilePerson != null)
                this.rootForProfilePerson.IsVisible = false;
            if (this.rootForCommunity != null)
                this.rootForCommunity.IsVisible = false;
            if (this.rootForEvents != null)
                this.rootForEvents.IsVisible = false;

            this.updateButtons();
            this.UpdateTheTitle();

			TraceHelper.TraceInfoForResponsiveness("InitAsRecord End +++++++++++");
        }

        public void InitAsEvents()
        {
			TraceHelper.TraceInfoForResponsiveness("InitAsEvents Begin +++++++++++");
			
            if (this.eventsControl == null)
            {
                this.eventsControl = new EventsControl();
                this.rootForEvents = this.eventsControl;
                grid.Children.Add(rootForEvents);
            }
            this.rootForEvents.IsVisible = true;
			this.eventsControl.ReloadAsync(true);

            if (this.rootForRecord != null)
                this.rootForRecord.IsVisible = false;
            if (this.rootForProfileVenue != null)
                this.rootForProfileVenue.IsVisible = false;
            if (this.rootForProfilePerson != null)
                this.rootForProfilePerson.IsVisible = false;
            if (this.rootForCommunity != null)
                this.rootForCommunity.IsVisible = false;

            //this.panelWait.IsVisible = false;
            //this.panelError.IsVisible = false;

            this.updateButtons();

			TraceHelper.TraceInfoForResponsiveness("InitAsEvents End +++++++++++");
        }

		public void InitAsMyProfile(ProfilePersonStateEnum state, bool forceReload = false)
        {
			TraceHelper.TraceInfoForResponsiveness("InitAsMyProfile Begin +++++++++++");
			
            Athlete athlete = App.Repository.GetMyAthlete();

            if (this.profilePersonControl == null)
            {
                this.profilePersonControl = new ProfilePersonControl();
				this.profilePersonControl.LoadStarted += () => { this.Title = "... loading ..."; };
				this.profilePersonControl.LoadCompleted += (ok) => { this.UpdateTheTitle(); };
                this.rootForProfilePerson = new ScrollView()
                {
                    Padding = new Thickness(0),
                    Content = this.profilePersonControl
                };
                grid.Children.Add(rootForProfilePerson);
            }
            if (state != ProfilePersonStateEnum.Unknown)
                this.profilePersonControl.State = state;

			this.profilePersonControl.FillAsync(athlete, !forceReload);
            this.rootForProfilePerson.IsVisible = true;

            if (this.rootForRecord != null)
                this.rootForRecord.IsVisible = false;
            if (this.rootForProfileVenue != null)
                this.rootForProfileVenue.IsVisible = false;
            if (this.rootForCommunity != null)
                this.rootForCommunity.IsVisible = false;
            if (this.rootForEvents != null)
                this.rootForEvents.IsVisible = false;

            this.updateButtons();

			TraceHelper.TraceInfoForResponsiveness("InitAsMyProfile End +++++++++++");
        }

        public void InitAsPersonProfile(int athleteID, ProfilePersonStateEnum state = ProfilePersonStateEnum.Unknown)
        {
			TraceHelper.TraceInfoForResponsiveness("InitAsPersonProfile Begin +++++++++++");

            if (this.profilePersonControl == null)
            {
                this.profilePersonControl = new ProfilePersonControl();
				this.profilePersonControl.LoadStarted += () => { this.Title = "... loading ..."; };
				this.profilePersonControl.LoadCompleted += (ok) => { this.UpdateTheTitle(); };
                this.rootForProfilePerson = new ScrollView()
                {
                    Padding = new Thickness(0),
                    Content = this.profilePersonControl
                };
                grid.Children.Add(this.rootForProfilePerson);
            }
            if (state != ProfilePersonStateEnum.Unknown)
                this.profilePersonControl.State = state;

			this.Title = "... loading ...";
			this.profilePersonControl.FillAsync(athleteID, true);
            this.rootForProfilePerson.IsVisible = true;

            if (this.rootForRecord != null)
                this.rootForRecord.IsVisible = false;
            if (this.rootForProfileVenue != null)
                this.rootForProfileVenue.IsVisible = false;
            if (this.rootForCommunity != null)
                this.rootForCommunity.IsVisible = false;
            if (this.rootForEvents != null)
                this.rootForEvents.IsVisible = false;

            this.updateButtons();

			TraceHelper.TraceInfoForResponsiveness("InitAsPersonProfile End +++++++++++");
        }

        public void InitAsVenueProfile(int venueID)
        {
			TraceHelper.TraceInfoForResponsiveness("InitAsVenueProfile Begin +++++++++++");
            
            if (this.profileVenueControl == null)
            {
                this.profileVenueControl = new ProfileVenueControl();
				this.profileVenueControl.LoadStarted += () => { this.Title = "... loading ..."; };
				this.profileVenueControl.LoadCompleted += (ok) => { this.UpdateTheTitle(); };
                this.rootForProfileVenue = new ScrollView()
                {
                    Padding = new Thickness(0),
                    Content = this.profileVenueControl
                };
                grid.Children.Add(this.rootForProfileVenue);
            }

			this.Title = "... loading ...";
			this.profileVenueControl.FillAsync(venueID, true);

            if (this.rootForRecord != null)
                this.rootForRecord.IsVisible = false;
            if (this.rootForProfilePerson != null)
                this.rootForProfilePerson.IsVisible = false;
            if (this.rootForCommunity != null)
                this.rootForCommunity.IsVisible = false;
            if (this.rootForEvents != null)
                this.rootForEvents.IsVisible = false;

            this.updateButtons();

			TraceHelper.TraceInfoForResponsiveness("InitAsVenueProfile End +++++++++++");
        }

        public void InitAsCommunity()
        {
			TraceHelper.TraceInfoForResponsiveness("InitAsCommunity Begin ++++++++++");
			
            if (this.rootForCommunity == null)
            {
				TraceHelper.TraceInfoForResponsiveness("InitAsCommunity 0.1");
                this.communityControl = new CommunityControl();
				TraceHelper.TraceInfoForResponsiveness("InitAsCommunity 0.2");
                this.rootForCommunity = this.communityControl;
				TraceHelper.TraceInfoForResponsiveness("InitAsCommunity 0.3");
                grid.Children.Add(this.rootForCommunity);
				TraceHelper.TraceInfoForResponsiveness("InitAsCommunity 0.4");
            }
            this.rootForCommunity.IsVisible = true;
			TraceHelper.TraceInfoForResponsiveness("InitAsCommunity 1");

            if (this.rootForRecord != null)
                this.rootForRecord.IsVisible = false;
            if (this.rootForProfileVenue != null)
                this.rootForProfileVenue.IsVisible = false;
            if (this.rootForProfilePerson != null)
                this.rootForProfilePerson.IsVisible = false;
            if (this.rootForEvents != null)
                this.rootForEvents.IsVisible = false;

			TraceHelper.TraceInfoForResponsiveness("InitAsCommunity 2");

            this.updateButtons();
            this.UpdateTheTitle();

			TraceHelper.TraceInfoForResponsiveness("InitAsCommunity 3");

			this.communityControl.DoOnOpen();

			TraceHelper.TraceInfoForResponsiveness("InitAsCommunity End +++++++++++");
        }

        public void DoOnVenueEdited(int venueID)
        {
            if (this.State == StateEnum.Venue && this.profileVenueControl != null)
				this.profileVenueControl.FillAsync(this.profileVenueControl.VenueID, false);
            else if (this.State == StateEnum.Person && this.profilePersonControl != null)
				this.profilePersonControl.RefillAsync(false);
            else if (this.State == StateEnum.Community && this.communityControl != null)
                this.communityControl.DoOnVenueEdited(venueID);
        }

        void updateButtons()
        {
			this.buttonRecord.IsSelected = this.State == StateEnum.Record;
			this.buttonProfile.IsSelected = this.State == StateEnum.Person;
			this.buttonCommunity.IsSelected = this.State == StateEnum.Community;
			this.buttonEvents.IsSelected = this.State == StateEnum.Events;
        }

        private void init()
        {
            this.BackgroundColor = Config.ColorBackground;

			int buttonsHeight = Config.LargeButtonsHeight + (Config.IsTablet ? 20 : 3);

            // the top level grid
            this.grid = new Grid()
            {
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition{ Height = new GridLength(buttonsHeight, GridUnitType.Absolute)}
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                }
            };

            /// navigation buttons
            /// 

            Grid gridForButtons = new Grid()
            {
                BackgroundColor = Config.ColorBlackBackground,//Config.ColorBackground,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0,3,0,0),
				ColumnSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                }
            };
            grid.Children.Add(gridForButtons, 0, 1);

			// record
			this.buttonRecord = new BybButtonWithImage ("add.png", "Record") { HeightRequest = buttonsHeight };
			this.buttonRecord.IsSelected = false;
			this.buttonRecord.Clicked += async () =>
			{
				await App.Navigator.GoToRecord();
			};
			gridForButtons.Children.Add (this.buttonRecord, 0, 0);

            // profile
			this.buttonProfile = new BybButtonWithImage ("profile.png", "Profile") { HeightRequest = buttonsHeight };
			this.buttonProfile.IsSelected = false;
			this.buttonProfile.Clicked += async () =>
			{
				await App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Unknown, false);
			};
			gridForButtons.Children.Add (this.buttonProfile, 1, 0);

            // community
			this.buttonCommunity = new BybButtonWithImage ("find.png", "Community") { HeightRequest = buttonsHeight };
			this.buttonCommunity.IsSelected = false;
			this.buttonCommunity.Clicked += async () =>
			{
				TraceHelper.TraceInfoForResponsiveness("community button tapped");
				await App.Navigator.GoToCommunity();
				TraceHelper.TraceInfoForResponsiveness("community button tap event completed");
			};
			gridForButtons.Children.Add (this.buttonCommunity, 2, 0);

            // events
			this.buttonEvents = new BybButtonWithImage ("calendar.png", "Events") { HeightRequest = buttonsHeight };
			this.buttonEvents.IsSelected = false;
			this.buttonEvents.Clicked += async () =>
			{
				TraceHelper.TraceInfoForResponsiveness("events button tapped");
				await App.Navigator.GoToEvents();
				TraceHelper.TraceInfoForResponsiveness("events button tap event completed");
			};
			gridForButtons.Children.Add (this.buttonEvents, 3, 0);

            Content = grid;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() =>
            {
                App.Navigator.UpdateNavigationMenuButtons(true);
            });
        }
	}
}
