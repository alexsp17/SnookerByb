using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class BybButtonWithImage : Grid
	{
		Image image;
		BybLabel label;

		public event Action Clicked;

		public bool IsSelected
		{
			get
			{
				return this.label.TextColor == Config.ColorTextOnBackground;
			}
			set
			{
				this.label.TextColor = value ? Config.ColorTextOnBackground : Config.ColorTextOnBackgroundGrayed;
				this.image.Opacity = value ? 1.0 : 0.5;
			}
		}
		
		public BybButtonWithImage (string imageName, string buttonText)
		{
			this.image = new Image ()
			{
				WidthRequest = 17,
				HeightRequest = 17,
				Source = new FileImageSource() { File = imageName },
				HorizontalOptions = LayoutOptions.Center,
			};
			this.label = new BybLabel ()
			{
				Text = buttonText,
				HorizontalOptions = LayoutOptions.Center,
				//VerticalTextAlignment = TextAlignment.Center,
				//HeightRequest = 30,
				TextColor = Config.ColorTextOnBackground,
			};

			this.HorizontalOptions = LayoutOptions.Fill;
			this.VerticalOptions = LayoutOptions.Fill;
			this.Padding = new Thickness (0);
			this.ColumnSpacing = 0;
			this.RowSpacing = 0;
			this.BackgroundColor = Config.ColorBlackBackground;

			this.Children.Add (new StackLayout ()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Spacing = 5,
				Children = 
				{
					this.image,
					this.label,
				}
			});

			this.GestureRecognizers.Add (new TapGestureRecognizer () {
				Command = new Command(() =>
				{
					if (this.Clicked != null)
						this.Clicked();
				})
			});
		}
	}
}

