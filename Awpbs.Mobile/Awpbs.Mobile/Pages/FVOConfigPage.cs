using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class FVOConfigPage : ContentPage
	{
        public bool ChangesMade { get; set; }

        Label labelVenue;
        BybNoBorderPicker pickerTableType;
        BybNoBorderPicker pickerNotableBreakThreshold;
        Label labelAdmin;
        Entry entryTableDescription;

        FVOConfig config;

        List<int> notableBreakThresholds = new List<int>() { 20, 25, 30, 35, 40, 50, 60, 70, 80, 90, 100 };

        public FVOConfigPage()
        {
            this.BackgroundColor = Config.ColorGrayBackground;
            double labelWidth1 = 160;

            // venue
            this.labelVenue = new BybLabel() { FontAttributes = FontAttributes.Bold, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center };
            Image imageVenue = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 20,
                HeightRequest = 20,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            var panelVenue = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Config.LargeButtonsHeight,
                Spacing = 0,
                Padding = new Thickness(15, 0, 15, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Venue", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    labelVenue,
                    imageVenue,
                }
            };
            panelVenue.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.buttonVenue_Clicked(); })
            });

			// table type
			this.pickerTableType = new BybNoBorderPicker() { HorizontalOptions = LayoutOptions.FillAndExpand };
            this.pickerTableType.Items.Add("10'");
            this.pickerTableType.Items.Add("12'");
			Image imageTableType = new Image()
			{
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = 20,
				HeightRequest = 20,
				Source = new FileImageSource() { File = "arrowRight.png" }
			};
			var panelTableType = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = Config.LargeButtonsHeight,
				Spacing = 0,
				Padding = new Thickness(15, 0, 15, 0),
				BackgroundColor = Color.White,
				Children =
				{
                    new BybLabel { Text = "Table size", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    pickerTableType,
                    imageTableType,
				}
			};
            panelTableType.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.pickerTableType.Focus(); })
            });

            // notable break threshold
            this.pickerNotableBreakThreshold = new BybNoBorderPicker() { HorizontalOptions = LayoutOptions.FillAndExpand };
            foreach (int threshold in notableBreakThresholds)
                this.pickerNotableBreakThreshold.Items.Add(threshold.ToString());
            Image imageNotableBreakThreshold = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 20,
                HeightRequest = 20,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            var panelNotableBreakThreshold = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                //HeightRequest = Config.LargeButtonsHeight,
                Spacing = 0,
                Padding = new Thickness(15, 0, 15, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Notable break threshold", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    pickerNotableBreakThreshold,
                    imageNotableBreakThreshold,
                }
            };
            panelNotableBreakThreshold.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.pickerNotableBreakThreshold.Focus(); })
            });

            // table description
            this.entryTableDescription = new BybNoBorderEntry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Placeholder = "optional",
            };
            var panelTableDescription = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Config.LargeButtonsHeight,
                Spacing = 0,
                Padding = new Thickness(15, 0, 15, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Table description", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    entryTableDescription,
                }
            };

            // admi
            this.labelAdmin = new BybLabel() { FontAttributes = FontAttributes.Bold, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center };
            Image imageAdmin = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 20,
                HeightRequest = 20,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            var panelAdmin = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Config.LargeButtonsHeight,
                Spacing = 0,
                Padding = new Thickness(15, 0, 15, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Admin", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    labelAdmin,
                    imageAdmin,
                }
            };
            panelAdmin.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    this.DisplayAlert("Byb", "To change the administrator - re-install the app.", "OK");
                })
            });

            // buttons
            Button buttonOk = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Ok" };
            buttonOk.Clicked += buttonOk_Clicked;
            Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
            buttonCancel.Clicked += buttonCancel_Clicked;

            var stackLayout = new StackLayout
            {
                Spacing = 0,
                Padding = new Thickness(0),
                BackgroundColor = Config.ColorGrayBackground,

                Children =
                {
                    new BybTitle("Settings") { VerticalOptions = LayoutOptions.Start },

                    new StackLayout()
                    {
                        Orientation = StackOrientation.Vertical,
                        WidthRequest = 500,
                        HorizontalOptions = LayoutOptions.Center,
                        Padding = new Thickness(20),
                        Spacing = 5,
                        Children =
                        {
                            panelVenue,
                            panelTableType,
                            panelTableDescription,
                            panelNotableBreakThreshold,
                            panelAdmin,

                            new StackLayout()
                            {
                                Spacing = 1,
                                Orientation = StackOrientation.Horizontal,
                                HorizontalOptions = LayoutOptions.Fill,
                                HeightRequest = Config.OkCancelButtonsHeight,
                                Padding = new Thickness(0,10,0,0),
                                Children =
                                {
                                    buttonCancel,
                                    buttonOk,
                                }
                            }
                        }
                    },
                }
            };

            this.Content = stackLayout;
            this.Padding = new Thickness(0, 0, 0, 0);

            this.fill();
		}

        async void buttonVenue_Clicked()
        {
			if (App.Navigator.GetOpenedPage (typeof(PickVenuePage)) != null)
				return;
			
            var pickVenuePage = new PickVenuePage();
            pickVenuePage.UserMadeSelection += (s1, venue) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();

                if (venue != null)
                {
                    config.VenueID = venue.ID;
                    config.VenueName = venue.Name;
                    this.labelVenue.Text = venue.Name;
                }
            };
            await App.Navigator.NavPage.Navigation.PushModalAsync(pickVenuePage);
        }

        private async void buttonOk_Clicked(object sender, EventArgs e)
        {
            ChangesMade = true;

            config.TableSize = SnookerTableSizeEnum.Table12Ft;
            if (this.pickerTableType.SelectedIndex == 0)
                config.TableSize = SnookerTableSizeEnum.Table10Ft;
            config.TableDescription = this.entryTableDescription.Text ?? "";
            config.NotableBreakThreshold = this.notableBreakThresholds[this.pickerNotableBreakThreshold.SelectedIndex];
            config.SaveToKeyChain(App.KeyChain);

            await App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        private async void buttonCancel_Clicked(object sender, EventArgs e)
        {
            await App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        void fill()
        {
            this.config = FVOConfig.LoadFromKeyChain(App.KeyChain);
            Athlete admin = App.Repository.GetMyAthlete();

            if (string.IsNullOrEmpty(config.VenueName) == false)
                this.labelVenue.Text = config.VenueName;
            else
                this.labelVenue.Text = "Pick venue";

            this.pickerTableType.SelectedIndex = config.TableSize == SnookerTableSizeEnum.Table10Ft ? 0 : 1;
            this.entryTableDescription.Text = config.TableDescription ?? "";

            int index = notableBreakThresholds.IndexOf(config.NotableBreakThreshold);
            if (index < 0)
                index = 0;
            this.pickerNotableBreakThreshold.SelectedIndex = index;

            this.labelAdmin.Text = admin.Name ?? "no name";
        }
	}
}
