using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class BybPersonImage : Grid
    {
        public BackgroundEnum Background { get; set; }

        public bool UseNameAbbreviationIfNoPicture { get; set; }

        RoundedBoxView.Forms.Plugin.Abstractions.RoundedBoxView box;
        Grid layout;
		Image image;

        public BybPersonImage()
        {
            this.Padding = new Thickness(0);
			//this.Orientation = StackOrientation.Vertical;
            //this.HasShadow = false;
			this.UseNameAbbreviationIfNoPicture = true;
        }

        public void SetImagePickOpponent()
        {
            this.box = null;

            this.image = new Image()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Source = new FileImageSource() { File = this.Background == BackgroundEnum.Black ? "plusBlack2.png" : "plus.png" }
            };
			this.Children.Clear();
			this.Children.Add (image);
            //this.Content = image;
        }

        public void SetImage(string personName, string picture)
        {
            this.box = null;
			this.image = null;

            if (string.IsNullOrEmpty(picture) && UseNameAbbreviationIfNoPicture)
            {
                this.box = new RoundedBoxView.Forms.Plugin.Abstractions.RoundedBoxView();
                box.WidthRequest = 60;
                box.HeightRequest = 60;
                box.CornerRadius = 30;
                box.BorderThickness = 1;
                box.BorderColor = Color.White;
                box.BackgroundColor = Config.ColorGrayBackground;
                box.HorizontalOptions = LayoutOptions.Center;
                box.VerticalOptions = LayoutOptions.Center;

                Label label = new BybLabel()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Text = new NameAbbreviationHelper().GetAbbreviation(personName),
                    FontSize = Config.VeryLargeFontSize,
					TextColor = Config.ColorBlackTextOnWhite,
                };

                layout = new Grid()
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                };
                layout.Children.Add(box);
                layout.Children.Add(label);

				this.Children.Clear();
				this.Children.Add (layout);
                //this.Content = this.layout;
            }
            else
            {
                this.image = new Image()
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Source = App.ImagesService.GetImageSource(picture, this.Background, false),
                };

				this.Children.Clear();
				this.Children.Add(image);
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
			double size = System.Math.Min(width, height);

			if (layout != null && box != null && size > 0 && box.WidthRequest != size)
            {
                box.WidthRequest = size;
                box.HeightRequest = size;
                box.CornerRadius = size / 2;
            }

			if (image != null && size > 0 && image.WidthRequest != size)
			{
				image.WidthRequest = size;
				image.HeightRequest = size;
			}

            base.OnSizeAllocated(width, height);
        }
    }
}
