using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class BybTitle : StackLayout
    {
        public string Text
        {
            get { return this.label.Text; }
            set { this.label.Text = value; }
        }

        Label label;

        public BybTitle(string text)
        {
            Orientation = StackOrientation.Horizontal;
            BackgroundColor = Config.ColorBackground;
            VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            HeightRequest = Config.TitleHeight;
            Padding = new Thickness(0, Config.IsIOS ? 25 : 10, 0, 0);

            this.label = new BybLabel
            {
				FontSize = Config.LargerFontSize,
                TextColor = Config.ColorTextOnBackground,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = text
            };

            Children.Add(label);
        }
    }
}
