using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
	public class FindVenuesControl : Grid
	{
		public readonly int MaxVenuesToLoad = 50;
		
		public event EventHandler<VenueWebModel> UserClickedOnVenue;

		public bool IsFindVenueMode { get; set; }

        public CommunitySelection CurrentCommunity
        {
            get
            {
                return this.communitySelectorControl.Selection;
            }
        }

        public bool IsShowingMap
        {
            get
            {
                return this.panelForMap.IsVisible;
            }
            set
            {
                if (value == true)
                {
                    layoutForTheList.IsVisible = false;
                    panelForMap.IsVisible = true;
                    buttonMapOrList.Text = "List";

                    // move the map (do it with a delay, otherwise zoom will be all off)
                    this.avoidLoadingVenuesUntilThisTime = DateTime.Now.AddSeconds(2);
                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
                        moveMap(false);
                        return false;
                    });
                }
                else
                {
                    panelForMap.IsVisible = false;
                    layoutForTheList.IsVisible = true;
                    buttonMapOrList.Text = "Map";

                    if (this.listNeedsToBeFilled)
                    {
						this.listOfVenuesControl.IsDistanceFromCurrentLocation = myLocation != null && Distance.Calculate (myLocation, currentLocation).Meters < 500;
						this.listOfVenuesControl.Fill(venues);
                        this.listNeedsToBeFilled = false;
                    }
                }
            }
        }

        // filters
        CommunitySelectorControl communitySelectorControl;
        Button buttonClearFilters;
        Entry entrySearch;

		// map / list switch
        Button buttonMapOrList;

		// status
        StackLayout panelStatus;
        Label labelStatus;

		// list
        ScrollView layoutForTheList;
		ListOfVenuesControl listOfVenuesControl;
        
		// map
		Frame panelForMap;
        Xamarin.Forms.Maps.Map map;

        List<VenueWebModel> venues;

        bool listNeedsToBeFilled = false;
        Thread thread = null;
        Location currentLocation = null;
		int? currentRadiusInMeters = null;
        Location myLocation = null;
        DateTime? timeCheckedMyLocation = null;
		DateTime? avoidLoadingVenuesUntilThisTime = null;

		public void DoOnVenueEdited(int venueID)
		{
			//listOfVenuesControl.DoOnVenueEdited (venueID);
		}

		public void Destroy()
		{
			if (this.thread != null)
				this.thread.Abort();
			this.thread = null;
		}

		public void ReloadAsync(CommunitySelection communitySelection)
		{
			if (this.thread == null)
			{
				this.thread = new Thread(threadProc);
				this.thread.Start();
			}

			if (this.CurrentCommunity.Compare(communitySelection) == false && communitySelection.IsFriendsOnly == false)
			{
				this.IsShowingMap = true;
				this.communitySelectorControl.Selection = communitySelection;
				this.setCurrentLocationFromCurrentCommunityAsync();
			}

			this.checkMyLocationIfNecessary ();
		}

        public FindVenuesControl(bool isMapByDefault = true)
		{
            this.Padding = new Thickness(0);
            this.RowSpacing = 0;
            this.ColumnSpacing = 0;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.BackgroundColor = Config.ColorGrayBackground;

            /// Map, list
            /// 
            this.map = new Xamarin.Forms.Maps.Map()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsShowingUser = true,
                MinimumHeightRequest = 200,
            };

            this.panelForMap = new Frame()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0),
                BackgroundColor = Color.Transparent
            };
            this.panelForMap.Content = this.map;

			this.listOfVenuesControl = new ListOfVenuesControl ();
            this.listOfVenuesControl.TextForFailedToLoad = ""; // do not show this error text, because this control will show something
			this.listOfVenuesControl.UserClickedOnVenue += (s1, e1) => {
				if (this.UserClickedOnVenue != null)
					this.UserClickedOnVenue(this, e1);
			};
            this.layoutForTheList = new ScrollView()
            {
                Padding = new Thickness(0,0,0,0),
                BackgroundColor = Config.ColorGrayBackground,
				Content = listOfVenuesControl
            };

            this.buttonMapOrList = new BybButton()
            {
                Text = "Map",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
            };
            this.buttonMapOrList.Clicked += (s1, e1) =>
            {
                this.IsShowingMap = !this.IsShowingMap;
            };

            /// Status
            /// 

            this.labelStatus = new BybLabel()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
				HeightRequest = 40,
                HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Config.ColorBlackTextOnWhite,
				//BackgroundColor = Color.Yellow
            };

            this.panelStatus = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0, 5, 0, 5),
                Spacing = 0,
                MinimumWidthRequest = 300,
                HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Config.ColorGrayBackground,
                IsVisible = false,
                Children =
                {
					labelStatus
                }
            };

            /// Filters
            /// 

            // community
            this.communitySelectorControl = new CommunitySelectorControl();
            this.communitySelectorControl.SelectionChanged += communitySelectorControl_SelectionChanged;

            // search
            this.entrySearch = new BybNoBorderEntry()
			{
				Placeholder = Config.IsAndroid ? "Search" : "Search by name",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center
			};
			this.entrySearch.TextChanged += (s1, e1) => {
				this.buttonClearFilters.IsVisible = string.IsNullOrEmpty(this.entrySearch.Text) == false;
			};
            this.entrySearch.Completed += (s1, e1) =>
            {
				this.needToSearchAgain = true;
            };
			this.buttonClearFilters = new BybButton() { Text = "x", IsVisible = false, Style = (Style)App.Current.Resources["SimpleButtonStyle"], WidthRequest = 30 };
            this.buttonClearFilters.Clicked += (s, e) =>
            {
                this.entrySearch.Text = "";
            };

            /// Content
            /// 

            this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Config.LargeButtonsHeight*2 + 10, GridUnitType.Absolute) });
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(Config.IsTablet ? 50 : 30, GridUnitType.Absolute) });
            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            this.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Padding = new Thickness(0,0,0,0),
                Children =
                {
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(0, 5, 0, 5),
                        HeightRequest = Config.LargeButtonsHeight,//40,
                        Spacing = 1,
                        BackgroundColor = Config.ColorGrayBackground,
                        Children =
                        {
                            communitySelectorControl,
                        }
                    },

                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HeightRequest = Config.LargeButtonsHeight,//40,
                        Padding = new Thickness(Config.IsIOS ? 15 : 5,0,10,0),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Spacing = 1,
                        BackgroundColor = Config.ColorBackgroundWhite,
                        Children =
                        {
                            this.entrySearch,
                            this.buttonClearFilters,
							this.buttonMapOrList,
                        }
                    }
               },
            }, 0, 0);
            this.Children.Add(panelForMap, 0, 1, 1, 3);
            this.Children.Add(layoutForTheList, 0, 1, 1, 3);

            this.Children.Add(panelStatus, 0, 1);

            this.IsShowingMap = isMapByDefault;
		}

        protected override void OnSizeAllocated(double width, double height)
        {
            // this is done in order to avoid xamarin incorrectly applying width when the user comes to the Venues page, then goes away, and then comes back
            this.panelStatus.WidthRequest = width;
            this.labelStatus.WidthRequest = width;

            base.OnSizeAllocated(width, height);
        }

        void communitySelectorControl_SelectionChanged(object sender, EventArgs e)
        {
            this.IsShowingMap = true;

			this.setCurrentLocationFromCurrentCommunityAsync();
        }

		void checkMyLocationIfNecessary()
		{
			TraceHelper.TraceInfoForResponsiveness ("checkMyLocationIfNecessary.Begin");
			
			if (timeCheckedMyLocation != null && (DateTime.Now - timeCheckedMyLocation.Value).TotalSeconds < 20)
			{
				TraceHelper.TraceInfoForResponsiveness ("checkMyLocationIfNecessary.End, too quickly");
				return;
			}
			this.timeCheckedMyLocation = DateTime.Now;
			
            this.labelStatus.Text = "finding your location...";
            this.panelStatus.IsVisible = true;

			var location = App.LocationService.GetLastKnownLocationQuickly ();
			if (location != null)
				this.doOnNewMyLocation (location);

            App.LocationService.RequestLocationAsync((s1, e1) =>
        	{
				TraceHelper.TraceInfoForResponsiveness ("RequestLocationAsync event");
				this.doOnNewMyLocation(App.LocationService.Location);
        	}, true);
        }

		void doOnNewMyLocation(Location newMyLocation)
		{
            if (newMyLocation == null)
            {
				TraceHelper.TraceInfoForResponsiveness ("doOnNewMyLocation. newMyLocation==null");
				
                if (this.currentLocation == null)
                    this.needToSearchAgain = true;
                return;
            }

			TraceHelper.TraceInfoForResponsiveness ("doOnNewMyLocation. newMyLocation!=null");

			if (currentLocation == null || myLocation == null || Distance.Calculate (myLocation, newMyLocation).Miles > 10)
			{
				this.myLocation = newMyLocation;
				currentLocation = myLocation;
				moveMap (false);
			}
		}

        class searchParams
        {
            public bool RequireSnookerTables { get; set; }
            public bool Require12ftTables { get; set; }
            public string SearchQuery { get; set; }
            public Location Location { get; set; }
			public int RadiusInMeters { get; set; }
			public string Country { get; set; }
        }
		searchParams searchParamsToUse = null;
		bool needToSearchAgain = false;

		async void threadProc()
		{
			for (long iter = 0; ; ++iter)
			{
				Thread.Sleep(500);

				bool shouldCheck = iter % 3 == 0; // check every 1500ms
				if (this.avoidLoadingVenuesUntilThisTime != null && DateTime.Now < this.avoidLoadingVenuesUntilThisTime.Value)
					shouldCheck = false;

				if (shouldCheck == true)
				{
					Device.BeginInvokeOnMainThread (() =>
					{
						// check if the user moved the map
						if (map.VisibleRegion != null)
						{
							Location newLocation = new Location (map.VisibleRegion.Center.Latitude, map.VisibleRegion.Center.Longitude);
							int newRadius = (int)map.VisibleRegion.Radius.Meters;
							if ((currentLocation != null && newLocation != null && Distance.Calculate (newLocation, currentLocation).Miles > 5) ||
							    (currentRadiusInMeters != null && System.Math.Abs (currentRadiusInMeters.Value - newRadius) > 10000))
							{
								this.currentLocation = newLocation;
								this.needToSearchAgain = true;
							}
						}

						if (this.needToSearchAgain)
						{
							TraceHelper.TraceInfoForResponsiveness ("threadProc - needToSearchAgain=true");
							
							this.needToSearchAgain = false;
							this.currentRadiusInMeters = (int)(map.VisibleRegion != null ? map.VisibleRegion.Radius.Meters : 50000);

							int radius = this.currentRadiusInMeters.Value;
							if (radius > 500000)
								radius = (int)(radius * 1.5);
							if (radius > 1000000)
								radius = (int)(radius * 1.5);
							
							// start the search
							searchParamsToUse = new searchParams ()
							{
								RequireSnookerTables = false,
								Require12ftTables = false,
								SearchQuery = this.entrySearch.Text,
								Location = this.currentLocation,
								RadiusInMeters = radius,
								Country = (this.CurrentCommunity != null && this.CurrentCommunity.Country != null) ? this.CurrentCommunity.Country.ThreeLetterCode : "",
							};

							this.panelStatus.IsVisible = true;
							this.labelStatus.Text = "Loading venues...";
						}
					});
				}

				if (this.searchParamsToUse == null)
					continue;

				var venuesFromWeb = await App.WebService.FindSnookerVenues2(searchParamsToUse.Location, searchParamsToUse.Country, searchParamsToUse.RadiusInMeters, searchParamsToUse.SearchQuery, MaxVenuesToLoad);
				this.searchParamsToUse = null;

				Device.BeginInvokeOnMainThread(() => { this.doOnVenuesLoaded(venuesFromWeb); });
			}
		}

		void doOnVenuesLoaded(FindVenuesWebModel venuesFromWeb)
		{
			TraceHelper.TraceInfoForResponsiveness ("doOnVenuesLoaded");
			
			this.venues = venuesFromWeb != null ? venuesFromWeb.Venues : null;
			this.fillMap ();
			this.fillList();
            this.layoutForTheList.Padding = new Thickness(0);
            if (venues == null)
			{
                this.labelStatus.Text = "Couldn't load venues. Internet issues?";
                this.panelStatus.IsVisible = true;
            }
            else if (venuesFromWeb != null && venuesFromWeb.TotalCountAvailable > MaxVenuesToLoad)
			{
				this.labelStatus.Text = "Showing random " + MaxVenuesToLoad.ToString() + " venues within the map area.";
                this.layoutForTheList.Padding = new Thickness(0, 30, 0, 0);
				this.panelStatus.IsVisible = true;
			}
			else
			{
				this.labelStatus.Text = "-";
				this.panelStatus.IsVisible = false;
			}

			bool isMapOnDifferentCommunity = false;
			if (venuesFromWeb != null && venuesFromWeb.ClosestMetro != null)
			{
				if (this.CurrentCommunity.MetroID > 0 && this.CurrentCommunity.MetroID != venuesFromWeb.ClosestMetro.ID)
					isMapOnDifferentCommunity = true;
				if (this.CurrentCommunity.MetroID <= 0 && this.CurrentCommunity.Country != null && venuesFromWeb.ClosestMetro.Country != this.CurrentCommunity.Country.ThreeLetterCode)
					isMapOnDifferentCommunity = true;
			}
			if (isMapOnDifferentCommunity)
				this.communitySelectorControl.AnimateWithRed ();

			this.communitySelectorControl.IsAskToTapVisible = isMapOnDifferentCommunity == true && this.IsFindVenueMode == false;
		}
        
        void fillMap()
        {
			TraceHelper.TraceInfoForResponsiveness ("fillMap");
			
            map.Pins.Clear();

			if (venues == null)
				return;

			int maxPins = System.Math.Min(venues.Count, MaxVenuesToLoad); // just in case

            // Add pins to the map
			for (int i = 0; i < maxPins; ++i)
            {
				var venue = venues [i];
				
                var pin = new Xamarin.Forms.Maps.Pin
                {
                    Type = Xamarin.Forms.Maps.PinType.Place,
                    Position = new Xamarin.Forms.Maps.Position(venue.Latitude.Value, venue.Longitude.Value),
                    Label = venue.Name,
                    Address = "- tap to select -"
                };
                pin.Clicked += (s, e) =>
                {
                    if (this.UserClickedOnVenue != null)
                        this.UserClickedOnVenue(this, venue);
                };
				map.Pins.Add(pin);
            }
        }

		void fillList()
		{
			if (this.IsShowingMap == false)
			{
				this.listOfVenuesControl.IsDistanceFromCurrentLocation = myLocation != null && Distance.Calculate (myLocation, currentLocation).Meters < 500;
				this.listOfVenuesControl.Fill (venues);
			}
			else
			{
				this.listNeedsToBeFilled = true;
			}
		}

		void setCurrentLocationFromCurrentCommunityAsync()
		{
			TraceHelper.TraceInfoForResponsiveness("setCurrentLocationFromCurrentCommunityAsync");

			if (this.CurrentCommunity == null)
			{
				TraceHelper.TraceInfoForResponsiveness("setCurrentLocationFromCurrentCommunityAsync CurrentCommunity==null");
				return;
			}

			Task.Run(async () =>
			{
				if (this.CurrentCommunity.MetroID > 0)
				{
					await App.Cache.LoadFromWebserviceIfNecessary_Metro(this.CurrentCommunity.MetroID);
					var metro = App.Cache.Metroes.Get (this.CurrentCommunity.MetroID);
					if (metro == null)
						return;
					this.currentLocation = metro.Location;
				}
				else if (this.CurrentCommunity.Country != null)
				{
					var metros = App.Cache.Metroes.GetForCountry (this.CurrentCommunity.Country.ThreeLetterCode);
					if (metros.Count <= 3)
						metros = await App.WebService.GetMetros (this.CurrentCommunity.Country.ThreeLetterCode);
					if (metros == null || metros.Count < 1)
						return;
					double lat = 0.5 * (metros.Select (i => i.Latitude).Min () + metros.Select (i => i.Latitude).Max ());
					double lon = 0.5 * (metros.Select (i => i.Longitude).Min () + metros.Select (i => i.Longitude).Max ());

	                if (this.CurrentCommunity.Country.IsCanada)
	                    lat += 10;
					this.currentLocation = new Location () { Latitude = lat, Longitude = lon };
				}

				Device.BeginInvokeOnMainThread(() => 
				{
					this.moveMap(true);
				});
			});
		}

        //bool hasNotMovedMapYet = true;
        double? radiusInMeters = null;

        void moveMap(bool updateZoom)
        {
			if (this.currentLocation == null)
				TraceHelper.TraceInfoForResponsiveness("moveMap. currentLocation=null");
			else
				TraceHelper.TraceInfoForResponsiveness("moveMap. currentLocation=" + this.currentLocation.ToString());

            /// what should be the map center point?
            /// 
            Location location = this.currentLocation;
            if (location == null)
            {
                // point to Portland
                double latitude_Portland = 45.53;
                double longtitude_Portland = -122.68;
                location = new Location(latitude_Portland, longtitude_Portland);
            }

            /// what should be the map "radius"?
            /// 
            double newRadiusInMeters;
            if (this.radiusInMeters != null && updateZoom == false)
            {
                // use previous radius
                newRadiusInMeters = radiusInMeters.Value;
            }
            else
            {
                if (this.CurrentCommunity != null && this.CurrentCommunity.IsPlanetEarth)
                {
                    // Planet Earth level
                    newRadiusInMeters = 5000000;
                }
                else if (this.CurrentCommunity != null && this.CurrentCommunity.MetroID == 0 && this.CurrentCommunity.Country != null)
                {
                    // country-level
                    Country country = this.CurrentCommunity.Country;
                    if (country.AreaSize == CountrySizeEnum.Regular)
                        newRadiusInMeters = 800000;
                    else
                        newRadiusInMeters = 5000000;
                }
                else
                {
                    // city-level
                    newRadiusInMeters = 50000;
                }
            }
            this.radiusInMeters = newRadiusInMeters;

            /// move the map
            /// 
            var loc2 = location.OffsetRoughly(newRadiusInMeters, newRadiusInMeters);
            double lat = System.Math.Abs(location.Latitude - loc2.Latitude);
            double lon = System.Math.Abs(location.Longitude - loc2.Longitude);
            this.avoidLoadingVenuesUntilThisTime = DateTime.Now.AddSeconds(2);
            TraceHelper.TraceInfoForResponsiveness("moveMap.MoveToRegion!!! location=" + location.ToString() + "   lat=" + lat + "   lon=" + lon);
            map.MoveToRegion(new Xamarin.Forms.Maps.MapSpan(
                new Xamarin.Forms.Maps.Position(location.Latitude, location.Longitude),
                lat, lon));

            this.needToSearchAgain = true;
        }
	}
}
