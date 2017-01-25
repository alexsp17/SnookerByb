using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    //public class BybButtonWithNumber : Frame
	public class BybButtonWithNumber : StackLayout
    {
        Label labelText;
        Label labelNumber;
        BoxView boxView;

        public string Text
        {
            get
            {
                return this.labelText.Text;
            }
            set
            {
                this.labelText.Text = value;
            }
        }

        public int? Number
        {
            get
            {
                int number;
                if (!int.TryParse(labelNumber.Text, out number))
                    return null;
                return number;
            }
            set
            {
                if (value == null)
                    labelNumber.Text = "";
                else
                    labelNumber.Text = value.ToString();
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.boxView.BackgroundColor == Config.ColorBackground;
            }
            set
            {
                this.labelText.FontAttributes = value ? FontAttributes.None : FontAttributes.None;
                this.boxView.BackgroundColor = value ? Config.ColorBackground : Color.Transparent;

                if (Config.App == MobileAppEnum.SnookerForVenues)
                {
                    if (value == true)
                    {
                        this.BackgroundColor = Color.White;
                        this.labelText.TextColor = Color.Black;
                    }
                    else
                    {
                        this.BackgroundColor = Config.ColorBackground;
                        this.labelText.TextColor = Color.White;
                    }
                }
            }
        }

        public bool IsNumberVisible
        {
            get
            {
                return this.labelNumber.IsVisible;
            }
            set
            {
                this.labelNumber.HeightRequest = 20;
                if (value == true)
                {
                    this.labelNumber.IsVisible = true;
                    this.labelText.HeightRequest = 20;
                    this.HeightRequest = 50;
                }
                else
                {
                    this.labelNumber.IsVisible = false;
                    this.labelText.HeightRequest = Config.IsTablet ? 50 : 40;
                    this.HeightRequest = 50;
                }
            }
        }

        public event EventHandler Clicked;

        public BybButtonWithNumber(string text)
        {
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            //this.HasShadow = false;
            this.Padding = new Thickness(0);
            this.BackgroundColor = Color.White;

            this.labelText = new BybLabel()
            {
				HeightRequest = Config.IsTablet ? 40 : 20,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = Config.DefaultFontSize,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Config.ColorBlackTextOnWhite,
                Text = text
            };

            this.labelNumber = new BybLabel()
            {
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = Config.DefaultFontSize,
                VerticalTextAlignment = TextAlignment.Center,
				TextColor = Config.ColorBlackTextOnWhite,
                Text = "-"
            };

            this.boxView = new BoxView()
            {
                HeightRequest = 5,
                HorizontalOptions = LayoutOptions.Fill,
            };
				
			//this.Content = new StackLayout
			//{
			this.Orientation = StackOrientation.Vertical;
			this.Padding = new Thickness (0, 0, 0, 0);
			this.Spacing = 0;

			//this.chil
			this.Children.Add(new BoxView() { HeightRequest = 5 });
			this.Children.Add(this.labelNumber);
			this.Children.Add (this.labelText);
			this.Children.Add (new BoxView () { HeightRequest = 5 });
			this.Children.Add(this.boxView);
			//};
			//};

            //this.labelNumber.Clicked += (s, e) => { this.onClicked(); };
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    onClicked();
                }),
                NumberOfTapsRequired = 1
            });

            this.IsNumberVisible = true;
        }

        void onClicked()
        {
            if (this.Clicked != null)
                this.Clicked(this, EventArgs.Empty);
        }
    }
}
