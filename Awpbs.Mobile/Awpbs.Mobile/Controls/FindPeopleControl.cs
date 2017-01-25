using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class FindPeopleControl : ScrollView
    {
		public event Action LoadStarted;
		public event Action LoadCompleted;
        public event EventHandler<SelectedPersonEventArgs> UserClickedOnPerson;

        public CommunitySelection CurrentCommunity
        {
            get
            {
                return this.communitySelectorControl.Selection;
            }
        }
		private CommunitySelection communityLoaded;

        public bool NameAsCommunity
        {
            get { return this.communitySelectorControl.NameAsCommunity; }
            set { this.communitySelectorControl.NameAsCommunity = value; }
        }

        public bool AllowFriendsSelection
        {
            get { return this.communitySelectorControl.AllowFriendsSelection; }
            set { this.communitySelectorControl.AllowFriendsSelection = value; }
        }

        bool ignoreUIEvents = false;

        // filters
        CommunitySelectorControl communitySelectorControl;
        Button buttonClearFilters;
        Entry entrySearch;

        StackLayout panelStatus;
        Label labelStatus;
        Button buttonSearchWorldwide;
        ListOfPeopleControl listOfPeopleControl;

		public void ReloadAsync(CommunitySelection communitySelection, bool onlyIfItsBeenAwhile)
		{
			bool isCommunitySelectionChanged = true;
			if (this.communityLoaded != null && communityLoaded.Compare (communitySelection) == true)
				isCommunitySelectionChanged = false;
			
			if (isCommunitySelectionChanged)
				this.communitySelectorControl.Selection = communitySelection;

			if (isCommunitySelectionChanged || onlyIfItsBeenAwhile == false)
				this.reloadAsync ();
		}

        public FindPeopleControl(bool friendsByDefault)
        {
            this.Padding = new Thickness(0);
            this.BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackground : Config.ColorGrayBackground;

            StackLayout stack = new StackLayout();
            stack.Orientation = StackOrientation.Vertical;
            stack.Spacing = 0;
            this.Content = stack;

            var myAthlete = App.Repository.GetMyAthlete();

            /// Filters
            /// 

            // community
            this.communitySelectorControl = new CommunitySelectorControl() { NameAsCommunity = false, AllowFriendsSelection = Config.App != MobileAppEnum.SnookerForVenues };
            if (friendsByDefault)
                this.communitySelectorControl.Selection = CommunitySelection.CreateFriendsOnly();
            this.communitySelectorControl.SelectionChanged += communitySelectorControl_SelectionChanged;
            stack.Children.Add(new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(0, 5, 0, 5),
                HeightRequest = Config.LargeButtonsHeight,// 40,
                Spacing = 1,
                BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackground : Config.ColorGrayBackground,
                Children =
                {
                    communitySelectorControl,
                }
            });

            // search
            this.buttonClearFilters = new BybButton()
            {
                Text = "x",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                WidthRequest = 30,
                TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Color.White : Color.Black,
            };
            this.buttonClearFilters.Clicked += (s, e) =>
            {
                this.ignoreUIEvents = true;
                this.entrySearch.Text = "";
                this.ignoreUIEvents = false;
				this.reloadAsync();
            };
			this.entrySearch = new BybNoBorderEntry() 
            { 
                Placeholder = "Search by name", 
                HorizontalOptions = LayoutOptions.FillAndExpand, 
				VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackground : Color.White,
                TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Color.White : Color.Black,
            };
            this.entrySearch.Completed += entrySearch_Completed;
            stack.Children.Add(new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(Config.IsIOS ? 15 : 5, 0, 0, 0),
                HeightRequest = Config.LargeButtonsHeight,//40,
                Spacing = 1,
                BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackground : Color.White,
                Children =
                {
                    entrySearch,
                    buttonClearFilters,
                }
            });

            /// Status panel
            /// 
            this.labelStatus = new BybLabel()
            {
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackgroundGrayed : Color.Black,
            };
            this.buttonSearchWorldwide = new BybButton()
            {
                Text = "Search world-wide",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Color.White : Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = false,
            };
            this.buttonSearchWorldwide.Clicked += buttonSearchWorldwide_Clicked;
            this.panelStatus = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0,20,0,10),
                Children =
                {
                    this.labelStatus,
                    this.buttonSearchWorldwide,
                }
            };
            stack.Children.Add(this.panelStatus);

            /// List of people
            /// 
            this.listOfPeopleControl = new ListOfPeopleControl();
            this.listOfPeopleControl.IsDarkBackground = Config.App == MobileAppEnum.SnookerForVenues;
            if (Config.App == MobileAppEnum.SnookerForVenues)
                this.listOfPeopleControl.BackgroundColor = Config.ColorBackground;
            listOfPeopleControl.UserClickedOnPerson += (s1, e1) =>
            {
                if (this.UserClickedOnPerson != null)
                    this.UserClickedOnPerson(this, e1);
            };
            stack.Children.Add(new ScrollView()
            {
                Padding = new Thickness(0, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Content = this.listOfPeopleControl,
            });
        }

        void buttonSearchWorldwide_Clicked(object sender, EventArgs e)
        {
			this.reloadAsync (true);
        }

        void communitySelectorControl_SelectionChanged(object sender, EventArgs e)
        {
            if (this.ignoreUIEvents)
            {
                TraceHelper.TraceInfoForResponsiveness("FindPeopleControl.communitySelectorControl_SelectionChanged - ignoreUIEvents=true");
                return;
            }
			this.reloadAsync();
        }

        void entrySearch_Completed(object sender, EventArgs e)
        {
            if (this.ignoreUIEvents)
                return;
			this.reloadAsync ();
        }

        bool isSearchingNow = false;
        bool shouldDoAnotherSearch = false;
        int countSearchedSuccessfully = 0;

		class searchParams
		{
			public string NameQuery { get; set; }
			public Country Country { get; set; }
			public int? MetroID { get; set; }
			public bool FriendsOnly { get; set; }
			public bool IsMyMetro { get; set; }
		}

		searchParams theSearchParams = null;

		void reloadAsync(bool forceWorldwide = false)
        {
            TraceHelper.TraceInfoForResponsiveness("FindPeopleControl.reloadAsync");

			/// search parameters to use
			/// 

			var community = this.communitySelectorControl.Selection;
			this.communityLoaded = community;
			Country country = community.Country;
			int? metroID = null;
			if (community.MetroID > 0)
				metroID = community.MetroID;
			string nameQuery = this.entrySearch.Text;
			if (nameQuery == null)
				nameQuery = "";
			nameQuery = nameQuery.Trim();
			bool friendsOnly = community.IsFriendsOnly == true;
			if (forceWorldwide)
			{
				country = null;
				metroID = null;
				friendsOnly = false;
			}
			this.theSearchParams = new searchParams ()
			{
				NameQuery = nameQuery,
				Country = country,
				MetroID = metroID,
				FriendsOnly = friendsOnly,
				IsMyMetro = community.IsMyMetro,
			};

			/// if already searching, search again
			///
            if (isSearchingNow == true)
            {
                shouldDoAnotherSearch = true;
                return;
            }
            shouldDoAnotherSearch = false;

			/// update the UI
			/// 
			if (this.LoadStarted != null)
				this.LoadStarted ();
			this.panelStatus.IsVisible = true;
            this.labelStatus.Text = "Querying snookerbyb.com ...";
            this.listOfPeopleControl.IsVisible = false;
            this.buttonSearchWorldwide.IsVisible = false;

			/// load data async
			/// 
			Task.Run(async () => { await this.threadProc(); });
        }

		async Task threadProc()
		{
			List<PersonBasicWebModel> peopleFromWebservice = null;
			List<PersonBasicWebModel> people = new List<PersonBasicWebModel>();
			for (int iter = 0; iter < 3; ++iter)
			{
				searchParams sp = this.theSearchParams;
				
				/// query the web service
				/// 
				isSearchingNow = true;
				if (sp != null)
					peopleFromWebservice = await App.WebService.FindPeople(sp.NameQuery, sp.Country, sp.MetroID, sp.FriendsOnly, Config.App == MobileAppEnum.SnookerForVenues);
				isSearchingNow = false;

				/// query the local database
				/// 
				List<PersonBasicWebModel> peopleFromLocalDb = new List<PersonBasicWebModel>();
				if (Config.App == MobileAppEnum.SnookerForVenues && sp.IsMyMetro)
				{
					var idsOfPeoplePlayed = App.Repository.GetAthleteNamesFromResults(true);
					peopleFromLocalDb = App.Cache.People.Get(idsOfPeoplePlayed, true);
				}

				/// merge peopleFromWebservice and peopleFromHere
				/// 
				people = new List<PersonBasicWebModel>();
				foreach (var person in peopleFromLocalDb)
					people.Add(person);
				if (peopleFromWebservice != null)
				{
					foreach (var person in peopleFromWebservice)
						if (people.Where(i => i.ID == person.ID).Count() == 0)
							people.Add(person);
				}

				if (shouldDoAnotherSearch == false)
					break;
			}

			/// fill up
			/// 
			Device.BeginInvokeOnMainThread(() => 
			{
				this.fill(people, peopleFromWebservice);
				if (this.LoadCompleted != null)
					this.LoadCompleted();
			});
		}

		void fill(List<PersonBasicWebModel> people, List<PersonBasicWebModel> peopleFromWebservice)
		{
			if (people == null || (people.Count == 0 && peopleFromWebservice == null))
			{
				this.listOfPeopleControl.IsVisible = false;
				this.panelStatus.IsVisible = true;
				this.labelStatus.Text = "Couldn't load people from the cloud. Internet issues?";

				if (this.theSearchParams != null && this.theSearchParams.FriendsOnly)
				{
					people = App.Cache.People.GetFriends();
					if (people != null && people.Count > 0)
					{
						this.listOfPeopleControl.IsVisible = true;
						this.panelStatus.IsVisible = false;
						this.listOfPeopleControl.Fill(people);
					}
				}
			}
			else if (people.Count == 0)
			{
				this.listOfPeopleControl.IsVisible = false;
				this.panelStatus.IsVisible = true;
				this.labelStatus.Text = "No-one found.";
				if (this.theSearchParams != null && (this.theSearchParams.MetroID != null || this.theSearchParams.Country != null || this.theSearchParams.FriendsOnly))
					this.buttonSearchWorldwide.IsVisible = true;
			}
			else
			{
				this.listOfPeopleControl.IsVisible = true;
				this.panelStatus.IsVisible = false;
				listOfPeopleControl.Fill(people);

				App.Cache.People.Put(people);
			}

			if (people != null)
				countSearchedSuccessfully++;
		}
    }
}
