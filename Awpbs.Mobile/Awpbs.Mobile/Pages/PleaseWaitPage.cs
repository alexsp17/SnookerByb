using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class PleaseWaitPage : ContentPage
	{
        private Label labelHeader;
        private Label labelStatus;
        private Button buttonCancel;

        public event EventHandler CancelButtonClicked;

        public PleaseWaitPage()
        {
			this.BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackground : Config.ColorGrayBackground;

			this.labelHeader = new BybLabel { Text = "", FontSize = Config.LargerFontSize, TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite };
            this.labelStatus = new BybLabel() { Text = "", HorizontalOptions = LayoutOptions.Center, TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite };
            this.buttonCancel = new BybButton() { Text = "Cancel", IsVisible = false, Style = (Style)App.Current.Resources["SimpleButtonStyle"], TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite };
            this.buttonCancel.Clicked += (s1, e1) =>
            {
                if (this.CancelButtonClicked != null)
                    this.CancelButtonClicked(this, EventArgs.Empty);
            };

            Content = new Grid
            {
                VerticalOptions = LayoutOptions.Center,
                Children = 
				{
					new StackLayout()
	                {
	                    Children = 
	                    {
	                        this.labelHeader,
	                        new BybLabel { Text = "Please wait...", HorizontalOptions = LayoutOptions.Center, TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite },
	                        this.labelStatus,
	                        new BoxView() { HeightRequest = 20 },
	                        buttonCancel,
	                    }
	                }
				}
            };
        }

        public string Header
        {
            get { return this.labelHeader.Text; }
            set { this.labelHeader.Text = value; }
        }

        public string StatusText
        {
            get { return this.labelStatus.Text; }
            set { this.labelStatus.Text = value; }
        }

        public bool ShowCancelButton
        {
            get { return this.buttonCancel.IsVisible; }
            set { this.buttonCancel.IsVisible = value; }
        }
	}
}
