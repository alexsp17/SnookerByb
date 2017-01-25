using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace Awpbs.Mobile
{
    public class SetupExistingUserPage : ContentPage
    {
        private Entry emailEditor;
        private Entry passwordEditor;

        public string EMail
        {
            get
            {
                return this.emailEditor.Text.Trim();
            }
            set
            {
                this.emailEditor.Text = value;
            }
        }

        public string Password
        {
            get
            {
                return this.passwordEditor.Text;
            }
            set
            {
                this.passwordEditor.Text = value;
            }
        }

        public event EventHandler ClickedLogin;

        public SetupExistingUserPage()
        {
			this.BackgroundColor = Color.White;
			
			emailEditor = new BybStandardEntry() { Placeholder = "Your e-mail address", Keyboard = Keyboard.Email };
            emailEditor.Focused  += (s,e) => { this.Content.VerticalOptions = LayoutOptions.Start; };
			emailEditor.Completed += (s, e) => { passwordEditor.Focus(); };

			passwordEditor = new BybStandardEntry() { Placeholder = "Password or PIN", IsPassword = true };
            passwordEditor.Focused += (s, e) => { this.Content.VerticalOptions = LayoutOptions.Start; };

            Button buttonForgotPassword = new BybButton()
            {
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                Text = "Forgot password",
                HorizontalOptions = LayoutOptions.Center
            };
            buttonForgotPassword.Clicked += (s1, e1) =>
            {
                App.Navigator.OpenBrowserApp("https://www.snookerbyb.com/account/forgotpassword");
            };

            if (Config.IsTablet)
            {
                emailEditor.HorizontalOptions = LayoutOptions.Center;
                passwordEditor.HorizontalOptions = LayoutOptions.Center;
                emailEditor.WidthRequest = 350;
                passwordEditor.WidthRequest = 350;
            }

            Button loginButton = new BybButton()
            {
                Text = "Sign in",
                Style = (Style)App.Current.Resources[Config.IsTablet ? "LargeButtonOfSetWidthStyle" : "LargeButtonStyle"]
            };
            Button facebookButton = new BybButton()
            {
                Text = "Sign in with Facebook",
                Style = (Style)App.Current.Resources[Config.IsTablet ? "LargeButtonOfSetWidthStyle" : "LargeButtonStyle"]
            };
            loginButton.Clicked += loginButton_Clicked;
            facebookButton.Clicked += facebookButton_Clicked;

            this.Title = "Existing user";
            this.Content = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                Children = {
					new BybLabel { Text = "With Facebook", FontSize = Config.VeryLargeFontSize, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.Center },
                    facebookButton,
					new BybLabel { Text = "We will never post without your permission", HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite },
                    new BoxView { HeightRequest = 10 },

					new BybLabel { Text = "With E-mail", FontSize = Config.VeryLargeFontSize, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.Center },
                    emailEditor,
                    passwordEditor,
                    loginButton,
                    new BoxView { HeightRequest = 1 },
                    buttonForgotPassword,
                    //new BybLabel { Text = "This will sign you in to " + Config.WebsiteName, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center },
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

        private async void loginButton_Clicked(object sender, EventArgs e)
        {
            string email = emailEditor.Text;
            if (email == null)
                email = "";
            email = email.Trim(); 
            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Byb", "Please enter a proper email", "Cancel");
                return;
            }
            string password = passwordEditor.Text;
            if (password == null)
                password = "";
            if (password.Length < 4 || password.Length > 200)
            {
                await DisplayAlert("Byb", "Please enter a proper PIN or password", "Cancel");
                return;
            }

            this.ClickedLogin(this, EventArgs.Empty);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
