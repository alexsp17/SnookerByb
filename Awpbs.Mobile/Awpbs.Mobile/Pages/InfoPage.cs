using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class InfoPage : ContentPage
	{
		public string Text
		{
			get { return this.label2.Text; }
			set { this.label2.Text = value; }
		}

		Label label1;
		Label label2;
		
		public InfoPage ()
		{
            Title = "Internal Info";

			string label1Text = "Mode: " + (Config.IsProduction ? "production" : "debug") + "\r\n";
			label1Text += "Tablet: " + (Config.IsTablet ? "tablet" : "phone") + "\r\n";
            label1Text += "OSVersion: " + Config.OSVersion + "  (major= " + Config.OSVersionMajor.ToString() + ")\r\n";
			label1Text += "Version: " + SnookerBybMobileVersions.Current.ToString() + "\r\n";
			label1Text += "Web api: " + App.WebService.WebApiUrl;
			this.label1 = new BybLabel () {
				Text = label1Text,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				TextColor = Color.Black,
			};

			this.label2 = new BybLabel () {
				Text = "",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				TextColor = Color.Black,
			};

			Button buttonClose = new BybButton () {
                WidthRequest = 100,
				Text = "Close",
				Style = (Style)App.Current.Resources["LargeButtonStyle"],
				HorizontalOptions = LayoutOptions.Center,
			};
			buttonClose.Clicked += (s1, e1) => {
				App.Navigator.NavPage.Navigation.PopModalAsync();
			};

			this.Content = new StackLayout ()
			{
				Orientation = StackOrientation.Vertical,
				BackgroundColor = Config.ColorGrayBackground,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				Spacing = 10,
				Padding = new Thickness(20),
				Children = 
				{
					this.label1,
					this.label2,
					buttonClose,
				}
			};
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
	}
}
