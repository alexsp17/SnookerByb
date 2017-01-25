using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XLabs.Forms.Controls;

namespace Awpbs.Mobile
{
	public class EditVenuePage : ContentPage
	{
        public VenueWebModel Venue { get; set; }

        Switch switchValidVenue;
		Entry entryPhone;
        Entry entryWebsite;
        Entry entryAddress;
        Picker picker10fTables;
        Picker picker12fTables;

        Map map;

        Button buttonPOIs;
        Label labelPOIsWaiting;
        Button buttonPhone;
        Button buttonWebsite;

        Button buttonOk;

		public EditVenuePage(VenueWebModel venue)
		{
            this.Venue = venue;
            this.Title = "Edit / Verify Venue";
            this.BackgroundColor = Color.White;

			double pickerHeight = Config.IsIOS ? 20 : 35;//Config.DefaultFontSize * 1.6;
			double buttonHeight = Config.IsIOS ? 20 : 35;
			double buttonWidth = (Config.IsAndroid ? 90 : 70) + (Config.IsTablet ? 30 : 0);

            switchValidVenue = new Switch() { };
            switchValidVenue.IsToggled = !venue.IsInvalid;
            switchValidVenue.Toggled += (s1, e1) => { this.updateReadonly(); };

			entryPhone = new BybStandardEntry() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Center, TextColor = Config.ColorBlackTextOnWhite, Placeholder = "phone #" };
            if (venue.HasPhoneNumber)
                entryPhone.Text = venue.PhoneNumber;
            entryPhone.Keyboard = Keyboard.Telephone;
			this.buttonPhone = new BybButton() { Text = "call", HeightRequest = buttonHeight, WidthRequest = buttonWidth, VerticalOptions = LayoutOptions.Center, Style = (Style)App.Current.Resources["SimpleButtonStyle"] };
            buttonPhone.Clicked += (s1, e1) =>
            {
                if (this.entryPhone.Text.Trim() != "")
                    App.Navigator.OpenPhoneCallApp(this.entryPhone.Text);
            };

			entryWebsite = new BybStandardEntry() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Center, Placeholder = "website, optional" };
            if (venue.HasWebsite)
                entryWebsite.Text = venue.Website;
			this.buttonWebsite = new BybButton() { Text = "browse", HeightRequest = buttonHeight, WidthRequest = buttonWidth, VerticalOptions = LayoutOptions.Center, Style = (Style)App.Current.Resources["SimpleButtonStyle"], HorizontalOptions = LayoutOptions.End };
            buttonWebsite.Clicked += (s1, e1) =>
            {
                if (this.entryWebsite.Text.Trim() != "")
                    App.Navigator.OpenBrowserApp(this.entryWebsite.Text);
            };

			entryAddress = new BybStandardEntry() { HorizontalOptions = LayoutOptions.Fill, Placeholder = "address, optional" };
            if (venue.HasAddress)
                entryAddress.Text = venue.Address;

            double height_MapAndTables = Config.IsTablet ? 180 : 130;
            this.map = new Xamarin.Forms.Maps.Map()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsShowingUser = true,
            };

			picker10fTables = new BybWithBorderPicker () { Title = "Set number of 10' tables", HeightRequest = pickerHeight };
			picker12fTables = new BybWithBorderPicker() { Title = "Set number of 12' tables", HeightRequest = pickerHeight };
            for (int i = 0; i < 40; ++i)
            {
                picker10fTables.Items.Add(i.ToString());
                picker12fTables.Items.Add(i.ToString());
            }
            if (venue.NumberOf10fSnookerTables != null)
                picker10fTables.SelectedIndex = venue.NumberOf10fSnookerTables.Value;
            if (venue.NumberOf12fSnookerTables != null)
                picker12fTables.SelectedIndex = venue.NumberOf12fSnookerTables.Value;

            this.buttonPOIs = new BybButton() { Text = "query the Internet", HorizontalOptions = LayoutOptions.Start, HeightRequest = buttonHeight, Style = (Style)App.Current.Resources["SimpleButtonStyle"] };
            this.buttonPOIs.Clicked += buttonPOIs_Clicked;
			this.labelPOIsWaiting = new BybLabel() { Text = "loading...", HorizontalOptions = LayoutOptions.Start, HeightRequest = buttonHeight, IsVisible = false, TextColor = Config.ColorBlackTextOnWhite };

            this.buttonOk = new BybButton { Text = "Yep, I Confirm", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            var buttonCancel = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
            buttonOk.Clicked += buttonOk_Clicked;
            buttonCancel.Clicked += buttonCancel_Clicked;

//            this.buttonConfirm = new BybButton() { Text = "Yes, this information is correct", Style = (Style)App.Current.Resources["SimpleButtonStyle"], TextColor = Config.ColorTextOnBackground, HeightRequest = buttonHeight };
//            this.buttonConfirm.Clicked += (s, e) =>
//            {
//                this.buttonConfirm.Text = "Fellow snooker players say 'thank you'";
//                this.buttonConfirm.TextColor = Config.ColorTextOnBackgroundGrayed;
//                this.updateOkButton(true);
//            };

            var aboutGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 0, 0, 0),
                ColumnSpacing = 0,
                RowSpacing = 3,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(buttonWidth, GridUnitType.Absolute) },
                }
            };
            aboutGrid.Children.Add(entryPhone, 0, 0);
            aboutGrid.Children.Add(buttonPhone, 1, 0);
            aboutGrid.Children.Add(entryWebsite, 0, 1);
            aboutGrid.Children.Add(buttonWebsite, 1, 1);
            aboutGrid.Children.Add(entryAddress, 0, 2, 2, 3);
            aboutGrid.Children.Add(new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = 
                    {
					new BybLabel { Text = "Valid snooker venue", VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite },
                        switchValidVenue,
                    }
                }, 0, 3);

			var theStackLayout = new StackLayout {
                Orientation = StackOrientation.Vertical,
                Spacing = 10,
				Children = {

                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Spacing = 3,
                        Padding = new Thickness(10,10,10,0),
                        Children = 
                        {
							new BybLabel { Text = venue.Name, FontAttributes = FontAttributes.Bold, TextColor = Config.ColorBlackTextOnWhite },
							new BybLabel { Text = venue.MetroName ?? "", TextColor = Config.ColorBlackTextOnWhite },
                        }
                    },

					new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(0),
                        HeightRequest = height_MapAndTables,
                        Children =
                        {
                            map,
                            new StackLayout
                            {
                                Orientation = StackOrientation.Vertical,
                                Spacing = 3,
                                Padding = new Thickness(10,0,10,0),
                                WidthRequest = 60,
                                VerticalOptions = LayoutOptions.CenterAndExpand,
                                Children = 
                                {
									new BybLabel { Text = "10' tables", VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Config.ColorBlackTextOnWhite },
                                    picker10fTables,
									new BybLabel { Text = "12' tables", VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Config.ColorBlackTextOnWhite },
                                    picker12fTables,
                                }
                            },
                        }
                    },

                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Spacing = 3,
                        Padding = new Thickness(10,10,10,0),
                        Children = 
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Children = 
                                {
									new BybLabel { Text = "About the venue:", HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand, TextColor = Config.ColorBlackTextOnWhite },
                                    buttonPOIs,
                                    labelPOIsWaiting
                                }
                            },
                        }
                    },

                    aboutGrid
				}
			};
            Content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Children = 
                {
                    new ScrollView()
                    {
                        Padding = new Thickness(0),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Content = theStackLayout
                    },

                    new BoxView() { Style = (Style)App.Current.Resources["BoxViewPadding1Style"], VerticalOptions = LayoutOptions.FillAndExpand, HeightRequest = 2 },

                    new StackLayout()
                    {
                        Orientation = StackOrientation.Vertical,
                        BackgroundColor = Config.ColorBlackBackground,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Padding = new Thickness(0,0,0,0),
                        Children =
                        {
//                            new StackLayout
//                            {
//                                Orientation = StackOrientation.Horizontal,
//                                HorizontalOptions = LayoutOptions.Center,
//                                VerticalOptions = LayoutOptions.Center,
//                                Padding = new Thickness(0,15,0,5),
//                                Children = 
//                                {
//                                    buttonConfirm
//                                }
//                            },
                            new StackLayout
                            {
								Spacing = Config.SpaceBetweenButtons,
								Padding = new Thickness(10,10,10,10),
                                Orientation = StackOrientation.Horizontal,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                HeightRequest = Config.OkCancelButtonsHeight,
                                //Padding = new Thickness(Config.OkCancelButtonsPadding),
                                Children = 
                                {
                                    buttonCancel,
                                    buttonOk,
                                }
                            }
                        }
                    }
                }
            };

            // update the map
            var position = new Xamarin.Forms.Maps.Position(venue.Latitude ?? 0, venue.Longitude ?? 0);
            double latlongdegrees = 0.005;
            map.MoveToRegion(new Xamarin.Forms.Maps.MapSpan(position, latlongdegrees, latlongdegrees));
            map.Pins.Clear();
            if (venue.Location != null)
            {
                var pin = new Pin
                {
                    Type = PinType.Generic,
                    Position = position,
                    Label = this.Venue.Name,
                };
                pin.Clicked += (s1, e1) => { App.Navigator.OpenMapsApp(Venue.Location, Venue.Name, Venue.Address); };
                map.Pins.Add(pin);
            }

            //updateOkButton(false);
            updateReadonly();
		}

//        void updateOkButton(bool enable)
//        {
//            if (enable)
//            {
//                buttonOk.IsEnabled = true;
//                buttonOk.Opacity = 1.0;
//            }
//            else
//            {
//                buttonOk.IsEnabled = false;
//                buttonOk.Opacity = 0.5;
//            }
//        }

        void updateReadonly()
        {
            bool isEnabled = this.switchValidVenue.IsToggled;

            this.picker10fTables.IsEnabled = isEnabled;
            this.picker12fTables.IsEnabled = isEnabled;
            this.map.IsEnabled = isEnabled;
            this.buttonPOIs.IsEnabled = isEnabled;
            this.buttonPhone.IsEnabled = isEnabled;
            this.buttonWebsite.IsEnabled = isEnabled;
            this.entryPhone.IsEnabled = isEnabled;
            this.entryAddress.IsEnabled = isEnabled;
            this.entryWebsite.IsEnabled = isEnabled;
        }

        async void buttonPOIs_Clicked(object sender, EventArgs e)
        {
            if (Venue.Location == null)
                return;

            this.buttonPOIs.IsVisible = false;
            this.labelPOIsWaiting.IsVisible = true;

            List<POIWebModel> pois = new List<POIWebModel>();
            string keyword = Venue.Name;
            pois = await App.WebService.FindPOIs(Venue.Location, Distance.FromMeters(250), keyword);

            this.buttonPOIs.IsVisible = true;
            this.labelPOIsWaiting.IsVisible = false;

            if (pois == null)
            {
                await DisplayAlert("Byb", "Internet issues?", "OK");
                return;
            }

            if (pois.Count == 0)
            {
                await DisplayAlert("Byb", "Nothing found nearby. Consider marking this venue as 'invalid' and creating another record for '" + Venue.Name + "'", "OK");
                return;
            }

            pois = pois.Take(100).ToList();

            var strs = pois.Select(i => i.Name).ToList();

            string str = await DisplayActionSheet("Pick where to take the info from", "Cancel", null, strs.ToArray());

            if (string.IsNullOrEmpty(str))
                return;

            int index = strs.IndexOf(str);
            if (index < 0)
                return;
            var poi = pois[index];

            this.entryPhone.Text = poi.Phone;
            this.entryAddress.Text = poi.Address;
            this.entryWebsite.Text = poi.Website;
        }

        void buttonCancel_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.PopAsync();
        }

        async void buttonOk_Clicked(object sender, EventArgs e)
        {
			if (await this.DisplayAlert ("Byb", "Information correct?", "Yes, correct", "Cancel") != true)
				return;
			
            VenueEditWebModel edit = new VenueEditWebModel();
            edit.VenueID = Venue.ID;
            if (this.picker10fTables.SelectedIndex >= 0)
                edit.NumberOf10fSnookerTables = this.picker10fTables.SelectedIndex;
            if (this.picker12fTables.SelectedIndex >= 0)
                edit.NumberOf12fSnookerTables = this.picker12fTables.SelectedIndex;
            edit.PhoneNumber = this.entryPhone.Text;
            edit.Website = this.entryWebsite.Text;
            edit.Address = this.entryAddress.Text;
            edit.IsInvalid = !this.switchValidVenue.IsToggled;

            PleaseWaitPage waitPage = new PleaseWaitPage();
            await App.Navigator.NavPage.Navigation.PushModalAsync(waitPage);

            bool ok = await App.WebService.VerifyOrEditVenue(edit);

            await App.Navigator.NavPage.Navigation.PopModalAsync();

            if (ok == false)
            {
                await DisplayAlert("Byb", "Couldn't post this to the web service. Internet issues?", "OK");
                return;
            }

            // update the venue in the Cache
            var updatedVenue = await App.WebService.GetVenue(Venue.ID);
            App.Cache.Venues.Put(updatedVenue);

            // let other pages know
            App.Navigator.DoOnVenueEdited(Venue.ID);
        }
	}
}
