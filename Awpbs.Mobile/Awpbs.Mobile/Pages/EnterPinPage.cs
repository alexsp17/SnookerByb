using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class EnterPinPage : ContentPage
	{
        public event Action UserClickedCancel;
        public event Action UserEnteredPin;

		public bool IsLabelBelowVisible
		{
			get
			{
				return this.labelBelow.IsVisible;
			}
			set
			{
				this.labelBelow.IsVisible = value;
			}
		}

        public string TheTitle
        {
            get
            {
                return this.labelTitle.Text;
            }
            set
            {
                this.labelTitle.Text = value;
            }
        }

        public bool IsPinOk
        {
            get
            {
                return Pin != null;
            }
        }

        public string Pin
        {
            get
            {
                string pin = this.entryPin.Text;
                if (new AccessPinHelper().Validate(pin) == false)
                    return null;
                return pin;
            }
        }

		public void ClearPin()
		{
			this.entryPin.Text = "";
		}

		Button buttonCancel;
        Label labelTitle;
        Entry entryPin;
		Label labelBelow;

		public EnterPinPage(bool isBlack)
        {
            this.labelTitle = new BybLabel()
            {
				FontSize = Config.VeryLargeFontSize,
                Text = "",
				TextColor = isBlack ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite,
				HorizontalOptions = LayoutOptions.Center,
            };

            this.entryPin = new BybPinEntry()
            {
                WidthRequest = 100,
                HeightRequest = 50,
            };
            this.entryPin.TextChanged += entryPin_TextChanged;

			this.labelBelow = new BybLabel ()
			{
				Text = "You will use this to log-in to Byb on any device.",
				TextColor = isBlack ? Config.ColorTextOnBackgroundGrayed : Config.ColorGrayTextOnWhite,
				HorizontalOptions = LayoutOptions.Center,
				IsVisible = false,
			};

            buttonCancel = new BybButton()
            {
                Text = "Cancel",
                Style = (Style)App.Current.Resources["LargeButtonStyle"],
                WidthRequest = 300,
            };
            buttonCancel.Clicked += buttonCancel_Clicked;

            Content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(0,50,0,0),
                Spacing = 20,
                Children =
                {
                    labelTitle,

                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = 20,
                        Children =
                        {
                            new BybLabel
                            {
                                Text = "4-digit PIN:",
								TextColor = isBlack ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite,
                                VerticalOptions = LayoutOptions.Center,
                            },
                            entryPin,
                        }
                    },

					labelBelow,

                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 0,
                        Children =
                        {
                            buttonCancel,
                        }
                    }
                }
            };
			
			this.BackgroundColor = isBlack ? Config.ColorBackground : Config.ColorBackgroundWhite;
        }

        private void entryPin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsPinOk)
            {
				this.buttonCancel.Focus(); // to hide the keyboard
                if (this.UserEnteredPin != null)
                    this.UserEnteredPin();
            }
        }

        private void buttonCancel_Clicked(object sender, EventArgs e)
        {
            if (this.UserClickedCancel != null)
                this.UserClickedCancel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.entryPin.Focus();
        }

		protected override bool OnBackButtonPressed ()
		{
			Device.BeginInvokeOnMainThread (() =>
			{
				this.buttonCancel_Clicked (this, EventArgs.Empty);
			});
			return true;
			
			//return base.OnBackButtonPressed ();
		}
    }
}
