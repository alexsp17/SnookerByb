using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class NewVenuePage : ContentPage
	{
        public event EventHandler UserClickedCanceled;
        public event EventHandler<VenueWebModel> VenueCreated;

        Location location;

		Switch switchAll;
        Button buttonNotListed;
        Button buttonCheckLocationAgain;
        StackLayout panelInfo;
        StackLayout panelList;
        Label labelInfo;

        public NewVenuePage()
		{
            this.BackgroundColor = Config.ColorGrayBackground;

            // buttons
            Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Cancel" };
            buttonCancel.Clicked += (s1, e1) =>
            {
                if (this.UserClickedCanceled != null)
                    this.UserClickedCanceled(this, EventArgs.Empty);
            };
            this.buttonNotListed = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "It's still not listed" };
            buttonNotListed.Clicked += buttonNotListed_Clicked;
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Spacing = 1,
                Children =
                {
                    buttonCancel,
                    buttonNotListed,
                }
            };

            // info panel
            labelInfo = new BybLabel() { TextColor = Config.ColorBlackTextOnWhite };
            panelInfo = new StackLayout()
            {
                Padding = new Thickness(20,30,20,20),
                Children =
                {
                    labelInfo
                }
            };

            buttonCheckLocationAgain = new BybButton() { Text = "Check location again", Style = (Style)App.Current.Resources["SimpleButtonStyle"] };
            buttonCheckLocationAgain.IsVisible = false;
            buttonCheckLocationAgain.Clicked += buttonCheckLocationAgain_Clicked;

			this.switchAll = new Switch () { };
			this.switchAll.Toggled += switchAll_Toggled;;

            // list panel
            panelList = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(10,10,10,0),
            };

			Content = new StackLayout {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0),
                Spacing = 0,
				Children = {
                    new BybTitle("Register New Venue") { VerticalOptions = LayoutOptions.Start },
                    new ScrollView
                    {
                        BackgroundColor = Config.ColorGrayBackground,
                        Padding = new Thickness(0),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Content = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Padding = new Thickness(0),
                            Spacing = 0,
                            Children = 
                            {
                                new StackLayout
                                {
                                    Padding = new Thickness(0,10,0,10),
                                    Children = 
                                    {
                                        new StackLayout
                                        {
                                            Orientation = StackOrientation.Vertical,
                                            Padding = new Thickness(20,0,20,0),
                                            Children = 
                                            {
                                                new BybLabel { Text = "Tap on an item in the list to register it as a snooker venue.", TextColor = Config.ColorBlackTextOnWhite },
												new StackLayout()
												{
													Orientation = StackOrientation.Horizontal,
													Children = 
													{
														this.switchAll,
														new BybLabel() { Text = "Show more places", VerticalOptions = LayoutOptions.Center, TextColor = Config.ColorBlackTextOnWhite }
													}
												}
                                            }
                                        },
                                        panelInfo,
                                        buttonCheckLocationAgain,
                                        panelList
                                    }
                                }
                            }
                        }
                    },

                    panelOkCancel
				}
			};
            Padding = new Thickness(0);
		}

        async void switchAll_Toggled (object sender, ToggledEventArgs e)
        {
			await this.queryPOIs(this.switchAll.IsToggled == true);
        }

        void buttonCheckLocationAgain_Clicked(object sender, EventArgs e)
        {
            requestLocation();
        }

        async void buttonNotListed_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage (typeof(NewVenue2Page)) != null)
				return;
			
			var page = new NewVenue2Page ();
			page.VenueCreated += (s1, venue) => {
				if (this.VenueCreated != null)
					this.VenueCreated(this, venue);
			};
			page.UserClickedCanceled += async (s1, e1) =>
			{
				await App.Navigator.NavPage.Navigation.PopModalAsync();
				await App.Navigator.NavPage.Navigation.PopModalAsync();
			};
			await App.Navigator.NavPage.Navigation.PushModalAsync (page);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.requestLocation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        void requestLocation()
        {
            this.buttonCheckLocationAgain.IsVisible = false;
            this.panelInfo.IsVisible = true;
            this.panelList.IsVisible = false;
            this.labelInfo.Text = "Please wait. Getting your location...";

            App.LocationService.RequestLocationAsync(async (s1, e1) =>
            {
                this.location = App.LocationService.Location;
                if (location == null)
                {
                    this.buttonCheckLocationAgain.IsVisible = true;
                    this.panelInfo.IsVisible = true;
                    this.panelList.IsVisible = false;
                    this.labelInfo.Text = "Error. We have to know your location to let you register a new venue. If you disallowed Snooker Byb to access your location, please change settings.";
                    return;
                }

                this.buttonCheckLocationAgain.IsVisible = false;

                await queryPOIs(false);

            }, true);
        }

        async Task queryPOIs(bool secondTry)
        {
			try
			{
				if (this.location == null)
					return;
				
	            this.buttonCheckLocationAgain.IsVisible = false;
	            this.panelInfo.IsVisible = true;
	            this.panelList.IsVisible = false;
	            this.labelInfo.Text = "Quering the Internet for venues around you...";

	            var pois = await App.WebService.FindPOIs(location, Distance.FromMeters(1600), secondTry ? "" : "snooker");

	            if (pois == null)
	            {
	                this.labelInfo.Text = "Error. Internet issues?";
	                this.buttonCheckLocationAgain.IsVisible = true;
	                this.buttonCheckLocationAgain.Text = "Try again";
	                return;
	            }

	            if (pois.Count == 0)
	            {
	                if (secondTry == false)
	                {
	                    await queryPOIs(true);
	                    return;
	                }
	                else
	                {
	                    this.labelInfo.Text = "No 'places' found around you. Hm... strange.";
	                    this.buttonCheckLocationAgain.IsVisible = true;
	                    this.buttonCheckLocationAgain.Text = "Try again";
	                    return;
	                }
	            }

	            this.buttonCheckLocationAgain.IsVisible = false;
	            this.panelInfo.IsVisible = false;
	            this.panelList.IsVisible = true;

	            pois = (from poi in pois
	                    orderby poi.Distance.Meters
	                    select poi).ToList();

	            foreach (var poi in pois)
	            {
	                var stackPOI = new StackLayout()
	                {
	                    Orientation = StackOrientation.Vertical,
						BackgroundColor = Color.White,
	                    Spacing = 1,
	                    Children =
	                    {
	                        new StackLayout
	                        {
	                            Orientation = StackOrientation.Horizontal,
	                            //BackgroundColor = Color.White,
	                            Padding = new Thickness(0,10,0,5),
	                            Spacing = 0,
	                            Children =
	                            {
	                                new StackLayout
	                                {
	                                    Padding = new Thickness(10,0,0,0),
	                                    Orientation = StackOrientation.Vertical,
	                                    HorizontalOptions = LayoutOptions.FillAndExpand,
	                                    VerticalOptions = LayoutOptions.Start,
	                                    Children = 
	                                    {
	                                        new BybLabel { Text = poi.Name, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold, TextColor = Config.ColorBlackTextOnWhite },
	                                        new BybLabel { Text = poi.Distance.ToString(), HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
	                                        new BybLabel { Text = poi.Address, TextColor = Config.ColorBlackTextOnWhite },
	                                    }
	                                },
	                            }
	                        },
	                    }
	                };
	                panelList.Children.Add(stackPOI);

	                stackPOI.GestureRecognizers.Add(new TapGestureRecognizer
	                {
	                    Command = new Command(() =>
	                    {
	                        createVenue(poi);
	                    }),
	                    NumberOfTapsRequired = 1
	                });
	            }
			}
			catch (Exception)
			{
			}
        }

        async void createVenue(POIWebModel poi)
        {
            this.panelInfo.IsVisible = true;
            this.panelList.IsVisible = false;
            this.labelInfo.Text = "Please wait. Registering the venue.";

            VenueWebModel venue = new VenueWebModel();
            venue.Address = poi.Address;
            venue.Country = "";
            venue.IsSnooker = true;
            venue.Latitude = poi.Location.Latitude;
            venue.Longitude = poi.Location.Longitude;
            venue.Name = poi.Name;
            venue.PhoneNumber = poi.Phone;
            venue.Website = poi.Website;

            venue.ID = await App.WebService.CreateNewVenue(venue);
            if (venue.ID == 0)
            {
                this.labelInfo.Text = "Couldn't register the venue. Internet issues? Venue already exists?";
                return;
            }

            if (this.VenueCreated != null)
                this.VenueCreated(this, venue);
        }
	}
}
