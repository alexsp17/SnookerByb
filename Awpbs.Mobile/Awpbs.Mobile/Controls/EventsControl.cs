using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class EventsControl : Grid
    {
		public bool IsShowingPastEvents
		{
			get
			{
				return this.listOfGameHostsPast.IsVisible;
			}
			set
			{
                this.panelFuture.IsVisible = !value;
                this.panelPast.IsVisible = value;
				this.buttonPastEvents.IsSelected = value;
				this.buttonNewEvents.IsSelected = !value;
                this.panelWithButtons.IsVisible = !value;
			}
		}

		BybButtonWithNumber buttonNewEvents;
		BybButtonWithNumber buttonPastEvents;
        StackLayout panelWithButtons;
        StackLayout panelFuture;
        StackLayout panelPast;
        ListOfGameHostsControl listOfGameHostsFuture;
        ListOfGameHostsControl listOfGameHostsPast;
        Refractored.XamForms.PullToRefresh.PullToRefreshLayout pullToRefresh;

        public EventsControl()
        {
            this.Padding = new Thickness(0);
            this.ColumnSpacing = 0;
            this.RowSpacing = 0;
            this.BackgroundColor = Config.ColorGrayBackground;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;

            this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
			this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

			// tabs
			buttonNewEvents = new BybButtonWithNumber("Current") { IsNumberVisible = false, HeightRequest = Config.OkCancelButtonsHeight + (Config.IsTablet ? 15 : 5) };
			buttonNewEvents.Clicked += (s, e) => { this.IsShowingPastEvents = false; };
			buttonPastEvents = new BybButtonWithNumber("Past") { IsNumberVisible = false, HeightRequest = Config.OkCancelButtonsHeight + (Config.IsTablet ? 15 : 5) };
			buttonPastEvents.Clicked += (s, e) => { this.IsShowingPastEvents = true; };
			this.Children.Add(new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 0, 0, 0),
				Spacing = 0,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					buttonNewEvents,
					buttonPastEvents,
				}
			}, 0, 0);

            // buttons
            Button buttonAdd = new BybButton() { Text = "New event", Style = (Style)App.Current.Resources["LargeButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonAdd.Clicked += (s1, e1) =>
            {
				if (App.Navigator.GetOpenedPage(typeof(NewGameHostPage)) != null)
					return;
				
                NewGameHostPage dlg = new NewGameHostPage();
                App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
				dlg.Disappearing += (s2, e2) => { this.ReloadAsync(false); };
            };
            Button buttonSync = new BybButton() { Text = "Sync", Style = (Style)App.Current.Resources["BlackButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonSync.Clicked += (s1, e1) =>
            {
                App.Navigator.StartSyncAndCheckForNotifications();
				this.ReloadAsync(false);
            };
            this.panelWithButtons = new StackLayout
            {
                Spacing = 1,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(10, 10, 10, 0),
                Children =
                {
                    buttonAdd,
                    buttonSync
                }
            };
            this.Children.Add(this.panelWithButtons, 0, 1);

            StackLayout stackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(),
            };

            // status
//            this.labelStatus = new BybLabel()
//            {
//                HorizontalOptions = LayoutOptions.Center,
//				TextColor = Config.ColorBlackTextOnWhite,
//            };
//            this.panelStatus = new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.Start,
//                Padding = new Thickness(10, 15, 0, 15),
//                Children =
//                {
//                    labelStatus
//                }
//            };
//            stackLayout.Children.Add(this.panelStatus);

            // current invites
            this.panelFuture = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0),
                Spacing = 0,
            };
            stackLayout.Children.Add(panelFuture);
            this.listOfGameHostsFuture = new ListOfGameHostsControl();
            this.listOfGameHostsFuture.TreatAsASingleItem = true;
            this.listOfGameHostsFuture.ShowCommentsCount = true;
            this.listOfGameHostsFuture.Padding = new Thickness(10, 10, 10, 0);
			this.listOfGameHostsFuture.UserChangedSomething += (s1, e1) => { this.ReloadAsync(false); };
            this.panelFuture.Children.Add(
                new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(10,10,0,0),
                    Children =
                    {
                        new BybLabel()
                        {
                            Text = "Your events and events you were invited to:",
                            HorizontalOptions = LayoutOptions.Start,
                            HorizontalTextAlignment = TextAlignment.Start,
							TextColor = Config.ColorGrayTextOnWhite,
                        }
                    }
                });
            panelFuture.Children.Add(this.listOfGameHostsFuture);
            var panel = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 10, 0, 10),
                    Children =
                    {
                        new BybLabel()
                        {
                            Text = "To see public events, go to the",
                            HorizontalOptions = LayoutOptions.Start,
                            HorizontalTextAlignment = TextAlignment.Start,
							TextColor = Config.ColorGrayTextOnWhite,
                        },
                        new BybLabel()
                        {
                            Text = "Community page",
                            FontAttributes = FontAttributes.Bold,
							TextColor = Config.ColorBlackTextOnWhite,
                        }
                    }
                };
            panelFuture.Children.Add(panel);
            panel.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(async () => { await App.Navigator.GoToCommunity(); }) });

            // past invites
            this.panelPast = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0),
                Spacing = 0,
            };
            stackLayout.Children.Add(panelPast);
            this.listOfGameHostsPast = new ListOfGameHostsControl();
            this.listOfGameHostsPast.TreatAsASingleItem = true;
            this.listOfGameHostsPast.ShowCommentsCount = true;
            this.listOfGameHostsPast.Padding = new Thickness(10, 10, 10, 0);
            this.listOfGameHostsPast.IsForPast = true;
			this.listOfGameHostsPast.UserChangedSomething += (s1, e1) => { this.ReloadAsync(false); };
            this.panelPast.Children.Add(this.listOfGameHostsPast);

            // scrolling and pulltorefresh
            var scrollView = new ScrollView
            {
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = stackLayout,
            };
            this.pullToRefresh = new Refractored.XamForms.PullToRefresh.PullToRefreshLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = scrollView,
                RefreshColor = Config.ColorRedBackground,
            };
			pullToRefresh.RefreshCommand = new Command(() => { this.ReloadAsync(false); });
            this.Children.Add(pullToRefresh, 0, 2);

			this.IsShowingPastEvents = false;
        }

        bool isLoadingNow;
        bool shouldDoAnotherLoad;
		DateTime timeOfLastReload = DateTime.MinValue;

		public void ReloadAsync(bool onlyIfItsBeenAwhile)
		{
			if (onlyIfItsBeenAwhile)
			{
				if ((DateTime.Now - timeOfLastReload).TotalMinutes < 2)
					return;
			}
			
			if (isLoadingNow == true)
			{
				shouldDoAnotherLoad = true;
				return;
			}
			shouldDoAnotherLoad = false;

			App.Navigator.SetRootPageTitleToLoading ();
			this.pullToRefresh.IsRefreshing = true;

			new Task (async () =>
			{
				// load data from web
				isLoadingNow = true;
				List<GameHostWebModel> gameHosts = null;
				for (int iter = 0; iter < 3; ++iter)
				{
					gameHosts = await App.WebService.GetMyGameHosts (true);
					if (shouldDoAnotherLoad == false)
						break;
				}
				isLoadingNow = false;
				this.timeOfLastReload = DateTime.Now;

				// fill
				Device.BeginInvokeOnMainThread(() => { fill(gameHosts); });
			}).Start ();
		}

		void fill(List<GameHostWebModel> gameHosts)
		{
			TraceHelper.TraceInfoForResponsiveness ("EventsControl.fill BEGIN");
			
            App.Navigator.SetRootPageTitleToNormal();
			this.pullToRefresh.IsRefreshing = false;
            
            if (gameHosts == null)
            {
				this.listOfGameHostsFuture.Fill (null);
				this.listOfGameHostsPast.Fill (null);
				TraceHelper.TraceInfoForResponsiveness ("EventsControl.fill END null");
				return;
            }

            DateTime now = DateTime.Now;

            List<GameHostWebModel> gameHosts_Past = new List<GameHostWebModel>();
            List<GameHostWebModel> gameHosts_Future = new List<GameHostWebModel>();
            foreach (var gameHost in gameHosts)
                if (gameHost.When >= now)
                    gameHosts_Future.Add(gameHost);
                else
                    gameHosts_Past.Add(gameHost);
            gameHosts_Future = gameHosts_Future.OrderBy(i => i.When).ToList();
            gameHosts_Past = gameHosts_Past.OrderByDescending(i => i.When).ToList();

			TraceHelper.TraceInfoForResponsiveness ("EventsControl.fill starting filling gameHosts_Future");
            this.listOfGameHostsFuture.Fill(gameHosts_Future);
			TraceHelper.TraceInfoForResponsiveness ("EventsControl.fill starting filling gameHosts_Past");
            this.listOfGameHostsPast.Fill(gameHosts_Past);

			TraceHelper.TraceInfoForResponsiveness ("EventsControl.fill END");
        }
    }
}
