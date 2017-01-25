using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Awpbs.Mobile
{
    public enum ProfileVenueStateEnum
    {
        Unknown = 0,
        Breaks,
		Matches,
        People,
        Games,
    }

    public class ProfileVenueControl : StackLayout
    {
		public Action LoadStarted;
		public event Action<bool> LoadCompleted;
		
		public int VenueID { get; private set; }
        public FullSnookerVenueData FullVenueData { get; private set; }

        public ProfileVenueStateEnum State
        {
            get
            {
                if (this.panelGameHosts.IsVisible)
                    return ProfileVenueStateEnum.Games;
                if (this.listOfPeopleControl.IsVisible)
                    return ProfileVenueStateEnum.People;
                return ProfileVenueStateEnum.Breaks;
            }
            set
            {
                if (value == ProfileVenueStateEnum.Breaks)
                {
					this.listOfMatchesControl.IsVisible = false;
                    this.listOfPeopleControl.IsVisible = false;
                    this.panelGameHosts.IsVisible = false;
                    this.listOfBreaksControl.IsVisible = true;
                }
				else if (value == ProfileVenueStateEnum.Matches)
				{
					this.listOfBreaksControl.IsVisible = false;
					this.panelGameHosts.IsVisible = false;
					this.listOfPeopleControl.IsVisible = false;
					this.listOfMatchesControl.IsVisible = true;
				}
                else if (value == ProfileVenueStateEnum.People)
                {
                    this.listOfBreaksControl.IsVisible = false;
					this.listOfMatchesControl.IsVisible = false;
                    this.panelGameHosts.IsVisible = false;
                    this.listOfPeopleControl.IsVisible = true;
                }
                else if (value == ProfileVenueStateEnum.Games)
                {
                    this.listOfBreaksControl.IsVisible = false;
					this.listOfMatchesControl.IsVisible = false;
                    this.listOfPeopleControl.IsVisible = false;
                    this.panelGameHosts.IsVisible = true;
                }

                this.buttonBreaks.IsSelected = value == ProfileVenueStateEnum.Breaks;
				this.buttonMatches.IsSelected = value == ProfileVenueStateEnum.Matches;
                this.buttonPeople.IsSelected = value == ProfileVenueStateEnum.People;
                this.buttonGames.IsSelected = value == ProfileVenueStateEnum.Games;

                this.fillVisibleControlsWithExistingData();
            }
        }

		public void FillAsync(int venueID, bool onlyIfItsBeenAwhile)
		{
			this.VenueID = venueID;
			loadDataAsyncAndFill(onlyIfItsBeenAwhile);
		}

        // about panel
        StackLayout panelAbout;
        Label labelMetro;
        StackLayout panelInvalid;
        Button buttonPhoneNumber;
        Label buttonWebsite;
        Button buttonDirections;
        Label label10ftTables;
        Label label12ftTables;
        Xamarin.Forms.Maps.Map map;

        // contribute panel
        StackLayout panelNotVerifiedYet;
        StackLayout panelVerified;
        Label labelVerifiedBy;
        Label labelVerifiedOn;
        Button buttonEdit;

		Grid panelWithLargeButtons;

        // tabs:
        BybButtonWithNumber buttonBreaks;
		BybButtonWithNumber buttonMatches;
        BybButtonWithNumber buttonPeople;
        BybButtonWithNumber buttonGames;
        ListOfSnookerBreaksControl listOfBreaksControl;
		ListOfSnookerMatchesControl listOfMatchesControl;
        ListOfPeopleControl listOfPeopleControl;
        StackLayout panelGameHosts;
        ListOfGameHostsControl listOfGameHostsControlFuture;
        ListOfGameHostsControl listOfGameHostsControlPast;
		Button buttonNewGameHost;

        public ProfileVenueControl()
        {
			this.Padding = new Thickness(0);
            this.BackgroundColor = Config.ColorBackground;
            this.Spacing = 0;

            // Metro
            labelMetro = new BybLabel() { Text = "", Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"], HorizontalOptions = LayoutOptions.Center };

            // tables
            label10ftTables = new BybLabel()
            {
                Text = "",
                FontFamily = Config.FontFamily,
                FontAttributes = FontAttributes.Bold,
                FontSize = Config.LargerFontSize,
                BackgroundColor = Color.Transparent,
                TextColor = Config.ColorRedBackground,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            label12ftTables = new BybLabel()
            {
                Text = "",
                FontFamily = Config.FontFamily,
                FontAttributes = FontAttributes.Bold,
                FontSize = Config.LargerFontSize,
                BackgroundColor = Color.Transparent,
                TextColor = Config.ColorRedBackground,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            // panel: not verified
            this.panelNotVerifiedYet = new StackLayout()
            {
                BackgroundColor = Config.ColorBackground,
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(0),
                Children =
                {
                    new BybLabel {  Text = "Not verified by the community yet", TextColor = Config.ColorTextOnBackgroundGrayed }
                }
            };

            // panel verified
			this.labelVerifiedBy = new BybLabel() { VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorTextOnBackground };
			this.labelVerifiedBy.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(async () =>
					{
						if (FullVenueData.Venue.LastContributorID > 0)
							await App.Navigator.GoToPersonProfile(FullVenueData.Venue.LastContributorID);
					}),
					NumberOfTapsRequired = 1
				});
            this.labelVerifiedOn = new BybLabel() { VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorTextOnBackground };
            this.panelVerified = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 2,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new BybLabel { Text = "Verified by ", VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorTextOnBackgroundGrayed },
                    labelVerifiedBy,
                    new BybLabel { Text = " on ", VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorTextOnBackgroundGrayed },
                    labelVerifiedOn
                }
            };

            // panel: Invalid
            this.panelInvalid = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 10, 0, 0),
                Children =
                {
                    new BybLabel { Text = "The venue is closed-down (or invalid entry)", FontAttributes = FontAttributes.Bold, TextColor = Config.ColorTextOnBackground }
                }
            };

            // map
            this.map = new Xamarin.Forms.Maps.Map()
            {
                HeightRequest = Config.IsTablet ? 200 : 160,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            // website
            buttonWebsite = new BybLabel
            {
                Text = "",
                TextColor = Config.ColorTextOnBackground,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttonWebsite.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (FullVenueData.Venue.HasWebsite)
                        App.Navigator.OpenBrowserApp(FullVenueData.Venue.Website);
                }),
                NumberOfTapsRequired = 1
            });

            this.panelAbout = new StackLayout()
            {
                BackgroundColor = Config.ColorBackground,
                Orientation = StackOrientation.Vertical,
                Spacing = 5,
                Padding = new Thickness(0, 0, 0, 0),
                Children =
                {
                    labelMetro,

                    new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        Padding = new Thickness(0,0,0,0),
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new BybLabel { Text = "10' tables", VerticalTextAlignment = TextAlignment.Center, Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"] },
                            label10ftTables,
                            new BybLabel { Text = " 12' tables", VerticalTextAlignment = TextAlignment.Center, Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"] },
                            label12ftTables,
                        }
                    },

                    this.panelInvalid,
                    this.panelNotVerifiedYet,
                    this.panelVerified,

                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Padding = new Thickness(0,10,0,0),
                        Children =
                        {
                            map
                        }
                    },

                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Padding = new Thickness (0,0,0,0),
                        Children =
                        {
                            new BybLabel
                            {
                                Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"],
                                VerticalTextAlignment = TextAlignment.Center,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Text = "Website: "
                            },
                            this.buttonWebsite
                        }
                    },
                }
            };
            this.Children.Add(panelAbout);

            /// Large buttons
            /// 

            buttonPhoneNumber = new BybButton() { Text = "", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonPhoneNumber.Clicked += (s1, e1) =>
            {
                if (FullVenueData.Venue.HasPhoneNumber)
                    App.Navigator.OpenPhoneCallApp(FullVenueData.Venue.PhoneNumber);
            };

            buttonDirections = new BybButton() { Text = "", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonDirections.Clicked += (s1, e1) =>
            {
                if (FullVenueData.Venue.Location != null)
                    App.Navigator.OpenMapsApp(FullVenueData.Venue.Location, FullVenueData.Venue.Name, FullVenueData.Venue.Address);
            };

            this.buttonEdit = new BybButton()
            {
                Text = "Edit / verify",
                Style = (Style)App.Current.Resources["BlackButtonStyle"],
            };
            this.buttonEdit.Clicked += buttonEdit_Clicked;

            this.panelWithLargeButtons = new Grid()
            {
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(0, 10, 0, 10),
                ColumnSpacing = 0,
                RowSpacing = 0,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height = new GridLength(Config.LargeButtonsHeight, GridUnitType.Absolute)},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition { Width = new GridLength(10, GridUnitType.Absolute)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute)},
                    new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute)},
                    new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(10, GridUnitType.Absolute)},
                }
            };
            panelWithLargeButtons.Children.Add(buttonPhoneNumber, 1, 0);
            panelWithLargeButtons.Children.Add(buttonDirections, 3, 0);
            panelWithLargeButtons.Children.Add(buttonEdit, 5, 0);
            this.Children.Add(panelWithLargeButtons);

            /// Tabs: Breaks / players / games
            /// 

            this.buttonBreaks = new BybButtonWithNumber("Breaks") { IsNumberVisible = false };
            buttonBreaks.Clicked += (s, e) => { this.State = ProfileVenueStateEnum.Breaks; };
			this.buttonMatches = new BybButtonWithNumber("Matches") { IsNumberVisible = false };
			buttonMatches.Clicked += (s, e) => { this.State = ProfileVenueStateEnum.Matches; };
            this.buttonPeople = new BybButtonWithNumber("Players") { IsNumberVisible = false };
            buttonPeople.Clicked += (s, e) => { this.State = ProfileVenueStateEnum.People; };
            this.buttonGames = new BybButtonWithNumber("Invites") { IsNumberVisible = false };
            buttonGames.Clicked += (s, e) => { this.State = ProfileVenueStateEnum.Games; };

            Grid gridWithButtons = new Grid()
            {
                BackgroundColor = Config.ColorBackgroundWhite,
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
            gridWithButtons.Children.Add(buttonBreaks, 1, 0);
			gridWithButtons.Children.Add(buttonMatches, 2, 0);
            gridWithButtons.Children.Add(buttonPeople, 3, 0);
			gridWithButtons.Children.Add(buttonGames, 0, 0);
            this.Children.Add(gridWithButtons);

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
                IsVisible = true,
				Type = ListTypeEnum.Venue,
                SortType = SnookerBreakSortEnum.ByPoints,
            };
            panelContent.Children.Add(this.listOfBreaksControl);

			// matches
			this.listOfMatchesControl = new ListOfSnookerMatchesControl ()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				IsVisible = false,
			};
			panelContent.Children.Add(this.listOfMatchesControl);

            // game hosts
            this.listOfGameHostsControlFuture = new ListOfGameHostsControl()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(5,5,5,5),
                IsForPast = false,
                ShowCommentsCount = true,
            };
            this.listOfGameHostsControlPast = new ListOfGameHostsControl()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(5, 5, 5, 5),
                IsForPast = true
            };
            this.buttonNewGameHost = new BybButton()
            {
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                HorizontalOptions = LayoutOptions.Start,
                Text = "Make a New Invite"
            };
            buttonNewGameHost.Clicked += buttonNewGameHost_Clicked;
            this.panelGameHosts = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                IsVisible = false,
                Padding = new Thickness(5,5,5,5),
                Children =
                {
					new StackLayout
					{
						Padding = new Thickness(5,0,0,0),
						Children =
						{
							buttonNewGameHost,
						}
					},
                    new StackLayout
                    {
                        Padding = new Thickness(5,10,0,0),
                        Children =
                        {
                            new BybLabel
                            {
                                Text = "Active Invites",
                                TextColor = Config.ColorGrayTextOnWhite,
                                HorizontalTextAlignment = TextAlignment.Start,
                            },
                        }
                    },
                    //this.panelNoInvitesFuture,
                    this.listOfGameHostsControlFuture,
                    new StackLayout
                    {
                        Padding = new Thickness(5,10,0,0),
                        Children =
                        {
                            new BybLabel
                            {
                                Text = "Past Invites",
                                TextColor = Config.ColorGrayTextOnWhite,
                                HorizontalTextAlignment = TextAlignment.Start,
                            },
                        }
                    },
                    this.listOfGameHostsControlPast,
                }
            };
            panelContent.Children.Add(this.panelGameHosts);

            // people
            this.listOfPeopleControl = new ListOfPeopleControl()
            {
                IsVisible = false,
            };
            this.listOfPeopleControl.UserClickedOnPerson += async (s, e) =>
            {
                await App.Navigator.GoToPersonProfile(e.Person.ID);
            };
            panelContent.Children.Add(this.listOfPeopleControl);
            this.Children.Add(panelContent);

            this.State = ProfileVenueStateEnum.Games;
        }

        private void buttonNewGameHost_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage(typeof(NewGameHostPage)) != null)
				return;
			
            NewGameHostPage dlg = new NewGameHostPage();
            dlg.SetVenue(this.FullVenueData.Venue);
            App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
            dlg.Disappearing += (s2, e2) =>
            {
                //var timer = new System.Timers.Timer(1000);
                //timer.Start();
                //timer.Elapsed += (s3, e1) =>
                //{
                //    timer.Stop();
                //    Device.BeginInvokeOnMainThread(async () =>
                //    {
                //        await App.Navigator.NavPage.Navigation.PopAsync();
                //        App.Navigator.GoToPersonProfile(this.FullVenueData.VenueID);
                //        //App.Navigator.NavPage.PopAsync();
                //        //App.Navigator.RootPage.InitAsVenueProfile(this.FullVenueData.VenueID);
                //    });
                //};
            };
        }

		void loadDataAsyncAndFill(bool loadDataAsyncAndFill)
		{
			this.FullVenueData = null;

			this.clear ();

			if (this.LoadStarted != null)
				this.LoadStarted ();

			new Task (async () =>
			{
				this.FullVenueData = await new FullSnookerVenueDataHelper ().Load (VenueID);
				//System.Threading.Thread.Sleep(5000);

				Device.BeginInvokeOnMainThread(() =>
				{
					this.fill();

					App.Navigator.SetRootPageTitleToNormal ();
					if (this.LoadCompleted != null)
						this.LoadCompleted(this.FullVenueData != null);
				});
			}).Start ();
		}

		void clear()
		{
			this.panelAbout.Opacity = 0;
			this.panelWithLargeButtons.Opacity = 0;
			this.buttonNewGameHost.IsVisible = false;
		}

		void fill()
		{
			if (this.FullVenueData == null)
			{
				this.labelMetro.Text = "Couldn't load. Internet issues?";
				return;
			}

			this.panelAbout.Opacity = 1;
			this.panelWithLargeButtons.Opacity = 1;
			this.buttonNewGameHost.IsVisible = true;
			
			this.labelMetro.Text = string.IsNullOrEmpty(FullVenueData.Venue.MetroName) ? "-" : FullVenueData.Venue.MetroName;
			
			// about panel
			if (FullVenueData.Venue.IsInvalid)
			{
				this.panelInvalid.IsVisible = true;
				this.buttonPhoneNumber.Text = "-";
				this.buttonWebsite.Text = "-";
				this.buttonDirections.Text = "-";
			}
			else
			{
				this.panelInvalid.IsVisible = false;
				this.buttonPhoneNumber.Text = FullVenueData.Venue.HasPhoneNumber ? FullVenueData.Venue.PhoneNumber : "no phone #";
				string textWebsite = FullVenueData.Venue.HasWebsite ? FullVenueData.Venue.Website : "none";
				if (textWebsite.Length > 30)
					textWebsite = textWebsite.Substring(0, 30) + "...";
				this.buttonWebsite.Text = textWebsite;// "website" : "no website";
				this.buttonDirections.Text = "Directions";
			}
			this.label10ftTables.Text = FullVenueData.Venue.NumberOf10fSnookerTables != null ? FullVenueData.Venue.NumberOf10fSnookerTables.Value.ToString() : "?";
			this.label12ftTables.Text = FullVenueData.Venue.NumberOf12fSnookerTables != null ? FullVenueData.Venue.NumberOf12fSnookerTables.Value.ToString() : "?";

			// community
			if (FullVenueData.Venue.LastContributorID > 0)
			{
				this.panelVerified.IsVisible = true;
				this.panelNotVerifiedYet.IsVisible = false;
				this.labelVerifiedOn.Text = DateTimeHelper.DateToString(FullVenueData.Venue.LastContributorDate.Value);// FullVenueData.Venue.LastContributorDate.Value.ToShortDateString();
				this.labelVerifiedBy.Text = FullVenueData.Venue.LastContributorName;
			}
			else
			{
				this.panelNotVerifiedYet.IsVisible = true;
				this.panelVerified.IsVisible = false;
			}

			// map
			var position = new Xamarin.Forms.Maps.Position(FullVenueData.Venue.Latitude ?? 0, FullVenueData.Venue.Longitude ?? 0);
			double latlongdegrees = 0.01;
			map.MoveToRegion(new Xamarin.Forms.Maps.MapSpan(position, latlongdegrees, latlongdegrees));
			map.Pins.Clear();
			if (FullVenueData.Venue.Location != null)
			{
				var pin = new Pin
				{
					Type = PinType.Generic,
					Position = position,
					Label = this.FullVenueData.Venue.Name,
				};
				pin.Clicked += (s1, e1) => { App.Navigator.OpenMapsApp(FullVenueData.Venue.Location, FullVenueData.Venue.Name, FullVenueData.Venue.Address); };
				map.Pins.Add(pin);
			}

			this.fillVisibleControlsWithExistingData();
		}

        async void buttonEdit_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage(typeof(EditVenuePage)) != null)
				return;
			
            var page = new EditVenuePage(this.FullVenueData.Venue);
            await App.Navigator.NavPage.PushAsync(page);
        }

        void fillVisibleControlsWithExistingData()
        {
            if (this.FullVenueData == null)
                return;

            if (this.listOfBreaksControl.IsVisible && this.listOfBreaksControl.AllBreaks != FullVenueData.Breaks)
                this.listOfBreaksControl.Fill(FullVenueData.Breaks);
			if (this.listOfMatchesControl.IsVisible && this.listOfMatchesControl.AllMatches != FullVenueData.Matches)
			{
				this.listOfMatchesControl.Type = ListTypeEnum.Venue;//.IsForPrimaryAthlete = false;
				this.listOfMatchesControl.Fill (FullVenueData.Matches);
			}
            if (this.listOfPeopleControl.IsVisible && this.listOfPeopleControl.List != FullVenueData.People)
                this.listOfPeopleControl.Fill(FullVenueData.People);

            if (this.panelGameHosts.IsVisible)
            {
                if (this.listOfGameHostsControlFuture.List != FullVenueData.GameHostsInTheFuture)
                    this.listOfGameHostsControlFuture.Fill(FullVenueData.GameHostsInTheFuture);
                if (this.listOfGameHostsControlPast.List != FullVenueData.GameHostsInThePast)
                    this.listOfGameHostsControlPast.Fill(FullVenueData.GameHostsInThePast);
            }
        }
    }
}
