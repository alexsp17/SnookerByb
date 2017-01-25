using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class SimpleButtonWithRightOrDownArrow : StackLayout
    {
        Label label;
        Image image;
		Frame imageFrame;
		public ActivityIndicator activityIndicator;

        public event EventHandler Clicked;
		public bool isLoading;

        public string Text
        {
            get
            {
                return this.label.Text;
            }
            set
            {
                this.label.Text = value;
            }
        }

        public bool IsArrowVisible
        {
            get
            {
                return this.image.IsVisible;
            }
            set
            {
                this.image.IsVisible = value;
            }
        }

        public bool IsRight
        {
            get
            {
                return isRight;
            }
            set
            {
                this.isRight = value;
                this.image.Rotation = this.isRight ? -90 : 0;
            }
        }
        bool isRight = true;

		//
		// Set activity indicator On/Off
		//   On - while the list of breaks in the frame is loading
		//   Off - when it's done loading
		public void setIsLoading(bool isLoading)
		{
			if (isLoading) 
			{
				this.isLoading = true;

				this.image.IsVisible = false;
				this.imageFrame.IsVisible = false;

				this.activityIndicator.IsEnabled = true;
				this.activityIndicator.IsVisible = true;
				this.activityIndicator.IsRunning = true;
			} 
            else 
            {
				this.isLoading = false;

				this.activityIndicator.IsEnabled = false;
				this.activityIndicator.IsVisible = false;
				this.activityIndicator.IsRunning = false;

				this.image.IsVisible = true;
				this.imageFrame.IsVisible = true;
			}
		}

        public SimpleButtonWithRightOrDownArrow()
        {
			isLoading = false;

            this.Orientation = StackOrientation.Horizontal;
            this.HeightRequest = Config.IsTablet ? 30 : 20;
            this.Spacing = 3;

			this.activityIndicator = new ActivityIndicator () 
			{
				Color = Color.Red,
				IsVisible = false,
				IsEnabled = false,
				IsRunning = false,

                HeightRequest = 10,
                WidthRequest = 10,
			};
			this.Children.Add (activityIndicator);

            this.image = new Image()
            {
                Source = new FileImageSource() { File = "down.png" },
                HeightRequest = 10,
                WidthRequest = 10,
            };
			this.imageFrame = new Frame () {
				BackgroundColor = Color.Transparent,
				Padding = new Thickness(0, 5, 0, 0),
				Content = this.image,
			};
			this.Children.Add (this.imageFrame);

            this.label = new BybLabel()
            {
                TextColor = Color.White,
                FontFamily = Config.FontFamily,
                FontSize = Config.DefaultFontSize,
                FontAttributes = FontAttributes.Bold,
                VerticalTextAlignment = TextAlignment.Center
            };
            this.Children.Add(this.label);

            this.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if (this.Clicked != null)
                        this.Clicked(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
        }
    }
}
