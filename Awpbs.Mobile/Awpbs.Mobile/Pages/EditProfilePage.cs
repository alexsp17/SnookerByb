using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace Awpbs.Mobile
{
	public class EditProfilePage : ContentPage
	{
        const string textShowAllMetros = "~ show all others ~";
		readonly double labelWidth1 = Config.IsTablet ? 100 : 70;
		const double rowHeight = 35;

        bool showCancelButton;
        Athlete athlete;
        List<Country> countries;
        List<MetroWebModel> metros;
        bool ignoreUIevents = false;

        Entry entryName;
        Picker pickerCountry;
        Picker pickerMetro;
        Entry entrySnookerAbout;
        Button buttonFind;
        Label labelStatus;
		Label labelPin;
		Label labelEmail;

        public event EventHandler<bool> UserClickedOkOrCancel;

		public EditProfilePage(bool showCancelButton, bool showSecurityPanel)
		{
            this.showCancelButton = showCancelButton;

			// name panel
			entryName = new BybStandardEntry()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				TextColor = Config.ColorBlackTextOnWhite,
				Placeholder = "(required)",
			};
			entryName.Completed += (s1, e1) => { entrySnookerAbout.Focus(); };
			var panelName = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = rowHeight,
				Spacing = 0,
				Padding = new Thickness(15, 5, 5, 5),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "Name", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
					entryName,
				}
			};

			// about panel
			entrySnookerAbout = new BybStandardEntry()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				TextColor = Config.ColorBlackTextOnWhite,
				Placeholder = "(optional)",
			};
			entrySnookerAbout.Completed += (s1, e1) =>
			{
				if (this.athlete.MetroID <= 0)
					buttonFind_Clicked(s1,e1);
			};
			var panelAbout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = rowHeight,
				Spacing = 0,
				Padding = new Thickness(15, 5, 5, 5),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "About", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
					entrySnookerAbout,
				}
			};

			// ask the phone
			this.buttonFind = new BybButton()
			{
				Text = "Ask the " + (Config.IsTablet ? "tablet" : "phone"),
				HorizontalOptions = LayoutOptions.Center,
				Style = (Style)App.Current.Resources["SimpleButtonStyle"],
			};
			buttonFind.Clicked += buttonFind_Clicked;
			labelStatus = new BybLabel() { VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite };

			// country panel
			Image imageCountry = new Image()
			{
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource() { File = "arrowRight.png" }
			};
			this.pickerCountry = new BybNoBorderPicker ()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Title = "Select country"
			};
			this.pickerCountry.SelectedIndexChanged += pickerCountry_SelectedIndexChanged;
			var panelCountry = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = rowHeight,
				Spacing = 0,
				Padding = new Thickness(15, 5, 5, 5),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "Country", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
					this.pickerCountry,
					imageCountry,
				}
			};
			panelCountry.GestureRecognizers.Add (new TapGestureRecognizer () { Command = new Command (() => { this.pickerCountry.Focus(); }) });

			// metro panel
			Image imageMetro = new Image()
			{
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource() { File = "arrowRight.png" }
			};
			this.pickerMetro = new BybNoBorderPicker()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Title = "Select city"
			};
			this.pickerMetro.SelectedIndexChanged += pickerMetro_SelectedIndexChanged;
			this.pickerCountry.SelectedIndexChanged += pickerCountry_SelectedIndexChanged;
			var panelMetro = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = rowHeight,
				Spacing = 0,
				Padding = new Thickness(15, 5, 5, 5),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "City", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
					this.pickerMetro,
					imageMetro,
				}
			};

			// pin panel
			this.labelPin = new BybLabel ()
			{
				Text = "Set your PIN code",
				TextColor = Color.Black,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center,
			};
			Image imagePin = new Image()
			{
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource() { File = "arrowRight.png" }
			};
			var panelPin = new StackLayout
			{
				IsVisible = showSecurityPanel,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = rowHeight,
				Spacing = 0,
				Padding = new Thickness(15, 5, 5, 5),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "PIN", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
					labelPin,
					imagePin,
				}
			};
			panelPin.GestureRecognizers.Add (new TapGestureRecognizer () { Command = new Command (() => { this.pinClicked(); }) });

			// password panel
			Label labelPassword = new BybLabel ()
			{
				Text = "Tap to reset",
				TextColor = Color.Black,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center,
			};
			Image imagePassword = new Image()
			{
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource() { File = "arrowRight.png" }
			};
			var panelPassword = new StackLayout
			{
				IsVisible = showSecurityPanel,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = rowHeight,
				Spacing = 0,
				Padding = new Thickness(15, 5, 5, 5),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "Password", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
					labelPassword,
					imagePassword,
				}
			};
			panelPassword.GestureRecognizers.Add (new TapGestureRecognizer () { Command = new Command (() => { this.passwordClicked(); }) });

			// e-mail panel
			labelEmail = new BybLabel ()
			{
				Text = "E-mail",
				TextColor = Color.Black,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center,
			};
			Image imageEmail = new Image()
			{
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource() { File = "arrowRight.png" }
			};
			var panelEmail = new StackLayout
			{
				IsVisible = showSecurityPanel,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = rowHeight,
				Spacing = 0,
				Padding = new Thickness(15, 5, 5, 5),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "E-mail", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
					labelEmail,
					imageEmail,
				}
			};
			panelEmail.GestureRecognizers.Add (new TapGestureRecognizer () { Command = new Command (() => { this.emailClicked(); }) });

            // ok, cancel
            Button buttonOk = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "OK" };
            Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
            buttonOk.Clicked += buttonOk_Clicked;
            buttonCancel.Clicked += buttonCancel_Clicked;
            if (showCancelButton == false)
                buttonCancel.IsVisible = false;

            var stackLayout = new StackLayout
            {
				Padding = new Thickness(0),
				Spacing = 0,
                Children = 
                {
                    new BybTitle(showCancelButton ? "Edit Profile" : "Your Profile") { VerticalOptions = LayoutOptions.Start },

                    new StackLayout
                    {
                        Padding = new Thickness(15,15,15,0),
						Spacing = 0,
                        Orientation = StackOrientation.Vertical,
                        Children = 
                        {
							panelName,
							panelAbout,

							new BoxView() { HeightRequest = 10, BackgroundColor = Config.ColorGrayBackground },
							new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 10,
                                Children =
                                {
                                    //buttonEditLocation,
                                    buttonFind,
                                    labelStatus
                                }
                            },

							panelCountry,
							panelMetro,

							new BoxView() { HeightRequest = 10, BackgroundColor = Config.ColorGrayBackground },
							panelPin,
							panelPassword,
							panelEmail,
                        }
                    },

                    new BoxView() { Style = (Style)App.Current.Resources["BoxViewPadding1Style"], VerticalOptions = LayoutOptions.FillAndExpand },

                    //new BoxView() { Style = (Style)App.Current.Resources["BoxViewPadding1Style"] },
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        //BackgroundColor = Config.ColorDarkGrayBackground,
                        HorizontalOptions = LayoutOptions.Fill,
                        HeightRequest = Config.OkCancelButtonsHeight,
                        Padding = Config.OkCancelButtonsPadding,
                        Spacing = 1,
                        Children =
                        {
                            buttonCancel,
                            buttonOk,
                        }
                    }
				}
            };

			this.Padding = new Thickness (0);
			this.BackgroundColor = Config.ColorGrayBackground;
			this.Content = stackLayout;

			this.fillCountriesPicker ();
		}

		void fillCountriesPicker()
		{
			countries = new List<Country>();
			foreach (var country in Country.List)
				if (country.CouldBeIgnored == false)
				{
					countries.Add(country);
					this.pickerCountry.Items.Add(country.Name);
				}
		}

        public async Task Init()
        {
            this.athlete = App.Repository.GetMyAthlete();

            this.entryName.Text = athlete.Name;
            this.entrySnookerAbout.Text = athlete.SnookerAbout;

            if (string.IsNullOrEmpty(this.athlete.RealEmail) == false)
            {
                this.labelEmail.Text = this.athlete.RealEmail;
            }
            else if (this.athlete.UserName != null)
            {
                if (this.athlete.UserName.Contains("@bestyourbest.com"))
                    this.labelEmail.Text = "Hidden, from Facebook";
                else
                    this.labelEmail.Text = this.athlete.UserName;
            }

            Country country = Country.Get(athlete.Country);
            int countryIndex = -1;
            if (country != null)
                countryIndex = this.countries.IndexOf(country);
            if (countryIndex >= 0)
            {
                this.ignoreUIevents = true;
                this.pickerCountry.SelectedIndex = this.countries.IndexOf(country);
                this.ignoreUIevents = false;
            }

            if (athlete.MetroID > 0)
                this.pickerMetro.Title = "Unchanged";

			await this.fillMetros();

			if (country == null) {
				var location = App.LocationService.GetLastKnownLocationQuickly ();
				if (location != null)
					await this.onLocationIdentified (location);
			}
        }

        async Task fillMetros()
        {
            if (this.pickerCountry.SelectedIndex < 0)
            {
                this.metros = null;
                this.pickerMetro.Items.Clear();
                return;
            }

			var country = countries[this.pickerCountry.SelectedIndex];
			//if (string.Compare(country.ThreeLetterCode, athlete.Country) != 0)
			//	this.pickerMetro.Title = "select your city";

			// load metros from webservice
            this.labelStatus.Text = "Loading cities...";
            this.metros = await App.WebService.GetMetros(country.ThreeLetterCode);
            if (metros == null || metros.Count == 0)
            {
                this.labelStatus.Text = "Couldn't load cities. Internet issues?";
                return;
            }

            this.labelStatus.Text = "";

			// fill metros picker
            this.pickerMetro.Items.Clear();
            foreach (var metro in metros)
                this.pickerMetro.Items.Add(metro.Name);

            var athleteMetro = this.metros.Where(i => i.ID == athlete.MetroID).FirstOrDefault();
            if (athleteMetro != null)
            {
                int index = metros.IndexOf(athleteMetro);
                this.pickerMetro.SelectedIndex = index;
            }
        }

        async void pickerCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ignoreUIevents)
                return;

            await this.fillMetros();
        }

        void pickerMetro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ignoreUIevents)
                return;

            if (pickerMetro.SelectedIndex >= 0 && pickerMetro.Items[pickerMetro.SelectedIndex] == textShowAllMetros)
            {
                Device.BeginInvokeOnMainThread(async () =>
                    {
                        await this.fillMetros();
                        this.pickerMetro.Focus();
                    });
            }
        }

		async Task onLocationIdentified(Location location)
		{
			this.metros = await App.WebService.GetMetrosNearby(location);
			if (metros == null || metros.Count == 0)
			{
				this.labelStatus.Text = "Couldn't find cities around. Internet issues?";
				return;
			}

			this.labelStatus.Text = "Thanks";

			// the country
			var country = Country.Get(metros[0].Country);
			int countryIndex = -1;
			if (country != null)
				countryIndex = countries.IndexOf(country);
			if (countryIndex < 0)
			{
				this.labelStatus.Text = "Hmmm... Unknown country.";
				return;
			}
			this.ignoreUIevents = true;
			this.pickerCountry.SelectedIndex = countryIndex;
			this.ignoreUIevents = false;

			this.ignoreUIevents = true;
			this.pickerMetro.Title = "";
			this.pickerMetro.Items.Clear();
			foreach (var metro in metros)
			{
				// skip metros from other countries
				Country thisMetroCountry = Country.Get(metro.Country);
				if (thisMetroCountry == null || thisMetroCountry.ThreeLetterCode != country.ThreeLetterCode)
					continue;

				this.pickerMetro.Items.Add(metro.Name);
			}
			this.pickerMetro.Items.Add(textShowAllMetros);
			this.pickerMetro.SelectedIndex = 0;
			this.ignoreUIevents = false;
		}

        void buttonFind_Clicked(object sender, EventArgs e)
        {
			this.labelStatus.Text = "Asking the " + (Config.IsTablet ? "tablet" : "phone") + "...";

            App.LocationService.RequestLocationAsync((s1, e1) =>
                {
					Device.BeginInvokeOnMainThread(async () =>
					{
	                    var location = App.LocationService.Location;
	                    if (location == null)
	                    {
					        this.labelStatus.Text = "The " + (Config.IsTablet ? "tablet" : "phone") + " didn't share your location.";
	                        return;
	                    }

						await this.onLocationIdentified(location);
					});
                }, true);
        }

        void buttonCancel_Clicked(object sender, EventArgs e)
        {
            this.UserClickedOkOrCancel(this, false);
        }

        async void buttonOk_Clicked(object sender, EventArgs e)
        {
            // name
            string athleteName = this.entryName.Text;
            if (athleteName == null)
                athleteName = "";
            athleteName = athleteName.Trim();
            if (athleteName.Length > 0)
                athleteName = athleteName.Substring(0, 1).ToUpper() + athleteName.Substring(1, athleteName.Length - 1);
            if (athleteName.Length < 3)
            {
                App.Navigator.DisplayAlertRegular("Please enter a proper name.");
                return;
            }

            // about
            string about = this.entrySnookerAbout.Text;
            if (about == null)
                about = "";
            about = about.Trim();
            if (about.Length > 1000)
                about = about.Substring(0, 1000);

            // country
            Country country = null;
            if (this.pickerCountry.SelectedIndex >= 0)
                country = countries[this.pickerCountry.SelectedIndex];
            if (country == null)
            {
                App.Navigator.DisplayAlertRegular("Please select your country.");
                return;
            }

            // metro
            int metroID;
            if (metros == null && this.athlete.MetroID > 0)
            {
                metroID = this.athlete.MetroID; // couldn't load metros, it's ok
            }
            else
            {
                MetroWebModel metro = null;
                if (this.pickerMetro.SelectedIndex >= 0)
                    metro = metros[this.pickerMetro.SelectedIndex];
                if (metro != null && metro.Country != country.ThreeLetterCode)
                    metro = null;
                if (metro == null)
                {
                    App.Navigator.DisplayAlertRegular("Please select your city. It doesn't have to be the city you live at, just a nearby city would do.");
                    return;
                }
                metroID = metro.ID;
            }

            // update local DB
            var athlete = App.Repository.GetMyAthlete();
            athlete.Name = athleteName;
            athlete.SnookerAbout = about;
            athlete.MetroID = metroID;
            if (country != null)
                athlete.Country = country.ThreeLetterCode;
            athlete.TimeModified = DateTimeHelper.GetUtcNow();
            App.Repository.UpdateAthlete(athlete);

            // done
            if (this.UserClickedOkOrCancel != null)
                this.UserClickedOkOrCancel(this, true);

            // update in the cloud
            if (App.LoginAndRegistrationLogic.RegistrationStatus == RegistrationStatusEnum.Registered)
                await App.WebService.SyncMyAthlete(athlete);
        }

		void emailClicked()
		{
			App.Navigator.DisplayAlertRegular ("This is the e-mail address Snooker Byb will send notifications to. This is also your user name. If you'd like to change it, please feel free to contact us at team@snookerbyb.com");
		}

		void passwordClicked()
		{
			App.Navigator.OpenBrowserApp ("https://www.snookerbyb.com/account/forgotpassword");
		}

		void pinClicked()
		{
            App.LoginAndRegistrationLogic.AskUserToEnterPin(true, false,
                (string pin) =>
                {
                    changePin(pin);
                },
                () =>
                {
                    // do nothing on cancel
                });
        }

        async void changePin(string newPin)
        {
            PleaseWaitPage pleaseWaitPage = new PleaseWaitPage();
            await App.Navigator.NavPage.Navigation.PushModalAsync(pleaseWaitPage);

            var result = await App.WebService.ChangePin(newPin);

            await App.Navigator.NavPage.Navigation.PopModalAsync();

            if (result == null)
            {
                await App.Navigator.NavPage.DisplayAlert("Byb", "Couldn't update PIN. Internet issues?", "OK");
            }
            else if (result.PinChanged == false)
            {
                await App.Navigator.NavPage.DisplayAlert("Byb", "Error. Couldn't update PIN.", "OK");
            }
            else
            {
                this.labelPin.Text = "X X X X";

                // remember the new PIN
                App.KeyChain.PinCode = newPin;

                // remember the new PIN as the new password
                if (result.PasswordChanged)
                {
                    App.KeyChain.Add(athlete.UserName, newPin);
                }

                if (result.PasswordChanged == true)
                {
                    await App.Navigator.NavPage.DisplayAlert("Byb", "Done. Your password and your PIN are now updated. Use this PIN on public Snooker Byb scoreboards to access your account.", "OK");
                    await App.LoginAndRegistrationLogic.ShowReloginPage();
                }
                else
                {
                    await App.Navigator.NavPage.DisplayAlert("Byb", "Done. Use this PIN on public Snooker Byb scoreboards to access your account.", "OK");
                    await App.LoginAndRegistrationLogic.ShowReloginPage();
                }
            }
        }
	}
}
