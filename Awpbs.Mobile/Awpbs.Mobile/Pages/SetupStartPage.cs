using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class SetupStartPage : ContentPage
    {
        public SetupStartPage()
        {
            this.Title = Config.ProductName;
            this.BackgroundColor = Color.White;

            // buttons
            Button newUserButton = new BybButton()
            {
                Text = "New user",
                Style = (Style)App.Current.Resources[Config.IsTablet ? "LargeButtonOfSetWidthStyle" : "LargeButtonStyle"],
            };
            Button existingUserButton = new BybButton()
            {
                Text = "Existing user",
                Style = (Style)App.Current.Resources[Config.IsTablet ? "LargeButtonOfSetWidthStyle" : "LargeButtonStyle"]
            };
            newUserButton.Clicked += newUserButton_Clicked;
            existingUserButton.Clicked += existingUserButton_Clicked;

            // title
            string header = "Hello";
            if (Config.App == MobileAppEnum.Snooker)
                header = "Hello, Fellow Snooker Player!";
            else if (Config.App == MobileAppEnum.SnookerForVenues)
                header = "Hello, Snooker Venue Owner!";

            // text
            List<string> strings = new List<string>();
            if (Config.App == MobileAppEnum.Snooker)
            {
                strings = new List<string>
                {
                    "Keep track of your scores & breaks",
                    "Connect with other players",
                    "Find places to play",
					"Grow the snooker community!",
                };
            }
            else if (Config.App == MobileAppEnum.SnookerForVenues)
            {
                strings = new List<string>()
                {
                    "Provide your players with a tablet-based scoreboard",
                    "Enable players to track their scores & breaks",
                    "Simplify tournaments and rating (coming soon)",
                    "Help players connect with each other",
                    "Grow the snooker community!",
                };
            }
            StackLayout stack = new StackLayout();
            foreach (var str in strings)
            {
                FontAttributes fontAttr = FontAttributes.None;
                if (Config.App == MobileAppEnum.Snooker && str == strings.Last())
                    fontAttr = FontAttributes.Bold;
                stack.Children.Add(new BybLabel
                {
                    Text = str,
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = fontAttr,
					TextColor = Config.ColorBlackTextOnWhite,
                });
            }
            if (Config.App == MobileAppEnum.SnookerForVenues)
            {
                stack.Children.Add(new BoxView() { HeightRequest = 20, BackgroundColor = Color.Transparent, });
                stack.Children.Add(new BybLabel
                {
                    Text = "This app is for snooker venue owners. For personal use - download 'Snooker Byb'.",
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
					TextColor = Config.ColorBlackTextOnWhite,
                });
                stack.Children.Add(new BybLabel
                {
                    Text = "If you already have a 'Snooker Byb' account - just use it to login.",
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
					TextColor = Config.ColorBlackTextOnWhite,
                });
            }

            this.Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
					new BybLabel { Text = header, FontSize = Config.VeryLargeFontSize, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center },
                    stack,
                    new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"]},
                    newUserButton,
                    existingUserButton
                }
            };

            if (Config.IsTablet)
                this.Padding = new Thickness(120, 0, 120, 0);
            else
                this.Padding = new Thickness(30, 0, 30, 0);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void newUserButton_Clicked(object sender, EventArgs e)
        {
            App.LoginAndRegistrationLogic.StartSetupNewUser();
        }

        private void existingUserButton_Clicked(object sender, EventArgs e)
        {
            App.LoginAndRegistrationLogic.StartSetupExistingUser();
        }
    }

}
