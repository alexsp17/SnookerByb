using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Refractored.XamForms.PullToRefresh;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Awpbs.Mobile
{
    public enum CommunityControlTabEnum
    {
        Venues,
        People,
        Feed,
    }

    public class CommunityControl : Grid
    {
		public CommunityControlTabEnum CurrentTab
        {
            get
            {
				if (this.rootForPeople != null && this.rootForPeople.IsVisible)
					return CommunityControlTabEnum.People;
				if (this.findVenuesControl != null && this.findVenuesControl.IsVisible)
					return CommunityControlTabEnum.Venues;
				return CommunityControlTabEnum.Feed;
            }
        }

        public CommunitySelection CurrentCommunity
        {
            get
            {
				if (CurrentTab == CommunityControlTabEnum.Feed)
                    return this.newsfeedControl.CurrentCommunity;
				if (CurrentTab == CommunityControlTabEnum.Venues)
                    return this.findVenuesControl.CurrentCommunity;
                return this.findPeopleControl.CurrentCommunity;
            }
        }

        BybButtonWithNumber buttonVenues;
        BybButtonWithNumber buttonPeople;
        BybButtonWithNumber buttonFeed;

        NewsfeedControl newsfeedControl;
        FindVenuesControl findVenuesControl;

		PullToRefreshLayout rootForPeople;
        FindPeopleControl findPeopleControl;

		PullToRefreshLayout rootForNewsfeed;
        ScrollView scrollViewForNewsfeed;

        public CommunityControl()
        {
			this.Padding = new Thickness(0);
            this.ColumnSpacing = 0;
            this.RowSpacing = 0;
            this.BackgroundColor = Config.ColorGrayBackground;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;

            this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

			// buttons
			buttonVenues = new BybButtonWithNumber("Venues") { IsNumberVisible = false, HeightRequest = Config.OkCancelButtonsHeight + (Config.IsTablet ? 15 : 5) };
			buttonVenues.Clicked += (s, e) => { this.OpenTab(CommunityControlTabEnum.Venues); };
			buttonPeople = new BybButtonWithNumber("Players") { IsNumberVisible = false, HeightRequest = Config.OkCancelButtonsHeight + (Config.IsTablet ? 15 : 5) };
			buttonPeople.Clicked += (s, e) => { this.OpenTab(CommunityControlTabEnum.People); };
			buttonFeed = new BybButtonWithNumber("Feed") { IsNumberVisible = false, HeightRequest = Config.OkCancelButtonsHeight + (Config.IsTablet ? 15 : 5) };
			buttonFeed.Clicked += (s, e) => { this.OpenTab(CommunityControlTabEnum.Feed); };

            this.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Padding = new Thickness(0, 0, 0, 0),
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    buttonPeople,
                    buttonFeed,
                    buttonVenues,
                }
            }, 0, 0);

			this.createFeedTabIfNotCreatedYet ();
        }

		public void OpenTab(CommunityControlTabEnum newTab)
		{
			CommunitySelection currentCommunity = this.CurrentCommunity;

			if (rootForNewsfeed != null)
				this.rootForNewsfeed.IsVisible = newTab == CommunityControlTabEnum.Feed;
			if (this.rootForPeople != null)
				this.rootForPeople.IsVisible = newTab == CommunityControlTabEnum.People;
			if (this.findVenuesControl != null)
				this.findVenuesControl.IsVisible = newTab == CommunityControlTabEnum.Venues;

			if (newTab == CommunityControlTabEnum.Feed)
			{
				this.createFeedTabIfNotCreatedYet ();
				this.newsfeedControl.ReloadAsync (currentCommunity, true);
			}
			else if (newTab == CommunityControlTabEnum.Venues)
			{
				this.createVenuesTabIfNotCreatedYet ();
				this.findVenuesControl.ReloadAsync(currentCommunity);
			}
			else
			{
				this.createPeopleTabIfNotCreatedYet ();
				this.findPeopleControl.ReloadAsync(currentCommunity, true);
			}

			if (this.scrollViewForNewsfeed != null)
				this.scrollViewForNewsfeed.IsVisible = this.rootForNewsfeed.IsVisible;
			if (this.findPeopleControl != null)
				this.findPeopleControl.IsVisible = this.rootForPeople.IsVisible;

			this.buttonFeed.IsSelected = newTab == CommunityControlTabEnum.Feed;
			this.buttonVenues.IsSelected = newTab == CommunityControlTabEnum.Venues;
			this.buttonPeople.IsSelected = newTab == CommunityControlTabEnum.People;
		}

        public void DoOnOpen()
        {
			OpenTab (this.CurrentTab);
        }

        public void DoOnVenueEdited(int venueID)
        {
            //this.findVenuesControl.DoOnVenueEdited(venueID);
        }

		void createVenuesTabIfNotCreatedYet()
		{
			if (this.findVenuesControl != null)
				return;
			
			this.findVenuesControl = new FindVenuesControl() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
			this.findVenuesControl.UserClickedOnVenue += async (s1, e1) =>
			{
				await App.Navigator.GoToVenueProfile(e1.ID);
			};
			this.Children.Add(this.findVenuesControl, 0, 1);
		}

		void createPeopleTabIfNotCreatedYet()
		{
			if (this.findPeopleControl != null)
				return;
			
			this.findPeopleControl = new FindPeopleControl(false) { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
			this.findPeopleControl.LoadStarted += () =>
			{
				if (this.rootForPeople != null)
					this.rootForPeople.IsRefreshing = true;
			};
			this.findPeopleControl.LoadCompleted += () =>
			{
				if (this.rootForNewsfeed != null)
					this.rootForNewsfeed.IsRefreshing = false;
				if (this.rootForPeople != null)
					this.rootForPeople.IsRefreshing = false;
			};
			this.findPeopleControl.NameAsCommunity = true;
			this.findPeopleControl.UserClickedOnPerson += async (s1, e1) =>
			{
				await App.Navigator.GoToPersonProfile(e1.Person.ID);
			};
			this.rootForPeople = new PullToRefreshLayout ()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = this.findPeopleControl,
				RefreshColor = Config.ColorRedBackground,
			};
			this.rootForPeople.RefreshCommand = new Command(() =>
			{
				this.findPeopleControl.ReloadAsync(this.CurrentCommunity, false);
			});
			this.Children.Add(this.rootForPeople, 0, 1);
		}

		void createFeedTabIfNotCreatedYet()
		{
			if (this.newsfeedControl != null)
				return;
			
			this.newsfeedControl = new NewsfeedControl();
			this.newsfeedControl.LoadStarted += () =>
			{
				if (this.rootForNewsfeed != null)
					this.rootForNewsfeed.IsRefreshing = true;
			};
			this.newsfeedControl.LoadCompleted += () =>
			{
				if (this.rootForNewsfeed != null)
					this.rootForNewsfeed.IsRefreshing = false;
				if (this.rootForPeople != null)
					this.rootForPeople.IsRefreshing = false;
			};
			this.scrollViewForNewsfeed = new ScrollView
			{
				Padding = new Thickness(0),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = this.newsfeedControl,
			};
			this.rootForNewsfeed = new Refractored.XamForms.PullToRefresh.PullToRefreshLayout ()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = scrollViewForNewsfeed,
				RefreshColor = Config.ColorRedBackground,
			};
			this.rootForNewsfeed.RefreshCommand = new Command(() =>
			{
				this.newsfeedControl.ReloadAsync(this.CurrentCommunity, false);
			});
			this.Children.Add (this.rootForNewsfeed, 0, 1);
		}
    }
}
