using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class UpgradeRequiredPage : ContentPage
	{
        private Label labelHeader;
        private Label labelStatus;

        public UpgradeRequiredPage()
        {
			this.BackgroundColor = Config.ColorGrayBackground;
			
			this.labelHeader = new BybLabel { Text = "", TextColor = Config.ColorBlackTextOnWhite, FontSize = Config.LargerFontSize };
			this.labelStatus = new BybLabel() { Text = "", TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };

            Content = new Frame
            {
                VerticalOptions = LayoutOptions.Center,
                Content = new StackLayout()
                {
                    Spacing = 20,
                    Children = 
                    {
                        this.labelHeader,
                        this.labelStatus
                    }
                }
            };

            this.Header = "Snooker Byb - Upgrade Required";
            this.StatusText = "This version of the application is no longer supported. Please update the app from the " + Config.AppStoreName;
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
	}
}
