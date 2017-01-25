using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Awpbs.Mobile
{
	public class NewVenue2Page : ContentPage
	{
		public event EventHandler UserClickedCanceled;
		public event EventHandler<VenueWebModel> VenueCreated;

		BybStandardEntry entryAddress;
		BybStandardEntry entryName;
		Map map;

		System.Timers.Timer timer;
		Location prevLocation = null;
		Pin pin;
		bool pinIsInAddressLocation = false;
		Location addressLocation = null;
		DateTime omitTimerUntilThisTime = DateTime.MinValue;

		public NewVenue2Page()
		{
			this.BackgroundColor = Config.ColorGrayBackground;// Config.ColorBackground;

			// buttons
			Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
			buttonCancel.Clicked += (s1, e1) =>
			{
				if (this.UserClickedCanceled != null)
					this.UserClickedCanceled(this, EventArgs.Empty);
			};
			Button buttonOk = new BybButton { Style = (Style)App.Current.Resources ["LargeButtonStyle"], Text = "OK" };
			buttonOk.Clicked += async (s1, e1) =>
			{
				await createVenue();
			};
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
					buttonOk,
				}
			};

			map = new Map () {
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			entryName = new BybStandardEntry () {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Placeholder = "enter venue name here",
			};

			entryAddress = new BybStandardEntry () {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Placeholder = "optionally - enter address here to move the map",
			};
			entryAddress.Completed += entryAddress_Completed;

			Content = new StackLayout {
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(0),
				Spacing = 0,
				Children = {
					new BybTitle("Register New Venue") { VerticalOptions = LayoutOptions.Start },

					new StackLayout()
					{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(10,10,10,10),
						Spacing = 10,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Children = 
						{
							new BybLabel() { Text = "Venue name:", VerticalOptions = LayoutOptions.Center, TextColor = Config.ColorBlackTextOnWhite },
							entryName,
						}
					},

                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(10,0,10,10),
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            entryAddress
                        }
                    },

                    map,

					panelOkCancel
				}
			};
			Padding = new Thickness(0);
		}

		async void entryAddress_Completed (object sender, EventArgs e)
		{
			string address = this.entryAddress.Text;
			if (address == null)
				address = "";
			if (address.Length < 3)
				return;

			Location location = await App.WebService.LocationFromAddress (address, prevLocation);

			if (location == null) {
				await DisplayAlert ("Byb", "Unknown address", "OK");
				return;
			}

			this.omitTimerUntilThisTime = DateTime.Now.AddSeconds (2);
			this.pinIsInAddressLocation = false;
			this.addressLocation = location;
			this.moveMapTo(location);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			this.requestLocation();

			timer = new System.Timers.Timer ();
			timer.Interval = 1000;
			timer.Start ();
			timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => 
			{
				if (timer == null)
					return;
				Device.BeginInvokeOnMainThread(() =>
					{
						if (DateTime.Now < this.omitTimerUntilThisTime)
							return;
						moveThePin();
					});
			};
		}

		protected override void OnDisappearing()
		{
			timer.Close ();
			timer = null;
			base.OnDisappearing();
		}

		void moveThePin()
		{
			var visibleRegion = map.VisibleRegion;
			if (visibleRegion == null)
				return;

			Location newLocation = new Location(visibleRegion.Center.Latitude, visibleRegion.Center.Longitude);
			if (prevLocation != null && System.Math.Abs(Distance.Calculate(newLocation, prevLocation).Meters) < 100)
				return; // location didn't change
			if (this.pinIsInAddressLocation && addressLocation != null && System.Math.Abs (Distance.Calculate (newLocation, addressLocation).Meters) < 1000)
				return; // location didn't change

			prevLocation = newLocation;

			if (pin != null)
				map.Pins.Remove (pin);

			pin = new Pin();
			pin.Label = "New Venue";
			pin.Position = new Position(newLocation.Latitude, newLocation.Longitude);
			map.Pins.Add(pin);

			if (addressLocation != null)
				this.pinIsInAddressLocation = System.Math.Abs (Distance.Calculate (newLocation, addressLocation).Meters) < 100;
			else
				this.pinIsInAddressLocation = false;

			if (this.pinIsInAddressLocation == false && addressLocation != null) {
				this.entryAddress.Text = "";
				this.addressLocation = null;
			}
		}

		void requestLocation()
		{
			App.LocationService.RequestLocationAsync((s1, e1) =>
				{
					var location = App.LocationService.Location;
					if (location == null)
						return;

					this.moveMapTo(location);
				}, true);
		}

		void moveMapTo(Location location)
		{
			double radiusInMeters = 5000;
			var loc2 = location.OffsetRoughly(radiusInMeters, radiusInMeters);
			double lat = System.Math.Abs(location.Latitude - loc2.Latitude);
			double lon = System.Math.Abs(location.Longitude - loc2.Longitude);
			this.map.MoveToRegion(new MapSpan(new Position(location.Latitude, location.Longitude), lat, lon));
		}

		async Task createVenue()
		{
			string name = this.entryName.Text;
			if (name == null)
				name = "";
			name = name.Trim ();
			string address = "";
			if (addressLocation != null && this.entryAddress.Text != null)
				address = this.entryAddress.Text.Trim ();
			Location location = prevLocation;

			if (name.Length < 3) {
				await App.Navigator.NavPage.DisplayAlert ("Byb", "Please enter a proper name for the venue.", "OK");
				return;
			};

			VenueWebModel venue = new VenueWebModel();
			venue.Address = address;
			venue.Country = "";
			venue.IsSnooker = true;
			venue.Latitude = location.Latitude;
			venue.Longitude = location.Longitude;
			venue.Name = name;
			venue.PhoneNumber = "";
			venue.Website = "";

			venue.ID = await App.WebService.CreateNewVenue(venue);
			if (venue.ID == 0)
			{
				await App.Navigator.NavPage.DisplayAlert("Byb", "Couldn't register the venue. Internet issues?", "OK");
				return;
			}

			await App.Navigator.NavPage.Navigation.PopModalAsync ();

			if (this.VenueCreated != null)
				this.VenueCreated(this, venue);
		}
	}
}
