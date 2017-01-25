using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class SetupNewUserEmailPage : ContentPage
    {
        //private Entry nameEditor;
        private Entry emailEditor;
        //private Entry passwordEditor;
        //private Entry passwordEditor2;

        public string EMail
        {
            get
            {
                return (this.emailEditor.Text ?? "").Trim();
            }
        }

//        public string Password
//        {
//            get
//            {
//                return this.passwordEditor.Text;
//            }
//        }

        //public string Name
        //{
        //    get
        //    {
        //        return (this.nameEditor.Text ?? "").Trim();
        //    }
        //}

        public event EventHandler ClickedRegister;

        public SetupNewUserEmailPage()
        {
			this.BackgroundColor = Color.White;
			
			emailEditor = new BybStandardEntry() { Placeholder = "Your e-mail address", Keyboard = Keyboard.Email };
			//passwordEditor = new BybStandardEntry() { Placeholder = "4-Digit PIN", IsPassword = true, Keyboard.Numeric };
			//passwordEditor2 = new BybStandardEntry() { Placeholder = "Confirm PIN", IsPassword = true, Keyboard.Numeric };
            //nameEditor = new BybStandardEntry() { Placeholder = "Your name", Keyboard = Keyboard.Text, IsVisible = Config.App == MobileAppEnum.SnookerForVenues };

//			passwordEditor.TextChanged += (s1, e1) => {
//				if (passwordEditor.Text.Length >= AccessPinHelper.PinLength)
//					passwordEditor2.Focus();
//			};
//            emailEditor.Completed += (s1, e1) => {
//				passwordEditor.Focus();
//			};
//			passwordEditor.Completed += (s1, e1) => {
//				passwordEditor2.Focus();
//			};
//            passwordEditor2.Completed += (s1, e1) =>
//            {
//            };

            Button okButton = new BybButton()
            {
                Text = "Register",
                Style = (Style)App.Current.Resources[Config.IsTablet ? "LargeButtonOfSetWidthStyle" : "LargeButtonStyle"]
            };
            okButton.Clicked += okButton_Clicked;

            if (Config.IsTablet)
            {
                emailEditor.HorizontalOptions = LayoutOptions.Center;
                //passwordEditor.HorizontalOptions = LayoutOptions.Center;
                //passwordEditor2.HorizontalOptions = LayoutOptions.Center;
                //nameEditor.HorizontalOptions = LayoutOptions.Center;
                emailEditor.WidthRequest = 350;
                //passwordEditor.WidthRequest = 350;
                //passwordEditor2.WidthRequest = 350;
                //nameEditor.WidthRequest = 350;
            }

            this.Content = new StackLayout
            {
                Children = {
                    new BoxView() { HeightRequest = 10 },
					new BybLabel { Text = "Register With E-mail", FontSize = Config.VeryLargeFontSize, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.Center },
                    new BoxView() { HeightRequest = 10 },

                    emailEditor,
                    //passwordEditor,
                    //passwordEditor2,
                    //nameEditor,
                    new BoxView() { HeightRequest = 10 },
                    okButton,
                }
            };

            if (Config.IsTablet)
                this.Padding = new Thickness(120, 0, 120, 0);
            else
                this.Padding = new Thickness(30, 0, 30, 0);
        }

        private async void okButton_Clicked(object sender, EventArgs e)
        {
            string email = emailEditor.Text;
            if (email == null)
                email = "";
            email = email.Trim();
            if (new EmailAddressHelper().Validate(email) == false)
            {
                await DisplayAlert("Byb", "Not a proper email", "OK");
                return;
            }

//			string pin = passwordEditor.Text ?? "";
//			if (new AccessPinHelper ().Validate (pin) == false) {
//				await DisplayAlert ("Byb", "The pin must be 4-digit long");
//				return;
//			}

//            string password = passwordEditor.Text;
//            if (password == null)
//                password = "";
//            if (password.Length < 6)
//            {
//                await DisplayAlert("Byb", "The password must be at least 6 characters long", "OK");
//                return;
//            }
//            if (password.Length > 100)
//            {
//                await DisplayAlert("Byb", "Password is too long", "OK");
//                return;
//            }
//            if (string.Compare(password, passwordEditor2.Text, false) != 0)
//            {
//                await DisplayAlert("Byb", "'Password' and 'Confirm password' are different", "OK");
//                return;
//            }

            //if (this.nameEditor.IsVisible)
            //{
            //    if (this.Name.Length < 4)
            //    {
            //        await DisplayAlert("Byb", "The name must be at least 4 characters long", "OK");
            //        return;
            //    }
            //}

            this.ClickedRegister(this, EventArgs.Empty);
        }

        protected override void OnAppearing()
        {
            this.emailEditor.Focus();

            base.OnAppearing();
        }
    }
}
