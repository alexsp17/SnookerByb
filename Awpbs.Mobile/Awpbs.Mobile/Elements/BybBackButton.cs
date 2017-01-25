using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Awpbs.Mobile;

namespace Awpbs.Mobile
{
    public class BybBackButton : StackLayout
    {
		Image image;
		Label label;

		public event EventHandler Clicked;

		public string LabelText
		{
			get { return this.label.Text; }
			set { this.label.Text = value; }
		}
		
        public BybBackButton()
        {
			this.image = new Image()
			{
				Source = new FileImageSource() { File = "thinArrow1Left.png" },
				HeightRequest = Config.IsTablet ? 25 : 20,
				WidthRequest = Config.IsTablet ? 25 : 20,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
			};

			this.label = new BybLabel ()
			{
				Text = "",
				TextColor = Color.White,
				FontFamily = Config.FontFamily,
				FontSize = Config.LargerFontSize,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center,
				IsVisible = Config.App == MobileAppEnum.SnookerForVenues,
			};

			this.Orientation = StackOrientation.Horizontal;
			this.Padding = new Thickness (Config.IsTablet ? 20 : 5, 0, 0, 0);
			this.BackgroundColor = Config.ColorBlackBackground;

			this.Children.Add(this.image);
			this.Children.Add(this.label);

			this.GestureRecognizers.Add (new TapGestureRecognizer () { Command = new Command(() =>
				{
					if (this.Clicked != null)
						this.Clicked(this, EventArgs.Empty);
				})});
        }
    }
}

