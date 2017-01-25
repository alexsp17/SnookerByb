using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ErrorPage : ContentPage
	{
        public ErrorPage(string errorText)
        {
            Content = new StackLayout
            {
                Children = {
					new BybLabel { Text = "Error", FontSize = Config.VeryLargeFontSize, TextColor = Config.ColorBlackTextOnWhite },
					new BybLabel { Text = errorText, TextColor = Config.ColorBlackTextOnWhite }
                }
            };
        }
	}
}
