using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace Awpbs.Mobile
{
    public class SetupNewUserPage : ContentPage
    {
        public SetupNewUserPage()
        {
			this.BackgroundColor = Color.White;
			
            Button facebookButton = new BybButton()
            {
                Text = "Connect with Facebook",
                Style = (Style)App.Current.Resources[Config.IsTablet ? "LargeButtonOfSetWidthStyle" : "LargeButtonStyle"]
            };
            Button emailButton = new BybButton()
            {
                Text = "Register with your e-mail",
                Style = (Style)App.Current.Resources[Config.IsTablet ? "LargeButtonOfSetWidthStyle" : "LargeButtonStyle"]
            };
            facebookButton.Clicked += facebookButton_Clicked;
            emailButton.Clicked += loginButton_Clicked;

            this.Title = "New User";
            this.Content = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Children = {
							new BybLabel { Text = "Option 1", FontSize = Config.VeryLargeFontSize, TextColor = Config.ColorBlackTextOnWhite },
                        }
                    },
                    facebookButton,
					new BybLabel { Text = "We will never post without your permission", HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite },
                    new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"]},

                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Children = {
							new BybLabel { Text = "Option 2", FontSize = Config.VeryLargeFontSize, TextColor = Config.ColorBlackTextOnWhite },
                        }
                    },
                    emailButton,
                    new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"]},
                }
            };

            if (Config.IsTablet)
                this.Padding = new Thickness(120, 0, 120, 0);
            else
                this.Padding = new Thickness(30, 0, 30, 0);
        }

        void facebookButton_Clicked(object sender, EventArgs e)
        {
            App.LoginAndRegistrationLogic.StartLoginFromFacebook();
        }

        private void loginButton_Clicked(object sender, EventArgs e)
        {
            App.LoginAndRegistrationLogic.StartSetupNewUserRegisterWithEmail();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
